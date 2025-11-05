using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IMS.Application.Services
{
    public class BattalionStoreService : IBattalionStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BattalionStoreService> _logger;
        private readonly IActivityLogService _activityLogService;

        public BattalionStoreService(
            IUnitOfWork unitOfWork,
            ILogger<BattalionStoreService> logger,
            IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _activityLogService = activityLogService;
        }

        public async Task<IEnumerable<BattalionStoreDto>> GetAllBattalionStoresAsync()
        {
            try
            {
                var battalionStores = await _unitOfWork.BattalionStores.GetAllAsync();
                var battalionStoreDtos = new List<BattalionStoreDto>();

                foreach (var bs in battalionStores)
                {
                    var dto = await MapToDtoAsync(bs);
                    battalionStoreDtos.Add(dto);
                }

                return battalionStoreDtos.OrderBy(bs => bs.BattalionName).ThenBy(bs => bs.StoreName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all battalion stores");
                throw;
            }
        }

        public async Task<IEnumerable<BattalionStoreDto>> GetBattalionStoresByBattalionAsync(int battalionId)
        {
            try
            {
                var battalionStores = await _unitOfWork.BattalionStores.FindAsync(bs => bs.BattalionId == battalionId);
                var battalionStoreDtos = new List<BattalionStoreDto>();

                foreach (var bs in battalionStores)
                {
                    var dto = await MapToDtoAsync(bs);
                    battalionStoreDtos.Add(dto);
                }

                return battalionStoreDtos.OrderBy(bs => bs.StoreName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving battalion stores for battalion {BattalionId}", battalionId);
                throw;
            }
        }

        public async Task<IEnumerable<BattalionStoreDto>> GetBattalionStoresByStoreAsync(int? storeId)
        {
            try
            {
                var battalionStores = await _unitOfWork.BattalionStores.FindAsync(bs => bs.StoreId == storeId);
                var battalionStoreDtos = new List<BattalionStoreDto>();

                foreach (var bs in battalionStores)
                {
                    var dto = await MapToDtoAsync(bs);
                    battalionStoreDtos.Add(dto);
                }

                return battalionStoreDtos.OrderBy(bs => bs.BattalionName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving battalion stores for store {StoreId}", storeId);
                throw;
            }
        }

        public async Task<BattalionStoreDto> GetBattalionStoreByIdAsync(int id)
        {
            try
            {
                var battalionStore = await _unitOfWork.BattalionStores.GetByIdAsync(id);
                return battalionStore != null ? await MapToDtoAsync(battalionStore) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving battalion store with ID {Id}", id);
                throw;
            }
        }

        public async Task<BattalionStoreDto> AssignBattalionToStoreAsync(BattalionStoreDto battalionStoreDto)
        {
            try
            {
                // Check if assignment already exists
                if (await IsBattalionAssignedToStoreAsync(battalionStoreDto.BattalionId, battalionStoreDto.StoreId))
                    throw new InvalidOperationException("Battalion is already assigned to this store");

                var battalionStore = new BattalionStore
                {
                    BattalionId = battalionStoreDto.BattalionId,
                    StoreId = battalionStoreDto.StoreId,
                    IsPrimaryStore = battalionStoreDto.IsPrimaryStore,
                    EffectiveFrom = battalionStoreDto.EffectiveFrom,
                    EffectiveTo = battalionStoreDto.EffectiveTo,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = battalionStoreDto.CreatedBy ?? "System"
                };

                // If this is set as primary, update other assignments
                if (battalionStore.IsPrimaryStore)
                {
                    await ClearPrimaryStoreForBattalionAsync(battalionStore.BattalionId);
                }

                await _unitOfWork.BattalionStores.AddAsync(battalionStore);
                await _unitOfWork.CompleteAsync();

                // Also update the Store entity's BattalionId if this is primary
                if (battalionStore.IsPrimaryStore)
                {
                    var store = await _unitOfWork.Stores.GetByIdAsync(battalionStore.StoreId);
                    if (store != null)
                    {
                        store.BattalionId = battalionStore.BattalionId;
                        _unitOfWork.Stores.Update(store);
                        await _unitOfWork.CompleteAsync();
                    }
                }

                await _activityLogService.LogActivityAsync(
                    "BattalionStore",
                    battalionStore.Id,
                    "Assign",
                    $"Assigned battalion {battalionStoreDto.BattalionName} to store {battalionStoreDto.StoreName}",
                    battalionStoreDto.CreatedBy ?? "System"
                );

                return await MapToDtoAsync(battalionStore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning battalion to store");
                throw;
            }
        }

        public async Task UpdateBattalionStoreAsync(BattalionStoreDto battalionStoreDto)
        {
            try
            {
                var battalionStore = await _unitOfWork.BattalionStores.GetByIdAsync(battalionStoreDto.Id);
                if (battalionStore == null)
                    throw new InvalidOperationException($"Battalion store assignment with ID {battalionStoreDto.Id} not found");

                // If setting as primary, clear other primary assignments
                if (battalionStoreDto.IsPrimaryStore && !battalionStore.IsPrimaryStore)
                {
                    await ClearPrimaryStoreForBattalionAsync(battalionStore.BattalionId);
                }

                battalionStore.IsPrimaryStore = battalionStoreDto.IsPrimaryStore;
                battalionStore.EffectiveFrom = battalionStoreDto.EffectiveFrom;
                battalionStore.EffectiveTo = battalionStoreDto.EffectiveTo;
                battalionStore.UpdatedAt = DateTime.UtcNow;
                battalionStore.UpdatedBy = battalionStoreDto.CreatedBy ?? "System";

                _unitOfWork.BattalionStores.Update(battalionStore);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "BattalionStore",
                    battalionStore.Id,
                    "Update",
                    $"Updated battalion store assignment",
                    battalionStoreDto.CreatedBy ?? "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating battalion store");
                throw;
            }
        }

        public async Task RemoveBattalionFromStoreAsync(int id)
        {
            try
            {
                var battalionStore = await _unitOfWork.BattalionStores.GetByIdAsync(id);
                if (battalionStore == null)
                    throw new InvalidOperationException($"Battalion store assignment with ID {id} not found");

                _unitOfWork.BattalionStores.Remove(battalionStore);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Removed battalion store assignment {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing battalion from store");
                throw;
            }
        }

        public async Task<bool> IsBattalionAssignedToStoreAsync(int battalionId, int? storeId)
        {
            var now = DateTime.UtcNow;
            return await _unitOfWork.BattalionStores.ExistsAsync(bs =>
                bs.BattalionId == battalionId &&
                bs.StoreId == storeId &&
                bs.EffectiveFrom <= now &&
                (bs.EffectiveTo == null || bs.EffectiveTo > now));
        }

        public async Task SetPrimaryStoreForBattalionAsync(int battalionId, int? storeId)
        {
            try
            {
                // Clear existing primary
                await ClearPrimaryStoreForBattalionAsync(battalionId);

                // Set new primary
                var battalionStore = await _unitOfWork.BattalionStores.FirstOrDefaultAsync(bs =>
                    bs.BattalionId == battalionId &&
                    bs.StoreId == storeId &&
                    bs.EffectiveTo == null);

                if (battalionStore == null)
                    throw new InvalidOperationException("Battalion is not assigned to this store");

                battalionStore.IsPrimaryStore = true;
                battalionStore.UpdatedAt = DateTime.UtcNow;
                battalionStore.UpdatedBy = "System";

                _unitOfWork.BattalionStores.Update(battalionStore);
                await _unitOfWork.CompleteAsync();

                // Update Store entity's BattalionId
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store != null)
                {
                    store.BattalionId = battalionId;
                    _unitOfWork.Stores.Update(store);
                    await _unitOfWork.CompleteAsync();
                }

                _logger.LogInformation("Set primary store {StoreId} for battalion {BattalionId}", storeId, battalionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting primary store for battalion");
                throw;
            }
        }

        public async Task<StoreDto> GetBattalionPrimaryStoreAsync(int battalionId)
        {
            try
            {
                var battalionStore = await _unitOfWork.BattalionStores.FirstOrDefaultAsync(bs =>
                    bs.BattalionId == battalionId &&
                    bs.IsPrimaryStore &&
                    bs.EffectiveTo == null);

                if (battalionStore == null)
                    return null;

                var store = await _unitOfWork.Stores.GetByIdAsync(battalionStore.StoreId);
                if (store == null)
                    return null;

                return new StoreDto
                {
                    Id = store.Id,
                    Name = store.Name,
                    Code = store.Code,
                    StoreTypeId = store.StoreTypeId,
                    StoreTypeName = store.StoreType?.Name,
                    Level = store.Level,
                    Location = store.Location,
                    IsActive = store.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting battalion primary store");
                throw;
            }
        }

        public async Task<IEnumerable<BattalionStoreDto>> GetActiveBattalionStoresAsync(DateTime? asOfDate = null)
        {
            try
            {
                var date = asOfDate ?? DateTime.UtcNow;
                var battalionStores = await _unitOfWork.BattalionStores.FindAsync(bs =>
                    bs.EffectiveFrom <= date &&
                    (bs.EffectiveTo == null || bs.EffectiveTo > date));

                var battalionStoreDtos = new List<BattalionStoreDto>();

                foreach (var bs in battalionStores)
                {
                    var dto = await MapToDtoAsync(bs);
                    battalionStoreDtos.Add(dto);
                }

                return battalionStoreDtos.OrderBy(bs => bs.BattalionName).ThenBy(bs => bs.StoreName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active battalion stores");
                throw;
            }
        }

        public async Task<IEnumerable<BattalionStoreDto>> GetBattalionStoreHistoryAsync(int battalionId)
        {
            try
            {
                var battalionStores = await _unitOfWork.BattalionStores.FindAsync(bs => bs.BattalionId == battalionId);
                var battalionStoreDtos = new List<BattalionStoreDto>();

                foreach (var bs in battalionStores)
                {
                    var dto = await MapToDtoAsync(bs);
                    battalionStoreDtos.Add(dto);
                }

                return battalionStoreDtos.OrderByDescending(bs => bs.EffectiveFrom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving battalion store history");
                throw;
            }
        }

        public async Task EndBattalionStoreAssignmentAsync(int battalionId, int? storeId, DateTime effectiveTo)
        {
            try
            {
                var battalionStore = await _unitOfWork.BattalionStores.FirstOrDefaultAsync(bs =>
                    bs.BattalionId == battalionId &&
                    bs.StoreId == storeId &&
                    bs.EffectiveTo == null);

                if (battalionStore == null)
                    throw new InvalidOperationException("Active battalion store assignment not found");

                battalionStore.EffectiveTo = effectiveTo;
                battalionStore.UpdatedAt = DateTime.UtcNow;
                battalionStore.UpdatedBy = "System";

                _unitOfWork.BattalionStores.Update(battalionStore);
                await _unitOfWork.CompleteAsync();

                // If this was the primary store, update the Store entity
                if (battalionStore.IsPrimaryStore)
                {
                    var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                    if (store != null && store.BattalionId == battalionId)
                    {
                        store.BattalionId = null;
                        _unitOfWork.Stores.Update(store);
                        await _unitOfWork.CompleteAsync();
                    }
                }

                _logger.LogInformation("Ended battalion store assignment for battalion {BattalionId} and store {StoreId}", battalionId, storeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending battalion store assignment");
                throw;
            }
        }

        private async Task ClearPrimaryStoreForBattalionAsync(int battalionId)
        {
            var primaryStores = await _unitOfWork.BattalionStores.FindAsync(bs =>
                bs.BattalionId == battalionId &&
                bs.IsPrimaryStore &&
                bs.EffectiveTo == null);

            foreach (var store in primaryStores)
            {
                store.IsPrimaryStore = false;
                store.UpdatedAt = DateTime.UtcNow;
                store.UpdatedBy = "System";
                _unitOfWork.BattalionStores.Update(store);
            }

            // Also clear from Store entities
            var stores = await _unitOfWork.Stores.FindAsync(s => s.BattalionId == battalionId);
            foreach (var store in stores)
            {
                store.BattalionId = null;
                _unitOfWork.Stores.Update(store);
            }
        }

        private async Task<BattalionStoreDto> MapToDtoAsync(BattalionStore battalionStore)
        {
            var dto = new BattalionStoreDto
            {
                Id = battalionStore.Id,
                BattalionId = battalionStore.BattalionId,
                StoreId = battalionStore.StoreId,
                IsPrimaryStore = battalionStore.IsPrimaryStore,
                EffectiveFrom = battalionStore.EffectiveFrom,
                EffectiveTo = battalionStore.EffectiveTo,
                CreatedAt = battalionStore.CreatedAt,
                CreatedBy = battalionStore.CreatedBy
            };

            // Get battalion details
            var battalion = await _unitOfWork.Battalions.GetByIdAsync(battalionStore.BattalionId);
            if (battalion != null)
            {
                dto.BattalionName = battalion.Name;
                dto.BattalionCode = battalion.Code;
            }

            // Get store details
            var store = await _unitOfWork.Stores.GetByIdAsync(battalionStore.StoreId);
            if (store != null)
            {
                dto.StoreName = store.Name;
                dto.StoreCode = store.Code;
                dto.StoreLocation = store.Location;
            }

            return dto;
        }
    }
}