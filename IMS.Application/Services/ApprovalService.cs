using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApprovalLevel = IMS.Domain.Entities.ApprovalLevel;

namespace IMS.Application.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ApprovalService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApprovalService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            UserManager<User> userManager,
            ILogger<ApprovalService> logger,
            IHttpContextAccessor httpContextAccessor)  // Add this
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;  // Add this
        }

        public async Task<ApprovalRequest> CreateApprovalRequestAsync(ApprovalRequestDto dto)
        {
            try
            {
                // ✅ FIX: Removed BeginTransactionAsync - participates in caller's transaction

                // Get approval workflow for the entity type
                var workflow = await GetApprovalWorkflowAsync(dto.EntityType, dto.Amount);

                var approvalRequest = new ApprovalRequest
                {
                    EntityType = dto.EntityType,
                    EntityId = dto.EntityId,
                    RequestedBy = dto.RequestedBy,
                    RequestedDate = dto.RequestedDate,
                    RequestDate = dto.RequestedDate,
                    Description = dto.Description,
                    Amount = dto.Amount ?? 0m,
                    Priority = dto.Priority ?? "Normal",
                    Status = ApprovalStatus.Pending.ToString(),
                    CurrentLevel = 1,
                    MaxLevel = workflow.RequiredLevels,
                    WorkflowId = workflow.Id,
                    ExpiryDate = CalculateExpiryDate(dto.Priority),
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.RequestedBy,
                    IsActive = true  // <-- Make sure this is set to true, not null
                };

                await _unitOfWork.ApprovalRequests.AddAsync(approvalRequest);
                await _unitOfWork.CompleteAsync(); // Save to get the ID

                // Create approval steps based on workflow
                var approvalSteps = new List<ApprovalStep>();
                foreach (var level in workflow.Levels)
                {
                    var step = new ApprovalStep
                    {
                        ApprovalRequestId = approvalRequest.Id,
                        StepLevel = level.Level,
                        ApproverRole = level.ApproverRole,
                        Status = level.Level == 1 ? ApprovalStatus.Pending : ApprovalStatus.Awaiting,
                        CreatedAt = DateTime.Now
                    };

                    // Assign specific approver if configured
                    if (!string.IsNullOrEmpty(level.SpecificApproverId))
                    {
                        step.AssignedTo = level.SpecificApproverId;
                    }

                    approvalSteps.Add(step);
                }

                await _unitOfWork.ApprovalSteps.AddRangeAsync(approvalSteps);
                await _unitOfWork.CompleteAsync();

                // Send notification to first level approvers
                await NotifyApproversAsync(approvalRequest, 1);

                // ✅ FIX: Removed CommitTransactionAsync - caller will commit

                _logger.LogInformation($"Approval request created for {dto.EntityType} {dto.EntityId}");
                return approvalRequest;
            }
            catch (Exception ex)
            {
                // ✅ FIX: Removed RollbackTransactionAsync - caller will rollback if needed
                _logger.LogError(ex, "Error creating approval request");
                throw;
            }
        }

        // Approve Request
        public async Task<bool> ApproveRequestAsync(ApprovalActionDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get the approval request
                var request = await _unitOfWork.ApprovalRequests
                    .FirstOrDefaultAsync(ar => ar.EntityType == dto.EntityType &&
                                               ar.EntityId == dto.EntityId &&
                                               ar.Status == "Pending");

                if (request == null)
                    throw new InvalidOperationException("Approval request not found or not pending");

                // Update the approval request
                request.Status = "Approved";
                request.ApprovedBy = dto.ApprovedBy;
                request.ApprovedDate = dto.ApprovedDate;
                request.CompletedDate = DateTime.Now;
                request.Remarks = dto.Remarks;

                _unitOfWork.ApprovalRequests.Update(request);

                // Create approval history
                var history = new ApprovalHistory
                {
                    ApprovalRequestId = request.Id,
                    Action = "Approved",
                    ActionBy = dto.ApprovedBy,
                    ActionDate = dto.ApprovedDate,
                    Comments = dto.Remarks,
                    EntityType = request.EntityType,
                    EntityId = request.EntityId,
                    PreviousStatus = "Pending",
                    NewStatus = "Approved",
                    Level = 1,  // Single level approval for now
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.ApprovalHistories.AddAsync(history);

                // Update entity status
                await UpdateEntityStatusAsync(request.EntityType, request.EntityId, "Approved");

                // ✅ CRITICAL FIX: Complete transaction BEFORE sending notification
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"✅ Approved {request.EntityType} {request.EntityId} by user {dto.ApprovedBy}");

                // ✅ Send notification AFTER transaction commits (so approval succeeds even if notification fails)
                await SendApprovalCompletionNotificationAsync(request);

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error approving request for {dto.EntityType} {dto.EntityId}");
                throw;
            }
        }
        public async Task<bool> RejectRequestAsync(ApprovalActionDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get the approval request
                var request = await _unitOfWork.ApprovalRequests
                    .FirstOrDefaultAsync(ar => ar.EntityType == dto.EntityType &&
                                               ar.EntityId == dto.EntityId &&
                                               ar.Status == "Pending");

                if (request == null)
                    throw new InvalidOperationException("Approval request not found or not pending");

                // Update the approval request
                request.Status = "Rejected";
                request.RejectedBy = dto.ApprovedBy;  // Using ApprovedBy field for the rejector
                request.RejectedDate = dto.ApprovedDate;  // Using ApprovedDate for rejection date
                request.RejectionReason = dto.Remarks;
                request.CompletedDate = DateTime.Now;

                _unitOfWork.ApprovalRequests.Update(request);

                // Create approval history
                var history = new ApprovalHistory
                {
                    ApprovalRequestId = request.Id,
                    Action = "Rejected",
                    ActionBy = dto.ApprovedBy,
                    ActionDate = dto.ApprovedDate,
                    Comments = dto.Remarks,
                    EntityType = request.EntityType,
                    EntityId = request.EntityId,
                    PreviousStatus = "Pending",
                    NewStatus = "Rejected",
                    Level = 1,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.ApprovalHistories.AddAsync(history);

                // Update entity status
                await UpdateEntityStatusAsync(request.EntityType, request.EntityId, "Rejected");

                // Send rejection notification
                await SendRejectionNotificationAsync(request);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Rejected {request.EntityType} {request.EntityId} by user {dto.ApprovedBy}");

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error rejecting request for {dto.EntityType} {dto.EntityId}");
                throw;
            }
        }

        // Get Pending Approvals for User
        public async Task<IEnumerable<PendingApprovalDto>> GetPendingApprovalsAsync(string userId)
        {
            try
            {
                // FIX: Use UserManager to get roles
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return new List<PendingApprovalDto>();

                var userRoles = await _userManager.GetRolesAsync(user);

                // Get pending approvals for user's roles
                var pendingApprovals = await _unitOfWork.ApprovalSteps.Query()
                    .Include(s => s.ApprovalRequest)
                    .Where(s => s.Status == ApprovalStatus.Pending &&
                               (s.AssignedTo == userId || userRoles.Contains(s.ApproverRole)))
                    .Select(s => new PendingApprovalDto
                    {
                        ApprovalRequestId = s.ApprovalRequestId,
                        EntityType = s.ApprovalRequest.EntityType,
                        EntityId = s.ApprovalRequest.EntityId,
                        Description = s.ApprovalRequest.Description,
                        Amount = s.ApprovalRequest.Amount,
                        RequestedBy = s.ApprovalRequest.RequestedBy,
                        RequestedDate = s.ApprovalRequest.RequestedDate,
                        Priority = s.ApprovalRequest.Priority,
                        Level = s.StepLevel,
                        ExpiryDate = s.ApprovalRequest.ExpiryDate,
                        IsEscalated = s.IsEscalated
                    })
                    .OrderBy(p => p.Priority == "Critical" ? 0 : p.Priority == "High" ? 1 : 2)
                    .ThenBy(p => p.RequestedDate)
                    .ToListAsync();

                return pendingApprovals;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting pending approvals for user {userId}");
                throw;
            }
        }
        private async Task<bool> CanApproveAsync(string userId, string requiredRole)
        {
            // FIX: Use UserManager to check roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var userRoles = await _userManager.GetRolesAsync(user);

            // Check direct role
            if (userRoles.Contains(requiredRole))
                return true;

            // Check delegation
            var delegation = await _unitOfWork.ApprovalDelegations
                .FirstOrDefaultAsync(d => d.ToUserId == userId &&
                                          d.IsActive &&
                                          d.StartDate <= DateTime.Now &&
                                          d.EndDate >= DateTime.Now);

            if (delegation != null)
            {
                var delegator = await _userManager.FindByIdAsync(delegation.FromUserId);
                if (delegator != null)
                {
                    var delegatorRoles = await _userManager.GetRolesAsync(delegator);
                    return delegatorRoles.Contains(requiredRole);
                }
            }

            return false;
        }
        public async Task<ApprovalThresholdDto> GetApprovalRequirementAsync(string entityType, decimal value)
        {
            var thresholds = await _unitOfWork.ApprovalThresholds
                .GetAllAsync(t => t.EntityType == entityType &&
                            t.IsActive &&
                            t.MinAmount <= value &&
                            (t.MaxAmount == null || t.MaxAmount >= value));

            var threshold = thresholds.OrderByDescending(t => t.MinAmount).FirstOrDefault();

            if (threshold == null) return null;

            return new ApprovalThresholdDto
            {
                Id = threshold.Id,
                EntityType = threshold.EntityType,
                MinAmount = threshold.MinAmount,
                MaxAmount = threshold.MaxAmount,
                ApprovalLevel = threshold.ApprovalLevel,
                RequiredRole = threshold.RequiredRole,
                ApproverRole = threshold.RequiredRole,
                IsActive = threshold.IsActive,
                RequiresApproval = true,  // If we found a threshold, approval is required
                Description = threshold.Description
            };
        }
        public async Task<bool> ValidateApprovalAsync(string userId, string entityType, int entityId, decimal value)
        {
            var requirement = await GetApprovalRequirementAsync(entityType, value);
            if (requirement == null) return true; // No approval required

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.Contains(requirement.RequiredRole);
        }
        public async Task<bool> CanUserApproveAsync(string userId, string entityType, decimal amount)
        {
            var requirement = await GetApprovalRequirementAsync(entityType, amount);
            if (requirement == null) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.Contains(requirement.RequiredRole);
        }
        public async Task<bool> CanUserApproveAsync(string userId, int approvalLevel)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var userRoles = await _userManager.GetRolesAsync(user);

            // Map approval levels to roles based on Ansar & VDP hierarchy
            var requiredRoles = GetRequiredRolesForLevel(approvalLevel);

            return userRoles.Any(role => requiredRoles.Contains(role));
        }
        public async Task<bool> EscalateRequestAsync(int requestId, string escalatedBy, string reason)
        {
            try
            {
                var request = await _unitOfWork.ApprovalRequests.Query()
                    .Include(ar => ar.Steps)
                    .FirstOrDefaultAsync(ar => ar.Id == requestId);

                if (request == null)
                    throw new InvalidOperationException("Approval request not found");

                // Skip current level and move to next
                var currentStep = request.Steps
                    .FirstOrDefault(s => s.StepLevel == request.CurrentLevel);

                if (currentStep != null)
                {
                    currentStep.Status = ApprovalStatus.Escalated;
                    currentStep.EscalatedBy = escalatedBy;
                    currentStep.EscalatedDate = DateTime.Now;
                    currentStep.EscalationReason = reason;
                }

                // Move to next level or highest level
                if (request.CurrentLevel < request.MaxLevel)
                {
                    request.CurrentLevel = request.MaxLevel; // Escalate to highest level

                    var highestStep = request.Steps
                        .FirstOrDefault(s => s.StepLevel == request.MaxLevel);

                    if (highestStep != null)
                    {
                        highestStep.Status = ApprovalStatus.Pending;
                        highestStep.IsEscalated = true;
                    }
                }

                request.Status = ApprovalStatus.Escalated.ToString();
                request.IsEscalated = true;
                request.EscalatedDate = DateTime.Now;

                // Create history
                var history = new ApprovalHistory
                {
                    ApprovalRequestId = request.Id,
                    Action = "Escalated",
                    ActionBy = escalatedBy,
                    ActionDate = DateTime.Now,
                    Level = request.CurrentLevel,
                    Comments = reason
                };

                await _unitOfWork.ApprovalHistories.AddAsync(history);
                await _unitOfWork.CompleteAsync();

                // Notify highest level approvers
                await NotifyEscalationAsync(request, reason);

                _logger.LogInformation($"Request escalated for {request.EntityType} {request.EntityId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error escalating request");
                throw;
            }
        }
        public async Task<IEnumerable<ApprovalHistoryDto>> GetApprovalHistoryAsync(
            string entityType, int entityId)
        {
            try
            {
                var request = await _unitOfWork.ApprovalRequests
                    .FirstOrDefaultAsync(ar => ar.EntityType == entityType && ar.EntityId == entityId);

                if (request == null) return new List<ApprovalHistoryDto>();

                var history = await _unitOfWork.ApprovalHistories.Query()
                    .Where(h => h.ApprovalRequestId == request.Id)
                    .OrderBy(h => h.ActionDate)
                    .Select(h => new ApprovalHistoryDto
                    {
                        Action = h.Action,
                        ActionBy = h.ActionBy,
                        ActionDate = h.ActionDate,
                        Level = h.Level,
                        Comments = h.Comments,
                        PreviousStatus = h.PreviousStatus,
                        NewStatus = h.NewStatus
                    })
                    .ToListAsync();

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting approval history for {entityType} {entityId}");
                throw;
            }
        }
        public async Task<bool> DelegateApprovalAsync(DelegationDto dto)
        {
            try
            {
                var delegation = new ApprovalDelegation
                {
                    FromUserId = dto.FromUserId,
                    ToUserId = dto.ToUserId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Reason = dto.Reason,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.ApprovalDelegations.AddAsync(delegation);
                await _unitOfWork.CompleteAsync();

                // Notify delegated user
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = dto.ToUserId,
                    Title = "Approval Authority Delegated",
                    Message = $"You have been delegated approval authority from {dto.FromUserId} " +
                             $"from {dto.StartDate:dd/MM/yyyy} to {dto.EndDate:dd/MM/yyyy}",
                    Type = "Delegation",
                    Priority = "High"
                });

                _logger.LogInformation($"Approval delegated from {dto.FromUserId} to {dto.ToUserId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delegating approval");
                throw;
            }
        }
        public async Task<ApprovalWorkflowDto> SetupWorkflowAsync(ApprovalWorkflowDto dto)
        {
            try
            {
                var workflow = new ApprovalWorkflow
                {
                    Name = dto.Name,
                    EntityType = dto.EntityType,
                    MinAmount = dto.MinAmount,
                    MaxAmount = dto.MaxAmount,
                    RequiredLevels = dto.Levels.Count,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    Levels = new List<ApprovalWorkflowLevel>()
                };

                foreach (var levelDto in dto.Levels)
                {
                    workflow.Levels.Add(new ApprovalWorkflowLevel
                    {
                        Level = levelDto.Level,
                        ApproverRole = levelDto.ApproverRole,
                        SpecificApproverId = levelDto.SpecificApproverId,
                        ThresholdAmount = levelDto.ThresholdAmount,
                        CanEscalate = levelDto.CanEscalate,
                        TimeoutHours = levelDto.TimeoutHours
                    });
                }

                await _unitOfWork.ApprovalWorkflows.AddAsync(workflow);
                await _unitOfWork.CompleteAsync();

                dto.Id = workflow.Id;
                _logger.LogInformation($"Approval workflow created: {workflow.Name}");

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up workflow");
                throw;
            }
        }
        public async Task<List<ApprovalStep>> GetCompletedApprovalsAsync(string entityType, int entityId)
        {
            var request = await _unitOfWork.ApprovalRequests.Query()
                .Include(ar => ar.Steps)
                .FirstOrDefaultAsync(ar => ar.EntityType == entityType && ar.EntityId == entityId);

            if (request == null) return new List<ApprovalStep>();

            return request.Steps
                .Where(s => s.Status == ApprovalStatus.Approved)
                .ToList();
        }
        private async Task<ApprovalWorkflow> GetApprovalWorkflowAsync(string entityType, decimal? amount)
        {
            var workflow = await _unitOfWork.ApprovalWorkflows.Query()
                .Include(w => w.Levels)
                .FirstOrDefaultAsync(w => w.EntityType == entityType &&
                                          w.IsActive &&
                                          (!amount.HasValue ||
                                           (w.MinAmount <= amount &&
                                            (w.MaxAmount == null || w.MaxAmount >= amount))));

            if (workflow == null)
            {
                // Return default workflow for any approval role
                return new ApprovalWorkflow
                {
                    Id = 0,
                    Name = "Default",
                    RequiredLevels = 1,
                    Levels = new List<ApprovalWorkflowLevel>
            {
                new ApprovalWorkflowLevel
                {
                    Level = 1,
                    ApproverRole = "Officer" // Default to Officer role or any role that can approve
                }
            }
                };
            }

            return workflow;
        }
        private DateTime CalculateExpiryDate(string priority)
        {
            return priority switch
            {
                "Critical" => DateTime.Now.AddHours(4),
                "High" => DateTime.Now.AddDays(1),
                "Normal" => DateTime.Now.AddDays(3),
                _ => DateTime.Now.AddDays(7)
            };
        }
        private async Task UpdateEntityStatusAsync(string entityType, int entityId, string status)
        {
            try
            {
                // Convert to uppercase to match database values
                switch (entityType?.ToUpper())
                {
                    case "PURCHASE":
                        var purchase = await _unitOfWork.Purchases.GetByIdAsync(entityId);
                        if (purchase != null)
                        {
                            purchase.Status = status == "Approved" ?
                                PurchaseStatus.Approved.ToString() :
                                PurchaseStatus.Rejected.ToString();

                            if (status == "Approved")
                            {
                                purchase.ApprovedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                purchase.ApprovedDate = DateTime.Now;
                            }

                            _unitOfWork.Purchases.Update(purchase);
                        }
                        break;

                    case "ISSUE":
                        var issue = await _unitOfWork.Issues.GetByIdAsync(entityId);
                        if (issue != null)
                        {
                            issue.Status = status == "Approved" ?
                                IssueStatus.Approved.ToString() :
                                IssueStatus.Rejected.ToString();

                            if (status == "Approved")
                            {
                                issue.ApprovedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                issue.ApprovedDate = DateTime.Now;

                                // ✅ CRITICAL FIX: Generate Voucher Number when Issue is approved
                                if (string.IsNullOrEmpty(issue.VoucherNumber))
                                {
                                    issue.VoucherNumber = $"IV-{issue.IssueNo}";
                                    issue.VoucherGeneratedDate = DateTime.Now;
                                    _logger.LogInformation($"✅ Generated Voucher {issue.VoucherNumber} for Issue {issue.IssueNo}");
                                }
                            }
                            else if (status == "Rejected")
                            {
                                issue.RejectedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                issue.RejectedDate = DateTime.Now;
                            }

                            _unitOfWork.Issues.Update(issue);
                            _logger.LogInformation($"Updated Issue {issue.IssueNo} status to {status}");
                        }
                        break;

                    case "TRANSFER":
                        var transfer = await _unitOfWork.Transfers.GetByIdAsync(entityId);
                        if (transfer != null)
                        {
                            transfer.Status = status == "Approved" ?
                                TransferStatus.Approved.ToString() :
                                TransferStatus.Rejected.ToString();

                            if (status == "Approved")
                            {
                                transfer.ApprovedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                transfer.ApprovedDate = DateTime.Now;
                            }

                            _unitOfWork.Transfers.Update(transfer);
                        }
                        break;

                    case "WRITEOFF":
                    case "WRITE_OFF":
                        var writeOff = await _unitOfWork.WriteOffRequests.GetByIdAsync(entityId);
                        if (writeOff != null)
                        {
                            writeOff.Status = status == "Approved" ?
                                WriteOffStatus.Approved :
                                WriteOffStatus.Rejected;

                            if (status == "Approved")
                            {
                                writeOff.ApprovedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                writeOff.ApprovedDate = DateTime.Now;
                            }
                            else if (status == "Rejected")
                            {
                                writeOff.RejectedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                writeOff.RejectedDate = DateTime.Now;
                            }

                            _unitOfWork.WriteOffRequests.Update(writeOff);
                        }
                        break;

                    case "STOCK_ADJUSTMENT":
                    case "STOCKADJUSTMENT":
                        var adjustment = await _unitOfWork.StockAdjustments.GetByIdAsync(entityId);
                        if (adjustment != null)
                        {
                            adjustment.Status = status; // "Approved" or "Rejected"

                            if (status == "Approved")
                            {
                                adjustment.ApprovedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                adjustment.ApprovedDate = DateTime.Now;

                                // Stock adjustment will be applied by StockAdjustmentService when approved
                                // No need to handle it here
                            }
                            else if (status == "Rejected")
                            {
                                adjustment.RejectedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                adjustment.RejectedDate = DateTime.Now;
                            }

                            _unitOfWork.StockAdjustments.Update(adjustment);
                        }
                        break;

                    case "REQUISITION":
                        var requisition = await _unitOfWork.Requisitions.GetByIdAsync(entityId);
                        if (requisition != null)
                        {
                            requisition.Status = status; // "Approved" or "Rejected"

                            if (status == "Approved")
                            {
                                requisition.ApprovedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
                                requisition.ApprovedDate = DateTime.Now;
                            }

                            _unitOfWork.Requisitions.Update(requisition);
                        }
                        break;

                    default:
                        _logger.LogWarning($"Unknown entity type for approval: {entityType}");
                        break;
                }

                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Updated {entityType} {entityId} status to {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating entity status for {entityType} {entityId}");
                throw;
            }
        }
        private async Task NotifyApproversAsync(ApprovalRequest request, int level)
        {
            await _notificationService.CreateApprovalNotificationAsync(request);
        }
        private async Task NotifyEscalationAsync(ApprovalRequest request, string reason)
        {
            // Notify highest level approvers about escalation
            await Task.CompletedTask;
        }
        private async Task SendApprovalCompletionNotificationAsync(ApprovalRequest request)
        {
            try
            {
                // Validate UserId exists before sending notification
                if (string.IsNullOrEmpty(request.RequestedBy))
                {
                    _logger.LogWarning($"Cannot send approval notification for {request.EntityType} {request.EntityId}: RequestedBy is null or empty");
                    return;
                }

                // Check if user exists
                var user = await _userManager.FindByIdAsync(request.RequestedBy);
                if (user == null)
                {
                    _logger.LogWarning($"Cannot send approval notification for {request.EntityType} {request.EntityId}: User {request.RequestedBy} not found");
                    return;
                }

                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = request.RequestedBy,
                    Title = "Request Approved",
                    Message = $"Your {request.EntityType} request has been approved",
                    Type = "Approval",
                    ReferenceType = request.EntityType,
                    ReferenceId = request.EntityId.ToString()
                });
            }
            catch (Exception ex)
            {
                // Log but don't throw - notification failure shouldn't break approval
                _logger.LogError(ex, $"Failed to send approval notification for {request.EntityType} {request.EntityId}");
            }
        }
        private async Task SendRejectionNotificationAsync(ApprovalRequest request)
        {
            try
            {
                // Validate UserId exists before sending notification
                if (string.IsNullOrEmpty(request.RequestedBy))
                {
                    _logger.LogWarning($"Cannot send rejection notification for {request.EntityType} {request.EntityId}: RequestedBy is null or empty");
                    return;
                }

                // Check if user exists
                var user = await _userManager.FindByIdAsync(request.RequestedBy);
                if (user == null)
                {
                    _logger.LogWarning($"Cannot send rejection notification for {request.EntityType} {request.EntityId}: User {request.RequestedBy} not found");
                    return;
                }

                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = request.RequestedBy,
                    Title = "Request Rejected",
                    Message = $"Your {request.EntityType} request has been rejected. Reason: {request.RejectionReason}",
                    Type = "Rejection",
                    ReferenceType = request.EntityType,
                    ReferenceId = request.EntityId.ToString()
                });
            }
            catch (Exception ex)
            {
                // Log but don't throw - notification failure shouldn't break rejection
                _logger.LogError(ex, $"Failed to send rejection notification for {request.EntityType} {request.EntityId}");
            }
        }
        public async Task<string> GetNextApproverAsync(string entityType, decimal value)
        {
            var requirement = await GetApprovalRequirementAsync(entityType, value);
            if (requirement == null) return null;

            // Get users with required role using UserManager
            var usersInRole = await _userManager.GetUsersInRoleAsync(requirement.RequiredRole);

            // Return first available approver
            return usersInRole.FirstOrDefault()?.Id;
        }
        public async Task RecordApprovalAsync(string entityType, int entityId, string approvedBy, string comments)
        {
            var approvalHistory = new ApprovalHistory
            {
                EntityType = entityType,
                EntityId = entityId,
                ApprovedBy = approvedBy,
                ApprovedDate = DateTime.Now,
                Comments = comments,
                Action = "Approved"
            };

            await _unitOfWork.ApprovalHistories.AddAsync(approvalHistory);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Approval recorded for {entityType} #{entityId} by {approvedBy}");
        }
        public async Task<List<ApprovalThresholdDto>> GetApprovalThresholdsAsync(string entityType)
        {
            var thresholds = await _unitOfWork.ApprovalThresholds
                .GetAllAsync(t => t.EntityType == entityType && t.IsActive);

            return thresholds.Select(t => new ApprovalThresholdDto
            {
                Id = t.Id,
                EntityType = t.EntityType,
                MinAmount = t.MinAmount,
                MaxAmount = t.MaxAmount,
                ApprovalLevel = t.ApprovalLevel,
                RequiredRole = t.RequiredRole,
                IsActive = t.IsActive
            }).OrderBy(t => t.MinAmount).ToList();
        }
        public async Task<ServiceResult> ProcessApprovalAsync(int approvalId, string action, string userId, string remarks = null)
        {
            try
            {
                var approval = await _unitOfWork.ApprovalRequests.GetByIdAsync(approvalId);
                if (approval == null)
                    return ServiceResult.Failure("Approval request not found");

                if (approval.Status != ApprovalStatus.Pending.ToString())
                    return ServiceResult.Failure("This approval has already been processed");

                // Get user - handle both string and int IDs
                User user = null;
                if (int.TryParse(userId, out int userIdInt))
                {
                    user = await _unitOfWork.Users.GetByIdAsync(userIdInt);
                }
                else
                {
                    user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == userId);
                }

                if (user == null)
                    return ServiceResult.Failure("User not found");

                switch (action.ToLower())
                {
                    case "approve":
                        approval.Status = ApprovalStatus.Approved.ToString();
                        approval.ApprovedBy = user.FullName;
                        approval.ApprovedDate = DateTime.Now;
                        approval.Remarks = remarks;

                        // Check if multi-level approval is needed
                        var hasMoreLevels = await CheckForNextApprovalLevel(approval);
                        if (hasMoreLevels)
                        {
                            await CreateNextLevelApproval(approval);
                        }
                        else
                        {
                            // Final approval - update the entity status
                            await UpdateEntityStatusAfterApproval(approval);
                        }
                        break;

                    case "reject":
                        approval.Status = ApprovalStatus.Rejected.ToString();
                        approval.RejectedBy = user.FullName;
                        approval.RejectedDate = DateTime.Now;
                        approval.RejectionReason = remarks;

                        // Update entity status to rejected
                        await UpdateEntityStatusAfterRejection(approval);
                        break;

                    default:
                        return ServiceResult.Failure("Invalid action");
                }

                _unitOfWork.ApprovalRequests.Update(approval);
                await _unitOfWork.CompleteAsync();

                // Send notifications
                await SendApprovalNotifications(approval, action);

                return ServiceResult.SuccessResult($"Approval {action}ed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing approval {approvalId}");
                return ServiceResult.Failure("An error occurred while processing the approval");
            }
        }
        public async Task CreateApprovalWorkflowAsync(string entityType, int entityId, List<ApprovalLevel> levels)
        {
            try
            {
                // Clear any existing pending approvals for this entity
                var existingApprovals = await _unitOfWork.ApprovalRequests
                    .GetAllAsync(a => a.EntityType == entityType &&
                                     a.EntityId == entityId &&
                                     a.Status == ApprovalStatus.Pending.ToString());

                foreach (var existing in existingApprovals)
                {
                    _unitOfWork.ApprovalRequests.Remove(existing);
                }

                // Create new approval workflow
                foreach (var level in levels.OrderBy(l => l.Level))
                {
                    var approval = new ApprovalRequest
                    {
                        EntityType = entityType,
                        EntityId = entityId,
                        Level = level.Level,
                        Role = level.Role,
                        Status = ApprovalStatus.Pending.ToString(),
                        RequiredRole = level.Role,
                        Description = level.Description,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System"
                    };

                    await _unitOfWork.ApprovalRequests.AddAsync(approval);
                }

                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Approval workflow created for {entityType} #{entityId} with {levels.Count} levels");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating approval workflow for {entityType} #{entityId}");
                throw;
            }
        }
        public async Task<int> GetCurrentApprovalLevelAsync(string entityType, int entityId)
        {
            var approvals = await _unitOfWork.ApprovalRequests
                .GetAllAsync(a => a.EntityType == entityType && a.EntityId == entityId);

            var pendingApproval = approvals
                .Where(a => a.Status == ApprovalStatus.Pending.ToString())
                .OrderBy(a => a.Level)
                .FirstOrDefault();

            return pendingApproval?.Level ?? 0;
        }
        public async Task ApproveAsync(string entityType, int entityId, string approvedBy, string remarks)
        {
            var currentLevel = await GetCurrentApprovalLevelAsync(entityType, entityId);

            var approval = await _unitOfWork.ApprovalRequests
                .FirstOrDefaultAsync(a => a.EntityType == entityType &&
                                         a.EntityId == entityId &&
                                         a.Level == currentLevel &&
                                         a.Status == ApprovalStatus.Pending.ToString());

            if (approval == null)
                throw new InvalidOperationException("No pending approval found at current level");

            approval.Status = ApprovalStatus.Approved.ToString();
            approval.ApprovedBy = approvedBy;
            approval.ApprovedDate = DateTime.Now;
            approval.Remarks = remarks;

            _unitOfWork.ApprovalRequests.Update(approval);
            await _unitOfWork.CompleteAsync();

            // Record in approval history
            await RecordApprovalAsync(entityType, entityId, approvedBy, remarks);

            // Check if there are more levels
            var nextLevel = await GetNextPendingApproval(entityType, entityId);
            if (nextLevel != null)
            {
                await NotifyNextApprover(nextLevel);
            }
            else
            {
                // All approvals complete
                await FinalizeApproval(entityType, entityId);
            }
        }
        public async Task<bool> AreAllApprovalsCompleteAsync(string entityType, int entityId)
        {
            var approvals = await _unitOfWork.ApprovalRequests
                .GetAllAsync(a => a.EntityType == entityType && a.EntityId == entityId);

            return approvals.All(a => a.Status == ApprovalStatus.Approved.ToString());
        }
        public async Task<List<ApprovalDto>> GetApprovalChainAsync(string entityType, int entityId)
        {
            var approvals = await _unitOfWork.ApprovalRequests
                .GetAllAsync(a => a.EntityType == entityType && a.EntityId == entityId);

            return approvals.OrderBy(a => a.Level).Select(a => new ApprovalDto
            {
                Level = a.Level,
                Role = a.Role,
                RoleDescription = GetRoleDescription(a.Role),
                IsApproved = a.Status == ApprovalStatus.Approved.ToString(),
                ApprovedBy = a.ApprovedBy,
                ApprovedDate = a.ApprovedDate,
                Remarks = a.Remarks
            }).ToList();
        }
        public async Task InitiateApprovalAsync(string entityType, int entityId)
        {
            var firstApproval = await _unitOfWork.ApprovalRequests
                .FirstOrDefaultAsync(a => a.EntityType == entityType &&
                                         a.EntityId == entityId &&
                                         a.Level == 1);

            if (firstApproval != null)
            {
                await NotifyNextApprover(firstApproval);
                _logger.LogInformation($"Approval initiated for {entityType} #{entityId}");
            }
        }
        private List<string> GetRequiredRolesForLevel(int level)
        {
            return level switch
            {
                1 => new List<string> { "StoreManager", "UpazilaCommander", "StoreKeeper" },
                2 => new List<string> { "ZilaCommander", "DistrictManager" },
                3 => new List<string> { "RangeDIG", "RegionalManager" },
                4 => new List<string> { "DirectorOps", "DirectorOperations" },
                5 => new List<string> { "DirectorGeneral", "DG" },
                _ => new List<string>()
            };
        }
        private string GetRoleDescription(string role)
        {
            return role switch
            {
                "UpazilaCommander" => "Upazila Commander",
                "ZilaCommander" => "Zila Commander",
                "RangeDIG" => "Range DIG",
                "DirectorOps" => "Director (Operations)",
                "DirectorGeneral" => "Director General",
                "StoreManager" => "Store Manager",
                "StoreKeeper" => "Store Keeper",
                _ => role
            };
        }
        private async Task<bool> CheckForNextApprovalLevel(ApprovalRequest currentApproval)
        {
            var nextLevel = await _unitOfWork.ApprovalRequests
                .FirstOrDefaultAsync(a => a.EntityType == currentApproval.EntityType &&
                                         a.EntityId == currentApproval.EntityId &&
                                         a.Level > currentApproval.Level);
            return nextLevel != null;
        }
        private async Task CreateNextLevelApproval(ApprovalRequest currentApproval)
        {
            var nextApproval = await _unitOfWork.ApprovalRequests
                .FirstOrDefaultAsync(a => a.EntityType == currentApproval.EntityType &&
                                         a.EntityId == currentApproval.EntityId &&
                                         a.Level == currentApproval.Level + 1);

            if (nextApproval != null)
            {
                await NotifyNextApprover(nextApproval);
            }
        }
        private async Task<ApprovalRequest> GetNextPendingApproval(string entityType, int entityId)
        {
            return await _unitOfWork.ApprovalRequests
                .FirstOrDefaultAsync(a => a.EntityType == entityType &&
                                         a.EntityId == entityId &&
                                         a.Status == ApprovalStatus.Pending.ToString());
        }
        private async Task NotifyNextApprover(ApprovalRequest approval)
        {
            var notification = new NotificationDto
            {
                Title = $"Approval Required - {approval.EntityType}",
                Message = $"A {approval.EntityType} (ID: {approval.EntityId}) requires your approval",
                Type = GetNotificationType(approval.EntityType).ToString(),
                Priority = Priority.High.ToString(),
                TargetRole = approval.Role,
                EntityType = approval.EntityType,
                EntityId = approval.EntityId,
                CreatedAt = DateTime.Now
            };

            await _notificationService.SendNotificationAsync(notification);
            _logger.LogInformation($"Notification sent to {approval.Role} for {approval.EntityType} #{approval.EntityId}");
        }
        private NotificationType GetNotificationType(string entityType)
        {
            return entityType switch
            {
                "PhysicalInventory" => NotificationType.PhysicalInventory,
                "PurchaseOrder" => NotificationType.PurchaseOrder,
                "StockAdjustment" => NotificationType.StockAdjustment,
                _ => NotificationType.General
            };
        }
        private async Task UpdateEntityStatusAfterApproval(ApprovalRequest approval)
        {
            switch (approval.EntityType)
            {
                case "PhysicalInventory":
                    var inventory = await _unitOfWork.PhysicalInventories.GetByIdAsync(approval.EntityId);
                    if (inventory != null)
                    {
                        inventory.Status = PhysicalInventoryStatus.Approved;
                        inventory.ApprovedBy = approval.ApprovedBy;
                        inventory.ApprovedDate = approval.ApprovedDate;
                        _unitOfWork.PhysicalInventories.Update(inventory);
                    }
                    break;

                case "PurchaseOrder":
                    var po = await _unitOfWork.PurchaseOrders.GetByIdAsync(approval.EntityId);
                    if (po != null)
                    {
                        po.Status = PurchaseOrderStatus.Approved.ToString();
                        po.ApprovedBy = approval.ApprovedBy;
                        po.ApprovedDate = approval.ApprovedDate;
                        _unitOfWork.PurchaseOrders.Update(po);
                    }
                    break;

                    // Add other entity types as needed
            }

            await _unitOfWork.CompleteAsync();
        }
        private async Task UpdateEntityStatusAfterRejection(ApprovalRequest approval)
        {
            switch (approval.EntityType)
            {
                case "PhysicalInventory":
                    var inventory = await _unitOfWork.PhysicalInventories.GetByIdAsync(approval.EntityId);
                    if (inventory != null)
                    {
                        inventory.Status = PhysicalInventoryStatus.Rejected;
                        inventory.RejectedBy = approval.RejectedBy;
                        inventory.RejectedDate = approval.RejectedDate;
                        inventory.RejectionReason = approval.RejectionReason;
                        _unitOfWork.PhysicalInventories.Update(inventory);
                    }
                    break;

                case "PurchaseOrder":
                    var po = await _unitOfWork.PurchaseOrders.GetByIdAsync(approval.EntityId);
                    if (po != null)
                    {
                        po.Status = PurchaseOrderStatus.Rejected.ToString();
                        po.RejectedBy = approval.RejectedBy;
                        po.RejectedDate = approval.RejectedDate;
                        _unitOfWork.PurchaseOrders.Update(po);
                    }
                    break;

                    // Add other entity types as needed
            }

            await _unitOfWork.CompleteAsync();
        }
        private async Task FinalizeApproval(string entityType, int entityId)
        {
            // Perform final actions after all approvals are complete
            _logger.LogInformation($"All approvals complete for {entityType} #{entityId}");

            // Send final notification
            var notification = new NotificationDto
            {
                Title = $"{entityType} Fully Approved",
                Message = $"{entityType} #{entityId} has been fully approved and is ready for processing",
                Type = GetNotificationType(entityType).ToString(),
                Priority = Priority.Medium.ToString(),
                EntityType = entityType,
                EntityId = entityId,
                CreatedAt = DateTime.Now
            };

            await _notificationService.SendNotificationAsync(notification);
        }
        private async Task SendApprovalNotifications(ApprovalRequest approval, string action)
        {
            var entityName = approval.EntityType;
            var message = action.ToLower() == "approve"
                ? $"{entityName} #{approval.EntityId} has been approved"
                : $"{entityName} #{approval.EntityId} has been rejected";

            var notification = new NotificationDto
            {
                Title = $"{entityName} {action}ed",
                Message = message,
                Type = GetNotificationType(approval.EntityType).ToString(),
                Priority = Priority.Medium.ToString(),
                EntityType = approval.EntityType,
                EntityId = approval.EntityId,
                CreatedAt = DateTime.Now
            };

            await _notificationService.SendNotificationAsync(notification);
        }
        public async Task<IEnumerable<PendingApprovalDto>> GetPendingApprovalsAsync(string userId, string approvalType = null)
        {
            var approvals = await _unitOfWork.ApprovalRequests.FindAsync(a =>
                a.Status == "Pending" &&
                (approvalType == null || a.EntityType == approvalType));

            return approvals.Select(a => new PendingApprovalDto
            {
                Id = a.Id,
                Type = a.EntityType,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                RequestedBy = a.RequestedBy,
                RequestDate = a.RequestDate,
                Status = a.Status,
                Amount = a.Amount
            });
        }

        public async Task<int> GetPendingCountAsync(string userId)
        {
            var approvals = await _unitOfWork.ApprovalRequests.FindAsync(a =>
                a.Status == "Pending");
            return approvals.Count();
        }

        // Approval Configuration Management
        public async Task<List<ApprovalThresholdDto>> GetAllThresholdsAsync()
        {
            var thresholds = await _unitOfWork.ApprovalThresholds.GetAllAsync();
            return thresholds.Select(t => new ApprovalThresholdDto
            {
                Id = t.Id,
                EntityType = t.EntityType,
                MinAmount = t.MinAmount,
                MaxAmount = t.MaxAmount,
                ApprovalLevel = t.ApprovalLevel,
                RequiredRole = t.RequiredRole,
                Description = t.Description,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy
            }).OrderBy(t => t.EntityType).ThenBy(t => t.MinAmount).ToList();
        }

        public async Task<ApprovalThresholdDto> GetThresholdByIdAsync(int id)
        {
            var threshold = await _unitOfWork.ApprovalThresholds.GetByIdAsync(id);
            if (threshold == null) return null;

            return new ApprovalThresholdDto
            {
                Id = threshold.Id,
                EntityType = threshold.EntityType,
                MinAmount = threshold.MinAmount,
                MaxAmount = threshold.MaxAmount,
                ApprovalLevel = threshold.ApprovalLevel,
                RequiredRole = threshold.RequiredRole,
                Description = threshold.Description,
                IsActive = threshold.IsActive
            };
        }

        public async Task<ApprovalThresholdDto> CreateThresholdAsync(ApprovalThresholdDto dto)
        {
            var threshold = new ApprovalThreshold
            {
                EntityType = dto.EntityType,
                MinAmount = dto.MinAmount,
                MaxAmount = dto.MaxAmount,
                ApprovalLevel = dto.ApprovalLevel,
                RequiredRole = dto.RequiredRole,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System"
            };

            await _unitOfWork.ApprovalThresholds.AddAsync(threshold);
            await _unitOfWork.CompleteAsync();

            dto.Id = threshold.Id;
            _logger.LogInformation($"Approval threshold created for {dto.EntityType} by {threshold.CreatedBy}");
            return dto;
        }

        public async Task<ApprovalThresholdDto> UpdateThresholdAsync(ApprovalThresholdDto dto)
        {
            var threshold = await _unitOfWork.ApprovalThresholds.GetByIdAsync(dto.Id);
            if (threshold == null)
                throw new InvalidOperationException("Approval threshold not found");

            threshold.EntityType = dto.EntityType;
            threshold.MinAmount = dto.MinAmount;
            threshold.MaxAmount = dto.MaxAmount;
            threshold.ApprovalLevel = dto.ApprovalLevel;
            threshold.RequiredRole = dto.RequiredRole;
            threshold.Description = dto.Description;
            threshold.UpdatedAt = DateTime.Now;
            threshold.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            _unitOfWork.ApprovalThresholds.Update(threshold);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Approval threshold {dto.Id} updated for {dto.EntityType}");
            return dto;
        }

        public async Task<bool> DeleteThresholdAsync(int id)
        {
            var threshold = await _unitOfWork.ApprovalThresholds.GetByIdAsync(id);
            if (threshold == null)
                return false;

            // Soft delete
            threshold.IsActive = false;
            threshold.UpdatedAt = DateTime.Now;
            threshold.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            _unitOfWork.ApprovalThresholds.Update(threshold);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Approval threshold {id} deleted");
            return true;
        }

        public async Task<bool> ToggleThresholdStatusAsync(int id)
        {
            var threshold = await _unitOfWork.ApprovalThresholds.GetByIdAsync(id);
            if (threshold == null)
                return false;

            threshold.IsActive = !threshold.IsActive;
            threshold.UpdatedAt = DateTime.Now;
            threshold.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            _unitOfWork.ApprovalThresholds.Update(threshold);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Approval threshold {id} status toggled to {threshold.IsActive}");
            return true;
        }

        // Workflow Management
        public async Task<List<ApprovalWorkflowDto>> GetAllWorkflowsAsync()
        {
            var workflows = await _unitOfWork.ApprovalWorkflows.Query()
                .Include(w => w.Levels)
                .ToListAsync();

            return workflows.Select(w => new ApprovalWorkflowDto
            {
                Id = w.Id,
                Name = w.Name,
                EntityType = w.EntityType,
                MinAmount = w.MinAmount,
                MaxAmount = w.MaxAmount,
                Levels = w.Levels?.Select(l => new WorkflowLevelDto
                {
                    Id = l.Id,
                    Level = l.Level,
                    ApproverRole = l.ApproverRole,
                    SpecificApproverId = l.SpecificApproverId,
                    ThresholdAmount = l.ThresholdAmount,
                    CanEscalate = l.CanEscalate,
                    TimeoutHours = l.TimeoutHours
                }).OrderBy(l => l.Level).ToList() ?? new List<WorkflowLevelDto>(),
                IsActive = w.IsActive
            }).OrderBy(w => w.EntityType).ThenBy(w => w.MinAmount).ToList();
        }

        public async Task<ApprovalWorkflowDto> GetWorkflowByIdAsync(int id)
        {
            var workflow = await _unitOfWork.ApprovalWorkflows.Query()
                .Include(w => w.Levels)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workflow == null) return null;

            return new ApprovalWorkflowDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                EntityType = workflow.EntityType,
                MinAmount = workflow.MinAmount,
                MaxAmount = workflow.MaxAmount,
                Levels = workflow.Levels?.Select(l => new WorkflowLevelDto
                {
                    Id = l.Id,
                    Level = l.Level,
                    ApproverRole = l.ApproverRole,
                    SpecificApproverId = l.SpecificApproverId,
                    ThresholdAmount = l.ThresholdAmount,
                    CanEscalate = l.CanEscalate,
                    TimeoutHours = l.TimeoutHours
                }).OrderBy(l => l.Level).ToList() ?? new List<WorkflowLevelDto>(),
                IsActive = workflow.IsActive
            };
        }

        public async Task<ApprovalWorkflowDto> CreateWorkflowAsync(ApprovalWorkflowDto dto)
        {
            var workflow = new ApprovalWorkflow
            {
                Name = dto.Name,
                EntityType = dto.EntityType,
                MinAmount = dto.MinAmount ?? 0,
                MaxAmount = dto.MaxAmount,
                RequiredLevels = dto.Levels?.Count ?? 1,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System",
                Levels = new List<ApprovalWorkflowLevel>()
            };

            if (dto.Levels != null)
            {
                foreach (var levelDto in dto.Levels)
                {
                    workflow.Levels.Add(new ApprovalWorkflowLevel
                    {
                        Level = levelDto.Level,
                        ApproverRole = levelDto.ApproverRole,
                        SpecificApproverId = levelDto.SpecificApproverId,
                        ThresholdAmount = levelDto.ThresholdAmount,
                        CanEscalate = levelDto.CanEscalate,
                        TimeoutHours = levelDto.TimeoutHours,
                        CreatedAt = DateTime.Now,
                        CreatedBy = workflow.CreatedBy,
                        IsActive = true
                    });
                }
            }

            await _unitOfWork.ApprovalWorkflows.AddAsync(workflow);
            await _unitOfWork.CompleteAsync();

            dto.Id = workflow.Id;
            _logger.LogInformation($"Approval workflow created: {workflow.Name}");
            return dto;
        }

        public async Task<ApprovalWorkflowDto> UpdateWorkflowAsync(ApprovalWorkflowDto dto)
        {
            var workflow = await _unitOfWork.ApprovalWorkflows.Query()
                .Include(w => w.Levels)
                .FirstOrDefaultAsync(w => w.Id == dto.Id);

            if (workflow == null)
                throw new InvalidOperationException("Workflow not found");

            workflow.Name = dto.Name;
            workflow.EntityType = dto.EntityType;
            workflow.MinAmount = dto.MinAmount ?? 0;
            workflow.MaxAmount = dto.MaxAmount;
            workflow.RequiredLevels = dto.Levels?.Count ?? 1;
            workflow.UpdatedAt = DateTime.Now;
            workflow.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            // Remove existing levels - just clear the collection
            // EF Core will handle deletion when we save
            if (workflow.Levels != null)
            {
                workflow.Levels.Clear();
            }

            // Add new levels
            workflow.Levels = new List<ApprovalWorkflowLevel>();
            if (dto.Levels != null)
            {
                foreach (var levelDto in dto.Levels)
                {
                    workflow.Levels.Add(new ApprovalWorkflowLevel
                    {
                        WorkflowId = workflow.Id,
                        Level = levelDto.Level,
                        ApproverRole = levelDto.ApproverRole,
                        SpecificApproverId = levelDto.SpecificApproverId,
                        ThresholdAmount = levelDto.ThresholdAmount,
                        CanEscalate = levelDto.CanEscalate,
                        TimeoutHours = levelDto.TimeoutHours,
                        CreatedAt = DateTime.Now,
                        CreatedBy = workflow.UpdatedBy,
                        IsActive = true
                    });
                }
            }

            _unitOfWork.ApprovalWorkflows.Update(workflow);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Approval workflow {dto.Id} updated");
            return dto;
        }

        public async Task<bool> DeleteWorkflowAsync(int id)
        {
            var workflow = await _unitOfWork.ApprovalWorkflows.GetByIdAsync(id);
            if (workflow == null)
                return false;

            // Soft delete
            workflow.IsActive = false;
            workflow.UpdatedAt = DateTime.Now;
            workflow.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            _unitOfWork.ApprovalWorkflows.Update(workflow);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Approval workflow {id} deleted");
            return true;
        }

        public async Task<bool> ToggleWorkflowStatusAsync(int id)
        {
            var workflow = await _unitOfWork.ApprovalWorkflows.GetByIdAsync(id);
            if (workflow == null)
                return false;

            workflow.IsActive = !workflow.IsActive;
            workflow.UpdatedAt = DateTime.Now;
            workflow.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            _unitOfWork.ApprovalWorkflows.Update(workflow);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Approval workflow {id} status toggled to {workflow.IsActive}");
            return true;
        }

        // Entity Type Configuration
        public async Task<List<string>> GetConfiguredEntityTypesAsync()
        {
            var thresholds = await _unitOfWork.ApprovalThresholds.GetAllAsync(t => t.IsActive);
            var workflows = await _unitOfWork.ApprovalWorkflows.GetAllAsync(w => w.IsActive);

            var entityTypes = thresholds.Select(t => t.EntityType)
                .Union(workflows.Select(w => w.EntityType))
                .Where(e => !string.IsNullOrEmpty(e))  // Filter out null/empty values
                .Distinct()
                .OrderBy(e => e)
                .ToList();

            return entityTypes;
        }

        public async Task<bool> IsApprovalRequiredForEntityAsync(string entityType)
        {
            var threshold = await _unitOfWork.ApprovalThresholds
                .FirstOrDefaultAsync(t => t.EntityType == entityType && t.IsActive);

            var workflow = await _unitOfWork.ApprovalWorkflows
                .FirstOrDefaultAsync(w => w.EntityType == entityType && w.IsActive);

            return threshold != null || workflow != null;
        }

        public async Task<bool> ToggleEntityApprovalAsync(string entityType, bool isRequired)
        {
            var thresholds = await _unitOfWork.ApprovalThresholds
                .GetAllAsync(t => t.EntityType == entityType);

            var workflows = await _unitOfWork.ApprovalWorkflows
                .GetAllAsync(w => w.EntityType == entityType);

            foreach (var threshold in thresholds)
            {
                threshold.IsActive = isRequired;
                threshold.UpdatedAt = DateTime.Now;
                threshold.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
                _unitOfWork.ApprovalThresholds.Update(threshold);
            }

            foreach (var workflow in workflows)
            {
                workflow.IsActive = isRequired;
                workflow.UpdatedAt = DateTime.Now;
                workflow.UpdatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
                _unitOfWork.ApprovalWorkflows.Update(workflow);
            }

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Approval for {entityType} {(isRequired ? "enabled" : "disabled")}");
            return true;
        }
    }
}