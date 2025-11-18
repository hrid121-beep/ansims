using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Services
{
    public class TemperatureLogService : ITemperatureLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActivityLogService _activityLogService;

        public TemperatureLogService(
            IUnitOfWork unitOfWork,
            IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork;
            _activityLogService = activityLogService;
        }

        public async Task<TemperatureLogDto> LogTemperatureAsync(TemperatureLogDto logDto)
        {
            var log = new TemperatureLog
            {
                LogTime = logDto.LogTime,
                StoreId = logDto.StoreId,
                Temperature = logDto.Temperature,
                Humidity = logDto.Humidity,
                Unit = logDto.Unit ?? "Celsius",
                RecordedBy = logDto.RecordedBy,
                Equipment = logDto.Equipment,
                IsAlert = logDto.IsAlert,
                AlertReason = logDto.AlertReason,
                Status = logDto.Status ?? "Normal",
                CreatedBy = logDto.RecordedBy,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            await _unitOfWork.TemperatureLogs.AddAsync(log);
            await _unitOfWork.CompleteAsync();

            logDto.Id = log.Id;
            return logDto;
        }

        public async Task<IEnumerable<TemperatureLogDto>> GetTemperatureLogsAsync(
            int? storeId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _unitOfWork.TemperatureLogs
                .Query()
                .Include(t => t.Store)
                .Where(t => t.IsActive);

            if (storeId.HasValue)
                query = query.Where(t => t.StoreId == storeId.Value);

            if (fromDate.HasValue)
                query = query.Where(t => t.LogTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.LogTime <= toDate.Value);

            var logs = await query.OrderByDescending(t => t.LogTime).ToListAsync();

            return logs.Select(log => new TemperatureLogDto
            {
                Id = log.Id,
                LogTime = log.LogTime,
                StoreId = log.StoreId,
                StoreName = log.Store?.Name,
                Temperature = log.Temperature,
                Humidity = log.Humidity,
                Unit = log.Unit,
                RecordedBy = log.RecordedBy,
                Equipment = log.Equipment,
                IsAlert = log.IsAlert,
                AlertReason = log.AlertReason,
                Status = log.Status
            }).ToList();
        }

        public async Task<IEnumerable<TemperatureLogDto>> GetAlertsAsync(int? storeId)
        {
            var query = _unitOfWork.TemperatureLogs
                .Query()
                .Include(t => t.Store)
                .Where(t => t.IsActive && t.IsAlert);

            if (storeId.HasValue)
                query = query.Where(t => t.StoreId == storeId.Value);

            var alerts = await query.OrderByDescending(t => t.LogTime).ToListAsync();

            return alerts.Select(log => new TemperatureLogDto
            {
                Id = log.Id,
                LogTime = log.LogTime,
                StoreId = log.StoreId,
                StoreName = log.Store?.Name,
                Temperature = log.Temperature,
                Humidity = log.Humidity,
                Unit = log.Unit,
                RecordedBy = log.RecordedBy,
                Equipment = log.Equipment,
                IsAlert = log.IsAlert,
                AlertReason = log.AlertReason,
                Status = log.Status
            }).ToList();
        }

        public async Task<bool> CheckTemperatureComplianceAsync(int storeId)
        {
            // Get last 24 hours logs
            var yesterday = DateTime.Now.AddHours(-24);
            var logs = await _unitOfWork.TemperatureLogs
                .Query()
                .Where(t => t.StoreId == storeId && t.LogTime >= yesterday && t.IsActive)
                .ToListAsync();

            // Check if there are any alerts in last 24 hours
            return !logs.Any(l => l.IsAlert);
        }

        public async Task<byte[]> GenerateTemperatureReportAsync(int? storeId, DateTime fromDate, DateTime toDate)
        {
            // TODO: Implement report generation
            throw new NotImplementedException("Temperature report generation not yet implemented");
        }

        public async Task SendTemperatureAlertAsync(int storeId, decimal temperature, string reason)
        {
            // TODO: Implement alert sending
            await Task.CompletedTask;
        }

        public async Task<TemperatureStatisticsDto> GetTemperatureStatisticsAsync(
            int? storeId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _unitOfWork.TemperatureLogs
                .Query()
                .Where(t => t.IsActive);

            if (storeId.HasValue)
                query = query.Where(t => t.StoreId == storeId.Value);

            if (fromDate.HasValue)
                query = query.Where(t => t.LogTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.LogTime <= toDate.Value);

            var logs = await query.ToListAsync();

            return new TemperatureStatisticsDto
            {
                TotalLogs = logs.Count,
                AlertCount = logs.Count(l => l.IsAlert),
                AverageTemperature = logs.Any() ? logs.Average(l => l.Temperature) : 0,
                MinTemperature = logs.Any() ? logs.Min(l => l.Temperature) : 0,
                MaxTemperature = logs.Any() ? logs.Max(l => l.Temperature) : 0,
                AverageHumidity = logs.Any() ? logs.Average(l => l.Humidity) : 0
            };
        }

        /// <summary>
        /// Deletes a temperature log with time-based validation
        /// </summary>
        public async Task<bool> DeleteTemperatureLogAsync(int logId, string deletedBy)
        {
            var log = await _unitOfWork.TemperatureLogs
                .Query()
                .FirstOrDefaultAsync(t => t.Id == logId);

            if (log == null)
                throw new InvalidOperationException("Temperature log not found");

            // Time-based validation: Only allow deletion within 48 hours
            var hoursSinceLog = (DateTime.Now - log.LogTime).TotalHours;
            if (hoursSinceLog > 48)
            {
                throw new InvalidOperationException(
                    $"Cannot delete temperature log older than 48 hours. " +
                    $"This log was recorded {hoursSinceLog:F1} hours ago. " +
                    $"Contact administrator for historical data corrections.");
            }

            // Soft delete (preserve audit trail)
            log.IsActive = false;
            log.UpdatedBy = deletedBy;
            log.UpdatedAt = DateTime.Now;

            _unitOfWork.TemperatureLogs.Update(log);
            await _unitOfWork.CompleteAsync();

            // Log the deletion
            await _activityLogService.LogActivityAsync(
                "Temperature Log",
                log.Id,
                "Delete",
                $"Temperature log from {log.LogTime:yyyy-MM-dd HH:mm} deleted by {deletedBy}. " +
                $"Temperature: {log.Temperature}°{log.Unit}, Store: {log.StoreId}",
                deletedBy
            );

            return true;
        }
    }
}
