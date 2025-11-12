using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services
{
    public class RequisitionService : IRequisitionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly INotificationService _notificationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IPurchaseService _purchaseService;
        private readonly ILogger<RequisitionService> _logger;

        public RequisitionService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            INotificationService notificationService,
            IActivityLogService activityLogService,
            IPurchaseService purchaseService,
            ILogger<RequisitionService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _notificationService = notificationService;
            _activityLogService = activityLogService;
            _purchaseService = purchaseService;
            _logger = logger;
        }
        public async Task<bool> MarkAsConvertedToPurchaseOrderAsync(int requisitionId, int purchaseOrderId)
        {
            try
            {
                var requisition = await _unitOfWork.Requisitions.GetByIdAsync(requisitionId);
                if (requisition == null)
                {
                    _logger.LogError($"Requisition {requisitionId} not found");
                    return false;
                }

                // Update requisition with PO reference
                requisition.PurchaseOrderId = purchaseOrderId;
                requisition.FulfillmentStatus = "PO Created";
                requisition.UpdatedAt = DateTime.Now;
                requisition.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Requisitions.Update(requisition);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Requisition",
                    requisitionId,
                    "Converted",
                    $"Requisition {requisition.RequisitionNumber} converted to Purchase Order",
                    _userContext.CurrentUserName
                );

                // Send notification to requester
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    Title = "Requisition Converted to Purchase Order",
                    Message = $"Your requisition {requisition.RequisitionNumber} has been converted to a purchase order",
                    Type = "Success",
                    UserId = requisition.RequestedBy,
                    Url = $"/Purchase/Details/{purchaseOrderId}"
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking requisition {requisitionId} as converted");
                return false;
            }
        }

        public async Task<RequisitionDto> UpdatePurchaseOrderReferenceAsync(int requisitionId, int purchaseOrderId, string purchaseOrderNo)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var requisition = await _unitOfWork.Requisitions
                    .GetAsync(r => r.Id == requisitionId, includes: new[] { "RequisitionItems" });

                if (requisition == null)
                    throw new InvalidOperationException($"Requisition {requisitionId} not found");

                // Update requisition
                requisition.PurchaseOrderId = purchaseOrderId;
                requisition.FulfillmentStatus = "PO Created";
                requisition.UpdatedAt = DateTime.Now;
                requisition.UpdatedBy = _userContext.CurrentUserName;

                // Update item fulfillment status
                foreach (var item in requisition.RequisitionItems)
                {
                    item.Status = "PO Created";
                    item.UpdatedAt = DateTime.Now;
                    item.UpdatedBy = _userContext.CurrentUserName;
                    _unitOfWork.RequisitionItems.Update(item);
                }

                _unitOfWork.Requisitions.Update(requisition);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Requisition",
                    requisitionId,
                    "PO_Created",
                    $"Purchase Order {purchaseOrderNo} created from Requisition {requisition.RequisitionNumber}",
                    _userContext.CurrentUserName
                );

                // Send notifications
                var notificationTasks = new List<Task>();

                // Notify requester
                notificationTasks.Add(_notificationService.CreateNotificationAsync(new NotificationDto
                {
                    Title = "Purchase Order Created",
                    Message = $"Purchase Order {purchaseOrderNo} has been created for your requisition {requisition.RequisitionNumber}",
                    Type = "Success",
                    UserId = requisition.RequestedBy,
                    Url = $"/Purchase/Details/{purchaseOrderId}"
                }));

                // Notify approvers
                if (!string.IsNullOrEmpty(requisition.Level1ApprovedBy))
                {
                    notificationTasks.Add(_notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        Title = "Requisition Converted",
                        Message = $"Requisition {requisition.RequisitionNumber} you approved has been converted to PO {purchaseOrderNo}",
                        Type = "Info",
                        UserId = requisition.Level1ApprovedBy,
                        Url = $"/Purchase/Details/{purchaseOrderId}"
                    }));
                }

                await Task.WhenAll(notificationTasks);

                return await GetRequisitionByIdAsync(requisitionId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error updating PO reference for requisition {requisitionId}");
                throw;
            }
        }
        public async Task<RequisitionDto> CreateRequisitionAsync(RequisitionDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var requisition = new Requisition
                {
                    RequisitionNumber = await GenerateRequisitionNumberAsync(),
                    RequestedBy = dto.RequestedBy ?? _userContext.CurrentUserName,
                    RequestDate = dto.RequestDate,
                    RequiredByDate = dto.RequiredByDate,
                    Priority = dto.Priority ?? "Normal",
                    Department = dto.Department,
                    Purpose = dto.Purpose,
                    FromStoreId = dto.FromStoreId,
                    ToStoreId = dto.ToStoreId,
                    Status = "Draft",
                    FulfillmentStatus = "Pending",
                    EstimatedValue = 0,
                    CurrentApprovalLevel = 0,
                    AutoConvertToPO = dto.AutoConvertToPO,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.Requisitions.AddAsync(requisition);
                await _unitOfWork.CompleteAsync();

                // Add requisition items
                decimal totalValue = 0;
                foreach (var itemDto in dto.Items)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                    if (item == null) continue;

                    // Use user-provided UnitPrice if available, otherwise use item's UnitCost
                    decimal unitPrice = itemDto.UnitPrice > 0 ? itemDto.UnitPrice : (item.UnitCost ?? 0);

                    var requisitionItem = new RequisitionItem
                    {
                        RequisitionId = requisition.Id,
                        ItemId = itemDto.ItemId,
                        RequestedQuantity = itemDto.RequestedQuantity,
                        ApprovedQuantity = 0,
                        IssuedQuantity = 0,
                        UnitPrice = unitPrice,
                        TotalPrice = unitPrice * itemDto.RequestedQuantity,
                        Specification = itemDto.Specification,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };

                    totalValue += requisitionItem.TotalPrice;
                    await _unitOfWork.RequisitionItems.AddAsync(requisitionItem);
                }

                // Update estimated value
                requisition.EstimatedValue = totalValue;
                _unitOfWork.Requisitions.Update(requisition);
                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();

                // TODO: Send notification based on priority
                // Temporarily commented out - GetApproverForLevel returns role name not UserId
                // Need to implement proper user lookup by role before enabling
                /*
                if (dto.Priority == "Urgent" || dto.Priority == "High")
                {
                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        Title = $"{dto.Priority} Priority Requisition",
                        Message = $"Requisition {requisition.RequisitionNumber} requires immediate attention",
                        Type = "Warning",
                        UserId = GetApproverForLevel(1, totalValue)
                    });
                }
                */

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Requisition",
                    requisition.Id,
                    "Create",
                    $"Created requisition {requisition.RequisitionNumber}",
                    _userContext.CurrentUserName
                );

                return await GetRequisitionByIdAsync(requisition.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating requisition");
                throw;
            }
        }

        public async Task<RequisitionDto> SubmitForApprovalAsync(int id)
        {
            var requisition = await _unitOfWork.Requisitions.GetByIdAsync(id);
            if (requisition == null)
                throw new InvalidOperationException("Requisition not found");

            if (requisition.Status != "Draft")
                throw new InvalidOperationException("Only draft requisitions can be submitted");

            requisition.Status = "Submitted";
            requisition.CurrentApprovalLevel = 1;
            requisition.UpdatedAt = DateTime.Now;
            requisition.UpdatedBy = _userContext.CurrentUserName;

            _unitOfWork.Requisitions.Update(requisition);
            await _unitOfWork.CompleteAsync();

            // Create approval request for Approval Center
            var approvalRequest = new ApprovalRequest
            {
                EntityType = "REQUISITION",
                EntityId = requisition.Id,
                RequestedBy = _userContext.CurrentUserName,
                RequestedDate = DateTime.Now,
                Status = "Pending",
                Priority = requisition.Priority ?? "Normal",
                Amount = requisition.EstimatedValue,
                Description = $"Requisition {requisition.RequisitionNumber} - {requisition.Purpose}",
                CurrentLevel = 1,
                MaxLevel = GetRequiredApprovalLevel(requisition.EstimatedValue),
                CreatedAt = DateTime.Now,
                CreatedBy = _userContext.CurrentUserName,
                IsActive = true
            };

            await _unitOfWork.ApprovalRequests.AddAsync(approvalRequest);
            await _unitOfWork.CompleteAsync();

            // Create approval step for Level 1
            var approvalStep = new ApprovalStep
            {
                ApprovalRequestId = approvalRequest.Id,
                StepLevel = 1,
                ApproverRole = GetApproverForLevel(1, requisition.EstimatedValue),
                Status = ApprovalStatus.Pending,
                CreatedAt = DateTime.Now,
                CreatedBy = _userContext.CurrentUserName,
                IsActive = true
            };

            await _unitOfWork.ApprovalSteps.AddAsync(approvalStep);
            await _unitOfWork.CompleteAsync();

            // Log activity
            await _activityLogService.LogActivityAsync(
                "Requisition",
                requisition.Id,
                "Submit",
                $"Submitted requisition {requisition.RequisitionNumber} for approval",
                _userContext.CurrentUserName
            );

            return await GetRequisitionByIdAsync(id);
        }

        public async Task<RequisitionDto> ApproveRequisitionAsync(int id, string comments = null)
        {
            var requisition = await _unitOfWork.Requisitions
                .GetAsync(r => r.Id == id, includes: new[] { "RequisitionItems" });

            if (requisition == null)
                throw new InvalidOperationException("Requisition not found");

            var currentUser = _userContext.CurrentUserName;
            var currentLevel = requisition.CurrentApprovalLevel;

            // Check approval authority based on value
            var approvalLevel = GetRequiredApprovalLevel(requisition.EstimatedValue);

            switch (currentLevel)
            {
                case 1:
                    requisition.Level1ApprovedBy = currentUser;
                    requisition.Level1ApprovedDate = DateTime.Now;
                    break;
                case 2:
                    requisition.Level2ApprovedBy = currentUser;
                    requisition.Level2ApprovedDate = DateTime.Now;
                    break;
                case 3:
                    requisition.FinalApprovedBy = currentUser;
                    requisition.FinalApprovedDate = DateTime.Now;
                    break;
            }

            // Check if more approvals needed
            if (currentLevel < approvalLevel)
            {
                requisition.CurrentApprovalLevel = currentLevel + 1;
                requisition.Status = $"Level {currentLevel} Approved";

                // TODO: Notify next level approver
                // Temporarily commented out - GetApproverForLevel returns role name not UserId
                /*
                var nextApprover = GetApproverForLevel(currentLevel + 1, requisition.EstimatedValue);
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    Title = "Requisition Pending Approval",
                    Message = $"Requisition {requisition.RequisitionNumber} requires Level {currentLevel + 1} approval",
                    Type = "Info",
                    UserId = nextApprover
                });
                */
            }
            else
            {
                // Final approval
                requisition.Status = "Approved";
                requisition.ApprovedBy = currentUser;
                requisition.ApprovedDate = DateTime.Now;
                requisition.ApprovalComments = comments;

                // Update approved quantities
                foreach (var item in requisition.RequisitionItems)
                {
                    item.ApprovedQuantity = item.RequestedQuantity;
                    _unitOfWork.RequisitionItems.Update(item);
                }

                requisition.ApprovedValue = requisition.EstimatedValue;

                // Auto-convert to PO if enabled
                if (requisition.AutoConvertToPO)
                {
                    await ConvertToPurchaseOrderAsync(requisition);
                }

                // Notify requester
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    Title = "Requisition Approved",
                    Message = $"Your requisition {requisition.RequisitionNumber} has been approved",
                    Type = "Success",
                    UserId = requisition.RequestedBy
                });
            }

            requisition.UpdatedAt = DateTime.Now;
            requisition.UpdatedBy = currentUser;
            _unitOfWork.Requisitions.Update(requisition);
            await _unitOfWork.CompleteAsync();

            // Log activity
            await _activityLogService.LogActivityAsync(
                "Requisition",
                requisition.Id,
                "Approve",
                $"Level {currentLevel} approval by {currentUser}",
                currentUser
            );

            return await GetRequisitionByIdAsync(id);
        }

        public async Task<RequisitionDto> RejectRequisitionAsync(int id, string reason)
        {
            var requisition = await _unitOfWork.Requisitions.GetByIdAsync(id);
            if (requisition == null)
                throw new InvalidOperationException("Requisition not found");

            requisition.Status = "Rejected";
            requisition.RejectionReason = reason;
            requisition.RejectedDate = DateTime.Now;
            requisition.UpdatedAt = DateTime.Now;
            requisition.UpdatedBy = _userContext.CurrentUserName;

            _unitOfWork.Requisitions.Update(requisition);
            await _unitOfWork.CompleteAsync();

            // Notify requester
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Requisition Rejected",
                Message = $"Your requisition {requisition.RequisitionNumber} has been rejected. Reason: {reason}",
                Type = "Error",
                UserId = requisition.RequestedBy
            });

            return await GetRequisitionByIdAsync(id);
        }

        public async Task<PurchaseDto> ConvertToPurchaseOrderAsync(Requisition requisition)
        {
            var purchaseDto = new PurchaseDto
            {
                PurchaseType = "From Requisition",
                RequisitionId = requisition.Id,
                VendorId = 0, // To be selected later
                Status = "Draft",
                Remarks = $"Generated from Requisition {requisition.RequisitionNumber}",
                Items = requisition.RequisitionItems.Select(ri => new PurchaseItemDto
                {
                    ItemId = ri.ItemId,
                    Quantity = ri.ApprovedQuantity ?? ri.RequestedQuantity,
                    UnitPrice = ri.UnitPrice,
                    StoreId = requisition.ToStoreId ?? 0
                }).ToList()
            };

            var purchase = await _purchaseService.CreatePurchaseAsync(purchaseDto);

            // Update requisition with PO reference
            requisition.PurchaseOrderId = purchase.Id;
            requisition.FulfillmentStatus = "PO Created";
            _unitOfWork.Requisitions.Update(requisition);
            await _unitOfWork.CompleteAsync();

            return purchase;
        }

        public async Task<bool> UpdateFulfillmentStatusAsync(int id)
        {
            var requisition = await _unitOfWork.Requisitions
                .GetAsync(r => r.Id == id, includes: new[] { "RequisitionItems" });

            if (requisition == null)
                return false;

            var totalRequested = requisition.RequisitionItems.Sum(i => i.RequestedQuantity);
            var totalIssued = requisition.RequisitionItems.Sum(i => i.IssuedQuantity ?? 0);

            if (totalIssued == 0)
                requisition.FulfillmentStatus = "Pending";
            else if (totalIssued < totalRequested)
                requisition.FulfillmentStatus = "Partial";
            else
                requisition.FulfillmentStatus = "Complete";

            _unitOfWork.Requisitions.Update(requisition);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private int GetRequiredApprovalLevel(decimal value)
        {
            // Define approval levels based on value
            if (value < 10000) return 1;      // Level 1: Below 10,000
            if (value < 50000) return 2;      // Level 2: 10,000 - 50,000
            return 3;                          // Level 3: Above 50,000
        }

        private string GetApproverForLevel(int level, decimal value)
        {
            // This should be configured in settings or roles
            // For now, returning placeholder
            return level switch
            {
                1 => "StoreManager",
                2 => "OperationsManager",
                3 => "Director",
                _ => "Admin"
            };
        }

        private async Task<string> GenerateRequisitionNumberAsync()
        {
            var prefix = "REQ";
            var year = DateTime.Now.Year.ToString().Substring(2);
            var month = DateTime.Now.Month.ToString("D2");

            var lastRequisition = await _unitOfWork.Requisitions
                .GetLastAsync(r => r.RequisitionNumber.StartsWith($"{prefix}{year}{month}"));

            int sequence = 1;
            if (lastRequisition != null)
            {
                var lastSequence = lastRequisition.RequisitionNumber.Substring(7);
                if (int.TryParse(lastSequence, out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            return $"{prefix}{year}{month}{sequence.ToString("D4")}";
        }

        public async Task<RequisitionDto> GetRequisitionByIdAsync(int id)
        {
            var requisition = await _unitOfWork.Requisitions
                .GetAsync(r => r.Id == id,
                    includes: new[] { "RequisitionItems", "RequisitionItems.Item", "RequestedByUser", "FromStore", "ToStore", "PurchaseOrder" });

            if (requisition == null)
                return null;

            // Calculate prices for items
            var items = requisition.RequisitionItems?.Select(i =>
            {
                // Fall back to Item.UnitCost if RequisitionItem.UnitPrice is 0
                var unitPrice = i.UnitPrice > 0 ? i.UnitPrice : (i.Item?.UnitCost ?? 0);
                var totalPrice = i.TotalPrice > 0 ? i.TotalPrice : (unitPrice * i.RequestedQuantity);

                return new RequisitionItemDto
                {
                    Id = i.Id,
                    RequisitionId = i.RequisitionId,
                    ItemId = i.ItemId,
                    ItemName = i.Item?.Name,
                    ItemCode = i.Item?.ItemCode,
                    Unit = i.Item?.Unit,
                    RequestedQuantity = i.RequestedQuantity,
                    ApprovedQuantity = i.ApprovedQuantity,
                    IssuedQuantity = i.IssuedQuantity,
                    EstimatedUnitPrice = unitPrice,
                    EstimatedTotalPrice = totalPrice,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice,
                    Specification = i.Specification,
                    Status = i.Status
                };
            }).ToList();

            // Calculate EstimatedValue from items
            var estimatedValue = items?.Sum(item => item.TotalPrice) ?? 0;

            return new RequisitionDto
            {
                Id = requisition.Id,
                RequisitionNumber = requisition.RequisitionNumber,
                RequestedBy = requisition.RequestedBy,
                RequestedByName = requisition.RequestedByUser?.FullName,
                RequestDate = requisition.RequestDate,
                RequiredByDate = requisition.RequiredByDate,
                Priority = requisition.Priority,
                Department = requisition.Department,
                Purpose = requisition.Purpose,
                FromStoreId = requisition.FromStoreId,
                FromStoreName = requisition.FromStore?.Name,
                ToStoreId = requisition.ToStoreId,
                ToStoreName = requisition.ToStore?.Name,
                Status = requisition.Status,
                FulfillmentStatus = requisition.FulfillmentStatus,
                EstimatedValue = estimatedValue,
                ApprovedValue = requisition.ApprovedValue,
                AutoConvertToPO = requisition.AutoConvertToPO,
                PurchaseOrderId = requisition.PurchaseOrderId,
                PurchaseOrderNo = requisition.PurchaseOrder?.PurchaseOrderNo,
                Level1ApprovedBy = requisition.Level1ApprovedBy,
                Level1ApprovedDate = requisition.Level1ApprovedDate,
                Level2ApprovedBy = requisition.Level2ApprovedBy,
                Level2ApprovedDate = requisition.Level2ApprovedDate,
                FinalApprovedBy = requisition.FinalApprovedBy,
                FinalApprovedDate = requisition.FinalApprovedDate,
                CurrentApprovalLevel = requisition.CurrentApprovalLevel,
                Notes = requisition.Notes,
                ApprovedBy = requisition.ApprovedBy,
                ApprovedDate = requisition.ApprovedDate,
                ApprovalComments = requisition.ApprovalComments,
                RejectedBy = requisition.RejectedBy,
                RejectedDate = requisition.RejectedDate,
                RejectionReason = requisition.RejectionReason,
                Items = items
            };
        }

        public async Task<IEnumerable<RequisitionDto>> GetAllRequisitionsAsync()
        {
            var requisitions = await _unitOfWork.Requisitions
                .GetAllAsync(includes: new[] { "RequestedByUser", "FromStore", "ToStore", "RequisitionItems", "RequisitionItems.Item" });

            return requisitions.Where(r => r.IsActive).Select(r =>
            {
                // Calculate prices for items
                var items = r.RequisitionItems?.Select(i =>
                {
                    // Fall back to Item.UnitCost if RequisitionItem.UnitPrice is 0
                    var unitPrice = i.UnitPrice > 0 ? i.UnitPrice : (i.Item?.UnitCost ?? 0);
                    var totalPrice = i.TotalPrice > 0 ? i.TotalPrice : (unitPrice * i.RequestedQuantity);

                    return new RequisitionItemDto
                    {
                        Id = i.Id,
                        RequisitionId = i.RequisitionId,
                        ItemId = i.ItemId,
                        ItemName = i.Item?.Name,
                        ItemCode = i.Item?.ItemCode,
                        Unit = i.Item?.Unit,
                        RequestedQuantity = i.RequestedQuantity,
                        ApprovedQuantity = i.ApprovedQuantity,
                        UnitPrice = unitPrice,
                        TotalPrice = totalPrice,
                        Specification = i.Specification,
                        Status = i.Status
                    };
                }).ToList() ?? new List<RequisitionItemDto>();

                // Calculate EstimatedValue from items
                var estimatedValue = items.Sum(item => item.TotalPrice);

                return new RequisitionDto
                {
                    Id = r.Id,
                    RequisitionNumber = r.RequisitionNumber,
                    RequestedBy = r.RequestedBy,
                    RequestedByName = r.RequestedByUser?.FullName,
                    RequestDate = r.RequestDate,
                    RequiredByDate = r.RequiredByDate,
                    Priority = r.Priority ?? "Normal",
                    Department = r.Department,
                    Purpose = r.Purpose,
                    Status = r.Status,
                    FulfillmentStatus = r.FulfillmentStatus,
                    EstimatedValue = estimatedValue,
                    FromStoreName = r.FromStore?.Name,
                    ToStoreName = r.ToStore?.Name,
                    PurchaseOrderId = r.PurchaseOrderId,
                    Items = items
                };
            });
        }

        public async Task<IEnumerable<RequisitionDto>> GetPendingApprovalsAsync(string approverRole)
        {
            var requisitions = await _unitOfWork.Requisitions
                .FindAsync(r => r.Status == "Submitted" || r.Status.Contains("Approved"));

            var pendingApprovals = new List<RequisitionDto>();

            foreach (var req in requisitions)
            {
                var requiredLevel = GetRequiredApprovalLevel(req.EstimatedValue);

                // Check if this approver needs to approve at current level
                if (req.CurrentApprovalLevel > 0 && req.CurrentApprovalLevel <= requiredLevel)
                {
                    var approverForLevel = GetApproverForLevel(req.CurrentApprovalLevel, req.EstimatedValue);
                    if (approverForLevel == approverRole)
                    {
                        pendingApprovals.Add(await GetRequisitionByIdAsync(req.Id));
                    }
                }
            }

            return pendingApprovals;
        }

        public async Task<IEnumerable<RequisitionDto>> GetRequisitionsByPriorityAsync(string priority)
        {
            var requisitions = await _unitOfWork.Requisitions
                .FindAsync(r => r.Priority == priority && r.IsActive);

            return requisitions.Select(r => new RequisitionDto
            {
                Id = r.Id,
                RequisitionNumber = r.RequisitionNumber,
                RequestedBy = r.RequestedBy,
                Priority = r.Priority,
                Status = r.Status,
                RequiredByDate = r.RequiredByDate,
                EstimatedValue = r.EstimatedValue
            });
        }

        public async Task<bool> DeleteRequisitionAsync(int id)
        {
            try
            {
                var requisition = await _unitOfWork.Requisitions.GetByIdAsync(id);
                if (requisition == null)
                    throw new InvalidOperationException("Requisition not found");

                if (requisition.Status != "Draft")
                    throw new InvalidOperationException("Only draft requisitions can be deleted");

                // Soft delete - mark as inactive
                requisition.IsActive = false;
                requisition.UpdatedAt = DateTime.Now;
                requisition.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Requisitions.Update(requisition);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Requisition",
                    requisition.Id,
                    "Delete",
                    $"Deleted requisition {requisition.RequisitionNumber}",
                    _userContext.CurrentUserName
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting requisition {Id}", id);
                throw;
            }
        }
    }
}