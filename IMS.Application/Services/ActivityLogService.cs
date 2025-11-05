using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActivityLogService> _logger;

        public ActivityLogService(IUnitOfWork unitOfWork, ILogger<ActivityLogService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Main logging method with all parameters
        public async Task LogAsync(string entityName, string action, string description,
            int? entityId, object oldValues, object newValues, string userId, string ipAddress)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    UserId = userId,
                    Action = action,
                    Entity = entityName,
                    EntityType = entityName,
                    EntityId = entityId,
                    Details = description,
                    OldValues = oldValues?.ToString(),
                    NewValues = newValues?.ToString(),
                    ActionDate = DateTime.Now,
                    Timestamp = DateTime.Now,
                    IPAddress = ipAddress,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId ?? "System"
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging activity: {Action} on {Entity}", action, entityName);
            }
        }

        // Simple 4-parameter overload (called by RangeService)
        public async Task LogActivityAsync(string entityName, int? entityId, string action,
            string description, string userId = null)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    UserId = userId ?? "System",
                    Action = action,
                    Entity = entityName,
                    EntityType = entityName,
                    EntityId = entityId,
                    Details = description,
                    ActionDate = DateTime.Now,
                    Timestamp = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId ?? "System"
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging activity: {Action} on {Entity}", action, entityName);
            }
        }

        // Overload with oldValues and newValues
        public async Task LogActivityAsync(string userId, string action, string entityType,
            int? entityId, object oldValues, object newValues, string ipAddress = null)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    UserId = userId,
                    Action = action,
                    Entity = entityType,
                    EntityType = entityType,
                    EntityId = entityId,
                    OldValues = oldValues?.ToString(),
                    NewValues = newValues?.ToString(),
                    ActionDate = DateTime.Now,
                    Timestamp = DateTime.Now,
                    IPAddress = ipAddress,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId ?? "System"
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging activity: {Action} on {Entity}", action, entityType);
            }
        }

        // Overload with string oldValue and newValue
        public async Task LogActivityAsync(string userId, string action, string entityType,
            int entityId, string oldValue, string newValue, string additionalInfo)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    UserId = userId,
                    Action = action,
                    Entity = entityType,
                    EntityType = entityType,
                    EntityId = entityId,
                    OldValues = oldValue,
                    NewValues = newValue,
                    Details = additionalInfo,
                    ActionDate = DateTime.Now,
                    Timestamp = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId ?? "System"
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging activity: {Action} on {Entity}", action, entityType);
            }
        }

        // Store user activity
        public async Task LogStorUsereActivityAsync(string entityType, int? entityId,
            string action, string description, string userId)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    UserId = userId,
                    Action = action,
                    Entity = entityType,
                    EntityType = entityType,
                    EntityId = entityId,
                    Details = description,
                    ActionDate = DateTime.Now,
                    Timestamp = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId ?? "System"
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging store user activity");
            }
        }

        // Remove user activity
        public async Task LogRemoveUserActivityAsync(string entityType, int? entityId,
            string action, string description, string userId)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    UserId = userId,
                    Action = action,
                    Entity = entityType,
                    EntityType = entityType,
                    EntityId = entityId,
                    Details = description,
                    ActionDate = DateTime.Now,
                    Timestamp = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId ?? "System"
                };

                await _unitOfWork.ActivityLogs.AddAsync(activityLog);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging remove user activity");
            }
        }

        // Simple log async
        public async Task LogAsync(string entityName, int entityId, string action, string description)
        {
            await LogActivityAsync(entityName, entityId, action, description, "System");
        }

        // Get all activity logs with filters
        public async Task<IEnumerable<ActivityLogDto>> GetAllActivityLogsAsync(
            string userId = null, string entityName = null,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var logs = await _unitOfWork.ActivityLogs.GetAllAsync();

                var filtered = logs.AsQueryable();

                if (!string.IsNullOrEmpty(userId))
                    filtered = filtered.Where(l => l.UserId == userId);

                if (!string.IsNullOrEmpty(entityName))
                    filtered = filtered.Where(l => l.Entity == entityName);

                if (fromDate.HasValue)
                    filtered = filtered.Where(l => l.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    filtered = filtered.Where(l => l.Timestamp <= toDate.Value);

                return filtered.OrderByDescending(l => l.Timestamp)
                    .Select(l => new ActivityLogDto
                    {
                        Id = l.Id,
                        Action = l.Action,
                        Description = l.Details,
                        UserId = l.UserId,
                        EntityName = l.Entity,
                        EntityId = l.EntityId,
                        Timestamp = l.Timestamp,
                        TimeAgo = GetTimeAgo(l.Timestamp)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity logs");
                return Enumerable.Empty<ActivityLogDto>();
            }
        }

        // Get entity activity logs
        public async Task<IEnumerable<ActivityLogDto>> GetEntityActivityLogsAsync(
            string entityName, int entityId)
        {
            try
            {
                var logs = await _unitOfWork.ActivityLogs.FindAsync(
                    l => l.Entity == entityName && l.EntityId == entityId);

                return logs.OrderByDescending(l => l.Timestamp)
                    .Select(l => new ActivityLogDto
                    {
                        Id = l.Id,
                        Action = l.Action,
                        Description = l.Details,
                        UserId = l.UserId,
                        EntityName = l.Entity,
                        EntityId = l.EntityId,
                        Timestamp = l.Timestamp,
                        TimeAgo = GetTimeAgo(l.Timestamp)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity activity logs");
                return Enumerable.Empty<ActivityLogDto>();
            }
        }

        // Get user activity logs
        public async Task<IEnumerable<ActivityLogDto>> GetUserActivityLogsAsync(
            string userId, int? limit = null)
        {
            try
            {
                var logs = await _unitOfWork.ActivityLogs.FindAsync(l => l.UserId == userId);
                var ordered = logs.OrderByDescending(l => l.Timestamp);

                if (limit.HasValue)
                    ordered = (IOrderedEnumerable<ActivityLog>)ordered.Take(limit.Value);

                return ordered.Select(l => new ActivityLogDto
                {
                    Id = l.Id,
                    Action = l.Action,
                    Description = l.Details,
                    UserId = l.UserId,
                    EntityName = l.Entity,
                    EntityId = l.EntityId,
                    Timestamp = l.Timestamp,
                    TimeAgo = GetTimeAgo(l.Timestamp)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user activity logs");
                return Enumerable.Empty<ActivityLogDto>();
            }
        }

        // Get activity log by ID
        public async Task<ActivityLogDto> GetActivityLogByIdAsync(int id)
        {
            try
            {
                var log = await _unitOfWork.ActivityLogs.GetByIdAsync(id);
                if (log == null) return null;

                return new ActivityLogDto
                {
                    Id = log.Id,
                    Action = log.Action,
                    Description = log.Details,
                    UserId = log.UserId,
                    EntityName = log.Entity,
                    EntityId = log.EntityId,
                    Timestamp = log.Timestamp,
                    TimeAgo = GetTimeAgo(log.Timestamp)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity log by ID");
                return null;
            }
        }

        // Get system activity stats
        public async Task<SystemActivityStatsDto> GetSystemActivityStatsAsync(
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var logs = await _unitOfWork.ActivityLogs.GetAllAsync();
                var filtered = logs.AsQueryable();

                if (fromDate.HasValue)
                    filtered = filtered.Where(l => l.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    filtered = filtered.Where(l => l.Timestamp <= toDate.Value);

                var topActions = filtered.GroupBy(l => l.Action)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count());

                var topEntities = filtered.GroupBy(l => l.Entity)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count());

                var hourlyDistribution = filtered
                    .GroupBy(l => l.Timestamp.Hour)
                    .ToDictionary(g => g.Key, g => g.Count());

                return new SystemActivityStatsDto
                {
                    TotalActivities = filtered.Count(),
                    UniqueUsers = filtered.Select(l => l.UserId).Distinct().Count(),
                    TopActions = topActions,
                    TopEntities = topEntities,
                    HourlyDistribution = hourlyDistribution,
                    DailyTrend = null // Can be populated based on requirements
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system activity stats");
                return new SystemActivityStatsDto();
            }
        }

        // Archive old logs
        public async Task<int> ArchiveOldLogsAsync(int daysToKeep = 90)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var oldLogs = await _unitOfWork.ActivityLogs.FindAsync(
                    l => l.Timestamp < cutoffDate);

                // In a real implementation, you would move these to an archive table
                // For now, just return the count
                return oldLogs.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving old logs");
                return 0;
            }
        }

        // Delete archived logs
        public async Task<int> DeleteArchivedLogsAsync(int daysToKeep = 365)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var oldLogs = await _unitOfWork.ActivityLogs.FindAsync(
                    l => l.Timestamp < cutoffDate);

                var count = oldLogs.Count();
                _unitOfWork.ActivityLogs.RemoveRange(oldLogs);
                await _unitOfWork.CompleteAsync();

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting archived logs");
                return 0;
            }
        }

        // Get recent activities
        public async Task<IEnumerable<ActivityLogDto>> GetRecentActivitiesAsync(int count = 10)
        {
            try
            {
                var logs = await _unitOfWork.ActivityLogs.GetAllAsync();
                return logs.OrderByDescending(a => a.Timestamp)
                    .Take(count)
                    .Select(a => new ActivityLogDto
                    {
                        Id = a.Id,
                        Action = a.Action,
                        Description = a.Details ?? a.Description,  // Fixed: Use Details first, fallback to Description
                        EntityType = a.EntityType,  // Added: Include EntityType for URL generation
                        EntityId = a.EntityId,  // Added: Include EntityId for URL generation
                        UserId = a.UserId,
                        UserName = "User",
                        Timestamp = a.Timestamp,
                        TimeAgo = GetTimeAgo(a.Timestamp),
                        Icon = GetIconForAction(a.Action),
                        Color = GetColorForAction(a.Action)
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent activities");
                return Enumerable.Empty<ActivityLogDto>();
            }
        }

        // Get activity logs by date range
        public async Task<IEnumerable<ActivityLogDto>> GetActivityLogsByDateRangeAsync(
            DateTime startDate, DateTime endDate)
        {
            try
            {
                var logs = await _unitOfWork.ActivityLogs.FindAsync(a =>
                    a.Timestamp >= startDate && a.Timestamp <= endDate);

                return logs.Select(a => new ActivityLogDto
                {
                    Id = a.Id,
                    Action = a.Action,
                    Description = a.Details,
                    UserId = a.UserId,
                    Timestamp = a.Timestamp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity logs by date range");
                return Enumerable.Empty<ActivityLogDto>();
            }
        }

        // Get unread notifications
        public async Task<List<NotificationDto>> GetUnreadNotificationsAsync(string userId)
        {
            try
            {
                var notifications = await _unitOfWork.Notifications.FindAsync(
                    n => n.UserId == userId && !n.IsRead);

                return notifications.Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unread notifications");
                return new List<NotificationDto>();
            }
        }

        // Helper methods
        private string GetTimeAgo(DateTime timestamp)
        {
            var timeSpan = DateTime.Now - timestamp;
            if (timeSpan.TotalMinutes < 1) return "just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} days ago";
            return timestamp.ToString("MMM dd, yyyy");
        }

        private string GetIconForAction(string action)
        {
            return action?.ToLower() switch
            {
                "create" => "plus-circle",
                "update" => "edit",
                "delete" => "trash",
                "issue" => "share",
                "receive" => "download",
                "transfer" => "exchange-alt",
                "purchase" => "shopping-cart",
                "adjustment" => "sliders-h",
                "return" => "undo",
                _ => "info-circle"
            };
        }

        private string GetColorForAction(string action)
        {
            return action?.ToLower() switch
            {
                "create" => "success",
                "update" => "info",
                "delete" => "danger",
                "issue" => "danger",
                "receive" => "success",
                "transfer" => "warning",
                "purchase" => "info",
                "adjustment" => "secondary",
                "return" => "primary",
                _ => "default"
            };
        }
    }
}