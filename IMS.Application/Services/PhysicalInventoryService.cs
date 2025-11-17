using DocumentFormat.OpenXml.Wordprocessing;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApprovalLevel = IMS.Domain.Entities.ApprovalLevel;

namespace IMS.Application.Services
{
    public class PhysicalInventoryService : IPhysicalInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStockService _stockService;
        private readonly INotificationService _notificationService;
        private readonly IApprovalService _approvalService;
        private readonly ILogger<PhysicalInventoryService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PhysicalInventoryService(
            IUnitOfWork unitOfWork,
            IStockService stockService,
            INotificationService notificationService,
            IApprovalService approvalService,
            ILogger<PhysicalInventoryService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _stockService = stockService;
            _notificationService = notificationService;
            _approvalService = approvalService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PhysicalInventoryDto> SchedulePhysicalCountAsync(ScheduleCountDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Check for existing scheduled counts
                var existingCount = await _unitOfWork.PhysicalInventories
                    .FirstOrDefaultAsync(pi =>
                        pi.StoreId == dto.StoreId &&
                        pi.Status != PhysicalInventoryStatus.Posted &&
                        pi.Status != PhysicalInventoryStatus.Cancelled);

                if (existingCount != null)
                {
                    throw new InvalidOperationException($"An active count already exists: {existingCount.ReferenceNumber}");
                }

                // Freeze stock movements if immediate count
                if (dto.FreezeStock)
                {
                    await FreezeStockMovementsAsync(dto.StoreId);
                }

                // Create physical inventory record
                var physicalInventory = new PhysicalInventory
                {
                    ReferenceNumber = await GenerateReferenceNumberAsync(dto.StoreId),
                    StoreId = dto.StoreId,
                    CountDate = dto.CountDate,
                    CountType = dto.CountType,
                    Status = PhysicalInventoryStatus.Initiated,
                    InitiatedBy = dto.InitiatedBy,
                    InitiatedDate = DateTime.Now,
                    FiscalYear = GetFiscalYear(dto.CountDate),
                    IsStockFrozen = dto.FreezeStock,
                    StockFrozenAt = dto.FreezeStock ? DateTime.Now : null,

                    // Ansar & VDP hierarchy
                    BattalionId = dto.BattalionId,
                    RangeId = dto.RangeId,
                    ZilaId = dto.ZilaId,
                    UpazilaId = dto.UpazilaId,

                    CountTeam = dto.CountTeam,
                    SupervisorId = dto.SupervisorId,
                    Remarks = dto.Remarks,

                    Details = new List<PhysicalInventoryDetail>()
                };

                // Get current stock snapshot
                var currentStock = await _stockService.GetStoreStockAsync(dto.StoreId);

                // Create detail records for selected items or all items
                foreach (var stock in currentStock)
                {
                    if (dto.SelectedItemIds == null || dto.SelectedItemIds.Contains(stock.ItemId))
                    {
                        var item = await _unitOfWork.Items.GetByIdAsync(stock.ItemId);

                        physicalInventory.Details.Add(new PhysicalInventoryDetail
                        {
                            ItemId = stock.ItemId,
                            SystemQuantity = stock.CurrentQuantity,
                            PhysicalQuantity = 0, // To be filled during count
                            Variance = 0,
                            VarianceValue = 0,
                            UnitPrice = item.UnitPrice ?? 0,
                            Status = CountStatus.NotCounted,
                            CategoryId = item.CategoryId,
                            SubCategoryId = item.SubCategoryId,
                            BatchNo = stock.BatchNo,
                            Location = stock.Location,
                            LastCountDate = stock.LastCountDate
                        });
                    }
                }

                await _unitOfWork.PhysicalInventories.AddAsync(physicalInventory);
                await _unitOfWork.CompleteAsync();

                // Create count sheets
                await GenerateCountSheetsAsync(physicalInventory);

                // Send notifications to count team
                await NotifyCountTeamAsync(physicalInventory);

                await _unitOfWork.CommitTransactionAsync();

                return MapToDto(physicalInventory);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error scheduling physical count");
                throw;
            }
        }
        private async Task FreezeStockMovementsAsync(int storeId)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            if (store != null)
            {
                store.IsStockFrozen = true;
                store.StockFrozenAt = DateTime.Now;
                store.StockFrozenReason = "Physical Inventory Count";
                _unitOfWork.Stores.Update(store);  // Fixed: Use Update instead of UpdateAsync
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Stock frozen for store {storeId} for physical count");
            }
        }
        public async Task<bool> RecordPhysicalCountAsync(int inventoryId, CountRecordDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var inventory = await _unitOfWork.PhysicalInventories.Query()
                    .Include(pi => pi.Details)
                    .FirstOrDefaultAsync(pi => pi.Id == inventoryId);

                if (inventory == null)
                    throw new InvalidOperationException("Physical inventory not found");

                if (inventory.Status != PhysicalInventoryStatus.Initiated &&
                    inventory.Status != PhysicalInventoryStatus.InProgress)
                {
                    throw new InvalidOperationException("Count is not in progress");
                }

                // Update status to InProgress if first count
                if (inventory.Status == PhysicalInventoryStatus.Initiated)
                {
                    inventory.Status = PhysicalInventoryStatus.InProgress;
                    inventory.StartTime = DateTime.Now;
                }

                // Record count for each item
                foreach (var countItem in dto.CountedItems)
                {
                    var detail = inventory.Details.FirstOrDefault(d => d.ItemId == countItem.ItemId);
                    if (detail == null) continue;

                    // Record first count
                    if (detail.Status == CountStatus.NotCounted)
                    {
                        detail.PhysicalQuantity = countItem.CountedQuantity;
                        detail.FirstCountQuantity = countItem.CountedQuantity;
                        detail.FirstCountBy = dto.CountedBy;
                        detail.FirstCountTime = DateTime.Now;
                        detail.CountLocation = countItem.Location;
                        detail.CountRemarks = countItem.Remarks;
                        detail.Status = CountStatus.Counted;

                        // Calculate variance
                        detail.Variance = detail.PhysicalQuantity - detail.SystemQuantity;
                        detail.VarianceValue = detail.Variance * detail.UnitPrice;
                        detail.VariancePercentage = detail.SystemQuantity != 0
                            ? (detail.Variance / detail.SystemQuantity) * 100
                            : 0;

                        // Determine variance type
                        if (detail.Variance > 0)
                            detail.VarianceType = VarianceType.Overage;
                        else if (detail.Variance < 0)
                            detail.VarianceType = VarianceType.Shortage;
                        else
                            detail.VarianceType = VarianceType.None;
                    }

                    // Record recount if variance exceeds threshold
                    else if (detail.Status == CountStatus.Counted &&
                             Math.Abs(detail.VariancePercentage ?? 0) > 5) // 5% threshold
                    {
                        detail.RecountQuantity = countItem.CountedQuantity;
                        detail.RecountBy = dto.CountedBy;
                        detail.RecountTime = DateTime.Now;
                        detail.Status = CountStatus.Verified;

                        // Use recount as final quantity
                        detail.PhysicalQuantity = countItem.CountedQuantity;
                        detail.Variance = detail.PhysicalQuantity - detail.SystemQuantity;
                        detail.VarianceValue = detail.Variance * detail.UnitPrice;
                    }

                    // Record blind count verification
                    if (dto.IsBlindCount && !detail.BlindCountCompleted)
                    {
                        detail.BlindCountQuantity = countItem.CountedQuantity;
                        detail.BlindCountBy = dto.CountedBy;
                        detail.BlindCountTime = DateTime.Now;
                        detail.BlindCountCompleted = true;
                    }
                }

                // Check if all items are counted
                var allCounted = inventory.Details.All(d => d.Status != CountStatus.NotCounted);
                if (allCounted)
                {
                    inventory.Status = PhysicalInventoryStatus.CountCompleted;
                    inventory.CountEndTime = DateTime.Now;
                    inventory.CompletedBy = dto.CountedBy;
                    inventory.CompletedDate = DateTime.Now;

                    // Calculate totals
                    inventory.TotalSystemValue = inventory.Details.Sum(d => d.SystemQuantity * d.UnitPrice);
                    inventory.TotalPhysicalValue = inventory.Details.Sum(d => d.PhysicalQuantity * d.UnitPrice);
                    inventory.TotalVarianceValue = inventory.TotalPhysicalValue - inventory.TotalSystemValue;
                }

                await _unitOfWork.CompleteAsync();

                // Generate variance report if count completed
                if (allCounted)
                {
                    await GenerateVarianceReportAsync(inventory);
                }

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error recording physical count");
                throw;
            }
        }
        public async Task<VarianceAnalysisDto> AnalyzeVarianceAsync(int inventoryId)
        {
            try
            {
                var inventory = await _unitOfWork.PhysicalInventories.Query()
                    .Include(pi => pi.Details)
                    .ThenInclude(d => d.Item)
                        .ThenInclude(i => i.SubCategory)
                            .ThenInclude(sc => sc.Category)
                    .FirstOrDefaultAsync(pi => pi.Id == inventoryId);

                if (inventory == null)
                    throw new InvalidOperationException("Physical inventory not found");

                var analysis = new VarianceAnalysisDto
                {
                    InventoryId = inventoryId,
                    ReferenceNumber = inventory.ReferenceNumber,
                    CountDate = inventory.CountDate,
                    TotalItems = inventory.Details.Count,

                    // Fixed: Use correct property names
                    ItemsWithPositiveVariance = inventory.Details.Count(d => d.Variance > 0),
                    ItemsWithNegativeVariance = inventory.Details.Count(d => d.Variance < 0),
                    ItemsWithOverage = inventory.Details.Count(d => d.Variance > 0),
                    ItemsWithShortage = inventory.Details.Count(d => d.Variance < 0),

                    // Fixed: Handle nullable decimals using null coalescing operator
                    TotalSystemValue = inventory.TotalSystemValue ?? 0,
                    TotalPhysicalValue = inventory.TotalPhysicalValue ?? 0,
                    TotalVarianceValue = inventory.TotalVarianceValue ?? 0,
                    OverageValue = inventory.Details.Where(d => d.Variance > 0).Sum(d => d.VarianceValue ?? 0),
                    ShortageValue = Math.Abs(inventory.Details.Where(d => d.Variance < 0).Sum(d => d.VarianceValue ?? 0)),

                    // Fixed: Handle nullable decimal division
                    VariancePercentage = (inventory.TotalSystemValue ?? 0) != 0
                        ? ((inventory.TotalVarianceValue ?? 0) / (inventory.TotalSystemValue ?? 1)) * 100
                        : 0,

                    // Fixed: Navigate through SubCategory to Category
                    VarianceByCategory = inventory.Details
                        .GroupBy(d => d.Item.SubCategory?.Category?.Name ?? "Uncategorized")
                        .Select(g => new CategoryVarianceDto
                        {
                            CategoryName = g.Key,
                            ItemCount = g.Count(),
                            TotalVariance = g.Sum(d => d.VarianceValue ?? 0),
                            VariancePercentage = g.Sum(d => d.SystemQuantity * d.UnitPrice) != 0
                                ? (g.Sum(d => d.VarianceValue ?? 0) / g.Sum(d => d.SystemQuantity * d.UnitPrice)) * 100
                                : 0
                        })
                        .OrderByDescending(c => Math.Abs(c.TotalVariance))
                        .ToList(),

                    // Top variance items
                    TopVarianceItems = inventory.Details
                        .Where(d => d.Variance != 0)
                        .OrderByDescending(d => Math.Abs(d.VarianceValue ?? 0))
                        .Take(10)
                        .Select(d => new ItemVarianceDto
                        {
                            ItemId = d.ItemId,
                            ItemName = d.Item.Name,
                            ItemCode = d.Item.ItemCode,
                            SystemQuantity = d.SystemQuantity,
                            PhysicalQuantity = d.PhysicalQuantity,
                            Variance = d.Variance,
                            VarianceValue = d.VarianceValue ?? 0,
                            VariancePercentage = d.VariancePercentage ?? 0,
                            VarianceType = d.VarianceType
                        })
                        .ToList(),

                    RequiresApproval = Math.Abs(inventory.TotalVarianceValue ?? 0) > 10000,
                    RecommendedActions = GenerateRecommendations(inventory)
                };

                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing variance");
                throw;
            }
        }
        public async Task<bool> CreateStockAdjustmentAsync(int inventoryId, AdjustmentCreationDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var inventory = await _unitOfWork.PhysicalInventories.Query()
                    .Include(pi => pi.Details)
                    .FirstOrDefaultAsync(pi => pi.Id == inventoryId);

                if (inventory == null)
                    throw new InvalidOperationException("Physical inventory not found");

                if (inventory.Status != PhysicalInventoryStatus.CountCompleted &&
                    inventory.Status != PhysicalInventoryStatus.UnderReview)
                {
                    throw new InvalidOperationException("Count must be completed before adjustment");
                }

                // Create adjustment document
                var adjustment = new StockAdjustment
                {
                    AdjustmentNo = await GenerateAdjustmentNoAsync(),
                    PhysicalInventoryId = inventoryId,
                    StoreId = inventory.StoreId,
                    AdjustmentDate = DateTime.Now,
                    AdjustmentType = AdjustmentType.Found.ToString(),
                    Status = AdjustmentStatus.Pending.ToString(),
                    TotalValue = (int)(inventory.TotalVarianceValue ?? 0),
                    Reason = dto.Reason,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.Now,
                    Items = new List<StockAdjustmentItem>()
                };

                // Create adjustment items for variances
                foreach (var detail in inventory.Details.Where(d => d.Variance != 0))
                {
                    adjustment.Items.Add(new StockAdjustmentItem
                    {
                        ItemId = detail.ItemId,
                        SystemQuantity = detail.SystemQuantity,
                        PhysicalQuantity = detail.PhysicalQuantity,
                        AdjustmentQuantity = detail.Variance,
                        AdjustmentValue = detail.VarianceValue ?? 0,
                        Reason = dto.ItemReasons?.FirstOrDefault(ir => ir.ItemId == detail.ItemId)?.Reason
                                ?? "Physical count variance",
                        BatchNo = detail.BatchNo
                    });
                }

                await _unitOfWork.StockAdjustments.AddAsync(adjustment);

                // Update inventory status
                inventory.Status = PhysicalInventoryStatus.UnderReview;
                inventory.AdjustmentCreatedDate = DateTime.Now;
                inventory.AdjustmentNo = adjustment.AdjustmentNo;

                await _unitOfWork.CompleteAsync();

                // Create approval request if value exceeds threshold
                if (Math.Abs((decimal)adjustment.TotalValue) > 5000)
                {
                    await _approvalService.CreateApprovalRequestAsync(new ApprovalRequestDto
                    {
                        EntityType = "StockAdjustment",
                        EntityId = adjustment.Id,
                        RequestedBy = dto.CreatedBy,
                        RequestedDate = DateTime.Now,
                        Amount = Math.Abs((decimal)adjustment.TotalValue),
                        Description = $"Stock Adjustment from Physical Count: {inventory.ReferenceNumber}"
                    });
                }

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating stock adjustment");
                throw;
            }
        }
        public async Task<bool> ApproveAndPostAdjustmentAsync(int inventoryId, ApprovalDto approvalDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var inventory = await _unitOfWork.PhysicalInventories.Query()
                    .Include(pi => pi.Details)
                    .FirstOrDefaultAsync(pi => pi.Id == inventoryId);

                if (inventory == null)
                    throw new InvalidOperationException("Physical inventory not found");

                var adjustment = await _unitOfWork.StockAdjustments.Query()
                    .Include(sa => sa.Items)
                    .FirstOrDefaultAsync(sa => sa.PhysicalInventoryId == inventoryId);

                if (adjustment == null)
                    throw new InvalidOperationException("Stock adjustment not found");

                // Approve adjustment
                adjustment.Status = AdjustmentStatus.Approved.ToString();
                adjustment.ApprovedBy = approvalDto.ApprovedBy;
                adjustment.ApprovedDate = DateTime.Now;
                adjustment.ApprovalRemarks = approvalDto.Remarks;

                // Post stock adjustments
                foreach (var item in adjustment.Items)
                {
                    // Update stock quantity
                    await _stockService.AdjustStockAsync(
                        adjustment.StoreId.GetValueOrDefault(),
                        item.ItemId,
                        item.AdjustmentQuantity,
                        $"Physical Count Adjustment: {adjustment.AdjustmentNo}",
                        approvalDto.ApprovedBy
                    );

                    // Create stock movement record
                    var stockMovement = new StockMovement
                    {
                        ItemId = item.ItemId,
                        StoreId = adjustment.StoreId.GetValueOrDefault(),
                        MovementType = StockMovementType.PhysicalCount.ToString(),
                        Quantity = item.AdjustmentQuantity,
                        OldBalance = item.SystemQuantity,
                        NewBalance = item.PhysicalQuantity,
                        ReferenceType = "PhysicalCount",
                        ReferenceNo = inventory.ReferenceNumber,
                        MovementDate = DateTime.Now,
                        CreatedBy = approvalDto.ApprovedBy,
                        Remarks = item.Reason
                    };

                    await _unitOfWork.StockMovements.AddAsync(stockMovement);
                }

                // Update inventory status
                inventory.Status = PhysicalInventoryStatus.Posted;
                inventory.ApprovedBy = approvalDto.ApprovedBy;
                inventory.ApprovedDate = DateTime.Now;
                inventory.PostedDate = DateTime.Now;

                // Update last count date for items
                foreach (var detail in inventory.Details)
                {
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == inventory.StoreId && si.ItemId == detail.ItemId);

                    if (storeItem != null)
                    {
                        storeItem.LastCountDate = inventory.CountDate;
                        storeItem.LastCountQuantity = detail.PhysicalQuantity;
                        _unitOfWork.StoreItems.Update(storeItem);
                    }
                }

                // Unfreeze stock if it was frozen
                if (inventory.IsStockFrozen)
                {
                    await UnfreezeStockMovementsAsync(inventory.StoreId);
                }

                await _unitOfWork.CompleteAsync();

                // Send completion notifications
                await SendCompletionNotificationsAsync(inventory);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving and posting adjustment");
                throw;
            }
        }
        private async Task UnfreezeStockMovementsAsync(int storeId)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            if (store != null)
            {
                store.IsStockFrozen = false;
                store.StockUnfrozenAt = DateTime.Now;
                store.StockFrozenReason = null;
                _unitOfWork.Stores.Update(store);

                _logger.LogInformation($"Stock unfrozen for store {storeId} after physical count");
            }
        }
        private async Task<string> GenerateReferenceNumberAsync(int storeId)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            var date = DateTime.Now;
            var prefix = $"PI-{store?.Code ?? "XX"}-{date:yyyyMM}";

            var lastInventory = await _unitOfWork.PhysicalInventories.Query()
                .Where(pi => pi.ReferenceNumber.StartsWith(prefix))
                .OrderByDescending(pi => pi.ReferenceNumber)
                .FirstOrDefaultAsync();

            if (lastInventory == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastInventory.ReferenceNumber.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }
        private async Task<string> GenerateAdjustmentNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"ADJ-{date:yyyyMM}";

            var lastAdjustment = await _unitOfWork.StockAdjustments.Query()
                .Where(sa => sa.AdjustmentNo.StartsWith(prefix))
                .OrderByDescending(sa => sa.AdjustmentNo)
                .FirstOrDefaultAsync();

            if (lastAdjustment == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastAdjustment.AdjustmentNo.Split('-').Last());
            return $"{prefix}-{(lastNumber + 1):D4}";
        }
        private string GetFiscalYear(DateTime date)
        {
            // Bangladesh fiscal year: July 1 to June 30
            var year = date.Year;
            if (date.Month < 7)
                year--;

            return $"{year}-{(year + 1).ToString().Substring(2)}";
        }
        private async Task GenerateCountSheetsAsync(PhysicalInventory inventory)
        {
            // Generate count sheets for printing/mobile app
            // Implementation for count sheet generation
            await Task.CompletedTask;
        }
        private async Task NotifyCountTeamAsync(PhysicalInventory inventory)
        {
            // Send notifications to count team members
            if (!string.IsNullOrEmpty(inventory.CountTeam))
            {
                var teamMembers = inventory.CountTeam.Split(',');
                foreach (var member in teamMembers)
                {
                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        UserId = member.Trim(),
                        Title = "Physical Count Scheduled",
                        Message = $"You are assigned to physical count {inventory.ReferenceNumber} on {inventory.CountDate:dd/MM/yyyy}",
                        Type = "Assignment",
                        Priority = "High",
                        CreatedAt = DateTime.Now
                    });
                }
            }
        }
        private async Task GenerateVarianceReportAsync(PhysicalInventory inventory)
        {
            // Generate variance report
            // Implementation for report generation
            await Task.CompletedTask;
        }
        private List<string> GenerateRecommendations(PhysicalInventory inventory)
        {
            var recommendations = new List<string>();

            // High variance items
            var highVarianceItems = inventory.Details
                .Where(d => Math.Abs(d.VariancePercentage ?? 0) > 10)
                .ToList();

            if (highVarianceItems.Any())
            {
                recommendations.Add($"Investigate {highVarianceItems.Count} items with variance > 10%");
            }

            // Pattern detection
            var shortagePattern = inventory.Details.Count(d => d.Variance < 0) >
                                 inventory.Details.Count(d => d.Variance > 0) * 2;

            if (shortagePattern)
            {
                recommendations.Add("Pattern detected: Significant shortages - Review security measures");
            }

            // Category issues
            var problemCategories = inventory.Details
                .GroupBy(d => d.Item?.CategoryId)
                .Where(g => g.Count(d => d.Variance != 0) > g.Count() * 0.5)
                .ToList();

            foreach (var category in problemCategories)
            {
                recommendations.Add($"High variance in category - requires focused review");
            }

            return recommendations;
        }
        private async Task SendCompletionNotificationsAsync(PhysicalInventory inventory)
        {
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Physical Count Completed",
                Message = $"Physical count {inventory.ReferenceNumber} has been completed and posted",
                Type = "Completion",
                Priority = "Normal",
                CreatedAt = DateTime.Now
            });
        }
        private PhysicalInventoryDto MapToDto(PhysicalInventory inventory)
        {
            return new PhysicalInventoryDto
            {
                Id = inventory.Id,
                ReferenceNumber = inventory.ReferenceNumber,
                StoreId = inventory.StoreId,
                StoreName = inventory.Store?.Name,
                CountDate = inventory.CountDate,
                CountType = inventory.CountType,
                Status = inventory.Status,
                FiscalYear = inventory.FiscalYear,
                InitiatedBy = inventory.InitiatedBy,
                InitiatedDate = inventory.InitiatedDate,
                TotalVariance = inventory.TotalVariance ?? 0,
                TotalVarianceValue = inventory.TotalVarianceValue ?? 0,
                Details = inventory.Details?.Select(d => new PhysicalInventoryDetailDto
                {
                    ItemId = d.ItemId,
                    Status = d.Status
                }).ToList()
            };
        }
        public async Task<PhysicalInventoryDto> InitiatePhysicalInventoryAsync(PhysicalInventoryDto dto)
        {
            try
            {
                // Validate store hierarchy permissions
                var store = await _unitOfWork.Stores.GetByIdAsync(dto.StoreId);
                if (store == null)
                    throw new InvalidOperationException("Store not found");

                // Check for ongoing inventory in same hierarchy
                var ongoingInventory = await _unitOfWork.PhysicalInventories
                    .FirstOrDefaultAsync(pi =>
                        pi.StoreId == dto.StoreId &&
                        pi.Status != PhysicalInventoryStatus.Approved &&
                        pi.Status != PhysicalInventoryStatus.Rejected &&
                        pi.Status != PhysicalInventoryStatus.Cancelled);

                if (ongoingInventory != null)
                    throw new InvalidOperationException($"An ongoing physical inventory already exists for this store (Ref: {ongoingInventory.ReferenceNumber}, Status: {ongoingInventory.Status}). Please complete or cancel the existing count before starting a new one.|{ongoingInventory.Id}");

                // Generate reference number based on organization hierarchy
                dto.ReferenceNumber = await GenerateHierarchicalReferenceNumberAsync(store);

                var physicalInventory = new PhysicalInventory
                {
                    ReferenceNumber = dto.ReferenceNumber,
                    StoreId = dto.StoreId,

                    // Ansar & VDP specific fields
                    BattalionId = store.BattalionId,
                    RangeId = store.RangeId,
                    ZilaId = store.ZilaId,
                    UpazilaId = store.UpazilaId,

                    CountDate = dto.CountDate,
                    FiscalYear = GetFiscalYear(dto.CountDate),
                    Status = PhysicalInventoryStatus.Initiated,
                    InitiatedBy = dto.InitiatedBy,
                    InitiatedDate = DateTime.Now,
                    Remarks = dto.Remarks,
                    CountType = dto.CountType,

                    // Government audit fields
                    IsAuditRequired = dto.IsAuditRequired,
                    AuditOfficer = dto.AuditOfficer,

                    Details = new List<PhysicalInventoryDetail>()
                };

                // Get current stock based on store hierarchy
                var currentStock = await _stockService.GetStoreStockAsync(dto.StoreId);

                // Create detail records with category grouping (important for government reporting)
                foreach (var stock in currentStock)
                {
                    if (dto.SelectedItemIds == null || dto.SelectedItemIds.Contains(stock.ItemId))
                    {
                        var item = await _unitOfWork.Items.GetByIdAsync(stock.ItemId);
                        var detail = new PhysicalInventoryDetail
                        {
                            ItemId = stock.ItemId,
                            CategoryId = item.CategoryId,
                            SystemQuantity = (decimal)stock.Quantity,
                            PhysicalQuantity = 0,
                            Variance = 0,
                            Status = CountStatus.Pending,

                            // Track last issue/receive for audit
                            LastIssueDate = stock.LastIssueDate,
                            LastReceiveDate = stock.LastReceiveDate
                        };
                        physicalInventory.Details.Add(detail);
                    }
                }

                await _unitOfWork.PhysicalInventories.AddAsync(physicalInventory);
                await _unitOfWork.CompleteAsync();

                // Create approval workflow based on store level
                await CreateApprovalWorkflowAsync(physicalInventory);

                // Send notifications to relevant officers
                await NotifyRelevantOfficersAsync(physicalInventory, "initiated");

                return await GetPhysicalInventoryByIdAsync(physicalInventory.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating physical inventory");
                throw;
            }
        }
        private async Task CreateApprovalWorkflowAsync(PhysicalInventory inventory)
        {
            var store = await _unitOfWork.Stores
                .Query()
                .Include(s => s.StoreType)
                .FirstOrDefaultAsync(s => s.Id == inventory.StoreId);

            var approvalLevels = new List<ApprovalLevel>();

            // Check StoreType first (Priority: Central/Provision)
            if (store?.StoreType?.Code == "CENTRAL")
            {
                approvalLevels.Add(new ApprovalLevel
                {
                    Level = 1,
                    Role = "Storekeeper",
                    Description = "Store Keeper - Central"
                });
                approvalLevels.Add(new ApprovalLevel
                {
                    Level = 2,
                    Role = "AD_DD_Store",
                    Description = "AD/DD Store"
                });
                approvalLevels.Add(new ApprovalLevel
                {
                    Level = 3,
                    Role = "DDG_Admin",
                    Description = "DDG Admin"
                });
            }
            else if (store?.StoreType?.Code == "PROVISION")
            {
                approvalLevels.Add(new ApprovalLevel
                {
                    Level = 1,
                    Role = "Storekeeper",
                    Description = "Store Keeper - Provision"
                });
                approvalLevels.Add(new ApprovalLevel
                {
                    Level = 2,
                    Role = "DD_Provision",
                    Description = "DD Provision"
                });
            }
            else
            {
                // Fallback to StoreLevel-based hierarchy
                switch (store.Level)
                {
                    case StoreLevel.Upazila:
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 1,
                            Role = "UpazilaCommander",
                            Description = "Upazila Ansar & VDP Officer"
                        });
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 2,
                            Role = "ZilaCommander",
                            Description = "District Commandant"
                        });
                        break;

                    case StoreLevel.Zila:
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 1,
                            Role = "ZilaCommander",
                            Description = "District Commandant"
                        });
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 2,
                            Role = "RangeCommander",
                            Description = "Range DIG"
                        });
                        break;

                    case StoreLevel.Battalion:
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 1,
                            Role = "BattalionCommander",
                            Description = "Battalion Commanding Officer"
                        });
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 2,
                            Role = "RangeCommander",
                            Description = "Range DIG"
                        });
                        break;

                    case StoreLevel.Range:
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 1,
                            Role = "RangeCommander",
                            Description = "Range DIG"
                        });
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 2,
                            Role = "DirectorOperations",
                            Description = "Director (Operations)"
                        });
                        break;

                    case StoreLevel.HQ:
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 1,
                            Role = "DirectorAdmin",
                            Description = "Director (Admin & Finance)"
                        });
                        approvalLevels.Add(new ApprovalLevel
                        {
                            Level = 2,
                            Role = "DirectorGeneral",
                            Description = "Director General"
                        });
                        break;
                }
            }

            // Add audit level if required
            if (inventory.IsAuditRequired)
            {
                approvalLevels.Add(new ApprovalLevel
                {
                    Level = approvalLevels.Count + 1,
                    Role = "AuditOfficer",
                    Description = "Government Audit Officer"
                });
            }

            await _approvalService.CreateApprovalWorkflowAsync(
                "PhysicalInventory",
                inventory.Id,
                approvalLevels);
        }
        private async Task<string> GenerateHierarchicalReferenceNumberAsync(Store store)
        {
            var prefix = "PI";
            var locationCode = "";

            // Build location code based on hierarchy
            if (store.Level == StoreLevel.HQ)
            {
                locationCode = "HQ";
            }
            else if (store.RangeId.HasValue)
            {
                var range = await _unitOfWork.Ranges.GetByIdAsync(store.RangeId.Value);
                locationCode = range.Code;
            }
            else if (store.BattalionId.HasValue)
            {
                var battalion = await _unitOfWork.Battalions.GetByIdAsync(store.BattalionId.Value);
                locationCode = battalion.Code;
            }
            else if (store.ZilaId.HasValue)
            {
                var zila = await _unitOfWork.Zilas.GetByIdAsync(store.ZilaId.Value);
                locationCode = zila.Code;
            }
            else if (store.UpazilaId.HasValue)
            {
                var upazila = await _unitOfWork.Upazilas.GetByIdAsync(store.UpazilaId.Value);
                locationCode = upazila.Code;
            }

            var fiscalYear = GetFiscalYear(DateTime.Now);
            var date = DateTime.Now.ToString("yyyyMMdd");
            var count = await _unitOfWork.PhysicalInventories.CountAsync(pi =>
                pi.ReferenceNumber.StartsWith($"{prefix}-{locationCode}-{fiscalYear}"));

            return $"{prefix}-{locationCode}-{fiscalYear}-{(count + 1).ToString("D4")}";
        }
        public async Task<PhysicalInventory> GetPhysicalInventoryWithDetailsAsync(int id)
        {
            var query = _unitOfWork.PhysicalInventories.Query();

            return await query
                .Include(pi => pi.Details)
                    .ThenInclude(d => d.Item)
                        .ThenInclude(i => i.SubCategory)
                            .ThenInclude(sc => sc.Category)  // Fix: Use SubCategory.Category
                .Include(pi => pi.Store)
                .Include(pi => pi.Battalion)
                .Include(pi => pi.Range)
                .Include(pi => pi.Zila)
                .Include(pi => pi.Upazila)
                .FirstOrDefaultAsync(pi => pi.Id == id);
        }
        public async Task<PhysicalInventoryDto> CreatePhysicalInventoryAsync(PhysicalInventoryDto dto)
        {
            return await InitiatePhysicalInventoryAsync(dto);
        }
        public async Task<PhysicalInventoryDto> GetPhysicalInventoryByIdAsync(int id)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(id);

            if (inventory == null)
                return null;

            return MapToDto(inventory);
        }
        public async Task<IEnumerable<PhysicalInventoryDto>> GetAllPhysicalInventoriesAsync()
        {
            var inventories = await _unitOfWork.PhysicalInventories
                .Query()
                .Include(pi => pi.Store)
                .Include(pi => pi.Details)
                .ToListAsync();

            return inventories.Select(MapToDto);
        }
        public async Task<IEnumerable<PhysicalInventoryDto>> GetPhysicalInventoriesAsync(
            int? storeId, PhysicalInventoryStatus? status, string fiscalYear)
        {
            var query = _unitOfWork.PhysicalInventories.Query();

            // Apply includes first
            query = query
                .Include(pi => pi.Store)
                .Include(pi => pi.Details);

            // Then apply filters
            if (storeId.HasValue)
                query = query.Where(pi => pi.StoreId == storeId.Value);

            if (status.HasValue)
                query = query.Where(pi => pi.Status == status.Value);

            if (!string.IsNullOrEmpty(fiscalYear))
                query = query.Where(pi => pi.FiscalYear == fiscalYear);

            var inventories = await query.ToListAsync();
            return inventories.Select(MapToDto);
        }
        public async Task<PhysicalInventoryDto> StartCountingAsync(int inventoryId)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            inventory.Status = PhysicalInventoryStatus.InProgress;
            inventory.StartTime = DateTime.Now;

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();

            return MapToDto(inventory);
        }
        public async Task UpdateCountAsync(int inventoryId, int itemId, decimal quantity, string countedBy)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            var detail = inventory.Details.FirstOrDefault(d => d.ItemId == itemId);
            if (detail != null)
            {
                detail.PhysicalQuantity = quantity;
                detail.Variance = quantity - detail.SystemQuantity;
                detail.CountedBy = countedBy;
                detail.CountedDate = DateTime.Now;
                detail.Status = CountStatus.Counted;

                // Calculate variance value
                var item = await _unitOfWork.Items.GetByIdAsync(itemId);
                detail.VarianceValue = detail.Variance * item.UnitPrice;
            }

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();
        }
        public async Task UpdatePhysicalCountAsync(int inventoryId, List<PhysicalCountUpdateDto> counts)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            foreach (var count in counts)
            {
                var detail = inventory.Details.FirstOrDefault(d => d.ItemId == count.ItemId);
                if (detail != null)
                {
                    detail.PhysicalQuantity = count.PhysicalQuantity;
                    detail.Variance = count.PhysicalQuantity - detail.SystemQuantity;
                    detail.CountedBy = count.CountedBy;
                    detail.CountedDate = DateTime.Now;
                    detail.Remarks = count.Remarks;
                    detail.Status = CountStatus.Counted;

                    // Calculate variance value
                    var item = await _unitOfWork.Items.GetByIdAsync(count.ItemId);
                    detail.VarianceValue = detail.Variance * item.UnitPrice;
                }
            }

            // Update totals
            inventory.TotalSystemQuantity = inventory.Details.Sum(d => d.SystemQuantity);
            inventory.TotalPhysicalQuantity = inventory.Details.Sum(d => d.PhysicalQuantity);
            inventory.TotalVariance = inventory.Details.Sum(d => d.Variance);
            inventory.TotalVarianceValue = inventory.Details.Sum(d => d.VarianceValue ?? 0);

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();
        }
        public async Task CompleteCountingAsync(int inventoryId)
        {
            await CompleteCountingAsync(inventoryId, GetCurrentUserName());
        }
        public async Task CompleteCountingAsync(int inventoryId, string completedBy)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            inventory.Status = PhysicalInventoryStatus.Completed;
            inventory.CompletedBy = completedBy;
            inventory.CompletedDate = DateTime.Now;
            inventory.EndTime = DateTime.Now;

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<PhysicalInventoryDto> ReviewPhysicalInventoryAsync(
            int inventoryId, string reviewedBy, string reviewRemarks)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            inventory.Status = PhysicalInventoryStatus.UnderReview;
            inventory.ReviewedBy = reviewedBy;
            inventory.ReviewedDate = DateTime.Now;
            inventory.ReviewRemarks = reviewRemarks;

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();

            return MapToDto(inventory);
        }
        public async Task<PhysicalInventoryDto> ApprovePhysicalInventoryAsync(
            int inventoryId, string approvedBy, string approvalRemarks, bool autoAdjust = false)
        {
            try
            {
                var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

                if (inventory == null)
                    throw new InvalidOperationException("Physical inventory not found");

                // Check approval hierarchy
                var currentApprovalLevel = await _approvalService.GetCurrentApprovalLevelAsync(
                    "PhysicalInventory", inventoryId);

                var canApprove = await _approvalService.CanUserApproveAsync(
                    approvedBy, currentApprovalLevel);

                if (!canApprove)
                    throw new InvalidOperationException("User not authorized to approve at this level");

                // Record approval
                await _approvalService.ApproveAsync(
                    "PhysicalInventory", inventoryId, approvedBy, approvalRemarks);

                // Check if all approvals are complete
                var allApproved = await _approvalService.AreAllApprovalsCompleteAsync(
                    "PhysicalInventory", inventoryId);

                if (allApproved)
                {
                    inventory.Status = PhysicalInventoryStatus.Approved;
                    inventory.ApprovedBy = approvedBy;
                    inventory.ApprovedDate = DateTime.Now;
                    inventory.ApprovalRemarks = approvalRemarks;

                    // Generate government audit report
                    await GenerateAuditReportAsync(inventory);

                    // Auto-adjust stock if requested and authorized
                    if (autoAdjust && await CanAutoAdjustAsync(inventory))
                    {
                        await AdjustStockWithAuditTrailAsync(inventory);
                        inventory.AdjustmentStatus = AdjustmentStatus.Completed;
                        inventory.AdjustedDate = DateTime.Now;
                    }

                    // Notify higher authorities
                    await NotifyHigherAuthoritiesAsync(inventory);
                }
                else
                {
                    inventory.Status = PhysicalInventoryStatus.UnderReview;

                    // Notify next approver
                    await NotifyNextApproverAsync(inventory);
                }

                _unitOfWork.PhysicalInventories.Update(inventory);
                await _unitOfWork.CompleteAsync();

                return MapToDto(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving physical inventory");
                throw;
            }
        }
        public async Task<PhysicalInventoryDto> RejectPhysicalInventoryAsync(
            int inventoryId, string rejectedBy, string rejectionReason)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            inventory.Status = PhysicalInventoryStatus.Rejected;
            inventory.RejectedBy = rejectedBy;
            inventory.RejectedDate = DateTime.Now;
            inventory.RejectionReason = rejectionReason;

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();

            await NotifyRelevantOfficersAsync(inventory, "rejected");

            return MapToDto(inventory);
        }
        public async Task<VarianceAnalysisDto> GetVarianceAnalysisAsync(int inventoryId)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            var varianceItems = inventory.Details
                .Where(d => d.Variance != 0)
                .Select(d => new VarianceItemDto
                {
                    ItemId = d.ItemId,
                    ItemName = d.Item.Name,
                    SystemQuantity = d.SystemQuantity,
                    PhysicalQuantity = d.PhysicalQuantity,
                    Variance = d.Variance,
                    VarianceValue = d.VarianceValue ?? 0
                }).ToList();

            return new VarianceAnalysisDto
            {
                InventoryId = inventoryId,
                TotalSystemQuantity = inventory.TotalSystemQuantity ?? 0,
                TotalPhysicalQuantity = inventory.TotalPhysicalQuantity ?? 0,
                TotalVariance = inventory.TotalVariance ?? 0,
                TotalVarianceValue = inventory.TotalVarianceValue ?? 0,
                ItemsWithVariance = varianceItems  // Fix: Assign to correct property
            };
        }
        public async Task RecountItemsAsync(int inventoryId, List<int> itemIds, string requestedBy)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            foreach (var itemId in itemIds)
            {
                var detail = inventory.Details.FirstOrDefault(d => d.ItemId == itemId);
                if (detail != null)
                {
                    detail.RecountRequestedBy = requestedBy;
                    detail.RecountRequestedDate = DateTime.Now;
                    detail.Status = CountStatus.PendingRecount;
                }
            }

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();
        }
        public async Task RecountItemAsync(int inventoryId, int itemId, decimal quantity)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            var detail = inventory.Details.FirstOrDefault(d => d.ItemId == itemId);
            if (detail != null)
            {
                detail.PhysicalQuantity = quantity;
                detail.Variance = quantity - detail.SystemQuantity;
                detail.Status = CountStatus.Recounted;
                detail.CountedDate = DateTime.Now;

                // Recalculate variance value
                var item = await _unitOfWork.Items.GetByIdAsync(itemId);
                detail.VarianceValue = detail.Variance * item.UnitPrice;
            }

            // Update inventory totals
            inventory.TotalPhysicalQuantity = inventory.Details.Sum(d => d.PhysicalQuantity);
            inventory.TotalVariance = inventory.Details.Sum(d => d.Variance);
            inventory.TotalVarianceValue = inventory.Details.Sum(d => d.VarianceValue ?? 0);

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();
        }
        public async Task VerifyCountAsync(int inventoryId, string verifiedBy)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            inventory.VerifiedBy = verifiedBy;
            inventory.Status = PhysicalInventoryStatus.Verified;

            foreach (var detail in inventory.Details)
            {
                detail.VerifiedBy = verifiedBy;
                detail.VerifiedDate = DateTime.Now;
                detail.Status = CountStatus.Verified;
            }

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();
        }
        //public async Task CreateReconciliationFromCountAsync(int inventoryId)
        //{
        //    var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

        //    if (inventory == null)
        //        throw new InvalidOperationException("Physical inventory not found");

        //    var reconciliation = new StockReconciliation
        //    {
        //        ReconciliationNo = $"SR-{inventory.ReferenceNumber}",
        //        ReconciliationDate = DateTime.Now,
        //        StoreId = inventory.StoreId,
        //        PhysicalInventoryId = inventoryId,
        //        Status = ReconciliationStatus.Pending.ToString(),  // Use enum ToString()
        //        TotalVariance = inventory.TotalVariance ?? 0,
        //        TotalVarianceValue = inventory.TotalVarianceValue ?? 0,
        //        CreatedBy = inventory.ApprovedBy,
        //        IsActive = true,
        //        CreatedAt = DateTime.Now
        //    };

        //    await _unitOfWork.StockReconciliations.AddAsync(reconciliation);
        //    await _unitOfWork.CompleteAsync();
        //}
        //public async Task<StockReconciliationDto> GetReconciliationByIdAsync(int reconciliationId)
        //{
        //    var reconciliation = await _unitOfWork.StockReconciliations
        //        .GetByIdAsync(reconciliationId);

        //    if (reconciliation == null)
        //        return null;

        //    return new StockReconciliationDto
        //    {
        //        Id = reconciliation.Id,
        //        ReconciliationNo = reconciliation.ReconciliationNo,
        //        ReconciliationDate = reconciliation.ReconciliationDate,
        //        StoreId = reconciliation.StoreId,
        //        Status = reconciliation.Status,
        //        TotalVariance = reconciliation.TotalVariance,
        //        TotalVarianceValue = reconciliation.TotalVarianceValue
        //    };
        //}
        public async Task<byte[]> ExportPhysicalInventoryAsync(int inventoryId, string format = "excel")
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            if (format.ToLower() == "excel")
            {
                return await GenerateExcelReport(inventory);
            }
            else
            {
                return await GeneratePdfReport(inventory);
            }
        }
        public async Task<IEnumerable<PhysicalInventoryDto>> GetScheduledCountsAsync(int storeId)
        {
            var scheduledCounts = await _unitOfWork.PhysicalInventories
                .Query()
                .Include(pi => pi.Store)
                .Where(pi => pi.StoreId == storeId &&
                       pi.Status == PhysicalInventoryStatus.Scheduled)
                .ToListAsync();

            return scheduledCounts.Select(MapToDto);
        }
        public async Task<bool> CanUserInitiateCountAsync(string userId, int storeId)
        {
            // Check if user has permission for the store
            var userStore = await _unitOfWork.UserStores
                .Query()
                .FirstOrDefaultAsync(us => us.UserId == userId && us.StoreId == storeId);

            if (userStore == null) return false;

            // Check if there's no ongoing inventory
            var ongoingInventory = await _unitOfWork.PhysicalInventories
                .Query()
                .FirstOrDefaultAsync(pi => pi.StoreId == storeId &&
                    (pi.Status == PhysicalInventoryStatus.Initiated ||
                     pi.Status == PhysicalInventoryStatus.InProgress));

            return ongoingInventory == null;
        }
        public async Task<PhysicalInventoryDto> SubmitForApprovalAsync(int inventoryId, string verifiedBy)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            inventory.VerifiedBy = verifiedBy;
            inventory.Status = PhysicalInventoryStatus.Completed;
            inventory.CompletedDate = DateTime.Now;

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();

            // Start approval workflow
            await _approvalService.InitiateApprovalAsync("PhysicalInventory", inventoryId);

            return MapToDto(inventory);
        }
        public async Task<IEnumerable<ItemDto>> GetStoreItemsAsync(int storeId)
        {
            var storeStock = await _unitOfWork.StoreStocks
                .Query()
                .Include(ss => ss.Item)
                    .ThenInclude(i => i.SubCategory)
                        .ThenInclude(sc => sc.Category)  // Fix: Use proper navigation
                .Where(ss => ss.StoreId == storeId && ss.Item != null)  // ✅ Ensure Item is not null
                .ToListAsync();

            return storeStock
                .Where(ss => ss.Item != null)  // ✅ Double check for null items
                .Select(ss => new ItemDto
                {
                    Id = ss.ItemId,
                    ItemCode = ss.Item.ItemCode ?? ss.Item.Code ?? "",  // ✅ Handle both property names
                    Code = ss.Item.ItemCode ?? ss.Item.Code ?? "",
                    Name = ss.Item.Name ?? "Unknown Item",
                    CategoryName = ss.Item.SubCategory?.Category?.Name ?? "Uncategorized"
                })
                .ToList();
        }
        public async Task<string> GetCurrentFiscalYearAsync()
        {
            var currentDate = DateTime.Now;
            return GetFiscalYear(currentDate);
        }
        public async Task<IEnumerable<string>> GetAvailableFiscalYearsAsync()
        {
            var fiscalYears = await _unitOfWork.PhysicalInventories
                .Query()
                .Select(pi => pi.FiscalYear)
                .Distinct()
                .OrderByDescending(fy => fy)
                .ToListAsync();

            return fiscalYears;
        }
        public async Task<PhysicalInventoryDto> CancelPhysicalInventoryAsync(
            int inventoryId, string cancelledBy, string cancellationReason)
        {
            var inventory = await GetPhysicalInventoryWithDetailsAsync(inventoryId);

            if (inventory == null)
                throw new InvalidOperationException("Physical inventory not found");

            inventory.Status = PhysicalInventoryStatus.Cancelled;
            inventory.CancelledBy = cancelledBy;
            inventory.CancelledDate = DateTime.Now;
            inventory.CancellationReason = cancellationReason;

            _unitOfWork.PhysicalInventories.Update(inventory);
            await _unitOfWork.CompleteAsync();

            await NotifyRelevantOfficersAsync(inventory, "cancelled");

            return MapToDto(inventory);
        }
        public async Task<IEnumerable<PhysicalInventoryHistoryDto>> GetInventoryHistoryAsync(
            int? storeId = null, int? itemId = null)
        {
            var query = _unitOfWork.PhysicalInventories
                .Query()
                .Include(pi => pi.Store)
                .Include(pi => pi.Details)
                .AsQueryable();

            if (storeId.HasValue)
                query = query.Where(pi => pi.StoreId == storeId.Value);

            if (itemId.HasValue)
                query = query.Where(pi => pi.Details.Any(d => d.ItemId == itemId.Value));

            var inventories = await query
                .OrderByDescending(pi => pi.CountDate)
                .Take(50)
                .ToListAsync();

            return inventories.Select(pi => new PhysicalInventoryHistoryDto
            {
                Id = pi.Id,
                ReferenceNumber = pi.ReferenceNumber,
                CountDate = pi.CountDate,
                StoreName = pi.Store?.Name,
                Status = pi.Status,
                TotalVariance = pi.TotalVariance ?? 0,
                TotalVarianceValue = pi.TotalVarianceValue ?? 0
            });
        }
        private async Task GenerateAuditReportAsync(PhysicalInventory inventory)
        {
            var report = new AuditReport
            {
                ReferenceNumber = $"AR-{inventory.ReferenceNumber}",
                InventoryId = inventory.Id,
                GeneratedDate = DateTime.Now,
                FiscalYear = inventory.FiscalYear,
                EntityType = "PhysicalInventory",
                EntityId = inventory.Id,
                AuditType = "Physical Count",
                AuditDate = DateTime.Now,
                AuditorName = inventory.AuditOfficer ?? inventory.ApprovedBy,
                StoreLevel = inventory.Store?.Level.ToString(),
                BattalionName = inventory.Battalion?.Name,
                RangeName = inventory.Range?.Name,
                ZilaName = inventory.Zila?.Name,
                UpazilaName = inventory.Upazila?.Name,
                TotalSystemValue = inventory.Details.Sum(d => d.SystemQuantity * (d.Item.UnitPrice ?? 0)),
                TotalPhysicalValue = inventory.Details.Sum(d => d.PhysicalQuantity * (d.Item.UnitPrice ?? 0)),
                TotalVarianceValue = inventory.TotalVarianceValue ?? 0,
                ComplianceStatus = await CheckComplianceStatusAsync(inventory),
                AuditFindingsJson = JsonConvert.SerializeObject(await GenerateAuditFindingsAsync(inventory)),
                Findings = string.Join("; ", await GenerateAuditFindingsAsync(inventory)),
                Recommendations = GenerateRecommendations(inventory).ToString(),
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.AuditReports.AddAsync(report);
            await _unitOfWork.CompleteAsync();
        }
        private async Task AdjustStockWithAuditTrailAsync(PhysicalInventory inventory)
        {
            foreach (var detail in inventory.Details.Where(d => d.Variance != 0))
            {
                var adjustment = new StockAdjustment
                {
                    StoreId = inventory.StoreId,
                    ItemId = detail.ItemId,
                    AdjustmentType = detail.Variance > 0
                        ? AdjustmentType.PositiveAdjustment.ToString()
                        : AdjustmentType.NegativeAdjustment.ToString(),
                    Quantity = Math.Abs(detail.Variance),
                    Reason = $"Physical Inventory Adjustment - {inventory.ReferenceNumber}",
                    ReferenceNumber = inventory.ReferenceNumber,
                    AdjustedBy = inventory.ApprovedBy,
                    AdjustedDate = DateTime.Now,
                    ApprovalReference = inventory.ApprovalReference,
                    FiscalYear = inventory.FiscalYear,
                    AdjustmentNo = $"ADJ-{inventory.ReferenceNumber}-{detail.ItemId}",
                    AdjustmentDate = DateTime.Now,
                    Status = "Approved",
                    ApprovedBy = inventory.ApprovedBy,
                    ApprovedDate = DateTime.Now,

                    // Store audit trail as JSON
                    AuditTrailJson = JsonConvert.SerializeObject(new
                    {
                        EntityName = "Stock",
                        EntityId = detail.ItemId.ToString(),
                        Action = "PhysicalInventoryAdjustment",
                        OldValue = detail.SystemQuantity.ToString(),
                        NewValue = detail.PhysicalQuantity.ToString(),
                        ChangedBy = inventory.ApprovedBy,
                        ChangedDate = DateTime.Now,
                        IPAddress = GetUserIPAddress(),
                        Remarks = $"Adjustment from PI: {inventory.ReferenceNumber}"
                    })
                };

                await _stockService.AdjustStockAsync(adjustment);
            }
        }
        private async Task NotifyHigherAuthoritiesAsync(PhysicalInventory inventory)
        {
            var notifications = new List<NotificationDto>();

            // Notify DG if variance exceeds threshold
            if (Math.Abs(inventory.TotalVarianceValue ?? 0) > 100000) // 1 Lakh BDT
            {
                notifications.Add(new NotificationDto
                {
                    UserId = "DG_USER_ID", // Get from configuration
                    Title = "High Value Variance in Physical Inventory",
                    Message = $"Physical Inventory {inventory.ReferenceNumber} has variance of {inventory.TotalVarianceValue:N2} BDT",
                    Type = NotificationType.Critical.ToString(),
                    Priority = Priority.High.ToString()
                });
            }

            foreach (var notification in notifications)
            {
                await _notificationService.SendNotificationAsync(notification);
            }
        }
        private async Task NotifyNextApproverAsync(PhysicalInventory inventory)
        {
            var nextApprover = await _approvalService.GetNextApproverAsync(
                "PhysicalInventory", inventory.Id);

            if (!string.IsNullOrEmpty(nextApprover))  // Fix: nextApprover is a string (userId)
            {
                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = nextApprover,  // Fix: Use the string directly
                    Title = "Physical Inventory Pending Approval",
                    Message = $"Please review Physical Inventory {inventory.ReferenceNumber}",
                    Type = NotificationType.Approval.ToString(),
                    Priority = Priority.High.ToString()
                });
            }
        }
        private async Task NotifyRelevantOfficersAsync(PhysicalInventory inventory, string action)
        {
            var notifications = new List<NotificationDto>();
            var store = await _unitOfWork.Stores.GetByIdAsync(inventory.StoreId);

            // Fix: Use proper Identity role checking
            if (store.BattalionId.HasValue)
            {
                var battalionOfficers = await _unitOfWork.Users
                    .Query()
                    .Include(u => u.UserRoles)
                    .Where(u => u.BattalionId == store.BattalionId)
                    .ToListAsync();

                // Filter officers using UserManager or role claims
                var officers = battalionOfficers.Where(u =>
                    u.UserRoles.Any(ur => ur.RoleId.Contains("Officer")));

                foreach (var officer in officers)
                {
                    notifications.Add(new NotificationDto
                    {
                        UserId = officer.Id,
                        Title = $"Physical Inventory {action}",
                        Message = $"Physical Inventory {inventory.ReferenceNumber} has been {action} for {store.Name}",
                        Type = NotificationType.PhysicalInventory.ToString(),
                        Priority = Priority.High.ToString()
                    });
                }
            }

            foreach (var notification in notifications)
            {
                await _notificationService.SendNotificationAsync(notification);
            }
        }
        private async Task<bool> CanAutoAdjustAsync(PhysicalInventory inventory)
        {
            // Check if variance is within acceptable limits for auto-adjustment
            var variancePercentage = (inventory.TotalVariance / inventory.TotalSystemQuantity) * 100;
            var maxAllowedVariance = await GetMaxAllowedVariancePercentageAsync(inventory.Store?.Level);

            return Math.Abs((decimal)variancePercentage) <= maxAllowedVariance;
        }
        private async Task<decimal> GetMaxAllowedVariancePercentageAsync(StoreLevel? level)
        {
            // Different variance thresholds for different levels
            return level switch
            {
                StoreLevel.HQ => 1.0m,        // 1% for HQ
                StoreLevel.Range => 2.0m,      // 2% for Range
                StoreLevel.Battalion => 3.0m,   // 3% for Battalion
                StoreLevel.Zila => 4.0m,        // 4% for Zila
                StoreLevel.Upazila => 5.0m,     // 5% for Upazila
                _ => 3.0m                       // Default 3%
            };
        }
        private async Task<string> CheckComplianceStatusAsync(PhysicalInventory inventory)
        {
            // Check compliance based on Ansar & VDP rules
            var variancePercentage = (inventory.TotalVariance / inventory.TotalSystemQuantity) * 100;

            if (Math.Abs((decimal)variancePercentage) < 2)
                return "Compliant";
            else if (Math.Abs((decimal)variancePercentage) < 5)
                return "Minor Non-Compliance";
            else
                return "Major Non-Compliance";
        }
        private async Task<List<string>> GenerateAuditFindingsAsync(PhysicalInventory inventory)
        {
            var findings = new List<string>();

            if (inventory.TotalVariance > 0)
                findings.Add($"Excess inventory found: {inventory.TotalVariance} units");
            else if (inventory.TotalVariance < 0)
                findings.Add($"Shortage identified: {Math.Abs((decimal)inventory.TotalVariance)} units");

            var highValueItems = inventory.Details
                .Where(d => Math.Abs(d.VarianceValue ?? 0) > 10000)
                .ToList();

            if (highValueItems.Any())
            {
                findings.Add($"{highValueItems.Count} items with high value variance (>10,000 BDT)");
            }

            return findings;
        }
        private string GetUserIPAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }
            return "System";
        }
        private string GetCurrentUserName()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.User?.Identity?.Name ?? "System";
        }

        public async Task<List<PhysicalInventoryDto>> GetAllCountsAsync()
        {
            var inventories = await _unitOfWork.PhysicalInventories
                .Query()
                .Include(pi => pi.Store)
                .ToListAsync();

            return inventories.Select(MapToDto).ToList();
        }

        public async Task<List<PhysicalInventoryDto>> GetPendingCountsAsync()
        {
            var inventories = await _unitOfWork.PhysicalInventories
                .Query()
                .Include(pi => pi.Store)
                .Where(pi => pi.Status == PhysicalInventoryStatus.Initiated ||
                             pi.Status == PhysicalInventoryStatus.InProgress)
                .ToListAsync();

            return inventories.Select(MapToDto).ToList();
        }
        private async Task<byte[]> GenerateExcelReport(PhysicalInventory inventory)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Physical Inventory");

            // Add headers
            worksheet.Cells[1, 1].Value = "Item Code";
            worksheet.Cells[1, 2].Value = "Item Name";
            worksheet.Cells[1, 3].Value = "System Qty";
            worksheet.Cells[1, 4].Value = "Physical Qty";
            worksheet.Cells[1, 5].Value = "Variance";
            worksheet.Cells[1, 6].Value = "Variance Value (BDT)";

            // Add data
            int row = 2;
            foreach (var detail in inventory.Details)
            {
                worksheet.Cells[row, 1].Value = detail.Item.Code;
                worksheet.Cells[row, 2].Value = detail.Item.Name;
                worksheet.Cells[row, 3].Value = detail.SystemQuantity;
                worksheet.Cells[row, 4].Value = detail.PhysicalQuantity;
                worksheet.Cells[row, 5].Value = detail.Variance;
                worksheet.Cells[row, 6].Value = detail.VarianceValue ?? 0;
                row++;
            }

            // Format
            worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;
            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }
        private async Task<byte[]> GeneratePdfReport(PhysicalInventory inventory)
        {
            // Implement PDF generation using a library like iTextSharp or QuestPDF
            // This is a placeholder
            return new byte[0];
        }

        public async Task<IEnumerable<PhysicalInventoryDto>> GetCompletedInventoriesAsync()
        {
            var inventories = await _unitOfWork.PhysicalInventories.Query()
                .Include(pi => pi.Store)
                .Where(pi => pi.Status == PhysicalInventoryStatus.Completed ||
                             pi.Status == PhysicalInventoryStatus.Posted)
                .OrderByDescending(pi => pi.CountDate)
                .ToListAsync();

            return inventories.Select(pi => new PhysicalInventoryDto
            {
                Id = pi.Id,
                ReferenceNumber = pi.ReferenceNumber,
                CountDate = pi.CountDate,
                StoreId = pi.StoreId,
                StoreName = pi.Store?.Name,
                Status = pi.Status,
                TotalVariance = pi.TotalVariance ?? 0,
                TotalVarianceValue = pi.TotalVarianceValue ?? 0
            });
        }

        public async Task<PhysicalInventoryDto> SchedulePhysicalInventoryAsync(PhysicalInventoryDto dto)
        {
            // Schedule is same as initiate but with Scheduled status
            dto.Status = PhysicalInventoryStatus.Scheduled;
            return await InitiatePhysicalInventoryAsync(dto);
        }

        public async Task<byte[]> ExportCountHistoryCsvAsync(int? storeId = null, PhysicalInventoryStatus? status = null, string fiscalYear = null)
        {
            var query = _unitOfWork.PhysicalInventories.Query()
                .Include(pi => pi.Store)
                .Include(pi => pi.Details)
                .AsQueryable();

            // Apply filters
            if (storeId.HasValue)
                query = query.Where(pi => pi.StoreId == storeId.Value);

            if (status.HasValue)
                query = query.Where(pi => pi.Status == status.Value);

            if (!string.IsNullOrEmpty(fiscalYear))
                query = query.Where(pi => pi.FiscalYear == fiscalYear);

            var inventories = await query
                .OrderByDescending(pi => pi.CountDate)
                .ToListAsync();

            // Build CSV
            var csv = new System.Text.StringBuilder();

            // Add UTF-8 BOM for Excel compatibility
            var bomBytes = System.Text.Encoding.UTF8.GetPreamble();

            // CSV Headers
            csv.AppendLine("#,Reference No,Store,Count Date,Type,Status,Variance,Progress %,Initiated By,Fiscal Year");

            // CSV Data
            int serial = 1;
            foreach (var inventory in inventories)
            {
                var progress = 0;
                if (inventory.Details != null && inventory.Details.Any())
                {
                    var countedItems = inventory.Details.Count(d => d.Status == CountStatus.Counted || d.Status == CountStatus.Verified);
                    progress = (countedItems * 100) / inventory.Details.Count;
                }

                csv.AppendLine($"{serial}," +
                    $"{EscapeCsv(inventory.ReferenceNumber)}," +
                    $"{EscapeCsv(inventory.Store?.Name)}," +
                    $"{inventory.CountDate:dd-MMM-yyyy}," +
                    $"{EscapeCsv(inventory.CountType.ToString())}," +
                    $"{EscapeCsv(inventory.Status.ToString())}," +
                    $"{inventory.TotalVariance ?? 0}," +
                    $"{progress}," +
                    $"{EscapeCsv(inventory.InitiatedBy)}," +
                    $"{EscapeCsv(inventory.FiscalYear)}");

                serial++;
            }

            // Combine BOM + CSV content
            var csvBytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var result = new byte[bomBytes.Length + csvBytes.Length];
            System.Buffer.BlockCopy(bomBytes, 0, result, 0, bomBytes.Length);
            System.Buffer.BlockCopy(csvBytes, 0, result, bomBytes.Length, csvBytes.Length);

            return result;
        }

        public async Task<byte[]> ExportCountHistoryPdfAsync(int? storeId = null, PhysicalInventoryStatus? status = null, string fiscalYear = null)
        {
            var query = _unitOfWork.PhysicalInventories.Query()
                .Include(pi => pi.Store)
                .Include(pi => pi.Details)
                .AsQueryable();

            // Apply filters
            if (storeId.HasValue)
                query = query.Where(pi => pi.StoreId == storeId.Value);

            if (status.HasValue)
                query = query.Where(pi => pi.Status == status.Value);

            if (!string.IsNullOrEmpty(fiscalYear))
                query = query.Where(pi => pi.FiscalYear == fiscalYear);

            var inventories = await query
                .OrderByDescending(pi => pi.CountDate)
                .ToListAsync();

            using (var memoryStream = new System.IO.MemoryStream())
            {
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate());
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add Header
                var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
                var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

                var titleColor = new iTextSharp.text.BaseColor(0, 123, 255); // Primary blue color

                // Title
                var title = new iTextSharp.text.Paragraph("Bangladesh Ansar & VDP\n", titleFont);
                title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(title);

                var subtitle = new iTextSharp.text.Paragraph("Physical Count History Report\n\n", headerFont);
                subtitle.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(subtitle);

                // Report Info
                var reportInfo = new iTextSharp.text.Paragraph();
                reportInfo.Add(new iTextSharp.text.Chunk($"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}\n", normalFont));
                if (storeId.HasValue)
                {
                    var storeName = inventories.FirstOrDefault()?.Store?.Name;
                    reportInfo.Add(new iTextSharp.text.Chunk($"Store: {storeName}\n", normalFont));
                }
                if (status.HasValue)
                    reportInfo.Add(new iTextSharp.text.Chunk($"Status: {status.Value}\n", normalFont));
                if (!string.IsNullOrEmpty(fiscalYear))
                    reportInfo.Add(new iTextSharp.text.Chunk($"Fiscal Year: {fiscalYear}\n", normalFont));
                reportInfo.Add(new iTextSharp.text.Chunk($"Total Records: {inventories.Count}\n\n", normalFont));
                document.Add(reportInfo);

                // Create table
                var table = new iTextSharp.text.pdf.PdfPTable(9) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 5f, 15f, 15f, 12f, 10f, 10f, 10f, 10f, 13f });

                // Table headers with blue background
                var headerBgColor = titleColor;
                var headerTextColor = new iTextSharp.text.BaseColor(255, 255, 255); // White color

                string[] headers = { "#", "Reference No", "Store", "Count Date", "Type", "Status", "Variance", "Progress %", "Fiscal Year" };
                foreach (var header in headers)
                {
                    var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                    cell.BackgroundColor = headerBgColor;
                    cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    var phrase = new iTextSharp.text.Phrase(header, iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 9, headerTextColor));
                    cell.Phrase = phrase;
                    table.AddCell(cell);
                }

                // Table data with alternating row colors
                var lightBlue = new iTextSharp.text.BaseColor(230, 240, 255);
                var white = new iTextSharp.text.BaseColor(255, 255, 255); // White color
                int serial = 1;

                foreach (var inventory in inventories)
                {
                    var rowColor = serial % 2 == 0 ? lightBlue : white;

                    var progress = 0;
                    if (inventory.Details != null && inventory.Details.Any())
                    {
                        var countedItems = inventory.Details.Count(d => d.Status == CountStatus.Counted || d.Status == CountStatus.Verified);
                        progress = (countedItems * 100) / inventory.Details.Count;
                    }

                    // Serial
                    var cellSerial = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(serial.ToString(), normalFont));
                    cellSerial.BackgroundColor = rowColor;
                    cellSerial.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cellSerial.Padding = 5;
                    table.AddCell(cellSerial);

                    // Reference No
                    var cellRef = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(inventory.ReferenceNumber ?? "", normalFont));
                    cellRef.BackgroundColor = rowColor;
                    cellRef.Padding = 5;
                    table.AddCell(cellRef);

                    // Store
                    var cellStore = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(inventory.Store?.Name ?? "", normalFont));
                    cellStore.BackgroundColor = rowColor;
                    cellStore.Padding = 5;
                    table.AddCell(cellStore);

                    // Count Date
                    var cellDate = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(inventory.CountDate.ToString("dd-MMM-yyyy"), normalFont));
                    cellDate.BackgroundColor = rowColor;
                    cellDate.Padding = 5;
                    table.AddCell(cellDate);

                    // Type
                    var cellType = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(inventory.CountType.ToString(), normalFont));
                    cellType.BackgroundColor = rowColor;
                    cellType.Padding = 5;
                    table.AddCell(cellType);

                    // Status
                    var cellStatus = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(inventory.Status.ToString(), normalFont));
                    cellStatus.BackgroundColor = rowColor;
                    cellStatus.Padding = 5;
                    table.AddCell(cellStatus);

                    // Variance
                    var cellVariance = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase((inventory.TotalVariance ?? 0).ToString("+#;-#;0"), normalFont));
                    cellVariance.BackgroundColor = rowColor;
                    cellVariance.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
                    cellVariance.Padding = 5;
                    table.AddCell(cellVariance);

                    // Progress
                    var cellProgress = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"{progress}%", normalFont));
                    cellProgress.BackgroundColor = rowColor;
                    cellProgress.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cellProgress.Padding = 5;
                    table.AddCell(cellProgress);

                    // Fiscal Year
                    var cellFiscal = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(inventory.FiscalYear ?? "", normalFont));
                    cellFiscal.BackgroundColor = rowColor;
                    cellFiscal.Padding = 5;
                    table.AddCell(cellFiscal);

                    serial++;
                }

                document.Add(table);

                // Footer
                var footer = new iTextSharp.text.Paragraph($"\n\nGenerated by IMS - {DateTime.Now:dd-MMM-yyyy HH:mm}",
                    iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_OBLIQUE, 8));
                footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();
                writer.Close();

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportCountHistoryExcelAsync(int? storeId = null, PhysicalInventoryStatus? status = null, string fiscalYear = null)
        {
            var query = _unitOfWork.PhysicalInventories.Query()
                .Include(pi => pi.Store)
                .Include(pi => pi.Details)
                .AsQueryable();

            // Apply filters
            if (storeId.HasValue)
                query = query.Where(pi => pi.StoreId == storeId.Value);

            if (status.HasValue)
                query = query.Where(pi => pi.Status == status.Value);

            if (!string.IsNullOrEmpty(fiscalYear))
                query = query.Where(pi => pi.FiscalYear == fiscalYear);

            var inventories = await query
                .OrderByDescending(pi => pi.CountDate)
                .ToListAsync();

            // Generate Excel using ClosedXML
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Physical Count History");

            // Title
            worksheet.Cell(1, 1).Value = "Bangladesh Ansar & VDP";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, 9).Merge();
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

            // Subtitle
            worksheet.Cell(2, 1).Value = "Physical Count History Report";
            worksheet.Cell(2, 1).Style.Font.Bold = true;
            worksheet.Cell(2, 1).Style.Font.FontSize = 12;
            worksheet.Range(2, 1, 2, 9).Merge();
            worksheet.Cell(2, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

            // Report Info
            int currentRow = 4;
            worksheet.Cell(currentRow, 1).Value = $"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}";
            currentRow++;

            if (storeId.HasValue)
            {
                var storeName = inventories.FirstOrDefault()?.Store?.Name;
                worksheet.Cell(currentRow, 1).Value = $"Store: {storeName}";
                currentRow++;
            }

            if (status.HasValue)
            {
                worksheet.Cell(currentRow, 1).Value = $"Status: {status.Value}";
                currentRow++;
            }

            if (!string.IsNullOrEmpty(fiscalYear))
            {
                worksheet.Cell(currentRow, 1).Value = $"Fiscal Year: {fiscalYear}";
                currentRow++;
            }

            worksheet.Cell(currentRow, 1).Value = $"Total Records: {inventories.Count}";
            currentRow += 2;

            // Headers
            var headerRow = currentRow;
            worksheet.Cell(headerRow, 1).Value = "#";
            worksheet.Cell(headerRow, 2).Value = "Reference No";
            worksheet.Cell(headerRow, 3).Value = "Store";
            worksheet.Cell(headerRow, 4).Value = "Count Date";
            worksheet.Cell(headerRow, 5).Value = "Type";
            worksheet.Cell(headerRow, 6).Value = "Status";
            worksheet.Cell(headerRow, 7).Value = "Variance";
            worksheet.Cell(headerRow, 8).Value = "Progress %";
            worksheet.Cell(headerRow, 9).Value = "Fiscal Year";

            worksheet.Range(headerRow, 1, headerRow, 9).Style.Font.Bold = true;
            worksheet.Range(headerRow, 1, headerRow, 9).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;
            worksheet.Range(headerRow, 1, headerRow, 9).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

            // Data
            int row = headerRow + 1;
            int serialNo = 1;
            foreach (var inventory in inventories)
            {
                var progress = 0;
                if (inventory.Details != null && inventory.Details.Any())
                {
                    var countedItems = inventory.Details.Count(d => d.Status == CountStatus.Counted || d.Status == CountStatus.Verified);
                    progress = (countedItems * 100) / inventory.Details.Count;
                }

                worksheet.Cell(row, 1).Value = serialNo;
                worksheet.Cell(row, 2).Value = inventory.ReferenceNumber;
                worksheet.Cell(row, 3).Value = inventory.Store?.Name;
                worksheet.Cell(row, 4).Value = inventory.CountDate.ToString("dd-MMM-yyyy");
                worksheet.Cell(row, 5).Value = inventory.CountType.ToString();
                worksheet.Cell(row, 6).Value = inventory.Status.ToString();
                worksheet.Cell(row, 7).Value = inventory.TotalVariance ?? 0;
                worksheet.Cell(row, 8).Value = progress;
                worksheet.Cell(row, 9).Value = inventory.FiscalYear;

                row++;
                serialNo++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var ms = new System.IO.MemoryStream();
            workbook.SaveAs(ms);

            return ms.ToArray();
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}