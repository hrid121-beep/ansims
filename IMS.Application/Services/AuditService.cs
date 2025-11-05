using ClosedXML.Excel;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace IMS.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuditService> _logger;

        public AuditService(IUnitOfWork unitOfWork, ILogger<AuditService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<AuditDto>> GetAuditTrailAsync(string entity, int entityId)
        {
            var audits = await _unitOfWork.AuditLogs
                .Query()
                .Include(a => a.User)
                .Where(a => a.EntityName == entity && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return audits.Select(a => new AuditDto
            {
                Id = a.Id,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Action = a.Action,
                Changes = a.Changes,
                UserId = a.UserId,
                UserName = a.User?.FullName ?? a.UserId,
                Timestamp = a.Timestamp,
                IPAddress = a.IPAddress
            });
        }

        public async Task<IEnumerable<AuditDto>> GetAuditsByUserAsync(string userId)
        {
            var audits = await _unitOfWork.AuditLogs
                .Query()
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(100)
                .ToListAsync();

            return audits.Select(a => new AuditDto
            {
                Id = a.Id,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Action = a.Action,
                Changes = a.Changes,
                UserId = a.UserId,
                UserName = a.User?.FullName ?? a.UserId,
                Timestamp = a.Timestamp
            });
        }

        public async Task<IEnumerable<AuditDto>> GetAuditsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var audits = await _unitOfWork.AuditLogs
                .Query()
                .Include(a => a.User)
                .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            return audits.Select(a => new AuditDto
            {
                Id = a.Id,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Action = a.Action,
                Changes = a.Changes,
                UserId = a.UserId,
                UserName = a.User?.FullName ?? a.UserId,
                Timestamp = a.Timestamp
            });
        }

        public async Task LogAuditAsync(string entity, int entityId, string action, string changes, string userId)
        {
            try
            {
                var audit = new AuditLog
                {
                    EntityName = entity,
                    EntityId = entityId,
                    Action = action,
                    Changes = changes,
                    UserId = userId,
                    Timestamp = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.AuditLogs.AddAsync(audit);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging audit");
            }
        }

        public async Task<byte[]> GenerateAuditReportAsync(DateTime fromDate, DateTime toDate)
        {
            var audits = await GetAuditsByDateRangeAsync(fromDate, toDate);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Audit Trail");

            // Headers
            worksheet.Cell(1, 1).Value = "Timestamp";
            worksheet.Cell(1, 2).Value = "User";
            worksheet.Cell(1, 3).Value = "Entity";
            worksheet.Cell(1, 4).Value = "Action";
            worksheet.Cell(1, 5).Value = "Changes";

            // Data
            var row = 2;
            foreach (var audit in audits)
            {
                worksheet.Cell(row, 1).Value = audit.Timestamp;
                worksheet.Cell(row, 2).Value = audit.UserName;
                worksheet.Cell(row, 3).Value = $"{audit.EntityName} #{audit.EntityId}";
                worksheet.Cell(row, 4).Value = audit.Action;
                worksheet.Cell(row, 5).Value = audit.Changes;
                row++;
            }

            worksheet.Range(1, 1, 1, 5).Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}