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
    public class StoreItemService : IStoreItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StoreItemService> _logger;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;

        public StoreItemService(
            IUnitOfWork unitOfWork,
            ILogger<StoreItemService> logger,
            IActivityLogService activityLogService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
        }

        // Get all store items
        public async Task<IEnumerable<StoreItemDto>> GetAllStoreItemsAsync()
        {
            try
            {
                var storeItems = await _unitOfWork.StoreItems.GetAllAsync();
                var storeItemDtos = new List<StoreItemDto>();

                foreach (var storeItem in storeItems.Where(si => si.IsActive))
                {
                    storeItemDtos.Add(await MapToDto(storeItem));
                }

                return storeItemDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all store items");
                throw;
            }
        }

        // Get store item by ID
        public async Task<StoreItemDto> GetStoreItemByIdAsync(int id)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems.GetByIdAsync(id);
                if (storeItem == null || !storeItem.IsActive)
                    return null;

                return await MapToDto(storeItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store item by id: {Id}", id);
                throw;
            }
        }

        // Get items by store
        public async Task<IEnumerable<StoreItemDto>> GetStoreItemsByStoreAsync(int? storeId)
        {
            try
            {
                var storeItems = await _unitOfWork.StoreItems.FindAsync(
                    si => si.StoreId == storeId && si.IsActive
                );

                var storeItemDtos = new List<StoreItemDto>();
                foreach (var storeItem in storeItems)
                {
                    storeItemDtos.Add(await MapToDto(storeItem));
                }

                return storeItemDtos.OrderBy(si => si.ItemName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store items by store: {StoreId}", storeId);
                throw;
            }
        }

        // Get stores by item
        public async Task<IEnumerable<StoreItemDto>> GetStoreItemsByItemAsync(int? itemId)
        {
            try
            {
                var storeItems = await _unitOfWork.StoreItems.FindAsync(
                    si => si.ItemId == itemId && si.IsActive
                );

                var storeItemDtos = new List<StoreItemDto>();
                foreach (var storeItem in storeItems)
                {
                    storeItemDtos.Add(await MapToDto(storeItem));
                }

                return storeItemDtos.OrderBy(si => si.StoreName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store items by item: {ItemId}", itemId);
                throw;
            }
        }

        // Create store item
        public async Task<int> CreateStoreItemAsync(StoreItemDto storeItemDto)
        {
            try
            {
                // Check if item already exists in store
                var existing = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                    si => si.StoreId == storeItemDto.StoreId &&
                          si.ItemId == storeItemDto.ItemId &&
                          si.IsActive
                );

                if (existing != null)
                {
                    throw new InvalidOperationException("Item already exists in this store");
                }

                var storeItem = new StoreItem
                {
                    StoreId = storeItemDto.StoreId,
                    ItemId = storeItemDto.ItemId,
                    Quantity = storeItemDto.Quantity,
                    MinimumStock = storeItemDto.MinimumStock,
                    MaximumStock = storeItemDto.MaximumStock,
                    ReorderLevel = storeItemDto.ReorderLevel,
                    Location = storeItemDto.Location,
                    CreatedAt = DateTime.Now,
                    CreatedBy = storeItemDto.CreatedBy ?? "System",
                    IsActive = true
                };

                await _unitOfWork.StoreItems.AddAsync(storeItem);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "StoreItem",
                    storeItem.Id,
                    "Create",
                    $"Added item to store with quantity {storeItem.Quantity}",
                    storeItem.CreatedBy
                );

                // Check for initial stock alerts
                await CheckStockLevelsAsync(storeItem);

                return storeItem.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store item");
                throw;
            }
        }

        // Update store item
        public async Task UpdateStoreItemAsync(StoreItemDto storeItemDto)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems.GetByIdAsync(storeItemDto.Id);
                if (storeItem == null || !storeItem.IsActive)
                {
                    throw new InvalidOperationException("Store item not found");
                }

                var oldQuantity = storeItem.Quantity;

                storeItem.Quantity = storeItemDto.Quantity;
                storeItem.MinimumStock = storeItemDto.MinimumStock;
                storeItem.MaximumStock = storeItemDto.MaximumStock;
                storeItem.ReorderLevel = storeItemDto.ReorderLevel;
                storeItem.Location = storeItemDto.Location;
                storeItem.UpdatedAt = DateTime.Now;
                storeItem.UpdatedBy = storeItemDto.UpdatedBy ?? "System";

                _unitOfWork.StoreItems.Update(storeItem);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "StoreItem",
                    storeItem.Id,
                    "Update",
                    $"Updated store item. Quantity changed from {oldQuantity} to {storeItem.Quantity}",
                    storeItem.UpdatedBy
                );

                // Check stock levels
                await CheckStockLevelsAsync(storeItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store item");
                throw;
            }
        }

        // Delete store item (soft delete)
        public async Task DeleteStoreItemAsync(int id)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems.GetByIdAsync(id);
                if (storeItem == null)
                {
                    throw new InvalidOperationException("Store item not found");
                }

                if (storeItem.Quantity > 0)
                {
                    throw new InvalidOperationException("Cannot delete store item with existing stock");
                }

                storeItem.IsActive = false;
                storeItem.UpdatedAt = DateTime.Now;
                storeItem.UpdatedBy = "System";

                _unitOfWork.StoreItems.Update(storeItem);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "StoreItem",
                    storeItem.Id,
                    "Delete",
                    "Deleted store item",
                    storeItem.UpdatedBy
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting store item");
                throw;
            }
        }

        // Get available quantity for specific item in store
        public async Task<decimal> GetAvailableQuantityAsync(int? storeId, int itemId)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                    si => si.StoreId == storeId &&
                          si.ItemId == itemId &&
                          si.IsActive
                );

                return storeItem?.Quantity ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available quantity");
                throw;
            }
        }

        // Get store item quantity (implements the missing method from error)
        public async Task<decimal> GetStoreItemQuantityAsync(int? storeId, int itemId)
        {
            return await GetAvailableQuantityAsync(storeId, itemId);
        }

        public async Task UpdateStockQuantityAsync(int? storeId, int itemId, decimal quantityChange, string reason, string updatedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                    si => si.StoreId == storeId &&
                          si.ItemId == itemId &&
                          si.IsActive
                );

                if (storeItem == null)
                {
                    throw new InvalidOperationException("Store item not found");
                }

                var oldQuantity = storeItem.Quantity;
                storeItem.Quantity += quantityChange;

                if (storeItem.Quantity < 0)
                {
                    throw new InvalidOperationException("Insufficient stock quantity");
                }

                storeItem.UpdatedAt = DateTime.Now;
                storeItem.UpdatedBy = updatedBy;

                _unitOfWork.StoreItems.Update(storeItem);

                // Create proper stock movement record
                var stockMovement = new StockMovement
                {
                    StoreId = storeId,
                    ItemId = itemId,
                    MovementType = quantityChange > 0 ? "In" : "Out",
                    Quantity = Math.Abs(quantityChange),
                    Reason = reason,
                    OldBalance = (decimal)oldQuantity,
                    NewBalance = (decimal)storeItem.Quantity,
                    CreatedAt = DateTime.Now,
                    CreatedBy = updatedBy,
                    IsActive = true
                };

                await _unitOfWork.StockMovements.AddAsync(stockMovement);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Check stock levels
                await CheckStockLevelsAsync(storeItem);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating stock quantity");
                throw;
            }
        }

        // Get low stock items
        public async Task<IEnumerable<StoreItemDto>> GetLowStockItemsAsync(int? storeId = null)
        {
            try
            {
                var query = await _unitOfWork.StoreItems.FindAsync(si => si.IsActive);

                if (storeId.HasValue)
                {
                    query = query.Where(si => si.StoreId == storeId.Value);
                }

                var lowStockItems = query.Where(si => si.Quantity <= si.MinimumStock);

                var storeItemDtos = new List<StoreItemDto>();
                foreach (var storeItem in lowStockItems)
                {
                    storeItemDtos.Add(await MapToDto(storeItem));
                }

                return storeItemDtos.OrderBy(si => si.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock items");
                throw;
            }
        }

        // Get out of stock items
        public async Task<IEnumerable<StoreItemDto>> GetOutOfStockItemsAsync(int? storeId = null)
        {
            try
            {
                var query = await _unitOfWork.StoreItems.FindAsync(
                    si => si.IsActive && si.Quantity == 0
                );

                if (storeId.HasValue)
                {
                    query = query.Where(si => si.StoreId == storeId.Value);
                }

                var storeItemDtos = new List<StoreItemDto>();
                foreach (var storeItem in query)
                {
                    storeItemDtos.Add(await MapToDto(storeItem));
                }

                return storeItemDtos.OrderBy(si => si.ItemName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting out of stock items");
                throw;
            }
        }

        // Get stock value by store
        public async Task<decimal> GetStockValueByStoreAsync(int? storeId)
        {
            try
            {
                var storeItems = await _unitOfWork.StoreItems.FindAsync(
                    si => si.StoreId == storeId && si.IsActive
                );

                decimal totalValue = 0;
                foreach (var storeItem in storeItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
                    if (item != null)
                    {
                        // Get last purchase price
                        var lastPurchase = await _unitOfWork.PurchaseItems
                            .FindAsync(pi => pi.ItemId == storeItem.ItemId);
                        var unitPrice = lastPurchase
                            .OrderByDescending(pi => pi.CreatedAt)
                            .FirstOrDefault()?.UnitPrice ?? 0;

                        totalValue += (decimal)(storeItem.Quantity * unitPrice);
                    }
                }

                return totalValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating stock value");
                throw;
            }
        }

        // Transfer stock between stores
        public async Task TransferStockAsync(int fromStoreId, int toStoreId, int itemId, decimal quantity, string transferredBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Decrease from source store
                await UpdateStockQuantityAsync(
                    fromStoreId,
                    itemId,
                    -quantity,
                    $"Transfer to store {toStoreId}",
                    transferredBy
                );

                // Check if item exists in destination store
                var destStoreItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                    si => si.StoreId == toStoreId &&
                          si.ItemId == itemId &&
                          si.IsActive
                );

                if (destStoreItem == null)
                {
                    // Create new store item in destination
                    var sourceItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                        si => si.StoreId == fromStoreId &&
                              si.ItemId == itemId &&
                              si.IsActive
                    );

                    await CreateStoreItemAsync(new StoreItemDto
                    {
                        StoreId = toStoreId,
                        ItemId = itemId,
                        Quantity = quantity,
                        MinimumStock = sourceItem?.MinimumStock ?? 0,
                        MaximumStock = sourceItem?.MaximumStock ?? 0,
                        ReorderLevel = sourceItem?.ReorderLevel ?? 0,
                        CreatedBy = transferredBy
                    });
                }
                else
                {
                    // Increase in destination store
                    await UpdateStockQuantityAsync(
                        toStoreId,
                        itemId,
                        quantity,
                        $"Transfer from store {fromStoreId}",
                        transferredBy
                    );
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error transferring stock");
                throw;
            }
        }

        // Get stock movement history
        public async Task<IEnumerable<StockMovementDto>> GetStockMovementHistoryAsync(int? storeId, int itemId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                    si => si.StoreId == storeId &&
                          si.ItemId == itemId &&
                          si.IsActive
                );

                if (storeItem == null)
                    return new List<StockMovementDto>();

                var stockMovements = new List<StockMovementDto>();

                // Get all issues for this item from this store
                var issueItems = await _unitOfWork.IssueItems.FindAsync(ii =>
                    ii.ItemId == itemId &&
                    ii.StoreId == storeId
                );

                foreach (var issueItem in issueItems)
                {
                    var issue = await _unitOfWork.Issues.GetByIdAsync(issueItem.IssueId);
                    if (issue != null && issue.IsActive)
                    {
                        if (!fromDate.HasValue || issue.IssueDate >= fromDate.Value)
                        {
                            if (!toDate.HasValue || issue.IssueDate <= toDate.Value)
                            {
                                stockMovements.Add(new StockMovementDto
                                {
                                    Id = issueItem.Id,
                                    MovementType = "Out",
                                    Quantity = issueItem.Quantity,
                                    Reason = $"Issue #{issue.IssueNo}",
                                    OldQuantity = 0,
                                    NewQuantity = 0,
                                    CreatedAt = issue.IssueDate,
                                    CreatedBy = issue.CreatedBy,
                                    ReferenceNo = issue.IssueNo,
                                    ReferenceType = "Issue"
                                });
                            }
                        }
                    }
                }

                // Get all receives for this item to this store
                var receiveItems = await _unitOfWork.ReceiveItems.FindAsync(ri =>
                    ri.ItemId == itemId &&
                    ri.StoreId == storeId
                );

                foreach (var receiveItem in receiveItems)
                {
                    var receive = await _unitOfWork.Receives.GetByIdAsync(receiveItem.ReceiveId);
                    if (receive != null && receive.IsActive)
                    {
                        if (!fromDate.HasValue || receive.ReceiveDate >= fromDate.Value)
                        {
                            if (!toDate.HasValue || receive.ReceiveDate <= toDate.Value)
                            {
                                stockMovements.Add(new StockMovementDto
                                {
                                    Id = receiveItem.Id,
                                    MovementType = "In",
                                    Quantity = (decimal)receiveItem.Quantity,
                                    Reason = $"Receive #{receive.ReceiveNo}",
                                    OldQuantity = 0,
                                    NewQuantity = 0,
                                    CreatedAt = receive.ReceiveDate,
                                    CreatedBy = receive.CreatedBy,
                                    ReferenceNo = receive.ReceiveNo,
                                    ReferenceType = "Receive"
                                });
                            }
                        }
                    }
                }

                // Get all purchase items for this store
                var purchaseItems = await _unitOfWork.PurchaseItems.FindAsync(pi =>
                    pi.ItemId == itemId &&
                    pi.StoreId == storeId
                );

                foreach (var purchaseItem in purchaseItems)
                {
                    var purchase = await _unitOfWork.Purchases.GetByIdAsync(purchaseItem.PurchaseId);
                    if (purchase != null && purchase.IsActive && purchase.Status == "Approved")
                    {
                        if (!fromDate.HasValue || purchase.PurchaseDate >= fromDate.Value)
                        {
                            if (!toDate.HasValue || purchase.PurchaseDate <= toDate.Value)
                            {
                                stockMovements.Add(new StockMovementDto
                                {
                                    Id = purchaseItem.Id,
                                    MovementType = "In",
                                    Quantity = purchaseItem.Quantity,
                                    Reason = $"Purchase #{purchase.PurchaseOrderNo}",
                                    OldQuantity = 0,
                                    NewQuantity = 0,
                                    CreatedAt = purchase.PurchaseDate,
                                    CreatedBy = purchase.CreatedBy,
                                    ReferenceNo = purchase.PurchaseOrderNo,
                                    ReferenceType = "Purchase"
                                });
                            }
                        }
                    }
                }

                // Get all transfers involving this item and store
                var transfersFrom = await _unitOfWork.Transfers.FindAsync(t =>
                    t.FromStoreId == storeId && t.IsActive
                );

                foreach (var transfer in transfersFrom)
                {
                    var transferItems = await _unitOfWork.TransferItems.FindAsync(ti =>
                        ti.TransferId == transfer.Id && ti.ItemId == itemId
                    );

                    foreach (var transferItem in transferItems)
                    {
                        if (!fromDate.HasValue || transfer.TransferDate >= fromDate.Value)
                        {
                            if (!toDate.HasValue || transfer.TransferDate <= toDate.Value)
                            {
                                stockMovements.Add(new StockMovementDto
                                {
                                    Id = transferItem.Id,
                                    MovementType = "Out",
                                    Quantity = transferItem.Quantity,
                                    Reason = $"Transfer #{transfer.TransferNo} to store",
                                    OldQuantity = 0,
                                    NewQuantity = 0,
                                    CreatedAt = transfer.TransferDate,
                                    CreatedBy = transfer.CreatedBy,
                                    ReferenceNo = transfer.TransferNo,
                                    ReferenceType = "Transfer"
                                });
                            }
                        }
                    }
                }

                var transfersTo = await _unitOfWork.Transfers.FindAsync(t =>
                    t.ToStoreId == storeId && t.IsActive
                );

                foreach (var transfer in transfersTo)
                {
                    var transferItems = await _unitOfWork.TransferItems.FindAsync(ti =>
                        ti.TransferId == transfer.Id && ti.ItemId == itemId
                    );

                    foreach (var transferItem in transferItems)
                    {
                        if (!fromDate.HasValue || transfer.TransferDate >= fromDate.Value)
                        {
                            if (!toDate.HasValue || transfer.TransferDate <= toDate.Value)
                            {
                                stockMovements.Add(new StockMovementDto
                                {
                                    Id = transferItem.Id,
                                    MovementType = "In",
                                    Quantity = transferItem.Quantity,
                                    Reason = $"Transfer #{transfer.TransferNo} from store",
                                    OldQuantity = 0,
                                    NewQuantity = 0,
                                    CreatedAt = transfer.TransferDate,
                                    CreatedBy = transfer.CreatedBy,
                                    ReferenceNo = transfer.TransferNo,
                                    ReferenceType = "Transfer"
                                });
                            }
                        }
                    }
                }

                // Get stock adjustments
                var adjustments = await _unitOfWork.StockAdjustments.FindAsync(sa =>
                    sa.StoreId == storeId &&
                    sa.ItemId == itemId &&
                    sa.IsActive
                );

                foreach (var adjustment in adjustments)
                {
                    if (!fromDate.HasValue || adjustment.AdjustmentDate >= fromDate.Value)
                    {
                        if (!toDate.HasValue || adjustment.AdjustmentDate <= toDate.Value)
                        {
                            stockMovements.Add(new StockMovementDto
                            {
                                Id = adjustment.Id,
                                MovementType = adjustment.AdjustmentType == "Increase" ? "In" : "Out",
                                Quantity = (decimal)adjustment.AdjustmentQuantity,
                                Reason = $"Stock Adjustment #{adjustment.AdjustmentNo} - {adjustment.Reason}",
                                OldQuantity = (decimal)adjustment.OldQuantity,
                                NewQuantity = (decimal)adjustment.NewQuantity,
                                CreatedAt = adjustment.AdjustmentDate,
                                CreatedBy = adjustment.CreatedBy,
                                ReferenceNo = adjustment.AdjustmentNo,
                                ReferenceType = "Adjustment"
                            });
                        }
                    }
                }

                // Sort by date descending
                return stockMovements.OrderByDescending(sm => sm.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock movement history");
                throw;
            }
        }

        // Private helper methods
        private async Task<StoreItemDto> MapToDto(StoreItem storeItem)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
            var store = await _unitOfWork.Stores.GetByIdAsync(storeItem.StoreId);

            return new StoreItemDto
            {
                Id = storeItem.Id,
                StoreId = storeItem.StoreId,
                StoreName = store?.Name ?? "Unknown",
                ItemId = storeItem.ItemId,
                ItemName = item?.Name ?? "Unknown",
                ItemCode = item?.Code,
                Quantity = storeItem.Quantity,
                Unit = item?.Unit,
                MinimumStock = storeItem.MinimumStock,
                MaximumStock = storeItem.MaximumStock,
                ReorderLevel = storeItem.ReorderLevel,
                Location = storeItem.Location,
                IsLowStock = storeItem.Quantity <= storeItem.MinimumStock,
                IsOutOfStock = storeItem.Quantity == 0,
                CreatedAt = storeItem.CreatedAt,
                UpdatedAt = storeItem.UpdatedAt
            };
        }

        private async Task CheckStockLevelsAsync(StoreItem storeItem)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
            var store = await _unitOfWork.Stores.GetByIdAsync(storeItem.StoreId);

            if (storeItem.Quantity == 0)
            {
                // Send out of stock notification
                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = null, // ✅ Use null for role-based notifications
                    TargetRole = "StoreManager", // Send to store managers
                    Title = "Out of Stock Alert",
                    Message = $"{item?.Name} is out of stock in {store?.Name}",
                    Type = "error",
                    Priority = "High",
                    RelatedEntity = "StoreItem",
                    RelatedEntityId = storeItem.Id,
                    Url = $"/StoreItem/Details/{storeItem.Id}"
                });
            }
            else if (storeItem.Quantity <= storeItem.MinimumStock)
            {
                // Send low stock notification
                await _notificationService.SendNotificationAsync(new NotificationDto
                {
                    UserId = null, // ✅ Use null for role-based notifications
                    TargetRole = "StoreManager", // Send to store managers
                    Title = "Low Stock Alert",
                    Message = $"{item?.Name} is low in stock ({storeItem.Quantity} {item?.Unit}) in {store?.Name}",
                    Type = "warning",
                    Priority = "Normal",
                    RelatedEntity = "StoreItem",
                    RelatedEntityId = storeItem.Id,
                    Url = $"/StoreItem/Details/{storeItem.Id}"
                });
            }
        }

        // Helper methods to extract data from activity log descriptions
        private decimal ExtractQuantityFromDescription(string description)
        {
            // Parse quantity change from description like "(+10)" or "(-5)"
            var match = System.Text.RegularExpressions.Regex.Match(description, @"\(([+-]?\d+\.?\d*)\)");
            if (match.Success && decimal.TryParse(match.Groups[1].Value, out var quantity))
            {
                return Math.Abs(quantity);
            }
            return 0;
        }

        private string ExtractReasonFromDescription(string description)
        {
            // Extract reason after "Reason: "
            var reasonIndex = description.IndexOf("Reason: ");
            if (reasonIndex >= 0)
            {
                return description.Substring(reasonIndex + 8).Trim();
            }
            return description;
        }

        private decimal ExtractOldQuantityFromDescription(string description)
        {
            // Parse old quantity from "changed from X to Y"
            var match = System.Text.RegularExpressions.Regex.Match(description, @"changed from (\d+\.?\d*) to");
            if (match.Success && decimal.TryParse(match.Groups[1].Value, out var oldQty))
            {
                return oldQty;
            }
            return 0;
        }

        private decimal ExtractNewQuantityFromDescription(string description)
        {
            // Parse new quantity from "changed from X to Y"
            var match = System.Text.RegularExpressions.Regex.Match(description, @"to (\d+\.?\d*)");
            if (match.Success && decimal.TryParse(match.Groups[1].Value, out var newQty))
            {
                return newQty;
            }
            return 0;
        }

    }
}
