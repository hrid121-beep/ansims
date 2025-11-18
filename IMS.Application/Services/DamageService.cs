using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace IMS.Application.Services
{
    public class DamageService : IDamageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWriteOffService _writeOffService;
        private readonly IUserContext _userContext;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;

        public DamageService(
            IUnitOfWork unitOfWork,
            IWriteOffService writeOffService,
            IUserContext userContext,
            IActivityLogService activityLogService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _writeOffService = writeOffService;
            _userContext = userContext;
            _activityLogService = activityLogService;
            _notificationService = notificationService;
        }

        #region Get Methods

        public async Task<IEnumerable<DamageDto>> GetAllDamagesAsync()
        {
            var damageReports = await _unitOfWork.DamageReports.GetAllAsync(
                includes: new[] { "Items", "Items.Item", "Store" });

            var damageDtos = new List<DamageDto>();

            foreach (var report in damageReports.Where(d => d.IsActive))
            {
                damageDtos.Add(await MapToDtoAsync(report));
            }

            return damageDtos.OrderByDescending(d => d.DamageDate);
        }

        public async Task<DamageDto> GetDamageByIdAsync(int id)
        {
            var damageReport = await _unitOfWork.DamageReports.GetAsync(
                d => d.Id == id && d.IsActive,
                includes: new[] { "Items", "Items.Item", "Store" });

            if (damageReport == null) return null;

            return await MapToDtoAsync(damageReport);
        }

        public async Task<IEnumerable<DamageDto>> GetDamagesByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var damageReports = await _unitOfWork.DamageReports.GetAllAsync(
                predicate: d => d.IsActive && d.ReportDate >= fromDate && d.ReportDate <= toDate,
                includes: new[] { "Items", "Items.Item", "Store" });

            var damageDtos = new List<DamageDto>();
            foreach (var report in damageReports)
            {
                damageDtos.Add(await MapToDtoAsync(report));
            }

            return damageDtos.OrderByDescending(d => d.DamageDate);
        }

        public async Task<IEnumerable<DamageDto>> GetDamagesByItemAsync(int? itemId)
        {
            if (!itemId.HasValue) return Enumerable.Empty<DamageDto>();

            var damageReports = await _unitOfWork.DamageReports.GetAllAsync(
                predicate: d => d.IsActive && d.ItemId == itemId.Value,
                includes: new[] { "Items", "Items.Item", "Store" });

            var damageDtos = new List<DamageDto>();
            foreach (var report in damageReports)
            {
                damageDtos.Add(await MapToDtoAsync(report));
            }

            return damageDtos.OrderByDescending(d => d.DamageDate);
        }

        public async Task<IEnumerable<DamageDto>> GetDamagesByStoreAsync(int? storeId)
        {
            if (!storeId.HasValue) return Enumerable.Empty<DamageDto>();

            var damageReports = await _unitOfWork.DamageReports.GetAllAsync(
                predicate: d => d.IsActive && d.StoreId == storeId.Value,
                includes: new[] { "Items", "Items.Item", "Store" });

            var damageDtos = new List<DamageDto>();
            foreach (var report in damageReports)
            {
                damageDtos.Add(await MapToDtoAsync(report));
            }

            return damageDtos.OrderByDescending(d => d.DamageDate);
        }

        public async Task<IEnumerable<DamageDto>> GetDamagesByTypeAsync(string damageType)
        {
            if (string.IsNullOrWhiteSpace(damageType)) return Enumerable.Empty<DamageDto>();

            var damageReports = await _unitOfWork.DamageReports.GetAllAsync(
                predicate: d => d.IsActive && d.DamageType.Equals(damageType, StringComparison.OrdinalIgnoreCase),
                includes: new[] { "Items", "Items.Item", "Store" });

            var damageDtos = new List<DamageDto>();
            foreach (var report in damageReports)
            {
                damageDtos.Add(await MapToDtoAsync(report));
            }

            return damageDtos.OrderByDescending(d => d.DamageDate);
        }

        public async Task<IEnumerable<DamageDto>> GetDamagesByStatusAsync(DamageStatus status)
        {
            var damageReports = await _unitOfWork.DamageReports.GetAllAsync(
                predicate: d => d.IsActive && d.Status == status,
                includes: new[] { "Items", "Items.Item", "Store" });

            var damageDtos = new List<DamageDto>();
            foreach (var report in damageReports)
            {
                damageDtos.Add(await MapToDtoAsync(report));
            }

            return damageDtos.OrderByDescending(d => d.DamageDate);
        }

        public async Task<IEnumerable<DamageDto>> GetPagedDamagesAsync(int pageNumber, int pageSize)
        {
            var allDamages = await GetAllDamagesAsync();
            return allDamages
                .OrderByDescending(d => d.DamageDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        #endregion

        #region Create/Update Methods

        public async Task<DamageDto> CreateDamageAsync(DamageDto damageDto)
        {
            var currentUser = _userContext.GetCurrentUserName();
            var userId = _userContext.GetCurrentUserId();

            // Create DamageReport
            var damageReport = new DamageReport
            {
                ReportNo = await GenerateDamageNoAsync(),
                StoreId = damageDto.StoreId ?? 0,
                ReportDate = damageDto.DamageDate,
                ReportedBy = currentUser,
                Status = DamageStatus.Reported,
                ItemId = damageDto.ItemId,
                Quantity = damageDto.Quantity ?? 0,
                DamageType = damageDto.DamageType,
                Cause = damageDto.Cause,
                EstimatedLoss = damageDto.EstimatedLoss,
                TotalValue = 0, // Will be calculated from items
                CreatedAt = DateTime.Now,
                CreatedBy = currentUser,
                IsActive = true
            };

            await _unitOfWork.DamageReports.AddAsync(damageReport);
            await _unitOfWork.CompleteAsync();

            // Calculate total value from item price
            var item = await _unitOfWork.Items.GetByIdAsync(damageDto.ItemId);
            var itemValue = (item?.UnitCost ?? 0) * (damageDto.Quantity ?? 0);

            damageReport.TotalValue = itemValue;
            _unitOfWork.DamageReports.Update(damageReport);
            await _unitOfWork.CompleteAsync();

            // Log activity
            await _activityLogService.LogActivityAsync(
                "Damage Report",
                damageReport.Id,
                "Create",
                $"Created damage report {damageReport.ReportNo}",
                userId);

            // Create notification
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "New Damage Report",
                Message = $"Damage report {damageReport.ReportNo} has been created",
                Type = NotificationType.Alert.ToString(),
                Priority = NotificationPriority.High.ToString(),
                CreatedBy = currentUser
            });

            // Check if needs auto write-off (value >= 10,000)
            if (itemValue >= 10000)
            {
                await CreateWriteOffRequestFromDamageAsync(damageReport.Id, currentUser);
            }

            return await GetDamageByIdAsync(damageReport.Id);
        }

        public async Task<DamageDto> CreateMultiItemDamageAsync(DamageDto damageDto, List<DamageItemDto> items)
        {
            var currentUser = _userContext.GetCurrentUserName();
            var userId = _userContext.GetCurrentUserId();

            // Create DamageReport
            var damageReport = new DamageReport
            {
                ReportNo = await GenerateDamageNoAsync(),
                StoreId = damageDto.StoreId ?? 0,
                ReportDate = damageDto.DamageDate,
                ReportedBy = currentUser,
                Status = DamageStatus.Reported,
                ItemId = items.First().ItemId, // Primary item
                Quantity = items.Sum(i => i.Quantity),
                DamageType = damageDto.DamageType,
                Cause = damageDto.Cause,
                EstimatedLoss = 0, // Will calculate
                TotalValue = 0,
                CreatedAt = DateTime.Now,
                CreatedBy = currentUser,
                IsActive = true
            };

            await _unitOfWork.DamageReports.AddAsync(damageReport);
            await _unitOfWork.CompleteAsync();

            // Add items
            decimal totalValue = 0;
            foreach (var itemDto in items)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(itemDto.ItemId);
                var itemValue = (item?.UnitCost ?? 0) * itemDto.Quantity;
                totalValue += itemValue;

                var damageReportItem = new DamageReportItem
                {
                    DamageReportId = damageReport.Id,
                    ItemId = itemDto.ItemId,
                    DamagedQuantity = itemDto.Quantity,
                    DamageType = itemDto.DamageType ?? damageDto.DamageType,
                    DamageDate = damageDto.DamageDate,
                    DiscoveredDate = DateTime.Now,
                    DamageDescription = itemDto.Description,
                    EstimatedValue = itemValue,
                    PhotoUrls = itemDto.PhotoUrls != null ? JsonSerializer.Serialize(itemDto.PhotoUrls) : null,
                    BatchNo = itemDto.BatchNo,
                    Remarks = itemDto.Remarks,
                    CreatedAt = DateTime.Now,
                    CreatedBy = currentUser,
                    IsActive = true
                };

                await _unitOfWork.DamageReportItems.AddAsync(damageReportItem);
            }

            // Update total value
            damageReport.TotalValue = totalValue;
            damageReport.EstimatedLoss = totalValue;
            _unitOfWork.DamageReports.Update(damageReport);
            await _unitOfWork.CompleteAsync();

            // Log activity
            await _activityLogService.LogActivityAsync(
                "Damage Report",
                damageReport.Id,
                "Create",
                $"Created multi-item damage report {damageReport.ReportNo} with {items.Count} items, total value: {totalValue:N2}",
                userId);

            // Create notification
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "New Multi-Item Damage Report",
                Message = $"Damage report {damageReport.ReportNo} created with {items.Count} items (Total: ৳{totalValue:N2})",
                Type = NotificationType.Alert.ToString(),
                Priority = (totalValue >= 10000 ? NotificationPriority.Critical : NotificationPriority.High).ToString(),
                CreatedBy = currentUser
            });

            // Auto create write-off request if value >= 10,000
            if (totalValue >= 10000)
            {
                await CreateWriteOffRequestFromDamageAsync(damageReport.Id, currentUser);
            }

            return await GetDamageByIdAsync(damageReport.Id);
        }

        public async Task<bool> UpdateDamageStatusAsync(int id, DamageStatus status, string remarks)
        {
            var damageReport = await _unitOfWork.DamageReports.GetByIdAsync(id);
            if (damageReport == null || !damageReport.IsActive) return false;

            var currentUser = _userContext.GetCurrentUserName();
            var userId = _userContext.GetCurrentUserId();

            var oldStatus = damageReport.Status;
            damageReport.Status = status;
            damageReport.UpdatedAt = DateTime.Now;
            damageReport.UpdatedBy = currentUser;

            _unitOfWork.DamageReports.Update(damageReport);
            await _unitOfWork.CompleteAsync();

            // Log activity
            await _activityLogService.LogActivityAsync(
                "Damage Report",
                damageReport.Id,
                "Update",
                $"Updated damage report {damageReport.ReportNo} status from {oldStatus} to {status}. Remarks: {remarks}",
                userId);

            return true;
        }

        #endregion

        #region Write-Off Integration

        private async Task CreateWriteOffRequestFromDamageAsync(int damageReportId, string currentUser)
        {
            var damageReport = await _unitOfWork.DamageReports.GetAsync(
                d => d.Id == damageReportId,
                includes: new[] { "Items", "Items.Item" });

            if (damageReport == null) return;

            // Create WriteOffRequest
            var writeOffRequest = new WriteOffRequest
            {
                RequestNo = await GenerateWriteOffRequestNoAsync(),
                DamageReportId = damageReportId,
                DamageReportNo = damageReport.ReportNo,
                StoreId = damageReport.StoreId,
                RequestDate = DateTime.Now,
                RequestedBy = currentUser,
                TotalValue = damageReport.TotalValue,
                Status = WriteOffStatus.Pending,
                Justification = $"Auto-generated from damage report {damageReport.ReportNo}. Damage type: {damageReport.DamageType}. Cause: {damageReport.Cause}",
                Reason = "High Value Damage",
                CreatedAt = DateTime.Now,
                CreatedBy = currentUser,
                IsActive = true
            };

            await _unitOfWork.WriteOffRequests.AddAsync(writeOffRequest);
            await _unitOfWork.CompleteAsync();

            // Update damage report status
            damageReport.Status = DamageStatus.UnderReview;
            _unitOfWork.DamageReports.Update(damageReport);
            await _unitOfWork.CompleteAsync();

            // Create notification
            await _notificationService.CreateNotificationAsync(new NotificationDto
            {
                Title = "Write-Off Request Auto-Created",
                Message = $"Write-off request {writeOffRequest.RequestNo} has been automatically created from damage report {damageReport.ReportNo} (Value: ৳{damageReport.TotalValue:N2})",
                Type = NotificationType.Approval.ToString(),
                Priority = NotificationPriority.Critical.ToString(),
                CreatedBy = currentUser
            });
        }

        private async Task<string> GenerateWriteOffRequestNoAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _unitOfWork.WriteOffRequests.CountAsync(w => w.CreatedAt.Year == year) + 1;
            return $"WOR-{year}-{count:D4}";
        }

        #endregion

        #region Statistics & Calculations

        public async Task<int> GetDamageCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var damageReports = await _unitOfWork.DamageReports.GetAllAsync();
            var query = damageReports.Where(d => d.IsActive);

            if (fromDate.HasValue)
                query = query.Where(d => d.ReportDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(d => d.ReportDate <= toDate.Value);

            return query.Count();
        }

        public async Task<decimal> GetTotalDamageValueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var damageReports = await _unitOfWork.DamageReports.GetAllAsync();
            var query = damageReports.Where(d => d.IsActive);

            if (fromDate.HasValue)
                query = query.Where(d => d.ReportDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(d => d.ReportDate <= toDate.Value);

            return query.Sum(d => d.TotalValue);
        }

        public async Task<Dictionary<string, int>> GetDamageCountByTypeAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var damageReports = await _unitOfWork.DamageReports.GetAllAsync();
            var query = damageReports.Where(d => d.IsActive);

            if (fromDate.HasValue)
                query = query.Where(d => d.ReportDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(d => d.ReportDate <= toDate.Value);

            return query.GroupBy(d => d.DamageType)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<DamageStatus, int>> GetDamageCountByStatusAsync()
        {
            var damageReports = await _unitOfWork.DamageReports.GetAllAsync();

            return damageReports.Where(d => d.IsActive)
                .GroupBy(d => d.Status)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        #endregion

        #region Helper Methods

        public async Task<string> GenerateDamageNoAsync()
        {
            var year = DateTime.Now.Year;
            var count = await _unitOfWork.DamageReports.CountAsync(d => d.CreatedAt.Year == year) + 1;
            return $"DMG-{year}-{count:D4}";
        }

        public async Task<bool> DamageNoExistsAsync(string damageNo)
        {
            var damageReport = await _unitOfWork.DamageReports.FirstOrDefaultAsync(d => d.ReportNo == damageNo);
            return damageReport != null;
        }

        private async Task<DamageDto> MapToDtoAsync(DamageReport damageReport)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(damageReport.StoreId);
            var item = await _unitOfWork.Items.GetByIdAsync(damageReport.ItemId);

            var damageItems = new List<DamageItemDto>();

            if (damageReport.Items != null && damageReport.Items.Any())
            {
                foreach (var reportItem in damageReport.Items.Where(i => i.IsActive))
                {
                    var itemDetail = await _unitOfWork.Items.GetByIdAsync(reportItem.ItemId);

                    List<string> photoUrls = null;
                    if (!string.IsNullOrWhiteSpace(reportItem.PhotoUrls))
                    {
                        try
                        {
                            photoUrls = JsonSerializer.Deserialize<List<string>>(reportItem.PhotoUrls);
                        }
                        catch { }
                    }

                    damageItems.Add(new DamageItemDto
                    {
                        Id = reportItem.Id,
                        ItemId = reportItem.ItemId,
                        ItemName = itemDetail?.Name,
                        Quantity = reportItem.DamagedQuantity,
                        DamageType = reportItem.DamageType,
                        Description = reportItem.DamageDescription,
                        EstimatedValue = reportItem.EstimatedValue,
                        PhotoUrls = photoUrls,
                        BatchNo = reportItem.BatchNo,
                        Remarks = reportItem.Remarks
                    });
                }
            }

            return new DamageDto
            {
                Id = damageReport.Id,
                DamageNo = damageReport.ReportNo,
                DamageDate = damageReport.ReportDate,
                ItemId = damageReport.ItemId,
                ItemName = item?.Name,
                StoreId = damageReport.StoreId,
                StoreName = store?.Name,
                Quantity = damageReport.Quantity,
                DamageType = damageReport.DamageType,
                Cause = damageReport.Cause,
                Status = damageReport.Status.ToString(),
                ReportedBy = damageReport.ReportedBy,
                EstimatedLoss = damageReport.EstimatedLoss,
                TotalValue = damageReport.TotalValue,
                Items = damageItems
            };
        }

        /// <summary>
        /// Deletes a damage report with comprehensive validation
        /// </summary>
        public async Task<bool> DeleteDamageAsync(int damageId, string deletedBy)
        {
            var damageReport = await _unitOfWork.DamageReports
                .Query()
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.Id == damageId);

            if (damageReport == null)
                throw new InvalidOperationException("Damage report not found");

            // 1. Status Check - Only allow deletion of Reported or Rejected damages
            if (damageReport.Status != DamageStatus.Reported &&
                damageReport.Status != DamageStatus.Rejected &&
                damageReport.Status != DamageStatus.Draft)
            {
                throw new InvalidOperationException(
                    "Only Reported, Draft, or Rejected damage reports can be deleted. " +
                    $"Current status: {damageReport.Status}");
            }

            // 2. CRITICAL: Check if WriteOffRequest exists for this damage
            var hasWriteOffRequest = await _unitOfWork.WriteOffRequests
                .Query()
                .AnyAsync(wr => wr.DamageReportId == damageId);

            if (hasWriteOffRequest)
            {
                throw new InvalidOperationException(
                    "Cannot delete - A Write-Off request has been created for this damage report. " +
                    "The damage has been processed into the write-off workflow. " +
                    "Please cancel or delete the write-off request first.");
            }

            // 3. Check StockMovement records
            var hasStockMovements = await _unitOfWork.StockMovements
                .Query()
                .AnyAsync(sm => (sm.ReferenceType == "Damage" || sm.ReferenceType == "DamageReport")
                    && sm.ReferenceId == damageId);

            if (hasStockMovements)
            {
                throw new InvalidOperationException(
                    "Cannot delete - Stock movement records exist for this damage report. " +
                    "This damage has already impacted inventory. Contact administrator for reversal.");
            }

            // 4. Check if damage is Approved or Processed
            if (damageReport.Status == DamageStatus.Approved ||
                damageReport.Status == DamageStatus.Processed ||
                damageReport.Status == DamageStatus.WriteOffCreated)
            {
                throw new InvalidOperationException(
                    "Cannot delete approved or processed damage reports. This violates audit trail integrity.");
            }

            // 5. Soft Delete (NEVER hard delete transactional data!)
            damageReport.IsActive = false;
            damageReport.UpdatedBy = deletedBy;
            damageReport.UpdatedAt = DateTime.Now;

            _unitOfWork.DamageReports.Update(damageReport);
            await _unitOfWork.CompleteAsync();

            // 6. Log the deletion
            await _activityLogService.LogActivityAsync(
                "Damage Report",
                damageReport.Id,
                "Delete",
                $"Damage report {damageReport.ReportNo} deleted by {deletedBy}. " +
                $"Type: {damageReport.DamageType}, Items: {damageReport.Items?.Count ?? 0}, " +
                $"Value: ₹{damageReport.TotalValue:N2}",
                deletedBy
            );

            return true;
        }

        #endregion
    }
}
