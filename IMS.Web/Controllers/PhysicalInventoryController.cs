using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class PhysicalInventoryController : Controller
    {
        private readonly IPhysicalInventoryService _physicalInventoryService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;
        private readonly IApprovalService _approvalService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PhysicalInventoryController> _logger;

        public PhysicalInventoryController(
            IPhysicalInventoryService physicalInventoryService,
            IStoreService storeService,
            IItemService itemService,
            IApprovalService approvalService,
            UserManager<User> userManager,
            ILogger<PhysicalInventoryController> logger)
        {
            _physicalInventoryService = physicalInventoryService;
            _storeService = storeService;
            _itemService = itemService;
            _approvalService = approvalService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: PhysicalInventory
        [HasPermission(Permission.ViewPhysicalInventory)]
        public async Task<IActionResult> Index(
            int? storeId = null,
            PhysicalInventoryStatus? status = null,
            string fiscalYear = null)
        {
            try
            {
                var inventories = await _physicalInventoryService.GetPhysicalInventoriesAsync(
                    storeId, status, fiscalYear);

                // Load filter options
                ViewBag.Stores = new SelectList(
                    await _storeService.GetAllStoresAsync(), "Id", "Name", storeId);
                ViewBag.CurrentStoreId = storeId;
                ViewBag.CurrentStatus = status;
                ViewBag.CurrentFiscalYear = fiscalYear;

                // Get fiscal years for filter
                ViewBag.FiscalYears = await GetFiscalYearsSelectListAsync(fiscalYear);

                return View(inventories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading physical inventories");
                TempData["Error"] = "An error occurred while loading physical inventories.";
                return View(new List<PhysicalInventoryDto>());
            }
        }

        // GET: PhysicalInventory/Initiate
        [HasPermission(Permission.CreatePhysicalInventory)]
        public async Task<IActionResult> Initiate(int? storeId = null)
        {
            try
            {
                // Check if user can initiate count
                var currentUser = await _userManager.GetUserAsync(User);
                if (storeId.HasValue)
                {
                    var canInitiate = await _physicalInventoryService.CanUserInitiateCountAsync(
                        currentUser.Id, storeId.Value);

                    if (!canInitiate)
                    {
                        TempData["Error"] = "You don't have permission to initiate count for this store.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                await LoadInitiateViewBagData(storeId);

                var model = new PhysicalInventoryDto
                {
                    StoreId = storeId ?? 0,
                    CountDate = DateTime.Now,
                    CountType = CountType.Full
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading initiate view");
                TempData["Error"] = "An error occurred while loading the page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PhysicalInventory/Initiate
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreatePhysicalInventory)]
        public async Task<IActionResult> Initiate(PhysicalInventoryDto model, string itemSelection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Set selected items based on selection type
                    if (itemSelection == "all")
                    {
                        model.SelectedItemIds = null; // null means all items
                    }

                    model.InitiatedBy = User.Identity.Name;
                    var result = await _physicalInventoryService.InitiatePhysicalInventoryAsync(model);

                    TempData["Success"] = $"Physical inventory {result.ReferenceNumber} initiated successfully!";
                    return RedirectToAction(nameof(Count), new { id = result.Id });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating physical inventory");
                ModelState.AddModelError("", "An error occurred while initiating physical inventory.");
            }

            await LoadInitiateViewBagData(model.StoreId);
            return View(model);
        }

        // GET: PhysicalInventory/Count/5
        [HasPermission(Permission.UpdatePhysicalInventory)]
        public async Task<IActionResult> Count(int id)
        {
            try
            {
                var inventory = await _physicalInventoryService.GetPhysicalInventoryByIdAsync(id);
                if (inventory == null)
                {
                    TempData["Error"] = "Physical inventory not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check status
                if (inventory.Status != PhysicalInventoryStatus.Initiated &&
                    inventory.Status != PhysicalInventoryStatus.InProgress)
                {
                    TempData["Error"] = "This inventory cannot be counted in its current status.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Load approval chain if exists
                ViewBag.ApprovalChain = await _approvalService.GetApprovalChainAsync(
                    "PhysicalInventory", id);

                return View(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading count page");
                TempData["Error"] = "An error occurred while loading the count page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PhysicalInventory/SaveCount (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdatePhysicalInventory)]
        public async Task<IActionResult> SaveCount(int inventoryId, List<PhysicalCountUpdateDto> counts)
        {
            try
            {
                if (counts == null || !counts.Any())
                {
                    return Json(new { success = false, message = "No count data provided." });
                }

                foreach (var count in counts)
                {
                    count.CountedBy = User.Identity.Name;
                }

                await _physicalInventoryService.UpdatePhysicalCountAsync(inventoryId, counts);

                return Json(new { success = true, message = "Count saved successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving count");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: PhysicalInventory/SubmitForApproval (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdatePhysicalInventory)]
        public async Task<IActionResult> SubmitForApproval(int inventoryId, string verifiedBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(verifiedBy))
                {
                    return Json(new { success = false, message = "Verifier name is required." });
                }

                var result = await _physicalInventoryService.SubmitForApprovalAsync(inventoryId, verifiedBy);

                return Json(new
                {
                    success = true,
                    message = "Physical inventory submitted for approval successfully!",
                    redirectUrl = Url.Action("Review", new { id = inventoryId })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting for approval");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: PhysicalInventory/CompleteCount
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdatePhysicalInventory)]
        public async Task<IActionResult> CompleteCount(int id)
        {
            try
            {
                await _physicalInventoryService.CompleteCountingAsync(id, User.Identity.Name);
                TempData["Success"] = "Physical inventory counting completed successfully!";
                return RedirectToAction(nameof(Review), new { id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Count), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing count");
                TempData["Error"] = "An error occurred while completing count.";
                return RedirectToAction(nameof(Count), new { id });
            }
        }

        // GET: PhysicalInventory/Review/5
        [HasPermission(Permission.ReviewPhysicalInventory)]
        public async Task<IActionResult> Review(int id)
        {
            try
            {
                var inventory = await _physicalInventoryService.GetPhysicalInventoryByIdAsync(id);
                if (inventory == null)
                {
                    TempData["Error"] = "Physical inventory not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get variance analysis
                var varianceAnalysis = await _physicalInventoryService.GetVarianceAnalysisAsync(id);
                ViewBag.VarianceAnalysis = varianceAnalysis;

                // Check if current user can approve
                var currentUser = await _userManager.GetUserAsync(User);
                ViewBag.CanApprove = await _approvalService.CanUserApproveAsync(
                    currentUser.Id,
                    await _approvalService.GetCurrentApprovalLevelAsync("PhysicalInventory", id));

                return View(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading review page");
                TempData["Error"] = "An error occurred while loading the review page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PhysicalInventory/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ApprovePhysicalInventory)]
        public async Task<IActionResult> Approve(int id, string approvalRemarks, bool autoAdjust = false)
        {
            try
            {
                var result = await _physicalInventoryService.ApprovePhysicalInventoryAsync(
                    id,
                    User.Identity.Name,
                    approvalRemarks,
                    autoAdjust);

                if (result.Status == PhysicalInventoryStatus.Approved)
                {
                    TempData["Success"] = "Physical inventory approved successfully!";
                    if (autoAdjust)
                    {
                        TempData["Success"] += " Stock has been automatically adjusted.";
                    }
                }
                else
                {
                    TempData["Success"] = "Your approval has been recorded. Waiting for next level approval.";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Review), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving physical inventory");
                TempData["Error"] = "An error occurred while approving.";
                return RedirectToAction(nameof(Review), new { id });
            }
        }

        // POST: PhysicalInventory/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ApprovePhysicalInventory)]
        public async Task<IActionResult> Reject(int id, string rejectionReason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rejectionReason))
                {
                    TempData["Error"] = "Rejection reason is required.";
                    return RedirectToAction(nameof(Review), new { id });
                }

                await _physicalInventoryService.RejectPhysicalInventoryAsync(
                    id,
                    User.Identity.Name,
                    rejectionReason);

                TempData["Success"] = "Physical inventory has been rejected.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting physical inventory");
                TempData["Error"] = "An error occurred while rejecting.";
                return RedirectToAction(nameof(Review), new { id });
            }
        }

        // GET: PhysicalInventory/Details/5
        [HasPermission(Permission.ViewPhysicalInventory)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var inventory = await _physicalInventoryService.GetPhysicalInventoryByIdAsync(id);
                if (inventory == null)
                {
                    TempData["Error"] = "Physical inventory not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get variance analysis if completed or approved
                if (inventory.Status == PhysicalInventoryStatus.Completed ||
                    inventory.Status == PhysicalInventoryStatus.Approved ||
                    inventory.Status == PhysicalInventoryStatus.UnderReview)
                {
                    var varianceAnalysis = await _physicalInventoryService.GetVarianceAnalysisAsync(id);
                    ViewBag.VarianceAnalysis = varianceAnalysis;
                }

                // Get approval chain
                ViewBag.ApprovalChain = await _approvalService.GetApprovalChainAsync(
                    "PhysicalInventory", id);

                return View(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading details");
                TempData["Error"] = "An error occurred while loading details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PhysicalInventory/RequestRecount (AJAX)
        [HttpPost]
        [HasPermission(Permission.UpdatePhysicalInventory)]
        public async Task<IActionResult> RequestRecount(int inventoryId, List<int> itemIds)
        {
            try
            {
                if (itemIds == null || !itemIds.Any())
                {
                    return Json(new { success = false, message = "No items selected for recount." });
                }

                await _physicalInventoryService.RecountItemsAsync(
                    inventoryId, itemIds, User.Identity.Name);

                return Json(new { success = true, message = "Recount requested successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting recount");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: PhysicalInventory/Export/5
        [HasPermission(Permission.ExportPhysicalInventory)]
        public async Task<IActionResult> Export(int id, string format = "excel")
        {
            try
            {
                var inventory = await _physicalInventoryService.GetPhysicalInventoryByIdAsync(id);
                if (inventory == null)
                {
                    TempData["Error"] = "Physical inventory not found.";
                    return RedirectToAction(nameof(Index));
                }

                var data = await _physicalInventoryService.ExportPhysicalInventoryAsync(id, format);

                var contentType = format.ToLower() == "excel"
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "application/pdf";

                var fileName = $"PhysicalInventory_{inventory.ReferenceNumber}_{DateTime.Now:yyyyMMdd}.{(format.ToLower() == "excel" ? "xlsx" : "pdf")}";

                return File(data, contentType, fileName);
            }
            catch (NotImplementedException)
            {
                TempData["Error"] = "Export functionality is under development.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting physical inventory");
                TempData["Error"] = "An error occurred while exporting.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: PhysicalInventory/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CancelPhysicalInventory)]
        public async Task<IActionResult> Cancel(int id, string cancellationReason)
        {
            try
            {
                await _physicalInventoryService.CancelPhysicalInventoryAsync(
                    id, User.Identity.Name, cancellationReason);

                TempData["Success"] = "Physical inventory has been cancelled.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling physical inventory");
                TempData["Error"] = "An error occurred while cancelling.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: PhysicalInventory/VarianceReport/5
        [HasPermission(Permission.ViewPhysicalInventory)]
        public async Task<IActionResult> VarianceReport(int id)
        {
            try
            {
                var inventory = await _physicalInventoryService.GetPhysicalInventoryByIdAsync(id);
                if (inventory == null)
                {
                    TempData["Error"] = "Physical inventory not found.";
                    return RedirectToAction(nameof(Index));
                }

                var varianceAnalysis = await _physicalInventoryService.GetVarianceAnalysisAsync(id);

                ViewBag.Inventory = inventory;
                return View(varianceAnalysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading variance report");
                TempData["Error"] = "An error occurred while loading variance report.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: PhysicalInventory/History
        [HasPermission(Permission.ViewPhysicalInventory)]
        public async Task<IActionResult> History(int? storeId = null, int? itemId = null)
        {
            try
            {
                var history = await _physicalInventoryService.GetInventoryHistoryAsync(storeId, itemId);

                ViewBag.Stores = new SelectList(
                    await _storeService.GetAllStoresAsync(), "Id", "Name", storeId);

                if (itemId.HasValue)
                {
                    var item = await _itemService.GetItemByIdAsync(itemId.Value);
                    ViewBag.ItemName = item?.Name;
                }

                return View(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading history");
                TempData["Error"] = "An error occurred while loading history.";
                return View(new List<PhysicalInventoryDto>());
            }
        }

        // Private helper methods
        private async Task LoadInitiateViewBagData(int? storeId = null)
        {
            // Get stores user has access to
            var currentUser = await _userManager.GetUserAsync(User);
            var stores = await _storeService.GetUserAccessibleStoresAsync(currentUser.Id);

            ViewBag.Stores = new SelectList(stores, "Id", "Name", storeId);
            ViewBag.CountTypes = new SelectList(
                Enum.GetValues(typeof(CountType))
                    .Cast<CountType>()
                    .Select(ct => new
                    {
                        Value = ct,
                        Text = ct.ToString()
                    }),
                "Value", "Text");

            ViewBag.CurrentFiscalYear = await _physicalInventoryService.GetCurrentFiscalYearAsync();
        }

        private async Task<SelectList> GetFiscalYearsSelectListAsync(string selectedYear = null)
        {
            var fiscalYears = await _physicalInventoryService.GetAvailableFiscalYearsAsync();
            return new SelectList(fiscalYears, selectedYear);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForApproval(int inventoryId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            // Use _physicalInventoryService, NOT _storeService!
            var result = await _physicalInventoryService.SubmitForApprovalAsync(
                inventoryId,
                currentUser.UserName
            );
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreItems(int storeId)
        {
            // Use _physicalInventoryService for this method!
            var items = await _physicalInventoryService.GetStoreItemsAsync(storeId);
            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentFiscalYear()
        {
            // Use _physicalInventoryService!
            var fiscalYear = await _physicalInventoryService.GetCurrentFiscalYearAsync();
            return Json(fiscalYear);
        }

        [HttpGet]
        public async Task<IActionResult> CanInitiateCount(int storeId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            // Use _physicalInventoryService!
            var canInitiate = await _physicalInventoryService.CanUserInitiateCountAsync(
                currentUser.Id,
                storeId
            );
            return Json(canInitiate);
        }

        // GET: PhysicalInventory/Schedule
        [HasPermission(Permission.CreatePhysicalInventory)]
        public async Task<IActionResult> Schedule()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var stores = await _storeService.GetUserAccessibleStoresAsync(currentUser.Id);

                ViewBag.Stores = new SelectList(stores, "Id", "Name");
                ViewBag.CountTypes = new SelectList(
                    Enum.GetValues(typeof(CountType))
                        .Cast<CountType>()
                        .Select(ct => new { Value = ct, Text = ct.ToString() }),
                    "Value", "Text");

                // Add current fiscal year
                ViewBag.CurrentFiscalYear = await _physicalInventoryService.GetCurrentFiscalYearAsync();

                // Get scheduled counts for sidebar display
                var scheduledCounts = await _physicalInventoryService.GetPhysicalInventoriesAsync(
                    null, PhysicalInventoryStatus.Scheduled, null);
                ViewBag.ScheduledCounts = scheduledCounts.Take(5).ToList();

                var model = new PhysicalInventoryDto
                {
                    CountDate = DateTime.Now.AddDays(1), // Default to tomorrow
                    CountType = CountType.Full
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading schedule view");
                TempData["Error"] = "An error occurred while loading the page.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PhysicalInventory/Schedule
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreatePhysicalInventory)]
        public async Task<IActionResult> Schedule(PhysicalInventoryDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.InitiatedBy = User.Identity.Name;
                    model.Status = PhysicalInventoryStatus.Scheduled;

                    var result = await _physicalInventoryService.SchedulePhysicalInventoryAsync(model);

                    TempData["Success"] = $"Physical count scheduled successfully for {model.CountDate:dd MMM yyyy}!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling physical count");
                ModelState.AddModelError("", "An error occurred while scheduling physical count.");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var stores = await _storeService.GetUserAccessibleStoresAsync(currentUser.Id);
            ViewBag.Stores = new SelectList(stores, "Id", "Name", model.StoreId);
            ViewBag.CountTypes = new SelectList(
                Enum.GetValues(typeof(CountType))
                    .Cast<CountType>()
                    .Select(ct => new { Value = ct, Text = ct.ToString() }),
                "Value", "Text", model.CountType);

            return View(model);
        }

        // GET: PhysicalInventory/ExportCountHistoryCsv
        [HasPermission(Permission.ExportPhysicalInventory)]
        public async Task<IActionResult> ExportCountHistoryCsv(
            int? storeId = null,
            PhysicalInventoryStatus? status = null,
            string fiscalYear = null)
        {
            try
            {
                var data = await _physicalInventoryService.ExportCountHistoryCsvAsync(storeId, status, fiscalYear);
                var fileName = $"CountHistory_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                return File(data, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting count history to CSV");
                TempData["Error"] = "Error generating CSV report: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PhysicalInventory/ExportCountHistoryPdf
        [HasPermission(Permission.ExportPhysicalInventory)]
        public async Task<IActionResult> ExportCountHistoryPdf(
            int? storeId = null,
            PhysicalInventoryStatus? status = null,
            string fiscalYear = null)
        {
            try
            {
                var data = await _physicalInventoryService.ExportCountHistoryPdfAsync(storeId, status, fiscalYear);
                var fileName = $"CountHistory_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(data, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting count history to PDF");
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}