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
    public class BatchTrackingService : IBatchTrackingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly ILogger<BatchTrackingService> _logger;

        public BatchTrackingService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            ILogger<BatchTrackingService> logger)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _logger = logger;
        }

        // Create Batch
        public async Task<BatchTrackingDto> CreateBatchAsync(BatchTrackingDto dto)
        {
            try
            {
                // Check if batch number already exists
                var existingBatch = await _unitOfWork.BatchTrackings
                    .FirstOrDefaultAsync(bt => bt.BatchNumber == dto.BatchNo && bt.ItemId == dto.ItemId);

                if (existingBatch != null)
                    throw new InvalidOperationException($"Batch {dto.BatchNo} already exists for this item");

                var batch = new BatchTracking
                {
                    BatchNumber = dto.BatchNo,
                    ItemId = dto.ItemId,
                    StoreId = dto.StoreId,
                    ReceivedDate = dto.ReceivedDate,
                    ReceivedQuantity = dto.ReceivedQuantity,
                    RemainingQuantity = dto.ReceivedQuantity,
                    ManufactureDate = dto.ManufactureDate,
                    ExpiryDate = dto.ExpiryDate,
                    VendorId = dto.VendorId,
                    VendorBatchNo = dto.VendorBatchNo,
                    PurchaseOrderNo = dto.PurchaseOrderNo,
                    UnitCost = dto.UnitCost,
                    Status = BatchStatus.Active.ToString(),
                    QualityCheckStatus = dto.QualityCheckStatus,
                    QualityCheckDate = dto.QualityCheckDate,
                    QualityCheckBy = dto.QualityCheckBy,
                    StorageLocation = dto.StorageLocation,
                    Temperature = dto.Temperature,
                    Humidity = dto.Humidity,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.BatchTrackings.AddAsync(batch);
                await _unitOfWork.CompleteAsync();

                // Create batch movement record
                await CreateBatchMovementAsync(new BatchMovementDto
                {
                    BatchId = batch.Id,
                    MovementType = "Receive",
                    Quantity = dto.ReceivedQuantity,
                    ReferenceNo = dto.PurchaseOrderNo,
                    MovementDate = dto.ReceivedDate,
                    CreatedBy = dto.CreatedBy
                });

                dto.Id = batch.Id;
                _logger.LogInformation($"Batch {batch.BatchNumber} created for item {dto.ItemId}");

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating batch");
                throw;
            }
        }

        // Get Batches for Stock Rotation (FIFO/LIFO/FEFO)
        public async Task<IEnumerable<BatchTrackingDto>> GetBatchesForIssueAsync(
            int storeId, int itemId, decimal requiredQuantity, StockRotationMethod method)
        {
            try
            {
                var query = _unitOfWork.BatchTrackings.Query()
                    .Include(bt => bt.Item)
                    .Where(bt => bt.StoreId == storeId &&
                                bt.ItemId == itemId &&
                                bt.RemainingQuantity > 0 &&
                                bt.Status == BatchStatus.Active.ToString());

                // Apply rotation method
                IQueryable<BatchTracking> orderedBatches = method switch
                {
                    StockRotationMethod.FIFO => query.OrderBy(bt => bt.ReceivedDate),
                    StockRotationMethod.LIFO => query.OrderByDescending(bt => bt.ReceivedDate),
                    StockRotationMethod.FEFO => query.OrderBy(bt => bt.ExpiryDate ?? DateTime.MaxValue),
                    _ => query.OrderBy(bt => bt.ReceivedDate)
                };

                var batches = await orderedBatches.ToListAsync();

                var selectedBatches = new List<BatchTrackingDto>();
                decimal totalQuantity = 0;

                foreach (var batch in batches)
                {
                    if (totalQuantity >= requiredQuantity)
                        break;

                    var quantityToTake = Math.Min(batch.RemainingQuantity ?? 0m, requiredQuantity - totalQuantity);

                    selectedBatches.Add(new BatchTrackingDto
                    {
                        Id = batch.Id,
                        BatchNo = batch.BatchNumber,
                        ItemId = batch.ItemId,
                        ItemName = batch.Item?.Name,
                        RemainingQuantity = batch.RemainingQuantity,
                        SuggestedQuantity = quantityToTake,
                        ManufactureDate = batch.ManufactureDate,
                        ExpiryDate = batch.ExpiryDate,
                        DaysToExpiry = batch.ExpiryDate.HasValue
                            ? (batch.ExpiryDate.Value - DateTime.Now).Days
                            : int.MaxValue,
                        UnitCost = batch.UnitCost,
                        StorageLocation = batch.StorageLocation
                    });

                    totalQuantity += quantityToTake;
                }

                if (totalQuantity < requiredQuantity)
                {
                    _logger.LogWarning($"Insufficient batch quantity. Required: {requiredQuantity}, Available: {totalQuantity}");
                }

                return selectedBatches;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting batches for issue: Store {storeId}, Item {itemId}");
                throw;
            }
        }

        // Issue from Batch
        public async Task<bool> IssueFromBatchAsync(int batchId, decimal quantity, string reference, string issuedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var batch = await _unitOfWork.BatchTrackings
                    .FirstOrDefaultAsync(bt => bt.Id == batchId);

                if (batch == null)
                    throw new InvalidOperationException("Batch not found");

                if (batch.RemainingQuantity < quantity)
                    throw new InvalidOperationException($"Insufficient quantity in batch. Available: {batch.RemainingQuantity}");

                // Check expiry
                if (batch.ExpiryDate.HasValue && batch.ExpiryDate.Value <= DateTime.Now)
                {
                    throw new InvalidOperationException($"Batch {batch.BatchNumber} has expired");
                }

                // Update batch quantity
                batch.RemainingQuantity -= quantity;
                batch.LastIssueDate = DateTime.Now;
                batch.UpdatedAt = DateTime.Now;

                if (batch.RemainingQuantity == 0)
                {
                    batch.Status = BatchStatus.Consumed.ToString();
                    batch.ConsumedDate = DateTime.Now;
                }

                // Create batch movement
                await CreateBatchMovementAsync(new BatchMovementDto
                {
                    BatchId = batchId,
                    MovementType = "Issue",
                    Quantity = -quantity,
                    OldBalance = (batch.RemainingQuantity ?? 0m) + quantity,
                    NewBalance = batch.RemainingQuantity ?? 0m,
                    ReferenceNo = reference,
                    MovementDate = DateTime.Now,
                    CreatedBy = issuedBy
                });

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Issued {quantity} units from batch {batch.BatchNumber}");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error issuing from batch {batchId}");
                throw;
            }
        }

        // Get Expiring Batches
        public async Task<IEnumerable<ExpiringBatchDto>> GetExpiringBatchesAsync(int storeId, int daysBeforeExpiry = 30)
        {
            try
            {
                var expiryDate = DateTime.Now.AddDays(daysBeforeExpiry);

                var expiringBatches = await _unitOfWork.BatchTrackings.Query()
                    .Include(bt => bt.Item)
                    .Where(bt => bt.StoreId == storeId &&
                                bt.ExpiryDate != null &&
                                bt.ExpiryDate <= expiryDate &&
                                bt.RemainingQuantity > 0 &&
                                bt.Status == BatchStatus.Active.ToString())
                    .OrderBy(bt => bt.ExpiryDate)
                    .Select(bt => new ExpiringBatchDto
                    {
                        BatchId = bt.Id,
                        BatchNo = bt.BatchNumber,
                        ItemId = bt.ItemId,
                        ItemName = bt.Item.Name,
                        ItemCode = bt.Item.ItemCode,
                        ExpiryDate = bt.ExpiryDate.Value,
                        DaysToExpiry = (bt.ExpiryDate.Value - DateTime.Now).Days,
                        RemainingQuantity = bt.RemainingQuantity,
                        EstimatedValue = bt.RemainingQuantity * bt.UnitCost,
                        StorageLocation = bt.StorageLocation,
                        AlertLevel = (bt.ExpiryDate.Value - DateTime.Now).Days <= 7 ? "Critical" :
                                    (bt.ExpiryDate.Value - DateTime.Now).Days <= 15 ? "High" : "Medium",
                        RecommendedAction = GetRecommendedAction(bt.ExpiryDate.Value, bt.RemainingQuantity)
                    })
                    .ToListAsync();

                return expiringBatches;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting expiring batches for store {storeId}");
                throw;
            }
        }

        // Transfer Batch Between Stores
        public async Task<bool> TransferBatchAsync(TransferBatchDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var sourceBatch = await _unitOfWork.BatchTrackings
                    .FirstOrDefaultAsync(bt => bt.Id == dto.SourceBatchId);

                if (sourceBatch == null)
                    throw new InvalidOperationException("Source batch not found");

                if (sourceBatch.RemainingQuantity < dto.TransferQuantity)
                    throw new InvalidOperationException("Insufficient quantity in source batch");

                // Create new batch in destination store or update existing
                var destBatch = await _unitOfWork.BatchTrackings
                    .FirstOrDefaultAsync(bt => bt.BatchNumber == sourceBatch.BatchNumber &&
                                               bt.StoreId == dto.DestinationStoreId &&
                                               bt.ItemId == sourceBatch.ItemId);

                if (destBatch == null)
                {
                    // Create new batch in destination
                    destBatch = new BatchTracking
                    {
                        BatchNumber = sourceBatch.BatchNumber,
                        ItemId = sourceBatch.ItemId,
                        StoreId = dto.DestinationStoreId,
                        ReceivedDate = DateTime.Now,
                        ReceivedQuantity = dto.TransferQuantity,
                        RemainingQuantity = dto.TransferQuantity,
                        ManufactureDate = sourceBatch.ManufactureDate,
                        ExpiryDate = sourceBatch.ExpiryDate,
                        VendorId = sourceBatch.VendorId,
                        VendorBatchNo = sourceBatch.VendorBatchNo,
                        UnitCost = sourceBatch.UnitCost,
                        Status = BatchStatus.Active.ToString(),
                        TransferredFromBatchId = sourceBatch.Id,
                        TransferReference = dto.TransferReference,
                        CreatedAt = DateTime.Now
                    };

                    await _unitOfWork.BatchTrackings.AddAsync(destBatch);
                }
                else
                {
                    // Update existing batch
                    destBatch.ReceivedQuantity += dto.TransferQuantity;
                    destBatch.RemainingQuantity += dto.TransferQuantity;
                    destBatch.UpdatedAt = DateTime.Now;
                }

                // Reduce source batch quantity
                sourceBatch.RemainingQuantity -= dto.TransferQuantity;
                sourceBatch.UpdatedAt = DateTime.Now;

                if (sourceBatch.RemainingQuantity == 0)
                {
                    sourceBatch.Status = BatchStatus.Transferred.ToString();
                }

                // Create batch movements
                await CreateBatchMovementAsync(new BatchMovementDto
                {
                    BatchId = sourceBatch.Id,
                    MovementType = "TransferOut",
                    Quantity = -dto.TransferQuantity,
                    ReferenceNo = dto.TransferReference,
                    MovementDate = DateTime.Now,
                    CreatedBy = dto.TransferredBy
                });

                await _unitOfWork.CompleteAsync();

                await CreateBatchMovementAsync(new BatchMovementDto
                {
                    BatchId = destBatch.Id,
                    MovementType = "TransferIn",
                    Quantity = dto.TransferQuantity,
                    ReferenceNo = dto.TransferReference,
                    MovementDate = DateTime.Now,
                    CreatedBy = dto.TransferredBy
                });

                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"Transferred {dto.TransferQuantity} units of batch {sourceBatch.BatchNumber}");
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error transferring batch");
                throw;
            }
        }

        // Quarantine Batch
        public async Task<bool> QuarantineBatchAsync(int batchId, string reason, string quarantinedBy)
        {
            try
            {
                var batch = await _unitOfWork.BatchTrackings
                    .FirstOrDefaultAsync(bt => bt.Id == batchId);

                if (batch == null)
                    throw new InvalidOperationException("Batch not found");

                batch.Status = BatchStatus.Quarantine.ToString();
                batch.QuarantineDate = DateTime.Now;
                batch.QuarantineReason = reason;
                batch.QuarantinedBy = quarantinedBy;
                batch.UpdatedAt = DateTime.Now;

                // Create batch movement
                await CreateBatchMovementAsync(new BatchMovementDto
                {
                    BatchId = batchId,
                    MovementType = "Quarantine",
                    Quantity = 0,
                    ReferenceNo = $"Quarantine: {reason}",
                    MovementDate = DateTime.Now,
                    CreatedBy = quarantinedBy
                });

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Batch {batch.BatchNumber} quarantined. Reason: {reason}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error quarantining batch {batchId}");
                throw;
            }
        }

        // Get Batch History
        public async Task<BatchHistoryDto> GetBatchHistoryAsync(string batchNo)
        {
            try
            {
                var batches = await _unitOfWork.BatchTrackings.Query()
                    .Include(bt => bt.Item)
                    .Include(bt => bt.Store)
                    .Where(bt => bt.BatchNumber == batchNo)
                    .ToListAsync();

                if (!batches.Any())
                    throw new InvalidOperationException($"Batch {batchNo} not found");

                var movements = await _unitOfWork.BatchMovements.Query()
                    .Where(bm => batches.Select(b => b.Id).Contains(bm.BatchId))
                    .OrderBy(bm => bm.MovementDate)
                    .ToListAsync();

                var history = new BatchHistoryDto
                {
                    BatchNo = batchNo,
                    ItemName = batches.First().Item?.Name,
                    ManufactureDate = batches.First().ManufactureDate,
                    ExpiryDate = batches.First().ExpiryDate,
                    TotalReceived = batches.Sum(b => b.ReceivedQuantity),
                    TotalRemaining = batches.Sum(b => b.RemainingQuantity ?? 0m),

                    Locations = batches.Select(b => new BatchLocationDto
                    {
                        StoreId = b.StoreId,
                        StoreName = b.Store?.Name,
                        Quantity = b.RemainingQuantity ?? 0m,
                        Status = b.Status.ToString(),
                        StorageLocation = b.StorageLocation
                    }).ToList(),

                    Movements = movements.Select(m => new BatchMovementHistoryDto
                    {
                        MovementDate = m.MovementDate,
                        MovementType = m.MovementType,
                        Quantity = m.Quantity,
                        NewBalance = m.BalanceAfter,
                        ReferenceNo = m.ReferenceNo,
                        CreatedBy = m.CreatedBy
                    }).ToList()
                };

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting batch history for {batchNo}");
                throw;
            }
        }

        // Batch Valuation
        public async Task<BatchValuationDto> GetBatchValuationAsync(int storeId, DateTime? asOfDate = null)
        {
            try
            {
                var date = asOfDate ?? DateTime.Now;

                var batches = await _unitOfWork.BatchTrackings.Query()
                    .Include(bt => bt.Item)
                    .Where(bt => bt.StoreId == storeId &&
                                bt.RemainingQuantity > 0 &&
                                bt.Status == BatchStatus.Active.ToString() &&
                                bt.ReceivedDate <= date)
                    .ToListAsync();

                var valuation = new BatchValuationDto
                {
                    StoreId = storeId,
                    ValuationDate = date,
                    TotalBatches = batches.Count,
                    TotalQuantity = batches.Sum(b => b.RemainingQuantity ?? 0m),

                    // FIFO Valuation
                    FIFOValue = CalculateFIFOValue(batches) ?? 0m,     
                    LIFOValue = CalculateLIFOValue(batches) ?? 0m,
                    WeightedAverageValue = CalculateWeightedAverageValue(batches) ?? 0m,

                    // By Category
                    CategoryValuation = batches
                        .GroupBy(b => b.Item?.SubCategory?.Category?.Name ?? "Uncategorized")
                        .Select(g => new CategoryBatchValuationDto
                        {
                            CategoryName = g.Key,
                            BatchCount = g.Count(),
                            TotalQuantity = g.Sum(b => b.RemainingQuantity ?? 0m),
                            TotalValue = g.Sum(b => (b.RemainingQuantity ?? 0m) * (b.UnitCost ?? 0m))
                        })
                        .ToList(),

                    // Aging Analysis
                    AgingAnalysis = new BatchAgingAnalysisDto
                    {
                        Under30Days = batches.Count(b => (date - b.ReceivedDate).TotalDays <= 30),
                        Between30And60Days = batches.Count(b => (date - b.ReceivedDate).TotalDays > 30 &&
                                                                (date - b.ReceivedDate).TotalDays <= 60),
                        Between60And90Days = batches.Count(b => (date - b.ReceivedDate).TotalDays > 60 &&
                                                                (date - b.ReceivedDate).TotalDays <= 90),
                        Over90Days = batches.Count(b => (date - b.ReceivedDate).TotalDays > 90)
                    }
                };

                return valuation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating batch valuation for store {storeId}");
                throw;
            }
        }

        // Helper Methods
        private async Task CreateBatchMovementAsync(BatchMovementDto dto)
        {
            var movement = new BatchMovement
            {
                BatchId = dto.BatchId,
                MovementType = dto.MovementType,
                Quantity = dto.Quantity,
                OldBalance = dto.BalanceBefore,
                NewBalance = dto.BalanceAfter,
                ReferenceNo = dto.ReferenceNo,
                MovementDate = dto.MovementDate,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.BatchMovements.AddAsync(movement);
        }
        private string GetRecommendedAction(DateTime expiryDate, decimal? quantity)
        {
            var daysToExpiry = (expiryDate - DateTime.Now).Days;

            if (daysToExpiry <= 7)
                return "Urgent: Issue immediately or prepare for write-off";
            else if (daysToExpiry <= 15)
                return "Priority issue required";
            else if (daysToExpiry <= 30)
                return "Plan for early issue";
            else
                return "Monitor expiry date";
        }
        private decimal? CalculateFIFOValue(List<BatchTracking> batches)
        {
            return batches
                .OrderBy(b => b.ReceivedDate)
                .Sum(b => (b.RemainingQuantity ?? 0m) * (b.UnitCost ?? 0m));
        }
        private decimal? CalculateLIFOValue(List<BatchTracking> batches)
        {
            return batches
                .OrderByDescending(b => b.ReceivedDate)
                .Sum(b => (b.RemainingQuantity ?? 0m) * (b.UnitCost ?? 0m));
        }
        private decimal? CalculateWeightedAverageValue(List<BatchTracking> batches)
        {
            var totalQuantity = batches.Sum(b => b.RemainingQuantity ?? 0m);
            if (totalQuantity == 0) return 0;

            var weightedCost = batches.Sum(b => (b.RemainingQuantity ?? 0m) * (b.UnitCost ?? 0m)) / totalQuantity;
            return totalQuantity * weightedCost;
        }

        public async Task<BatchDto> CreateBatchAsync(BatchDto dto)
        {
            try
            {
                var batch = new BatchTracking
                {
                    BatchNumber = dto.BatchNumber ?? await GenerateBatchNumberAsync(),
                    ItemId = dto.ItemId,
                    StoreId = dto.StoreId,
                    Quantity = dto.Quantity,
                    InitialQuantity = dto.Quantity,
                    ManufactureDate = dto.ManufactureDate,
                    ExpiryDate = dto.ExpiryDate,
                    SupplierBatchNo = dto.SupplierBatchNo,
                    CostPrice = dto.CostPrice,
                    SellingPrice = dto.SellingPrice,
                    Location = dto.Location,
                    Status = "Active",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.BatchTrackings.AddAsync(batch);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Batch {batch.BatchNumber} created for item {dto.ItemId}");

                return await GetBatchByIdAsync(batch.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating batch");
                throw;
            }
        }
        public async Task<StockAllocationDto> AllocateStockAsync(int itemId, int storeId, decimal requiredQuantity, string rotationMethod = "FIFO")
        {
            try
            {
                var batches = await GetAvailableBatchesAsync(itemId, storeId);

                // Sort batches based on rotation method
                IEnumerable<BatchTracking> sortedBatches = rotationMethod.ToUpper() switch
                {
                    "FIFO" => batches.OrderBy(b => b.CreatedAt),           // First In First Out
                    "LIFO" => batches.OrderByDescending(b => b.CreatedAt), // Last In First Out
                    "FEFO" => batches.OrderBy(b => b.ExpiryDate ?? DateTime.MaxValue), // First Expired First Out
                    "HCFO" => batches.OrderByDescending(b => b.CostPrice), // Highest Cost First Out
                    "LCFO" => batches.OrderBy(b => b.CostPrice),          // Lowest Cost First Out
                    _ => batches.OrderBy(b => b.CreatedAt)
                };

                var allocations = new List<BatchAllocationDto>();
                decimal remainingQuantity = requiredQuantity;

                foreach (var batch in sortedBatches)
                {
                    if (remainingQuantity <= 0)
                        break;

                    // Skip expired batches for FEFO
                    if (rotationMethod == "FEFO" && batch.ExpiryDate.HasValue && batch.ExpiryDate.Value < DateTime.Now)
                        continue;

                    decimal allocateQty = Math.Min(batch.Quantity, remainingQuantity);

                    allocations.Add(new BatchAllocationDto
                    {
                        BatchId = batch.Id,
                        BatchNumber = batch.BatchNumber,
                        AllocatedQuantity = allocateQty,
                        RemainingInBatch = batch.Quantity - allocateQty,
                        ExpiryDate = batch.ExpiryDate,
                        CostPrice = batch.CostPrice,
                        Location = batch.Location,
                        Priority = GetAllocationPriority(batch, rotationMethod)
                    });

                    remainingQuantity -= allocateQty;
                }

                return new StockAllocationDto
                {
                    ItemId = itemId,
                    StoreId = storeId,
                    RequestedQuantity = requiredQuantity,
                    AllocatedQuantity = requiredQuantity - remainingQuantity,
                    RotationMethod = rotationMethod,
                    Allocations = allocations,
                    IsFullyAllocated = remainingQuantity <= 0,
                    ShortageQuantity = remainingQuantity > 0 ? remainingQuantity : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error allocating stock for item {itemId}");
                throw;
            }
        }
        public async Task<bool> ConsumeBatchQuantityAsync(string batchNumber, decimal quantity)
        {
            try
            {
                var batch = await _unitOfWork.BatchTrackings
                    .FirstOrDefaultAsync(b => b.BatchNumber == batchNumber);

                if (batch == null)
                    throw new InvalidOperationException($"Batch {batchNumber} not found");

                if (batch.Quantity < quantity)
                    throw new InvalidOperationException($"Insufficient quantity in batch {batchNumber}");

                batch.Quantity -= quantity;
                batch.UpdatedAt = DateTime.Now;
                batch.UpdatedBy = _userContext.CurrentUserName;

                // Update status if fully consumed
                if (batch.Quantity <= 0)
                {
                    batch.Status = "Consumed";
                    batch.ConsumedDate = DateTime.Now;
                }

                _unitOfWork.BatchTrackings.Update(batch);

                // Create batch movement record
                var movement = new BatchMovement
                {
                    BatchId = batch.Id,
                    MovementType = "OUT",
                    Quantity = quantity,
                    NewBalance = batch.Quantity,
                    MovementDate = DateTime.Now,
                    Remarks = "Stock consumption",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.BatchMovements.AddAsync(movement);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error consuming batch {batchNumber}");
                throw;
            }
        }
        public async Task<IEnumerable<BatchDto>> GetExpiringBatchesAsync(int daysBeforeExpiry = 30)
        {
            var expiryDate = DateTime.Now.AddDays(daysBeforeExpiry);

            var batches = await _unitOfWork.BatchTrackings
                .FindAsync(b => b.IsActive &&
                          b.Status == "Active" &&
                          b.ExpiryDate.HasValue &&
                          b.ExpiryDate.Value <= expiryDate);

            return batches.Select(b => new BatchDto
            {
                Id = b.Id,
                BatchNumber = b.BatchNumber,
                ItemId = b.ItemId,
                ItemName = b.Item?.Name,
                StoreId = b.StoreId,
                StoreName = b.Store?.Name,
                Quantity = b.Quantity,
                ExpiryDate = b.ExpiryDate,
                DaysUntilExpiry = b.ExpiryDate.HasValue ?
                    (b.ExpiryDate.Value - DateTime.Now).Days : 0,
                Status = b.Status,
                Location = b.Location
            }).OrderBy(b => b.ExpiryDate);
        }
        public async Task<BatchTraceabilityDto> GetBatchTraceabilityAsync(string batchNumber)
        {
            var batch = await _unitOfWork.BatchTrackings
                .FirstOrDefaultAsync(b => b.BatchNumber == batchNumber);

            if (batch == null)
                return null;

            var movements = await _unitOfWork.BatchMovements
                .FindAsync(m => m.BatchId == batch.Id);

            var issues = await _unitOfWork.IssueItems
                .FindAsync(i => i.BatchNumber == batchNumber);

            var receives = await _unitOfWork.ReceiveItems
                .FindAsync(r => r.BatchNumber == batchNumber);

            return new BatchTraceabilityDto
            {
                BatchNumber = batch.BatchNumber,
                ItemId = batch.ItemId,
                ItemName = batch.Item?.Name,
                InitialQuantity = batch.InitialQuantity,
                CurrentQuantity = batch.Quantity,
                ManufactureDate = batch.ManufactureDate,
                ExpiryDate = batch.ExpiryDate,
                SupplierBatchNo = batch.SupplierBatchNo,

                Movements = movements.Select(m => new BatchMovementDto
                {
                    MovementType = m.MovementType,
                    Quantity = m.Quantity,
                    NewBalance = m.BalanceAfter,
                    MovementDate = m.MovementDate,
                    ReferenceType = m.ReferenceType,
                    ReferenceNo = m.ReferenceNo,
                    Remarks = m.Remarks
                }).OrderByDescending(m => m.MovementDate).ToList(),

                IssueHistory = issues.Select(i => new TransactionHistoryDto
                {
                    TransactionType = "Issue",
                    TransactionNo = i.Issue?.IssueNo,
                    TransactionDate = i.Issue?.IssueDate ?? DateTime.MinValue,
                    Quantity = i.Quantity,
                    ToLocation = i.Issue?.IssuedTo
                }).ToList(),

                ReceiveHistory = receives.Select(r => new TransactionHistoryDto
                {
                    TransactionType = "Receive",
                    TransactionNo = r.Receive?.ReceiveNo,
                    TransactionDate = r.Receive?.ReceiveDate ?? DateTime.MinValue,
                    Quantity = r.Quantity,
                    FromLocation = r.Receive?.Source
                }).ToList()
            };
        }
        public async Task<StockRotationReportDto> GetStockRotationAnalysisAsync(int storeId, string rotationMethod)
        {
            var batches = await _unitOfWork.BatchTrackings
                .FindAsync(b => b.StoreId == storeId && b.IsActive && b.Status == "Active");

            var groupedByItem = batches.GroupBy(b => b.ItemId);
            var analysis = new List<ItemRotationAnalysisDto>();

            foreach (var itemGroup in groupedByItem)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(itemGroup.Key);
                var itemBatches = itemGroup.ToList();

                // Calculate rotation metrics
                var oldestBatch = itemBatches.OrderBy(b => b.CreatedAt).FirstOrDefault();
                var newestBatch = itemBatches.OrderByDescending(b => b.CreatedAt).FirstOrDefault();
                var totalValue = itemBatches.Sum(b => b.Quantity * b.CostPrice);
                var expiringCount = itemBatches.Count(b => b.ExpiryDate.HasValue &&
                    (b.ExpiryDate.Value - DateTime.Now).Days < 30);

                analysis.Add(new ItemRotationAnalysisDto
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    TotalBatches = itemBatches.Count,
                    TotalQuantity = itemBatches.Sum(b => b.Quantity),
                    TotalValue = totalValue,
                    OldestBatchDate = oldestBatch?.CreatedAt,
                    OldestBatchAge = oldestBatch != null ?
                        (DateTime.Now - oldestBatch.CreatedAt).Days : 0,
                    ExpiringBatchCount = expiringCount,
                    RecommendedAction = GetRotationRecommendation(itemBatches, rotationMethod)
                });
            }

            return new StockRotationReportDto
            {
                StoreId = storeId,
                RotationMethod = rotationMethod,
                GeneratedAt = DateTime.Now,
                ItemAnalysis = analysis,
                TotalItems = analysis.Count,
                TotalValue = analysis.Sum(a => a.TotalValue),
                ItemsNeedingAttention = analysis.Count(a => a.ExpiringBatchCount > 0 || a.OldestBatchAge > 180)
            };
        }

        // Helper methods
        private async Task<string> GenerateBatchNumberAsync()
        {
            var prefix = "BTH";
            var date = DateTime.Now.ToString("yyMMdd");
            var random = new Random().Next(1000, 9999);

            return $"{prefix}{date}{random}";
        }
        private async Task<IEnumerable<BatchTracking>> GetAvailableBatchesAsync(int itemId, int storeId)
        {
            return await _unitOfWork.BatchTrackings
                .FindAsync(b => b.ItemId == itemId &&
                          b.StoreId == storeId &&
                          b.IsActive &&
                          b.Status == "Active" &&
                          b.Quantity > 0);
        }
        private string GetAllocationPriority(BatchTracking batch, string rotationMethod)
        {
            if (rotationMethod == "FEFO" && batch.ExpiryDate.HasValue)
            {
                var daysUntilExpiry = (batch.ExpiryDate.Value - DateTime.Now).Days;
                if (daysUntilExpiry < 7) return "Critical";
                if (daysUntilExpiry < 30) return "High";
                if (daysUntilExpiry < 90) return "Medium";
            }

            return "Normal";
        }
        private string GetRotationRecommendation(List<BatchTracking> batches, string rotationMethod)
        {
            var hasExpiring = batches.Any(b => b.ExpiryDate.HasValue &&
                (b.ExpiryDate.Value - DateTime.Now).Days < 30);

            if (hasExpiring)
                return "Prioritize expiring batches";

            var oldestAge = batches.Min(b => (DateTime.Now - b.CreatedAt).Days);
            if (oldestAge > 180)
                return "Consider promoting old stock";

            return "Stock rotation optimal";
        }
        public async Task<BatchDto> GetBatchByIdAsync(int id)
        {
            var batch = await _unitOfWork.BatchTrackings
                .GetAsync(b => b.Id == id, includes: new[] { "Item", "Store" });

            if (batch == null)
                return null;

            return new BatchDto
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                ItemId = batch.ItemId,
                ItemName = batch.Item?.Name,
                StoreId = batch.StoreId,
                StoreName = batch.Store?.Name,
                Quantity = batch.Quantity,
                InitialQuantity = batch.InitialQuantity,
                ManufactureDate = batch.ManufactureDate,
                ExpiryDate = batch.ExpiryDate,
                SupplierBatchNo = batch.SupplierBatchNo,
                CostPrice = batch.CostPrice,
                SellingPrice = batch.SellingPrice,
                Location = batch.Location,
                Status = batch.Status
            };
        }
        public Task<IEnumerable<BatchDto>> GetBatchesByItemAsync(int? itemId)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateBatchExpiryAsync(string batchNumber, DateTime newExpiryDate)
        {
            throw new NotImplementedException();
        }


        public async Task<int> GetExpiringCountAsync(int daysToExpire = 30)
        {
            var expiryDate = DateTime.Today.AddDays(daysToExpire);

            var batches = await _unitOfWork.BatchTrackings.FindAsync(b =>
                b.ExpiryDate.HasValue &&
                b.ExpiryDate.Value <= expiryDate &&
                b.Status == "Active" &&
                b.CurrentQuantity > 0);

            return batches.Count();
        }
    }
}