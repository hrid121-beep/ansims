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
    public class InventoryCycleCountService : IInventoryCycleCountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockAdjustmentService _stockAdjustmentService;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<InventoryCycleCountService> _logger;
        private readonly IUserContext _userContext;

        public InventoryCycleCountService(
            IUnitOfWork unitOfWork,
            IStockAdjustmentService stockAdjustmentService,
            IActivityLogService activityLogService,
            INotificationService notificationService,
            ILogger<InventoryCycleCountService> logger,
            IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _stockAdjustmentService = stockAdjustmentService;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<CycleCountDto> CreateCycleCountAsync(CycleCountDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cycleCount = new InventoryCycleCount
                {
                    CountNumber = await GenerateCountNumberAsync(),
                    CountDate = dto.CountDate,
                    StoreId = dto.StoreId,
                    CountType = dto.CountType,
                    Status = "Planned",
                    CountedBy = _userContext.UserId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.InventoryCycleCounts.AddAsync(cycleCount);

                // Get items to count based on count type
                var itemsToCount = await GetItemsToCountAsync(dto.StoreId.Value, dto.CountType);

                foreach (var item in itemsToCount)
                {
                    var countItem = new InventoryCycleCountItem
                    {
                        CycleCountId = cycleCount.Id,
                        ItemId = item.ItemId,
                        SystemQuantity = item.Quantity,
                        CountedQuantity = 0,
                        VarianceQuantity = 0,
                        IsAdjusted = false,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userContext.UserId,
                        IsActive = true
                    };

                    await _unitOfWork.InventoryCycleCountItems.AddAsync(countItem);
                }

                cycleCount.TotalItems = itemsToCount.Count();
                _unitOfWork.InventoryCycleCounts.Update(cycleCount);

                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogAsync(
                    "InventoryCycleCount",
                    cycleCount.Id,
                    "Created",
                    $"Created cycle count {cycleCount.CountNumber}");

                dto.Id = cycleCount.Id;
                dto.CountNumber = cycleCount.CountNumber;
                dto.Status = cycleCount.Status;
                dto.TotalItems = cycleCount.TotalItems;

                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating cycle count");
                throw;
            }
        }

        public async Task<bool> StartCycleCountAsync(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cycleCount = await _unitOfWork.InventoryCycleCounts.GetByIdAsync(id);
                if (cycleCount == null || cycleCount.Status != "Planned")
                    return false;

                cycleCount.Status = "InProgress";
                cycleCount.StartTime = DateTime.Now;
                cycleCount.UpdatedAt = DateTime.Now;
                cycleCount.UpdatedBy = _userContext.UserId;

                _unitOfWork.InventoryCycleCounts.Update(cycleCount);

                await _unitOfWork.CommitTransactionAsync();

                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = cycleCount.CountedBy,
                    Title = "Cycle Count Started",
                    Message = $"Cycle count {cycleCount.CountNumber} has been started",
                    Type = "info",
                    RelatedEntity = "InventoryCycleCount",
                    RelatedEntityId = cycleCount.Id
                });

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error starting cycle count");
                throw;
            }
        }

        public async Task<bool> AddCountItemAsync(int cycleCountId, CycleCountItemDto itemDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var countItem = await _unitOfWork.InventoryCycleCountItems
                    .FirstOrDefaultAsync(ci => ci.CycleCountId == cycleCountId && ci.ItemId == itemDto.ItemId);

                if (countItem == null)
                    return false;

                countItem.CountedQuantity = itemDto.CountedQuantity;
                countItem.VarianceQuantity = (decimal)(itemDto.CountedQuantity - countItem.SystemQuantity);
                countItem.VarianceReason = itemDto.VarianceReason;
                countItem.UpdatedAt = DateTime.Now;
                countItem.UpdatedBy = _userContext.UserId;

                // Calculate variance value
                var item = await _unitOfWork.Items.GetByIdAsync(countItem.ItemId);
                countItem.VarianceValue = countItem.VarianceQuantity * (item?.UnitPrice ?? 0);

                _unitOfWork.InventoryCycleCountItems.Update(countItem);

                // Update cycle count progress
                var cycleCount = await _unitOfWork.InventoryCycleCounts.GetByIdAsync(cycleCountId);
                var allItems = await _unitOfWork.InventoryCycleCountItems
                    .FindAsync(ci => ci.CycleCountId == cycleCountId);

                cycleCount.CountedItems = allItems.Count(ci => ci.CountedQuantity > 0);
                cycleCount.VarianceItems = allItems.Count(ci => ci.VarianceQuantity != 0);
                cycleCount.TotalVarianceValue = allItems.Sum(ci => Math.Abs(ci.VarianceValue));
                cycleCount.UpdatedAt = DateTime.Now;
                cycleCount.UpdatedBy = _userContext.UserId;

                _unitOfWork.InventoryCycleCounts.Update(cycleCount);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error adding count item");
                throw;
            }
        }

        public async Task<bool> CompleteCycleCountAsync(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cycleCount = await _unitOfWork.InventoryCycleCounts
                    .Query()
                    .Include(cc => cc.Items)
                    .FirstOrDefaultAsync(cc => cc.Id == id);

                if (cycleCount == null || cycleCount.Status != "InProgress")
                    return false;

                cycleCount.Status = "Completed";
                cycleCount.EndTime = DateTime.Now;
                cycleCount.Duration = cycleCount.EndTime - cycleCount.StartTime;
                cycleCount.UpdatedAt = DateTime.Now;
                cycleCount.UpdatedBy = _userContext.UserId;

                _unitOfWork.InventoryCycleCounts.Update(cycleCount);

                // Send notification for approval if variances exist
                if (cycleCount.VarianceItems > 0)
                {
                    await _notificationService.SendToRoleAsync("StoreManager",
                        "Cycle Count Approval Required",
                        $"Cycle count {cycleCount.CountNumber} completed with {cycleCount.VarianceItems} variances",
                        "warning");
                }

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error completing cycle count");
                throw;
            }
        }

        public async Task<bool> ApproveAdjustmentsAsync(int id, string approvedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cycleCount = await _unitOfWork.InventoryCycleCounts
                    .Query()
                    .Include(cc => cc.Items)
                    .FirstOrDefaultAsync(cc => cc.Id == id);

                if (cycleCount == null || cycleCount.Status != "Completed")
                    return false;

                cycleCount.VerifiedBy = approvedBy;
                cycleCount.UpdatedAt = DateTime.Now;
                cycleCount.UpdatedBy = approvedBy;

                // Create stock adjustments for variances
                foreach (var countItem in cycleCount.Items.Where(ci => ci.VarianceQuantity != 0))
                {
                    var adjustmentDto = new StockAdjustmentDto
                    {
                        StoreId = cycleCount.StoreId,
                        ItemId = countItem.ItemId,
                        AdjustmentType = countItem.VarianceQuantity > 0 ? "Increase" : "Decrease",
                        OldQuantity = countItem.SystemQuantity,
                        NewQuantity = countItem.CountedQuantity,
                        AdjustmentQuantity = Math.Abs(countItem.VarianceQuantity),
                        Reason = $"Cycle Count Adjustment - {countItem.VarianceReason}",
                        ReferenceDocument = cycleCount.CountNumber,
                        Status = "Approved",
                        CreatedBy = approvedBy
                    };

                    await _stockAdjustmentService.CreateStockAdjustmentAsync(adjustmentDto);

                    countItem.IsAdjusted = true;
                    countItem.AdjustmentDate = DateTime.Now;
                    countItem.AdjustedBy = approvedBy;
                    _unitOfWork.InventoryCycleCountItems.Update(countItem);
                }

                _unitOfWork.InventoryCycleCounts.Update(cycleCount);
                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving adjustments");
                throw;
            }
        }

        public async Task<byte[]> GenerateCycleCountReportAsync(int id)
        {
            var cycleCount = await GetCycleCountByIdAsync(id);
            if (cycleCount == null)
                throw new InvalidOperationException("Cycle count not found");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Cycle Count Report");

            // Header information
            worksheet.Cell(1, 1).Value = "Cycle Count Report";
            worksheet.Cell(2, 1).Value = $"Count Number: {cycleCount.CountNumber}";
            worksheet.Cell(3, 1).Value = $"Date: {cycleCount.CountDate:yyyy-MM-dd}";
            worksheet.Cell(4, 1).Value = $"Store: {cycleCount.StoreName}";
            worksheet.Cell(5, 1).Value = $"Status: {cycleCount.Status}";

            // Summary
            worksheet.Cell(7, 1).Value = "Summary";
            worksheet.Cell(8, 1).Value = "Total Items:";
            worksheet.Cell(8, 2).Value = cycleCount.TotalItems;
            worksheet.Cell(9, 1).Value = "Counted Items:";
            worksheet.Cell(9, 2).Value = cycleCount.CountedItems;
            worksheet.Cell(10, 1).Value = "Variance Items:";
            worksheet.Cell(10, 2).Value = cycleCount.VarianceItems;
            worksheet.Cell(11, 1).Value = "Total Variance Value:";
            worksheet.Cell(11, 2).Value = cycleCount.TotalVarianceValue;

            // Detail headers
            var row = 13;
            worksheet.Cell(row, 1).Value = "Item Code";
            worksheet.Cell(row, 2).Value = "Item Name";
            worksheet.Cell(row, 3).Value = "System Qty";
            worksheet.Cell(row, 4).Value = "Counted Qty";
            worksheet.Cell(row, 5).Value = "Variance";
            worksheet.Cell(row, 6).Value = "Variance Value";
            worksheet.Cell(row, 7).Value = "Reason";
            worksheet.Cell(row, 8).Value = "Adjusted";

            // Detail data
            row++;
            foreach (var item in cycleCount.Items)
            {
                worksheet.Cell(row, 1).Value = item.ItemCode;
                worksheet.Cell(row, 2).Value = item.ItemName;
                worksheet.Cell(row, 3).Value = item.SystemQuantity;
                worksheet.Cell(row, 4).Value = item.CountedQuantity;
                worksheet.Cell(row, 5).Value = item.VarianceQuantity;
                worksheet.Cell(row, 6).Value = item.VarianceValue;
                worksheet.Cell(row, 7).Value = item.VarianceReason;
                worksheet.Cell(row, 8).Value = item.IsAdjusted ? "Yes" : "No";

                if (item.VarianceQuantity != 0)
                    worksheet.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.Yellow;

                row++;
            }

            // Formatting
            worksheet.Range(1, 1, 1, 1).Style.Font.Bold = true;
            worksheet.Range(1, 1, 1, 1).Style.Font.FontSize = 16;
            worksheet.Range(7, 1, 7, 1).Style.Font.Bold = true;
            worksheet.Range(13, 1, 13, 8).Style.Font.Bold = true;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<CycleCountDto> GetCycleCountByIdAsync(int id)
        {
            var cycleCount = await _unitOfWork.InventoryCycleCounts
                .Query()
                .Include(cc => cc.Store)
                .Include(cc => cc.CountedByUser)
                .Include(cc => cc.VerifiedByUser)
                .Include(cc => cc.Items)
                    .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (cycleCount == null)
                return null;

            return new CycleCountDto
            {
                Id = cycleCount.Id,
                CountNumber = cycleCount.CountNumber,
                CountDate = cycleCount.CountDate,
                StoreId = cycleCount.StoreId,
                StoreName = cycleCount.Store?.Name,
                CountType = cycleCount.CountType,
                Status = cycleCount.Status,
                CountedBy = cycleCount.CountedBy,
                CountedByName = cycleCount.CountedByUser?.FullName,
                VerifiedBy = cycleCount.VerifiedBy,
                VerifiedByName = cycleCount.VerifiedByUser?.FullName,
                StartTime = cycleCount.StartTime,
                EndTime = cycleCount.EndTime,
                Duration = cycleCount.Duration,
                TotalItems = cycleCount.TotalItems,
                CountedItems = cycleCount.CountedItems,
                VarianceItems = cycleCount.VarianceItems,
                TotalVarianceValue = cycleCount.TotalVarianceValue,
                Notes = cycleCount.Notes,
                Items = cycleCount.Items.Select(ci => new CycleCountItemDto
                {
                    Id = ci.Id,
                    ItemId = ci.ItemId,
                    ItemName = ci.Item?.Name,
                    ItemCode = ci.Item?.ItemCode,
                    SystemQuantity = ci.SystemQuantity,
                    CountedQuantity = ci.CountedQuantity,
                    VarianceQuantity = ci.VarianceQuantity,
                    VarianceValue = ci.VarianceValue,
                    VarianceReason = ci.VarianceReason,
                    IsAdjusted = ci.IsAdjusted,
                    AdjustmentDate = ci.AdjustmentDate
                }).ToList()
            };
        }

        public async Task<IEnumerable<CycleCountDto>> GetActiveCycleCountsAsync()
        {
            var counts = await _unitOfWork.InventoryCycleCounts
                .Query()
                .Include(cc => cc.Store)
                .Include(cc => cc.CountedByUser)
                .Where(cc => cc.IsActive && cc.Status != "Cancelled")
                .OrderByDescending(cc => cc.CountDate)
                .ToListAsync();

            return counts.Select(cc => new CycleCountDto
            {
                Id = cc.Id,
                CountNumber = cc.CountNumber,
                CountDate = cc.CountDate,
                StoreId = cc.StoreId,
                StoreName = cc.Store?.Name,
                CountType = cc.CountType,
                Status = cc.Status,
                CountedByName = cc.CountedByUser?.FullName,
                TotalItems = cc.TotalItems,
                CountedItems = cc.CountedItems,
                VarianceItems = cc.VarianceItems
            });
        }

        public async Task<CycleCountStatisticsDto> GetCycleCountStatisticsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _unitOfWork.InventoryCycleCounts.Query()
                .Where(cc => cc.IsActive);

            if (fromDate.HasValue)
                query = query.Where(cc => cc.CountDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(cc => cc.CountDate <= toDate.Value);

            var counts = await query.ToListAsync();

            return new CycleCountStatisticsDto
            {
                TotalCounts = counts.Count,
                CompletedCounts = counts.Count(cc => cc.Status == "Completed"),
                InProgressCounts = counts.Count(cc => cc.Status == "InProgress"),
                AverageCountTime = counts.Where(cc => cc.Duration.HasValue)
                    .Select(cc => cc.Duration.Value.TotalMinutes)
                    .DefaultIfEmpty(0)
                    .Average(),
                AverageVarianceItems = counts.Average(cc => cc.VarianceItems),
                TotalVarianceValue = counts.Sum(cc => cc.TotalVarianceValue)
            };
        }

        private async Task<string> GenerateCountNumberAsync()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var lastCount = await _unitOfWork.InventoryCycleCounts
                .Query()
                .Where(cc => cc.CountNumber.StartsWith($"CC-{year}{month:D2}-"))
                .OrderByDescending(cc => cc.CountNumber)
                .FirstOrDefaultAsync();

            if (lastCount == null)
                return $"CC-{year}{month:D2}-0001";

            var lastNumber = int.Parse(lastCount.CountNumber.Split('-').Last());
            return $"CC-{year}{month:D2}-{(lastNumber + 1).ToString("D4")}";
        }

        private async Task<IEnumerable<StoreItem>> GetItemsToCountAsync(int storeId, string countType)
        {
            var query = _unitOfWork.StoreItems.Query()
                .Include(si => si.Item)
                .Where(si => si.StoreId == storeId && si.IsActive);

            switch (countType)
            {
                case "Full":
                    return await query.ToListAsync();

                case "ABC":
                    // Get high-value items (A category)
                    return await query
                        .OrderByDescending(si => si.Quantity * (si.Item.UnitPrice ?? 0))
                        .Take(20)
                        .ToListAsync();

                case "Random":
                    // Get random 10% of items
                    var totalItems = await query.CountAsync();
                    var sampleSize = Math.Max(1, totalItems / 10);
                    return await query
                        .OrderBy(si => Guid.NewGuid())
                        .Take(sampleSize)
                        .ToListAsync();

                case "Partial":
                default:
                    // Get items with recent movements
                    var recentMovements = await _unitOfWork.StockMovements
                        .Query()
                        .Where(sm => sm.StoreId == storeId &&
                               sm.MovementDate >= DateTime.Now.AddDays(-30))
                        .Select(sm => sm.ItemId)
                        .Distinct()
                        .ToListAsync();

                    return await query
                        .Where(si => recentMovements.Contains(si.ItemId))
                        .ToListAsync();
            }
        }
    }
}
