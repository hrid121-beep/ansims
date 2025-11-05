using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly IIssueService _issueService;
        private readonly IPurchaseService _purchaseService;
        private readonly IRequisitionService _requisitionService;
        private readonly IReceiveService _receiveService;
        private readonly IPhysicalInventoryService _physicalInventoryService;
        private readonly IBatchTrackingService _batchTrackingService;
        private readonly IOrganizationService _organizationService;
        private readonly IUserService _userService;
        private readonly IActivityLogService _activityLogService;
        private readonly IApprovalService _approvalService;
        private readonly ICategoryService _categoryService;
        private readonly IStockService _stockService;
        private readonly IStockAlertService _stockAlertService;

        public HomeController(
            ILogger<HomeController> logger,
            IItemService itemService,
            IStoreService storeService,
            IIssueService issueService,
            IPurchaseService purchaseService,
            IRequisitionService requisitionService,
            IReceiveService receiveService,
            IPhysicalInventoryService physicalInventoryService,
            IBatchTrackingService batchTrackingService,
            IOrganizationService organizationService,
            IUserService userService,
            IActivityLogService activityLogService,
            IApprovalService approvalService,
            ICategoryService categoryService,
            IStockService stockService,
            IStockAlertService stockAlertService)
        {
            _logger = logger;
            _itemService = itemService;
            _storeService = storeService;
            _issueService = issueService;
            _purchaseService = purchaseService;
            _requisitionService = requisitionService;
            _receiveService = receiveService;
            _physicalInventoryService = physicalInventoryService;
            _batchTrackingService = batchTrackingService;
            _organizationService = organizationService;
            _userService = userService;
            _activityLogService = activityLogService;
            _approvalService = approvalService;
            _categoryService = categoryService;
            _stockService = stockService;
            _stockAlertService = stockAlertService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new DashboardViewModel();

                // Basic Statistics
                var items = await _itemService.GetAllItemsAsync();
                model.TotalItems = items.Count();
                model.ActiveItems = items.Count(i => i.IsActive);

                var stores = await _storeService.GetAllStoresAsync();
                model.TotalStores = stores.Count(s => s.IsActive);

                var users = await _userService.GetAllUsersAsync();
                model.TotalUsers = users.Count(u => u.IsActive);

                // Stock Value and Accuracy
                model.TotalStockValue = await _stockService.GetTotalStockValueAsync();
                model.StockAccuracy = await _stockService.GetStockAccuracyPercentageAsync();

                // Alerts
                model.LowStockItems = items.Count(i => i.CurrentStock < i.MinimumStock);
                model.CriticalAlerts = items.Count(i => i.CurrentStock <= 0) +
                                      items.Count(i => i.CurrentStock < i.MinimumStock * 0.5m);

                // Today's Transactions
                var todayStart = DateTime.Today;
                var yesterdayStart = DateTime.Today.AddDays(-1);

                var todayIssues = await _issueService.GetIssuesByDateRangeAsync(todayStart, DateTime.Now);
                var todayReceives = await _receiveService.GetReceivesByDateRangeAsync(todayStart, DateTime.Now);
                var todayPurchases = await _purchaseService.GetPurchasesByDateRangeAsync(todayStart, DateTime.Now);

                model.TodayTransactions = todayIssues.Count() + todayReceives.Count() + todayPurchases.Count();

                // Calculate growth
                var yesterdayIssues = await _issueService.GetIssuesByDateRangeAsync(yesterdayStart, todayStart);
                var yesterdayReceives = await _receiveService.GetReceivesByDateRangeAsync(yesterdayStart, todayStart);
                var yesterdayPurchases = await _purchaseService.GetPurchasesByDateRangeAsync(yesterdayStart, todayStart);
                var yesterdayTotal = yesterdayIssues.Count() + yesterdayReceives.Count() + yesterdayPurchases.Count();

                if (yesterdayTotal > 0)
                {
                    model.TransactionGrowth = (int)(((model.TodayTransactions - yesterdayTotal) / (decimal)yesterdayTotal) * 100);
                }

                // Requisitions
                var requisitions = await _requisitionService.GetAllRequisitionsAsync();
                model.PendingRequisitions = requisitions.Count(r => r.Status == "Pending");

                // Purchases
                var purchases = await _purchaseService.GetAllPurchasesAsync();
                model.ActivePurchases = purchases.Count(p => p.Status == "InProgress" || p.Status == "Approved");
                model.PurchaseValue = purchases.Where(p => p.Status == "InProgress" || p.Status == "Approved")
                                              .Sum(p => p.TotalAmount);

                // Approval Counts (for Officers/Admin)
                if (User.IsInRole("Officer") || User.IsInRole("Admin"))
                {
                    var approvals = await _approvalService.GetPendingApprovalsAsync(User.Identity.Name);
                    model.PendingRequisitionApprovals = approvals.Count(a => a.Type == "Requisition");
                    model.PendingPurchaseApprovals = approvals.Count(a => a.Type == "Purchase");
                    model.PendingIssueApprovals = approvals.Count(a => a.Type == "Issue");
                    model.PendingAdjustmentApprovals = approvals.Count(a => a.Type == "Adjustment");
                    model.TotalPendingApprovals = approvals.Count();
                }

                // Physical Inventory
                var physicalCounts = await _physicalInventoryService.GetAllCountsAsync();
                model.ScheduledCounts = physicalCounts.Count(pc => pc.Status == PhysicalInventoryStatus.Scheduled);
                model.InProgressCounts = physicalCounts.Count(pc => pc.Status == PhysicalInventoryStatus.InProgress);
                model.PendingCountReview = physicalCounts.Count(pc => pc.Status == PhysicalInventoryStatus.PendingReview);

                // Expiring Items
                var expiringBatches = await _batchTrackingService.GetExpiringBatchesAsync(30); // Next 30 days
                model.ExpiringItems = expiringBatches.Count();
                model.ExpiringItemsList = expiringBatches.Select(b => new ExpiringItemDto
                {
                    Id = b.Id,
                    BatchNumber = b.BatchNumber,
                    ItemName = b.ItemName,
                    ExpiryDate = b.ExpiryDate.Value,
                    DaysRemaining = (b.ExpiryDate.Value - DateTime.Today).Days,
                    Quantity = (int)b.Quantity,
                    StoreName = b.StoreName
                }).OrderBy(e => e.DaysRemaining).Take(5).ToList();

                // Organization Stats
                var organization = await _organizationService.GetOrganizationStatsAsync();
                model.TotalRanges = organization.RangeCount;
                model.TotalBattalions = organization.BattalionCount;
                model.TotalZilas = organization.ZilaCount;
                model.TotalUpazilas = organization.UpazilaCount;

                // Performance Metrics
                model.InventoryTurnover = (int)await _stockService.GetInventoryTurnoverAsync();
                model.FillRate = (int)await _stockService.GetFillRatePercentageAsync();
                model.AverageLeadTime = (int)await _stockService.GetAverageLeadTimeDaysAsync();

                // Recent Activities (last 30 days)
                var issues = await _issueService.GetAllIssuesAsync();
                model.PendingIssues = issues.Count(i => i.Status == "Pending");
                model.RecentIssues = issues.OrderByDescending(i => i.IssueDate)
                                          .Take(5)
                                          .ToList();

                model.RecentPurchases = purchases.OrderByDescending(p => p.PurchaseDate)
                                                .Take(5)
                                                .ToList();

                // Low Stock Items
                model.LowStockItemsList = items.Where(i => i.CurrentStock < i.MinimumStock)
                                               .OrderBy(i => i.CurrentStock)
                                               .Take(10)
                                               .ToList();

                // Critical Stock Alerts
                try
                {
                    var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                    var alertsDashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);
                    model.CriticalAlertsList = alertsDashboard.CriticalAlerts.Take(10).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load critical alerts");
                    model.CriticalAlertsList = new List<StockAlertDto>();
                }

                // Activity Log
                var activities = await _activityLogService.GetRecentActivitiesAsync(10);
                model.RecentActivities = activities.Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Icon = GetActivityIcon(a.Action),
                    Color = GetActivityColor(a.Action),
                    Title = a.Action,
                    Description = a.Description,
                    TimeAgo = GetTimeAgo(a.Timestamp),  // Fixed: Use Timestamp instead of CreatedDate
                    ActionUrl = GetActivityUrl(a),
                    ActivityDate = a.Timestamp  // Fixed: Use Timestamp instead of CreatedDate
                }).ToList();

                // Chart Data - Last 30 days
                await PopulateChartData(model);

                // System Alerts
                await PopulateSystemAlerts();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View(new DashboardViewModel());
            }
        }

        private async Task PopulateChartData(DashboardViewModel model)
        {
            // Last 7 days for chart
            var dates = new List<string>();
            var issueData = new List<int>();
            var receiveData = new List<int>();
            var purchaseData = new List<int>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                dates.Add(date.ToString("MMM dd"));

                var dayIssues = await _issueService.GetIssuesByDateRangeAsync(date, date.AddDays(1));
                var dayReceives = await _receiveService.GetReceivesByDateRangeAsync(date, date.AddDays(1));
                var dayPurchases = await _purchaseService.GetPurchasesByDateRangeAsync(date, date.AddDays(1));

                issueData.Add(dayIssues.Count());
                receiveData.Add(dayReceives.Count());
                purchaseData.Add(dayPurchases.Count());
            }

            model.ChartLabels = dates;
            model.IssueData = issueData;
            model.ReceiveData = receiveData;
            model.PurchaseData = purchaseData;

            // Category Distribution
            var categories = await _categoryService.GetAllCategoriesAsync();
            var categoryData = new List<int>();
            var categoryLabels = new List<string>();

            foreach (var category in categories.Take(6))
            {
                var itemCount = await _itemService.GetItemCountByCategoryAsync(category.Id);
                if (itemCount > 0)
                {
                    categoryLabels.Add(category.Name);
                    categoryData.Add(itemCount);
                }
            }

            model.CategoryLabels = categoryLabels;
            model.CategoryData = categoryData;
        }

        private async Task PopulateSystemAlerts()
        {
            var alerts = new List<string>();

            // Check for critical stock
            var criticalItems = await _itemService.GetCriticalStockItemsAsync();
            if (criticalItems.Any())
            {
                alerts.Add($"{criticalItems.Count()} items are critically low on stock");
            }

            // Check for pending counts
            var pendingCounts = await _physicalInventoryService.GetPendingCountsAsync();
            if (pendingCounts.Any())
            {
                alerts.Add($"{pendingCounts.Count()} physical counts are pending completion");
            }

            // Check for expiring items
            var expiringItems = await _batchTrackingService.GetExpiringBatchesAsync(7);
            if (expiringItems.Any())
            {
                alerts.Add($"{expiringItems.Count()} items are expiring within 7 days");
            }

            ViewBag.SystemAlerts = alerts;
        }

        private string GetActivityIcon(string action)
        {
            return action switch
            {
                var a when a.Contains("Create") => "plus",
                var a when a.Contains("Update") => "edit",
                var a when a.Contains("Delete") => "trash",
                var a when a.Contains("Approve") => "check",
                var a when a.Contains("Issue") => "share",
                var a when a.Contains("Receive") => "inbox",
                var a when a.Contains("Transfer") => "exchange-alt",
                _ => "circle"
            };
        }

        private string GetActivityColor(string action)
        {
            return action switch
            {
                var a when a.Contains("Create") => "success",
                var a when a.Contains("Update") => "info",
                var a when a.Contains("Delete") => "danger",
                var a when a.Contains("Approve") => "primary",
                var a when a.Contains("Issue") => "warning",
                _ => "secondary"
            };
        }

        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.Now - date;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} days ago";

            return date.ToString("MMM dd, yyyy");
        }

        private string GetActivityUrl(ActivityLogDto activity)
        {
            if (string.IsNullOrEmpty(activity.EntityType) || activity.EntityId == 0)
                return "#";

            return activity.EntityType switch
            {
                "Issue" => Url.Action("Details", "Issue", new { id = activity.EntityId }),
                "Purchase" => Url.Action("Details", "Purchase", new { id = activity.EntityId }),
                "Receive" => Url.Action("Details", "Receive", new { id = activity.EntityId }),
                "Transfer" => Url.Action("Details", "Transfer", new { id = activity.EntityId }),
                "Item" => Url.Action("Details", "Item", new { id = activity.EntityId }),
                _ => "#"
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Officer")]
        public async Task<IActionResult> SendStockAlerts()
        {
            var stockAlertService = HttpContext.RequestServices.GetService<IStockAlertService>();
            await stockAlertService.CheckAndSendLowStockAlertsAsync();

            TempData["Success"] = "Stock alert emails sent successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExportDashboard()
        {
            // Implementation for dashboard export
            return File(new byte[0], "application/vnd.ms-excel", $"Dashboard_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        // API endpoints for AJAX calls
        [HttpGet]
        public async Task<JsonResult> GetNotifications()
        {
            var notifications = await _activityLogService.GetUnreadNotificationsAsync(User.Identity.Name);
            return Json(new
            {
                lowStock = await _itemService.GetLowStockCountAsync(),
                expiring = await _batchTrackingService.GetExpiringCountAsync(7)
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetPendingCounts()
        {
            var result = new
            {
                requisitions = 0,
                purchases = 0,
                issues = 0,
                adjustments = 0
            };

            if (User.IsInRole("Officer") || User.IsInRole("Admin"))
            {
                var approvals = await _approvalService.GetPendingApprovalsAsync(User.Identity.Name);
                result = new
                {
                    requisitions = approvals.Count(a => a.Type == "Requisition"),
                    purchases = approvals.Count(a => a.Type == "Purchase"),
                    issues = approvals.Count(a => a.Type == "Issue"),
                    adjustments = approvals.Count(a => a.Type == "Adjustment")
                };
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetStockMovementData(string period = "week")
        {
            try
            {
                int days = period.ToLower() switch
                {
                    "week" => 7,
                    "month" => 30,
                    "year" => 365,
                    _ => 7
                };

                var dates = new List<string>();
                var issueData = new List<int>();
                var receiveData = new List<int>();
                var purchaseData = new List<int>();

                // Determine date format and interval
                string dateFormat = period.ToLower() switch
                {
                    "year" => "MMM",
                    "month" => "MMM dd",
                    _ => "MMM dd"
                };

                int interval = period.ToLower() switch
                {
                    "year" => 30, // Monthly intervals for year view
                    "month" => 5, // Every 5 days for month view
                    _ => 1 // Daily for week view
                };

                // Generate data points
                for (int i = days; i >= 0; i -= interval)
                {
                    var startDate = DateTime.Today.AddDays(-i);
                    var endDate = startDate.AddDays(interval);
                    dates.Add(startDate.ToString(dateFormat));

                    var dayIssues = await _issueService.GetIssuesByDateRangeAsync(startDate, endDate);
                    var dayReceives = await _receiveService.GetReceivesByDateRangeAsync(startDate, endDate);
                    var dayPurchases = await _purchaseService.GetPurchasesByDateRangeAsync(startDate, endDate);

                    issueData.Add(dayIssues.Count());
                    receiveData.Add(dayReceives.Count());
                    purchaseData.Add(dayPurchases.Count());
                }

                return Json(new
                {
                    labels = dates,
                    issues = issueData,
                    receives = receiveData,
                    purchases = purchaseData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock movement data");
                return Json(new { labels = new List<string>(), issues = new List<int>(), receives = new List<int>(), purchases = new List<int>() });
            }
        }

    }
}