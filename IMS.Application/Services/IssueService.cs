using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class IssueService : IIssueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IValidationService _validationService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILogger<IssueService> _logger;
        private readonly INotificationService _notificationService;
        private readonly IBarcodeService _barcodeService;
        private readonly IFileService _fileService;
        private readonly IStoreService _storeService; // ADD THIS FIELD
        private readonly UserManager<User> _userManager;
        private readonly IStockService _stockService;
        private readonly IApprovalService _approvalService;
        private readonly IDigitalSignatureService _signatureService;

        public IssueService(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            IStockService stockService,
            IApprovalService approvalService,
            IDigitalSignatureService signatureService,
            INotificationService notificationService,
            IValidationService validationService,
            IActivityLogService activityLogService,
            IBarcodeService barcodeService,
            IFileService fileService,
            IStoreService storeService, // ADD THIS PARAMETER
            ILogger<IssueService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _validationService = validationService;
            _stockService = stockService;
            _approvalService = approvalService;
            _signatureService = signatureService;
            _notificationService = notificationService;
            _activityLogService = activityLogService;
            _barcodeService = barcodeService;
            _fileService = fileService;
            _storeService = storeService; // ADD THIS
            _logger = logger;
            _userManager = userManager;
        }

        // Add to IssueService.cs:

        public async Task<bool> DeleteIssueAsync(int issueId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
                if (issue == null)
                    return false;

                // Only allow deletion of Draft issues
                if (issue.Status != "Draft")
                {
                    throw new InvalidOperationException("Only draft issues can be deleted");
                }

                // Delete issue items first
                var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issueId);
                foreach (var item in issueItems)
                {
                    _unitOfWork.IssueItems.Remove(item);
                }

                // Soft delete the issue
                issue.IsActive = false;
                issue.UpdatedAt = DateTime.Now;
                issue.UpdatedBy = _userContext.CurrentUserName;
                _unitOfWork.Issues.Update(issue);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Issue",
                    issueId,
                    "Delete",
                    $"Deleted issue {issue.IssueNo}",
                    _userContext.CurrentUserName
                );

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting issue");
                throw;
            }
        }

        public async Task<bool> CanDeleteIssueAsync(int issueId)
        {
            var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
            return issue != null && issue.Status == "Draft" && issue.IsActive;
        }

        public async Task<ServiceResult> CancelIssueAsync(int issueId, string reason)
        {
            try
            {
                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
                if (issue == null)
                    return ServiceResult.Failure("Issue not found");

                if (issue.Status == "Issued" || issue.Status == "Received")
                    return ServiceResult.Failure("Cannot cancel an issued or received issue");

                issue.Status = "Cancelled";
                issue.Remarks = $"Cancelled: {reason}";
                issue.UpdatedAt = DateTime.Now;
                issue.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Issues.Update(issue);
                await _unitOfWork.CompleteAsync();

                // If the issue was approved, we need to restore the reserved stock
                if (issue.Status == "Approved")
                {
                    var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issueId);
                    foreach (var item in issueItems)
                    {
                        // Instead of ReleaseReservedStockAsync, use AdjustStockAsync to restore stock
                        await _stockService.AdjustStockAsync(
                            item.StoreId,
                            item.ItemId,
                            item.Quantity, // Positive value to add back to stock
                            $"Cancelled Issue: {issue.IssueNo}",
                            _userContext.CurrentUserName
                        );
                    }
                }

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Issue",
                    issueId,
                    "Cancel",
                    $"Issue {issue.IssueNo} cancelled. Reason: {reason}",
                    _userContext.CurrentUserName
                );

                // Send notification
                var notification = new NotificationDto
                {
                    Title = "Issue Cancelled",
                    Message = $"Issue {issue.IssueNo} has been cancelled. Reason: {reason}",
                    Type = "warning",
                    UserId = issue.CreatedBy,
                    Priority = "Normal",
                    Category = "Issue"
                };
                await _notificationService.CreateNotificationAsync(notification);

                return ServiceResult.SuccessResult("Issue cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling issue");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<IssueDto> UpdateIssueAsync(IssueDto dto)
        {
            try
            {
                var issue = await _unitOfWork.Issues.GetByIdAsync(dto.Id);
                if (issue == null || issue.Status != "Draft")
                    throw new InvalidOperationException("Issue not found or cannot be edited");

                // Update issue fields
                issue.IssueDate = dto.IssueDate;
                issue.IssuedToType = dto.IssuedToType;
                issue.IssuedToBattalionId = dto.IssuedToBattalionId;
                issue.IssuedToRangeId = dto.IssuedToRangeId;
                issue.IssuedToZilaId = dto.IssuedToZilaId;
                issue.IssuedToUpazilaId = dto.IssuedToUpazilaId;
                issue.IssuedToIndividualName = dto.IssuedToIndividualName;
                issue.IssuedToIndividualBadgeNo = dto.IssuedToIndividualBadgeNo;
                issue.Purpose = dto.Purpose;
                issue.Remarks = dto.Remarks;
                issue.Status = dto.Status;
                issue.UpdatedAt = DateTime.Now;
                issue.UpdatedBy = _userContext.CurrentUserName;

                // Update items - Use Remove instead of DeleteAsync
                var existingItems = await _unitOfWork.IssueItems.FindAsync(i => i.IssueId == issue.Id);
                foreach (var item in existingItems)
                {
                    _unitOfWork.IssueItems.Remove(item); // Use Remove instead of DeleteAsync
                }

                foreach (var itemDto in dto.Items)
                {
                    var issueItem = new IssueItem
                    {
                        IssueId = issue.Id,
                        ItemId = itemDto.ItemId,
                        StoreId = itemDto.StoreId,
                        Quantity = itemDto.Quantity,
                        Unit = itemDto.Unit,
                        Remarks = itemDto.Remarks,
                        LedgerNo = itemDto.LedgerNo,
                        PageNo = itemDto.PageNo,
                        UsableQuantity = itemDto.UsableQuantity,
                        PartiallyUsableQuantity = itemDto.PartiallyUsableQuantity,
                        UnusableQuantity = itemDto.UnusableQuantity,
                        CreatedBy = _userContext.CurrentUserName,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    await _unitOfWork.IssueItems.AddAsync(issueItem);
                }

                _unitOfWork.Issues.Update(issue);
                await _unitOfWork.CompleteAsync();

                return await GetIssueByIdAsync(issue.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating issue");
                throw;
            }
        }

        // Create Issue Request with Validation
        public async Task<IssueDto> CreateIssueRequestAsync(IssueDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate required fields
                if (!dto.FromStoreId.HasValue)
                {
                    throw new InvalidOperationException("FromStoreId is required for issue request");
                }

                if (!dto.ToEntityId.HasValue)
                {
                    throw new InvalidOperationException("ToEntityId is required for issue request");
                }

                // Validate FromStoreId
                if (!dto.FromStoreId.HasValue)
                {
                    throw new InvalidOperationException("From Store is required");
                }

                // Validate ToEntityId
                if (!dto.ToEntityId.HasValue)
                {
                    throw new InvalidOperationException("Destination is required");
                }

                // Validate stock availability
                foreach (var item in dto.Items)
                {
                    var availableStock = await _stockService.GetAvailableStockAsync(dto.FromStoreId.Value, item.ItemId);
                    var requestedQty = item.Quantity; // Quantity is decimal (not nullable)
                    if (availableStock < requestedQty)
                    {
                        throw new InvalidOperationException($"Insufficient stock for item {item.ItemName}. Available: {availableStock}, Requested: {requestedQty}");
                    }
                }

                // Create issue with pending status
                var issue = new Issue
                {
                    IssueNo = await GenerateIssueNoAsync(),
                    FromStoreId = dto.FromStoreId.Value,
                    ToEntityType = dto.ToEntityType,
                    ToEntityId = dto.ToEntityId.Value,
                    IssueDate = dto.IssueDate,
                    Status = IssueStatus.Pending.ToString(),
                    Purpose = dto.Purpose,
                    RequestedBy = dto.RequestedBy,
                    RequestedDate = DateTime.Now,
                    VoucherDocumentPath = dto.VoucherDocumentPath,
                    MemoNo = dto.MemoNo,
                    MemoDate = dto.MemoDate,
                    // Ansar & VDP specific fields
                    IssuedToBattalionId = dto.BattalionId,
                    IssuedToRangeId = dto.RangeId,
                    IssuedToZilaId = dto.ZilaId,
                    IssuedToUpazilaId = dto.UpazilaId,

                    Items = new List<IssueItem>()
                };

                foreach (var itemDto in dto.Items)
                {
                    var quantity = itemDto.Quantity; // Quantity is decimal (not nullable)
                    issue.Items.Add(new IssueItem
                    {
                        ItemId = itemDto.ItemId,
                        StoreId = itemDto.StoreId,
                        Quantity = quantity,
                        RequestedQuantity = quantity,
                        ApprovedQuantity = 0m, // Will be set during approval
                        IssuedQuantity = 0m, // Will be set during physical handover
                        Unit = itemDto.Unit ?? "Piece",
                        Remarks = itemDto.Remarks
                    });
                }

                await _unitOfWork.Issues.AddAsync(issue);
                await _unitOfWork.CompleteAsync();

                // Create approval request
                await _approvalService.CreateApprovalRequestAsync(new ApprovalRequestDto
                {
                    EntityType = "Issue",
                    EntityId = issue.Id,
                    RequestedBy = dto.RequestedBy,
                    RequestedDate = DateTime.Now,
                    Description = $"Issue Request: {issue.IssueNo} to {dto.ToEntityType}",
                    Priority = dto.Priority ?? "Normal"
                });

                // Send notification to approver
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    Title = "New Issue Request",
                    Message = $"Issue request {issue.IssueNo} requires approval",
                    Type = "Approval",
                    Priority = dto.Priority ?? "Normal",
                    CreatedAt = DateTime.Now
                });

                await _unitOfWork.CommitTransactionAsync();

                dto.Id = issue.Id;
                dto.Status = issue.Status;
                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating issue request");
                throw;
            }
        }

        // Approve Issue Request
        public async Task<bool> ApproveIssueAsync(int issueId, IssueApprovalDto approvalDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var issue = await _unitOfWork.Issues.Query()
                    .Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.Id == issueId);

                if (issue == null)
                    throw new InvalidOperationException("Issue not found");

                if (issue.Status != IssueStatus.Pending.ToString())
                    throw new InvalidOperationException("Issue is not pending for approval");

                // Update approved quantities
                foreach (var item in issue.Items)
                {
                    var approvedItem = approvalDto.ApprovedItems.FirstOrDefault(ai => ai.ItemId == item.ItemId);
                    if (approvedItem != null)
                    {
                        item.ApprovedQuantity = approvedItem.ApprovedQuantity;

                        // Reserve stock
                        await _stockService.ReserveStockAsync(issue.FromStoreId, item.ItemId, item.ApprovedQuantity);
                    }
                }

                issue.Status = IssueStatus.Approved.ToString();
                issue.ApprovedBy = approvalDto.ApprovedBy;
                issue.ApprovedDate = DateTime.Now;
                issue.ApprovalRemarks = approvalDto.Remarks;

                // Update approval record
                await _approvalService.ApproveRequestAsync(new ApprovalActionDto
                {
                    EntityType = "Issue",
                    EntityId = issueId,
                    ApprovedBy = approvalDto.ApprovedBy,
                    ApprovedDate = DateTime.Now,
                    Remarks = approvalDto.Remarks
                });

                // Generate Issue Voucher
                var voucher = await GenerateIssueVoucherAsync(issue);
                issue.VoucherNo = voucher.VoucherNo;
                issue.VoucherQRCode = voucher.QRCode;

                await _unitOfWork.CompleteAsync();

                // Send notification
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = issue.RequestedBy,
                    Title = "Issue Request Approved",
                    Message = $"Your issue request {issue.IssueNo} has been approved. Please collect the voucher.",
                    Type = "Approval",
                    CreatedAt = DateTime.Now
                });

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving issue");
                throw;
            }
        }

        // Generate Issue Voucher with QR Code
        private async Task<IssueVoucherDto> GenerateIssueVoucherAsync(Issue issue)
        {
            try
            {
                var voucher = new IssueVoucherDto
                {
                    VoucherNo = $"IV-{DateTime.Now:yyyyMMdd}-{issue.Id:D4}",
                    IssueId = issue.Id,
                    IssueNo = issue.IssueNo,
                    IssueDate = issue.IssueDate,
                    FromStoreId = issue.FromStoreId,
                    ToEntityType = issue.ToEntityType,
                    ToEntityId = issue.ToEntityId,
                    Items = issue.Items.Select(i => new IssueItemDto
                    {
                        ItemId = i.ItemId,
                        ItemName = i.Item?.Name,
                        ApprovedQuantity = i.ApprovedQuantity,
                        Unit = i.Unit
                    }).ToList()
                };

                // Generate QR code data
                var qrData = new
                {
                    Type = "ISSUE_VOUCHER",
                    VoucherNo = voucher.VoucherNo,
                    IssueId = issue.Id,
                    IssueNo = issue.IssueNo,
                    Date = issue.IssueDate,
                    Items = voucher.Items.Count,
                    ValidUntil = DateTime.Now.AddDays(7)
                };

                var qrJson = JsonSerializer.Serialize(qrData);
                voucher.QRCode = await _barcodeService.GenerateQRCodeAsync(qrJson);

                return voucher;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating issue voucher");
                throw;
            }
        }

        // Physical Handover with Digital Signature
        public async Task<bool> CompletePhysicalHandoverAsync(int issueId, HandoverDto handoverDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var issue = await _unitOfWork.Issues.Query()
                    .Include(i => i.Items)
                    .ThenInclude(ii => ii.Item)
                    .FirstOrDefaultAsync(i => i.Id == issueId);

                if (issue == null)
                    throw new InvalidOperationException("Issue not found");

                if (issue.Status != IssueStatus.Approved.ToString())
                    throw new InvalidOperationException("Issue is not approved for handover");

                // ✅ FIX: Use ReceiverBadgeId if ReceiverCode is not provided
                var receiverCode = !string.IsNullOrEmpty(handoverDto.ReceiverCode)
                    ? handoverDto.ReceiverCode
                    : handoverDto.ReceiverBadgeId;

                // Verify receiver identity (could be via OTP, biometric, etc.)
                if (!await VerifyReceiverIdentityAsync(receiverCode))
                {
                    throw new InvalidOperationException("Receiver identity verification failed");
                }

                // ✅ FIX: Auto-populate HandoverItems if not provided (simple handover)
                if (handoverDto.HandoverItems == null || !handoverDto.HandoverItems.Any())
                {
                    handoverDto.HandoverItems = issue.Items.Select(i => new HandoverItemDto
                    {
                        ItemId = i.ItemId,
                        IssuedQuantity = i.Quantity,
                        Condition = "Good",
                        Remarks = handoverDto.HandoverNotes
                    }).ToList();
                }

                // Update issued quantities
                foreach (var item in issue.Items)
                {
                    var handoverItem = handoverDto.HandoverItems.FirstOrDefault(hi => hi.ItemId == item.ItemId);
                    if (handoverItem != null)
                    {
                        item.IssuedQuantity = handoverItem.IssuedQuantity;
                        item.Condition = handoverItem.Condition;
                        item.HandoverRemarks = handoverItem.Remarks;

                        // ✅ FIX: Update stock directly without nested transaction
                        // Don't call IssueStockAsync as it starts its own transaction
                        var storeItem = await _unitOfWork.StoreItems.Query()
                            .FirstOrDefaultAsync(si => si.StoreId == issue.FromStoreId && si.ItemId == item.ItemId);

                        if (storeItem != null)
                        {
                            if (storeItem.CurrentStock < item.IssuedQuantity)
                                throw new InvalidOperationException($"Insufficient stock for item {item.ItemId}");

                            // Deduct from current stock
                            storeItem.CurrentStock -= item.IssuedQuantity;

                            // Release reserved stock if any
                            if (storeItem.ReservedStock > 0)
                            {
                                storeItem.ReservedStock = Math.Max(0, (storeItem.ReservedStock ?? 0) - item.IssuedQuantity);
                            }

                            storeItem.LastIssueDate = DateTime.Now;
                            storeItem.UpdatedAt = DateTime.Now;
                            _unitOfWork.StoreItems.Update(storeItem);
                        }
                    }
                }

                issue.Status = IssueStatus.Issued.ToString();
                issue.IssuedBy = handoverDto.HandedOverBy ?? handoverDto.IssuedBy;
                issue.IssuedDate = DateTime.Now;

                // ✅ FIX: Use ReceiverName if ReceivedBy is not provided
                issue.ReceivedBy = !string.IsNullOrEmpty(handoverDto.ReceivedBy)
                    ? handoverDto.ReceivedBy
                    : handoverDto.ReceiverName;
                issue.ReceiverDesignation = handoverDto.ReceiverDesignation;

                // ✅ FIX: Use ReceiverPhone if ReceiverContact is not provided
                issue.ReceiverContact = !string.IsNullOrEmpty(handoverDto.ReceiverContact)
                    ? handoverDto.ReceiverContact
                    : handoverDto.ReceiverPhone;

                // ✅ FIX: Use ReceiverSignature if SignatureData is not provided
                var signatureData = !string.IsNullOrEmpty(handoverDto.SignatureData)
                    ? handoverDto.SignatureData
                    : handoverDto.ReceiverSignature;

                // Create digital signature record
                var signature = await _signatureService.CreateSignatureAsync(new DigitalSignatureDto
                {
                    EntityType = "Issue",
                    EntityId = issueId,
                    SignedBy = issue.ReceivedBy,
                    SignedDate = DateTime.Now,
                    SignatureData = signatureData, // Base64 encoded signature image
                    SignatureType = "Receiver",
                    IPAddress = handoverDto.IPAddress,
                    DeviceInfo = handoverDto.DeviceInfo
                });

                issue.ReceiverSignatureId = signature.Id;

                // ✅ FIX: Store signature metadata for display
                // SignaturePath stores the Base64 signature data for display
                issue.SignaturePath = signatureData;
                issue.SignerName = handoverDto.ReceiverName;
                issue.SignerBadgeId = handoverDto.ReceiverBadgeId;
                issue.SignedDate = DateTime.Now;

                // Create stock movement records
                foreach (var item in issue.Items.Where(i => i.IssuedQuantity > 0))
                {
                    var stockMovement = new StockMovement
                    {
                        ItemId = item.ItemId,
                        StoreId = issue.FromStoreId,
                        MovementType = StockMovementType.Issue.ToString(),
                        Quantity = -item.IssuedQuantity, // Negative for issue
                        ReferenceType = "Issue",
                        ReferenceNo = issue.IssueNo,
                        MovementDate = DateTime.Now,
                        CreatedBy = handoverDto.HandedOverBy ?? handoverDto.IssuedBy,
                        Remarks = $"Issued to {issue.ToEntityType}"
                    };

                    await _unitOfWork.StockMovements.AddAsync(stockMovement);
                }

                await _unitOfWork.CompleteAsync();

                // Generate receipt
                var receipt = await GenerateIssueReceiptAsync(issue, signature);

                // Send notifications
                await SendHandoverNotificationsAsync(issue, receipt);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error completing physical handover");
                throw;
            }
        }

        // Generate Issue Receipt
        private async Task<IssueReceiptDto> GenerateIssueReceiptAsync(Issue issue, DigitalSignature signature)
        {
            var receipt = new IssueReceiptDto
            {
                ReceiptNo = $"IR-{DateTime.Now:yyyyMMdd}-{issue.Id:D4}",
                IssueNo = issue.IssueNo,
                IssueDate = issue.IssueDate,
                IssuedDate = issue.IssuedDate.Value,
                FromStore = issue.FromStore?.Name,
                ToEntity = $"{issue.ToEntityType}",
                ReceivedBy = issue.ReceivedBy,
                ReceiverDesignation = issue.ReceiverDesignation,
                Items = issue.Items.Select(i => new IssueReceiptItemDto
                {
                    ItemName = i.Item?.Name,
                    ItemCode = i.Item?.ItemCode,
                    IssuedQuantity = i.IssuedQuantity,
                    Unit = i.Unit,
                    Condition = i.Condition
                }).ToList(),
                SignatureId = signature.Id,
                SignatureData = signature.SignatureData
            };

            // Generate PDF receipt (implementation)
            receipt.ReceiptPdf = await GenerateReceiptPdfAsync(receipt);

            return receipt;
        }

        // Verify Receiver Identity
        private async Task<bool> VerifyReceiverIdentityAsync(string receiverCode)
        {
            // Implementation for identity verification
            // Could use OTP, biometric, or ID card verification
            await Task.Delay(100); // Simulated verification
            return !string.IsNullOrEmpty(receiverCode);
        }

        // Helper Methods
        public async Task<string> GenerateIssueNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"ISS-{date:yyyyMM}";

            var lastIssue = await _unitOfWork.Issues.Query()
                .Where(i => i.IssueNo.StartsWith(prefix))
                .OrderByDescending(i => i.IssueNo)
                .FirstOrDefaultAsync();

            if (lastIssue == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastIssue.IssueNo.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }

        private async Task SendHandoverNotificationsAsync(Issue issue, IssueReceiptDto receipt)
        {
            try
            {
                // ✅ FIX: Only send notification if CreatedBy is a valid user
                // ReceivedBy might be just a name string, not a UserId
                if (!string.IsNullOrEmpty(issue.CreatedBy))
                {
                    // Check if user exists before sending notification
                    var user = await _unitOfWork.Users.Query()
                        .FirstOrDefaultAsync(u => u.Id == issue.CreatedBy);

                    if (user != null)
                    {
                        await _notificationService.CreateNotificationAsync(new NotificationDto
                        {
                            UserId = issue.CreatedBy,
                            Title = "Physical Handover Completed",
                            Message = $"Issue {issue.IssueNo} has been physically handed over. Receipt: {receipt.ReceiptNo}. Received by: {issue.ReceivedBy}",
                            Type = "Receipt",
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log but don't throw - notification failure shouldn't prevent handover
                _logger.LogWarning(ex, $"Failed to send handover notification for issue {issue.IssueNo}");
            }
        }

        private async Task<byte[]> GenerateReceiptPdfAsync(IssueReceiptDto receipt)
        {
            // PDF generation implementation
            await Task.Delay(100); // Simulated
            return new byte[0];
        }

        // Interface implementations continue...
        public async Task<IEnumerable<IssueDto>> GetAllIssuesAsync()
        {
            var issues = await _unitOfWork.Issues.Query()
                .Include(i => i.FromStore)
                .Include(i => i.Items)
                .ToListAsync();

            return issues.Select(i => MapToDto(i));
        }

        public async Task<IssueDto> GetIssueByIdAsync(int id)
        {
            try
            {
                // Get the issue with all related entities
                var issue = await _unitOfWork.Issues.Query()
                    .Include(i => i.IssuedToBattalion)
                    .Include(i => i.IssuedToRange)
                    .Include(i => i.IssuedToZila)
                    .Include(i => i.IssuedToUpazila)
                    .Include(i => i.FromStore)
                    .Include(i => i.Items)
                        .ThenInclude(ii => ii.Item)
                            .ThenInclude(item => item.SubCategory)
                                .ThenInclude(sc => sc.Category)
                    .Include(i => i.Items)
                        .ThenInclude(ii => ii.Store)
                    .FirstOrDefaultAsync(i => i.Id == id && i.IsActive);

                if (issue == null)
                {
                    return null;
                }

                // Map issue items with all details
                var items = new List<IssueItemDto>();
                foreach (var issueItem in issue.Items)
                {
                    var item = issueItem.Item; // This should already be loaded via Include
                    var store = issueItem.Store; // This should already be loaded via Include

                    // Get unit price from last purchase or item
                    var lastPurchase = await _unitOfWork.PurchaseItems
                        .Query()
                        .Where(pi => pi.ItemId == issueItem.ItemId)
                        .OrderByDescending(pi => pi.CreatedAt)
                        .FirstOrDefaultAsync();
                    var unitPrice = lastPurchase?.UnitPrice ?? item?.UnitPrice ?? 0;

                    items.Add(new IssueItemDto
                    {
                        Id = issueItem.Id,
                        IssueId = issueItem.IssueId,
                        ItemId = issueItem.ItemId,
                        StoreId = issueItem.StoreId,

                        // Item details
                        ItemCode = item?.Code ?? item?.ItemCode ?? "N/A",
                        ItemName = item?.Name ?? "Unknown Item",
                        CategoryName = item?.SubCategory?.Category?.Name ?? "Uncategorized",
                        SubCategoryName = item?.SubCategory?.Name ?? "N/A",
                        Unit = item?.Unit ?? "Piece",

                        // Store details
                        StoreName = store?.Name ?? "Unknown Store",

                        // Quantities
                        Quantity = issueItem.Quantity,
                        IssuedQuantity = issueItem.IssuedQuantity,
                        RequestedQuantity = issueItem.RequestedQuantity,
                        ApprovedQuantity = issueItem.ApprovedQuantity,

                        // Price
                        UnitPrice = unitPrice,

                        // Ledger tracking fields
                        BatchNumber = issueItem.BatchNumber,
                        LedgerBookId = issueItem.LedgerBookId,
                        LedgerNo = issueItem.LedgerNo,
                        PageNo = issueItem.PageNo,

                        // Quantity breakdown fields
                        UsableQuantity = issueItem.UsableQuantity,
                        PartiallyUsableQuantity = issueItem.PartiallyUsableQuantity,
                        UnusableQuantity = issueItem.UnusableQuantity,

                        // Other fields
                        Remarks = issueItem.Remarks ?? "-",
                        Condition = issueItem.Condition ?? "Good"
                    });
                }

                // Fetch attachments/documents for this issue
                var attachments = await _unitOfWork.Documents
                    .Query()
                    .Where(d => d.EntityType == "ISSUE" && d.EntityId == id)
                    .OrderByDescending(d => d.UploadDate)
                    .Select(d => new AttachmentDto
                    {
                        Id = d.Id,
                        FileName = d.FileName,
                        FilePath = d.FilePath,
                        FileSize = d.FileSize,
                        UploadDate = d.UploadDate
                    })
                    .ToListAsync();

                // Fetch approval history for this issue
                var approvalHistory = await _unitOfWork.ApprovalHistories
                    .Query()
                    .Where(h => h.EntityType == "ISSUE" && h.EntityId == id)
                    .OrderBy(h => h.ActionDate)
                    .Select(h => new ApprovalHistoryDto
                    {
                        Id = h.Id,
                        Action = h.Action,
                        ActionBy = h.ActionBy,
                        ActionDate = h.ActionDate,
                        Comments = h.Comments,
                        Level = h.Level,
                        PreviousStatus = h.PreviousStatus,
                        NewStatus = h.NewStatus
                    })
                    .ToListAsync();

                // Fetch digital signatures for this issue
                var signatures = await _unitOfWork.Signatures
                    .Query()
                    .Where(s => s.ReferenceType == "ISSUE" && s.ReferenceId == id && s.IsActive)
                    .OrderBy(s => s.SignedDate)
                    .Select(s => new DigitalSignatureDto
                    {
                        Id = s.Id,
                        ReferenceType = s.ReferenceType,
                        ReferenceId = s.ReferenceId,
                        SignatureType = s.SignatureType,
                        SignatureData = s.SignatureData,
                        SignedBy = s.SignerName,
                        SignerBadgeId = s.SignerBadgeId,
                        SignerDesignation = s.SignerDesignation,
                        SignedDate = s.SignedDate,
                        IPAddress = s.IPAddress,
                        DeviceInfo = s.DeviceInfo
                    })
                    .ToListAsync();

                // Create the IssueDto with all mapped data
                var issueDto = new IssueDto
                {
                    Id = issue.Id,
                    IssueNo = issue.IssueNo,
                    IssueDate = issue.IssueDate,
                    Status = issue.Status ?? "Draft",
                    Purpose = issue.Purpose,
                    Remarks = issue.Remarks,

                    // Issue Type and Destination
                    IssuedToType = issue.IssuedToType,
                    IssuedTo = issue.IssuedTo,
                    IssuedToName = GetIssuedToName(issue),

                    // Organizational hierarchy IDs
                    IssuedToBattalionId = issue.IssuedToBattalionId,
                    IssuedToRangeId = issue.IssuedToRangeId,
                    IssuedToZilaId = issue.IssuedToZilaId,
                    IssuedToUpazilaId = issue.IssuedToUpazilaId,

                    // Organizational hierarchy names
                    IssuedToBattalionName = issue.IssuedToBattalion?.Name,
                    IssuedToRangeName = issue.IssuedToRange?.Name,
                    IssuedToZilaName = issue.IssuedToZila?.Name,
                    IssuedToUpazilaName = issue.IssuedToUpazila?.Name,

                    // Individual details
                    IssuedToIndividualName = issue.IssuedToIndividualName,
                    IssuedToIndividualBadgeNo = issue.IssuedToIndividualBadgeNo,

                    // Store information
                    FromStoreId = issue.FromStoreId,
                    FromStoreName = issue.FromStore?.Name ?? "Unknown Store",

                    // Delivery information
                    DeliveryLocation = issue.DeliveryLocation,

                    // Voucher information
                    VoucherNo = issue.VoucherNo ?? issue.VoucherNumber,
                    VoucherDate = issue.VoucherDate,
                    VoucherGeneratedDate = issue.VoucherGeneratedDate,
                    VoucherNumber = issue.VoucherNumber,
                    QRCode = issue.QRCode ?? issue.VoucherQRCode,

                    // Authorization & Signature
                    IssuedBy = issue.IssuedBy,
                    SignerName = issue.SignerName,
                    SignerBadgeId = issue.SignerBadgeId,
                    SignerDesignation = issue.SignerDesignation,
                    SignaturePath = issue.SignaturePath,
                    SignedDate = issue.SignedDate,

                    // Approval information
                    ApprovedBy = issue.ApprovedBy,
                    ApprovedByName = issue.ApprovedByName,
                    ApprovedByBadgeNo = issue.ApprovedByBadgeNo,
                    ApprovedDate = issue.ApprovedDate,
                    ApprovalComments = issue.ApprovalComments,
                    RejectionReason = issue.RejectionReason,

                    // Partial issue information
                    IsPartialIssue = issue.IsPartialIssue,
                    ParentIssueId = issue.ParentIssueId,
                    ParentIssueNo = issue.ParentIssue?.IssueNo,

                    // Audit fields
                    CreatedAt = issue.CreatedAt,
                    CreatedBy = issue.CreatedBy,
                    UpdatedAt = issue.UpdatedAt,
                    UpdatedBy = issue.UpdatedBy,
                    VoucherDocumentPath = issue.VoucherDocumentPath, // MAKE SURE THIS LINE EXISTS

                    // Items collection
                    Items = items,

                    // Attachments, History and Signatures
                    Attachments = attachments,
                    ApprovalHistory = approvalHistory,
                    Signatures = signatures,

                    // Signature IDs
                    IssuerSignatureId = issue.IssuerSignatureId,
                    ApproverSignatureId = issue.ApproverSignatureId,
                    ReceiverSignatureId = issue.ReceiverSignatureId,

                    // Additional fields
                    AllotmentLetterId = issue.AllotmentLetterId,
                    AllotmentLetterNo = issue.AllotmentLetterNo,
                    MemoNo = issue.MemoNo,
                    MemoDate = issue.MemoDate,
                    ReceivedBy = issue.ReceivedBy,
                    ReceivedDate = issue.ReceivedDate
                };

                return issueDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue by id: {Id}", id);
                throw;
            }
        }

        private IssueDto MapToDto(Issue issue)
        {
            return new IssueDto
            {
                Id = issue.Id,
                IssueNo = issue.IssueNo,
                FromStoreId = issue.FromStoreId,
                FromStoreName = issue.FromStore?.Name,
                ToEntityType = issue.ToEntityType,
                ToEntityId = issue.ToEntityId,
                IssueDate = issue.IssueDate,
                Status = issue.Status,
                Purpose = issue.Purpose,
                VoucherDocumentPath = issue.VoucherDocumentPath,

                // ✅ FIX: Include VoucherNumber in mapping
                VoucherNumber = issue.VoucherNumber,
                VoucherGeneratedDate = issue.VoucherGeneratedDate,

                Items = issue.Items?.Select(i => new IssueItemDto
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    ItemName = i.Item?.Name,
                    Quantity = i.RequestedQuantity,
                    ApprovedQuantity = i.ApprovedQuantity,
                    IssuedQuantity = i.IssuedQuantity,
                    Unit = i.Unit,
                    LedgerNo = i.LedgerNo,
                    PageNo = i.PageNo,
                    UsableQuantity = i.UsableQuantity,
                    PartiallyUsableQuantity = i.PartiallyUsableQuantity,
                    UnusableQuantity = i.UnusableQuantity
                }).ToList()
            };
        }

        // Get paged issues with filters
        public async Task<PagedResult<IssueDto>> GetAllIssuesAsync(int pageNumber = 1, int pageSize = 50,
            string searchTerm = null, string status = null, string issueType = null,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // Get all active issues
                var allIssues = await _unitOfWork.Issues.FindAsync(i => i.IsActive);

                // Apply filters
                var filteredIssues = allIssues.AsEnumerable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    filteredIssues = filteredIssues.Where(i =>
                        i.IssueNo.ToLower().Contains(searchTerm) ||
                        (i.VoucherNumber != null && i.VoucherNumber.ToLower().Contains(searchTerm)) ||
                        (i.IssuedTo != null && i.IssuedTo.ToLower().Contains(searchTerm)) ||
                        (i.Purpose != null && i.Purpose.ToLower().Contains(searchTerm))
                    );
                }

                if (!string.IsNullOrEmpty(status))
                {
                    filteredIssues = filteredIssues.Where(i => i.Status == status);
                }

                if (!string.IsNullOrEmpty(issueType))
                {
                    filteredIssues = filteredIssues.Where(i => i.IssuedToType == issueType);
                }

                if (fromDate.HasValue)
                {
                    filteredIssues = filteredIssues.Where(i => i.IssueDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    filteredIssues = filteredIssues.Where(i => i.IssueDate <= toDate.Value);
                }

                // Get total count after filtering
                var totalCount = filteredIssues.Count();

                // Apply pagination
                var pagedIssues = filteredIssues
                    .OrderByDescending(i => i.IssueDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Map to DTOs
                var issueDtos = new List<IssueDto>();
                foreach (var issue in pagedIssues)
                {
                    issueDtos.Add(await MapToIssueDto(issue));
                }

                return new PagedResult<IssueDto>
                {
                    Items = issueDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged issues with filters");
                throw;
            }
        }
        public async Task<IssueDto> CreateIssueAsync(IssueDto issueDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Validate Provision Store requirements
                if (issueDto.FromStoreId.HasValue)
                {
                    var fromStore = await _unitOfWork.Stores.GetByIdAsync(issueDto.FromStoreId.Value);

                    if (fromStore?.StoreType.Name != null &&
                        fromStore.StoreType.Name.Contains("Provision", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(issueDto.VoucherDocumentPath))
                        {
                            throw new InvalidOperationException(
                                "Document upload is mandatory for issues from Provision Store. Please upload allotment letter or authorization document.");
                        }

                        if (string.IsNullOrEmpty(issueDto.MemoNo))
                        {
                            throw new InvalidOperationException(
                                "Memo number is mandatory for issues from Provision Store.");
                        }

                        // AllotmentLetter required for Provision Store (unless Store-to-Store)
                        if (issueDto.IssuedToType != "Store" && !issueDto.AllotmentLetterId.HasValue)
                        {
                            throw new InvalidOperationException(
                                "Allotment Letter is mandatory for issues from Provision Store.");
                        }

                        // Validate AllotmentLetter if provided
                        if (issueDto.AllotmentLetterId.HasValue)
                        {
                            var allotment = await _unitOfWork.AllotmentLetters
                                .Query()
                                .Include(a => a.Items)
                                .FirstOrDefaultAsync(a => a.Id == issueDto.AllotmentLetterId.Value);

                            if (allotment == null || allotment.Status != "Active")
                                throw new InvalidOperationException("Invalid or inactive Allotment Letter");

                            if (allotment.ValidUntil < DateTime.Now)
                                throw new InvalidOperationException("Allotment Letter has expired");

                            if (allotment.FromStoreId != issueDto.FromStoreId)
                                throw new InvalidOperationException("Allotment Letter is for a different store");

                            // Validate item quantities
                            foreach (var item in issueDto.Items ?? Enumerable.Empty<IssueItemDto>())
                            {
                                var allotmentItem = allotment.Items.FirstOrDefault(ai => ai.ItemId == item.ItemId);
                                if (allotmentItem == null)
                                    throw new InvalidOperationException($"Item not found in Allotment Letter");

                                if (allotmentItem.RemainingQuantity < item.Quantity)
                                    throw new InvalidOperationException($"Insufficient quantity in Allotment Letter for {item.ItemName}");
                            }
                        }
                    }
                }

                // Generate Issue Number if not provided
                if (string.IsNullOrEmpty(issueDto.IssueNo))
                {
                    issueDto.IssueNo = await GenerateIssueNoAsync();
                }

                // Create Issue entity WITHOUT signature data
                var issue = new Issue
                {
                    IssueNo = issueDto.IssueNo,
                    IssueDate = issueDto.IssueDate,
                    Status = issueDto.Status ?? "Draft",
                    Purpose = issueDto.Purpose,
                    Remarks = issueDto.Remarks,

                    // Set issued to fields
                    IssuedToType = issueDto.IssuedToType,
                    IssuedTo = issueDto.IssuedTo ?? issueDto.IssuedToName,

                    // Set organizational hierarchy
                    IssuedToBattalionId = issueDto.IssuedToBattalionId,
                    IssuedToRangeId = issueDto.IssuedToRangeId,
                    IssuedToZilaId = issueDto.IssuedToZilaId,
                    IssuedToUpazilaId = issueDto.IssuedToUpazilaId,

                    // Individual details
                    IssuedToIndividualName = issueDto.IssuedToIndividualName,
                    IssuedToIndividualBadgeNo = issueDto.IssuedToIndividualBadgeNo,

                    // Store and delivery
                    FromStoreId = issueDto.FromStoreId,
                    DeliveryLocation = issueDto.DeliveryLocation,

                    // Authorization
                    IssuedBy = issueDto.IssuedBy,
                    SignerName = issueDto.SignerName,
                    SignerBadgeId = issueDto.SignerBadgeId,
                    SignerDesignation = issueDto.SignerDesignation,
                    SignedDate = issueDto.SignedDate,

                    // Partial issue
                    IsPartialIssue = issueDto.IsPartialIssue,
                    ParentIssueId = issueDto.ParentIssueId,

                    VoucherDocumentPath = issueDto.VoucherDocumentPath,
                    MemoNo = issueDto.MemoNo,
                    MemoDate = issueDto.MemoDate,

                    // Audit fields
                    CreatedAt = DateTime.Now,
                    CreatedBy = issueDto.CreatedBy ?? _userContext.UserId,
                    IsActive = true
                };

                // Add issue first to get the ID
                await _unitOfWork.Issues.AddAsync(issue);
                await _unitOfWork.SaveChangesAsync();

                // Save the signature if provided  
                if (!string.IsNullOrEmpty(issueDto.SignaturePath))
                {
                    var signature = await _signatureService.SaveSignatureAsync(
                        "Issue",
                        issue.Id,
                        "Issuer",
                        issueDto.SignaturePath,  // This contains the base64 data
                        issueDto.SignerName,
                        issueDto.SignerBadgeId,
                        issueDto.SignerDesignation
                    );

                    // Update issue with signature ID
                    issue.IssuerSignatureId = signature.Id;
                    issue.SignaturePath = null; // Don't store base64 in SignaturePath
                    issue.SignatureDate = signature.SignedDate;

                    _unitOfWork.Issues.Update(issue);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Add issue items
                if (issueDto.Items != null && issueDto.Items.Any())
                {
                    foreach (var itemDto in issueDto.Items)
                    {
                        var issueItem = new IssueItem
                        {
                            IssueId = issue.Id,
                            ItemId = itemDto.ItemId,
                            StoreId = itemDto.StoreId ?? issueDto.FromStoreId ?? 0,
                            Quantity = itemDto.Quantity,
                            RequestedQuantity = itemDto.RequestedQuantity ?? itemDto.Quantity,
                            IssuedQuantity = 0,
                            ApprovedQuantity = 0,
                            Unit = itemDto.Unit,
                            BatchNumber = itemDto.BatchNumber,
                            Condition = itemDto.Condition ?? "Good",
                            Remarks = itemDto.Remarks,
                            LedgerNo = itemDto.LedgerNo,
                            PageNo = itemDto.PageNo,
                            UsableQuantity = itemDto.UsableQuantity,
                            PartiallyUsableQuantity = itemDto.PartiallyUsableQuantity,
                            UnusableQuantity = itemDto.UnusableQuantity,
                            CreatedAt = DateTime.Now,
                            CreatedBy = issueDto.CreatedBy ?? _userContext.UserId,
                            IsActive = true
                        };

                        await _unitOfWork.IssueItems.AddAsync(issueItem);
                    }

                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Issue",
                    issue.Id,
                    "Create",
                    $"Issue {issue.IssueNo} created",
                    issue.CreatedBy ?? _userContext.UserId
                );

                // Send notification if needed
                if (issue.Status == "Pending")
                {
                    await SendIssueCreatedNotificationAsync(issue);
                }

                return await GetIssueByIdAsync(issue.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating issue");
                throw;
            }
        }

        // Add this private helper method to the IssueService class
        private async Task SendIssueCreatedNotificationAsync(Issue issue)
        {
            // Create notification for approvers
            var notification = new NotificationDto
            {
                UserId = issue.CreatedBy ?? _userContext.UserId,
                Title = "Issue Created - Pending Approval",
                Message = $"Issue {issue.IssueNo} has been created and is pending approval. " +
                          $"Purpose: {issue.Purpose ?? "N/A"}. " +
                          $"Items: {await GetIssueItemCountAsync(issue.Id)}",
                Type = "Info",
                Priority = "High",
                Category = "Issue",
                ReferenceType = "Issue",
                ReferenceId = issue.Id.ToString(),
                ActionUrl = $"/Issue/Details/{issue.Id}"
            };

            await _notificationService.CreateNotificationAsync(notification);

            // If you need to notify specific approvers based on store or role
            var approverRoles = new[] { "StoreOfficer", "Manager", "Admin" };
            foreach (var role in approverRoles)
            {
                await _notificationService.SendToRoleAsync(
                    role,
                    "New Issue Pending Approval",
                    $"Issue {issue.IssueNo} requires approval",
                    "info"
                );
            }
        }

        // Helper method to get issue item count
        private async Task<int> GetIssueItemCountAsync(int issueId)
        {
            var items = await _unitOfWork.IssueItems
                .Query()
                .Where(i => i.IssueId == issueId && i.IsActive)
                .CountAsync();

            return items;
        }

        public async Task<bool> SubmitForApprovalAsync(int issueId, string submittedBy)
        {
            try
            {
                _logger.LogInformation($"🔍 [SUBMIT] Starting submission for Issue #{issueId} by {submittedBy}");

                // First update the issue status (no transaction here)
                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
                if (issue == null)
                {
                    _logger.LogError($"❌ [SUBMIT] Issue #{issueId} not found");
                    return false;
                }

                if (issue.Status != "Draft")
                {
                    _logger.LogWarning($"⚠️ [SUBMIT] Issue #{issueId} status is '{issue.Status}', not 'Draft'");
                    return false;
                }

                issue.Status = "Pending";
                issue.UpdatedAt = DateTime.UtcNow;
                issue.UpdatedBy = submittedBy;
                _unitOfWork.Issues.Update(issue);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"✅ [SUBMIT] Issue #{issueId} status updated to Pending");

                // Calculate total value for approval threshold
                decimal totalValue = 0;
                var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issueId);
                foreach (var item in issueItems)
                {
                    var itemDetails = await _unitOfWork.Items.GetByIdAsync(item.ItemId);
                    if (itemDetails != null)
                    {
                        var lastPurchase = await _unitOfWork.PurchaseItems
                            .Query()
                            .Where(pi => pi.ItemId == item.ItemId)
                            .OrderByDescending(pi => pi.CreatedAt)
                            .FirstOrDefaultAsync();

                        var unitPrice = lastPurchase?.UnitPrice ?? itemDetails.UnitPrice ?? 100;
                        // ✅ FIX: Handle nullable Quantity value
                        var quantity = item.Quantity;
                        totalValue += quantity * unitPrice;
                    }
                }

                // Call ApprovalService which handles its own transaction
                await _approvalService.CreateApprovalRequestAsync(new ApprovalRequestDto
                {
                    EntityType = "ISSUE",
                    EntityId = issue.Id,
                    RequestedBy = submittedBy,
                    RequestedDate = DateTime.Now,
                    Description = $"Issue {issue.IssueNo} - {issue.Purpose}",
                    Amount = totalValue,
                    Priority = "Normal"
                });

                // Send notification
                await SendApprovalNotificationAsync(issue);

                await _activityLogService.LogActivityAsync(
                    "Issue",
                    issue.Id,
                    "Submit",
                    $"Submitted issue {issue.IssueNo} for approval",
                    submittedBy
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ [SUBMIT] ERROR submitting issue for approval: {ex.Message}");
                _logger.LogError(ex, $"❌ [SUBMIT] Stack trace: {ex.StackTrace}");
                throw; // ⚠️ Throw the exception so we can see the actual error
            }
        }

        public async Task<IssueDto> ApproveIssueAsync(int issueId, string approvedBy, string comments = null)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
                if (issue == null || issue.Status != "Pending")
                    return null;

                // Update issue status
                issue.Status = "Approved";
                issue.ApprovedBy = approvedBy;
                issue.ApprovedDate = DateTime.UtcNow;
                issue.ApprovalComments = comments;
                issue.UpdatedAt = DateTime.UtcNow;
                issue.UpdatedBy = approvedBy;

                _unitOfWork.Issues.Update(issue);

                // Update approval request
                var approvalRequest = await _unitOfWork.ApprovalRequests
                    .FirstOrDefaultAsync(ar => ar.EntityType == "ISSUE" &&
                                               ar.EntityId == issueId &&
                                               ar.Status == "Pending");

                if (approvalRequest != null)
                {
                    approvalRequest.Status = "Approved";
                    approvalRequest.ApprovedBy = approvedBy;
                    approvalRequest.ApprovedDate = DateTime.Now;
                    approvalRequest.Remarks = comments;
                    _unitOfWork.ApprovalRequests.Update(approvalRequest);
                }

                // Update stock levels
                var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issueId);
                foreach (var item in issueItems)
                {
                    var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                        si => si.ItemId == item.ItemId && si.StoreId == item.StoreId
                    );

                    if (storeItem != null)
                    {
                        if (storeItem.Quantity < item.Quantity)
                        {
                            throw new InvalidOperationException($"Insufficient stock for item {item.ItemId}");
                        }

                        storeItem.Quantity -= item.Quantity;
                        storeItem.UpdatedAt = DateTime.UtcNow;
                        storeItem.UpdatedBy = approvedBy;
                        _unitOfWork.StoreItems.Update(storeItem);

                        // Check for low stock
                        await CheckAndCreateStockAlertAsync(storeItem);
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogActivityAsync(
                    _userContext.GetCurrentUserId(),
                    "Issue Approved",
                    "Issue",
                    issue.Id,
                    null,
                    $"Approved issue {issue.IssueNo}",
                    approvedBy
                );

                // Return the updated issue as DTO
                return await GetIssueByIdAsync(issueId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving issue");
                throw;
            }
        }


        // Reject issue
        public async Task<IssueDto> RejectIssueAsync(int issueId, string rejectedBy, string reason)
        {
            var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
            if (issue == null || issue.Status != "Pending")
                return null;

            issue.Status = "Rejected";
            issue.RejectionReason = reason;
            issue.UpdatedAt = DateTime.UtcNow;
            issue.UpdatedBy = rejectedBy;

            _unitOfWork.Issues.Update(issue);
            await _unitOfWork.CompleteAsync();

            await _activityLogService.LogActivityAsync(
                _userContext.GetCurrentUserId(),
                "Issue Rejected",
                "Issue",
                issue.Id,
                null,
                $"Rejected issue {issue.IssueNo}: {reason}",
                rejectedBy
            );

            // Return the updated issue as DTO
            return await GetIssueByIdAsync(issueId);
        }

        // Create partial issue
        public async Task<IssueDto> CreatePartialIssueAsync(int parentIssueId, Dictionary<int, decimal> actualQuantities, string createdBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var parentIssue = await _unitOfWork.Issues.GetByIdAsync(parentIssueId);
                if (parentIssue == null || parentIssue.Status != "Approved")
                    throw new InvalidOperationException("Parent issue not found or not approved");

                var partialIssue = new Issue
                {
                    IssueNo = await GenerateIssueNoAsync() + "-P",
                    IssueDate = DateTime.Now,
                    IssuedToType = parentIssue.IssuedToType,
                    IssuedToBattalionId = parentIssue.IssuedToBattalionId,
                    IssuedToRangeId = parentIssue.IssuedToRangeId,
                    IssuedToZilaId = parentIssue.IssuedToZilaId,
                    IssuedToUpazilaId = parentIssue.IssuedToUpazilaId,
                    IssuedToIndividualName = parentIssue.IssuedToIndividualName,
                    IssuedToIndividualBadgeNo = parentIssue.IssuedToIndividualBadgeNo,
                    IssuedBy = createdBy,
                    Purpose = parentIssue.Purpose,
                    Status = "Approved", // Auto-approve partial issues
                    ApprovedBy = createdBy,
                    ApprovedDate = DateTime.UtcNow,
                    IsPartialIssue = true,
                    ParentIssueId = parentIssueId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    IsActive = true
                };

                await _unitOfWork.Issues.AddAsync(partialIssue);
                await _unitOfWork.CompleteAsync();

                // Add partial issue items
                foreach (var kvp in actualQuantities)
                {
                    var parentItem = await _unitOfWork.IssueItems.SingleOrDefaultAsync(
                        ii => ii.IssueId == parentIssueId && ii.ItemId == kvp.Key
                    );

                    if (parentItem != null && kvp.Value > 0)
                    {
                        var issueItem = new IssueItem
                        {
                            IssueId = partialIssue.Id,
                            ItemId = kvp.Key,
                            StoreId = parentItem.StoreId,
                            Quantity = kvp.Value,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = createdBy
                        };

                        await _unitOfWork.IssueItems.AddAsync(issueItem);

                        // Update stock
                        var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                            si => si.ItemId == kvp.Key && si.StoreId == parentItem.StoreId
                        );

                        if (storeItem != null)
                        {
                            storeItem.Quantity -= kvp.Value;
                            _unitOfWork.StoreItems.Update(storeItem);
                            await CheckAndCreateStockAlertAsync(storeItem);
                        }
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetIssueByIdAsync(partialIssue.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating partial issue");
                throw;
            }
        }

        public async Task<bool> IssueNoExistsAsync(string issueNo)
        {
            if (string.IsNullOrWhiteSpace(issueNo))
                return false;

            return await _unitOfWork.Issues.ExistsAsync(i => i.IssueNo == issueNo);
        }

        // Get issues by date range
        public async Task<IEnumerable<IssueDto>> GetIssuesByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var issues = await _unitOfWork.Issues.FindAsync(i =>
                i.IsActive &&
                i.IssueDate >= fromDate &&
                i.IssueDate <= toDate
            );

            var issueDtos = new List<IssueDto>();
            foreach (var issue in issues)
            {
                issueDtos.Add(await MapToIssueDto(issue));
            }

            return issueDtos.OrderByDescending(i => i.IssueDate);
        }

        // Get issues by recipient
        public async Task<IEnumerable<IssueDto>> GetIssuesByRecipientAsync(string recipient)
        {
            var issues = await _unitOfWork.Issues.FindAsync(i =>
                i.IsActive && (
                    i.IssuedToIndividualName.Contains(recipient) ||
                    i.IssuedToIndividualBadgeNo.Contains(recipient)
                )
            );

            var issueDtos = new List<IssueDto>();
            foreach (var issue in issues)
            {
                issueDtos.Add(await MapToIssueDto(issue));
            }

            return issueDtos.OrderByDescending(i => i.IssueDate);
        }

        // Get issues by type
        public async Task<IEnumerable<IssueDto>> GetIssuesByTypeAsync(string recipientType)
        {
            var issues = await _unitOfWork.Issues.FindAsync(i =>
                i.IsActive && i.IssuedToType == recipientType
            );

            var issueDtos = new List<IssueDto>();
            foreach (var issue in issues)
            {
                issueDtos.Add(await MapToIssueDto(issue));
            }

            return issueDtos.OrderByDescending(i => i.IssueDate);
        }

        // Get issue count
        public async Task<int> GetIssueCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var issues = await _unitOfWork.Issues.GetAllAsync();
            var query = issues.Where(i => i.IsActive);

            if (fromDate.HasValue)
                query = query.Where(i => i.IssueDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(i => i.IssueDate <= toDate.Value);

            return query.Count();
        }

        // Get total issued value
        public async Task<decimal> GetTotalIssuedValueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var issues = await _unitOfWork.Issues.GetAllAsync();
            var query = issues.Where(i => i.IsActive && i.Status == "Approved");

            if (fromDate.HasValue)
                query = query.Where(i => i.IssueDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(i => i.IssueDate <= toDate.Value);

            decimal totalValue = 0;
            foreach (var issue in query)
            {
                var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);
                foreach (var issueItem in issueItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(issueItem.ItemId);
                    if (item != null)
                    {
                        // Get last purchase price
                        var purchaseItems = await _unitOfWork.PurchaseItems.FindAsync(pi => pi.ItemId == issueItem.ItemId);
                        var lastPrice = purchaseItems.OrderByDescending(pi => pi.CreatedAt).FirstOrDefault()?.UnitPrice ?? 0;
                        totalValue += issueItem.Quantity * lastPrice;
                    }
                }
            }

            return totalValue;
        }

        public async Task<IEnumerable<IssueDto>> GetPendingIssuesAsync()
        {
            var issues = await _unitOfWork.Issues.FindAsync(i =>
                i.IsActive && i.Status == "Pending"
            );

            var issueDtos = new List<IssueDto>();
            foreach (var issue in issues)
            {
                issueDtos.Add(await MapToIssueDto(issue));
            }

            return issueDtos.OrderByDescending(i => i.IssueDate);
        }

        // Get paged issues (simple version)
        public async Task<IEnumerable<IssueDto>> GetPagedIssuesAsync(int pageNumber, int pageSize)
        {
            var allIssues = await GetAllIssuesAsync();
            return allIssues
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        // Get approved issues without receives
        public async Task<IEnumerable<IssueDto>> GetApprovedIssuesWithoutReceivesAsync()
        {
            var approvedIssues = await _unitOfWork.Issues.FindAsync(i => i.Status == "Approved" && i.IsActive);
            var issuesWithoutReceives = new List<IssueDto>();

            foreach (var issue in approvedIssues)
            {
                // Check if this issue has any receives
                var receives = await _unitOfWork.Receives.FindAsync(r => r.OriginalIssueId == issue.Id && r.IsActive);

                if (!receives.Any())
                {
                    issuesWithoutReceives.Add(await GetIssueByIdAsync(issue.Id));
                }
                else
                {
                    // Check if partially received
                    var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);
                    var totalIssued = issueItems.Sum(ii => ii.Quantity);

                    var totalReceived = 0m;
                    foreach (var receive in receives)
                    {
                        var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri => ri.ReceiveId == receive.Id);
                        totalReceived += (decimal)receiveItems.Sum(ri => ri.Quantity);
                    }

                    if (totalReceived < totalIssued)
                    {
                        var issueDto = await GetIssueByIdAsync(issue.Id);
                        issueDto.Remarks = $"Partially received: {totalReceived}/{totalIssued}";
                        issuesWithoutReceives.Add(issueDto);
                    }
                }
            }

            return issuesWithoutReceives.OrderBy(i => i.IssueDate);
        }

        // Private helper methods
        private async Task<IssueDto> MapToIssueDto(Issue issue)
        {
            // Load related entities
            if (issue.IssuedToBattalionId.HasValue)
                issue.IssuedToBattalion = await _unitOfWork.Battalions.GetByIdAsync(issue.IssuedToBattalionId.Value);
            if (issue.IssuedToRangeId.HasValue)
                issue.IssuedToRange = await _unitOfWork.Ranges.GetByIdAsync(issue.IssuedToRangeId.Value);
            if (issue.IssuedToZilaId.HasValue)
                issue.IssuedToZila = await _unitOfWork.Zilas.GetByIdAsync(issue.IssuedToZilaId.Value);
            if (issue.IssuedToUpazilaId.HasValue)
                issue.IssuedToUpazila = await _unitOfWork.Upazilas.GetByIdAsync(issue.IssuedToUpazilaId.Value);

            // Load FromStore - this was missing!
            if (issue.FromStoreId.HasValue)
                issue.FromStore = await _unitOfWork.Stores.GetByIdAsync(issue.FromStoreId.Value);

            var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);

            var items = new List<IssueItemDto>();
            foreach (var ii in issueItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(ii.ItemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(ii.StoreId);

                items.Add(new IssueItemDto
                {
                    ItemId = ii.ItemId,
                    ItemCode = item?.Code,
                    ItemName = item?.Name ?? "Unknown",
                    StoreId = ii.StoreId,
                    StoreName = store?.Name ?? "Unknown",
                    Quantity = ii.Quantity,
                    Unit = item?.Unit,
                    Remarks = ii.Remarks
                });
            }

            return new IssueDto
            {
                Id = issue.Id,
                IssueNo = issue.IssueNo,
                IssueDate = issue.IssueDate,
                IssuedTo = issue.IssuedTo,
                IssuedToName = GetIssuedToName(issue),
                IssuedToType = issue.IssuedToType,
                Purpose = issue.Purpose,
                IssuedToBattalionId = issue.IssuedToBattalionId,
                IssuedToRangeId = issue.IssuedToRangeId,
                IssuedToZilaId = issue.IssuedToZilaId,
                IssuedToUpazilaId = issue.IssuedToUpazilaId,
                IssuedToIndividualName = issue.IssuedToIndividualName,
                IssuedToIndividualBadgeNo = issue.IssuedToIndividualBadgeNo,
                IssuedBy = issue.IssuedBy,
                ApprovedByName = issue.ApprovedBy,
                ApprovedByBadgeNo = issue.ApprovedByBadgeNo,
                Status = issue.Status,
                ApprovedBy = issue.ApprovedBy,
                ApprovedDate = issue.ApprovedDate,
                ApprovalComments = issue.ApprovalComments,
                RejectionReason = issue.RejectionReason,

                // ✅ FIX: Include VoucherNumber in mapping so Vouchers page can display them
                VoucherNumber = issue.VoucherNumber,
                VoucherGeneratedDate = issue.VoucherGeneratedDate,

                SignaturePath = issue.SignaturePath,
                SignerName = issue.SignerName,
                SignerBadgeId = issue.SignerBadgeId,
                SignedDate = issue.SignedDate,
                IsPartialIssue = issue.IsPartialIssue,
                ParentIssueId = issue.ParentIssueId,
                Remarks = issue.Remarks,
                VoucherDocumentPath = issue.VoucherDocumentPath,
                CreatedAt = issue.CreatedAt,
                CreatedBy = issue.CreatedBy,
                UpdatedAt = issue.UpdatedAt,
                UpdatedBy = issue.UpdatedBy,
                Items = items
            };
        }

        private string GetIssuedToName(Issue issue)
        {
            switch (issue.IssuedToType)
            {
                case "Battalion":
                    return issue.IssuedToBattalion?.Name ?? $"Battalion ID: {issue.IssuedToBattalionId}";
                case "Range":
                    return issue.IssuedToRange?.Name ?? $"Range ID: {issue.IssuedToRangeId}";
                case "Zila":
                    return issue.IssuedToZila?.Name ?? $"Zila ID: {issue.IssuedToZilaId}";
                case "Upazila":
                    return issue.IssuedToUpazila?.Name ?? $"Upazila ID: {issue.IssuedToUpazilaId}";
                case "Individual":
                    return $"{issue.IssuedToIndividualName} ({issue.IssuedToIndividualBadgeNo})";
                default:
                    return issue.IssuedToIndividualName ?? "Unknown";
            }
        }

        // Check and create stock alerts
        private async Task CheckAndCreateStockAlertAsync(StoreItem storeItem)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
            if (item == null) return;

            // Check if already alerted
            var existingAlert = await _unitOfWork.StockAlerts.SingleOrDefaultAsync(
                sa => sa.ItemId == storeItem.ItemId &&
                      sa.StoreId == storeItem.StoreId &&
                      sa.Status == "Active"
            );

            if (existingAlert != null) return;

            // Create alert if below minimum
            if (storeItem.Quantity <= item.MinimumStock)
            {
                var alert = new StockAlert
                {
                    ItemId = storeItem.ItemId,
                    StoreId = storeItem.StoreId,
                    AlertType = storeItem.Quantity == 0 ? "OutOfStock" : "LowStock",
                    CurrentStock = storeItem.Quantity,
                    MinimumStock = item.MinimumStock,
                    Status = "Active",
                    AlertDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _unitOfWork.StockAlerts.AddAsync(alert);
                await _unitOfWork.CompleteAsync();

                // Send notification
                await SendStockAlertNotificationAsync(alert, item);
            }
        }

        // Send notifications
        private async Task SendApprovalNotificationAsync(Issue issue)
        {
            var notification = new NotificationDto
            {
                Title = "Issue Approval Required",
                Message = $"Issue {issue.IssueNo} requires approval",
                Type = "Approval", // Use string directly
                Priority = "High", // Use string directly
                TargetRole = "Officer",
                RelatedEntity = "Issue",
                RelatedEntityId = issue.Id,
                UserId = null // ✅ Use null for role-based notifications
            };

            await _notificationService.CreateNotificationAsync(notification);
        }

        private async Task SendStockAlertNotificationAsync(StockAlert alert, Item item)
        {
            var notification = new NotificationDto
            {
                Title = $"{alert.AlertType}: {item.Name}",
                Message = $"Stock level for {item.Name} is {alert.CurrentStock} (Min: {alert.MinimumStock})",
                Type = "StockAlert", // Use string directly
                Priority = "High", // Use string directly
                TargetRole = "StoreManager",
                RelatedEntity = "StockAlert",
                RelatedEntityId = alert.Id,
                UserId = null // ✅ Use null for role-based notifications
            };

            await _notificationService.CreateNotificationAsync(notification);
        }
        public async Task<bool> SubmitForApprovalAsync(int issueId)
        {
            return await SubmitForApprovalAsync(issueId, _userContext.CurrentUserName);
        }

        // ADD this overload method for CreatePartialIssueAsync (with CreatePartialIssueDto):
        public async Task<IssueDto> CreatePartialIssueAsync(int parentIssueId, CreatePartialIssueDto dto)
        {
            return await CreatePartialIssueAsync(parentIssueId, dto.ActualQuantities, _userContext.CurrentUserName);
        }

        // ADD this overload method for AddDigitalSignatureAsync (with individual parameters):
        public async Task<bool> AddDigitalSignatureAsync(int issueId, string signatureData, string signerName, string signerBadgeId)
        {
            return await AddDigitalSignatureAsync(issueId, new DigitalSignatureDto
            {
                SignatureData = signatureData,
                SignerName = signerName,
                SignerBadgeId = signerBadgeId,
                SignerDesignation = "Commander" // Default value
            });
        }

        Task<IssueDto> IIssueService.SubmitForApprovalAsync(int issueId)
        {
            throw new NotImplementedException();
        }

        public async Task<IssueVoucherDto> GenerateIssueVoucherAsync(int issueId)
        {
            try
            {
                var issue = await GetIssueByIdAsync(issueId);
                if (issue == null)
                    throw new InvalidOperationException("Issue not found");

                // Generate voucher number if not exists
                if (string.IsNullOrEmpty(issue.VoucherNumber))
                {
                    issue.VoucherNumber = $"IV-{issue.IssueNo}";
                    var issueEntity = await _unitOfWork.Issues.GetByIdAsync(issueId);
                    issueEntity.VoucherNumber = issue.VoucherNumber;
                    _unitOfWork.Issues.Update(issueEntity);
                    await _unitOfWork.CompleteAsync();
                }

                // Generate QR code
                var qrData = $"ISSUE|{issue.VoucherNumber}|{issue.IssueNo}|{issue.IssueDate:yyyyMMdd}";
                var qrCodeImage = _barcodeService.GenerateQRCodeBase64(qrData);

                var voucher = new IssueVoucherDto
                {
                    Id = issue.Id,
                    VoucherNumber = issue.VoucherNumber,
                    IssueId = issue.Id,
                    QRCodeData = qrData,
                    QRCodeImage = qrCodeImage,
                    IssuedTo = issue.IssuedToName,
                    IssuedToDetails = GetIssuedToDetails(issue),
                    Purpose = issue.Purpose,
                    IssueDate = issue.IssueDate,
                    IssuedBy = issue.IssuedBy,
                    ApprovedBy = issue.ApprovedBy,
                    Items = issue.Items,
                    SignatureImage = issue.SignaturePath,
                    PrintedDate = DateTime.Now,
                    PrintedBy = _userContext.CurrentUserName
                };

                return voucher;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating issue voucher");
                throw;
            }
        }

        public async Task<bool> CaptureDigitalSignatureAsync(int issueId, DigitalSignatureDto signatureDto)
        {
            try
            {
                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
                if (issue == null)
                    return false;

                // Save signature image
                var fileName = $"signature_issue_{issueId}_{DateTime.Now:yyyyMMddHHmmss}.png";
                var filePath = await _fileService.SaveBase64ImageAsync(
                    signatureDto.SignatureData,
                    "signatures/issues",
                    fileName
                );

                issue.SignaturePath = filePath;
                issue.SignerName = signatureDto.SignerName;
                issue.SignerBadgeId = signatureDto.SignerBadgeId;
                issue.SignedDate = DateTime.Now;
                issue.UpdatedAt = DateTime.Now;
                issue.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Issues.Update(issue);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "Issue",
                    issueId,
                    "DigitalSignature",
                    $"Digital signature captured by {signatureDto.SignerName}",
                    _userContext.CurrentUserName
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing digital signature");
                return false;
            }
        }

        public async Task<IssueDto> ApproveWithSignatureAsync(int issueId, string approvedBy, string comments, DigitalSignatureDto signature)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // First approve the issue
                var approvedIssue = await ApproveIssueAsync(issueId, approvedBy, comments);

                // Then add signature if provided
                if (signature != null && !string.IsNullOrEmpty(signature.SignatureData))
                {
                    await CaptureDigitalSignatureAsync(issueId, signature);
                }

                // Generate voucher
                await GenerateIssueVoucherAsync(issueId);

                await _unitOfWork.CommitTransactionAsync();

                // Send notifications
                await SendApprovalNotificationsAsync(approvedIssue);

                return approvedIssue;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving issue with signature");
                throw;
            }
        }

        private async Task SendApprovalNotificationsAsync(IssueDto issue)
        {
            var notification = new NotificationDto
            {
                Title = "Issue Approved",
                Message = $"Issue {issue.IssueNo} has been approved. You can now proceed with collection.",
                Type = "success",
                UserId = issue.CreatedBy
            };

            await _notificationService.CreateNotificationAsync(notification);
        }

        public async Task<string> GenerateVoucherQRCodeAsync(int issueId)
        {
            var issue = await GetIssueByIdAsync(issueId);
            if (issue == null) return null;

            var qrData = $"ISSUE|{issue.VoucherNumber}|{issue.IssueNo}|{issue.IssueDate:yyyyMMdd}";
            return _barcodeService.GenerateQRCodeBase64(qrData);
        }

        public async Task<byte[]> PrintIssueVoucherAsync(int issueId)
        {
            // Implementation for PDF generation
            var issue = await GetIssueByIdAsync(issueId);
            // Use a PDF library like iTextSharp or QuestPDF
            return new byte[0]; // Placeholder
        }

        public async Task<IEnumerable<IssueDto>> GetIssuesByBarcodeAsync(string barcode)
        {
            var issueItems = await _unitOfWork.IssueItems.FindAsync(ii =>
                ii.Issue.IsActive &&
                ii.Issue.Items.Any(item => item.Item.Barcodes.Any(b => b.BarcodeNumber == barcode))
            );

            var issueIds = issueItems.Select(ii => ii.IssueId).Distinct();
            var issues = new List<IssueDto>();

            foreach (var issueId in issueIds)
            {
                issues.Add(await GetIssueByIdAsync(issueId));
            }

            return issues;
        }

        public async Task<Dictionary<string, object>> GetIssueAnalyticsAsync(DateTime fromDate, DateTime toDate)
        {
            var issues = await _unitOfWork.Issues.FindAsync(i =>
                i.IssueDate >= fromDate && i.IssueDate <= toDate && i.IsActive);

            var issueItems = new List<IssueItem>();
            foreach (var issue in issues)
            {
                var items = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);
                issueItems.AddRange(items);
            }

            return new Dictionary<string, object>
            {
                ["TotalIssues"] = issues.Count(),
                ["TotalValue"] = await CalculateTotalValueAsync(issues),
                ["ByStatus"] = issues.GroupBy(i => i.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() }).ToList(),
                ["ByType"] = issues.GroupBy(i => i.IssuedToType)
                    .Select(g => new { Type = g.Key, Count = g.Count() }).ToList(),
                ["TopItems"] = issueItems.GroupBy(ii => ii.ItemId)
                    .OrderByDescending(g => g.Sum(ii => ii.Quantity))
                    .Take(10)
                    .Select(g => new { ItemId = g.Key, TotalQuantity = g.Sum(ii => ii.Quantity) }).ToList()
            };
        }

        private async Task<decimal> CalculateTotalValueAsync(IEnumerable<Issue> issues)
        {
            decimal totalValue = 0;

            foreach (var issue in issues)
            {
                var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);
                foreach (var issueItem in issueItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(issueItem.ItemId);
                    if (item != null)
                    {
                        // Get last purchase price
                        var purchaseItems = await _unitOfWork.PurchaseItems
                            .FindAsync(pi => pi.ItemId == issueItem.ItemId);
                        var lastPrice = purchaseItems
                            .OrderByDescending(pi => pi.CreatedAt)
                            .FirstOrDefault()?.UnitPrice ?? 0;
                        totalValue += issueItem.Quantity * lastPrice;
                    }
                }
            }

            return totalValue;
        }
        public async Task<IssueVoucherDto> CreateIssueWithVoucherAsync(int issueId)
        {
            try
            {
                var issue = await GetIssueByIdAsync(issueId);
                if (issue == null) throw new InvalidOperationException("Issue not found");

                var voucher = new IssueVoucherDto
                {
                    Id = issue.Id,
                    IssueId = issue.Id,
                    VoucherNumber = $"IV-{DateTime.Now:yyyyMMdd}-{issue.Id:D4}",
                    IssueDate = issue.IssueDate,
                    IssuedTo = GetIssuedToDisplay(issue),
                    IssuedToDetails = GetIssuedToDetails(issue),
                    Purpose = issue.Purpose,
                    IssuedBy = issue.IssuedBy,
                    ApprovedBy = issue.ApprovedBy,
                    Items = issue.Items,
                    Department = issue.Department ?? "General",
                    AuthorizedBy = issue.ApprovedBy,
                    PrintedDate = DateTime.Now,
                    PrintedBy = _userContext.CurrentUserName
                };

                // Generate QR code data
                var qrData = new
                {
                    Type = "ISSUE_VOUCHER",
                    VoucherNo = voucher.VoucherNumber,
                    IssueId = issue.Id,
                    IssueNo = issue.IssueNo,
                    Date = issue.IssueDate.ToString("yyyyMMdd"),
                    Items = issue.Items.Count
                };

                voucher.QRCodeData = JsonSerializer.Serialize(qrData);
                voucher.QRCodeImage = _barcodeService.GenerateQRCodeBase64(voucher.QRCodeData);

                return voucher;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating issue voucher");
                throw;
            }
        }

        public async Task<bool> AddDigitalSignatureAsync(int issueId, DigitalSignatureDto signatureDto)
        {
            try
            {
                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
                if (issue == null) return false;

                // Save signature data
                issue.SignaturePath = signatureDto.SignatureData;
                issue.SignerName = signatureDto.SignerName;
                issue.SignerBadgeId = signatureDto.SignerBadgeId;
                issue.UpdatedAt = DateTime.UtcNow;
                issue.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Issues.Update(issue);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding digital signature");
                return false;
            }
        }

        public async Task<DigitalSignatureDto> GetDigitalSignatureAsync(int issueId)
        {
            var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
            if (issue == null || string.IsNullOrEmpty(issue.SignaturePath))
                return null;

            return new DigitalSignatureDto
            {
                SignerName = issue.SignerName ?? issue.ApprovedBy,
                SignerBadgeId = issue.SignerBadgeId ?? issue.ApprovedByBadgeNo,
                SignatureData = issue.SignaturePath,
                SignerDesignation = issue.ApprovedBy,
                SignedDate = issue.ApprovedDate ?? DateTime.Now
            };
        }

        public async Task<IssueScanDto> ScanItemForIssueAsync(string barcodeNumber, int? storeId)
        {
            try
            {
                var (isValid, message, barcode) = await _barcodeService.ValidateBarcodeAsync(barcodeNumber);

                if (!isValid)
                {
                    return new IssueScanDto
                    {
                        IsValid = false,
                        ValidationMessage = message,
                        BarcodeNumber = barcodeNumber
                    };
                }

                // Get item details
                var item = await _unitOfWork.Items.GetByIdAsync(barcode.ItemId);
                if (item == null)
                {
                    return new IssueScanDto
                    {
                        IsValid = false,
                        ValidationMessage = "Item not found",
                        BarcodeNumber = barcodeNumber
                    };
                }

                // Get stock level
                var stockLevel = await _storeService.GetStockLevelAsync(item.Id, storeId);

                return new IssueScanDto
                {
                    IsValid = true,
                    ItemId = item.Id,
                    ItemName = item.Name,
                    ItemCode = item.Code,
                    BarcodeNumber = barcodeNumber,
                    StoreId = storeId,
                    StoreName = stockLevel.StoreName,
                    CurrentStock = stockLevel.CurrentStock,
                    Unit = item.Unit,
                    IsLowStock = stockLevel.IsLowStock,
                    MinimumStock = item.MinimumStock,
                    ValidationMessage = stockLevel.CurrentStock <= 0 ? "Warning: No stock available" : "Valid"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning item for issue");
                return new IssueScanDto
                {
                    IsValid = false,
                    ValidationMessage = "Error scanning barcode",
                    BarcodeNumber = barcodeNumber
                };
            }
        }

        public async Task<IEnumerable<IssueScanDto>> BulkScanItemsAsync(List<string> barcodes, int? storeId)
        {
            var results = new List<IssueScanDto>();
            foreach (var barcode in barcodes.Where(b => !string.IsNullOrWhiteSpace(b)))
            {
                var result = await ScanItemForIssueAsync(barcode.Trim(), storeId);
                results.Add(result);
            }
            return results;
        }

        private async Task<string> GenerateVoucherNoAsync()
        {
            var date = DateTime.Now;
            return await Task.FromResult($"VCH/{date:yyyy}/{date:MM}/{new Random().Next(1000, 9999)}");
        }

        // After issue is approved, check and create stock alerts
        private async Task CheckAndCreateStockAlertsAsync(int issueId)
        {
            var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
            if (issue == null) return;

            var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issueId);

            foreach (var issueItem in issueItems)
            {
                var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                    si => si.ItemId == issueItem.ItemId && si.StoreId == issueItem.StoreId);

                if (storeItem != null)
                {
                    await CheckAndCreateStockAlertAsync(storeItem);
                }
            }
        }

        private string GetIssuedToDisplay(IssueDto issue)
        {
            switch (issue.IssuedToType)
            {
                case "Battalion":
                    return issue.IssuedToBattalion?.Name ?? $"Battalion ID: {issue.IssuedToBattalionId}";
                case "Range":
                    return issue.IssuedToRange?.Name ?? $"Range ID: {issue.IssuedToRangeId}";
                case "Zila":
                    return issue.IssuedToZila?.Name ?? $"Zila ID: {issue.IssuedToZilaId}";
                case "Upazila":
                    return issue.IssuedToUpazila?.Name ?? $"Upazila ID: {issue.IssuedToUpazilaId}";
                case "Individual":
                    return $"{issue.IssuedToIndividualName} ({issue.IssuedToIndividualBadgeNo})";
                default:
                    return issue.IssuedToIndividualName ?? "Unknown";
            }
        }

        private string GetIssuedToDetails(IssueDto issue)
        {
            var details = new List<string>();

            if (!string.IsNullOrEmpty(issue.IssuedToType))
                details.Add($"Type: {issue.IssuedToType}");

            if (!string.IsNullOrEmpty(issue.IssuedToName))
                details.Add($"Name: {issue.IssuedToName}");

            if (!string.IsNullOrEmpty(issue.IssuedToIndividualBadgeNo))
                details.Add($"Badge: {issue.IssuedToIndividualBadgeNo}");

            if (!string.IsNullOrEmpty(issue.DeliveryLocation))
                details.Add($"Location: {issue.DeliveryLocation}");

            return string.Join(" | ", details);
        }

        // Add missing GetIssueByTrackingCode method
        public async Task<IssueDto> GetIssueByTrackingCode(string trackingCode)
        {
            var issue = await _unitOfWork.Issues
                .FirstOrDefaultAsync(i => i.QRCode == trackingCode && i.IsActive);

            if (issue == null)
                return null;

            return await MapToIssueDto(issue);
        }

        // Fix CreateIssueVoucherAsync to use correct DTO properties
        public async Task<ServiceResult> CreateIssueVoucherAsync(IssueDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create issue without StoreId (not in Issue entity)
                var issue = new Issue
                {
                    IssueNo = await GenerateIssueNoAsync(),
                    VoucherNumber = await GenerateVoucherNoAsync(),
                    IssueDate = DateTime.Now,
                    IssuedToType = dto.IssuedToType,
                    IssuedToBattalionId = dto.IssuedToBattalionId,
                    IssuedToRangeId = dto.IssuedToRangeId,
                    IssuedToZilaId = dto.IssuedToZilaId,
                    IssuedToUpazilaId = dto.IssuedToUpazilaId,
                    IssuedToIndividualName = dto.IssuedToIndividualName,
                    IssuedToIndividualBadgeNo = dto.IssuedToIndividualBadgeNo,
                    VoucherDocumentPath = dto.VoucherDocumentPath,
                    MemoNo = dto.MemoNo,
                    MemoDate = dto.MemoDate,
                    Purpose = dto.Purpose,
                    Status = "Draft",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.Issues.AddAsync(issue);
                await _unitOfWork.CompleteAsync();

                // Add issue items with correct properties
                foreach (var itemDto in dto.Items)
                {
                    // Get store ID from the item DTO
                    var issueItem = new IssueItem
                    {
                        IssueId = issue.Id,
                        ItemId = itemDto.ItemId,
                        StoreId = itemDto.StoreId, // This is on IssueItem, not Issue
                        Quantity = itemDto.Quantity, // Use Quantity instead of RequestedQuantity
                        Remarks = itemDto.Remarks,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };

                    await _unitOfWork.IssueItems.AddAsync(issueItem);

                    // Validate stock availability
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == itemDto.StoreId && si.ItemId == itemDto.ItemId);

                    if (storeItem == null || storeItem.Quantity < itemDto.Quantity)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        var itemName = (await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId))?.Name;
                        return ServiceResult.Failure($"Insufficient stock for item: {itemName}");
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Issue",
                    issue.Id,
                    "Create",
                    $"Created issue voucher {issue.VoucherNumber}",
                    _userContext.CurrentUserName
                );

                return ServiceResult.SuccessResult("Issue voucher created successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating issue voucher");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ProcessIssueAsync(int issueId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var issue = await _unitOfWork.Issues
                    .GetAsync(i => i.Id == issueId,
                        includes: new[] { "IssueItems.Item", "IssueItems.Store" });

                if (issue == null)
                    return ServiceResult.Failure("Issue not found");

                if (issue.Status != "Approved")
                    return ServiceResult.Failure("Only approved issues can be processed");

                // Generate tracking QR code - store in existing QRCode field
                var trackingData = new
                {
                    Type = "ISSUE_VOUCHER",
                    IssueId = issue.Id,
                    VoucherNo = issue.VoucherNumber,
                    IssueDate = issue.IssueDate,
                    Timestamp = DateTime.Now.Ticks
                };

                var trackingJson = JsonSerializer.Serialize(trackingData);
                issue.QRCode = _barcodeService.GenerateQRCodeBase64(trackingJson);

                // Process each item
                foreach (var issueItem in issue.Items)
                {
                    // Update store stock
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == issueItem.StoreId && si.ItemId == issueItem.ItemId);

                    if (storeItem != null)
                    {
                        storeItem.Quantity -= issueItem.Quantity;
                        storeItem.UpdatedAt = DateTime.Now;
                        storeItem.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.StoreItems.Update(storeItem);

                        // Create stock movement
                        var movement = new StockMovement
                        {
                            StoreId = issueItem.StoreId,
                            ItemId = issueItem.ItemId,
                            MovementType = "OUT",
                            MovementDate = DateTime.Now,
                            Quantity = issueItem.Quantity,
                            ReferenceType = "ISSUE",
                            ReferenceNo = issue.VoucherNumber,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userContext.CurrentUserName,
                            IsActive = true
                        };

                        await _unitOfWork.StockMovements.AddAsync(movement);
                    }

                    // Update issue item status
                    issueItem.UpdatedAt = DateTime.Now;
                    issueItem.UpdatedBy = _userContext.CurrentUserName;
                    _unitOfWork.IssueItems.Update(issueItem);
                }

                // Update issue status
                issue.Status = "Dispatched";
                issue.UpdatedAt = DateTime.Now;
                issue.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Issues.Update(issue);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Send notification
                var notification = new NotificationDto
                {
                    Title = "Issue Voucher Dispatched",
                    Message = $"Issue voucher {issue.VoucherNumber} has been dispatched.",
                    Type = "info",
                    UserId = issue.CreatedBy
                };
                await _notificationService.CreateNotificationAsync(notification);

                return ServiceResult.SuccessResult("Issue processed and dispatched successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error processing issue");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ConfirmReceiptAsync(string trackingCode)
        {
            try
            {
                var issue = await _unitOfWork.Issues
                    .FirstOrDefaultAsync(i => i.QRCode == trackingCode && i.IsActive);

                if (issue == null)
                    return ServiceResult.Failure("Invalid tracking code");

                if (issue.Status != "Dispatched")
                    return ServiceResult.Failure("Issue is not in dispatched status");

                issue.Status = "Received";
                issue.ReceivedDate = DateTime.Now;
                issue.ReceivedBy = _userContext.CurrentUserName;
                issue.UpdatedAt = DateTime.Now;
                issue.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.Issues.Update(issue);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Issue",
                    issue.Id,
                    "Receive",
                    $"Confirmed receipt of issue voucher {issue.VoucherNumber}",
                    _userContext.CurrentUserName
                );

                return ServiceResult.SuccessResult("Receipt confirmed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming receipt");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> CreateEmergencyRequestAsync(EmergencyRequestDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create issue with emergency priority
                var issue = new Issue
                {
                    IssueNo = await GenerateIssueNoAsync() + "-EMG",
                    VoucherNumber = await GenerateVoucherNoAsync(),
                    IssueDate = DateTime.Now,
                    IssuedToType = dto.IssuedToType,
                    IssuedToBattalionId = dto.IssuedToBattalionId,
                    IssuedToRangeId = dto.IssuedToRangeId,
                    IssuedToZilaId = dto.IssuedToZilaId,
                    IssuedToUpazilaId = dto.IssuedToUpazilaId,
                    IssuedToIndividualName = dto.IssuedToIndividualName,
                    IssuedToIndividualBadgeNo = dto.IssuedToIndividualBadgeNo,
                    VoucherDocumentPath = dto.VoucherDocumentPath,
                    MemoNo = dto.MemoNo,
                    MemoDate = dto.MemoDate,
                    Purpose = $"EMERGENCY: {dto.Purpose}",
                    Status = "Emergency",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.Issues.AddAsync(issue);
                await _unitOfWork.CompleteAsync();

                // Find nearest stores with stock
                foreach (var itemDto in dto.Items)
                {
                    var nearestStores = await FindNearestStockAsync(itemDto.ItemId, itemDto.Quantity, dto.PreferredStoreId);

                    if (!nearestStores.Any())
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        var item = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                        return ServiceResult.Failure($"No stock available for item: {item?.Name}");
                    }

                    var selectedStore = nearestStores.First();

                    var issueItem = new IssueItem
                    {
                        IssueId = issue.Id,
                        ItemId = itemDto.ItemId,
                        StoreId = selectedStore.StoreId,
                        Quantity = itemDto.Quantity,
                        Remarks = $"Emergency allocation from {selectedStore.StoreName}",
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };

                    await _unitOfWork.IssueItems.AddAsync(issueItem);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Fast-track approval process
                await ProcessEmergencyApprovalAsync(issue.Id);

                // Send high-priority notifications
                await SendEmergencyNotificationsAsync(issue);

                return ServiceResult.SuccessResult("Emergency request created and fast-tracked");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating emergency request");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ProcessEmergencyApprovalAsync(int issueId)
        {
            try
            {
                var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
                if (issue == null)
                    return ServiceResult.Failure("Issue not found");

                // Auto-approve emergency requests
                issue.Status = "Approved";
                issue.ApprovedBy = "EMERGENCY_AUTO";
                issue.ApprovedDate = DateTime.Now;
                issue.ApprovalComments = "Auto-approved due to emergency";
                issue.UpdatedAt = DateTime.Now;

                _unitOfWork.Issues.Update(issue);
                await _unitOfWork.CompleteAsync();

                // Immediately process for dispatch
                await ProcessIssueAsync(issueId);

                return ServiceResult.SuccessResult("Emergency request approved and processed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing emergency approval");
                return ServiceResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<List<StoreStockInfo>> FindNearestStockAsync(int itemId, decimal quantity, int? currentStoreId)
        {
            var storeItems = await _unitOfWork.StoreItems
                .GetAllAsync(si => si.ItemId == itemId && si.Quantity >= quantity && si.IsActive,
                    includes: new[] { "Store" });

            var stockInfo = storeItems.Select(si => new StoreStockInfo
            {
                StoreId = si.StoreId,
                StoreName = si.Store.Name,
                Quantity = si.Quantity,
                Location = si.Location,
                LastUpdated = si.UpdatedAt ?? si.CreatedAt
            }).ToList();

            // Sort by priority: current store first, then by available quantity
            return stockInfo
                .OrderBy(s => s.StoreId == currentStoreId ? 0 : 1)
                .ThenByDescending(s => s.Quantity)
                .ToList();
        }

        private async Task SendEmergencyNotificationsAsync(Issue issue)
        {
            // Send to all managers and relevant personnel
            var managers = await _userManager.GetUsersInRoleAsync("StoreManager");

            foreach (var manager in managers)
            {
                var notification = new NotificationDto
                {
                    Title = "URGENT: Emergency Stock Request",
                    Message = $"Emergency issue {issue.IssueNo} requires immediate attention",
                    Type = "critical",
                    Priority = "critical",
                    UserId = manager.Id
                };
                await _notificationService.CreateNotificationAsync(notification);
            }

            // Send SMS/Email alerts if configured
            // This would integrate with SMS/Email service
        }


        public async Task<int> GetPendingIssuesCount()
        {
            return await _unitOfWork.Issues.CountAsync(i => i.IsActive && i.Status == "Pending");
        }

        public async Task<List<IssueDto>> GetRecentIssues(int count)
        {
            var issues = await GetAllIssuesAsync();
            return issues.OrderByDescending(i => i.CreatedAt).Take(count).ToList();
        }

        public async Task<int> GetPendingApprovalsCount()
        {
            return await GetPendingIssuesCount();
        }

        public async Task<IEnumerable<IssueDto>> SearchIssuesAsync(
            string searchTerm,
            string status,
            string issueType,
            DateTime? fromDate,
            DateTime? toDate)
        {
            try
            {
                var query = _unitOfWork.Issues.Query()
                    .Include(i => i.IssuedToBattalion)
                    .Include(i => i.IssuedToRange)
                    .Include(i => i.IssuedToZila)
                    .Include(i => i.IssuedToUpazila)
                    .Include(i => i.FromStore)
                    .Include(i => i.Items)
                        .ThenInclude(ii => ii.Item)
                    .Where(i => i.IsActive);

                // Apply search term filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(i =>
                        i.IssueNo.ToLower().Contains(searchTerm) ||
                        i.IssuedTo.ToLower().Contains(searchTerm) ||
                        i.IssuedToIndividualName.ToLower().Contains(searchTerm) ||
                        i.IssuedToIndividualBadgeNo.ToLower().Contains(searchTerm) ||
                        i.Purpose.ToLower().Contains(searchTerm) ||
                        i.VoucherNumber.ToLower().Contains(searchTerm)
                    );
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(i => i.Status == status);
                }

                // Apply issue type filter
                if (!string.IsNullOrWhiteSpace(issueType))
                {
                    query = query.Where(i => i.IssuedToType == issueType);
                }

                // Apply date range filter
                if (fromDate.HasValue)
                {
                    query = query.Where(i => i.IssueDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    // Add one day to include the entire end date
                    var endDate = toDate.Value.AddDays(1);
                    query = query.Where(i => i.IssueDate < endDate);
                }

                var issues = await query.OrderByDescending(i => i.IssueDate).ToListAsync();

                // Map to DTOs
                var issueDtos = new List<IssueDto>();
                foreach (var issue in issues)
                {
                    issueDtos.Add(await MapToIssueDto(issue));
                }

                return issueDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching issues");
                throw;
            }
        }

        public async Task<byte[]> ExportIssuesToExcelAsync(
            string searchTerm,
            string status,
            string issueType,
            DateTime? fromDate,
            DateTime? toDate)
        {
            try
            {
                // Get filtered issues
                var issues = await SearchIssuesAsync(searchTerm, status, issueType, fromDate, toDate);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Issues");

                    // Add headers
                    var headers = new[]
                    {
                "Issue No",
                "Voucher No",
                "Issue Date",
                "Status",
                "Type",
                "Recipient",
                "Badge No",
                "Purpose",
                "From Store",
                "Total Items",
                "Issued By",
                "Approved By",
                "Approved Date",
                "Created Date",
                "Created By"
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Add data
                    int row = 2;
                    foreach (var issue in issues)
                    {
                        worksheet.Cells[row, 1].Value = issue.IssueNo;
                        worksheet.Cells[row, 2].Value = issue.VoucherNumber;
                        worksheet.Cells[row, 3].Value = issue.IssueDate.ToString("dd-MMM-yyyy");
                        worksheet.Cells[row, 4].Value = issue.Status;
                        worksheet.Cells[row, 5].Value = issue.IssuedToType;
                        worksheet.Cells[row, 6].Value = issue.IssuedToName;
                        worksheet.Cells[row, 7].Value = issue.IssuedToIndividualBadgeNo;
                        worksheet.Cells[row, 8].Value = issue.Purpose;
                        worksheet.Cells[row, 9].Value = issue.FromStoreName;
                        worksheet.Cells[row, 10].Value = issue.Items?.Count ?? 0;
                        worksheet.Cells[row, 11].Value = issue.IssuedBy;
                        worksheet.Cells[row, 12].Value = issue.ApprovedBy;
                        worksheet.Cells[row, 13].Value = issue.ApprovedDate?.ToString("dd-MMM-yyyy");
                        worksheet.Cells[row, 14].Value = issue.CreatedAt.ToString("dd-MMM-yyyy HH:mm");
                        worksheet.Cells[row, 15].Value = issue.CreatedBy;

                        row++;
                    }

                    // Add Items Sheet
                    var itemsSheet = package.Workbook.Worksheets.Add("Issue Items");

                    // Add headers for items
                    var itemHeaders = new[]
                    {
                "Issue No",
                "Item Code",
                "Item Name",
                "Category",
                "Store",
                "Requested Qty",
                "Approved Qty",
                "Issued Qty",
                "Unit",
                "Batch No",
                "Condition",
                "Remarks"
            };

                    for (int i = 0; i < itemHeaders.Length; i++)
                    {
                        itemsSheet.Cells[1, i + 1].Value = itemHeaders[i];
                        itemsSheet.Cells[1, i + 1].Style.Font.Bold = true;
                        itemsSheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        itemsSheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    }

                    // Add items data
                    int itemRow = 2;
                    foreach (var issue in issues)
                    {
                        if (issue.Items != null)
                        {
                            foreach (var item in issue.Items)
                            {
                                itemsSheet.Cells[itemRow, 1].Value = issue.IssueNo;
                                itemsSheet.Cells[itemRow, 2].Value = item.ItemCode;
                                itemsSheet.Cells[itemRow, 3].Value = item.ItemName;
                                itemsSheet.Cells[itemRow, 4].Value = item.CategoryName;
                                itemsSheet.Cells[itemRow, 5].Value = item.StoreName;
                                itemsSheet.Cells[itemRow, 6].Value = item.RequestedQuantity;
                                itemsSheet.Cells[itemRow, 7].Value = item.ApprovedQuantity;
                                itemsSheet.Cells[itemRow, 8].Value = item.IssuedQuantity;
                                itemsSheet.Cells[itemRow, 9].Value = item.Unit;
                                itemsSheet.Cells[itemRow, 10].Value = item.BatchNumber;
                                itemsSheet.Cells[itemRow, 11].Value = item.Condition;
                                itemsSheet.Cells[itemRow, 12].Value = item.Remarks;

                                itemRow++;
                            }
                        }
                    }

                    // Auto-fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    itemsSheet.Cells[itemsSheet.Dimension.Address].AutoFitColumns();

                    // Add summary sheet
                    var summarySheet = package.Workbook.Worksheets.Add("Summary");
                    summarySheet.Cells[1, 1].Value = "Issue Summary Report";
                    summarySheet.Cells[1, 1].Style.Font.Bold = true;
                    summarySheet.Cells[1, 1].Style.Font.Size = 14;

                    summarySheet.Cells[3, 1].Value = "Report Generated:";
                    summarySheet.Cells[3, 2].Value = DateTime.Now.ToString("dd-MMM-yyyy HH:mm");

                    summarySheet.Cells[4, 1].Value = "Total Issues:";
                    summarySheet.Cells[4, 2].Value = issues.Count();

                    // Status breakdown
                    summarySheet.Cells[6, 1].Value = "Status Breakdown:";
                    summarySheet.Cells[6, 1].Style.Font.Bold = true;

                    var statusGroups = issues.GroupBy(i => i.Status)
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .ToList();

                    int summaryRow = 7;
                    foreach (var statusGroup in statusGroups)
                    {
                        summarySheet.Cells[summaryRow, 1].Value = statusGroup.Status + ":";
                        summarySheet.Cells[summaryRow, 2].Value = statusGroup.Count;
                        summaryRow++;
                    }

                    // Type breakdown
                    summarySheet.Cells[summaryRow + 1, 1].Value = "Type Breakdown:";
                    summarySheet.Cells[summaryRow + 1, 1].Style.Font.Bold = true;

                    var typeGroups = issues.GroupBy(i => i.IssuedToType)
                        .Select(g => new { Type = g.Key, Count = g.Count() })
                        .ToList();

                    summaryRow += 2;
                    foreach (var typeGroup in typeGroups)
                    {
                        summarySheet.Cells[summaryRow, 1].Value = typeGroup.Type + ":";
                        summarySheet.Cells[summaryRow, 2].Value = typeGroup.Count;
                        summaryRow++;
                    }

                    summarySheet.Cells[summarySheet.Dimension.Address].AutoFitColumns();

                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting issues to Excel");
                throw;
            }
        }

    }
}