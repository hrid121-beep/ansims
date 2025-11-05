using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly IVendorService _vendorService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly INotificationService _notificationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IWebHostEnvironment _environment;
        private readonly IRequisitionService _requisitionService;
        private readonly UserManager<User> _userManager;
        private readonly ITransferService _transferService;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(
            IPurchaseService purchaseService,
            IVendorService vendorService,
            IItemService itemService,
            IStoreService storeService,
            INotificationService notificationService,
            IActivityLogService activityLogService,
            IWebHostEnvironment environment,
            UserManager<User> userManager,
            IRequisitionService requisitionService,
            ITransferService transferService,
            ILogger<PurchaseController> logger)
        {
            _purchaseService = purchaseService;
            _vendorService = vendorService;
            _itemService = itemService;
            _storeService = storeService;
            _notificationService = notificationService;
            _activityLogService = activityLogService;
            _environment = environment;
            _userManager = userManager;
            _requisitionService = requisitionService;
            _transferService = transferService;
            _logger = logger;
        }

        [HasPermission(Permission.CreatePurchase)]
        public async Task<IActionResult> FromRequisition()
        {
            try
            {
                var allRequisitions = await _requisitionService.GetAllRequisitionsAsync();

                var approvedRequisitions = allRequisitions
                    .Where(r => r.Status == "Approved" &&
                               r.FulfillmentStatus != "PO Created" &&
                               !r.PurchaseOrderId.HasValue)
                    .ToList();

                return View(approvedRequisitions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading requisitions for FromRequisition page");
                TempData["Error"] = "Failed to load requisitions. Please try again.";
                return View(new List<RequisitionDto>());
            }
        }

        [HttpPost]
        [HasPermission(Permission.CreatePurchase)]
        public async Task<IActionResult> ConvertFromRequisition(int requisitionId)
        {
            try
            {
                var requisition = await _requisitionService.GetRequisitionByIdAsync(requisitionId);
                if (requisition == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Requisition not found!"
                    });
                }

                if (requisition.Status != "Approved")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Only approved requisitions can be converted to purchase orders!"
                    });
                }

                if (requisition.PurchaseOrderId.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"This requisition has already been converted to Purchase Order!"
                    });
                }

                if (requisition.Items == null || !requisition.Items.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Requisition has no items to convert!"
                    });
                }

                var purchaseDto = new PurchaseDto
                {
                    PurchaseDate = DateTime.Now,
                    ExpectedDeliveryDate = requisition.RequiredByDate,
                    Status = "Draft",
                    CreatedBy = User.Identity.Name,
                    Remarks = $"Created from Requisition #{requisition.RequisitionNumber}\n" +
                             $"Department: {requisition.Department}\n" +
                             $"Purpose: {requisition.Purpose}\n" +
                             $"Priority: {requisition.Priority}",
                    TotalAmount = requisition.EstimatedValue,
                    Items = requisition.Items.Select(item => new PurchaseItemDto
                    {
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        Quantity = item.ApprovedQuantity ?? item.RequestedQuantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice,
                        StoreId = requisition.ToStoreId ?? requisition.FromStoreId
                    }).ToList()
                };

                var result = await _purchaseService.CreatePurchaseAsync(purchaseDto);

                try
                {
                    var updatedRequisition = await _requisitionService.UpdatePurchaseOrderReferenceAsync(
                        requisitionId,
                        result.Id,
                        result.PurchaseOrderNo
                    );

                    if (updatedRequisition == null)
                    {
                        _logger.LogWarning($"Failed to update requisition {requisitionId} with PO reference");
                    }
                }
                catch (Exception updateEx)
                {
                    _logger.LogWarning(updateEx, $"Failed to update requisition {requisitionId} with PO reference");
                }

                await _activityLogService.LogActivityAsync(
                    "Purchase",
                    result.Id,
                    "CreateFromRequisition",
                    $"Created purchase order {result.PurchaseOrderNo} from requisition {requisition.RequisitionNumber}",
                    User.Identity.Name
                );

                TempData["Success"] = $"Purchase Order #{result.PurchaseOrderNo} created from Requisition #{requisition.RequisitionNumber} successfully! ✅";

                return Json(new
                {
                    success = true,
                    purchaseId = result.Id,
                    purchaseOrderNo = result.PurchaseOrderNo,
                    message = $"Purchase Order {result.PurchaseOrderNo} created successfully!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to create purchase order. Please try again."
                });
            }
        }

        [HasPermission(Permission.ViewPurchase)]
        public async Task<IActionResult> Index(string status = null, DateTime? fromDate = null,
            DateTime? toDate = null, int? vendorId = null)
        {
            try
            {
                var purchases = await _purchaseService.GetAllPurchasesAsync();

                if (!string.IsNullOrEmpty(status))
                    purchases = purchases.Where(p => p.Status == status);

                if (fromDate.HasValue)
                    purchases = purchases.Where(p => p.PurchaseDate >= fromDate.Value);

                if (toDate.HasValue)
                    purchases = purchases.Where(p => p.PurchaseDate <= toDate.Value);

                if (vendorId.HasValue)
                    purchases = purchases.Where(p => p.VendorId == vendorId);

                ViewBag.TotalPurchases = purchases.Count();
                ViewBag.PendingApprovals = purchases.Count(p => p.Status == "Pending");
                ViewBag.TotalAmount = purchases.Sum(p => p.TotalAmount).ToString("N2");
                ViewBag.RejectedCount = purchases.Count(p => p.Status == "Rejected");

                ViewBag.Status = status;
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                ViewBag.VendorId = vendorId;
                ViewBag.Vendors = new SelectList(await _vendorService.GetAllVendorsAsync(), "Id", "Name");

                return View(purchases);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load purchase order list. Please try again.";
                return View(new List<PurchaseDto>());
            }
        }

        [HasPermission(Permission.CreatePurchase)]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.PurchaseOrderNo = await _purchaseService.GeneratePurchaseOrderNoAsync();
                await LoadViewBagData();

                var model = new PurchaseDto
                {
                    PurchaseDate = DateTime.Now,
                    ExpectedDeliveryDate = DateTime.Now.AddDays(7),
                    Items = new List<PurchaseItemDto>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load new purchase order form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreatePurchase)]
        public async Task<IActionResult> Create(PurchaseDto purchaseDto, List<IFormFile> attachments, string action)
        {
            if (purchaseDto.Items == null || !purchaseDto.Items.Any())
            {
                TempData["Error"] = "Please add at least one item.";
                await LoadViewBagData();
                return View(purchaseDto);
            }

            if (!purchaseDto.IsMarketplacePurchase && !purchaseDto.VendorId.HasValue)
            {
                TempData["Error"] = "Please select a vendor.";
                await LoadViewBagData();
                return View(purchaseDto);
            }

            if (purchaseDto.PurchaseDate > DateTime.Now.AddDays(1))
            {
                TempData["Warning"] = "Purchase date cannot be in the future.";
                await LoadViewBagData();
                return View(purchaseDto);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (attachments != null && attachments.Any())
                    {
                        if (attachments.Count > 5)
                        {
                            TempData["Warning"] = "Maximum 5 files can be uploaded.";
                            await LoadViewBagData();
                            return View(purchaseDto);
                        }

                        foreach (var file in attachments)
                        {
                            if (file.Length > 5 * 1024 * 1024)
                            {
                                TempData["Error"] = $"File '{file.FileName}' size exceeds 5 MB. Please upload a smaller file.";
                                await LoadViewBagData();
                                return View(purchaseDto);
                            }
                        }

                        purchaseDto.Attachments = await SaveAttachments(attachments);
                    }

                    if (purchaseDto.Items != null && purchaseDto.Items.Any())
                    {
                        purchaseDto.TotalAmount = purchaseDto.Items.Sum(i => i.TotalPrice);

                        if (purchaseDto.TotalAmount <= 0)
                        {
                            TempData["Error"] = "Total amount cannot be zero. Please check item prices.";
                            await LoadViewBagData();
                            return View(purchaseDto);
                        }
                    }

                    // Set status based on action button clicked
                    purchaseDto.Status = "Draft"; // Always save as Draft first
                    purchaseDto.CreatedBy = User.Identity.Name;

                    var result = await _purchaseService.CreatePurchaseAsync(purchaseDto);

                    await _activityLogService.LogActivityAsync(
                        "Purchase",
                        result.Id,
                        "Create",
                        $"Created purchase order {result.PurchaseOrderNo}",
                        User.Identity.Name
                    );

                    // If user clicked "Submit for Approval", submit it
                    if (action == "submit")
                    {
                        var submitResult = await _purchaseService.SubmitForApprovalAsync(result.Id);
                        if (submitResult.Success)
                        {
                            TempData["Success"] = $"Purchase Order #{result.PurchaseOrderNo} successfully created and submitted for approval! ✅";
                        }
                        else
                        {
                            TempData["Warning"] = $"Purchase Order created but approval submission failed: {submitResult.Message}";
                        }
                    }
                    else
                    {
                        TempData["Success"] = $"Purchase Order #{result.PurchaseOrderNo} saved as draft! 📝";
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = $"⚠️ {ex.Message}";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Failed to create purchase order. Please contact IT support.";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Form has some issues: " + string.Join(", ", errors);
            }

            await LoadViewBagData();
            return View(purchaseDto);
        }

        [HasPermission(Permission.UpdatePurchase)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                {
                    TempData["Error"] = "Purchase order not found! ❌";
                    return RedirectToAction(nameof(Index));
                }

                if (purchase.Status != "Draft")
                {
                    TempData["Warning"] = "Only draft purchase orders can be edited! ⚠️";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await LoadViewBagData();
                TempData["Info"] = "Purchase order is in edit mode. Make changes carefully.";
                return View(purchase);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load purchase order.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdatePurchase)]
        public async Task<IActionResult> Edit(int id, PurchaseDto purchaseDto)
        {
            if (id != purchaseDto.Id)
            {
                TempData["Error"] = "Invalid request! Purchase order ID does not match.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (purchaseDto.Items == null || !purchaseDto.Items.Any())
                    {
                        TempData["Error"] = "Purchase order must have at least one item.";
                        await LoadViewBagData();
                        return View(purchaseDto);
                    }

                    purchaseDto.TotalAmount = purchaseDto.Items.Sum(i => i.TotalPrice);

                    if (purchaseDto.TotalAmount <= 0)
                    {
                        TempData["Error"] = "Total amount must be greater than zero.";
                        await LoadViewBagData();
                        return View(purchaseDto);
                    }

                    purchaseDto.UpdatedBy = User.Identity.Name;
                    await _purchaseService.UpdatePurchaseAsync(purchaseDto);

                    await _activityLogService.LogActivityAsync(
                        "Purchase",
                        id,
                        "Update",
                        $"Updated purchase order {purchaseDto.PurchaseOrderNo}",
                        User.Identity.Name
                    );

                    TempData["Success"] = $"Purchase Order #{purchaseDto.PurchaseOrderNo} successfully updated! ✅";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Failed to update purchase order. Please try again.";
                }
            }
            else
            {
                TempData["Warning"] = "Some information in the form is incorrect. Please check.";
            }

            await LoadViewBagData();
            return View(purchaseDto);
        }

        [HasPermission(Permission.ViewPurchase)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                {
                    TempData["Error"] = $"Purchase Order #{id} not found!";
                    return RedirectToAction(nameof(Index));
                }

                purchase.ApprovalHistory = await _purchaseService.GetApprovalHistoryAsync(id);

                return View(purchase);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load purchase order details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [HasPermission(Permission.DeletePurchase)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                {
                    return Json(new { success = false, message = "Purchase order not found!" });
                }

                if (purchase.Status != "Draft")
                {
                    return Json(new { success = false, message = "Only draft purchase orders can be deleted!" });
                }

                await _purchaseService.DeletePurchaseAsync(id);

                await _activityLogService.LogActivityAsync(
                    "Purchase",
                    id,
                    "Delete",
                    $"Deleted purchase order #{purchase.PurchaseOrderNo}",
                    User.Identity.Name
                );

                TempData["Success"] = $"Purchase Order #{purchase.PurchaseOrderNo} successfully deleted! 🗑️";
                return Json(new { success = true, message = "Successfully deleted!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to delete. Please try again." });
            }
        }

        [HttpPost]
        [HasPermission(Permission.CreatePurchase)]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                {
                    return Json(new { success = false, message = "Purchase order not found!" });
                }

                if (purchase.Status != "Draft")
                {
                    return Json(new { success = false, message = "This purchase order has already been submitted!" });
                }

                var result = await _purchaseService.SubmitForApprovalAsync(id);

                if (result.Success)
                {
                    purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                    await SendApprovalNotification(purchase);

                    await _activityLogService.LogActivityAsync(
                        "Purchase",
                        id,
                        "Submit",
                        $"Submitted purchase order #{purchase.PurchaseOrderNo} for approval",
                        User.Identity.Name
                    );

                    return Json(new
                    {
                        success = true,
                        message = $"Purchase Order #{purchase.PurchaseOrderNo} submitted for approval! ✅"
                    });
                }

                return Json(new { success = false, message = result.Message ?? "Failed to submit!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to submit for approval. Please try again." });
            }
        }

        [HttpPost]
        [HasPermission(Permission.UpdatePurchase)]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                {
                    return Json(new { success = false, message = "Purchase order not found!" });
                }

                if (purchase.Status != "Approved" && purchase.Status != "Received")
                {
                    return Json(new { success = false, message = "Only approved purchase orders can be completed!" });
                }

                await _purchaseService.UpdateStatusAsync(id, "Completed", User.Identity.Name);

                await _activityLogService.LogActivityAsync(
                    "Purchase",
                    id,
                    "Complete",
                    $"Marked purchase order #{purchase.PurchaseOrderNo} as completed",
                    User.Identity.Name
                );

                return Json(new
                {
                    success = true,
                    message = $"Purchase Order #{purchase.PurchaseOrderNo} completed! ✅"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to complete. Please try again." });
            }
        }

        [HasPermission(Permission.ReceivePurchase)]
        public async Task<IActionResult> Receive(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                {
                    TempData["Error"] = "Purchase order not found!";
                    return RedirectToAction(nameof(Index));
                }

                if (purchase.Status != "Approved")
                {
                    TempData["Warning"] = "Only approved purchase orders can be received! ⚠️";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var model = new ReceivePurchaseViewModel
                {
                    PurchaseId = id,
                    PurchaseOrderNo = purchase.PurchaseOrderNo,
                    VendorName = purchase.VendorName ?? "Marketplace Purchase",
                    Items = purchase.Items.Select(pi => new ReceiveItemViewModel
                    {
                        ItemId = pi.ItemId,
                        ItemName = pi.ItemName,
                        OrderedQuantity = pi.Quantity,
                        ReceivedQuantity = pi.Quantity,
                        AcceptedQuantity = pi.Quantity,
                        RejectedQuantity = 0
                    }).ToList()
                };

                TempData["Info"] = "Please verify the received quantity for each item.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load receive form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ReceivePurchase)]
        public async Task<IActionResult> ProcessReceive(ReceivePurchaseViewModel model)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(model.PurchaseId);
                if (purchase == null || purchase.Status != "Approved")
                {
                    TempData["Error"] = "Invalid purchase order or already received!";
                    return RedirectToAction(nameof(Index));
                }

                var receiveDto = new ReceiveGoodsDto
                {
                    PurchaseId = model.PurchaseId,
                    InvoiceNo = model.InvoiceNo,
                    ChallanNo = model.ChallanNo,
                    ReceivedDate = DateTime.Now,
                    ReceivedBy = User.Identity.Name,
                    Remarks = model.Remarks,
                    Items = model.Items.Select(item => new ReceiveItemDto
                    {
                        ItemId = item.ItemId,
                        ReceivedQuantity = item.ReceivedQuantity,
                        AcceptedQuantity = item.AcceptedQuantity,
                        RejectedQuantity = item.RejectedQuantity,
                        BatchNo = item.BatchNo,
                        ExpiryDate = item.ExpiryDate,
                        Remarks = item.Remarks
                    }).ToList()
                };

                var success = await _purchaseService.ReceiveGoodsAsync(model.PurchaseId, receiveDto);

                if (!success)
                {
                    TempData["Error"] = "Failed to receive goods.";
                    return RedirectToAction("Receive", new { id = model.PurchaseId });
                }

                foreach (var item in model.Items.Where(i => i.AcceptedQuantity > 0))
                {
                    var purchaseItem = purchase.Items.FirstOrDefault(pi => pi.ItemId == item.ItemId);
                    var storeId = purchaseItem?.StoreId ?? purchase.StoreId;

                    if (storeId.HasValue)
                    {
                        await _storeService.UpdateStockAsync(new StockUpdateDto
                        {
                            StoreId = storeId.Value,
                            ItemId = item.ItemId,
                            Quantity = item.AcceptedQuantity,
                            Type = "IN",
                            Reference = $"PO-{purchase.PurchaseOrderNo}",
                            BatchNo = item.BatchNo,
                            ExpiryDate = item.ExpiryDate,
                            UpdatedBy = User.Identity.Name
                        });
                    }
                }

                await _activityLogService.LogActivityAsync(
                    "Purchase",
                    model.PurchaseId,
                    "Receive",
                    $"Received goods for PO #{purchase.PurchaseOrderNo}. Invoice: {model.InvoiceNo}, Challan: {model.ChallanNo}",
                    User.Identity.Name
                );

                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = purchase.CreatedBy,
                    Title = "Goods Received ✅",
                    Message = $"Purchase Order #{purchase.PurchaseOrderNo} goods have been received.",
                    Type = "Success",
                    Url = Url.Action("Details", "Purchase", new { id = purchase.Id })
                });

                TempData["Success"] = $"Purchase Order #{purchase.PurchaseOrderNo} goods received successfully! ✅";

                if (model.Items.Any(i => i.RejectedQuantity > 0))
                {
                    TempData["Info"] = "Some items were rejected. Please perform Quality Check.";
                    return RedirectToAction("QualityCheck", new { id = model.PurchaseId });
                }

                var firstStoreId = purchase.Items.FirstOrDefault()?.StoreId ?? purchase.StoreId;

                if (firstStoreId.HasValue)
                {
                    var store = await _storeService.GetStoreByIdAsync(firstStoreId.Value);

                    if (store?.StoreTypeName != null && store.StoreTypeName.Contains("Central"))
                    {
                        TempData["Info"] = "📦 Items received in Central Store. Inspection required!";
                        return RedirectToAction("InspectionRequired", new { purchaseId = model.PurchaseId });
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to receive goods. Please try again.";
                return RedirectToAction("Receive", new { id = model.PurchaseId });
            }
        }

        [HasPermission(Permission.ReceivePurchase)]
        public async Task<IActionResult> InspectionRequired(int purchaseId)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(purchaseId);

                if (purchase == null)
                {
                    TempData["Error"] = "Purchase order not found!";
                    return RedirectToAction(nameof(Index));
                }

                var inspectionItems = new List<InspectionItemViewModel>();

                foreach (var item in purchase.Items)
                {
                    var storeId = item.StoreId ?? purchase.StoreId;
                    if (storeId.HasValue)
                    {
                        inspectionItems.Add(new InspectionItemViewModel
                        {
                            ItemId = item.ItemId,
                            ItemCode = item.ItemCode,
                            ItemName = item.ItemName,
                            Quantity = item.ReceivedQuantity ?? item.Quantity,
                            Condition = "Serviceable",
                            ApprovedForTransfer = true
                        });
                    }
                }

                var centralStore = await _storeService.GetStoreByIdAsync(
                    purchase.Items.FirstOrDefault()?.StoreId ?? purchase.StoreId ?? 0);

                var model = new InspectionViewModel
                {
                    PurchaseId = purchaseId,
                    PurchaseOrderNo = purchase.PurchaseOrderNo,
                    FromStoreId = centralStore?.Id ?? 0,
                    FromStoreName = centralStore?.Name ?? "Central Store",
                    InspectionDate = DateTime.Now,
                    Items = inspectionItems
                };

                TempData["Info"] = "📦 Please inspect items before transferring to Provision Store.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load inspection form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ReceivePurchase)]
        public async Task<IActionResult> CompleteInspection(InspectionViewModel model)
        {
            try
            {
                var allStores = await _storeService.GetAllStoresAsync();
                var provisionStore = allStores.FirstOrDefault(s =>
                    s.StoreTypeName != null && s.StoreTypeName.Contains("Provision"));

                if (provisionStore == null)
                {
                    TempData["Error"] = "❌ Provision Store not found! Please create one first.";
                    return RedirectToAction(nameof(Index));
                }

                var transferDto = new TransferDto
                {
                    FromStoreId = model.FromStoreId,
                    ToStoreId = provisionStore.Id,
                    TransferDate = DateTime.Now,
                    TransferType = "Inspection Transfer",
                    Purpose = $"Post-inspection transfer for PO #{model.PurchaseOrderNo}",
                    Status = "Pending",
                    Remarks = $"Inspected by {User.Identity.Name} on {DateTime.Now:yyyy-MM-dd HH:mm}",
                    CreatedBy = User.Identity.Name,
                    Items = model.Items
                        .Where(i => i.ApprovedForTransfer)
                        .Select(item => new TransferItemDto
                        {
                            ItemId = item.ItemId,
                            Quantity = item.Quantity,
                            Remarks = $"Condition: {item.Condition}. {item.InspectionRemarks}"
                        }).ToList()
                };

                if (!transferDto.Items.Any())
                {
                    TempData["Warning"] = "⚠️ No items approved for transfer!";
                    return View("InspectionRequired", model);
                }

                var transfer = await _transferService.CreateTransferAsync(transferDto);

                await _activityLogService.LogActivityAsync(
                    "Transfer",
                    transfer.Id,
                    "InspectionTransfer",
                    $"Inspection transfer created from Central to Provision Store for PO #{model.PurchaseOrderNo}",
                    User.Identity.Name
                );

                TempData["Success"] = $"✅ Inspection completed! Transfer #{transfer.TransferNo} created. {transferDto.Items.Count} items approved.";

                return RedirectToAction("Details", "Transfer", new { id = transfer.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing inspection for Purchase {PurchaseId}", model.PurchaseId);
                TempData["Error"] = "Failed to complete inspection. Please try again.";
                return View("InspectionRequired", model);
            }
        }

        [HasPermission(Permission.ReceivePurchase)]
        public async Task<IActionResult> PendingReceives()
        {
            try
            {
                var purchases = await _purchaseService.GetAllPurchasesAsync();
                var pendingReceives = purchases
                    .Where(p => p.Status == "Approved")
                    .OrderBy(p => p.ExpectedDeliveryDate)
                    .ToList();

                ViewBag.PendingReceiveCount = pendingReceives.Count();
                return View(pendingReceives);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load pending receives list.";
                return View(new List<PurchaseDto>());
            }
        }

        [Authorize(Roles = "Manager,Admin,StoreKeeper")]
        public async Task<IActionResult> QualityCheck(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                if (purchase == null)
                {
                    TempData["Error"] = "Purchase order not found!";
                    return RedirectToAction(nameof(Index));
                }

                if (purchase.Status != "Received")
                {
                    TempData["Warning"] = "Only received purchase orders can be quality checked!";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var model = new QualityCheckDto
                {
                    PurchaseId = id,
                    CheckNumber = $"QC-{DateTime.Now:yyyyMMdd}-{id}",
                    CheckDate = DateTime.Now,
                    CheckType = "Incoming",
                    Items = purchase.Items.Select(pi => new QualityCheckItemDto
                    {
                        ItemId = pi.ItemId,
                        ItemName = pi.ItemName,
                        CheckedQuantity = pi.ReceivedQuantity ?? pi.Quantity,
                        PassedQuantity = pi.AcceptedQuantity ?? pi.ReceivedQuantity ?? pi.Quantity,
                        FailedQuantity = pi.RejectedQuantity ?? 0,
                        Status = 0
                    }).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load quality check form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.QualityCheck)]
        public async Task<IActionResult> QualityCheck(QualityCheckDto model)
        {
            try
            {
                await _purchaseService.ProcessQualityCheckAsync(model);

                var allPassed = model.Items.All(i => i.Status == QualityCheckStatus.Pass);
                var newStatus = allPassed ? "QC-Passed" : "QC-Failed";
                await _purchaseService.UpdateStatusAsync(model.PurchaseId, newStatus, User.Identity.Name);

                if (allPassed)
                {
                    await _purchaseService.UpdateStatusAsync(model.PurchaseId, "Completed", User.Identity.Name);
                }

                await _activityLogService.LogActivityAsync(
                    "Purchase",
                    model.PurchaseId,
                    "QualityCheck",
                    $"Quality check completed. Status: {model.Status}",
                    User.Identity.Name
                );

                TempData["Success"] = "Quality check completed! ✅";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to process quality check.";
                return View(model);
            }
        }

        #region Private Helper Methods

        private async Task LoadViewBagData()
        {
            try
            {
                var vendors = await _vendorService.GetAllVendorsAsync();
                var items = await _itemService.GetAllItemsAsync();
                var stores = await _storeService.GetAllStoresAsync();

                ViewBag.Vendors = new SelectList(vendors.Where(v => v.IsActive), "Id", "Name");
                ViewBag.Items = items.Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.ItemCode} - {i.Name}"
                }).ToList();
                ViewBag.Stores = new SelectList(stores.Where(s => s.IsActive), "Id", "Name");
            }
            catch (Exception ex)
            {
                TempData["Warning"] = "Failed to load some data.";
                ViewBag.Vendors = new SelectList(new List<VendorDto>(), "Id", "Name");
                ViewBag.Items = new List<SelectListItem>();
                ViewBag.Stores = new SelectList(new List<StoreDto>(), "Id", "Name");
            }
        }

        private async Task<List<AttachmentDto>> SaveAttachments(List<IFormFile> files)
        {
            var attachments = new List<AttachmentDto>();
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "purchase");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var file in files.Take(5))
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    attachments.Add(new AttachmentDto
                    {
                        FileName = file.FileName,
                        FilePath = $"/uploads/purchase/{uniqueFileName}",
                        FileSize = file.Length,
                        UploadDate = DateTime.Now
                    });
                }
            }

            return attachments;
        }

        private async Task SendApprovalNotification(PurchaseDto purchase)
        {
            try
            {
                var approvers = await _purchaseService.GetApproversAsync();

                foreach (var approver in approvers)
                {
                    await _notificationService.SendNotificationAsync(new NotificationDto
                    {
                        UserId = approver.Id,
                        Title = "New Purchase Order Pending Approval 📋",
                        Message = $"Purchase Order #{purchase.PurchaseOrderNo} is waiting for approval. Amount: ৳{purchase.TotalAmount:N2}",
                        Type = "Warning",
                        Url = Url.Action("Details", "Purchase", new { id = purchase.Id })
                    });
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}