using IMS.Application.DTOs;
using IMS.Application.Helpers;
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
    public class StockMovementService : IStockMovementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StockMovementService> _logger;
        private readonly IUserContext _userContext;

        public StockMovementService(
            IUnitOfWork unitOfWork,
            ILogger<StockMovementService> logger,
            IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<PagedResult<StockMovementDto>> GetStockMovementsAsync(
            int pageNumber, int pageSize, int? storeId = null, int? itemId = null,
            string movementType = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _unitOfWork.StockMovements.Query()
                    .Include(sm => sm.Item)
                    .Include(sm => sm.Store)
                    .Include(sm => sm.SourceStore)
                    .Include(sm => sm.DestinationStore)
                    .Where(sm => sm.IsActive);

                if (storeId.HasValue)
                    query = query.Where(sm => sm.StoreId == storeId || sm.SourceStoreId == storeId || sm.DestinationStoreId == storeId);

                if (itemId.HasValue)
                    query = query.Where(sm => sm.ItemId == itemId);

                if (!string.IsNullOrEmpty(movementType))
                    query = query.Where(sm => sm.MovementType == movementType);

                if (fromDate.HasValue)
                    query = query.Where(sm => sm.MovementDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(sm => sm.MovementDate <= toDate.Value);

                var totalCount = await query.CountAsync();

                var movements = await query
                    .OrderByDescending(sm => sm.MovementDate)
                    .ThenByDescending(sm => sm.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var movementDtos = movements.Select(sm => new StockMovementDto
                {
                    Id = sm.Id,
                    MovementType = sm.MovementType,
                    MovementDate = sm.MovementDate,
                    ItemId = sm.ItemId,
                    ItemName = sm.Item?.Name,
                    ItemCode = sm.Item?.ItemCode,
                    StoreId = sm.StoreId,
                    StoreName = sm.Store?.Name,
                    SourceStoreId = sm.SourceStoreId,
                    SourceStoreName = sm.SourceStore?.Name,
                    DestinationStoreId = sm.DestinationStoreId,
                    DestinationStoreName = sm.DestinationStore?.Name,
                    Quantity = sm.Quantity ?? 0,
                    OldBalance = sm.OldBalance,
                    NewBalance = sm.NewBalance,
                    UnitPrice = sm.UnitPrice ?? 0,
                    TotalValue = sm.TotalValue ?? 0,
                    ReferenceType = sm.ReferenceType,
                    ReferenceNo = sm.ReferenceNo,
                    Reason = sm.Reason,
                    Notes = sm.Notes,
                    MovedBy = sm.MovedBy,
                    CreatedAt = sm.CreatedAt
                }).ToList();

                return new PagedResult<StockMovementDto>
                {
                    Items = movementDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock movements");
                throw;
            }
        }

        public async Task<StockMovementDto> GetStockMovementByIdAsync(int id)
        {
            var movement = await _unitOfWork.StockMovements.Query()
                .Include(sm => sm.Item)
                .Include(sm => sm.Store)
                .Include(sm => sm.SourceStore)
                .Include(sm => sm.DestinationStore)
                .FirstOrDefaultAsync(sm => sm.Id == id);

            if (movement == null)
                return null;

            return new StockMovementDto
            {
                Id = movement.Id,
                MovementType = movement.MovementType,
                MovementDate = movement.MovementDate,
                ItemId = movement.ItemId,
                ItemName = movement.Item?.Name,
                ItemCode = movement.Item?.ItemCode,
                StoreId = movement.StoreId,
                StoreName = movement.Store?.Name,
                SourceStoreId = movement.SourceStoreId,
                SourceStoreName = movement.SourceStore?.Name,
                DestinationStoreId = movement.DestinationStoreId,
                DestinationStoreName = movement.DestinationStore?.Name,
                Quantity = movement.Quantity ?? 0,
                OldBalance = movement.OldBalance,
                NewBalance = movement.NewBalance,
                UnitPrice = movement.UnitPrice ?? 0,
                TotalValue = movement.TotalValue ?? 0,
                ReferenceType = movement.ReferenceType,
                ReferenceNo = movement.ReferenceNo,
                Reason = movement.Reason,
                Notes = movement.Notes,
                Remarks = movement.Remarks,
                MovedBy = movement.MovedBy,
                CreatedAt = movement.CreatedAt
            };
        }

        public async Task<StockMovementDto> CreateStockMovementAsync(StockMovementDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var movement = new StockMovement
                {
                    MovementType = dto.MovementType,
                    MovementDate = dto.MovementDate,
                    ItemId = dto.ItemId,
                    StoreId = dto.StoreId,
                    SourceStoreId = dto.SourceStoreId,
                    DestinationStoreId = dto.DestinationStoreId,
                    Quantity = dto.Quantity,
                    OldBalance = dto.OldBalance,
                    NewBalance = dto.NewBalance,
                    UnitPrice = dto.UnitPrice,
                    TotalValue = dto.TotalValue,
                    ReferenceType = dto.ReferenceType,
                    ReferenceNo = dto.ReferenceNo,
                    Reason = dto.Reason,
                    Notes = dto.Notes,
                    Remarks = dto.Remarks,
                    MovedBy = dto.MovedBy ?? _userContext.CurrentUserName,
                    CreatedBy = _userContext.CurrentUserName,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.StockMovements.AddAsync(movement);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                dto.Id = movement.Id;
                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating stock movement");
                throw;
            }
        }

        public async Task<bool> DeleteStockMovementAsync(int id)
        {
            try
            {
                var movement = await _unitOfWork.StockMovements.GetByIdAsync(id);
                if (movement == null)
                    return false;

                movement.IsActive = false;
                movement.UpdatedBy = _userContext.CurrentUserName;
                movement.UpdatedAt = DateTime.Now;

                _unitOfWork.StockMovements.Update(movement);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock movement");
                return false;
            }
        }

        public async Task<IEnumerable<StockMovementDto>> GetItemMovementHistoryAsync(int itemId, int? storeId = null)
        {
            var query = _unitOfWork.StockMovements.Query()
                .Include(sm => sm.Store)
                .Where(sm => sm.ItemId == itemId && sm.IsActive);

            if (storeId.HasValue)
                query = query.Where(sm => sm.StoreId == storeId);

            var movements = await query
                .OrderByDescending(sm => sm.MovementDate)
                .ToListAsync();

            return movements.Select(sm => new StockMovementDto
            {
                Id = sm.Id,
                MovementType = sm.MovementType,
                MovementDate = sm.MovementDate,
                StoreName = sm.Store?.Name,
                Quantity = sm.Quantity ?? 0,
                OldBalance = sm.OldBalance,
                NewBalance = sm.NewBalance,
                ReferenceType = sm.ReferenceType,
                ReferenceNo = sm.ReferenceNo,
                Reason = sm.Reason,
                MovedBy = sm.MovedBy
            });
        }

        public async Task<IEnumerable<StockMovementDto>> GetStoreMovementHistoryAsync(int storeId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var movements = await _unitOfWork.StockMovements.Query()
                .Include(sm => sm.Item)
                .Where(sm => (sm.StoreId == storeId || sm.SourceStoreId == storeId || sm.DestinationStoreId == storeId)
                    && sm.MovementDate >= startDate
                    && sm.MovementDate < endDate
                    && sm.IsActive)
                .OrderBy(sm => sm.MovementDate)
                .ToListAsync();

            return movements.Select(sm => new StockMovementDto
            {
                Id = sm.Id,
                MovementType = sm.MovementType,
                MovementDate = sm.MovementDate,
                ItemId = sm.ItemId,
                ItemName = sm.Item?.Name,
                ItemCode = sm.Item?.ItemCode,
                Quantity = sm.Quantity ?? 0,
                OldBalance = sm.OldBalance,
                NewBalance = sm.NewBalance,
                ReferenceType = sm.ReferenceType,
                ReferenceNo = sm.ReferenceNo,
                MovedBy = sm.MovedBy
            });
        }

        public async Task<IEnumerable<StockMovementDto>> GetMovementsByReferenceAsync(string referenceType, string referenceNo)
        {
            var movements = await _unitOfWork.StockMovements.Query()
                .Include(sm => sm.Item)
                .Include(sm => sm.Store)
                .Where(sm => sm.ReferenceType == referenceType && sm.ReferenceNo == referenceNo && sm.IsActive)
                .OrderBy(sm => sm.Id)
                .ToListAsync();

            return movements.Select(sm => new StockMovementDto
            {
                Id = sm.Id,
                MovementType = sm.MovementType,
                MovementDate = sm.MovementDate,
                ItemId = sm.ItemId,
                ItemName = sm.Item?.Name,
                StoreId = sm.StoreId,
                StoreName = sm.Store?.Name,
                Quantity = sm.Quantity ?? 0,
                OldBalance = sm.OldBalance,
                NewBalance = sm.NewBalance
            });
        }

        public async Task<StockMovementSummaryDto> GetMovementSummaryAsync(int? storeId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var query = _unitOfWork.StockMovements.Query()
                .Where(sm => sm.MovementDate >= startDate && sm.MovementDate < endDate && sm.IsActive);

            if (storeId.HasValue)
                query = query.Where(sm => sm.StoreId == storeId);

            var movements = await query.ToListAsync();

            return new StockMovementSummaryDto
            {
                Date = date,
                StoreId = storeId,
                TotalInQuantity = movements.Where(m => m.MovementType == "IN").Sum(m => m.Quantity ?? 0),
                TotalOutQuantity = movements.Where(m => m.MovementType == "OUT").Sum(m => m.Quantity ?? 0),
                TotalInValue = movements.Where(m => m.MovementType == "IN").Sum(m => m.TotalValue ?? 0),
                TotalOutValue = movements.Where(m => m.MovementType == "OUT").Sum(m => m.TotalValue ?? 0),
                TransferInCount = movements.Count(m => m.MovementType == "TRANSFER" && m.DestinationStoreId == storeId),
                TransferOutCount = movements.Count(m => m.MovementType == "TRANSFER" && m.SourceStoreId == storeId),
                AdjustmentCount = movements.Count(m => m.MovementType == "ADJUSTMENT"),
                TotalMovements = movements.Count
            };
        }

        public async Task<decimal> GetStockBalanceAtDateAsync(int itemId, int storeId, DateTime date)
        {
            // Get all movements up to the specified date
            var movements = await _unitOfWork.StockMovements.Query()
                .Where(sm => sm.ItemId == itemId && sm.StoreId == storeId
                    && sm.MovementDate <= date && sm.IsActive)
                .OrderBy(sm => sm.MovementDate)
                .ThenBy(sm => sm.Id)
                .ToListAsync();

            // The last movement should have the balance at that date
            var lastMovement = movements.LastOrDefault();
            return lastMovement?.NewBalance ?? 0;
        }

        public async Task RecordMovementAsync(string movementType, int itemId, int? storeId, decimal quantity,
            string referenceType, string referenceNo, string remarks, string movedBy)
        {
            try
            {
                // Get current stock
                decimal oldBalance = 0;
                if (storeId.HasValue)
                {
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ItemId == itemId);
                    oldBalance = storeItem?.Quantity ?? 0;
                }

                decimal newBalance = movementType == "IN" ? oldBalance + quantity :
                                   movementType == "OUT" ? oldBalance - quantity : oldBalance;

                var movement = new StockMovement
                {
                    MovementType = movementType,
                    MovementDate = DateTime.Now,
                    ItemId = itemId,
                    StoreId = storeId,
                    Quantity = quantity,
                    OldBalance = oldBalance,
                    NewBalance = newBalance,
                    ReferenceType = referenceType,
                    ReferenceNo = referenceNo,
                    Remarks = remarks,
                    MovedBy = movedBy ?? _userContext.CurrentUserName,
                    CreatedBy = _userContext.CurrentUserName,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.StockMovements.AddAsync(movement);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording stock movement");
                throw;
            }
        }

        public async Task RecordTransferMovementAsync(int itemId, int fromStoreId, int toStoreId, decimal quantity,
            string transferNo, string movedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get current stock for both stores
                var fromStoreItem = await _unitOfWork.StoreItems
                    .FirstOrDefaultAsync(si => si.StoreId == fromStoreId && si.ItemId == itemId);
                var toStoreItem = await _unitOfWork.StoreItems
                    .FirstOrDefaultAsync(si => si.StoreId == toStoreId && si.ItemId == itemId);

                decimal fromOldBalance = fromStoreItem?.Quantity ?? 0;
                decimal toOldBalance = toStoreItem?.Quantity ?? 0;

                // Record OUT movement for source store
                var outMovement = new StockMovement
                {
                    MovementType = "TRANSFER",
                    MovementDate = DateTime.Now,
                    ItemId = itemId,
                    SourceStoreId = fromStoreId,
                    DestinationStoreId = toStoreId,
                    StoreId = fromStoreId,
                    Quantity = -quantity,
                    OldBalance = fromOldBalance,
                    NewBalance = fromOldBalance - quantity,
                    ReferenceType = "Transfer",
                    ReferenceNo = transferNo,
                    MovedBy = movedBy ?? _userContext.CurrentUserName,
                    CreatedBy = _userContext.CurrentUserName,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                // Record IN movement for destination store
                var inMovement = new StockMovement
                {
                    MovementType = "TRANSFER",
                    MovementDate = DateTime.Now,
                    ItemId = itemId,
                    SourceStoreId = fromStoreId,
                    DestinationStoreId = toStoreId,
                    StoreId = toStoreId,
                    Quantity = quantity,
                    OldBalance = toOldBalance,
                    NewBalance = toOldBalance + quantity,
                    ReferenceType = "Transfer",
                    ReferenceNo = transferNo,
                    MovedBy = movedBy ?? _userContext.CurrentUserName,
                    CreatedBy = _userContext.CurrentUserName,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.StockMovements.AddAsync(outMovement);
                await _unitOfWork.StockMovements.AddAsync(inMovement);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error recording transfer movement");
                throw;
            }
        }

        public async Task RecordAdjustmentMovementAsync(int itemId, int storeId, decimal oldQuantity, decimal newQuantity,
            string adjustmentNo, string reason, string adjustedBy)
        {
            var movement = new StockMovement
            {
                MovementType = "ADJUSTMENT",
                MovementDate = DateTime.Now,
                ItemId = itemId,
                StoreId = storeId,
                Quantity = newQuantity - oldQuantity,
                OldBalance = oldQuantity,
                NewBalance = newQuantity,
                ReferenceType = "Adjustment",
                ReferenceNo = adjustmentNo,
                Reason = reason,
                MovedBy = adjustedBy ?? _userContext.CurrentUserName,
                CreatedBy = _userContext.CurrentUserName,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            await _unitOfWork.StockMovements.AddAsync(movement);
            await _unitOfWork.CompleteAsync();
        }

        // Other methods implementation...
        public async Task<IEnumerable<StockMovementTrendDto>> GetMovementTrendsAsync(int? storeId, DateTime fromDate, DateTime toDate)
        {
            // Implementation for movement trends
            return new List<StockMovementTrendDto>();
        }

        public async Task<Dictionary<string, int>> GetMovementTypeCountsAsync(int? storeId, DateTime fromDate, DateTime toDate)
        {
            var query = _unitOfWork.StockMovements.Query()
                .Where(sm => sm.MovementDate >= fromDate && sm.MovementDate <= toDate && sm.IsActive);

            if (storeId.HasValue)
                query = query.Where(sm => sm.StoreId == storeId);

            var counts = await query
                .GroupBy(sm => sm.MovementType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);

            return counts;
        }

        public async Task<StockCardDto> GetStockCardAsync(int itemId, int storeId, DateTime fromDate, DateTime toDate)
        {
            // Implementation for stock card
            return new StockCardDto();
        }

        public async Task<IEnumerable<StockLedgerDto>> GetStockLedgerAsync(int? storeId, DateTime fromDate, DateTime toDate)
        {
            // Implementation for stock ledger
            return new List<StockLedgerDto>();
        }

        public async Task<byte[]> ExportMovementsAsync(int? storeId, int? itemId, string movementType,
            DateTime fromDate, DateTime toDate, string format = "excel")
        {
            // Implementation for export
            return new byte[0];
        }

        public async Task<byte[]> GenerateStockCardReportAsync(int itemId, int storeId, DateTime fromDate, DateTime toDate)
        {
            // Implementation for stock card report
            return new byte[0];
        }

        public async Task<bool> ValidateMovementAsync(StockMovementDto dto)
        {
            // Basic validation
            if (dto.ItemId <= 0)
                return false;

            if (dto.Quantity <= 0)
                return false;

            if (string.IsNullOrEmpty(dto.MovementType))
                return false;

            return true;
        }

        public async Task<IEnumerable<string>> GetMovementTypesAsync()
        {
            return await Task.FromResult(new[]
            {
                "IN", "OUT", "TRANSFER", "ADJUSTMENT", "PHYSICAL_COUNT", "RETURN", "WRITE_OFF"
            });
        }
    }
}