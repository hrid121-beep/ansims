using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<BatchTrackingService> _logger;
        private readonly IConfiguration _configuration; // ADD THIS

        public NotificationService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<BatchTrackingService> logger,
            UserManager<User> userManager,
            IConfiguration configuration) // ADD THIS PARAMETER
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration; // ADD THIS ASSIGNMENT
        }

        public async Task<NotificationDto> CreateNotificationAsync(NotificationDto dto)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = dto.UserId,
                    Title = dto.Title,
                    Message = dto.Message,
                    Type = dto.Type,
                    Priority = dto.Priority ?? "Normal",
                    Category = dto.Category,
                    IsRead = false,
                    IsSent = false,
                    CreatedAt = DateTime.Now,

                    // Reference information
                    ReferenceType = dto.ReferenceType,
                    ReferenceId = dto.ReferenceId,
                    ActionUrl = dto.ActionUrl,

                    // Metadata
                    Metadata = dto.Metadata
                };

                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.CompleteAsync();

                // Send notification based on priority and user preferences
                await SendNotificationAsync(notification);

                dto.Id = notification.Id;
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                throw;
            }
        }

        // Send Notification
        private async Task SendNotificationAsync(Notification notification)
        {
            try
            {
                // Get user preferences
                var userPreferences = await GetUserNotificationPreferencesAsync(notification.UserId);

                // Send based on notification type and user preferences
                var tasks = new List<Task>();

                if (userPreferences.PushEnabled)
                {
                    tasks.Add(SendPushNotificationAsync(notification));
                }

                await Task.WhenAll(tasks);

                notification.IsSent = true;
                notification.SentAt = DateTime.Now;
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification {notification.Id}");
            }
        }

        // Create Low Stock Alert
        public async Task CreateLowStockAlertAsync(int storeId, int itemId)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems.Query()
                    .Include(si => si.Store)
                    .Include(si => si.Item)
                    .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ItemId == itemId);

                if (storeItem == null) return;

                // Check if alert already exists for today
                var existingAlert = await _unitOfWork.Notifications
                    .FirstOrDefaultAsync(n =>
                        n.ReferenceType == "LowStock" &&
                        n.ReferenceId == $"{storeId}-{itemId}" &&
                        n.CreatedAt.Date == DateTime.Today);

                if (existingAlert != null) return;

                // Get store managers
                var storeManagers = await GetStoreManagersAsync(storeId);

                foreach (var manager in storeManagers)
                {
                    await CreateNotificationAsync(new NotificationDto
                    {
                        UserId = manager.Id,
                        Title = "Low Stock Alert",
                        Message = $"Item {storeItem.Item.Name} ({storeItem.Item.ItemCode}) " +
                                 $"in {storeItem.Store.Name} has low stock. " +
                                 $"Current: {storeItem.CurrentStock}, Minimum: {storeItem.MinimumStock}",
                        Type = "Alert",
                        Priority = storeItem.CurrentStock == 0 ? "Critical" : "High",
                        Category = "Stock",
                        ReferenceType = "LowStock",
                        ReferenceId = $"{storeId}-{itemId}",
                        ActionUrl = $"/StockEntry/LowStock?storeId={storeId}&itemId={itemId}"
                    });
                }

                // Create stock alert record
                var stockAlert = new StockAlert
                {
                    StoreId = storeId,
                    ItemId = itemId,
                    AlertType = AlertType.LowStock.ToString(),
                    CurrentQuantity = storeItem.CurrentStock,
                    ThresholdQuantity = storeItem.MinimumStock,
                    AlertDate = DateTime.Now,
                    IsResolved = false
                };

                await _unitOfWork.StockAlerts.AddAsync(stockAlert);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating low stock alert for store {storeId}, item {itemId}");
            }
        }

        // Create Expiry Alert
        public async Task CreateExpiryAlertAsync(BatchTracking batch)
        {
            try
            {
                if (!batch.ExpiryDate.HasValue) return;

                var daysToExpiry = (batch.ExpiryDate.Value - DateTime.Now).Days;
                string priority = daysToExpiry <= 7 ? "Critical" : daysToExpiry <= 15 ? "High" : "Medium";

                // Get relevant users
                var users = await GetStoreUsersAsync(batch.StoreId);

                foreach (var user in users)
                {
                    await CreateNotificationAsync(new NotificationDto
                    {
                        UserId = user.Id,
                        Title = $"Expiry Alert - {daysToExpiry} days",
                        Message = $"Batch {batch.BatchNumber} of {batch.Item?.Name} " +
                                 $"expires on {batch.ExpiryDate.Value:dd/MM/yyyy}. " +
                                 $"Quantity: {batch.RemainingQuantity}",
                        Type = "Alert",
                        Priority = priority,
                        Category = "Expiry",
                        ReferenceType = "ExpiryAlert",
                        ReferenceId = batch.Id.ToString(),
                        ActionUrl = $"/BatchTracking/Expiring?batchId={batch.Id}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating expiry alert for batch {batch.Id}");
            }
        }
        public async Task CreateApprovalNotificationAsync(ApprovalRequest approval)
        {
            try
            {
                // Convert int ApprovalLevel to string
                var approvers = await GetApproversAsync($"Level{approval.ApprovalLevel}");

                foreach (var approver in approvers)
                {
                    await CreateNotificationAsync(new NotificationDto
                    {
                        UserId = approver.Id,
                        Title = "Approval Required",
                        Message = $"{approval.EntityType} {approval.Description} requires your approval. " +
                                 $"Amount: {(approval.Amount ?? 0):N2}",
                        Type = "Approval",
                        Priority = approval.Priority,
                        Category = "Workflow",
                        ReferenceType = approval.EntityType,
                        ReferenceId = approval.EntityId.ToString(),
                        ActionUrl = $"/Approval/Pending?id={approval.Id}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating approval notification for {approval.Id}");
            }
        }

        // Send Push Notification
        private async Task SendPushNotificationAsync(Notification notification)
        {
            try
            {
                // Implementation for push notifications (Firebase, OneSignal, etc.)
                // This would integrate with a push notification service

                notification.PushSent = true;
                notification.PushSentAt = DateTime.Now;

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending push notification {notification.Id}");
            }
        }

        // Get User Notifications
        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(
            string userId, bool unreadOnly = false)
        {
            try
            {
                var query = _unitOfWork.Notifications.Query()
                    .Where(n => n.UserId == userId);

                if (unreadOnly)
                    query = query.Where(n => !n.IsRead);

                var notifications = await query
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(50)
                    .Select(n => new NotificationDto
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Message = n.Message,
                        Type = n.Type,
                        Priority = n.Priority,
                        Category = n.Category,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        ActionUrl = n.ActionUrl
                    })
                    .ToListAsync();

                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting notifications for user {userId}");
                throw;
            }
        }

        // Mark as Read
        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var notification = await _unitOfWork.Notifications
                    .FirstOrDefaultAsync(n => n.Id == notificationId);

                if (notification == null) return false;

                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notification {notificationId} as read");
                throw;
            }
        }

        // Send Batch Notifications
        public async Task SendBatchNotificationsAsync(BatchNotificationDto dto)
        {
            try
            {
                var tasks = new List<Task>();

                foreach (var userId in dto.UserIds)
                {
                    tasks.Add(CreateNotificationAsync(new NotificationDto
                    {
                        UserId = userId,
                        Title = dto.Title,
                        Message = dto.Message,
                        Type = dto.Type,
                        Priority = dto.Priority,
                        Category = dto.Category
                    }));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending batch notifications");
                throw;
            }
        }

        // Create Daily Summary
        public async Task CreateDailySummaryAsync()
        {
            try
            {
                var stores = await _unitOfWork.Stores.GetAllAsync();

                foreach (var store in stores)
                {
                    var summary = await GenerateDailySummaryAsync(store.Id);
                    var managers = await GetStoreManagersAsync(store.Id);

                    foreach (var manager in managers)
                    {
                        await CreateNotificationAsync(new NotificationDto
                        {
                            UserId = manager.Id,
                            Title = $"Daily Summary - {store.Name}",
                            Message = summary,
                            Type = "Summary",
                            Priority = "Low",
                            Category = "Report"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating daily summary");
            }
        }

        // Helper Methods
        private async Task<UserNotificationPreferences> GetUserNotificationPreferencesAsync(string userId)
        {
            var preferences = await _unitOfWork.UserNotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return preferences ?? new UserNotificationPreferences
            {
                EmailEnabled = true,
                SmsEnabled = false,
                PushEnabled = true
            };
        }
        private async Task<List<User>> GetStoreManagersAsync(int storeId)
        {
            return await _unitOfWork.Users.Query().Where(u => u.UserStores.Any(su => su.StoreId == storeId && su.Role == "StoreManager"))
                .ToListAsync();
        }
        private async Task<List<User>> GetStoreUsersAsync(int storeId)
        {
            return await _unitOfWork.Users.Query()
                .Where(u => u.UserStores.Any(su => su.StoreId == storeId && su.IsActive))
                .ToListAsync();
        }
        private async Task<List<User>> GetApproversAsync(string approvalLevel)
        {
            var roleName = approvalLevel switch
            {
                "Level1" => "StoreManager",
                "Level2" => "DepartmentHead",
                "Level3" => "FinanceManager",
                "Level4" => "Director",
                _ => "StoreManager"
            };

            // Use UserManager to get users in role
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return usersInRole.ToList();
        }
        private string GenerateEmailBody(Notification notification, User user)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<h2>Hello {user.FullName},</h2>");
            sb.AppendLine($"<h3>{notification.Title}</h3>");
            sb.AppendLine($"<p>{notification.Message}</p>");

            if (!string.IsNullOrEmpty(notification.ActionUrl))
            {
                var baseUrl = _configuration["AppSettings:BaseUrl"];
                sb.AppendLine($"<p><a href='{baseUrl}{notification.ActionUrl}'>Click here to take action</a></p>");
            }

            sb.AppendLine("<hr>");
            sb.AppendLine("<p><small>This is an automated message from Ansar & VDP Inventory Management System</small></p>");

            return sb.ToString();
        }
        private async Task<string> GenerateDailySummaryAsync(int storeId)
        {
            var today = DateTime.Today;

            // Get today's statistics
            var todayIssues = await _unitOfWork.Issues
                .CountAsync(i => i.FromStoreId == storeId && i.IssueDate.Date == today);

            var todayReceives = await _unitOfWork.Receives
                .CountAsync(r => r.StoreId == storeId && r.ReceiveDate.Date == today);

            var lowStockItems = await _unitOfWork.StoreItems
                .CountAsync(si => si.StoreId == storeId && si.CurrentStock <= si.MinimumStock);

            var expiringBatches = await _unitOfWork.BatchTrackings
                .CountAsync(bt => bt.StoreId == storeId &&
                                 bt.ExpiryDate != null &&
                                 bt.ExpiryDate <= DateTime.Now.AddDays(30));

            return $"Today's Activity:\n" +
                   $"- Issues: {todayIssues}\n" +
                   $"- Receives: {todayReceives}\n" +
                   $"- Low Stock Items: {lowStockItems}\n" +
                   $"- Items Expiring Soon: {expiringBatches}";
        }
    
        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            var notifications = await _unitOfWork.Notifications.GetAllAsync();
            return notifications.Select(MapToDto).OrderByDescending(n => n.CreatedAt);
        }
        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(n => n.UserId == userId && n.IsActive);
            return notifications.Select(MapToDto).OrderByDescending(n => n.CreatedAt);
        }
        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(string userId)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(
                n => n.UserId == userId && !n.IsRead && n.IsActive
            );
            return notifications.Select(MapToDto).OrderByDescending(n => n.CreatedAt);
        }
        public async Task<NotificationDto> GetNotificationByIdAsync(int id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            return notification != null ? MapToDto(notification) : null;
        }
        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(
                n => n.UserId == userId && !n.IsRead && n.IsActive
            );

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
            }

            _unitOfWork.Notifications.UpdateRange(notifications);
            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.IsActive = false;
                _unitOfWork.Notifications.Update(notification);
                await _unitOfWork.CompleteAsync();
            }
        }
        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _unitOfWork.Notifications.CountAsync(
                n => n.UserId == userId && !n.IsRead && n.IsActive
            );
        }
        public async Task SendBroadcastNotificationAsync(string title, string message, string type = "info")
        {
            // Get all active users using UserManager
            var users = await _userManager.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            var notifications = users.Select(user => new Notification
            {
                UserId = user.Id,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.Now,
                CreatedBy = GetCurrentUserId()
            });

            await _unitOfWork.Notifications.AddRangeAsync(notifications);
            await _unitOfWork.CompleteAsync();
        }
        private string GetCurrentUserId()
        {
            // CRITICAL FIX: Use NameIdentifier (User.Id) not Name (Username)
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
        }
        private string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.TotalDays > 1)
                return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalHours > 1)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalMinutes > 1)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            return "Just now";
        }
        private string GetIconForType(string type)
        {
            return type?.ToLower() switch
            {
                "info" => "info-circle",
                "warning" => "exclamation-triangle",
                "error" => "times-circle",
                "success" => "check-circle",
                _ => "bell"
            };
        }
        private string GetCssClassForType(string type)
        {
            return type?.ToLower() switch
            {
                "info" => "text-info",
                "warning" => "text-warning",
                "error" => "text-danger",
                "success" => "text-success",
                _ => "text-primary"
            };
        }
        public async Task<IEnumerable<NotificationDto>> GetRecentNotificationsAsync(string userId, int count = 5)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(
                n => n.UserId == userId && n.IsActive
            );

            return notifications
                .OrderByDescending(n => !n.IsRead) // Unread first
                .ThenByDescending(n => n.CreatedAt)
                .Take(count)
                .Select(MapToDto);
        }
        public async Task DeleteAllReadNotificationsAsync(string userId)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(
                n => n.UserId == userId && n.IsRead && n.IsActive
            );

            foreach (var notification in notifications)
            {
                notification.IsActive = false;
                notification.UpdatedAt = DateTime.Now;
                notification.UpdatedBy = GetCurrentUserId();
            }

            _unitOfWork.Notifications.UpdateRange(notifications);
            await _unitOfWork.CompleteAsync();
        }
        public async Task SendToRoleAsync(string roleName, string title, string message, string type = "info")
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);

            var notifications = users.Where(u => u.IsActive).Select(user => new Notification
            {
                UserId = user.Id,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.Now,
                CreatedBy = GetCurrentUserId(),
                IsActive = true
            });

            await _unitOfWork.Notifications.AddRangeAsync(notifications);
            await _unitOfWork.CompleteAsync();
        }
        public async Task SendNotificationAsync(NotificationDto notification)
        {
            await CreateNotificationAsync(notification);
        }

        // PRIMARY METHOD - Used by most services
        public async Task SendNotificationAsync(string userId, string title, string message, string type = "info")
        {
            await CreateNotificationAsync(new NotificationDto
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                Priority = "Normal",
                IsRead = false
            });
        }
        private NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                Url = notification.Url,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                TimeAgo = CalculateTimeAgo(notification.CreatedAt),
                Icon = GetIconForType(notification.Type),
                CssClass = GetCssClassForType(notification.Type),
                Priority = notification.Priority ?? "Normal",
                RelatedEntity = notification.RelatedEntity,
                RelatedEntityId = notification.RelatedEntityId,
                TargetRole = notification.TargetRole
            };
        }
    }
}