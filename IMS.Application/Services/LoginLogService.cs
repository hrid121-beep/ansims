using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;

namespace IMS.Application.Services
{
    public class LoginLogService : ILoginLogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LoginLogDto>> GetAllLoginLogsAsync()
        {
            var logs = await _unitOfWork.LoginLogs.GetAllAsync();
            return logs.Select(log => new LoginLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                UserName = log.User?.FullName ?? "Unknown",
                IpAddress = log.IpAddress,
                LoginTime = log.LoginTime,
                LogoutTime = log.LogoutTime
            });
        }

        public async Task<IEnumerable<LoginLogDto>> GetLoginLogsByUserAsync(string userId)
        {
            var logs = await _unitOfWork.LoginLogs.FindAsync(l => l.UserId == userId);
            return logs.Select(log => new LoginLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                UserName = log.User?.FullName ?? "Unknown",
                IpAddress = log.IpAddress,
                LoginTime = log.LoginTime,
                LogoutTime = log.LogoutTime
            });
        }

        public async Task<IEnumerable<LoginLogDto>> GetLoginLogsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var logs = await _unitOfWork.LoginLogs.FindAsync(l => l.LoginTime >= fromDate && l.LoginTime <= toDate);
            return logs.Select(log => new LoginLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                UserName = log.User?.FullName ?? "Unknown",
                IpAddress = log.IpAddress,
                LoginTime = log.LoginTime,
                LogoutTime = log.LogoutTime
            });
        }

        public async Task<IEnumerable<LoginLogDto>> GetActiveSessionsAsync()
        {
            var logs = await _unitOfWork.LoginLogs.FindAsync(l => l.LogoutTime == null);
            return logs.Select(log => new LoginLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                UserName = log.User?.FullName ?? "Unknown",
                IpAddress = log.IpAddress,
                LoginTime = log.LoginTime,
                LogoutTime = log.LogoutTime
            });
        }

        public async Task LogLoginAsync(string userId, string ipAddress)
        {
            var loginLog = new LoginLog
            {
                UserId = userId,
                IpAddress = ipAddress,
                LoginTime = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            await _unitOfWork.LoginLogs.AddAsync(loginLog);
            await _unitOfWork.CompleteAsync();
        }

        public async Task LogLogoutAsync(string userId)
        {
            var logs = await _unitOfWork.LoginLogs.FindAsync(l => l.UserId == userId && l.LogoutTime == null);
            var latestLog = logs.OrderByDescending(l => l.LoginTime).FirstOrDefault();

            if (latestLog != null)
            {
                latestLog.LogoutTime = DateTime.Now;
                _unitOfWork.LoginLogs.Update(latestLog);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<int> GetLoginCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var logs = await _unitOfWork.LoginLogs.GetAllAsync();
            var query = logs.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(l => l.LoginTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(l => l.LoginTime <= toDate.Value);

            return query.Count();
        }

        public async Task<IEnumerable<LoginLogDto>> GetFailedLoginAttemptsAsync(DateTime? fromDate = null)
        {
            // In this implementation, we don't track failed attempts separately
            // This would need to be implemented with a separate table or field
            return new List<LoginLogDto>();
        }

        public async Task<IEnumerable<LoginLogDto>> GetPagedLoginLogsAsync(int pageNumber, int pageSize)
        {
            var allLogs = await GetAllLoginLogsAsync();
            return allLogs
                .OrderByDescending(l => l.LoginTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task<TimeSpan> GetAverageSessionDurationAsync(string userId = null)
        {
            var logs = await _unitOfWork.LoginLogs.GetAllAsync();

            if (!string.IsNullOrEmpty(userId))
                logs = logs.Where(l => l.UserId == userId);

            var completedSessions = logs.Where(l => l.LogoutTime.HasValue);

            if (!completedSessions.Any())
                return TimeSpan.Zero;

            var totalDuration = completedSessions
                .Select(l => (l.LogoutTime.Value - l.LoginTime).TotalMinutes)
                .Average();

            return TimeSpan.FromMinutes(totalDuration);
        }

        public async Task CleanupOldLogsAsync(int daysToKeep = 90)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
            var oldLogs = await _unitOfWork.LoginLogs.FindAsync(l => l.LoginTime < cutoffDate);

            _unitOfWork.LoginLogs.RemoveRange(oldLogs);
            await _unitOfWork.CompleteAsync();
        }
    }
}