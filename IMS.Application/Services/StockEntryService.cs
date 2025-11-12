using CsvHelper;
using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class StockEntryService : IStockEntryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBarcodeService _barcodeService;
        private readonly IUserContext _userContext;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StockEntryService> _logger;

        public StockEntryService(
            IUnitOfWork unitOfWork,
            IBarcodeService barcodeService,
            IUserContext userContext,
            IActivityLogService activityLogService,
            INotificationService notificationService,
            ILogger<StockEntryService> logger)
        {
            _unitOfWork = unitOfWork;
            _barcodeService = barcodeService;
            _userContext = userContext;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<string> GenerateEntryNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"SE{date:yyyyMMdd}";

            var lastEntry = await _unitOfWork.StockEntries
                .FindAsync(e => e.EntryNo.StartsWith(prefix));

            var sequence = 1;
            if (lastEntry.Any())
            {
                var lastNo = lastEntry.OrderByDescending(e => e.EntryNo).First().EntryNo;
                var lastSequence = lastNo.Substring(prefix.Length);
                if (int.TryParse(lastSequence, out var seq))
                    sequence = seq + 1;
            }

            return $"{prefix}{sequence:D4}";
        }

        public async Task<IEnumerable<StockLevelReportDto>> GetStockLevelsAsync(int? storeId = null, bool lowStockOnly = false)
        {
            try
            {
                var query = _unitOfWork.StoreItems.Query()
                    .Include(si => si.Store)
                    .Include(si => si.Item)
                        .ThenInclude(i => i.SubCategory)
                            .ThenInclude(sc => sc.Category)
                    .Where(si => si.IsActive);

                if (storeId.HasValue)
                    query = query.Where(si => si.StoreId == storeId.Value);

                if (lowStockOnly)
                    query = query.Where(si => si.Quantity <= (si.Item.MinimumStock ?? 0));

                var storeItems = await query.ToListAsync();

                return storeItems.Select(si => new StockLevelReportDto
                {
                    ItemId = si.ItemId,
                    StoreId = si.StoreId,
                    ItemCode = si.Item?.ItemCode ?? si.Item?.Code ?? "N/A",
                    ItemName = si.Item?.Name ?? "Unknown Item",
                    CategoryName = si.Item?.SubCategory?.Category?.Name ?? "Uncategorized",
                    SubCategoryName = si.Item?.SubCategory?.Name ?? "",
                    StoreName = si.Store?.Name ?? "Unknown Store",
                    CurrentStock = (decimal)si.Quantity,
                    MinimumStock = si.Item?.MinimumStock,
                    ReorderLevel = si.Item?.ReorderLevel,
                    MaximumStock = si.Item?.MaximumStock,
                    Unit = si.Item?.Unit ?? "Unit",
                    UnitPrice = si.Item?.UnitPrice ?? 0,
                    StockValue = (decimal)(si.Quantity * (si.Item?.UnitPrice ?? 0)),
                    StockStatus = GetStockStatus((decimal)si.Quantity, si.Item?.MinimumStock ?? 0, si.Item?.MaximumStock ?? 0)
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock levels");
                return new List<StockLevelReportDto>();
            }
        }

        private string GetStockStatus(decimal currentStock, decimal minimumStock, decimal maximumStock)
        {
            if (currentStock <= 0)
                return "OutOfStock";

            if (minimumStock > 0 && currentStock <= minimumStock)
                return "LowStock";

            if (maximumStock > 0 && currentStock >= maximumStock)
                return "Overstock";

            return "InStock";
        }
        public async Task<StockEntryDto> CreateStockEntryAsync(StockEntryDto dto)
        {
            try
            {
                _logger.LogInformation($"Starting stock entry creation for Store ID: {dto.StoreId}");

                // Validate store exists
                var store = await _unitOfWork.Stores.GetByIdAsync(dto.StoreId);
                if (store == null)
                {
                    throw new InvalidOperationException($"Store with ID {dto.StoreId} not found");
                }

                await _unitOfWork.BeginTransactionAsync();

                // Create stock entry
                var entry = new StockEntry
                {
                    EntryNo = await GenerateEntryNoAsync(),
                    EntryDate = dto.EntryDate,
                    StoreId = dto.StoreId,
                    EntryType = dto.EntryType ?? "Direct",
                    Remarks = dto.Remarks,
                    Status = "Draft",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                _logger.LogInformation($"Creating stock entry with EntryNo: {entry.EntryNo}");
                await _unitOfWork.StockEntries.AddAsync(entry);
                await _unitOfWork.CompleteAsync();

                // Process each item
                int itemIndex = 0;
                foreach (var itemDto in dto.Items)
                {
                    itemIndex++;
                    _logger.LogInformation($"Processing item {itemIndex}/{dto.Items.Count} - ItemId: {itemDto.ItemId}, Quantity: {itemDto.Quantity}");

                    // Declare item variable outside try block so it's accessible in catch
                    Item item = null;

                    try
                    {
                        // Validate item exists
                        item = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                        if (item == null)
                        {
                            var errorMsg = $"Item with ID {itemDto.ItemId} not found (Item #{itemIndex})";
                            _logger.LogError(errorMsg);
                            throw new InvalidOperationException(errorMsg);
                        }

                        // Generate batch number if not provided
                        if (string.IsNullOrEmpty(itemDto.BatchNumber))
                        {
                            itemDto.BatchNumber = await GenerateBatchNumberAsync();
                            _logger.LogDebug($"Generated batch number: {itemDto.BatchNumber}");
                        }

                        // Add stock entry item
                        var entryItem = new StockEntryItem
                        {
                            StockEntryId = entry.Id,
                            ItemId = itemDto.ItemId,
                            Quantity = itemDto.Quantity,
                            Location = itemDto.Location,
                            BatchNumber = itemDto.BatchNumber,
                            ExpiryDate = itemDto.ExpiryDate,
                            UnitCost = itemDto.UnitCost,
                            BarcodesGenerated = 0,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userContext.CurrentUserName,
                            IsActive = true
                        };

                        await _unitOfWork.StockEntryItems.AddAsync(entryItem);
                        await _unitOfWork.CompleteAsync();
                        _logger.LogDebug($"Stock entry item added for ItemId: {itemDto.ItemId}");

                        // CRITICAL FIX: Stock is NOT updated here anymore
                        // Stock will only be updated when entry is APPROVED in ApproveStockEntryAsync()
                        // This prevents phantom inventory from draft/rejected entries
                        _logger.LogInformation($"Stock entry item created (stock NOT updated yet). ItemId: {itemDto.ItemId}, Qty: {itemDto.Quantity}");

                        // Generate barcodes if requested
                        if (itemDto.GenerateBarcodes)
                        {
                            var barcodesToGenerate = Math.Max(1, itemDto.BarcodesToGenerate);
                            var generatedBarcodes = new List<string>();
                            _logger.LogInformation($"Generating {barcodesToGenerate} barcodes for ItemId: {itemDto.ItemId}");

                            for (int i = 0; i < barcodesToGenerate; i++)
                            {
                                try
                                {
                                    var barcodeDto = new BarcodeDto
                                    {
                                        ItemId = itemDto.ItemId,
                                        StoreId = dto.StoreId,
                                        Location = itemDto.Location,
                                        BatchNumber = entryItem.BatchNumber,
                                        Notes = $"Stock Entry: {entry.EntryNo}",
                                        GeneratedBy = _userContext.CurrentUserName,
                                        GeneratedDate = DateTime.Now
                                    };

                                    var createdBarcode = await _barcodeService.CreateBarcodeAsync(barcodeDto);
                                    generatedBarcodes.Add(createdBarcode.BarcodeNumber);
                                }
                                catch (Exception bcEx)
                                {
                                    _logger.LogError(bcEx, $"Error generating barcode {i + 1}/{barcodesToGenerate} for ItemId: {itemDto.ItemId}");
                                    // Continue with other barcodes instead of failing completely
                                }
                            }

                            entryItem.BarcodesGenerated = generatedBarcodes.Count;
                            _unitOfWork.StockEntryItems.Update(entryItem);
                            itemDto.GeneratedBarcodeNumbers = generatedBarcodes;
                            _logger.LogInformation($"Generated {generatedBarcodes.Count} barcodes successfully");
                        }

                        await _unitOfWork.CompleteAsync();

                        // NOTE: Low stock alerts will be checked when entry is APPROVED,
                        // not at creation time, since stock is not updated yet
                    }
                    catch (Exception itemEx)
                    {
                        _logger.LogError(itemEx, $"Error processing item #{itemIndex} (ItemId: {itemDto.ItemId})");
                        // Now item is accessible here because it's declared outside the try block
                        var itemName = item?.Name ?? $"Unknown (ID: {itemDto.ItemId})";
                        throw new InvalidOperationException($"Error processing item '{itemName}': {itemEx.Message}", itemEx);
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation($"Stock entry transaction committed successfully. EntryNo: {entry.EntryNo}");

                // Log activity
                try
                {
                    await _activityLogService.LogActivityAsync(
                        "StockEntry",
                        entry.Id,
                        "Create",
                        $"Created stock entry {entry.EntryNo} with {dto.Items.Count} items",
                        _userContext.CurrentUserName
                    );
                }
                catch (Exception logEx)
                {
                    // Don't fail the operation if activity logging fails
                    _logger.LogError(logEx, "Failed to log activity for stock entry creation");
                }

                var result = await GetStockEntryByIdAsync(entry.Id);
                _logger.LogInformation($"Stock entry created successfully: {entry.EntryNo} with ID: {entry.Id}");
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error creating stock entry. StoreId: {dto.StoreId}, Items Count: {dto.Items?.Count ?? 0}");

                // Add more context to the exception
                var detailedMessage = $"Failed to create stock entry: {ex.Message}";
                if (ex.InnerException != null)
                {
                    detailedMessage += $" Inner Exception: {ex.InnerException.Message}";
                }

                throw new InvalidOperationException(detailedMessage, ex);
            }
        }
        public async Task<bool> SubmitStockEntryForApprovalAsync(int id)
        {
            try
            {
                var entry = await _unitOfWork.StockEntries.GetByIdAsync(id);
                if (entry == null || entry.Status != "Draft")
                    return false;

                entry.Status = "Submitted";
                entry.SubmittedBy = _userContext.CurrentUserName;
                entry.SubmittedDate = DateTime.Now;
                entry.UpdatedAt = DateTime.Now;
                entry.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.StockEntries.Update(entry);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "StockEntry",
                    id,
                    "Submit",
                    $"Submitted stock entry {entry.EntryNo} for approval",
                    _userContext.CurrentUserName
                );

                // Send notification to approvers
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    Type = "StockEntryApproval",
                    Title = "Stock Entry Approval Required",
                    Message = $"Stock entry {entry.EntryNo} has been submitted for approval",
                    RelatedEntity = "StockEntry",
                    RelatedEntityId = id,
                    Priority = "Medium"
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting stock entry {Id} for approval", id);
                return false;
            }
        }

        public async Task<bool> ApproveStockEntryAsync(int id, string comments = null)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var entry = await _unitOfWork.StockEntries.GetByIdAsync(id);
                if (entry == null || entry.Status != "Submitted")
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }

                // CRITICAL FIX: Update stock quantities ONLY on approval
                // Get all entry items for this stock entry
                var entryItems = await _unitOfWork.StockEntryItems
                    .GetAllAsync(ei => ei.StockEntryId == id);

                foreach (var entryItem in entryItems)
                {
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == entry.StoreId && si.ItemId == entryItem.ItemId);

                    if (storeItem != null)
                    {
                        // Update existing store item
                        var oldQuantity = storeItem.Quantity;
                        storeItem.Quantity += entryItem.Quantity;
                        storeItem.Location = entryItem.Location ?? storeItem.Location;
                        storeItem.UpdatedAt = DateTime.Now;
                        storeItem.UpdatedBy = _userContext.CurrentUserName;
                        _unitOfWork.StoreItems.Update(storeItem);
                        _logger.LogInformation($"Approved: Updated stock for ItemId {entryItem.ItemId} from {oldQuantity} to {storeItem.Quantity}");
                    }
                    else
                    {
                        // Create new store item
                        storeItem = new StoreItem
                        {
                            StoreId = entry.StoreId,
                            ItemId = entryItem.ItemId,
                            Quantity = entryItem.Quantity,
                            Location = entryItem.Location,
                            Status = ItemStatus.Available,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userContext.CurrentUserName,
                            IsActive = true
                        };
                        await _unitOfWork.StoreItems.AddAsync(storeItem);
                        _logger.LogInformation($"Approved: Created new stock for ItemId {entryItem.ItemId} with quantity {entryItem.Quantity}");
                    }

                    // Create stock movement record
                    var movement = new StockMovement
                    {
                        ItemId = entryItem.ItemId,
                        StoreId = entry.StoreId,
                        MovementType = "StockEntry",
                        Quantity = entryItem.Quantity,
                        MovementDate = DateTime.Now,
                        ReferenceType = "StockEntry",
                        ReferenceId = entry.Id,
                        ReferenceNo = entry.EntryNo,
                        Remarks = $"Stock entry approved: {entry.EntryNo}",
                        MovedBy = _userContext.CurrentUserName,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };
                    await _unitOfWork.StockMovements.AddAsync(movement);
                }

                entry.Status = "Approved";
                entry.ApprovedBy = _userContext.CurrentUserName;
                entry.ApprovedDate = DateTime.Now;
                entry.UpdatedAt = DateTime.Now;
                entry.UpdatedBy = _userContext.CurrentUserName;
                if (!string.IsNullOrEmpty(comments))
                {
                    entry.Remarks += $"\nApproval Comments: {comments}";
                }

                _unitOfWork.StockEntries.Update(entry);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogActivityAsync(
                    "StockEntry",
                    id,
                    "Approve",
                    $"Approved stock entry {entry.EntryNo} and updated stock",
                    _userContext.CurrentUserName
                );

                // Send notification to submitter
                if (!string.IsNullOrEmpty(entry.SubmittedBy))
                {
                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        Type = "StockEntryApproved",
                        Title = "Stock Entry Approved",
                        Message = $"Your stock entry {entry.EntryNo} has been approved and stock updated",
                        RelatedEntity = "StockEntry",
                        RelatedEntityId = id,
                        Priority = "Low",
                        TargetUserId = entry.SubmittedBy
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving stock entry {Id}", id);
                return false;
            }
        }

        public async Task<bool> RejectStockEntryAsync(int id, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                    throw new ArgumentException("Rejection reason is required");

                var entry = await _unitOfWork.StockEntries.GetByIdAsync(id);
                if (entry == null || entry.Status != "Submitted")
                    return false;

                entry.Status = "Rejected";
                entry.RejectedBy = _userContext.CurrentUserName;
                entry.RejectedDate = DateTime.Now;
                entry.RejectionReason = reason;
                entry.UpdatedAt = DateTime.Now;
                entry.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.StockEntries.Update(entry);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "StockEntry",
                    id,
                    "Reject",
                    $"Rejected stock entry {entry.EntryNo}. Reason: {reason}",
                    _userContext.CurrentUserName
                );

                // Send notification to submitter
                if (!string.IsNullOrEmpty(entry.SubmittedBy))
                {
                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        Type = "StockEntryRejected",
                        Title = "Stock Entry Rejected",
                        Message = $"Your stock entry {entry.EntryNo} has been rejected. Reason: {reason}",
                        RelatedEntity = "StockEntry",
                        RelatedEntityId = id,
                        Priority = "High",
                        TargetUserId = entry.SubmittedBy
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting stock entry {Id}", id);
                return false;
            }
        }

        public async Task<bool> CompleteStockEntryAsync(int id)
        {
            try
            {
                var entry = await _unitOfWork.StockEntries.GetByIdAsync(id);
                if (entry == null || entry.Status != "Draft")
                    return false;

                entry.Status = "Completed";
                entry.UpdatedAt = DateTime.Now;
                entry.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.StockEntries.Update(entry);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "StockEntry",
                    id,
                    "Complete",
                    $"Completed stock entry {entry.EntryNo}",
                    _userContext.CurrentUserName
                );

                // Send notification
                var store = await _unitOfWork.Stores.GetByIdAsync(entry.StoreId);
                await _notificationService.SendNotificationAsync(
                    new NotificationDto
                    {
                        Title = "Stock Entry Completed",
                        Message = $"Stock entry {entry.EntryNo} has been completed for {store?.Name}",
                        Type = "success",
                        UserId = entry.CreatedBy
                    }
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing stock entry");
                return false;
            }
        }

        public async Task<PagedResult<StockEntryDto>> GetStockEntriesAsync(int pageNumber, int pageSize, int? storeId = null, string status = null)
        {
            try
            {
                var query = _unitOfWork.StockEntries.Query()
                    .Include(e => e.Store)
                    .Include(e => e.Items)
                        .ThenInclude(i => i.Item)
                            .ThenInclude(i => i.SubCategory)
                                .ThenInclude(sc => sc.Category)
                    .Where(e => e.IsActive);

                if (storeId.HasValue)
                    query = query.Where(e => e.StoreId == storeId.Value);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(e => e.Status == status);

                var totalCount = await query.CountAsync();

                var entries = await query
                    .OrderByDescending(e => e.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = entries.Select(e => new StockEntryDto
                {
                    Id = e.Id,
                    EntryNo = e.EntryNo,
                    EntryDate = e.EntryDate,
                    StoreId = e.StoreId,
                    StoreName = e.Store?.Name,
                    EntryType = e.EntryType,
                    Remarks = e.Remarks,
                    Status = e.Status,
                    CreatedAt = e.CreatedAt,
                    CreatedBy = e.CreatedBy,
                    Items = e.Items.Select(i => new StockEntryItemDto
                    {
                        Id = i.Id,
                        ItemId = i.ItemId,
                        ItemCode = i.Item?.ItemCode,
                        ItemName = i.Item?.Name,
                        CategoryName = i.Item?.SubCategory?.Category?.Name,
                        SubCategoryName = i.Item?.SubCategory?.Name,
                        Quantity = i.Quantity,
                        Location = i.Location,
                        BatchNumber = i.BatchNumber,
                        ExpiryDate = i.ExpiryDate,
                        BarcodesGenerated = i.BarcodesGenerated,
                        UnitCost = i.UnitCost,
                        Unit = i.Item?.Unit
                    }).ToList()
                });

                return new PagedResult<StockEntryDto>
                {
                    Items = dtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock entries");
                throw;
            }
        }

        public async Task<StockEntryDto> GetStockEntryByIdAsync(int id)
        {
            var entry = await _unitOfWork.StockEntries.Query()
                .Include(e => e.Store)
                .Include(e => e.Items)
                    .ThenInclude(i => i.Item)
                        .ThenInclude(i => i.SubCategory)
                            .ThenInclude(sc => sc.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null)
                return null;

            // Get current stock levels
            var storeItems = await _unitOfWork.StoreItems
                .FindAsync(si => si.StoreId == entry.StoreId && entry.Items.Select(i => i.ItemId).Contains(si.ItemId));

            var stockDict = storeItems.ToDictionary(si => si.ItemId, si => si.Quantity);

            return new StockEntryDto
            {
                Id = entry.Id,
                EntryNo = entry.EntryNo,
                EntryDate = entry.EntryDate,
                StoreId = entry.StoreId,
                StoreName = entry.Store?.Name,
                EntryType = entry.EntryType,
                Remarks = entry.Remarks,
                Status = entry.Status,
                CreatedAt = entry.CreatedAt,
                CreatedBy = entry.CreatedBy,
                Items = entry.Items.Select(i => new StockEntryItemDto
                {
                    Id = i.Id,
                    ItemId = i.ItemId,
                    ItemCode = i.Item?.ItemCode,
                    ItemName = i.Item?.Name,
                    CategoryName = i.Item?.SubCategory?.Category?.Name,
                    SubCategoryName = i.Item?.SubCategory?.Name,
                    Quantity = i.Quantity,
                    Location = i.Location,
                    BatchNumber = i.BatchNumber,
                    ExpiryDate = i.ExpiryDate,
                    BarcodesGenerated = i.BarcodesGenerated,
                    UnitCost = i.UnitCost,
                    Unit = i.Item?.Unit,
                    CurrentStock = stockDict.ContainsKey(i.ItemId) ? stockDict[i.ItemId] : 0
                }).ToList()
            };
        }

        public async Task<BulkStockUploadDto> ProcessBulkUploadAsync(BulkStockUploadDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create stock entry for bulk upload
                var entry = new StockEntry
                {
                    EntryNo = await GenerateEntryNoAsync(),
                    EntryDate = DateTime.Now,
                    StoreId = dto.StoreId,
                    EntryType = "Bulk Upload",
                    Remarks = $"Bulk upload from file: {dto.FileName}",
                    Status = "Completed",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.StockEntries.AddAsync(entry);
                await _unitOfWork.CompleteAsync();

                var processedCount = 0;
                foreach (var uploadItem in dto.Items.Where(i => i.IsValid))
                {
                    try
                    {
                        // Get item details
                        var item = await _unitOfWork.Items.FirstOrDefaultAsync(i => i.ItemCode == uploadItem.ItemCode);
                        if (item == null) continue;

                        // Create stock entry item
                        var entryItem = new StockEntryItem
                        {
                            StockEntryId = entry.Id,
                            ItemId = item.Id,
                            Quantity = uploadItem.Quantity,
                            Location = uploadItem.Location,
                            BatchNumber = uploadItem.BatchNumber ?? await GenerateBatchNumberAsync(),
                            ExpiryDate = uploadItem.ExpiryDate,
                            UnitCost = uploadItem.UnitCost,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userContext.CurrentUserName,
                            IsActive = true
                        };

                        await _unitOfWork.StockEntryItems.AddAsync(entryItem);

                        // Update store stock
                        var storeItem = await _unitOfWork.StoreItems
                            .FirstOrDefaultAsync(si => si.StoreId == dto.StoreId && si.ItemId == item.Id);

                        if (storeItem != null)
                        {
                            storeItem.Quantity += uploadItem.Quantity;
                            storeItem.UpdatedAt = DateTime.Now;
                            storeItem.UpdatedBy = _userContext.CurrentUserName;
                            _unitOfWork.StoreItems.Update(storeItem);
                        }
                        else
                        {
                            storeItem = new StoreItem
                            {
                                StoreId = dto.StoreId,
                                ItemId = item.Id,
                                Quantity = uploadItem.Quantity,
                                Location = uploadItem.Location,
                                Status = ItemStatus.Available,
                                CreatedAt = DateTime.Now,
                                CreatedBy = _userContext.CurrentUserName,
                                IsActive = true
                            };
                            await _unitOfWork.StoreItems.AddAsync(storeItem);
                        }

                        processedCount++;
                    }
                    catch (Exception itemEx)
                    {

                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogActivityAsync(
                    "StockEntry",
                    entry.Id,
                    "BulkUpload",
                    $"Processed bulk upload with {processedCount} items",
                    _userContext.CurrentUserName
                );

                return dto;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error processing bulk upload");
                throw;
            }
        }

        public async Task<decimal> GetCurrentStockAsync(int itemId, int? storeId)
        {
            var storeItem = await _unitOfWork.StoreItems
                .FirstOrDefaultAsync(si => si.ItemId == itemId && si.StoreId == storeId);

            return storeItem?.Quantity ?? 0;
        }

        public async Task<IEnumerable<StockLevelReportDto>> GetLowStockItemsAsync(int? storeId)
        {
            var storeItems = await _unitOfWork.StoreItems.Query()
                .Include(si => si.Item)
                    .ThenInclude(i => i.SubCategory)
                        .ThenInclude(sc => sc.Category)
                .Include(si => si.Store)
                .Where(si => si.StoreId == storeId && si.IsActive)
                .ToListAsync();

            var lowStockItems = storeItems
                .Where(si => si.Item.MinimumStock > 0 && si.Quantity <= si.Item.MinimumStock)
                .GroupBy(si => si.ItemId)
                .Select(g => new StockLevelReportDto
                {
                    ItemId = g.Key,
                    ItemCode = g.First().Item.ItemCode,
                    ItemName = g.First().Item.Name,
                    CategoryName = g.First().Item.SubCategory?.Category?.Name,
                    SubCategoryName = g.First().Item.SubCategory?.Name,
                    TotalStock = g.Sum(si => si.Quantity),
                    MinimumStock = g.First().Item.MinimumStock,
                    ReorderLevel = g.First().Item.ReorderLevel,
                })
                .OrderBy(i => i.TotalStock)
                .ToList();

            return lowStockItems;
        }

        private async Task SendLowStockAlertAsync(Item item, StoreItem storeItem, int? storeId)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);

            var notification = new NotificationDto
            {
                Title = "Low Stock Alert",
                Message = $"Item {item.Name} (Code: {item.ItemCode}) in {store.Name} has low stock. Current: {storeItem.Quantity}, Minimum: {item.MinimumStock}",
                Type = "warning",
                Priority = "high"
            };

            // Send to store managers
            var storeUsers = await _unitOfWork.UserStores
                .FindAsync(us => us.StoreId == storeId && us.IsActive);

            foreach (var userStore in storeUsers)
            {
                notification.UserId = userStore.UserId;
                await _notificationService.CreateNotificationAsync(notification);
            }

            // Create stock alert record
            var stockAlert = new StockAlert
            {
                ItemId = item.Id,
                StoreId = storeId,
                AlertType = "LowStock",
                CurrentQuantity = storeItem.Quantity,
                ThresholdQuantity = item.MinimumStock,
                Message = notification.Message,
                IsResolved = false,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            };

            await _unitOfWork.StockAlerts.AddAsync(stockAlert);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> GenerateBatchNumberAsync()
        {
            var date = DateTime.Now;
            var random = new Random().Next(1000, 9999);
            return $"BN{date:yyyyMMdd}{random}";
        }

        // Stock Adjustment Methods
        public async Task<string> GenerateAdjustmentNoAsync()
        {
            var date = DateTime.Now;
            var prefix = $"SA{date:yyyyMMdd}";

            var lastAdjustment = await _unitOfWork.StockAdjustments
                .FindAsync(a => a.AdjustmentNo.StartsWith(prefix));

            var sequence = 1;
            if (lastAdjustment.Any())
            {
                var lastNo = lastAdjustment.OrderByDescending(a => a.AdjustmentNo).First().AdjustmentNo;
                var lastSequence = lastNo.Substring(prefix.Length);
                if (int.TryParse(lastSequence, out var seq))
                    sequence = seq + 1;
            }

            return $"{prefix}{sequence:D4}";
        }

        public async Task<StockAdjustmentDto> CreateStockAdjustmentAsync(StockAdjustmentDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var adjustment = new StockAdjustment
                {
                    AdjustmentNo = await GenerateAdjustmentNoAsync(),
                    AdjustmentDate = dto.AdjustmentDate,
                    StoreId = dto.StoreId,
                    AdjustmentType = dto.AdjustmentType,
                    Reason = dto.Reason,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userContext.CurrentUserName,
                    IsActive = true
                };

                await _unitOfWork.StockAdjustments.AddAsync(adjustment);
                await _unitOfWork.CompleteAsync();

                foreach (var itemDto in dto.Items)
                {
                    var adjustmentItem = new StockAdjustmentItem
                    {
                        StockAdjustmentId = adjustment.Id,
                        ItemId = itemDto.ItemId,
                        SystemQuantity = itemDto.SystemQuantity,
                        ActualQuantity = itemDto.ActualQuantity,
                        AdjustmentQuantity = itemDto.AdjustmentQuantity,
                        Remarks = itemDto.Remarks,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userContext.CurrentUserName,
                        IsActive = true
                    };

                    await _unitOfWork.StockAdjustmentItems.AddAsync(adjustmentItem);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogActivityAsync(
                    "StockAdjustment",
                    adjustment.Id,
                    "Create",
                    $"Created stock adjustment {adjustment.AdjustmentNo}",
                    _userContext.CurrentUserName
                );

                return await GetStockAdjustmentByIdAsync(adjustment.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating stock adjustment");
                throw;
            }
        }

        public async Task<bool> ApproveStockAdjustmentAsync(int id, string approvedBy)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var adjustment = await _unitOfWork.StockAdjustments.Query()
                    .Include(a => a.Items)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (adjustment == null || adjustment.Status != "Pending")
                    return false;

                adjustment.Status = "Approved";
                adjustment.ApprovedBy = approvedBy;
                adjustment.ApprovedDate = DateTime.Now;
                adjustment.UpdatedAt = DateTime.Now;
                adjustment.UpdatedBy = approvedBy;

                _unitOfWork.StockAdjustments.Update(adjustment);

                // Apply adjustments to stock
                foreach (var item in adjustment.Items)
                {
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == adjustment.StoreId && si.ItemId == item.ItemId);

                    if (storeItem != null)
                    {
                        storeItem.Quantity = item.ActualQuantity;
                        storeItem.UpdatedAt = DateTime.Now;
                        storeItem.UpdatedBy = approvedBy;
                        _unitOfWork.StoreItems.Update(storeItem);

                        // Create stock entry for audit trail
                        var stockEntry = new StockEntry
                        {
                            EntryNo = await GenerateEntryNoAsync(),
                            EntryDate = DateTime.Now,
                            StoreId = adjustment.StoreId,
                            EntryType = "Adjustment",
                            Remarks = $"Stock adjustment {adjustment.AdjustmentNo}: {adjustment.Reason}",
                            Status = "Completed",
                            CreatedAt = DateTime.Now,
                            CreatedBy = approvedBy,
                            IsActive = true
                        };

                        await _unitOfWork.StockEntries.AddAsync(stockEntry);
                        await _unitOfWork.CompleteAsync();

                        var stockEntryItem = new StockEntryItem
                        {
                            StockEntryId = stockEntry.Id,
                            ItemId = item.ItemId,
                            Quantity = item.AdjustmentQuantity,
                            Location = storeItem.Location,
                            BatchNumber = await GenerateBatchNumberAsync(),
                            CreatedAt = DateTime.Now,
                            CreatedBy = approvedBy,
                            IsActive = true
                        };

                        await _unitOfWork.StockEntryItems.AddAsync(stockEntryItem);
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                await _activityLogService.LogActivityAsync(
                    "StockAdjustment",
                    id,
                    "Approve",
                    $"Approved stock adjustment {adjustment.AdjustmentNo}",
                    approvedBy
                );

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error approving stock adjustment");
                return false;
            }
        }

        // Other interface methods implementation...
        public async Task<StockAdjustmentDto> GetStockAdjustmentByIdAsync(int id)
        {
            var adjustment = await _unitOfWork.StockAdjustments.Query()
                .Include(a => a.Store)
                .Include(a => a.Items)
                    .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (adjustment == null)
                return null;

            return new StockAdjustmentDto
            {
                Id = adjustment.Id,
                AdjustmentNo = adjustment.AdjustmentNo,
                AdjustmentDate = adjustment.AdjustmentDate,
                StoreId = adjustment.StoreId,
                StoreName = adjustment.Store?.Name,
                AdjustmentType = adjustment.AdjustmentType.ToString(),
                Reason = adjustment.Reason,
                Status = adjustment.Status,
                ApprovedBy = adjustment.ApprovedBy,
                ApprovedDate = adjustment.ApprovedDate,
                CreatedAt = adjustment.CreatedAt,
                CreatedBy = adjustment.CreatedBy,
                Items = adjustment.Items.Select(i => new StockAdjustmentItemDto
                {
                    ItemId = i.ItemId,
                    ItemCode = i.Item?.ItemCode,
                    ItemName = i.Item?.Name,
                    SystemQuantity = i.SystemQuantity,
                    ActualQuantity = i.ActualQuantity,
                    AdjustmentQuantity = i.AdjustmentQuantity,
                    Remarks = i.Remarks
                }).ToList()
            };
        }

        public async Task<byte[]> GenerateBulkUploadTemplateAsync()
        {
            var csv = new StringBuilder();
            csv.AppendLine("ItemCode,Quantity,Location,BatchNumber,ExpiryDate,UnitCost");
            csv.AppendLine("ITEM001,100,A1-B2,BN20240115,2025-12-31,150.00");
            csv.AppendLine("ITEM002,50,A2-C3,BN20240116,,200.00");

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<StockEntryDto> UpdateStockEntryAsync(int id, StockEntryDto stockEntryDto)
        {
            var stockEntry = await _unitOfWork.StockEntries.GetByIdAsync(id);
            if (stockEntry == null)
                throw new InvalidOperationException("Stock entry not found");

            stockEntry.Remarks = stockEntryDto.Remarks;
            stockEntry.UpdatedAt = DateTime.Now;
            stockEntry.UpdatedBy = _userContext.CurrentUserName;

            _unitOfWork.StockEntries.Update(stockEntry);
            await _unitOfWork.CompleteAsync();

            // Return the updated DTO
            return await GetStockEntryByIdAsync(id);
        }

        public async Task<bool> DeleteStockEntryAsync(int id)
        {
            try
            {
                var stockEntry = await _unitOfWork.StockEntries.GetByIdAsync(id);
                if (stockEntry == null)
                    return false;

                stockEntry.IsActive = false;
                stockEntry.UpdatedAt = DateTime.Now;
                stockEntry.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.StockEntries.Update(stockEntry);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock entry");
                return false;
            }
        }

        public async Task<bool> CancelStockEntryAsync(int id, string reason)
        {
            try
            {
                var stockEntry = await _unitOfWork.StockEntries.GetByIdAsync(id);
                if (stockEntry == null)
                    return false;

                stockEntry.Status = "Cancelled";
                stockEntry.Remarks = reason;
                stockEntry.UpdatedAt = DateTime.Now;
                stockEntry.UpdatedBy = _userContext.CurrentUserName;

                _unitOfWork.StockEntries.Update(stockEntry);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling stock entry");
                return false;
            }
        }

        public async Task<PagedResult<StockAdjustmentDto>> GetStockAdjustmentsAsync(
            int pageNumber, int pageSize, int? storeId = null, string status = null)
        {
            var query = _unitOfWork.StockAdjustments.Query()
                .Include(sa => sa.Store)
                .Include(sa => sa.Items)
                .Where(sa => sa.IsActive);

            if (storeId.HasValue)
                query = query.Where(sa => sa.StoreId == storeId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(sa => sa.Status == status);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(sa => sa.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var adjustmentDtos = items.Select(sa => new StockAdjustmentDto
            {
                Id = sa.Id,
                AdjustmentNo = sa.AdjustmentNo,
                StoreId = sa.StoreId,
                StoreName = sa.Store?.Name,
                AdjustmentDate = sa.AdjustmentDate,
                Reason = sa.Reason,
                Status = sa.Status,
                Items = sa.Items.Select(i => new StockAdjustmentItemDto
                {
                    ItemId = i.ItemId,
                    SystemQuantity = i.SystemQuantity,
                    ActualQuantity = i.ActualQuantity,
                    Variance = i.ActualQuantity - i.SystemQuantity, // Calculate variance here
                    Remarks = i.Remarks
                }).ToList()
            }).ToList();

            return new PagedResult<StockAdjustmentDto>
            {
                Items = adjustmentDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> RejectStockAdjustmentAsync(int id, string rejectedBy, string reason)
        {
            try
            {
                var adjustment = await _unitOfWork.StockAdjustments.GetByIdAsync(id);
                if (adjustment == null)
                    return false;

                adjustment.Status = "Rejected";
                adjustment.ApprovedBy = rejectedBy;
                adjustment.ApprovedDate = DateTime.Now;
                adjustment.Remarks = reason;
                adjustment.UpdatedAt = DateTime.Now;
                adjustment.UpdatedBy = rejectedBy;

                _unitOfWork.StockAdjustments.Update(adjustment);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting stock adjustment");
                return false;
            }
        }
        public async Task<bool> CheckStockAvailabilityAsync(int? storeId, int itemId, decimal requiredQuantity)
        {
            var storeItem = await _unitOfWork.StoreItems
                .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ItemId == itemId && si.IsActive);

            return storeItem != null && storeItem.Quantity >= requiredQuantity;
        }
        public async Task<byte[]> ExportStockReportAsync(int? storeId = null, string format = "excel")
        {
            var stockLevels = await GetStockLevelsAsync(storeId);

            if (format.ToLower() == "excel")
            {
                // Use EPPlus or similar to generate Excel
                return new byte[0]; // Placeholder
            }
            else
            {
                // Generate PDF
                return new byte[0]; // Placeholder
            }
        }

        public async Task<IEnumerable<dynamic>> GetStockMovementReportAsync(
            int? storeId, DateTime fromDate, DateTime toDate)
        {
            var movements = new List<dynamic>();

            // Get stock entries
            var stockEntries = await _unitOfWork.StockEntries.Query()
                .Include(se => se.Items)
                    .ThenInclude(i => i.Item)
                .Where(se => se.StoreId == storeId &&
                             se.EntryDate >= fromDate &&
                             se.EntryDate <= toDate &&
                             se.IsActive)
                .ToListAsync();

            foreach (var entry in stockEntries)
            {
                foreach (var item in entry.Items)
                {
                    movements.Add(new
                    {
                        MovementDate = entry.EntryDate,
                        MovementType = "Stock Entry",
                        ReferenceNo = entry.EntryNo,
                        ItemId = item.ItemId,
                        ItemName = item.Item?.Name,
                        Quantity = item.Quantity,
                        MovementDirection = "In",
                        Remarks = entry.Remarks,
                        CreatedAt = entry.CreatedAt,
                        CreatedBy = entry.CreatedBy
                    });
                }
            }

            return movements.OrderBy(m => m.MovementDate);
        }
        public async Task<bool> CheckStockAvailabilityAsync(int itemId, int? storeId, decimal requiredQuantity)
        {
            var storeItem = await _unitOfWork.StoreItems
                .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ItemId == itemId && si.IsActive);

            return storeItem != null && storeItem.Quantity >= requiredQuantity;
        }

        public async Task<ValidationResult> ValidateBulkUploadFileAsync(Stream fileStream)
        {
            var result = new ValidationResult { IsValid = true };
            var errors = new List<string>();

            try
            {
                using var reader = new StreamReader(fileStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                // Validate headers
                await csv.ReadAsync();
                csv.ReadHeader();

                var headers = csv.HeaderRecord;
                var requiredHeaders = new[] { "ItemCode", "Quantity", "Location", "BatchNumber", "ExpiryDate", "UnitCost" };

                foreach (var header in requiredHeaders)
                {
                    if (!headers.Contains(header))
                    {
                        errors.Add($"Missing required column: {header}");
                    }
                }

                if (errors.Any())
                {
                    result.IsValid = false;
                    result.ErrorMessage = string.Join("; ", errors);
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Error reading file: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult> BulkUploadStockAsync(BulkStockUploadDto bulkUpload)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var store = await _unitOfWork.Stores.GetByIdAsync(bulkUpload.StoreId);
                if (store == null)
                    return ServiceResult.Failure("Store not found");

                var currentUser = _userContext.CurrentUserName;

                foreach (var itemDto in bulkUpload.Items)
                {
                    // Validate item exists
                    var item = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                    if (item == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ServiceResult.Failure($"Item with ID {itemDto.ItemId} not found");
                    }

                    // Create stock entry
                    var stockEntry = new StockEntry
                    {
                        EntryNo = await GenerateEntryNoAsync(),
                        EntryDate = DateTime.Now,
                        StoreId = bulkUpload.StoreId,
                        EntryType = "BULK_UPLOAD",
                        Status = "Completed",
                        Remarks = $"Bulk upload by {currentUser}",
                        CreatedAt = DateTime.Now,
                        CreatedBy = currentUser,
                        IsActive = true
                    };

                    await _unitOfWork.StockEntries.AddAsync(stockEntry);
                    await _unitOfWork.CompleteAsync();

                    // Create stock entry item (without UnitPrice)
                    var stockEntryItem = new StockEntryItem
                    {
                        StockEntryId = stockEntry.Id,
                        ItemId = itemDto.ItemId,
                        Quantity = itemDto.Quantity,
                        UnitCost = itemDto.UnitCost, // Use UnitCost instead of UnitPrice
                        BatchNumber = await _barcodeService.GenerateBatchNumberAsync(),
                        Location = itemDto.Location,
                        CreatedAt = DateTime.Now,
                        CreatedBy = currentUser,
                        IsActive = true
                    };

                    await _unitOfWork.StockEntryItems.AddAsync(stockEntryItem);

                    // Update store item quantity (use Quantity instead of CurrentStock)
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.StoreId == bulkUpload.StoreId && si.ItemId == itemDto.ItemId);

                    if (storeItem != null)
                    {
                        storeItem.Quantity += itemDto.Quantity; // Use Quantity instead of CurrentStock
                        storeItem.UpdatedAt = DateTime.Now;
                        storeItem.UpdatedBy = currentUser;
                        _unitOfWork.StoreItems.Update(storeItem);
                    }
                    else
                    {
                        // Create new store item
                        storeItem = new StoreItem
                        {
                            StoreId = bulkUpload.StoreId,
                            ItemId = itemDto.ItemId,
                            Quantity = itemDto.Quantity, // Use Quantity
                            ReorderLevel = 10, // Default
                            Location = itemDto.Location,
                            Status = ItemStatus.Available,
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            CreatedBy = currentUser
                        };
                        await _unitOfWork.StoreItems.AddAsync(storeItem);
                    }

                    // Create stock movement record
                    var stockMovement = new StockMovement
                    {
                        StoreId = bulkUpload.StoreId,
                        ItemId = itemDto.ItemId,
                        MovementType = "IN",
                        MovementDate = DateTime.Now,
                        Quantity = itemDto.Quantity,
                        ReferenceType = "STOCK_ENTRY",
                        ReferenceNo = stockEntry.EntryNo,
                        Remarks = "Bulk upload",
                        CreatedAt = DateTime.Now,
                        CreatedBy = currentUser,
                        IsActive = true
                    };
                    await _unitOfWork.StockMovements.AddAsync(stockMovement);

                    // Generate barcode (remove ExpiryDate from BarcodeDto)
                    if (itemDto.GenerateBarcode)
                    {
                        var barcodeDto = new BarcodeDto
                        {
                            ItemId = itemDto.ItemId,
                            BarcodeNumber = await _barcodeService.GenerateUniqueBarcodeNumberAsync(itemDto.ItemId),
                            BatchNumber = stockEntryItem.BatchNumber,
                            SerialNumber = await _barcodeService.GenerateSerialNumberAsync(),
                            GeneratedDate = DateTime.Now,
                            CreatedBy = currentUser
                        };
                        await _barcodeService.CreateBarcodeAsync(barcodeDto);
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "StockEntry",
                    0,
                    "BulkUpload",
                    $"Bulk uploaded {bulkUpload.Items.Count} items to store {store.Name}",
                    currentUser
                );

                // Send notification
                var notification = new NotificationDto
                {
                    Title = "Bulk Stock Upload Completed",
                    Message = $"Successfully uploaded {bulkUpload.Items.Count} items to {store.Name}",
                    Type = "success",
                    UserId = currentUser
                };
                await _notificationService.CreateNotificationAsync(notification);

                return ServiceResult.SuccessResult($"Successfully uploaded {bulkUpload.Items.Count} items");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error in bulk stock upload");
                return ServiceResult.Failure($"Bulk upload failed: {ex.Message}");
            }
        }

        public async Task<Stream> DownloadBulkUploadTemplateAsync()
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Write headers
                csv.WriteField("ItemCode");
                csv.WriteField("ItemName");
                csv.WriteField("Quantity");
                csv.WriteField("UnitPrice");
                csv.WriteField("BatchNumber");
                csv.WriteField("ExpiryDate");
                csv.WriteField("Location");
                csv.NextRecord();

                // Add sample data
                csv.WriteField("ITEM001");
                csv.WriteField("Sample Item");
                csv.WriteField("100");
                csv.WriteField("50.00");
                csv.WriteField("BATCH001");
                csv.WriteField(DateTime.Now.AddYears(2).ToString("yyyy-MM-dd"));
                csv.WriteField("A1-B2");
                csv.NextRecord();
            }

            stream.Position = 0;
            return stream;
        }

        public Task<Dictionary<int, decimal>> GetMultipleItemStocksAsync(int? storeId, List<int> itemIds)
        {
            throw new NotImplementedException();
        }

        public Task<BulkStockUploadDto> ValidateBulkUploadAsync(Stream fileStream, int? storeId)
        {
            throw new NotImplementedException();
        }
    }
}
