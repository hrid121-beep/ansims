using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StockAdjustmentController : Controller
    {
        private readonly IStockAdjustmentService _stockAdjustmentService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;

        public StockAdjustmentController(
            IStockAdjustmentService stockAdjustmentService,
            IItemService itemService,
            IStoreService storeService)
        {
            _stockAdjustmentService = stockAdjustmentService;
            _itemService = itemService;
            _storeService = storeService;
        }

        public async Task<IActionResult> Index()
        {
            var adjustments = await _stockAdjustmentService.GetAllStockAdjustmentsAsync();
            ViewBag.Statistics = await _stockAdjustmentService.GetAdjustmentStatisticsAsync();
            return View(adjustments);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.AdjustmentNo = await _stockAdjustmentService.GenerateAdjustmentNoAsync();
            await LoadViewBagData();
            return View(new StockAdjustmentDto { AdjustmentDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockAdjustmentDto adjustmentDto)
        {
            // Additional validation beyond DataAnnotations
            if (adjustmentDto.ItemId <= 0)
            {
                ModelState.AddModelError("ItemId", "Please select a valid item");
            }

            if (!adjustmentDto.StoreId.HasValue || adjustmentDto.StoreId.Value <= 0)
            {
                ModelState.AddModelError("StoreId", "Please select a valid store");
            }

            if (adjustmentDto.NewQuantity.HasValue && adjustmentDto.OldQuantity.HasValue
                && adjustmentDto.NewQuantity.Value == adjustmentDto.OldQuantity.Value)
            {
                ModelState.AddModelError("NewQuantity", "New quantity cannot be the same as current quantity");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _stockAdjustmentService.CreateStockAdjustmentAsync(adjustmentDto);
                    TempData["Success"] = "Stock adjustment created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException argEx)
                {
                    ModelState.AddModelError(string.Empty, argEx.Message);
                    TempData["Error"] = argEx.Message;
                }
                catch (InvalidOperationException invEx)
                {
                    ModelState.AddModelError(string.Empty, invEx.Message);
                    TempData["Error"] = invEx.Message;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the stock adjustment.");
                    TempData["Error"] = $"Error: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "Please correct the validation errors and try again.";
            }

            ViewBag.AdjustmentNo = adjustmentDto.AdjustmentNo;
            await LoadViewBagData();
            return View(adjustmentDto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var adjustment = await _stockAdjustmentService.GetStockAdjustmentByIdAsync(id);
            if (adjustment == null)
            {
                return NotFound();
            }
            return View(adjustment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _stockAdjustmentService.ApproveAdjustmentAsync(id, User.Identity.Name);
                TempData["Success"] = "Adjustment approved successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                await _stockAdjustmentService.RejectAdjustmentAsync(id, User.Identity.Name, reason);
                TempData["Success"] = "Adjustment rejected!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentStock(int itemId, int? storeId)
        {
            var stockLevel = await _stockAdjustmentService.GetCurrentStockLevelAsync(itemId, storeId);
            return Json(stockLevel);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemsByStore(int storeId)
        {
            var items = await _itemService.GetItemsByStoreAsync(storeId);
            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var pendingAdjustments = await _stockAdjustmentService.GetPendingAdjustmentsAsync();
            ViewBag.Title = "Pending Adjustments";
            return View("Index", pendingAdjustments);
        }

        [HttpGet]
        public async Task<IActionResult> Report(DateTime? fromDate, DateTime? toDate, int? storeId = null, string adjustmentType = null)
        {
            fromDate ??= DateTime.Today.AddMonths(-1);
            toDate ??= DateTime.Today;

            var adjustments = await _stockAdjustmentService.GetAdjustmentsByDateRangeAsync(fromDate.Value, toDate.Value);

            // Filter by store if provided
            if (storeId.HasValue)
            {
                adjustments = adjustments.Where(a => a.StoreId == storeId.Value).ToList();
            }

            // Filter by adjustment type if provided
            if (!string.IsNullOrEmpty(adjustmentType))
            {
                adjustments = adjustments.Where(a => a.AdjustmentType == adjustmentType).ToList();
            }

            ViewBag.Statistics = await _stockAdjustmentService.GetAdjustmentStatisticsAsync(fromDate, toDate);
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            // Load stores for dropdown
            var stores = await _storeService.GetAllStoresAsync();
            ViewBag.Stores = new SelectList(stores, "Id", "Name", storeId);

            return View(adjustments);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf(DateTime? fromDate, DateTime? toDate)
        {
            fromDate ??= DateTime.Today.AddMonths(-1);
            toDate ??= DateTime.Today;

            var adjustments = await _stockAdjustmentService.GetAdjustmentsByDateRangeAsync(fromDate.Value, toDate.Value);
            var statistics = await _stockAdjustmentService.GetAdjustmentStatisticsAsync(fromDate, toDate);

            // TODO: Implement PDF generation using iTextSharp or similar library
            // For now, return NotImplemented
            TempData["Info"] = "PDF export feature will be implemented soon";
            return RedirectToAction(nameof(Report), new { fromDate, toDate });
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(DateTime? fromDate, DateTime? toDate)
        {
            fromDate ??= DateTime.Today.AddMonths(-1);
            toDate ??= DateTime.Today;

            var adjustments = await _stockAdjustmentService.GetAdjustmentsByDateRangeAsync(fromDate.Value, toDate.Value);
            var statistics = await _stockAdjustmentService.GetAdjustmentStatisticsAsync(fromDate, toDate);

            // TODO: Implement Excel generation using EPPlus or ClosedXML
            // For now, return NotImplemented
            TempData["Info"] = "Excel export feature will be implemented soon";
            return RedirectToAction(nameof(Report), new { fromDate, toDate });
        }

        private async Task LoadViewBagData()
        {
            var items = await _itemService.GetAllItemsAsync();
            var stores = await _storeService.GetAllStoresAsync();

            ViewBag.Items = new SelectList(items, "Id", "Name");
            ViewBag.Stores = new SelectList(stores, "Id", "Name");

            ViewBag.AdjustmentReasons = new SelectList(new[]
            {
                "Physical Count Variance",
                "Damaged Goods",
                "Expired Products",
                "System Error Correction",
                "Stock Take Adjustment",
                "Quality Issues",
                "Documentation Error",
                "Initial Stock Entry",
                "Other"
            });
        }
    }
}