using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IPurchaseService _purchaseService;
        private readonly IIssueService _issueService;
        private readonly IStockAlertService _stockAlertService;
        private readonly IActivityLogService _activityLogService;
        private readonly IDashboardService _dashboardService;

        private readonly IApprovalService _approvalService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        // Update your constructor to include these dependencies:
        public DashboardController(
            IItemService itemService,
            IPurchaseService purchaseService,
            IIssueService issueService,
            IStockAlertService stockAlertService,
            IActivityLogService activityLogService,
            IDashboardService dashboardService,
            IApprovalService approvalService,  // Add this
            IUnitOfWork unitOfWork,           // Add this
            UserManager<User> userManager)    // Add this
        {
            _itemService = itemService;
            _purchaseService = purchaseService;
            _issueService = issueService;
            _stockAlertService = stockAlertService;
            _activityLogService = activityLogService;
            _dashboardService = dashboardService;
            _approvalService = approvalService;    // Add this
            _unitOfWork = unitOfWork;             // Add this
            _userManager = userManager;           // Add this
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get dashboard statistics
                var stats = await _dashboardService.GetDashboardStatsAsync();

                // Get recent activities
                var activities = await _dashboardService.GetRecentActivitiesAsync(10);

                // Get alerts
                var alerts = await _dashboardService.GetAlertsAsync();

                // Calculate valuation summary
                var currentValue = stats.TotalInventoryValue ?? 0;
                var lastMonthValue = await GetLastMonthInventoryValueAsync();
                var valueChange = currentValue - lastMonthValue;
                var percentageChange = lastMonthValue > 0 ? (valueChange / lastMonthValue) * 100 : 0;

                var model = new DashboardViewModel
                {
                    // Basic Statistics
                    TotalItems = stats.TotalItems,
                    LowStockItems = stats.LowStockItems,

                    // Stats object for view compatibility
                    Stats = new {
                        PurchaseOrders = stats.PurchaseOrders,
                        TotalInventoryValue = stats.TotalInventoryValue ?? 0
                    },

                    // Alerts - map AlertDto to DashboardAlertDto format expected by view
                    Alerts = alerts.Select(a => new DashboardAlertDto
                    {
                        Type = a.Type,
                        Title = a.Title,
                        Message = a.Message,
                        Link = a.Link,
                        Icon = a.Icon,
                        CreatedAt = a.CreatedAt
                    }).ToList(),

                    // Recent Activities - map RecentActivityDto to ActivityDto format
                    RecentActivities = activities.Select(a => new ActivityDto
                    {
                        Icon = a.Icon,
                        Color = a.Color,
                        Title = a.Title,
                        Description = a.Description,
                        ActivityDate = a.Timestamp,
                        ActionUrl = a.Link,
                        TimeAgo = GetTimeAgo(a.Timestamp)
                    }).ToList(),

                    // Valuation Summary
                    ValuationSummary = new ValuationSummaryDto
                    {
                        CurrentValue = currentValue,
                        ValueChange = valueChange,
                        PercentageChange = percentageChange,
                        LastUpdated = DateTime.Now
                    }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // Log error
                TempData["Error"] = "An error occurred while loading the dashboard.";

                // Return view with empty model
                return View(new DashboardViewModel
                {
                    Alerts = new List<DashboardAlertDto>(),
                    RecentActivities = new List<ActivityDto>(),
                    Stats = new { PurchaseOrders = 0, TotalInventoryValue = 0m },
                    ValuationSummary = new ValuationSummaryDto
                    {
                        CurrentValue = 0,
                        ValueChange = 0,
                        PercentageChange = 0,
                        LastUpdated = DateTime.Now
                    }
                });
            }
        }

        private async Task<decimal> GetLastMonthInventoryValueAsync()
        {
            try
            {
                // Calculate inventory value from one month ago
                // This is a simplified calculation - you may want to enhance this
                var oneMonthAgo = DateTime.Now.AddMonths(-1);

                // Get stock movements from last month
                var movements = await _unitOfWork.StockMovements
                    .Query()
                    .Where(sm => sm.CreatedAt <= oneMonthAgo)
                    .ToListAsync();

                // For now, return 90% of current value as estimate
                // You should implement proper historical tracking
                var currentValue = (await _dashboardService.GetDashboardStatsAsync()).TotalInventoryValue ?? 0;
                return currentValue * 0.90m;
            }
            catch
            {
                return 0;
            }
        }

        private string GetTimeAgo(DateTime timestamp)
        {
            var timeSpan = DateTime.Now - timestamp;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) != 1 ? "s" : "")} ago";

            return timestamp.ToString("MMM dd, yyyy");
        }
        // Replace the GetPendingCounts method in DashboardController with this version using BatchTrackings:

        [HttpGet]
        [Route("api/dashboard/pendingcounts")]
        public async Task<IActionResult> GetPendingCounts()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new
                    {
                        requisitions = 0,
                        purchases = 0,
                        issues = 0,
                        adjustments = 0,
                        transfers = 0,
                        allotments = 0,
                        lowStock = 0,
                        expiringBatches = 0
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                var userRoles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();

                // Get all pending approvals
                var pendingApprovals = await _unitOfWork.ApprovalRequests
                    .GetAllAsync(a => a.Status == ApprovalStatus.Pending.ToString() && a.IsActive);

                // Count by type, filtering by user's approval authority
                int requisitionCount = 0, purchaseCount = 0, issueCount = 0, adjustmentCount = 0, transferCount = 0, allotmentCount = 0;

                // Admin can see ALL pending approvals
                bool isAdmin = userRoles.Contains("Admin");

                foreach (var approval in pendingApprovals)
                {
                    if (isAdmin)
                    {
                        // Admin sees everything
                        switch (approval.EntityType?.ToUpper())
                        {
                            case "REQUISITION":
                                requisitionCount++;
                                break;
                            case "PURCHASE":
                                purchaseCount++;
                                break;
                            case "ISSUE":
                                issueCount++;
                                break;
                            case "STOCK_ADJUSTMENT":
                                adjustmentCount++;
                                break;
                            case "TRANSFER":
                                transferCount++;
                                break;
                            case "ALLOTMENT_LETTER":
                                allotmentCount++;
                                break;
                        }
                    }
                    else
                    {
                        // Other roles check threshold
                        var threshold = await _approvalService.GetApprovalRequirementAsync(
                            approval.EntityType,
                            approval.Amount ?? 0m);

                        if (threshold != null && userRoles.Contains(threshold.RequiredRole))
                        {
                            switch (approval.EntityType?.ToUpper())
                            {
                                case "REQUISITION":
                                    requisitionCount++;
                                    break;
                                case "PURCHASE":
                                    purchaseCount++;
                                    break;
                                case "ISSUE":
                                    issueCount++;
                                    break;
                                case "STOCK_ADJUSTMENT":
                                    adjustmentCount++;
                                    break;
                                case "TRANSFER":
                                    transferCount++;
                                    break;
                                case "ALLOTMENT_LETTER":
                                    allotmentCount++;
                                    break;
                            }
                        }
                    }
                }

                // Get low stock count
                var lowStockCount = await _unitOfWork.StoreItems
                    .CountAsync(si => si.IsActive && si.Quantity <= si.MinimumStock && si.Quantity > 0);

                // Get expiring batches count using BatchTrackings entity
                var expiringBatchCount = 0;
                try
                {
                    var thirtyDaysFromNow = DateTime.Now.AddDays(30);
                    var batchTrackings = await _unitOfWork.BatchTrackings.GetAllAsync(b =>
                        b.IsActive &&
                        b.ExpiryDate.HasValue &&
                        b.ExpiryDate.Value <= thirtyDaysFromNow &&
                        b.ExpiryDate.Value >= DateTime.Now);
                    expiringBatchCount = batchTrackings.Count();
                }
                catch (Exception ex)
                {
                    // Log the error if needed
                    expiringBatchCount = 0;
                }

                return Json(new
                {
                    requisitions = requisitionCount,
                    purchases = purchaseCount,
                    issues = issueCount,
                    adjustments = adjustmentCount,
                    transfers = transferCount,
                    allotments = allotmentCount,
                    lowStock = lowStockCount,
                    expiringBatches = expiringBatchCount
                });
            }
            catch (Exception ex)
            {
                // Log the error
                return Json(new
                {
                    requisitions = 0,
                    purchases = 0,
                    issues = 0,
                    adjustments = 0,
                    transfers = 0,
                    allotments = 0,
                    lowStock = 0,
                    expiringBatches = 0
                });
            }
        }

        private async Task<List<DashboardAlertDto>> GetDashboardAlerts()
        {
            var alerts = new List<DashboardAlertDto>();

            // Low stock alerts
            var lowStockItems = await _stockAlertService.GetLowStockItems();
            foreach (var item in lowStockItems.Take(3))
            {
                alerts.Add(new DashboardAlertDto
                {
                    Type = "danger",
                    Title = $"Low Stock: {item.ItemName}",
                    Message = $"Only {item.CurrentStock} {item.Unit} remaining",
                    Link = Url.Action("Details", "Item", new { id = item.ItemId })
                });
            }

            // Expiring items alerts
            var expiringItems = await _itemService.GetExpiringItems(7);
            foreach (var item in expiringItems.Take(2))
            {
                alerts.Add(new DashboardAlertDto
                {
                    Type = "warning",
                    Title = "Item Expiring Soon",
                    Message = $"{item.ItemName} - Batch: {item.BatchNo} expires in {item.DaysToExpiry} days",
                    Link = Url.Action("ExpiringItems", "ExpiryTracking")
                });
            }

            // Pending approvals
            var pendingCount = await _issueService.GetPendingApprovalsCount();
            if (pendingCount > 0)
            {
                alerts.Add(new DashboardAlertDto
                {
                    Type = "info",
                    Title = "Pending Approvals",
                    Message = $"You have {pendingCount} issues waiting for approval",
                    Link = Url.Action("PendingApprovals", "Issue")
                });
            }

            return alerts;
        }

        [HttpPost]
        public async Task<IActionResult> RefreshDashboard()
        {
            try
            {
                await Task.CompletedTask;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData(string chartType)
        {
            switch (chartType?.ToLower())
            {
                case "purchase":
                    var purchaseData = await _purchaseService.GetMonthlyPurchaseTrend(6);
                    return Json(purchaseData.Select(p => new
                    {
                        label = p.Month.ToString("MMM yyyy"),
                        value = p.Count
                    }));

                case "category":
                    var categoryData = await _itemService.GetItemDistributionByCategory();
                    return Json(categoryData.Select(c => new
                    {
                        label = c.CategoryName,
                        value = c.ItemCount
                    }));

                default:
                    return Json(new List<object>());
            }
        }
    }
}
