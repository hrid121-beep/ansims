using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class StockAlertService : IStockAlertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StockAlertService> _logger;
        private readonly IConfiguration _configuration;

        public StockAlertService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IEmailService emailService,
            INotificationService notificationService,
            ILogger<StockAlertService> logger,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _notificationService = notificationService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<PersonalizedDashboardDto> GetPersonalizedAlertsAsync(string userId)
        {
            var dashboard = new PersonalizedDashboardDto();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return dashboard;

            // Get user's assigned stores
            var userStores = await GetUserStoresAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user);

            // Get alerts based on user's role and stores
            var allAlerts = new List<StockAlertDto>();

            if (userRoles.Contains("Admin") || userRoles.Contains("LogisticsOfficer"))
            {
                // Admin and Logistics see all alerts
                allAlerts = await CheckAllStoresForAlertsAsync();
            }
            else
            {
                // Other users see only their store alerts
                foreach (var storeId in userStores)
                {
                    var storeAlerts = await GetStoreAlertsAsync(storeId);
                    allAlerts.AddRange(storeAlerts);
                }
            }

            // Categorize alerts by level
            dashboard.CriticalAlerts = allAlerts.Where(a => a.Level == StockAlertLevel.Critical).ToList();
            dashboard.WarningAlerts = allAlerts.Where(a => a.Level == StockAlertLevel.High || a.Level == StockAlertLevel.Medium).ToList();
            dashboard.InfoAlerts = allAlerts.Where(a => a.Level == StockAlertLevel.Low || a.Level == StockAlertLevel.Info).ToList();
            dashboard.TotalAlerts = allAlerts.Count;
            dashboard.LastChecked = DateTime.Now;

            // Get store statistics
            foreach (var storeId in userStores)
            {
                if (storeId.HasValue)
                {
                    var stats = await GetStoreStatisticsAsync(storeId);
                    dashboard.StoreStatistics[storeId.Value] = stats;
                }
            }

            // Get pending actions
            dashboard.PendingActions = await GetPendingActionsAsync(userId, userRoles);

            return dashboard;
        }

        public async Task<List<StockAlertDto>> GetStoreAlertsAsync(int? storeId)
        {
            var alerts = new List<StockAlertDto>();

            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId && si.IsActive);
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);

            foreach (var storeItem in storeItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
                if (item == null) continue;

                var alertLevel = CalculateAlertLevel(storeItem.Quantity, item.MinimumStock);
                if (alertLevel != null)
                {
                    alerts.Add(new StockAlertDto
                    {
                        ItemId = item.Id,
                        ItemCode = item.ItemCode,
                        ItemName = item.Name,
                        StoreId = storeId,
                        StoreName = store?.Name,
                        CurrentStock = storeItem.Quantity,
                        MinimumStock = item.MinimumStock,
                        PercentageRemaining = item.MinimumStock > 0 ? (storeItem.Quantity / item.MinimumStock) * 100 : 0,
                        Level = alertLevel.Value,
                        LevelDisplay = GetAlertLevelDisplay(alertLevel.Value),
                        LevelColor = GetAlertLevelColor(alertLevel.Value),
                        DetectedAt = DateTime.Now,
                        IsAcknowledged = false
                    });
                }
            }

            return alerts.OrderBy(a => a.Level).ThenBy(a => a.PercentageRemaining).ToList();
        }

        public async Task<List<StockAlertDto>> CheckAllStoresForAlertsAsync()
        {
            var allAlerts = new List<StockAlertDto>();
            var stores = await _unitOfWork.Stores.GetAllAsync();

            foreach (var store in stores.Where(s => s.IsActive))
            {
                var storeAlerts = await GetStoreAlertsAsync(store.Id);
                allAlerts.AddRange(storeAlerts);
            }

            return allAlerts;
        }

        public async Task SendLowStockEmailsAsync()
        {
            var alerts = await CheckAllStoresForAlertsAsync();
            var criticalAlerts = alerts.Where(a => a.Level == StockAlertLevel.Critical).ToList();
            var highAlerts = alerts.Where(a => a.Level == StockAlertLevel.High).ToList();

            if (!criticalAlerts.Any() && !highAlerts.Any())
            {
                _logger.LogInformation("No critical or high priority stock alerts to send");
                return;
            }

            // Get email recipients based on configuration
            var recipients = await GetEmailRecipientsAsync();

            // Send critical alerts to all recipients
            if (criticalAlerts.Any())
            {
                var criticalEmailBody = BuildCriticalAlertEmail(criticalAlerts);
                foreach (var recipient in recipients["Critical"])
                {
                    await _emailService.SendEmailAsync(
                        recipient,
                        "🚨 Critical Stock Alert - Immediate Action Required",
                        criticalEmailBody,
                        true
                    );
                }
            }

            // Send high priority alerts to managers
            if (highAlerts.Any())
            {
                var highEmailBody = BuildHighAlertEmail(highAlerts);
                foreach (var recipient in recipients["High"])
                {
                    await _emailService.SendEmailAsync(
                        recipient,
                        "⚠️ High Priority Stock Alert",
                        highEmailBody,
                        true
                    );
                }
            }

            // Log email sending
            _logger.LogInformation($"Sent {criticalAlerts.Count} critical and {highAlerts.Count} high priority alerts");
        }

        public async Task AcknowledgeAlertAsync(int itemId, int? storeId, string userId)
        {
            // In a real implementation, you would store acknowledgments in database
            // For now, we'll just log it
            var user = await _userManager.FindByIdAsync(userId);
            _logger.LogInformation($"Alert acknowledged for Item {itemId} in Store {storeId} by {user?.UserName}");
        }

        public async Task<StockAlertConfigDto> GetAlertConfigurationAsync(int itemId, int? storeId)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(itemId);
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                si => si.ItemId == itemId && si.StoreId == storeId
            );

            return new StockAlertConfigDto
            {
                ItemId = itemId,
                ItemName = item?.Name,
                StoreId = storeId,
                StoreName = store?.Name,
                CurrentStock = storeItem?.Quantity ?? 0,
                MinimumStock = item?.MinimumStock ?? 0,
                AlertLevel = StockAlertLevel.Info.ToString(),
                SendEmail = true,
                ShowInDashboard = true,
                EmailRecipients = new List<string>()
            };
        }

        public async Task UpdateAlertConfigurationAsync(StockAlertConfigDto config)
        {
            // In a real implementation, save configuration to database
            _logger.LogInformation($"Alert configuration updated for Item {config.ItemId} in Store {config.StoreId}");
            await Task.CompletedTask;
        }

        public async Task<Dictionary<StockAlertLevel, int>> GetAlertSummaryAsync(string userId)
        {
            var dashboard = await GetPersonalizedAlertsAsync(userId);

            return new Dictionary<StockAlertLevel, int>
            {
                { StockAlertLevel.Critical, dashboard.CriticalAlerts.Count },
                { StockAlertLevel.High, dashboard.WarningAlerts.Count(a => a.Level == StockAlertLevel.High) },
                { StockAlertLevel.Medium, dashboard.WarningAlerts.Count(a => a.Level == StockAlertLevel.Medium) },
                { StockAlertLevel.Low, dashboard.InfoAlerts.Count(a => a.Level == StockAlertLevel.Low) },
                { StockAlertLevel.Info, dashboard.InfoAlerts.Count(a => a.Level == StockAlertLevel.Info) }
            };
        }

        public async Task CheckAndSendLowStockAlertsAsync()
        {
            try
            {
                var lowStockItems = await GetAllLowStockItemsAsync();

                // Group by severity
                var criticalItems = lowStockItems.Where(i => i.CurrentStock == 0).ToList();
                var lowItems = lowStockItems.Where(i => i.CurrentStock > 0).ToList();

                if (criticalItems.Any())
                {
                    await SendCriticalStockAlertsAsync(criticalItems);
                }

                if (lowItems.Any())
                {
                    await SendLowStockAlertsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking low stock alerts");
            }
        }

        private async Task<List<StockAlertDto>> GetAllLowStockItemsAsync()
        {
            var alerts = new List<StockAlertDto>();
            var items = await _unitOfWork.Items.GetAllAsync();

            foreach (var item in items.Where(i => i.IsActive && i.MinimumStock > 0))
            {
                var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);

                foreach (var storeItem in storeItems)
                {
                    if (storeItem.Quantity <= item.MinimumStock)
                    {
                        var store = await _unitOfWork.Stores.GetByIdAsync(storeItem.StoreId);

                        alerts.Add(new StockAlertDto
                        {
                            ItemId = item.Id,
                            ItemCode = item.ItemCode,
                            ItemName = item.Name,
                            StoreId = store.Id,
                            StoreName = store.Name,
                            CurrentStock = storeItem.Quantity,
                            MinimumStock = item.MinimumStock,
                            AlertLevel = storeItem.Quantity == 0 ? "Critical" : "Low"
                        });
                    }
                }
            }

            return alerts;
        }

        private async Task SendCriticalStockAlertsAsync(List<StockAlertDto> criticalItems)
        {
            // Get logistics officers and store managers
            var logisticsOfficers = await _userManager.GetUsersInRoleAsync("LogisticsOfficer");
            var storeManagers = await _userManager.GetUsersInRoleAsync("StoreManager");

            var recipients = logisticsOfficers.Union(storeManagers)
                .Where(u => u.IsActive && !string.IsNullOrEmpty(u.Email))
                .Select(u => u.Email)
                .Distinct()
                .ToList();

            if (!recipients.Any()) return;

            var emailBody = BuildCriticalAlertEmail(criticalItems);

            foreach (var email in recipients)
            {
                await _emailService.SendEmailAsync(
                    email,
                    "🚨 Critical Stock Alert - Immediate Action Required",
                    emailBody,
                    true
                );
            }
        }

        public async Task<IEnumerable<StockAlertDto>> GetLowStockItemsAsync(string storeCode = null)
        {
            var alerts = new List<StockAlertDto>();

            var items = await _unitOfWork.Items.FindAsync(i => i.IsActive);

            foreach (var item in items)
            {
                var storeItemsQuery = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);

                if (!string.IsNullOrWhiteSpace(storeCode))
                {
                    var store = await _unitOfWork.Stores.FirstOrDefaultAsync(s => s.Code == storeCode && s.IsActive);
                    if (store != null)
                    {
                        storeItemsQuery = storeItemsQuery.Where(si => si.StoreId == store.Id);
                    }
                }

                foreach (var storeItem in storeItemsQuery)
                {
                    if (storeItem.Quantity <= item.MinimumStock ||
                        (item.ReorderLevel > 0 && storeItem.Quantity <= item.ReorderLevel))
                    {
                        var store = await _unitOfWork.Stores.GetByIdAsync(storeItem.StoreId);
                        var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                        var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;

                        alerts.Add(new StockAlertDto
                        {
                            ItemId = item.Id,
                            ItemCode = item.ItemCode,
                            ItemName = item.Name,
                            CategoryName = category?.Name,
                            SubCategoryName = subCategory?.Name,
                            StoreId = storeItem.StoreId,
                            StoreName = store?.Name,
                            CurrentStock = storeItem.Quantity,
                            MinimumStock = item.MinimumStock,
                            ReorderLevel = item.ReorderLevel,
                            AlertType = storeItem.Quantity <= item.MinimumStock ? "Critical" : "Low",
                            AlertDate = DateTime.Now
                        });
                    }
                }
            }

            return alerts.OrderBy(a => a.CurrentStock).ThenBy(a => a.ItemName);
        }

        public async Task SendLowStockAlertsAsync()
        {
            try
            {
                var alerts = await GetLowStockItemsAsync();

                foreach (var alert in alerts.Where(a => a.AlertType == "Critical"))
                {
                    var notification = new NotificationDto
                    {
                        Title = $"Critical Stock Alert: {alert.ItemName}",
                        Message = $"Item {alert.ItemCode} in {alert.StoreName} has critically low stock. Current: {alert.CurrentStock}, Minimum: {alert.MinimumStock}",
                        Type = NotificationType.StockAlert.ToString(),
                        Priority = NotificationPriority.High.ToString(),
                        TargetRole = "StoreManager"
                    };

                    await _notificationService.CreateNotificationAsync(notification);
                }

                _logger.LogInformation("Sent {Count} low stock alerts", alerts.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending low stock alerts");
                throw;
            }
        }

        #region Private Helper Methods

        private StockAlertLevel? CalculateAlertLevel(decimal? currentStock, decimal? minimumStock)
        {
            if (minimumStock <= 0) return null;

            if (currentStock == 0)
                return StockAlertLevel.Critical;

            var percentage = (currentStock / minimumStock) * 100;

            if (percentage < 25)
                return StockAlertLevel.High;
            else if (percentage < 50)
                return StockAlertLevel.Medium;
            else if (percentage < 100)
                return StockAlertLevel.Low;
            else if (percentage < 110)
                return StockAlertLevel.Info;

            return null; // No alert needed
        }

        private string GetAlertLevelDisplay(StockAlertLevel level)
        {
            return level switch
            {
                StockAlertLevel.Critical => "Critical - Out of Stock",
                StockAlertLevel.High => "High - Urgent Reorder",
                StockAlertLevel.Medium => "Medium - Reorder Soon",
                StockAlertLevel.Low => "Low - Below Minimum",
                StockAlertLevel.Info => "Info - Approaching Minimum",
                _ => "Unknown"
            };
        }

        private string GetAlertLevelColor(StockAlertLevel level)
        {
            return level switch
            {
                StockAlertLevel.Critical => "danger",
                StockAlertLevel.High => "warning",
                StockAlertLevel.Medium => "orange",
                StockAlertLevel.Low => "info",
                StockAlertLevel.Info => "secondary",
                _ => "light"
            };
        }

        private async Task<List<int?>> GetUserStoresAsync(string userId)
        {
            var userStores = await _unitOfWork.UserStores.FindAsync(us => us.UserId == userId);
            return userStores.Select(us => us.StoreId).ToList();
        }

        private async Task<StoreStatisticsDto> GetStoreStatisticsAsync(int? storeId)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId && si.IsActive);

            var stats = new StoreStatisticsDto
            {
                StoreId = storeId,
                StoreName = store?.Name,
                TotalItems = storeItems.Count()
            };

            foreach (var storeItem in storeItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
                if (item == null) continue;

                var alertLevel = CalculateAlertLevel(storeItem.Quantity, item.MinimumStock);

                if (alertLevel == StockAlertLevel.Critical)
                    stats.CriticalItems++;
                else if (alertLevel.HasValue && alertLevel.Value <= StockAlertLevel.Low)
                    stats.LowStockItems++;
                else
                    stats.HealthyItems++;
            }

            stats.StockHealthPercentage = stats.TotalItems > 0
                ? (decimal)stats.HealthyItems / stats.TotalItems * 100
                : 0;

            return stats;
        }

        private async Task<List<PendingActionDto>> GetPendingActionsAsync(string userId, IList<string> userRoles)
        {
            var actions = new List<PendingActionDto>();

            // Check for pending write-offs
            if (userRoles.Any(r => r == "Admin" || r == "Commander" || r == "StoreManager"))
            {
                var pendingWriteOffs = await _unitOfWork.WriteOffs.FindAsync(
                    w => string.IsNullOrEmpty(w.ApprovedBy) && w.IsActive
                );

                foreach (var writeOff in pendingWriteOffs)
                {
                    actions.Add(new PendingActionDto
                    {
                        ActionType = "WriteOffApproval",
                        Description = $"Write-off #{writeOff.WriteOffNo} pending approval",
                        EntityType = "WriteOff",
                        EntityId = writeOff.Id,
                        CreatedAt = writeOff.CreatedAt,
                        Priority = "High"
                    });
                }
            }

            return actions.OrderBy(a => a.Priority).ThenBy(a => a.CreatedAt).ToList();
        }

        private async Task<Dictionary<string, List<string>>> GetEmailRecipientsAsync()
        {
            var recipients = new Dictionary<string, List<string>>
            {
                { "Critical", new List<string>() },
                { "High", new List<string>() }
            };

            // Get Store Managers
            var storeManagers = await _userManager.GetUsersInRoleAsync("StoreManager");
            var storeManagerEmails = storeManagers.Where(u => !string.IsNullOrEmpty(u.Email)).Select(u => u.Email).ToList();

            // Get Logistics Officers
            var logisticsOfficers = await _userManager.GetUsersInRoleAsync("LogisticsOfficer");
            var logisticsEmails = logisticsOfficers.Where(u => !string.IsNullOrEmpty(u.Email)).Select(u => u.Email).ToList();

            // Critical alerts go to both Store Managers and Logistics Officers
            recipients["Critical"].AddRange(storeManagerEmails);
            recipients["Critical"].AddRange(logisticsEmails);

            // High priority alerts go to Store Managers only
            recipients["High"].AddRange(storeManagerEmails);

            return recipients;
        }

        private string BuildCriticalAlertEmail(List<StockAlertDto> alerts)
        {
            var html = @"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #dc3545;'>🚨 Critical Stock Alert</h2>
                    <p>The following items are completely out of stock and require immediate attention:</p>
                    <table style='border-collapse: collapse; width: 100%;'>
                        <thead>
                            <tr style='background-color: #f8f9fa;'>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Item Code</th>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Item Name</th>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Store</th>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Minimum Required</th>
                            </tr>
                        </thead>
                        <tbody>";

            foreach (var alert in alerts)
            {
                html += $@"
                    <tr>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.ItemCode}</td>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.ItemName}</td>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.StoreName}</td>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.MinimumStock}</td>
                    </tr>";
            }

            html += @"
                        </tbody>
                    </table>
                    <p style='margin-top: 20px;'><strong>Action Required:</strong> Please initiate purchase orders or stock transfers immediately.</p>
                    <p style='color: #6c757d; font-size: 12px;'>This is an automated alert from the Ansar & VDP Inventory Management System</p>
                </body>
                </html>";

            return html;
        }

        private string BuildHighAlertEmail(List<StockAlertDto> alerts)
        {
            var html = @"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #ffc107;'>⚠️ High Priority Stock Alert</h2>
                    <p>The following items are running critically low and need urgent reordering:</p>
                    <table style='border-collapse: collapse; width: 100%;'>
                        <thead>
                            <tr style='background-color: #f8f9fa;'>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Item Code</th>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Item Name</th>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Store</th>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Current Stock</th>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>Minimum Required</th>
                                <th style='border: 1px solid #dee2e6; padding: 8px;'>% Remaining</th>
                            </tr>
                        </thead>
                        <tbody>";

            foreach (var alert in alerts)
            {
                html += $@"
                    <tr>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.ItemCode}</td>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.ItemName}</td>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.StoreName}</td>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.CurrentStock}</td>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.MinimumStock}</td>
                        <td style='border: 1px solid #dee2e6; padding: 8px;'>{alert.PercentageRemaining:F0}%</td>
                    </tr>";
            }

            html += @"
                        </tbody>
                    </table>
                    <p style='margin-top: 20px;'><strong>Recommendation:</strong> Plan reordering within the next 2-3 days to avoid stockouts.</p>
                    <p style='color: #6c757d; font-size: 12px;'>This is an automated alert from the Ansar & VDP Inventory Management System</p>
                </body>
                </html>";

            return html;
        }

        #endregion
        public async Task<List<StockAlertDto>> GetLowStockItems()
        {
            var items = await _unitOfWork.Items.GetAllAsync(i => i.IsActive);
            var lowStockAlerts = new List<StockAlertDto>();

            foreach (var item in items)
            {
                var storeItems = await _unitOfWork.StoreItems
                    .GetAllAsync(si => si.ItemId == item.Id && si.IsActive);

                var totalStock = storeItems.Sum(si => si.Quantity);

                if (totalStock <= item.MinimumStock)
                {
                    lowStockAlerts.Add(new StockAlertDto
                    {
                        ItemId = item.Id,
                        ItemName = item.Name,
                        ItemCode = item.Code ?? item.ItemCode,
                        CurrentStock = totalStock,
                        MinimumStock = item.MinimumStock,
                        Unit = item.Unit,
                        AlertType = totalStock == 0 ? "OutOfStock" : "LowStock",
                        Severity = totalStock == 0 ? "Critical" : "Warning",
                        CreatedAt = DateTime.Now
                    });
                }
            }

            return lowStockAlerts;
        }

        public async Task<PersonalizedAlertDashboard> GetPersonalizedAlerts(string userName)
        {
            return new PersonalizedAlertDashboard
            {
                UserName = userName,
                LowStockAlerts = await GetLowStockItems(),
                ExpiryAlerts = new List<ExpiryTrackingDto>(),
                ReorderAlerts = new List<StockAlertDto>()
            };
        }

        public async Task<PersonalizedAlertDashboard> GetStockAlertDashboard()
        {
            return await GetPersonalizedAlerts("Dashboard");
        }

    }
}