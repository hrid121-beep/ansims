using DocumentFormat.OpenXml.InkML;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StockService> _logger;
        private readonly object _stockLock = new object();

        public StockService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<StockService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<decimal> GetAvailableStockAsync(int storeId, int itemId)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems
                    .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ItemId == itemId);

                if (storeItem == null)
                    return 0;

                // Available = Current - Reserved
                return storeItem.CurrentStock - (storeItem.ReservedStock ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting available stock for store {storeId}, item {itemId}");
                throw;
            }
        }

        public async Task<IEnumerable<StockDto>> GetStoreStockAsync(int storeId)
        {
            try
            {
                var stockItems = await _unitOfWork.StoreItems
                    .Query()
                    .Include(si => si.Item)
                    .ThenInclude(i => i.SubCategory)
                    .ThenInclude(i => i.Category)
                    .Include(si => si.Item.SubCategory)
                    .Include(si => si.Item.Brand)
                    .Where(si => si.StoreId == storeId)
                    .Select(si => new StockDto
                    {
                        ItemId = si.ItemId,
                        ItemName = si.Item.Name,
                        ItemNameBn = si.Item.NameBn,
                        ItemCode = si.Item.ItemCode,
                        CategoryName = si.Item.SubCategory.Category.Name,
                        SubCategoryName = si.Item.SubCategory.Name,
                        BrandName = si.Item.Brand.Name,
                        CurrentQuantity = si.CurrentStock,
                        ReservedQuantity = si.ReservedStock ?? 0,
                        AvailableQuantity = si.CurrentStock - (si.ReservedStock ?? 0),
                        MinimumStock = si.MinimumStock,
                        MaximumStock = si.MaximumStock,
                        ReorderLevel = si.ReorderLevel,
                        Unit = si.Item.Unit,
                        UnitPrice = si.Item.UnitPrice ?? 0,
                        TotalValue = si.CurrentStock * (si.Item.UnitPrice ?? 0),
                        LastCountDate = si.LastCountDate,
                        Location = si.Location,
                        StockStatus = GetStockStatus(si)
                    })
                    .ToListAsync();

                return stockItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting store stock for store {storeId}");
                throw;
            }
        }

        public async Task<bool> ReserveStockAsync(int? storeId, int itemId, decimal? quantity)
        {
            try
            {
                lock (_stockLock)
                {
                    var storeItem = _unitOfWork.StoreItems.Query()
                        .FirstOrDefault(si => si.StoreId == storeId && si.ItemId == itemId);

                    if (storeItem == null)
                        throw new InvalidOperationException("Stock item not found");

                    var availableStock = storeItem.CurrentStock - (storeItem.ReservedStock ?? 0);
                    if (availableStock < quantity)
                        throw new InvalidOperationException("Insufficient available stock");

                    storeItem.ReservedStock = (storeItem.ReservedStock ?? 0) + quantity;
                    storeItem.UpdatedAt = DateTime.Now;

                    _unitOfWork.SaveChangesAsync().Wait();
                }

                _logger.LogInformation($"Reserved {quantity} units of item {itemId} in store {storeId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reserving stock for store {storeId}, item {itemId}");
                throw;
            }
        }

        public async Task<bool> IssueStockAsync(int? storeId, int itemId, decimal quantity,
            string reference, string issuedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                lock (_stockLock)
                {
                    var storeItem = _unitOfWork.StoreItems.Query()
                        .FirstOrDefault(si => si.StoreId == storeId && si.ItemId == itemId);

                    if (storeItem == null)
                        throw new InvalidOperationException("Stock item not found");

                    if (storeItem.CurrentStock < quantity)
                        throw new InvalidOperationException("Insufficient stock");

                    var previousStock = storeItem.CurrentStock;

                    // Deduct from current stock
                    storeItem.CurrentStock -= quantity;

                    // Release reserved stock if any
                    if (storeItem.ReservedStock > 0)
                    {
                        storeItem.ReservedStock = Math.Max(0, (storeItem.ReservedStock ?? 0) - quantity);
                    }

                    storeItem.LastIssueDate = DateTime.Now;
                    storeItem.UpdatedAt = DateTime.Now;

                    // Create stock movement
                    var movement = new StockMovement
                    {
                        ItemId = itemId,
                        StoreId = storeId,
                        MovementType = StockMovementType.Issue.ToString(),
                        Quantity = -quantity,
                        OldBalance = previousStock,
                        NewBalance = storeItem.CurrentStock,
                        ReferenceType = "Issue",
                        ReferenceNo = reference,
                        MovementDate = DateTime.Now,
                        CreatedBy = issuedBy
                    };

                    _unitOfWork.StockMovements.AddAsync(movement).Wait();
                    _unitOfWork.SaveChangesAsync().Wait();

                    // Check for low stock alert
                    if (storeItem.CurrentStock <= storeItem.MinimumStock)
                    {
                        if (storeId.HasValue)
                            CreateLowStockAlertAsync(storeId.Value, itemId, storeItem.CurrentStock).Wait();
                    }
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Issued {quantity} units of item {itemId} from store {storeId}");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error issuing stock for store {storeId}, item {itemId}");
                throw;
            }
        }

        public async Task<bool> ReceiveStockAsync(int storeId, int itemId, decimal quantity,
            string reference, string receivedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                lock (_stockLock)
                {
                    var storeItem = _unitOfWork.StoreItems.Query()
                        .FirstOrDefault(si => si.StoreId == storeId && si.ItemId == itemId);

                    if (storeItem == null)
                    {
                        // Create new store item if doesn't exist
                        var item = _unitOfWork.Items.GetByIdAsync(itemId).Result;
                        storeItem = new StoreItem
                        {
                            StoreId = storeId,
                            ItemId = itemId,
                            CurrentStock = 0,
                            MinimumStock = 10, // Default
                            MaximumStock = 100, // Default
                            ReorderLevel = 20, // Default
                            CreatedAt = DateTime.Now
                        };
                        _unitOfWork.StoreItems.AddAsync(storeItem).Wait();
                    }

                    var previousStock = storeItem.CurrentStock;

                    // Add to current stock
                    storeItem.CurrentStock += quantity;
                    storeItem.LastReceiveDate = DateTime.Now;
                    storeItem.UpdatedAt = DateTime.Now;

                    // Create stock movement
                    var movement = new StockMovement
                    {
                        ItemId = itemId,
                        StoreId = storeId,
                        MovementType = StockMovementType.PurchaseReceive.ToString(),
                        Quantity = quantity,
                        OldBalance = previousStock,
                        NewBalance = storeItem.CurrentStock,
                        ReferenceType = "Receive",
                        ReferenceNo = reference,
                        MovementDate = DateTime.Now,
                        CreatedBy = receivedBy
                    };

                    _unitOfWork.StockMovements.AddAsync(movement).Wait();
                    _unitOfWork.SaveChangesAsync().Wait();

                    // Check for overstock
                    if (storeItem.CurrentStock > storeItem.MaximumStock)
                    {
                        CreateOverstockAlertAsync(storeId, itemId, storeItem.CurrentStock).Wait();
                    }
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Received {quantity} units of item {itemId} in store {storeId}");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error receiving stock for store {storeId}, item {itemId}");
                throw;
            }
        }

        public async Task<bool> TransferOutStockAsync(int fromStoreId, int itemId, decimal quantity,
            string reference, string transferredBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                lock (_stockLock)
                {
                    var storeItem = _unitOfWork.StoreItems.Query()
                        .FirstOrDefault(si => si.StoreId == fromStoreId && si.ItemId == itemId);

                    if (storeItem == null)
                        throw new InvalidOperationException("Stock item not found");

                    if (storeItem.CurrentStock < quantity)
                        throw new InvalidOperationException("Insufficient stock for transfer");

                    var previousStock = storeItem.CurrentStock;

                    // Deduct from current stock
                    storeItem.CurrentStock -= quantity;

                    // Release reserved stock
                    if (storeItem.ReservedStock > 0)
                    {
                        storeItem.ReservedStock = Math.Max(0, (storeItem.ReservedStock ?? 0) - quantity);
                    }

                    storeItem.LastTransferDate = DateTime.Now;
                    storeItem.UpdatedAt = DateTime.Now;

                    _unitOfWork.SaveChangesAsync().Wait();
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Transferred out {quantity} units of item {itemId} from store {fromStoreId}");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error transferring out stock from store {fromStoreId}, item {itemId}");
                throw;
            }
        }

        public async Task<bool> TransferInStockAsync(int toStoreId, int itemId, decimal quantity,
            string reference, string receivedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                lock (_stockLock)
                {
                    var storeItem = _unitOfWork.StoreItems.Query()
                        .FirstOrDefault(si => si.StoreId == toStoreId && si.ItemId == itemId);

                    if (storeItem == null)
                    {
                        // Create new store item if doesn't exist
                        storeItem = new StoreItem
                        {
                            StoreId = toStoreId,
                            ItemId = itemId,
                            CurrentStock = 0,
                            MinimumStock = 10,
                            MaximumStock = 100,
                            ReorderLevel = 20,
                            CreatedAt = DateTime.Now
                        };
                        _unitOfWork.StoreItems.AddAsync(storeItem).Wait();
                    }

                    // Add to current stock
                    storeItem.CurrentStock += quantity;
                    storeItem.LastTransferDate = DateTime.Now;
                    storeItem.UpdatedAt = DateTime.Now;

                    _unitOfWork.SaveChangesAsync().Wait();
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Transferred in {quantity} units of item {itemId} to store {toStoreId}");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error transferring in stock to store {toStoreId}, item {itemId}");
                throw;
            }
        }

        public async Task<bool> AdjustStockAsync(int? storeId, int itemId, decimal adjustmentQuantity,
            string reason, string adjustedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                lock (_stockLock)
                {
                    var storeItem = _unitOfWork.StoreItems.Query()
                        .FirstOrDefault(si => si.StoreId == storeId && si.ItemId == itemId);

                    if (storeItem == null)
                        throw new InvalidOperationException("Stock item not found");

                    var previousStock = storeItem.CurrentStock;

                    // Apply adjustment
                    storeItem.CurrentStock += adjustmentQuantity;

                    // Ensure stock doesn't go negative
                    if (storeItem.CurrentStock < 0)
                        storeItem.CurrentStock = 0;

                    storeItem.LastAdjustmentDate = DateTime.Now;
                    storeItem.UpdatedAt = DateTime.Now;

                    // Create stock movement
                    var movement = new StockMovement
                    {
                        ItemId = itemId,
                        StoreId = storeId,
                        MovementType = StockMovementType.Adjustment.ToString(),
                        Quantity = adjustmentQuantity,
                        OldBalance = previousStock,
                        NewBalance = storeItem.CurrentStock,
                        ReferenceType = "Adjustment",
                        ReferenceNo = reason,
                        MovementDate = DateTime.Now,
                        CreatedBy = adjustedBy,
                        Remarks = reason
                    };

                    _unitOfWork.StockMovements.AddAsync(movement).Wait();
                    _unitOfWork.SaveChangesAsync().Wait();
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Adjusted stock by {adjustmentQuantity} units for item {itemId} in store {storeId}");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error adjusting stock for store {storeId}, item {itemId}");
                throw;
            }
        }

        public async Task<bool> WriteOffStockAsync(int storeId, int itemId, decimal quantity,
            string reason, string writtenOffBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                lock (_stockLock)
                {
                    var storeItem = _unitOfWork.StoreItems.Query()
                        .FirstOrDefault(si => si.StoreId == storeId && si.ItemId == itemId);

                    if (storeItem == null)
                        throw new InvalidOperationException("Stock item not found");

                    if (storeItem.CurrentStock < quantity)
                        throw new InvalidOperationException("Cannot write-off more than current stock");

                    var previousStock = storeItem.CurrentStock;

                    // Deduct from current stock
                    storeItem.CurrentStock -= quantity;
                    storeItem.UpdatedAt = DateTime.Now;

                    // Create stock movement
                    var movement = new StockMovement
                    {
                        ItemId = itemId,
                        StoreId = storeId,
                        MovementType = StockMovementType.WriteOff.ToString(),
                        Quantity = -quantity,
                        OldBalance = previousStock,
                        NewBalance = storeItem.CurrentStock,
                        ReferenceType = "WriteOff",
                        ReferenceNo = reason,
                        MovementDate = DateTime.Now,
                        CreatedBy = writtenOffBy,
                        Remarks = reason
                    };

                    _unitOfWork.StockMovements.AddAsync(movement).Wait();
                    _unitOfWork.SaveChangesAsync().Wait();
                }

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Written off {quantity} units of item {itemId} from store {storeId}");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error writing off stock for store {storeId}, item {itemId}");
                throw;
            }
        }

        public async Task<IEnumerable<StockMovementDto>> GetStockMovementsAsync(int storeId,
            int? itemId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _unitOfWork.StockMovements.Query()
                    .Include(sm => sm.Item)
                    .Include(sm => sm.Store)
                    .Where(sm => sm.StoreId == storeId);

                if (itemId.HasValue)
                    query = query.Where(sm => sm.ItemId == itemId.Value);

                if (fromDate.HasValue)
                    query = query.Where(sm => sm.MovementDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(sm => sm.MovementDate <= toDate.Value);

                var movements = await query
                    .OrderByDescending(sm => sm.MovementDate)
                    .Select(sm => new StockMovementDto
                    {
                        Id = sm.Id,
                        ItemId = sm.ItemId,
                        ItemName = sm.Item.Name,
                        ItemCode = sm.Item.ItemCode,
                        StoreId = sm.StoreId,
                        StoreName = sm.Store.Name,
                        MovementType = sm.MovementType,
                        Quantity = sm.Quantity,
                        OldBalance = sm.OldBalance,
                        NewBalance = sm.NewBalance,
                        ReferenceType = sm.ReferenceType,
                        ReferenceNo = sm.ReferenceNo,
                        MovementDate = sm.MovementDate,
                        CreatedBy = sm.CreatedBy,
                        Remarks = sm.Remarks
                    })
                    .ToListAsync();

                return movements;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stock movements for store {storeId}");
                throw;
            }
        }

        public async Task<StockAnalysisDto> GetStockAnalysisAsync(int storeId)
        {
            try
            {
                var storeItems = await _unitOfWork.StoreItems.Query()
                    .Include(si => si.Item)
                    .Where(si => si.StoreId == storeId)
                    .ToListAsync();

                var analysis = new StockAnalysisDto
                {
                    StoreId = storeId,
                    TotalItems = storeItems.Count,
                    TotalValue = storeItems.Sum(si => si.CurrentStock * (si.Item.UnitPrice ?? 0)),

                    // Stock status counts
                    InStockItems = storeItems.Count(si => si.CurrentStock > si.MinimumStock),
                    LowStockItems = storeItems.Count(si => si.CurrentStock <= si.MinimumStock && si.CurrentStock > 0),
                    OutOfStockItems = storeItems.Count(si => si.CurrentStock == 0),
                    OverstockItems = storeItems.Count(si => si.CurrentStock > si.MaximumStock),

                    // Stock value by status
                    LowStockValue = storeItems
                        .Where(si => si.CurrentStock <= si.MinimumStock && si.CurrentStock > 0)
                        .Sum(si => si.CurrentStock * (si.Item.UnitPrice ?? 0)),

                    OverstockValue = storeItems
                        .Where(si => si.CurrentStock > si.MaximumStock)
                        .Sum(si => (si.CurrentStock - si.MaximumStock) * (si.Item.UnitPrice ?? 0)),

                    // Turnover analysis
                    FastMovingItems = await GetFastMovingItemsAsync(storeId, 10),
                    SlowMovingItems = await GetSlowMovingItemsAsync(storeId, 10),

                    // Reorder suggestions
                    ItemsToReorder = storeItems
                        .Where(si => si.CurrentStock <= si.ReorderLevel)
                        .Select(si => new ReorderSuggestionDto
                        {
                            ItemId = si.ItemId,
                            ItemName = si.Item.Name,
                            CurrentStock = si.CurrentStock,
                            ReorderLevel = si.ReorderLevel,
                            SuggestedQuantity = si.MaximumStock - si.CurrentStock,
                            EstimatedCost = (si.MaximumStock - si.CurrentStock) * (si.Item.UnitPrice ?? 0)
                        })
                        .ToList(),

                    AnalysisDate = DateTime.Now
                };

                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stock analysis for store {storeId}");
                throw;
            }
        }

        private string GetStockStatus(StoreItem storeItem)
        {
            if (storeItem.CurrentStock == 0)
                return "Out of Stock";
            else if (storeItem.CurrentStock <= storeItem.MinimumStock)
                return "Low Stock";
            else if (storeItem.CurrentStock > storeItem.MaximumStock)
                return "Overstock";
            else
                return "In Stock";
        }

        private async Task CreateLowStockAlertAsync(int storeId, int itemId, decimal currentStock)
        {
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Low Stock Alert",
                Message = $"Item {itemId} in store {storeId} has low stock: {currentStock} units",
                Type = "LowStock",
                Priority = "High",
                CreatedAt = DateTime.Now
            });
        }

        private async Task CreateOverstockAlertAsync(int storeId, int itemId, decimal currentStock)
        {
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Overstock Alert",
                Message = $"Item {itemId} in store {storeId} exceeds maximum stock: {currentStock} units",
                Type = "Overstock",
                Priority = "Medium",
                CreatedAt = DateTime.Now
            });
        }

        private async Task<List<ItemMovementDto>> GetFastMovingItemsAsync(int storeId, int count)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-1);

            var fastMovers = await _unitOfWork.StockMovements.Query()
                .Include(sm => sm.Item)
                .Where(sm => sm.StoreId == storeId &&
                            sm.MovementType == StockMovementType.Issue.ToString() &&
                            sm.MovementDate >= startDate &&
                            sm.MovementDate <= endDate)
                .GroupBy(sm => new { sm.ItemId, sm.Item.Name })
                .Select(g => new ItemMovementDto
                {
                    ItemId = g.Key.ItemId,
                    ItemName = g.Key.Name,
                    MovementCount = g.Count(),
                    TotalQuantity = Math.Abs(g.Sum(sm => sm.Quantity) ?? 0)  // Fixed: Handle nullable with ?? 0
                })
                .OrderByDescending(im => im.TotalQuantity)
                .Take(count)
                .ToListAsync();

            return fastMovers;
        }

        private async Task<List<ItemMovementDto>> GetSlowMovingItemsAsync(int storeId, int count)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-3);

            var slowMovers = await _unitOfWork.StoreItems.Query()
                .Include(si => si.Item)
                .Where(si => si.StoreId == storeId && si.CurrentStock > 0)
                .Select(si => new
                {
                    StoreItem = si,
                    MovementCount = _unitOfWork.StockMovements.Query()
                        .Count(sm => sm.StoreId == storeId &&
                                    sm.ItemId == si.ItemId &&
                                    sm.MovementType == StockMovementType.Issue.ToString() &&
                                    sm.MovementDate >= startDate)
                })
                .Where(x => x.MovementCount < 3) // Less than 3 movements in 3 months
                .OrderBy(x => x.MovementCount)
                .Take(count)
                .Select(x => new ItemMovementDto
                {
                    ItemId = x.StoreItem.ItemId,
                    ItemName = x.StoreItem.Item.Name,
                    MovementCount = x.MovementCount,
                    TotalQuantity = x.StoreItem.CurrentStock
                })
                .ToListAsync();

            return slowMovers;
        }
        public async Task<bool> AdjustStockAsync(StockAdjustment adjustment)
        {
            // Implementation for stock adjustment
            _unitOfWork.StockAdjustments.Update(adjustment);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        Task IStockService.AdjustStockAsync(StockAdjustment adjustment)
        {
            return AdjustStockAsync(adjustment);
        }

        public async Task<List<StockMovementDto>> GetStockMovementsAsync(int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);

            return await _unitOfWork.StockMovements.Query()
                .Where(m => m.MovementDate >= startDate)
                .OrderByDescending(m => m.MovementDate)
                .Select(m => new StockMovementDto
                {
                    Id = m.Id,
                    ItemId = m.ItemId,
                    ItemName = m.Item.Name,
                    MovementType = m.MovementType,
                    Quantity = m.Quantity,
                    MovementDate = m.MovementDate,
                    ReferenceNumber = m.ReferenceNo,
                    FromStore = m.SourceStore.Name,
                    ToStore = m.DestinationStore.Name
                })
                .Take(100)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetCategoryWiseStockValueAsync()
        {
            var storeItems = await _unitOfWork.StoreItems.Query()
                .Include(s => s.Item)
                .ThenInclude(i => i.SubCategory)
                .ThenInclude(sc => sc.Category)
                .Where(s => s.IsActive)
                .ToListAsync();

            return storeItems
                .GroupBy(s => s.Item.SubCategory?.Category?.Name ?? "Uncategorized")
                .Select(g => new
                {
                    Category = g.Key,
                    Value = g.Sum(s => s.CurrentStock * (s.Item.UnitPrice ?? 0))
                })
                .ToDictionary(x => x.Category, x => x.Value);
        }

        public async Task<StockSummaryDto> GetStockSummaryAsync(int? storeId = null)
        {
            var query = _unitOfWork.StoreItems.Query().Where(s => s.IsActive);

            if (storeId.HasValue)
                query = query.Where(s => s.StoreId == storeId.Value);

            var storeItems = await query.Include(s => s.Item).ToListAsync();

            return new StockSummaryDto
            {
                TotalItems = storeItems.Select(s => s.ItemId).Distinct().Count(),
                TotalQuantity = (int)storeItems.Sum(s => s.CurrentStock),
                TotalValue = storeItems.Sum(s => s.CurrentStock * (s.Item?.UnitPrice ?? 0)),
                LastUpdated = DateTime.Now
            };
        }

        public async Task<bool> CheckStockAvailabilityAsync(int itemId, int storeId, int quantity)
        {
            var storeItem = await _unitOfWork.StoreItems
                .FirstOrDefaultAsync(s => s.ItemId == itemId && s.StoreId == storeId && s.IsActive);

            return storeItem != null && storeItem.CurrentStock >= quantity;
        }

        public async Task UpdateStockAsync(int itemId, int storeId, int quantity, string transactionType)
        {
            var storeItem = await _unitOfWork.StoreItems
                .FirstOrDefaultAsync(s => s.ItemId == itemId && s.StoreId == storeId && s.IsActive);

            if (storeItem != null)
            {
                if (transactionType == "Issue" || transactionType == "Transfer-Out")
                    storeItem.CurrentStock -= quantity;
                else
                    storeItem.CurrentStock += quantity;

                storeItem.UpdatedAt = DateTime.Now;
                _unitOfWork.StoreItems.Update(storeItem);
                await _unitOfWork.CompleteAsync();
            }
        }
        public async Task<decimal> GetTotalStockValueAsync()
        {
            var storeItems = await _unitOfWork.StoreItems.Query()
                .Include(si => si.Item)
                .Where(s => s.IsActive)
                .ToListAsync();

            return storeItems.Sum(s => s.CurrentStock * (s.Item.UnitPrice ?? 0));
        }
        public async Task<IEnumerable<StockLevelDto>> GetLowStockItemsAsync(int? storeId)
        {
            var query = _unitOfWork.StoreItems.Query()
                .Include(si => si.Item)
                    .ThenInclude(i => i.SubCategory)
                        .ThenInclude(sc => sc.Category)
                .Include(si => si.Store)
                .Where(si => si.IsActive);

            if (storeId.HasValue)
            {
                query = query.Where(si => si.StoreId == storeId);
            }

            var storeItems = await query.ToListAsync();

            var lowStockItems = storeItems
                .Where(si => si.Item != null && si.Quantity <= si.Item.MinimumStock)
                .Select(si => new StockLevelDto
                {
                    ItemId = si.ItemId,
                    ItemCode = si.Item.ItemCode ?? si.Item.Code,
                    ItemName = si.Item.Name,
                    CategoryName = si.Item.SubCategory?.Category?.Name,
                    SubCategoryName = si.Item.SubCategory?.Name,
                    StoreId = si.StoreId,
                    StoreName = si.Store?.Name,
                    CurrentStock = si.Quantity,
                    MinimumStock = si.Item.MinimumStock,
                    MaximumStock = si.Item.MaximumStock,
                    ReorderLevel = si.Item.ReorderLevel,
                    Unit = si.Item.Unit,
                    Status = si.Quantity == 0 ? "Out of Stock" : "Low Stock",
                    IsLowStock = true,
                    IsOutOfStock = si.Quantity == 0
                })
                .OrderBy(i => i.CurrentStock)
                .ToList();

            return lowStockItems;
        }

        // Keep the simplified versions you already have for these:
        public async Task<int> GetStockAccuracyPercentageAsync() => 98;
        public async Task<decimal> GetInventoryTurnoverAsync() => 3.5m;
        public async Task<int> GetFillRatePercentageAsync() => 95;
        public async Task<int> GetAverageLeadTimeDaysAsync() => 3;
    }
}
