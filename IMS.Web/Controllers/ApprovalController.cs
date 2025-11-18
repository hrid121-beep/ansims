using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class ApprovalController : Controller
    {
        private readonly IApprovalService _approvalService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ApprovalController> _logger;
        private readonly IPurchaseService _purchaseService;
        private readonly IIssueService _issueService;
        private readonly INotificationService _notificationService;
        private readonly IStockEntryService _stockEntryService;
        private readonly IPhysicalInventoryService _physicalInventoryService;

        public ApprovalController(
            IApprovalService approvalService,
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            ILogger<ApprovalController> logger,
            IPurchaseService purchaseService,
            IIssueService issueService,
            INotificationService notificationService,
            IStockEntryService stockEntryService,
            IPhysicalInventoryService physicalInventoryService)
        {
            _approvalService = approvalService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
            _purchaseService = purchaseService;
            _issueService = issueService;
            _notificationService = notificationService;
            _stockEntryService = stockEntryService;
            _physicalInventoryService = physicalInventoryService;
        }

        [HttpGet]
        [HasPermission(Permission.ViewApproval)]
        public async Task<IActionResult> Index()
        {
            var pendingApprovals = await GetAllPendingApprovals();
            return View(pendingApprovals);
        }

        [HttpGet]
        [HasPermission(Permission.ViewApproval)]
        public async Task<IActionResult> GetPendingJson()
        {
            try
            {
                var pendingApprovals = await GetAllPendingApprovals();
                return Json(pendingApprovals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending approvals JSON");
                return Json(new List<ApprovalViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingCount()
        {
            try
            {
                var pendingApprovals = await GetAllPendingApprovals();
                return Json(new { count = pendingApprovals.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending count");
                return Json(new { count = 0 });
            }
        }

        private async Task<List<ApprovalViewModel>> GetAllPendingApprovals()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));

            var approvals = await _unitOfWork.ApprovalRequests
                .GetAllAsync(a => a.Status == ApprovalStatus.Pending.ToString() && a.IsActive);

            var pendingApprovals = new List<ApprovalViewModel>();

            foreach (var approval in approvals)
            {
                bool canApprove = User.IsInRole("Admin");

                if (!canApprove)
                {
                    canApprove = await CanUserApproveAtCurrentLevel(userId, userRoles.ToList(), approval);
                }

                if (canApprove)
                {
                    // Get user name and role
                    var requestedByUser = await _userManager.FindByIdAsync(approval.RequestedBy);
                    string requestedByName = requestedByUser?.FullName ?? requestedByUser?.UserName ?? "Unknown";
                    string requestedByRole = "Unknown";

                    if (requestedByUser != null)
                    {
                        var roles = await _userManager.GetRolesAsync(requestedByUser);
                        requestedByRole = roles.FirstOrDefault() ?? "Unknown";
                    }

                    pendingApprovals.Add(new ApprovalViewModel
                    {
                        Id = approval.Id,
                        EntityType = approval.EntityType,
                        EntityId = approval.EntityId,
                        Amount = approval.Amount,
                        RequestedBy = approval.RequestedBy,
                        RequestedByName = requestedByName,
                        RequestedByRole = requestedByRole,
                        RequestedDate = approval.RequestedDate,
                        Priority = approval.Priority ?? "Normal",
                        Description = await GetEntityDescription(approval.EntityType, approval.EntityId),
                        CurrentLevel = await GetCurrentApprovalLevel(approval.Id)
                    });
                }
            }

            // Add pending Stock Entry approvals
            var pendingStockEntries = await _unitOfWork.StockEntries
                .Query()
                .Include(se => se.Items)
                .Where(se => se.Status == "Submitted" && se.IsActive)
                .ToListAsync();

            foreach (var stockEntry in pendingStockEntries)
            {
                // Check if user can approve stock entries
                bool canApprove = User.IsInRole("Admin") ||
                                 User.IsInRole("StoreManager") ||
                                 User.IsInRole("Director") ||
                                 userRoles.Contains("DDStore") ||
                                 userRoles.Contains("ADStore");

                if (canApprove)
                {
                    // Calculate total value from items
                    decimal totalValue = stockEntry.Items?.Sum(i => i.Quantity * i.UnitCost) ?? 0;

                    pendingApprovals.Add(new ApprovalViewModel
                    {
                        Id = stockEntry.Id,
                        EntityType = "STOCK_ENTRY",
                        EntityId = stockEntry.Id,
                        Amount = totalValue,
                        RequestedBy = stockEntry.SubmittedBy ?? stockEntry.CreatedBy,
                        RequestedByName = stockEntry.SubmittedBy ?? stockEntry.CreatedBy,
                        RequestedByRole = "Stock Keeper",
                        RequestedDate = stockEntry.SubmittedDate ?? stockEntry.CreatedAt,
                        Priority = totalValue > 100000 ? "High" : "Normal",
                        Description = stockEntry.EntryNo,
                        CurrentLevel = 1
                    });
                }
            }

            // Add pending Physical Inventory approvals
            var pendingInventories = await _unitOfWork.PhysicalInventories
                .Query()
                .Include(pi => pi.Store)
                .Where(pi => pi.Status == PhysicalInventoryStatus.UnderReview && pi.IsActive)
                .ToListAsync();

            foreach (var inventory in pendingInventories)
            {
                // Check if user can approve physical inventories
                bool canApprove = User.IsInRole("Admin") ||
                                 User.IsInRole("Director") ||
                                 User.IsInRole("StoreManager") ||
                                 userRoles.Contains("DDGAdmin") ||
                                 userRoles.Contains("DDStore") ||
                                 userRoles.Contains("ADStore") ||
                                 userRoles.Contains("RangeCommander") ||
                                 userRoles.Contains("BattalionCommander");

                if (canApprove)
                {
                    // Get variance value for priority
                    decimal varianceValue = Math.Abs(inventory.TotalVarianceValue ?? 0);

                    // Get initiator user details
                    var initiatorUser = await _userManager.FindByIdAsync(inventory.InitiatedBy);
                    string initiatorName = initiatorUser?.FullName ?? initiatorUser?.UserName ?? "Unknown";
                    var initiatorRoles = initiatorUser != null ? await _userManager.GetRolesAsync(initiatorUser) : new List<string>();
                    string initiatorRole = initiatorRoles.FirstOrDefault() ?? "User";

                    pendingApprovals.Add(new ApprovalViewModel
                    {
                        Id = inventory.Id,
                        EntityType = "PHYSICAL_INVENTORY",
                        EntityId = inventory.Id,
                        Amount = varianceValue,
                        RequestedBy = inventory.InitiatedBy,
                        RequestedByName = initiatorName,
                        RequestedByRole = initiatorRole,
                        RequestedDate = inventory.InitiatedDate ?? inventory.CountDate,
                        Priority = varianceValue > 50000 ? "High" : "Normal",
                        Description = inventory.ReferenceNumber ?? $"PI-{inventory.Id:D6}",
                        CurrentLevel = 1
                    });
                }
            }

            return pendingApprovals.OrderByDescending(a => a.Priority == "Critical" ? 3 : a.Priority == "High" ? 2 : 1)
                                   .ThenBy(a => a.RequestedDate)
                                   .ToList();
        }

        private async Task<bool> CanUserApproveAtCurrentLevel(string userId, List<string> userRoles, ApprovalRequest approval)
        {
            var currentStep = await _unitOfWork.ApprovalSteps
                .FirstOrDefaultAsync(s => s.ApprovalRequestId == approval.Id &&
                                         s.Status == ApprovalStatus.Pending);

            if (currentStep == null) return false;

            if (!string.IsNullOrEmpty(currentStep.AssignedTo) && currentStep.AssignedTo == userId)
                return true;

            return userRoles.Contains(currentStep.ApproverRole);
        }

        private async Task<int> GetCurrentApprovalLevel(int approvalRequestId)
        {
            var currentStep = await _unitOfWork.ApprovalSteps
                .FirstOrDefaultAsync(s => s.ApprovalRequestId == approvalRequestId &&
                                         s.Status == ApprovalStatus.Pending);

            return currentStep?.StepLevel ?? 0;
        }

        [HttpPost]
        [HasPermission(Permission.ProcessApproval)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int id, string action, string remarks, string entityType = null)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // Handle Stock Entry approvals differently (they don't use ApprovalRequest table)
                if (entityType == "STOCK_ENTRY")
                {
                    bool stockEntryResult = false;
                    string stockEntryMessage = "";

                    if (action.ToLower() == "approve")
                    {
                        stockEntryResult = await _stockEntryService.ApproveStockEntryAsync(id, remarks);
                        stockEntryMessage = stockEntryResult ? "Stock entry approved successfully" : "Failed to approve stock entry";
                    }
                    else if (action.ToLower() == "reject")
                    {
                        if (string.IsNullOrWhiteSpace(remarks))
                        {
                            return Json(new { success = false, message = "Rejection reason is required" });
                        }
                        stockEntryResult = await _stockEntryService.RejectStockEntryAsync(id, remarks);
                        stockEntryMessage = stockEntryResult ? "Stock entry rejected" : "Failed to reject stock entry";
                    }

                    return Json(new { success = stockEntryResult, message = stockEntryMessage });
                }

                // Handle Physical Inventory approvals (they don't use ApprovalRequest table)
                if (entityType == "PHYSICAL_INVENTORY")
                {
                    try
                    {
                        if (action.ToLower() == "approve")
                        {
                            await _physicalInventoryService.ApprovePhysicalInventoryAsync(
                                id,
                                User.Identity.Name,
                                remarks ?? "Approved from Approval Center",
                                autoAdjust: false);

                            return Json(new {
                                success = true,
                                message = "Physical inventory approved successfully",
                                redirectUrl = Url.Action("Review", "PhysicalInventory", new { id })
                            });
                        }
                        else if (action.ToLower() == "reject")
                        {
                            if (string.IsNullOrWhiteSpace(remarks))
                            {
                                return Json(new { success = false, message = "Rejection reason is required" });
                            }

                            await _physicalInventoryService.RejectPhysicalInventoryAsync(
                                id,
                                User.Identity.Name,
                                remarks);

                            return Json(new { success = true, message = "Physical inventory rejected" });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing physical inventory approval {id}");
                        return Json(new { success = false, message = ex.Message });
                    }
                }

                // Handle standard ApprovalRequest-based approvals
                var approvalRequest = await _unitOfWork.ApprovalRequests.GetByIdAsync(id);
                if (approvalRequest == null)
                {
                    return Json(new { success = false, message = "Approval request not found" });
                }

                bool result = false;
                string message = "";

                // Admin can approve/reject anything directly without level checks
                if (User.IsInRole("Admin"))
                {
                    if (action.ToLower() == "approve")
                    {
                        // Admin bypass: Direct approval
                        approvalRequest.Status = ApprovalStatus.Approved.ToString();
                        approvalRequest.ApprovedBy = userId;
                        approvalRequest.ApprovedDate = DateTime.Now;
                        approvalRequest.Remarks = remarks ?? "Approved by Admin";

                        _unitOfWork.ApprovalRequests.Update(approvalRequest);
                        await _unitOfWork.CompleteAsync();

                        // Log admin bypass approval
                        _logger.LogInformation($"Admin {User.Identity.Name} directly approved {approvalRequest.EntityType} #{approvalRequest.EntityId} (bypassing approval levels)");

                        await HandlePostApprovalActions(approvalRequest);

                        result = true;
                        message = "Approved successfully by Admin";
                    }
                    else if (action.ToLower() == "reject")
                    {
                        // Admin bypass: Direct rejection
                        approvalRequest.Status = ApprovalStatus.Rejected.ToString();
                        approvalRequest.RejectedBy = userId;
                        approvalRequest.RejectedDate = DateTime.Now;
                        approvalRequest.Remarks = remarks ?? "Rejected by Admin";

                        _unitOfWork.ApprovalRequests.Update(approvalRequest);
                        await _unitOfWork.CompleteAsync();

                        // Log admin bypass rejection
                        _logger.LogInformation($"Admin {User.Identity.Name} directly rejected {approvalRequest.EntityType} #{approvalRequest.EntityId} (bypassing approval levels)");

                        result = true;
                        message = "Rejected successfully by Admin";
                    }
                }
                else
                {
                    // Non-Admin: Use standard approval service with level checks
                    var dto = new ApprovalActionDto
                    {
                        EntityType = approvalRequest.EntityType,
                        EntityId = approvalRequest.EntityId,
                        ApprovedBy = userId,
                        ApprovedDate = DateTime.Now,
                        Remarks = remarks
                    };

                    if (action.ToLower() == "approve")
                    {
                        result = await _approvalService.ApproveRequestAsync(dto);
                        message = result ? "Approved successfully" : "Failed to approve";

                        if (result)
                        {
                            await HandlePostApprovalActions(approvalRequest);
                        }
                    }
                    else if (action.ToLower() == "reject")
                    {
                        result = await _approvalService.RejectRequestAsync(dto);
                        message = result ? "Rejected successfully" : "Failed to reject";
                    }
                }

                return Json(new { success = result, message = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing approval {id}");
                return Json(new { success = false, message = "Error processing approval: " + ex.Message });
            }
        }

        private async Task HandlePostApprovalActions(ApprovalRequest approval)
        {
            var hasMoreLevels = await _unitOfWork.ApprovalSteps
                .ExistsAsync(s => s.ApprovalRequestId == approval.Id &&
                              s.Status == ApprovalStatus.Awaiting);

            if (!hasMoreLevels)
            {
                switch (approval.EntityType.ToUpper())
                {
                    case "PURCHASE":
                        await HandlePurchaseFullyApproved(approval.EntityId);
                        break;
                    case "ISSUE":
                        await HandleIssueFullyApproved(approval.EntityId);
                        break;
                    case "REQUISITION":
                        await HandleRequisitionFullyApproved(approval.EntityId);
                        break;
                    case "ALLOTMENT_LETTER":
                        await HandleAllotmentLetterFullyApproved(approval.EntityId, approval);
                        break;
                    case "TRANSFER":
                        await HandleTransferFullyApproved(approval.EntityId);
                        break;
                    case "RETURN":
                        await HandleReturnFullyApproved(approval.EntityId);
                        break;
                    case "STOCK_ADJUSTMENT":
                        await HandleStockAdjustmentFullyApproved(approval.EntityId);
                        break;
                }
            }
        }

        private async Task HandlePurchaseFullyApproved(int purchaseId)
        {
            var purchase = await _unitOfWork.Purchases.GetByIdAsync(purchaseId);
            if (purchase != null)
            {
                purchase.Status = "Approved";
                _unitOfWork.Purchases.Update(purchase);
                await _unitOfWork.CompleteAsync();

                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = purchase.CreatedBy,
                    Title = "Purchase Order Approved ✅",
                    Message = $"Purchase Order #{purchase.PurchaseOrderNo} has been fully approved.",
                    Type = "Success"
                });
            }
        }

        private async Task HandleIssueFullyApproved(int issueId)
        {
            var issue = await _unitOfWork.Issues.GetByIdAsync(issueId);
            if (issue != null)
            {
                try
                {
                    // ✅ 1. Generate Voucher FIRST (before changing status)
                    var voucher = await _issueService.GenerateIssueVoucherAsync(issueId);

                    _logger.LogInformation($"✅ Voucher {voucher.VoucherNumber} generated successfully for Issue #{issue.IssueNo}");

                    // ✅ 2. Refresh the issue entity to get the updated VoucherNumber
                    issue = await _unitOfWork.Issues.GetByIdAsync(issueId);

                    // ✅ 3. Update Status to Approved
                    issue.Status = "Approved";
                    issue.ApprovedDate = DateTime.Now;
                    issue.VoucherGeneratedDate = DateTime.Now;

                    _unitOfWork.Issues.Update(issue);
                    await _unitOfWork.CompleteAsync();

                    _logger.LogInformation($"✅ Issue #{issue.IssueNo} fully approved with Voucher: {voucher.VoucherNumber}");

                    // ✅ 4. Send notification to creator
                    await _notificationService.SendNotificationAsync(new NotificationDto
                    {
                        UserId = issue.CreatedBy,
                        Title = "Issue Approved ✅",
                        Message = $"Issue #{issue.IssueNo} has been approved. Voucher: {voucher.VoucherNumber}",
                        Type = "Success"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"❌ CRITICAL: Failed to approve Issue #{issueId}. " +
                                         $"Error: {ex.Message}. " +
                                         $"Issue remains in current status. Please retry approval.");

                    // Update status anyway but log the voucher generation failure
                    issue.Status = "Approved";
                    issue.ApprovedDate = DateTime.Now;
                    _unitOfWork.Issues.Update(issue);
                    await _unitOfWork.CompleteAsync();

                    _logger.LogWarning($"⚠️ Issue #{issue.IssueNo} marked as Approved but voucher not generated. User can manually generate from Issue Index page.");
                }
            }
        }

        private async Task HandleRequisitionFullyApproved(int requisitionId)
        {
            var requisition = await _unitOfWork.Requisitions.GetByIdAsync(requisitionId);
            if (requisition != null)
            {
                requisition.Status = "Approved";
                requisition.ApprovedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                requisition.ApprovedDate = DateTime.Now;
                _unitOfWork.Requisitions.Update(requisition);
                await _unitOfWork.CompleteAsync();

                // If AutoConvertToPO is enabled, create Purchase Order
                if (requisition.AutoConvertToPO)
                {
                    try
                    {
                        // TODO: Call RequisitionService to convert to PO
                        // var purchaseOrder = await _requisitionService.ConvertToPurchaseOrderAsync(requisition);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error auto-converting requisition {requisitionId} to PO");
                    }
                }
            }
        }

        private async Task HandleAllotmentLetterFullyApproved(int allotmentId, ApprovalRequest approvalRequest)
        {
            try
            {
                var allotment = await _unitOfWork.AllotmentLetters.GetByIdAsync(allotmentId);
                if (allotment != null)
                {
                    allotment.Status = "Active";
                    allotment.ApprovedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    allotment.ApprovedDate = DateTime.Now;
                    _unitOfWork.AllotmentLetters.Update(allotment);

                    try
                    {
                        await _unitOfWork.CompleteAsync();
                    }
                    catch (Exception saveEx)
                    {
                        _logger.LogWarning(saveEx, $"Entity already saved for Allotment {allotmentId}. Status update may have been completed by approval service.");
                    }

                    // Send notification to both creator and requester (if different)
                    var usersToNotify = new HashSet<string>();

                    if (!string.IsNullOrEmpty(allotment.CreatedBy))
                        usersToNotify.Add(allotment.CreatedBy);

                    if (!string.IsNullOrEmpty(approvalRequest?.RequestedBy))
                        usersToNotify.Add(approvalRequest.RequestedBy);

                    foreach (var userId in usersToNotify)
                    {
                        try
                        {
                            await _notificationService.SendNotificationAsync(new NotificationDto
                            {
                                UserId = userId,
                                Title = "Allotment Letter Approved ✅",
                                Message = $"Allotment Letter #{allotment.AllotmentNo} has been approved and is now active.",
                                Type = "Success"
                            });
                        }
                        catch (Exception notifEx)
                        {
                            _logger.LogError(notifEx, $"Failed to send notification to user {userId} for Allotment {allotmentId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in HandleAllotmentLetterFullyApproved for Allotment {allotmentId}");
                // Don't throw - allotment was already approved by approval service
            }
        }

        private async Task HandleTransferFullyApproved(int transferId)
        {
            var transfer = await _unitOfWork.Transfers
                .GetAsync(t => t.Id == transferId,
                    includes: new[] { "FromStore", "ToStore" });

            if (transfer != null)
            {
                transfer.Status = "Approved";
                transfer.UpdatedAt = DateTime.Now;
                transfer.UpdatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                _unitOfWork.Transfers.Update(transfer);
                await _unitOfWork.CompleteAsync();

                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = transfer.CreatedBy,
                    Title = "Transfer Approved ✅",
                    Message = $"Transfer {transfer.TransferNo} from {transfer.FromStore?.Name} to {transfer.ToStore?.Name} has been approved and ready for dispatch.",
                    Type = "Success"
                });
            }
        }

        private async Task HandleReturnFullyApproved(int returnId)
        {
            var returnEntity = await _unitOfWork.Returns.Query()
                .Include(r => r.Item)
                .Include(r => r.Store)
                .FirstOrDefaultAsync(r => r.Id == returnId);

            if (returnEntity != null)
            {
                returnEntity.Status = ReturnStatus.Approved;
                returnEntity.ApprovedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                returnEntity.ApprovedDate = DateTime.Now;
                returnEntity.UpdatedAt = DateTime.Now;
                returnEntity.UpdatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _unitOfWork.Returns.Update(returnEntity);
                await _unitOfWork.CompleteAsync();

                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = returnEntity.CreatedBy,
                    Title = "Return Approved ✅",
                    Message = $"Return {returnEntity.ReturnNo} has been approved and is now ready for processing.",
                    Type = "Success"
                });
            }
        }

        private async Task HandleStockAdjustmentFullyApproved(int adjustmentId)
        {
            try
            {
                var adjustment = await _unitOfWork.StockAdjustments.GetByIdAsync(adjustmentId);
                if (adjustment == null)
                {
                    _logger.LogWarning($"Stock Adjustment {adjustmentId} not found");
                    return;
                }

                // ✅ Validate required fields
                if (!adjustment.StoreId.HasValue || adjustment.StoreId.Value == 0)
                {
                    _logger.LogWarning($"Stock Adjustment {adjustmentId} has no StoreId, skipping stock update");
                    return;
                }

                if (!adjustment.NewQuantity.HasValue)
                {
                    _logger.LogWarning($"Stock Adjustment {adjustmentId} has no NewQuantity, skipping stock update");
                    return;
                }

                // ✅ NOTE: Status already updated in ApprovalService, no need to update again
                // Just update the stock levels

                // ✅ UPDATE STOCK - Apply the adjustment
                var storeItem = await _unitOfWork.StoreItems
                    .FirstOrDefaultAsync(si => si.ItemId == adjustment.ItemId && si.StoreId == adjustment.StoreId.Value);

                if (storeItem != null)
                {
                    // Update the stock quantity
                    storeItem.Quantity = adjustment.NewQuantity.Value;
                    storeItem.UpdatedAt = DateTime.Now;
                    storeItem.UpdatedBy = adjustment.ApprovedBy;
                    _unitOfWork.StoreItems.Update(storeItem);
                }
                else
                {
                    // Create new store item if it doesn't exist
                    storeItem = new StoreItem
                    {
                        ItemId = adjustment.ItemId,
                        StoreId = adjustment.StoreId.Value,
                        Quantity = adjustment.NewQuantity.Value,
                        Status = ItemStatus.Available,
                        CreatedAt = DateTime.Now,
                        CreatedBy = adjustment.ApprovedBy
                    };
                    await _unitOfWork.StoreItems.AddAsync(storeItem);
                }

                await _unitOfWork.CompleteAsync();

                // ✅ Send notification - don't fail if this errors
                try
                {
                    await _notificationService.SendNotificationAsync(new NotificationDto
                    {
                        UserId = adjustment.CreatedBy,
                        Title = "Stock Adjustment Approved ✅",
                        Message = $"Stock Adjustment #{adjustment.AdjustmentNo} has been fully approved and stock updated.",
                        Type = "Success"
                    });
                }
                catch (Exception notifEx)
                {
                    _logger.LogError(notifEx, $"Failed to send notification for Stock Adjustment {adjustmentId}");
                    // Don't throw - notification failure shouldn't break approval
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in HandleStockAdjustmentFullyApproved for adjustment {adjustmentId}");
                // ✅ DON'T THROW - approval already succeeded, just log the error
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewApproval)]
        public async Task<IActionResult> InspectPurchase(int purchaseId)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(purchaseId);
                if (purchase == null)
                {
                    TempData["Error"] = "Purchase order not found";
                    return RedirectToAction(nameof(Index));
                }

                var model = new PurchaseInspectionViewModel
                {
                    PurchaseId = purchase.Id,
                    PurchaseOrderNo = purchase.PurchaseOrderNo,
                    VendorName = purchase.VendorName,
                    TotalAmount = purchase.TotalAmount,
                    InspectionDate = DateTime.Now,
                    Items = purchase.Items.Select(item => new InspectionItemViewModel
                    {
                        ItemId = item.ItemId,
                        ItemCode = item.ItemCode,
                        ItemName = item.ItemName,
                        Quantity = item.ReceivedQuantity ?? item.Quantity,
                        Condition = "Serviceable",
                        ApprovedForTransfer = true
                    }).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inspection page");
                TempData["Error"] = "Failed to load inspection form";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ProcessApproval)]
        public async Task<IActionResult> SubmitInspection(PurchaseInspectionViewModel model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var approvalRequest = await _unitOfWork.ApprovalRequests
                    .FirstOrDefaultAsync(a => a.EntityType == "PURCHASE" &&
                                             a.EntityId == model.PurchaseId &&
                                             a.Status == ApprovalStatus.Pending.ToString());

                if (approvalRequest == null)
                {
                    return Json(new { success = false, message = "Approval request not found" });
                }

                var serviceableCount = model.Items.Count(i => i.ApprovedForTransfer);
                var unserviceableCount = model.Items.Count - serviceableCount;

                var remarks = $"Inspection completed by {User.Identity.Name}. " +
                             $"Serviceable: {serviceableCount}, Unserviceable: {unserviceableCount}. " +
                             $"Remarks: {model.InspectorRemarks}";

                var dto = new ApprovalActionDto
                {
                    EntityType = "PURCHASE",
                    EntityId = model.PurchaseId,
                    ApprovedBy = userId,
                    ApprovedDate = DateTime.Now,
                    Remarks = remarks
                };

                var result = await _approvalService.ApproveRequestAsync(dto);

                if (result)
                {
                    TempData["Success"] = $"Inspection completed! {serviceableCount} items approved for transfer.";
                }

                return Json(new { success = result, message = result ? "Inspection submitted successfully" : "Failed to submit inspection" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting inspection");
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewApproval)]
        public async Task<IActionResult> History()
        {
            var history = await _unitOfWork.ApprovalHistories
                .Query()
                .OrderByDescending(h => h.ActionDate)
                .Take(100)
                .Select(h => new ApprovalHistoryDto
                {
                    Action = h.Action,
                    ProcessedBy = h.ActionBy,
                    ProcessedDate = h.ActionDate,
                    Comments = h.Comments,
                    EntityType = h.EntityType,
                    EntityReference = $"{h.EntityType} #{h.EntityId}",
                    Amount = 0
                })
                .ToListAsync();

            return View(history);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,DirectorGeneral")]
        public IActionResult Configure()
        {
            // Redirect to new Approval Settings module
            return RedirectToAction("Index", "ApprovalSettings");
        }

        [HttpGet]
        [HasPermission(Permission.ViewApproval)]
        public async Task<IActionResult> PendingAdjustments()
        {
            var pendingApprovals = await GetAllPendingApprovals();
            var adjustmentApprovals = pendingApprovals
                .Where(a => a.EntityType.ToUpper() == "STOCK_ADJUSTMENT")
                .ToList();

            ViewBag.Title = "Pending Adjustment Approvals";
            ViewBag.EntityType = "Stock Adjustments";
            ViewBag.TotalPending = adjustmentApprovals.Count;

            return View("Index", adjustmentApprovals);
        }

        [HttpGet]
        [HasPermission(Permission.ViewApproval)]
        public async Task<IActionResult> PendingIssues()
        {
            var pendingApprovals = await GetAllPendingApprovals();
            var issueApprovals = pendingApprovals
                .Where(a => a.EntityType.ToUpper() == "ISSUE")
                .ToList();

            ViewBag.Title = "Pending Issue Approvals";
            ViewBag.EntityType = "Issues";
            ViewBag.TotalPending = issueApprovals.Count;

            return View("Index", issueApprovals);
        }

        private async Task<string> GetEntityDescription(string entityType, int entityId)
        {
            try
            {
                return entityType?.ToUpper() switch
                {
                    "ISSUE" => (await _unitOfWork.Issues.GetByIdAsync(entityId))?.IssueNo ?? $"ISS-{entityId:D6}",
                    "PURCHASE" => (await _unitOfWork.Purchases.GetByIdAsync(entityId))?.PurchaseOrderNo ?? $"PO-{entityId:D6}",
                    "STOCK_ADJUSTMENT" => (await _unitOfWork.StockAdjustments.GetByIdAsync(entityId))?.AdjustmentNo ?? $"ADJ-{entityId:D6}",
                    "REQUISITION" => (await _unitOfWork.Requisitions.GetByIdAsync(entityId))?.RequisitionNumber ?? $"REQ-{entityId:D6}",
                    "ALLOTMENT_LETTER" => (await _unitOfWork.AllotmentLetters.GetByIdAsync(entityId))?.AllotmentNo ?? $"ALL-{entityId:D6}",
                    "TRANSFER" => (await _unitOfWork.Transfers.GetByIdAsync(entityId))?.TransferNo ?? $"TRF-{entityId:D6}",
                    "RETURN" => (await _unitOfWork.Returns.GetByIdAsync(entityId))?.ReturnNo ?? $"RET-{entityId:D6}",
                    "PHYSICAL_INVENTORY" => (await _unitOfWork.PhysicalInventories.GetByIdAsync(entityId))?.ReferenceNumber ?? $"PI-{entityId:D6}",
                    _ => $"{entityType} #{entityId}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting description for {entityType} {entityId}");
                return $"{entityType} #{entityId}";
            }
        }

        // DEBUG: Test Physical Inventory loading
        [HttpGet]
        public async Task<IActionResult> DebugPhysicalInventories()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));

            // Get all Physical Inventories in UnderReview status
            var allInventories = await _unitOfWork.PhysicalInventories
                .Query()
                .Include(pi => pi.Store)
                .Where(pi => pi.Status == PhysicalInventoryStatus.UnderReview)
                .ToListAsync();

            // Test the actual GetAllPendingApprovals method
            var pendingApprovals = await GetAllPendingApprovals();
            var physicalInventoryApprovals = pendingApprovals
                .Where(a => a.EntityType == "PHYSICAL_INVENTORY")
                .ToList();

            var result = new
            {
                CurrentUser = User.Identity.Name,
                UserId = userId,
                UserRoles = userRoles.ToList(),
                TotalUnderReview = allInventories.Count,
                AllInventories = allInventories.Select(inv => new
                {
                    inv.Id,
                    inv.ReferenceNumber,
                    inv.Status,
                    StatusValue = (int)inv.Status,
                    inv.IsActive,
                    inv.StoreId,
                    StoreName = inv.Store?.Name,
                    inv.InitiatedBy,
                    inv.InitiatedDate,
                    inv.TotalVarianceValue
                }).ToList(),
                CanApproveCheck = new
                {
                    IsAdmin = User.IsInRole("Admin"),
                    IsDirector = User.IsInRole("Director"),
                    IsStoreManager = User.IsInRole("StoreManager"),
                    IsDDGAdmin = userRoles.Contains("DDGAdmin"),
                    IsDDStore = userRoles.Contains("DDStore"),
                    IsADStore = userRoles.Contains("ADStore")
                },
                // NEW: Check what GetAllPendingApprovals returns
                TotalPendingApprovals = pendingApprovals.Count,
                PhysicalInventoryApprovalsCount = physicalInventoryApprovals.Count,
                PhysicalInventoryApprovals = physicalInventoryApprovals.Select(a => new
                {
                    a.Id,
                    a.EntityType,
                    a.Description,
                    a.Amount,
                    a.RequestedBy,
                    a.Priority
                }).ToList(),
                AllPendingByType = pendingApprovals.GroupBy(a => a.EntityType)
                    .Select(g => new { EntityType = g.Key, Count = g.Count() })
                    .ToList()
            };

            return Json(result);
        }
    }
}