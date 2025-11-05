using ClosedXML.Excel;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class ExpiryTrackingService : IExpiryTrackingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ExpiryTrackingService> _logger;
        private readonly IUserContext _userContext;

        public ExpiryTrackingService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IEmailService emailService,
            ILogger<ExpiryTrackingService> logger,
            IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<ExpiryTrackingDto> AddExpiryTrackingAsync(ExpiryTrackingDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var tracking = new ExpiryTracking
                {
                    ItemId = dto.ItemId,
                    StoreId = dto.StoreId,
                    BatchNumber = dto.BatchNumber,
                    ExpiryDate = dto.ExpiryDate,
                    Quantity = dto.Quantity,
                    Status = CalculateStatus(dto.ExpiryDate),
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.UserId,
                    IsActive = true
                };

                await _unitOfWork.ExpiryTrackings.AddAsync(tracking);

                // Check if alert needed
                if (tracking.Status == "Warning" || tracking.Status == "Expired")
                {
                    await SendExpiryAlertAsync(tracking);
                }

                await _unitOfWork.CommitTransactionAsync();

                dto.Id = tracking.Id;
                dto.Status = tracking.Status;
                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error adding expiry tracking");
                throw;
            }
        }

        public async Task<IEnumerable<ExpiryTrackingDto>> GetExpiringItemsAsync(int daysBeforeExpiry = 30)
        {
            var expiryDate = DateTime.Now.AddDays(daysBeforeExpiry);

            var items = await _unitOfWork.ExpiryTrackings.Query()
                .Include(e => e.Item)
                .Include(e => e.Store)
                .Where(e => e.IsActive &&
                           e.ExpiryDate <= expiryDate &&
                           e.ExpiryDate > DateTime.Now &&
                           e.Status != "Disposed")
                .OrderBy(e => e.ExpiryDate)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<ExpiryTrackingDto>> GetExpiredItemsAsync()
        {
            var items = await _unitOfWork.ExpiryTrackings.Query()
                .Include(e => e.Item)
                .Include(e => e.Store)
                .Where(e => e.IsActive &&
                           e.ExpiryDate <= DateTime.Now &&
                           e.Status != "Disposed")
                .OrderBy(e => e.ExpiryDate)
                .ToListAsync();

            return items.Select(MapToDto);
        }

        public async Task<bool> ProcessExpiredItemAsync(int id, string disposalReason)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var tracking = await _unitOfWork.ExpiryTrackings.GetByIdAsync(id);
                if (tracking == null)
                    return false;

                tracking.Status = "Disposed";
                tracking.DisposalDate = DateTime.Now;
                tracking.DisposalReason = disposalReason;
                tracking.DisposedBy = _userContext.UserId;
                tracking.UpdatedAt = DateTime.Now;
                tracking.UpdatedBy = _userContext.UserId;

                _unitOfWork.ExpiryTrackings.Update(tracking);

                // Create write-off for expired item
                var writeOff = new WriteOff
                {
                    WriteOffNo = await GenerateWriteOffNoAsync(),
                    WriteOffDate = DateTime.Now,
                    StoreId = tracking.StoreId,
                    Reason = $"Expired - {disposalReason}",
                    Status = "Approved",
                    TotalValue = 0, // Will be calculated based on item cost
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.UserId,
                    ApprovedBy = _userContext.UserId,
                    ApprovedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.WriteOffs.AddAsync(writeOff);

                // Add write-off item
                var item = await _unitOfWork.Items.GetByIdAsync(tracking.ItemId);
                var writeOffItem = new WriteOffItem
                {
                    WriteOffId = writeOff.Id,
                    ItemId = tracking.ItemId,
                    Quantity = tracking.Quantity,
                    UnitCost = item?.UnitPrice ?? 0,
                    TotalCost = tracking.Quantity * (item?.UnitPrice ?? 0),
                    Reason = "Expired",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.UserId,
                    IsActive = true
                };

                await _unitOfWork.WriteOffItems.AddAsync(writeOffItem);
                writeOff.TotalValue = writeOffItem.TotalCost;
                _unitOfWork.WriteOffs.Update(writeOff);

                // Update stock
                var storeItem = await _unitOfWork.StoreItems
                    .FirstOrDefaultAsync(si => si.StoreId == tracking.StoreId && si.ItemId == tracking.ItemId);

                if (storeItem != null)
                {
                    storeItem.Quantity -= tracking.Quantity;
                    storeItem.UpdatedAt = DateTime.Now;
                    storeItem.UpdatedBy = _userContext.UserId;
                    _unitOfWork.StoreItems.Update(storeItem);
                }

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error processing expired item");
                throw;
            }
        }

        public async Task SendExpiryAlertsAsync()
        {
            // Get items expiring in next 30 days
            var expiringItems = await GetExpiringItemsAsync(30);

            // Get items expiring in next 7 days
            var urgentItems = expiringItems.Where(e => e.ExpiryDate <= DateTime.Now.AddDays(7));

            if (urgentItems.Any())
            {
                var emailBody = GenerateExpiryAlertEmail(urgentItems.ToList(), "Urgent: Items Expiring Soon");
                await _emailService.SendEmailAsync(
                    "storemanager@company.com",
                    "URGENT: Items Expiring Within 7 Days",
                    emailBody,
                    true);
            }

            // Send notifications
            foreach (var item in expiringItems)
            {
                var daysToExpiry = (item.ExpiryDate - DateTime.Now).Days;
                var priority = daysToExpiry <= 7 ? "High" : "Normal";

                await _notificationService.SendToRoleAsync("StoreManager",
                    "Expiry Alert",
                    $"{item.ItemName} (Batch: {item.BatchNumber}) expires in {daysToExpiry} days",
                    daysToExpiry <= 7 ? "error" : "warning");
            }
        }

        public async Task<byte[]> GenerateExpiryReportAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _unitOfWork.ExpiryTrackings.Query()
                .Include(e => e.Item)
                .Include(e => e.Store)
                .Where(e => e.IsActive);

            if (fromDate.HasValue)
                query = query.Where(e => e.ExpiryDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(e => e.ExpiryDate <= toDate.Value);

            var items = await query.OrderBy(e => e.ExpiryDate).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Expiry Report");

            // Headers
            worksheet.Cell(1, 1).Value = "Item Code";
            worksheet.Cell(1, 2).Value = "Item Name";
            worksheet.Cell(1, 3).Value = "Batch Number";
            worksheet.Cell(1, 4).Value = "Store";
            worksheet.Cell(1, 5).Value = "Quantity";
            worksheet.Cell(1, 6).Value = "Expiry Date";
            worksheet.Cell(1, 7).Value = "Days to Expiry";
            worksheet.Cell(1, 8).Value = "Status";

            // Data
            var row = 2;
            foreach (var item in items)
            {
                var daysToExpiry = (item.ExpiryDate - DateTime.Now).Days;
                worksheet.Cell(row, 1).Value = item.Item?.ItemCode;
                worksheet.Cell(row, 2).Value = item.Item?.Name;
                worksheet.Cell(row, 3).Value = item.BatchNumber;
                worksheet.Cell(row, 4).Value = item.Store?.Name;
                worksheet.Cell(row, 5).Value = item.Quantity;
                worksheet.Cell(row, 6).Value = item.ExpiryDate;
                worksheet.Cell(row, 7).Value = daysToExpiry;
                worksheet.Cell(row, 8).Value = item.Status;

                // Color coding
                if (daysToExpiry < 0)
                    worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.Red;
                else if (daysToExpiry <= 7)
                    worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.Orange;
                else if (daysToExpiry <= 30)
                    worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.Yellow;

                row++;
            }

            worksheet.Range(1, 1, 1, 8).Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<ExpiryStatisticsDto> GetExpiryStatisticsAsync(int? storeId)
        {
            var query = _unitOfWork.ExpiryTrackings.Query()
                .Where(e => e.IsActive && e.Status != "Disposed");

            if (storeId.HasValue)
                query = query.Where(e => e.StoreId == storeId);

            var items = await query.ToListAsync();

            return new ExpiryStatisticsDto
            {
                TotalItems = items.Count,
                ExpiredItems = items.Count(i => i.Status == "Expired"),
                ExpiringIn7Days = items.Count(i => i.ExpiryDate > DateTime.Now &&
                                                  i.ExpiryDate <= DateTime.Now.AddDays(7)),
                ExpiringIn30Days = items.Count(i => i.ExpiryDate > DateTime.Now &&
                                                   i.ExpiryDate <= DateTime.Now.AddDays(30)),
                TotalValue = 0 // Calculate based on item values
            };
        }

        private string CalculateStatus(DateTime expiryDate)
        {
            var daysToExpiry = (expiryDate - DateTime.Now).Days;

            if (daysToExpiry < 0)
                return "Expired";
            else if (daysToExpiry <= 30)
                return "Warning";
            else
                return "Valid";
        }

        private async Task SendExpiryAlertAsync(ExpiryTracking tracking)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(tracking.ItemId);
            var store = await _unitOfWork.Stores.GetByIdAsync(tracking.StoreId);

            await _notificationService.SendNotificationAsync(new NotificationDto
            {
                UserId = null, // ✅ Use null for role-based notifications
                TargetRole = "StoreManager", // Send to store managers
                Title = "Expiry Alert",
                Message = $"{item?.Name} (Batch: {tracking.BatchNumber}) in {store?.Name} is {tracking.Status}",
                Type = tracking.Status == "Expired" ? "error" : "warning",
                Priority = tracking.Status == "Expired" ? "High" : "Normal",
                RelatedEntity = "ExpiryTracking",
                RelatedEntityId = tracking.Id
            });
        }

        private ExpiryTrackingDto MapToDto(ExpiryTracking tracking)
        {
            return new ExpiryTrackingDto
            {
                Id = tracking.Id,
                ItemId = tracking.ItemId,
                ItemName = tracking.Item?.Name,
                StoreId = tracking.StoreId,
                StoreName = tracking.Store?.Name,
                BatchNumber = tracking.BatchNumber,
                ExpiryDate = tracking.ExpiryDate,
                Quantity = tracking.Quantity,
                Status = tracking.Status,
                DisposalDate = tracking.DisposalDate,
                DisposalReason = tracking.DisposalReason,
                DisposedBy = tracking.DisposedBy,
                IsAlertSent = tracking.IsAlertSent
            };
        }

        private string GenerateExpiryAlertEmail(List<ExpiryTrackingDto> items, string subject)
        {
            var html = $@"
            <h3>{subject}</h3>
            <table border='1' cellpadding='5' cellspacing='0'>
                <tr>
                    <th>Item</th>
                    <th>Batch</th>
                    <th>Store</th>
                    <th>Quantity</th>
                    <th>Expiry Date</th>
                    <th>Days Remaining</th>
                </tr>";

            foreach (var item in items)
            {
                var daysRemaining = (item.ExpiryDate - DateTime.Now).Days;
                html += $@"
                <tr>
                    <td>{item.ItemName}</td>
                    <td>{item.BatchNumber}</td>
                    <td>{item.StoreName}</td>
                    <td>{item.Quantity}</td>
                    <td>{item.ExpiryDate:yyyy-MM-dd}</td>
                    <td>{daysRemaining}</td>
                </tr>";
            }

            html += "</table>";
            return html;
        }

        // === CONTINUING PHASE 3 IMPLEMENTATION ===

        private async Task<string> GenerateWriteOffNoAsync()
        {
            var year = DateTime.Now.Year;
            var lastWriteOff = await _unitOfWork.WriteOffs
                .Query()
                .Where(w => w.WriteOffNo.StartsWith($"WO-{year}-"))
                .OrderByDescending(w => w.WriteOffNo)
                .FirstOrDefaultAsync();

            if (lastWriteOff == null)
                return $"WO-{year}-0001";

            var lastNumber = int.Parse(lastWriteOff.WriteOffNo.Split('-').Last());
            return $"WO-{year}-{(lastNumber + 1).ToString("D4")}";
        }
    }
}
