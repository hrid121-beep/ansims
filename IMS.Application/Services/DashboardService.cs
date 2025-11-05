using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var cacheKey = "dashboard_stats";
            var cached = await _cacheService.GetAsync<DashboardStatsDto>(cacheKey);
            if (cached != null)
                return cached;

            try
            {
                var stats = new DashboardStatsDto
                {
                    TotalItems = await _unitOfWork.Items.CountAsync(i => i.IsActive),
                    TotalStores = await _unitOfWork.Stores.CountAsync(s => s.IsActive),
                    TotalVendors = await _unitOfWork.Vendors.CountAsync(v => v.IsActive),
                    TotalUsers = await _unitOfWork.Users.CountAsync(u => u.IsActive),

                    PurchaseOrders = await _unitOfWork.Purchases.CountAsync(p => p.IsActive),
                    PendingPurchases = await _unitOfWork.Purchases.CountAsync(p => p.Status == "Pending"),
                    MonthlyPurchases = await _unitOfWork.Purchases.CountAsync(p => p.IsActive && p.CreatedAt >= DateTime.Now.AddMonths(-1)),
                    MonthlyPurchaseValue = await GetMonthlyPurchaseValueAsync(),

                    Issues = await _unitOfWork.Issues.CountAsync(i => i.IsActive),
                    PendingIssues = await _unitOfWork.Issues.CountAsync(i => i.Status == "Pending"),
                    MonthlyIssues = await _unitOfWork.Issues.CountAsync(i => i.IsActive && i.CreatedAt >= DateTime.Now.AddDays(-30)),

                    LowStockItems = await GetLowStockCountAsync(),
                    OutOfStockItems = await GetOutOfStockCountAsync(),

                    TotalInventoryValue = await CalculateTotalInventoryValueAsync(),
                    CategoryStock = new List<CategoryStockDto>(),

                    LastUpdated = DateTime.Now
                };

                await _cacheService.SetAsync(cacheKey, stats, TimeSpan.FromMinutes(5));
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                throw;
            }
        }

        public async Task<IEnumerable<RecentActivityDto>> GetRecentActivitiesAsync(int count = 10)
        {
            try
            {
                var activities = new List<RecentActivityDto>();

                // Recent purchases
                var recentPurchases = await _unitOfWork.Purchases
                    .Query()
                    .Include(p => p.CreatedByUser)
                    .Include(p => p.Vendor)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(count)
                    .ToListAsync();

                foreach (var purchase in recentPurchases)
                {
                    activities.Add(new RecentActivityDto
                    {
                        ActivityType = "Purchase",
                        Icon = "fa-shopping-cart",
                        Color = "primary",
                        Title = $"Purchase Order #{purchase.PurchaseOrderNo}",
                        Description = $"Created by {purchase.CreatedByUser?.FullName} from {purchase.Vendor?.Name ?? "Marketplace"}",
                        Timestamp = purchase?.CreatedAt ?? purchase.PurchaseDate,
                        Link = $"/Purchase/Details/{purchase.Id}"
                    });
                }

                // Recent issues
                var recentIssues = await _unitOfWork.Issues
                    .Query()
                    .Include(i => i.CreatedByUser)
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(count)
                    .ToListAsync();

                foreach (var issue in recentIssues)
                {
                    activities.Add(new RecentActivityDto
                    {
                        ActivityType = "Issue",
                        Icon = "fa-arrow-right",
                        Color = "success",
                        Title = $"Issue #{issue.IssueNo}",
                        Description = $"Items issued to {issue.IssuedTo} by {issue.CreatedByUser?.FullName}",
                        Timestamp = issue?.CreatedAt ?? issue.IssueDate,
                        Link = $"/Issue/Details/{issue.Id}"
                    });
                }

                return activities.OrderByDescending(a => a.Timestamp).Take(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activities");
                return new List<RecentActivityDto>();
            }
        }

        public async Task<IEnumerable<AlertDto>> GetAlertsAsync()
        {
            try
            {
                var alerts = new List<AlertDto>();

                // Low stock alerts
                var lowStockItems = await _unitOfWork.StoreItems
                    .Query()
                    .Include(si => si.Item)
                    .Include(si => si.Store)
                    .Where(si => si.IsActive && si.Quantity <= si.MinimumStock && si.Quantity > 0)
                    .ToListAsync();

                foreach (var item in lowStockItems)
                {
                    alerts.Add(new AlertDto
                    {
                        Type = "warning",
                        Title = "Low Stock Alert",
                        Message = $"{item.Item?.Name} is low in stock at {item.Store?.Name} ({item.Quantity}/{item.MinimumStock})",
                        Icon = "exclamation-circle",
                        Link = $"/StoreItem/Details/{item.Id}",
                        Timestamp = DateTime.Now,
                        CreatedAt = DateTime.Now
                    });
                }

                // Out of stock alerts
                var outOfStockItems = await _unitOfWork.StoreItems
                    .Query()
                    .Include(si => si.Item)
                    .Include(si => si.Store)
                    .Where(si => si.IsActive && si.Quantity == 0)
                    .ToListAsync();

                foreach (var item in outOfStockItems)
                {
                    alerts.Add(new AlertDto
                    {
                        Type = "danger",
                        Title = "Out of Stock",
                        Message = $"{item.Item?.Name} is out of stock at {item.Store?.Name}",
                        Icon = "exclamation-triangle",
                        Link = $"/StoreItem/Details/{item.Id}",
                        Timestamp = DateTime.Now,
                        CreatedAt = DateTime.Now
                    });
                }

                // Expiring items
                var expiringItems = await _unitOfWork.ExpiryTrackings
                    .Query()
                    .Include(e => e.Item)
                    .Where(e => e.IsActive &&
                               e.Status != "Disposed" &&
                               e.ExpiryDate <= DateTime.Now.AddDays(30))
                    .ToListAsync();

                foreach (var item in expiringItems)
                {
                    var daysToExpiry = (item.ExpiryDate - DateTime.Now).Days;
                    alerts.Add(new AlertDto
                    {
                        Type = daysToExpiry <= 0 ? "danger" : "warning",
                        Title = daysToExpiry <= 0 ? "Expired" : "Expiring Soon",
                        Message = $"{item.Item?.Name} (Batch: {item.BatchNumber}) expires in {daysToExpiry} days",
                        Icon = daysToExpiry <= 0 ? "exclamation-triangle" : "exclamation-circle",
                        Link = $"/ExpiryTracking/Details/{item.Id}",
                        Timestamp = DateTime.Now,
                        CreatedAt = DateTime.Now
                    });
                }

                return alerts.OrderBy(a => a.Type == "danger" ? 0 : 1)
                           .ThenBy(a => a.CreatedAt)
                           .Take(10);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alerts");
                return new List<AlertDto>();
            }
        }

        public async Task<IEnumerable<ChartDataDto>> GetPurchaseTrendAsync(int months = 12)
        {
            try
            {
                var startDate = DateTime.Now.AddMonths(-months);
                var purchases = await _unitOfWork.Purchases
                    .Query()
                    .Where(p => p.IsActive && p.CreatedAt >= startDate)
                    .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                    .Select(g => new ChartDataDto
                    {
                        Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Value = g.Sum(p => p.TotalAmount),
                        Count = g.Count(),
                        Amount = g.Sum(p => p.TotalAmount)
                    })
                    .OrderBy(c => c.Label)
                    .ToListAsync();

                return purchases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase trend");
                return new List<ChartDataDto>();
            }
        }

        public async Task<IEnumerable<ChartDataDto>> GetIssueTrendAsync(int months = 12)
        {
            try
            {
                var startDate = DateTime.Now.AddMonths(-months);
                var issues = await _unitOfWork.Issues
                    .Query()
                    .Where(i => i.IsActive && i.CreatedAt >= startDate)
                    .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                    .Select(g => new ChartDataDto
                    {
                        Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Value = g.Count(),
                        Count = g.Count()
                    })
                    .OrderBy(c => c.Label)
                    .ToListAsync();

                // Calculate issue values
                foreach (var chartItem in issues)
                {
                    var monthYear = chartItem.Label.Split('-');
                    var year = int.Parse(monthYear[0]);
                    var month = int.Parse(monthYear[1]);

                    var monthlyIssues = await _unitOfWork.Issues
                        .Query()
                        .Where(i => i.IsActive &&
                                   i.CreatedAt.Year == year &&
                                   i.CreatedAt.Month == month)
                        .ToListAsync();

                    var totalValue = 0m;
                    foreach (var issue in monthlyIssues)
                    {
                        var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.IssueId == issue.Id);
                        foreach (var issueItem in issueItems)
                        {
                            // Get item value from last purchase
                            var lastPurchase = await _unitOfWork.PurchaseItems
                                .FirstOrDefaultAsync(pi => pi.ItemId == issueItem.ItemId);
                            if (lastPurchase != null)
                            {
                                totalValue += issueItem.Quantity * lastPurchase.UnitPrice;
                            }
                        }
                    }
                    chartItem.Amount = totalValue;
                }

                return issues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issue trend");
                return new List<ChartDataDto>();
            }
        }

        public async Task<IEnumerable<ChartDataDto>> GetCategoryDistributionAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var chartData = new List<ChartDataDto>();

                foreach (var category in categories.Where(c => c.IsActive))
                {
                    var categoryValue = 0m;
                    var categoryItemCount = 0;

                    var subCategories = await _unitOfWork.SubCategories.FindAsync(sc => sc.CategoryId == category.Id && sc.IsActive);
                    foreach (var subCategory in subCategories)
                    {
                        var items = await _unitOfWork.Items.FindAsync(i => i.SubCategoryId == subCategory.Id && i.IsActive);
                        categoryItemCount += items.Count();

                        foreach (var item in items)
                        {
                            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);
                            var totalStock = storeItems.Sum(si => si.Quantity);

                            // Calculate value based on last purchase price or unit price
                            var lastPurchase = await _unitOfWork.PurchaseItems
                                .FirstOrDefaultAsync(pi => pi.ItemId == item.Id);
                            var unitPrice = lastPurchase?.UnitPrice ?? item.UnitPrice ;
                            categoryValue += (decimal)(totalStock * unitPrice);
                        }
                    }

                    chartData.Add(new ChartDataDto
                    {
                        Label = category.Name,
                        Value = categoryValue,
                        Count = categoryItemCount,
                        Amount = categoryValue
                    });
                }

                return chartData.OrderByDescending(d => d.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category distribution");
                return new List<ChartDataDto>();
            }
        }

        public async Task<IEnumerable<ChartDataDto>> GetStoreWiseStockAsync()
        {
            try
            {
                var stores = await _unitOfWork.Stores.GetAllAsync();
                var chartData = new List<ChartDataDto>();

                foreach (var store in stores.Where(s => s.IsActive))
                {
                    var storeValue = 0m;
                    var storeItemCount = 0;

                    var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == store.Id && si.IsActive);

                    foreach (var storeItem in storeItems)
                    {
                        if (storeItem.Quantity > 0)
                        {
                            storeItemCount++;

                            // Get item for unit price fallback
                            var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);

                            // Calculate value based on last purchase price or item unit price
                            var lastPurchase = await _unitOfWork.PurchaseItems
                                .FirstOrDefaultAsync(pi => pi.ItemId == storeItem.ItemId);
                            var unitPrice = lastPurchase?.UnitPrice ?? item?.UnitPrice ?? 0;
                            storeValue += (decimal)(storeItem.Quantity * unitPrice);
                        }
                    }

                    chartData.Add(new ChartDataDto
                    {
                        Label = store.Name,
                        Value = storeValue,
                        Count = storeItemCount,
                        Amount = storeValue
                    });
                }

                return chartData.OrderByDescending(d => d.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store-wise stock");
                return new List<ChartDataDto>();
            }
        }

        public async Task RefreshCacheAsync()
        {
            try
            {
                // Clear all dashboard-related cache
                await _cacheService.RemoveAsync("dashboard_stats");
                await _cacheService.RemoveAsync("recent_activities");
                await _cacheService.RemoveAsync("alerts");
                await _cacheService.RemoveAsync("category_stock");

                // Pre-populate cache
                await GetDashboardStatsAsync();
                await GetRecentActivitiesAsync();
                await GetAlertsAsync();

                _logger.LogInformation("Dashboard cache refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing dashboard cache");
                throw;
            }
        }

        #region Private Helper Methods

        private async Task<int> GetLowStockCountAsync()
        {
            return await _unitOfWork.StoreItems
                .CountAsync(si => si.IsActive && si.Quantity <= si.MinimumStock && si.Quantity > 0);
        }

        private async Task<int> GetOutOfStockCountAsync()
        {
            return await _unitOfWork.StoreItems
                .CountAsync(si => si.IsActive && si.Quantity == 0);
        }

        private async Task<decimal> CalculateTotalInventoryValueAsync()
        {
            var storeItems = await _unitOfWork.StoreItems
                .Query()
                .Include(si => si.Item)
                .Where(si => si.IsActive)
                .ToListAsync();

            var totalValue = 0m;
            foreach (var storeItem in storeItems)
            {
                // Try to get last purchase price, fallback to item unit price
                var lastPurchase = await _unitOfWork.PurchaseItems
                    .FirstOrDefaultAsync(pi => pi.ItemId == storeItem.ItemId);
                var unitPrice = lastPurchase?.UnitPrice ?? storeItem.Item?.UnitPrice ?? 0;
                totalValue += (decimal)(storeItem.Quantity * unitPrice);
            }

            return totalValue;
        }

        private async Task<decimal> GetMonthlyPurchaseValueAsync()
        {
            var startDate = DateTime.Now.AddMonths(-1);
            var monthlyPurchases = await _unitOfWork.Purchases
                .Query()
                .Where(p => p.IsActive && p.CreatedAt >= startDate)
                .ToListAsync();

            return monthlyPurchases.Sum(p => p.TotalAmount);
        }

        private async Task<IEnumerable<CategoryStockDto>> GetCategoryStockAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var categoryStock = new List<CategoryStockDto>();

                foreach (var category in categories.Where(c => c.IsActive))
                {
                    var categoryItemCount = 0;
                    var categoryValue = 0m;

                    var subCategories = await _unitOfWork.SubCategories.FindAsync(sc => sc.CategoryId == category.Id && sc.IsActive);
                    foreach (var subCategory in subCategories)
                    {
                        var categoryItems = await _unitOfWork.Items.FindAsync(i => i.SubCategoryId == subCategory.Id && i.IsActive);
                        categoryItemCount += categoryItems.Count();

                        foreach (var item in categoryItems)
                        {
                            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);
                            var totalStock = storeItems.Sum(si => si.Quantity);

                            // Calculate value based on last purchase price or unit price
                            var lastPurchase = await _unitOfWork.PurchaseItems
                                .FirstOrDefaultAsync(pi => pi.ItemId == item.Id);
                            var unitPrice = lastPurchase?.UnitPrice ?? item.UnitPrice;
                            categoryValue += (decimal)(totalStock * unitPrice);
                        }
                    }

                    categoryStock.Add(new CategoryStockDto
                    {
                        CategoryName = category.Name,
                        ItemCount = categoryItemCount,
                        TotalValue = categoryValue
                    });
                }

                return categoryStock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category stock");
                return new List<CategoryStockDto>();
            }
        }

      #endregion
    }
}