using IMS.Application.DTOs;
using IMS.Application.Extensions;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StockMovementController : Controller
    {
        private readonly IStockMovementService _stockMovementService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;
        private readonly ILogger<StockMovementController> _logger;

        public StockMovementController(
            IStockMovementService stockMovementService,
            IStoreService storeService,
            IItemService itemService,
            ILogger<StockMovementController> logger)
        {
            _stockMovementService = stockMovementService;
            _storeService = storeService;
            _itemService = itemService;
            _logger = logger;
        }

        // GET: StockMovement
        [HasPermission(Permission.ViewStockMovement)]
        public async Task<IActionResult> Index(
            int? storeId = null,
            int? itemId = null,
            string movementType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1)
        {
            try
            {
                var pageSize = 20;

                // Set default date range if not provided
                fromDate ??= DateTime.Now.AddMonths(-1);
                toDate ??= DateTime.Now;

                var movements = await _stockMovementService.GetStockMovementsAsync(
                    pageNumber, pageSize, storeId, itemId, movementType, fromDate.Value, toDate.Value);

                ViewBag.Stores = new SelectList(
                    await _storeService.GetActiveStoresAsync(), "Id", "Name", storeId);
                ViewBag.Items = new SelectList(
                    await _itemService.GetActiveItemsAsync(), "Id", "Name", itemId);
                ViewBag.MovementTypes = new SelectList(new[]
                {
                    new { Value = "IN", Text = "Stock In" },
                    new { Value = "OUT", Text = "Stock Out" },
                    new { Value = "TRANSFER", Text = "Transfer" },
                    new { Value = "ADJUSTMENT", Text = "Adjustment" },
                    new { Value = "PHYSICAL_COUNT", Text = "Physical Count" },
                    new { Value = "RETURN", Text = "Return" },
                    new { Value = "WRITE_OFF", Text = "Write-off" }
                }, "Value", "Text", movementType);

                ViewBag.CurrentStoreId = storeId;
                ViewBag.CurrentItemId = itemId;
                ViewBag.CurrentMovementType = movementType;
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

                return View(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock movements");
                TempData["Error"] = "An error occurred while loading stock movements.";
                return View(new PagedResult<StockMovementDto>());
            }
        }

        // GET: StockMovement/Details/5
        [HasPermission(Permission.ViewStockMovement)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var movement = await _stockMovementService.GetStockMovementByIdAsync(id);
                if (movement == null)
                {
                    TempData["Error"] = "Stock movement not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(movement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock movement details");
                TempData["Error"] = "An error occurred while loading movement details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: StockMovement/ItemHistory/5
        [HasPermission(Permission.ViewStockMovement)]
        public async Task<IActionResult> ItemHistory(int itemId, int? storeId = null)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(itemId);
                if (item == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                var movements = await _stockMovementService.GetItemMovementHistoryAsync(itemId, storeId);

                ViewBag.Item = item;
                ViewBag.StoreId = storeId;
                if (storeId.HasValue)
                {
                    var store = await _storeService.GetStoreByIdAsync(storeId.Value);
                    ViewBag.Store = store;
                }

                return View(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading item movement history");
                TempData["Error"] = "An error occurred while loading movement history.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: StockMovement/StoreHistory/5
        [HasPermission(Permission.ViewStockMovement)]
        public async Task<IActionResult> StoreHistory(int storeId, DateTime? date = null)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(storeId);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                date ??= DateTime.Today;
                var movements = await _stockMovementService.GetStoreMovementHistoryAsync(storeId, date.Value);

                ViewBag.Store = store;
                ViewBag.Date = date.Value.ToString("yyyy-MM-dd");

                return View(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading store movement history");
                TempData["Error"] = "An error occurred while loading movement history.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: StockMovement/Export
        [HasPermission(Permission.ExportStockMovement)]
        public async Task<IActionResult> Export(
            int? storeId = null,
            int? itemId = null,
            string movementType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string format = "excel")
        {
            try
            {
                fromDate ??= DateTime.Now.AddMonths(-1);
                toDate ??= DateTime.Now;

                var bytes = await _stockMovementService.ExportMovementsAsync(
                    storeId, itemId, movementType, fromDate.Value, toDate.Value, format);

                var contentType = format == "excel"
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "application/pdf";
                var fileName = $"stock_movements_{DateTime.Now:yyyyMMdd}.{(format == "excel" ? "xlsx" : "pdf")}";

                return File(bytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stock movements");
                TempData["Error"] = "An error occurred while exporting movements.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: StockMovement/Summary
        [HasPermission(Permission.ViewStockMovement)]
        public async Task<IActionResult> Summary(int? storeId = null, DateTime? date = null)
        {
            try
            {
                date ??= DateTime.Today;
                var summary = await _stockMovementService.GetMovementSummaryAsync(storeId, date.Value);

                ViewBag.Stores = new SelectList(
                    await _storeService.GetActiveStoresAsync(), "Id", "Name", storeId);
                ViewBag.Date = date.Value.ToString("yyyy-MM-dd");
                ViewBag.StoreId = storeId;

                return View(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading movement summary");
                TempData["Error"] = "An error occurred while loading summary.";
                return View(new StockMovementSummaryDto());
            }
        }

        // AJAX: Get movement details for modal
        [HttpGet]
        public async Task<IActionResult> GetMovementDetails(int id)
        {
            try
            {
                var movement = await _stockMovementService.GetStockMovementByIdAsync(id);
                if (movement == null)
                {
                    return Json(new { success = false, message = "Movement not found" });
                }

                return Json(new { success = true, data = movement });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movement details");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        // AJAX: Get stock balance at date
        [HttpGet]
        public async Task<IActionResult> GetStockBalance(int itemId, int storeId, DateTime date)
        {
            try
            {
                var balance = await _stockMovementService.GetStockBalanceAtDateAsync(itemId, storeId, date);
                return Json(new { success = true, balance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock balance");
                return Json(new { success = false, message = "An error occurred" });
            }
        }
    }
}