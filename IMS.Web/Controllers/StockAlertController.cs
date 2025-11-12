using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StockAlertController : Controller
    {
        private readonly IStockAlertService _stockAlertService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;
        private readonly ILogger<StockAlertController> _logger;

        public StockAlertController(
            IStockAlertService stockAlertService,
            IStoreService storeService,
            IItemService itemService,
            ILogger<StockAlertController> logger)
        {
            _stockAlertService = stockAlertService;
            _storeService = storeService;
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string level = null, int? storeId = null, string search = null, int page = 1)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get all alerts for the user
                var dashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);

                var alerts = new List<StockAlertDto>();
                alerts.AddRange(dashboard.CriticalAlerts);
                alerts.AddRange(dashboard.WarningAlerts);
                alerts.AddRange(dashboard.InfoAlerts);

                // Apply filters
                if (!string.IsNullOrEmpty(level))
                {
                    alerts = alerts.Where(a => a.AlertLevel?.Equals(level, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }

                if (storeId.HasValue)
                {
                    alerts = alerts.Where(a => a.StoreId == storeId.Value).ToList();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    alerts = alerts.Where(a =>
                        a.ItemName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                        a.ItemCode?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                        a.StoreName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true
                    ).ToList();
                }

                // Populate ViewBag for filters
                ViewBag.Stores = await _storeService.GetAllStoresAsync();
                ViewBag.SelectedLevel = level;
                ViewBag.SelectedStoreId = storeId;
                ViewBag.SearchQuery = search;
                ViewBag.CurrentPage = page;

                // Pagination
                int pageSize = 20;
                var paginatedAlerts = alerts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.TotalPages = (int)Math.Ceiling(alerts.Count / (double)pageSize);

                return View(paginatedAlerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock alerts");
                TempData["Error"] = "Failed to load stock alerts. Please try again.";
                return View(new List<StockAlertDto>());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,LogisticsOfficer,StoreManager")]
        public async Task<IActionResult> AcknowledgeAlert(int itemId, int? storeId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _stockAlertService.AcknowledgeAlertAsync(itemId, storeId, userId);

                return Json(new { success = true, message = "Alert acknowledged successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acknowledging alert");
                return Json(new { success = false, message = "Failed to acknowledge alert: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,LogisticsOfficer")]
        public async Task<IActionResult> CheckAllStores()
        {
            try
            {
                var alerts = await _stockAlertService.CheckAllStoresForAlertsAsync();
                TempData["Success"] = $"Stock check completed. Found {alerts.Count} alerts.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking all stores");
                TempData["Error"] = "Failed to check all stores: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,LogisticsOfficer")]
        public async Task<IActionResult> SendStockAlerts()
        {
            try
            {
                await _stockAlertService.SendLowStockEmailsAsync();
                TempData["Success"] = "Stock alert emails sent successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending stock alerts");
                TempData["Error"] = "Failed to send stock alerts: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetAlertSummary()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var summary = await _stockAlertService.GetAlertSummaryAsync(userId);

                return Json(new
                {
                    success = true,
                    data = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alert summary");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetRecentAlerts(int count = 10)
        {
            try
            {
                // Return empty result if user is not authenticated
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = true, alerts = new List<object>(), count = 0 });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);

                var recentAlerts = dashboard.CriticalAlerts
                    .OrderByDescending(a => a.AlertDate)
                    .Take(count)
                    .Select(a => new
                    {
                        a.ItemName,
                        a.StoreName,
                        a.CurrentStock,
                        a.MinimumStock,
                        a.AlertLevel,
                        a.ItemId,
                        a.StoreId,
                        TimeAgo = GetTimeAgo(a.AlertDate)
                    })
                    .ToList();

                return Json(new
                {
                    success = true,
                    alerts = recentAlerts,
                    count = dashboard.CriticalAlerts.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent alerts");
                return Json(new { success = false, message = ex.Message, alerts = new List<object>(), count = 0 });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Export(string level = null, int? storeId = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);

                var alerts = new List<StockAlertDto>();
                alerts.AddRange(dashboard.CriticalAlerts);
                alerts.AddRange(dashboard.WarningAlerts);
                alerts.AddRange(dashboard.InfoAlerts);

                // Apply filters
                if (!string.IsNullOrEmpty(level))
                {
                    alerts = alerts.Where(a => a.AlertLevel?.Equals(level, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }

                if (storeId.HasValue)
                {
                    alerts = alerts.Where(a => a.StoreId == storeId.Value).ToList();
                }

                // Create CSV
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Item Code,Item Name,Store,Alert Level,Current Stock,Minimum Stock,Alert Date,Status");

                foreach (var alert in alerts)
                {
                    csv.AppendLine($"{alert.ItemCode},{alert.ItemName},{alert.StoreName},{alert.AlertLevel},{alert.CurrentStock},{alert.MinimumStock},{alert.AlertDate:yyyy-MM-dd},{alert.Status}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"StockAlerts_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stock alerts");
                TempData["Error"] = "Failed to export alerts: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.Now - date;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} min ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hr ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} days ago";

            return date.ToString("MMM dd, yyyy");
        }
    }
}
