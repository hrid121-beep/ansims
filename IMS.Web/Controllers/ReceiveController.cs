using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Infrastructure.Repositories;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class ReceiveController : Controller
    {
        private readonly IReceiveService _receiveService;
        private readonly IIssueService _issueService;
        private readonly IStoreService _storeService;
        private readonly IBarcodeService _barcodeService;
        private readonly IStockService _stockService;
        private readonly IItemService _itemService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ReceiveController> _logger;
        private readonly IVoucherService _voucherService;

        public ReceiveController(
            IReceiveService receiveService,
            IIssueService issueService,
            IStoreService storeService,
            IBarcodeService barcodeService,
            IStockService stockService,
            IItemService itemService,
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            UserManager<User> userManager,
            ILogger<ReceiveController> logger,
            IVoucherService voucherService)
        {
            _receiveService = receiveService;
            _issueService = issueService;
            _storeService = storeService;
            _barcodeService = barcodeService;
            _stockService = stockService;
            _itemService = itemService;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _userManager = userManager;
            _logger = logger;
            _voucherService = voucherService;
        }

        // GET: Receive/Index - List all receives
        [HasPermission(Permission.ViewReceive)]
        public async Task<IActionResult> Index(string searchTerm, string receiveType, string status,
            DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            try
            {
                var receives = await _receiveService.GetAllReceivesAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    receives = receives.Where(r =>
                        r.ReceiveNo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        r.ReceivedFromName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(receiveType))
                {
                    receives = receives.Where(r => r.ReceiveType == receiveType);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    receives = receives.Where(r => r.Status == status);
                }

                if (fromDate.HasValue)
                {
                    receives = receives.Where(r => r.ReceiveDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    receives = receives.Where(r => r.ReceiveDate <= toDate.Value);
                }

                // Pagination
                var pageSize = 10;
                var totalItems = receives.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                receives = receives.Skip((page - 1) * pageSize).Take(pageSize);

                ViewBag.SearchTerm = searchTerm;
                ViewBag.ReceiveType = receiveType;
                ViewBag.Status = status;
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(receives);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading receives");
                TempData["Error"] = "Error loading receives. Please try again.";
                return View(new List<ReceiveDto>());
            }
        }

        // GET: Receive/Create - Direct receive without issue voucher
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.ReceiveNo = await _receiveService.GenerateReceiveNoAsync();
                await LoadReceiveViewBags();
                return View(new ReceiveDto { ReceiveDate = DateTime.Now });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create receive page");
                TempData["Error"] = "Error loading page. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Receive/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> Create(ReceiveDto model, string action)
        {
            try
            {
                // ✅ FIX: Validate store matching for linked issues
                if (model.OriginalIssueId.HasValue)
                {
                    var originalIssue = await _issueService.GetIssueByIdAsync(model.OriginalIssueId.Value);
                    if (originalIssue != null)
                    {
                        // Determine the correct receiving store based on items
                        if (model.Items != null && model.Items.Any())
                        {
                            // Check if all items are from the same store as the original issue
                            var firstItemStoreId = model.Items.First().StoreId;

                            // Get the original issue's store
                            var originalStoreId = originalIssue.FromStoreId ?? firstItemStoreId;

                            // Validate that receive is going to the SAME store
                            foreach (var item in model.Items)
                            {
                                if (item.StoreId != originalStoreId)
                                {
                                    ModelState.AddModelError("",
                                        $"Items must be received back to the SAME store from which they were issued. " +
                                        $"Original issue was from Store ID: {originalStoreId}");
                                    break;
                                }
                            }
                        }
                    }
                }

                // Additional validation for complete action
                if (action == "complete")
                {
                    if (model.Items == null || !model.Items.Any())
                    {
                        ModelState.AddModelError("", "Please add at least one item to receive.");
                    }

                    if (string.IsNullOrEmpty(model.ReceiverSignature))
                    {
                        ModelState.AddModelError("ReceiverSignature", "Signature is required to complete receive.");
                    }

                    if (string.IsNullOrEmpty(model.ReceiverName) || string.IsNullOrEmpty(model.ReceiverBadgeNo))
                    {
                        ModelState.AddModelError("", "Receiver information is required to complete receive.");
                    }
                }

                if (ModelState.IsValid)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    model.ReceivedBy = currentUser.FullName;
                    model.CreatedBy = currentUser.Id;

                    // ✅ FIX: Always create as Draft first, then complete if needed
                    model.Status = "Draft";

                    var result = await _receiveService.CreateReceiveAsync(model);

                    if (result != null)
                    {
                        // If action is complete, call CompleteReceiveAsync to update stock
                        if (action == "complete")
                        {
                            await _receiveService.CompleteReceiveAsync(result.Id, model.ReceiverSignature);
                            TempData["Success"] = "Receive completed successfully!";
                        }
                        else
                        {
                            TempData["Success"] = "Receive saved as draft successfully!";
                        }
                        return RedirectToAction(nameof(Details), new { id = result.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating receive");
                ModelState.AddModelError("", "Error creating receive. Please try again.");
            }

            ViewBag.ReceiveNo = model.ReceiveNo;
            await LoadReceiveViewBags();
            return View(model);
        }

        // GET: Receive/ScanVoucher - Page for scanning issue voucher
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> ScanVoucher()
        {
            ViewBag.ReceiveNo = await _receiveService.GenerateReceiveNoAsync();
            return View();
        }

        // POST: Receive/ScanVoucher - Process scanned QR code or manual voucher number
        [HttpPost]
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> ScanVoucher(string qrCode)
        {
            try
            {
                if (string.IsNullOrEmpty(qrCode))
                {
                    return Json(new { success = false, message = "Please enter or scan a voucher number" });
                }

                string voucherNo = null;

                // Try to parse as JSON (QR code data)
                try
                {
                    using (var doc = JsonDocument.Parse(qrCode))
                    {
                        var root = doc.RootElement;

                        // Check if it's a valid issue voucher QR code
                        if (root.TryGetProperty("Type", out var typeElement) &&
                            typeElement.GetString() == "ISSUE_VOUCHER")
                        {
                            // Extract voucher number from QR code JSON
                            if (root.TryGetProperty("VoucherNo", out var voucherElement))
                            {
                                voucherNo = voucherElement.GetString();
                            }
                            else if (root.TryGetProperty("IssueNo", out var issueNoElement))
                            {
                                voucherNo = issueNoElement.GetString();
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = "Invalid QR code. Expected Issue Voucher QR code." });
                        }
                    }
                }
                catch (JsonException)
                {
                    // Not JSON - treat as plain voucher number entered manually
                    voucherNo = qrCode.Trim();
                    _logger.LogInformation("Manual voucher entry: {VoucherNo}", voucherNo);
                }
                catch (FormatException)
                {
                    // If base64 decode fails, treat as plain text
                    voucherNo = qrCode.Trim();
                }

                if (string.IsNullOrEmpty(voucherNo))
                {
                    return Json(new { success = false, message = "Could not extract voucher number from input" });
                }

                // Create receive from voucher number
                var receive = await _receiveService.CreateReceiveFromVoucherAsync(voucherNo);

                if (receive != null)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Receive created successfully for voucher {voucherNo}",
                        redirectUrl = Url.Action(nameof(Process), new { id = receive.Id })
                    });
                }

                return Json(new { success = false, message = $"Could not create receive for voucher: {voucherNo}. Please verify the voucher number exists and has not been received yet." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning voucher: {QrCode}", qrCode);
                return Json(new { success = false, message = "Error processing voucher: " + ex.Message });
            }
        }

        // GET: Receive/Process/{id} - Process receive after scanning
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> Process(int id)
        {
            try
            {
                var receive = await _receiveService.GetReceiveByIdAsync(id);
                if (receive == null)
                {
                    TempData["Error"] = "Receive not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Load stores for destination selection
                ViewBag.Stores = await _storeService.GetActiveStoresAsync();

                // Load original issue details if linked
                if (receive.OriginalIssueId.HasValue)
                {
                    ViewBag.OriginalIssue = await _issueService.GetIssueByIdAsync(receive.OriginalIssueId.Value);
                }

                return View(receive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading process page");
                TempData["Error"] = "Error loading receive details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Receive/UpdateItemCondition - Update item condition assessment
        [HttpPost]
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> UpdateItemCondition(int receiveId, int itemId,
            string condition, decimal receivedQuantity, string damagePhoto, string notes)
        {
            try
            {
                // Update item condition
                var result = await _receiveService.AssessItemConditionAsync(
                    receiveId, itemId, condition, notes);

                // Save damage photo if provided
                if (condition == "Damaged" && !string.IsNullOrEmpty(damagePhoto))
                {
                    await _receiveService.AddDamagePhotoAsync(receiveId, itemId, damagePhoto);
                }

                // Update received quantity
                if (receivedQuantity > 0)
                {
                    await _receiveService.UpdateReceivedQuantityAsync(receiveId, itemId, receivedQuantity);
                }

                return Json(new { success = true, message = "Item condition updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item condition");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Receive/Complete - Complete the receive process
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> Complete(int id, string receiverSignature,
            string receiverName, string receiverBadgeNo, string notes)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var receive = await _receiveService.GetReceiveByIdAsync(id);

                if (receive == null)
                {
                    return Json(new { success = false, message = "Receive not found" });
                }

                // Validate items are assessed
                if (receive.Items.Any(i => string.IsNullOrEmpty(i.Condition)))
                {
                    return Json(new { success = false, message = "Please assess condition of all items" });
                }

                // Complete the receive
                var result = await _receiveService.CompleteReceiveAsync(id, receiverSignature);

                if (result)
                {
                    // Send notification
                    await _notificationService.SendNotificationAsync(new NotificationDto
                    {
                        UserId = currentUser.Id,
                        Title = "Receive Completed",
                        Message = $"Receive {receive.ReceiveNo} has been completed successfully",
                        Type = "Success"
                    });

                    TempData["Success"] = "Items received successfully!";
                    return Json(new
                    {
                        success = true,
                        redirectUrl = Url.Action(nameof(Details), new { id })
                    });
                }

                return Json(new { success = false, message = "Error completing receive" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing receive");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Receive/Pending - Show pending/processing receives
        [HasPermission(Permission.ViewReceive)]
        public async Task<IActionResult> Pending()
        {
            try
            {
                var receives = await _receiveService.GetAllReceivesAsync();
                var pendingReceives = receives.Where(r =>
                    r.Status == "Pending" || r.Status == "Processing" || r.Status == "Draft"
                ).OrderBy(r => r.ReceiveDate);

                return View("Index", pendingReceives);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading pending receives");
                TempData["Error"] = "Error loading pending receives.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Receive/QuickReceive - Quick receive without issue voucher
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> QuickReceive()
        {
            try
            {
                ViewBag.ReceiveNo = await _receiveService.GenerateReceiveNoAsync();

                // Convert stores to SelectListItem
                var stores = await _storeService.GetActiveStoresAsync();
                ViewBag.Stores = stores.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();

                // Convert items to SelectListItem
                var items = await _itemService.GetAllItemsAsync();
                ViewBag.Items = items.Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.ItemCode} - {i.Name}"
                }).ToList();

                // Load organization data from UnitOfWork and convert to SelectListItem
                var battalions = await _unitOfWork.Battalions.GetAllAsync();
                ViewBag.Battalions = battalions.Where(b => b.IsActive).Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList();

                var ranges = await _unitOfWork.Ranges.GetAllAsync();
                ViewBag.Ranges = ranges.Where(r => r.IsActive).Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();

                return View(new QuickReceiveDto { ReceiveDate = DateTime.Now });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading quick receive page");
                TempData["Error"] = "Error loading page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Receive/QuickReceive
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> QuickReceive(QuickReceiveDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    model.ReceivedBy = currentUser.Id;

                    var result = await _receiveService.CreateQuickReceiveAsync(model);

                    if (result != null)
                    {
                        TempData["Success"] = "Quick receive completed successfully!";
                        return RedirectToAction(nameof(Details), new { id = result.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in quick receive");
                ModelState.AddModelError("", ex.Message);
            }

            await LoadQuickReceiveViewBag();
            return View(model);
        }

        // GET: Receive/VerifyItems - Verify received items
        [HasPermission(Permission.ViewReceive)]
        public async Task<IActionResult> VerifyItems(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    // Verify specific receive
                    var receive = await _receiveService.GetReceiveByIdAsync(id.Value);
                    if (receive == null)
                    {
                        TempData["Error"] = "Receive not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    return View(receive);
                }
                else
                {
                    // Show pending verification list
                    var receives = await _receiveService.GetAllReceivesAsync();
                    var pendingVerification = receives.Where(r => r.Status == "Pending" || r.Status == "Processing");

                    ViewBag.ShowList = true;
                    return View("VerifyItems", pendingVerification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading verify items page");
                TempData["Error"] = "Error loading verification page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Receive/VerifyItems
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> VerifyItems(int id, List<ReceiveItemDto> items)
        {
            try
            {
                foreach (var item in items)
                {
                    await _receiveService.UpdateReceivedQuantityAsync(id, item.ItemId, item.ReceivedQuantity ?? 0);

                    if (!string.IsNullOrEmpty(item.Condition))
                    {
                        await _receiveService.AssessItemConditionAsync(id, item.ItemId, item.Condition, item.Remarks);
                    }
                }

                TempData["Success"] = "Items verified successfully!";
                return RedirectToAction(nameof(Process), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying items");
                TempData["Error"] = "Error verifying items.";
                return RedirectToAction(nameof(VerifyItems), new { id });
            }
        }

        // GET: Receive/Details/{id} - View receive details
        [HasPermission(Permission.ViewReceive)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var receive = await _receiveService.GetReceiveByIdAsync(id);
                if (receive == null)
                {
                    TempData["Error"] = "Receive not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Load related data
                if (receive.OriginalIssueId.HasValue)
                {
                    ViewBag.OriginalIssue = await _issueService.GetIssueByIdAsync(receive.OriginalIssueId.Value);
                }

                return View(receive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading receive details");
                TempData["Error"] = "Error loading details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Receive/Edit/{id} - Edit receive (only for draft status)
        [HasPermission(Permission.EditReceive)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var receive = await _receiveService.GetReceiveByIdAsync(id);
                if (receive == null)
                {
                    TempData["Error"] = "Receive not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (receive.Status != "Draft" && receive.Status != "Pending")
                {
                    TempData["Error"] = "Cannot edit completed receive.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await LoadReceiveViewBags();
                return View(receive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit page");
                TempData["Error"] = "Error loading page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Receive/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.EditReceive)]
        public async Task<IActionResult> Edit(ReceiveDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Since UpdateReceiveAsync doesn't exist, we can manually update
                    // For now, just redirect back with message
                    TempData["Info"] = "Edit functionality is being implemented.";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating receive");
                ModelState.AddModelError("", "Error updating receive.");
            }

            await LoadReceiveViewBags();
            return View(model);
        }

        // GET: Receive/Print/{id} - Print receive receipt
        [HasPermission(Permission.ViewReceive)]
        public async Task<IActionResult> Print(int id)
        {
            try
            {
                var receive = await _receiveService.GetReceiveByIdAsync(id);
                if (receive == null)
                {
                    TempData["Error"] = "Receive not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Load additional data for printing
                if (receive.OriginalIssueId.HasValue)
                {
                    ViewBag.OriginalIssue = await _issueService.GetIssueByIdAsync(receive.OriginalIssueId.Value);
                }

                return View("PrintReceipt", receive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating print view");
                TempData["Error"] = "Error generating receipt.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Receive/Delete/{id} - Soft delete receive
        [HasPermission(Permission.DeleteReceive)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var receive = await _receiveService.GetReceiveByIdAsync(id);
                if (receive == null)
                {
                    TempData["Error"] = "Receive not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (receive.Status == "Completed")
                {
                    TempData["Error"] = "Cannot delete completed receive.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                return View(receive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete confirmation");
                TempData["Error"] = "Error loading page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Receive/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteReceive)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Since DeleteReceiveAsync doesn't exist, implement soft delete manually
                // For now, just redirect with message
                TempData["Info"] = "Delete functionality is being implemented.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting receive");
                TempData["Error"] = "Error deleting receive.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: Receive/Export - Export receives to Excel
        [HasPermission(Permission.ExportData)]
        public async Task<IActionResult> Export(string format = "excel")
        {
            try
            {
                var receives = await _receiveService.GetAllReceivesAsync();

                if (format.ToLower() == "pdf")
                {
                    var pdfBytes = _receiveService.ExportToPdf(receives);
                    return File(pdfBytes, "application/pdf", $"Receives_{DateTime.Now:yyyyMMdd}.pdf");
                }
                else
                {
                    var excelBytes = _receiveService.ExportToExcel(receives);
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Receives_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting receives");
                TempData["Error"] = "Error exporting data.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Receive/GetIssueItems/{id} - Get issue items for receive creation
        [HttpGet]
        public async Task<IActionResult> GetIssueItems(int id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                if (issue == null)
                {
                    return Json(new { success = false, message = "Issue not found" });
                }

                var items = issue.Items.Select(i => new
                {
                    itemId = i.ItemId,
                    storeId = i.StoreId,
                    quantity = i.Quantity,
                    itemName = i.ItemName,
                    itemCode = i.ItemCode,
                    unit = i.Unit
                });

                return Json(new { success = true, items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue items");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #region Voucher Actions

        // POST: Receive/GenerateVoucher - Generate voucher for receive
        [HttpPost]
        [HasPermission(Permission.CreateReceive)]
        public async Task<IActionResult> GenerateVoucher(int id)
        {
            try
            {
                var voucherNo = await _voucherService.GenerateReceiveVoucherAsync(id);
                TempData["Success"] = $"ভাউচার তৈরি হয়েছে: {voucherNo}";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating voucher for Receive {ReceiveId}", id);
                TempData["Error"] = "ভাউচার তৈরিতে সমস্যা হয়েছে।";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Receive/DownloadVoucher - Download voucher PDF
        [HttpGet]
        [HasPermission(Permission.ViewReceive)]
        public async Task<IActionResult> DownloadVoucher(int id)
        {
            try
            {
                var pdfBytes = await _voucherService.GetReceiveVoucherPdfAsync(id);
                var receive = await _receiveService.GetReceiveByIdAsync(id);

                string fileName = $"Receive_Voucher_{receive.VoucherNo}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading voucher for Receive {ReceiveId}", id);
                TempData["Error"] = "ভাউচার ডাউনলোডে সমস্যা হয়েছে।";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion

        #region Helper Methods

        private async Task LoadReceiveViewBags()
        {
            // Convert stores to SelectListItem
            var stores = await _storeService.GetActiveStoresAsync();
            ViewBag.Stores = stores.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();

            // Load organization data from UnitOfWork and convert to SelectListItem
            var battalions = await _unitOfWork.Battalions.GetAllAsync();
            ViewBag.Battalions = battalions.Where(b => b.IsActive).Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            var ranges = await _unitOfWork.Ranges.GetAllAsync();
            ViewBag.Ranges = ranges.Where(r => r.IsActive).Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            var zilas = await _unitOfWork.Zilas.GetAllAsync();
            ViewBag.Zilas = zilas.Where(z => z.IsActive).Select(z => new SelectListItem
            {
                Value = z.Id.ToString(),
                Text = z.Name
            }).ToList();

            var upazilas = await _unitOfWork.Upazilas.GetAllAsync();
            ViewBag.Upazilas = upazilas.Where(u => u.IsActive).Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();

            // Convert items to SelectListItem
            var items = await _itemService.GetAllItemsAsync();
            ViewBag.Items = items.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = $"{i.ItemCode} - {i.Name}"
            }).ToList();

            ViewBag.ReceiveTypes = new List<SelectListItem>
            {
                new SelectListItem("Battalion", "Battalion"),
                new SelectListItem("Range", "Range"),
                new SelectListItem("Zila", "Zila"),
                new SelectListItem("Upazila", "Upazila"),
                new SelectListItem("Individual", "Individual"),
                new SelectListItem("External", "External")
            };

            ViewBag.Conditions = new List<SelectListItem>
            {
                new SelectListItem("Good", "Good"),
                new SelectListItem("Damaged", "Damaged"),
                new SelectListItem("Expired", "Expired")
            };
        }

        private async Task LoadQuickReceiveViewBag()
        {
            await LoadReceiveViewBags();
        }

        #endregion
    }
}