// Add this to your IMS.Application/Services/StockAdjustmentService.cs
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly IWriteOffService _writeOffService;
        private readonly IActivityLogService _activityLogService;
        private readonly IApprovalService _approvalService;

        // Write-off related reasons that trigger automatic WriteOff creation
        private readonly string[] WRITEOFF_REASONS = new[]
        {
            "Damaged", "Expired", "Lost", "Stolen", "Obsolete",
            "Quality Issues", "Theft", "Natural Disaster", "Fire Damage", "Water Damage"
        };

        private const decimal WRITEOFF_APPROVAL_THRESHOLD = 10000m; // ₹10,000

        public StockAdjustmentService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IWriteOffService writeOffService,
            IActivityLogService activityLogService,
            IApprovalService approvalService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _writeOffService = writeOffService;
            _activityLogService = activityLogService;
            _approvalService = approvalService;
        }

        public async Task<IEnumerable<StockAdjustmentDto>> GetAllStockAdjustmentsAsync()
        {
            var adjustments = await _unitOfWork.StockAdjustments.GetAllAsync();
            var adjustmentDtos = new List<StockAdjustmentDto>();

            foreach (var adjustment in adjustments.Where(a => a.IsActive))
            {
                var item = await _unitOfWork.Items.GetByIdAsync(adjustment.ItemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(adjustment.StoreId);

                adjustmentDtos.Add(MapToDto(adjustment, item, store));
            }

            return adjustmentDtos.OrderByDescending(a => a.AdjustmentDate);
        }

        public async Task<StockAdjustmentDto> GetStockAdjustmentByIdAsync(int id)
        {
            var adjustment = await _unitOfWork.StockAdjustments.GetByIdAsync(id);
            if (adjustment == null || !adjustment.IsActive) return null;

            var item = await _unitOfWork.Items.GetByIdAsync(adjustment.ItemId);
            var store = await _unitOfWork.Stores.GetByIdAsync(adjustment.StoreId);

            return MapToDto(adjustment, item, store);
        }

        public async Task<StockAdjustmentDto> CreateStockAdjustmentAsync(StockAdjustmentDto adjustmentDto)
        {
            // Validate input
            if (adjustmentDto.ItemId <= 0)
                throw new ArgumentException("Invalid Item ID", nameof(adjustmentDto.ItemId));

            if (!adjustmentDto.StoreId.HasValue || adjustmentDto.StoreId.Value <= 0)
                throw new ArgumentException("Store is required", nameof(adjustmentDto.StoreId));

            // Verify Item exists
            var item = await _unitOfWork.Items.GetByIdAsync(adjustmentDto.ItemId);
            if (item == null)
                throw new InvalidOperationException($"Item with ID {adjustmentDto.ItemId} does not exist");

            // Verify Store exists
            var store = await _unitOfWork.Stores.GetByIdAsync(adjustmentDto.StoreId.Value);
            if (store == null)
                throw new InvalidOperationException($"Store with ID {adjustmentDto.StoreId.Value} does not exist");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Get current stock level
                var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                    si => si.ItemId == adjustmentDto.ItemId && si.StoreId == adjustmentDto.StoreId
                );

                bool isNewStoreItem = false;
                if (storeItem == null)
                {
                    // Create new store item if it doesn't exist
                    storeItem = new StoreItem
                    {
                        ItemId = adjustmentDto.ItemId,
                        StoreId = adjustmentDto.StoreId,
                        Quantity = 0,
                        Status = Domain.Enums.ItemStatus.Available,
                        CreatedAt = DateTime.Now,
                        CreatedBy = GetCurrentUserId()
                    };
                    await _unitOfWork.StoreItems.AddAsync(storeItem);
                    isNewStoreItem = true;
                }

                var oldQuantity = storeItem.Quantity;
                var newQuantity = adjustmentDto.NewQuantity;
                var adjustmentQuantity = newQuantity - oldQuantity;
                var adjustmentType = adjustmentQuantity >= 0 ? "Increase" : "Decrease";

                // Create adjustment record
                var adjustment = new StockAdjustment
                {
                    AdjustmentNo = await GenerateAdjustmentNoAsync(),
                    AdjustmentDate = adjustmentDto.AdjustmentDate,
                    ItemId = adjustmentDto.ItemId,
                    StoreId = adjustmentDto.StoreId,
                    OldQuantity = oldQuantity,
                    NewQuantity = newQuantity,
                    AdjustmentQuantity = Math.Abs((decimal)adjustmentQuantity),
                    AdjustmentType = adjustmentType,
                    Reason = adjustmentDto.Reason,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    CreatedBy = GetCurrentUserId()
                };

                await _unitOfWork.StockAdjustments.AddAsync(adjustment);
                await _unitOfWork.CompleteAsync();

                // ✅ CREATE APPROVAL REQUEST - Send to Approval Center
                await _approvalService.CreateApprovalRequestAsync(new ApprovalRequestDto
                {
                    EntityType = "STOCK_ADJUSTMENT",
                    EntityId = adjustment.Id,
                    RequestedBy = GetCurrentUserId(),
                    RequestedDate = DateTime.Now,
                    Amount = Math.Abs((decimal)adjustmentQuantity) * (item.UnitPrice ?? 0),
                    Description = $"Stock Adjustment {adjustment.AdjustmentNo} - {adjustment.Reason}",
                    Priority = Math.Abs((decimal)adjustmentQuantity) > 100 ? "High" : "Normal",
                    Status = "Pending"
                });

                await _unitOfWork.CommitTransactionAsync();

                // 🎯 SMART LOGIC: Check if this adjustment should trigger a WriteOff
                if (ShouldTriggerWriteOff(adjustmentDto.Reason, adjustmentType))
                {
                    var itemValue = Math.Abs((decimal)adjustmentQuantity) * (item.UnitPrice ?? 0);

                    // Auto-create WriteOff for damage/loss/expiry reasons
                    await CreateWriteOffFromAdjustmentAsync(adjustment, item, store, itemValue);

                    // Log activity
                    await _activityLogService.LogActivityAsync(
                        "Stock Adjustment",
                        adjustment.Id,
                        "AutoWriteOff",
                        $"Auto-created WriteOff for adjustment #{adjustment.AdjustmentNo} due to reason: {adjustmentDto.Reason}",
                        GetCurrentUserId()
                    );
                }

                // Send notification
                await _notificationService.SendNotificationAsync(
                    GetCurrentUserId(),
                    "Stock Adjustment Created",
                    $"Adjustment #{adjustment.AdjustmentNo} for {Math.Abs((decimal)adjustmentQuantity)} units has been created.",
                    "info"
                );

                return await GetStockAdjustmentByIdAsync(adjustment.Id);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<string> GenerateAdjustmentNoAsync()
        {
            var lastAdjustment = (await _unitOfWork.StockAdjustments.GetAllAsync())
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastAdjustment != null && !string.IsNullOrEmpty(lastAdjustment.AdjustmentNo))
            {
                var lastNumber = lastAdjustment.AdjustmentNo.Substring(3);
                if (int.TryParse(lastNumber, out int number))
                {
                    nextNumber = number + 1;
                }
            }

            return $"ADJ{nextNumber:D6}";
        }

        public async Task ApproveAdjustmentAsync(int id, string approvedBy)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var adjustment = await _unitOfWork.StockAdjustments.GetByIdAsync(id);
                if (adjustment == null || adjustment.Status != "Pending")
                    throw new InvalidOperationException("Adjustment not found or already processed");

                // Update adjustment status
                adjustment.Status = "Approved";
                adjustment.ApprovedBy = approvedBy;
                adjustment.ApprovedDate = DateTime.Now;
                _unitOfWork.StockAdjustments.Update(adjustment);

                // Update stock
                var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                    si => si.ItemId == adjustment.ItemId && si.StoreId == adjustment.StoreId
                );

                if (storeItem != null)
                {
                    storeItem.Quantity = adjustment.NewQuantity;
                    storeItem.UpdatedAt = DateTime.Now;
                    storeItem.UpdatedBy = approvedBy;
                    _unitOfWork.StoreItems.Update(storeItem);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Send notification
                await _notificationService.SendNotificationAsync(
                    adjustment.CreatedBy,
                    "Stock Adjustment Approved",
                    $"Your adjustment #{adjustment.AdjustmentNo} has been approved.",
                    "success"
                );
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RejectAdjustmentAsync(int id, string rejectedBy, string reason)
        {
            var adjustment = await _unitOfWork.StockAdjustments.GetByIdAsync(id);
            if (adjustment == null || adjustment.Status != "Pending")
                throw new InvalidOperationException("Adjustment not found or already processed");

            adjustment.Status = "Rejected";
            adjustment.RejectionReason = reason;
            adjustment.UpdatedAt = DateTime.Now;
            adjustment.UpdatedBy = rejectedBy;

            _unitOfWork.StockAdjustments.Update(adjustment);
            await _unitOfWork.CompleteAsync();

            // Send notification
            await _notificationService.SendNotificationAsync(
                adjustment.CreatedBy,
                "Stock Adjustment Rejected",
                $"Your adjustment #{adjustment.AdjustmentNo} has been rejected. Reason: {reason}",
                "error"
            );
        }

        public async Task<StockLevelDto> GetCurrentStockLevelAsync(int itemId, int? storeId)
        {
            var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(
                si => si.ItemId == itemId && si.StoreId == storeId
            );

            var item = await _unitOfWork.Items.GetByIdAsync(itemId);
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);

            return new StockLevelDto
            {
                ItemId = itemId,
                StoreId = storeId,
                ItemName = item?.Name,
                ItemCode = item?.ItemCode,
                StoreName = store?.Name,
                CurrentQuantity = storeItem?.Quantity ?? 0,
                MinimumStock = item?.MinimumStock ?? 0,
                Unit = item?.Unit,
                Status = storeItem?.Status.ToString() ?? "Not Available",
                LastUpdated = storeItem?.UpdatedAt ?? DateTime.Now
            };
        }

        public async Task<IEnumerable<StockAdjustmentDto>> GetAdjustmentsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var adjustments = await _unitOfWork.StockAdjustments.FindAsync(
                a => a.AdjustmentDate >= fromDate && a.AdjustmentDate <= toDate && a.IsActive
            );

            var adjustmentDtos = new List<StockAdjustmentDto>();
            foreach (var adjustment in adjustments)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(adjustment.ItemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(adjustment.StoreId);
                adjustmentDtos.Add(MapToDto(adjustment, item, store));
            }

            return adjustmentDtos.OrderByDescending(a => a.AdjustmentDate);
        }

        public async Task<IEnumerable<StockAdjustmentDto>> GetPendingAdjustmentsAsync()
        {
            var adjustments = await _unitOfWork.StockAdjustments.FindAsync(
                a => a.Status == "Pending" && a.IsActive
            );

            var adjustmentDtos = new List<StockAdjustmentDto>();
            foreach (var adjustment in adjustments)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(adjustment.ItemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(adjustment.StoreId);
                adjustmentDtos.Add(MapToDto(adjustment, item, store));
            }

            return adjustmentDtos.OrderByDescending(a => a.CreatedAt);
        }

        public async Task<Dictionary<string, int>> GetAdjustmentStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = await _unitOfWork.StockAdjustments.FindAsync(a => a.IsActive);

            if (fromDate.HasValue)
                query = query.Where(a => a.AdjustmentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.AdjustmentDate <= toDate.Value);

            return new Dictionary<string, int>
            {
                ["Total"] = query.Count(),
                ["Pending"] = query.Count(a => a.Status == "Pending"),
                ["Approved"] = query.Count(a => a.Status == "Approved"),
                ["Rejected"] = query.Count(a => a.Status == "Rejected"),
                ["Increases"] = query.Count(a => a.AdjustmentType.ToString() == "Increase"),
                ["Decreases"] = query.Count(a => a.AdjustmentType.ToString() == "Decrease")
            };
        }

        // Implement other interface methods...
        public async Task<IEnumerable<StockAdjustmentDto>> GetAdjustmentsByItemAsync(int? itemId)
        {
            var adjustments = await _unitOfWork.StockAdjustments.FindAsync(
                a => a.ItemId == itemId && a.IsActive
            );
            return await MapToDtoList(adjustments);
        }

        public async Task<IEnumerable<StockAdjustmentDto>> GetAdjustmentsByStoreAsync(int? storeId)
        {
            var adjustments = await _unitOfWork.StockAdjustments.FindAsync(
                a => a.StoreId == storeId && a.IsActive
            );
            return await MapToDtoList(adjustments);
        }

        public async Task<IEnumerable<StockAdjustmentDto>> GetAdjustmentsByTypeAsync(string adjustmentType)
        {
            var adjustments = await _unitOfWork.StockAdjustments.FindAsync(
                a => a.AdjustmentType.ToString() == adjustmentType && a.IsActive
            );
            return await MapToDtoList(adjustments);
        }

        public async Task<IEnumerable<StockAdjustmentDto>> GetApprovedAdjustmentsAsync()
        {
            var adjustments = await _unitOfWork.StockAdjustments.FindAsync(
                a => a.Status == "Approved" && a.IsActive
            );
            return await MapToDtoList(adjustments);
        }

        public async Task<decimal> GetTotalAdjustmentValueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = await _unitOfWork.StockAdjustments.FindAsync(a => a.IsActive && a.Status == "Approved");

            if (fromDate.HasValue)
                query = query.Where(a => a.AdjustmentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.AdjustmentDate <= toDate.Value);

            // This would need item prices to calculate actual value
            // For now, return quantity adjustments
            return (decimal)query.Sum(a => a.AdjustmentQuantity);
        }

        public async Task<bool> AdjustmentNoExistsAsync(string adjustmentNo)
        {
            return await _unitOfWork.StockAdjustments.ExistsAsync(a => a.AdjustmentNo == adjustmentNo);
        }

        public async Task<IEnumerable<StockAdjustmentDto>> GetPagedAdjustmentsAsync(int pageNumber, int pageSize)
        {
            var adjustments = await _unitOfWork.StockAdjustments.GetPagedAsync(pageNumber, pageSize, a => a.IsActive);
            return await MapToDtoList(adjustments);
        }

        private StockAdjustmentDto MapToDto(StockAdjustment adjustment, Item item, Store store)
        {
            return new StockAdjustmentDto
            {
                Id = adjustment.Id,
                AdjustmentNo = adjustment.AdjustmentNo,
                AdjustmentDate = adjustment.AdjustmentDate,
                ItemId = adjustment.ItemId,
                ItemName = item?.Name,
                ItemCode = item?.ItemCode,
                StoreId = adjustment.StoreId,
                StoreName = store?.Name,
                OldQuantity = adjustment.OldQuantity,
                NewQuantity = adjustment.NewQuantity,
                AdjustmentQuantity = adjustment.AdjustmentQuantity,
                AdjustmentType = adjustment.AdjustmentType.ToString(),
                Reason = adjustment.Reason,
                Status = adjustment.Status,
                ApprovedBy = adjustment.ApprovedBy,
                ApprovedDate = adjustment.ApprovedDate,
                RejectionReason = adjustment.RejectionReason,
                CreatedAt = adjustment.CreatedAt,
                CreatedBy = adjustment.CreatedBy,
                AdjustmentPercentage = adjustment.OldQuantity > 0
                    ? $"{((adjustment.NewQuantity - adjustment.OldQuantity) / adjustment.OldQuantity * 100):F2}%"
                    : "N/A"
            };
        }

        private async Task<IEnumerable<StockAdjustmentDto>> MapToDtoList(IEnumerable<StockAdjustment> adjustments)
        {
            var adjustmentDtos = new List<StockAdjustmentDto>();
            foreach (var adjustment in adjustments)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(adjustment.ItemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(adjustment.StoreId);
                adjustmentDtos.Add(MapToDto(adjustment, item, store));
            }
            return adjustmentDtos.OrderByDescending(a => a.AdjustmentDate);
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        }

        private bool HasApprovalPermission()
        {
            // Check if user has Admin or Manager role
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.IsInRole("Admin") == true || user?.IsInRole("Manager") == true;
        }

        public async Task<StockAdjustmentDto> CreateAdjustmentAsync(StockAdjustmentDto adjustment)
        {
            // Implementation
            return adjustment;
        }

        public async Task<StockAdjustmentDto> GetAdjustmentAsync(int id)
        {
            // Implementation
            return new StockAdjustmentDto();
        }

        public async Task<List<StockAdjustmentDto>> GetAdjustmentsAsync()
        {
            // Implementation
            return new List<StockAdjustmentDto>();
        }

        public async Task<bool> ProcessAdjustmentAsync(int id)
        {
            // Implementation
            return true;
        }

        #region Smart WriteOff Integration

        /// <summary>
        /// Determines if an adjustment should trigger automatic WriteOff creation
        /// </summary>
        private bool ShouldTriggerWriteOff(string reason, string adjustmentType)
        {
            // Only negative adjustments can trigger write-offs
            if (adjustmentType != "Decrease")
                return false;

            // Check if reason matches any write-off trigger reasons
            return WRITEOFF_REASONS.Any(r => reason.Contains(r, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Auto-creates a WriteOff request from a Stock Adjustment
        /// </summary>
        private async Task CreateWriteOffFromAdjustmentAsync(
            StockAdjustment adjustment,
            Item item,
            Store store,
            decimal totalValue)
        {
            try
            {
                // Create WriteOff DTO
                var writeOffDto = new WriteOffDto
                {
                    WriteOffDate = adjustment.AdjustmentDate,
                    Reason = adjustment.Reason,
                    Items = new List<WriteOffItemDto>
                    {
                        new WriteOffItemDto
                        {
                            ItemId = item.Id,
                            ItemName = item.Name,
                            ItemCode = item.ItemCode,
                            StoreId = adjustment.StoreId,
                            StoreName = store?.Name,
                            Quantity = (decimal)adjustment.AdjustmentQuantity,
                            UnitPrice = item.UnitPrice ?? 0,
                            Value = totalValue
                        }
                    },
                    TotalValue = totalValue,
                    ApprovalComments = $"Auto-created from Stock Adjustment #{adjustment.AdjustmentNo}. Reason: {adjustment.Reason}",
                    CreatedBy = adjustment.CreatedBy
                };

                // Create the WriteOff
                var createdWriteOff = await _writeOffService.CreateWriteOffAsync(writeOffDto);

                if (createdWriteOff != null)
                {
                    // Send notification based on value
                    string notificationTitle;
                    string notificationType;
                    string message;

                    if (totalValue >= WRITEOFF_APPROVAL_THRESHOLD)
                    {
                        notificationTitle = "⚠️ High-Value WriteOff Created";
                        notificationType = "warning";
                        message = $"WriteOff #{createdWriteOff.WriteOffNo} for ₹{totalValue:N2} requires approval. " +
                                 $"Linked to adjustment #{adjustment.AdjustmentNo}.";
                    }
                    else
                    {
                        notificationTitle = "WriteOff Created";
                        notificationType = "info";
                        message = $"WriteOff #{createdWriteOff.WriteOffNo} for ₹{totalValue:N2} has been created. " +
                                 $"Linked to adjustment #{adjustment.AdjustmentNo}.";
                    }

                    // Notify creator
                    await _notificationService.SendNotificationAsync(
                        GetCurrentUserId(),
                        notificationTitle,
                        message,
                        notificationType
                    );

                    // If high value, notify approvers
                    if (totalValue >= WRITEOFF_APPROVAL_THRESHOLD)
                    {
                        await NotifyApproversAsync(createdWriteOff.WriteOffNo, totalValue, adjustment.Reason);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the adjustment creation
                await _activityLogService.LogActivityAsync(
                    "Stock Adjustment",
                    adjustment.Id,
                    "WriteOffError",
                    $"Failed to auto-create WriteOff: {ex.Message}",
                    GetCurrentUserId()
                );

                // Send error notification
                await _notificationService.SendNotificationAsync(
                    GetCurrentUserId(),
                    "WriteOff Creation Failed",
                    $"Could not auto-create WriteOff for adjustment #{adjustment.AdjustmentNo}. " +
                    $"Please create it manually. Error: {ex.Message}",
                    "error"
                );
            }
        }

        /// <summary>
        /// Notifies relevant approvers about high-value write-off
        /// </summary>
        private async Task NotifyApproversAsync(string writeOffNo, decimal value, string reason)
        {
            // Get approver roles based on value threshold
            var approverRoles = new List<string> { "DDProvision", "DDStore", "ADStore", "DDGAdmin" };

            string message = $"High-value WriteOff {writeOffNo} requires your approval. " +
                           $"Value: ₹{value:N2}, Reason: {reason}";

            // In a real implementation, you would get users by role and notify each
            // For now, we'll use a system notification
            await _notificationService.SendNotificationAsync(
                "system",
                "🔔 High-Value WriteOff Approval Required",
                message,
                "warning"
            );
        }

        #endregion
    }
}