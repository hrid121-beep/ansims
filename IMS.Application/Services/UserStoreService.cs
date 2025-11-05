using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace IMS.Application.Services
{
    public class UserStoreService : IUserStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserStoreService> _logger;
        private readonly IActivityLogService _activityLogService;
        private readonly UserManager<User> _userManager;

        public UserStoreService(
            IUnitOfWork unitOfWork,
            ILogger<UserStoreService> logger,
            IActivityLogService activityLogService,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _activityLogService = activityLogService;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserStoreDto>> GetAllUserStoresAsync()
        {
            try
            {
                var userStores = await _unitOfWork.UserStores.GetAllAsync();
                var userStoreDtos = new List<UserStoreDto>();

                foreach (var userStore in userStores)
                {
                    var dto = await MapToDtoAsync(userStore);
                    userStoreDtos.Add(dto);
                }

                return userStoreDtos.OrderBy(us => us.UserFullName).ThenBy(us => us.StoreName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all user stores");
                throw;
            }
        }

        public async Task<IEnumerable<UserStoreDto>> GetUserStoresByStoreAsync(int? storeId)
        {
            try
            {
                var userStores = await _unitOfWork.UserStores.FindAsync(us => us.StoreId == storeId);
                var userStoreDtos = new List<UserStoreDto>();

                foreach (var userStore in userStores)
                {
                    var dto = await MapToDtoAsync(userStore);
                    userStoreDtos.Add(dto);
                }

                return userStoreDtos.OrderBy(us => us.UserFullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user stores for store {StoreId}", storeId);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetStoreUsersAsync(int? storeId)
        {
            try
            {
                var userStores = await _unitOfWork.UserStores.FindAsync(us => us.StoreId == storeId && us.IsActive);
                var users = new List<UserDto>();

                foreach (var userStore in userStores)
                {
                    var user = await _userManager.FindByIdAsync(userStore.UserId);
                    if (user != null)
                    {
                        users.Add(new UserDto
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            FullName = user.FullName,
                            Designation = user.Designation,
                            BadgeNumber = user.BadgeNumber,
                            IsActive = user.IsActive
                        });
                    }
                }

                return users.OrderBy(u => u.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users for store {StoreId}", storeId);
                throw;
            }
        }

        public async Task<IEnumerable<StoreDto>> GetUserStoresAsync(string userId)
        {
            try
            {
                var userStores = await _unitOfWork.UserStores.FindAsync(us => us.UserId == userId && us.IsActive);
                var stores = new List<StoreDto>();

                foreach (var userStore in userStores)
                {
                    var store = await _unitOfWork.Stores.GetByIdAsync(userStore.StoreId);
                    if (store != null)
                    {
                        stores.Add(new StoreDto
                        {
                            Id = store.Id,
                            Name = store.Name,
                            Code = store.Code,
                            StoreTypeId = store.StoreTypeId,
                            StoreTypeName = store.StoreType?.Name,
                            StoreTypeCode = store.StoreType?.Code,
                            Level = store.Level,
                            Location = store.Location,
                            IsActive = store.IsActive
                        });
                    }
                }

                return stores.OrderBy(s => s.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stores for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserStoreDto> AssignUserToStoreAsync(UserStoreDto userStoreDto)
        {
            try
            {
                // Check if assignment already exists
                if (await IsUserAssignedToStoreAsync(userStoreDto.UserId, userStoreDto.StoreId))
                    throw new InvalidOperationException("User is already assigned to this store");

                var userStore = new UserStore
                {
                    UserId = userStoreDto.UserId,
                    StoreId = userStoreDto.StoreId,
                    IsPrimary = userStoreDto.IsPrimary,
                    AssignedDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userStoreDto.UserName ?? "System"
                };

                // If this is set as primary, update other assignments
                if (userStore.IsPrimary)
                {
                    await ClearPrimaryStoreForUserAsync(userStore.UserId);
                }

                await _unitOfWork.UserStores.AddAsync(userStore);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "UserStore",
                    userStore.Id,
                    "Assign",
                    $"Assigned user {userStoreDto.UserFullName} to store {userStoreDto.StoreName}",
                    userStoreDto.UserName ?? "System"
                );

                return await MapToDtoAsync(userStore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning user to store");
                throw;
            }
        }

        public async Task UpdateUserStoreAsync(UserStoreDto userStoreDto)
        {
            try
            {
                var userStore = await _unitOfWork.UserStores.GetByIdAsync(userStoreDto.Id);
                if (userStore == null)
                    throw new InvalidOperationException($"User store assignment with ID {userStoreDto.Id} not found");

                // If setting as primary, clear other primary assignments
                if (userStoreDto.IsPrimary && !userStore.IsPrimary)
                {
                    await ClearPrimaryStoreForUserAsync(userStore.UserId);
                }

                userStore.IsPrimary = userStoreDto.IsPrimary;
                userStore.IsActive = userStoreDto.IsActive;
                userStore.UpdatedAt = DateTime.UtcNow;
                userStore.UpdatedBy = userStoreDto.UserName ?? "System";

                _unitOfWork.UserStores.Update(userStore);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "UserStore",
                    userStore.Id,
                    "Update",
                    $"Updated user store assignment",
                    userStoreDto.UserName ?? "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user store");
                throw;
            }
        }

        public async Task RemoveUserFromStoreAsync(int id)
        {
            try
            {
                var userStore = await _unitOfWork.UserStores.GetByIdAsync(id);
                if (userStore == null)
                    throw new InvalidOperationException($"User store assignment with ID {id} not found");

                userStore.UnassignedDate = DateTime.UtcNow;
                userStore.IsActive = false;
                userStore.UpdatedAt = DateTime.UtcNow;
                userStore.UpdatedBy = "System";

                _unitOfWork.UserStores.Update(userStore);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Removed user store assignment {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from store");
                throw;
            }
        }

        public async Task RemoveUserFromStoreAsync(string userId, int? storeId)
        {
            try
            {
                var userStore = await _unitOfWork.UserStores.FirstOrDefaultAsync(us =>
                    us.UserId == userId && us.StoreId == storeId && us.IsActive);

                if (userStore != null)
                {
                    await RemoveUserFromStoreAsync(userStore.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from store");
                throw;
            }
        }

        public async Task<bool> IsUserAssignedToStoreAsync(string userId, int? storeId)
        {
            return await _unitOfWork.UserStores.ExistsAsync(us =>
                us.UserId == userId && us.StoreId == storeId && us.IsActive);
        }

        public async Task SetPrimaryStoreAsync(string userId, int? storeId)
        {
            try
            {
                // Clear existing primary
                await ClearPrimaryStoreForUserAsync(userId);

                // Set new primary
                var userStore = await _unitOfWork.UserStores.FirstOrDefaultAsync(us =>
                    us.UserId == userId && us.StoreId == storeId && us.IsActive);

                if (userStore == null)
                    throw new InvalidOperationException("User is not assigned to this store");

                userStore.IsPrimary = true;
                userStore.UpdatedAt = DateTime.UtcNow;
                userStore.UpdatedBy = "System";

                _unitOfWork.UserStores.Update(userStore);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Set primary store {StoreId} for user {UserId}", storeId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting primary store");
                throw;
            }
        }

        public async Task<StoreDto> GetUserPrimaryStoreAsync(string userId)
        {
            try
            {
                var userStore = await _unitOfWork.UserStores.FirstOrDefaultAsync(us =>
                    us.UserId == userId && us.IsPrimary && us.IsActive);

                if (userStore == null)
                    return null;

                var store = await _unitOfWork.Stores.GetByIdAsync(userStore.StoreId);
                if (store == null)
                    return null;

                return new StoreDto
                {
                    Id = store.Id,
                    Name = store.Name,
                    Code = store.Code,
                    StoreTypeId = store.StoreTypeId,
                    StoreTypeName = store.StoreType?.Name,
                    StoreTypeCode = store.StoreType?.Code,
                    Level = store.Level,
                    Location = store.Location,
                    IsActive = store.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user primary store");
                throw;
            }
        }

        public async Task BulkAssignUsersToStoreAsync(int? storeId, List<string> userIds)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                foreach (var userId in userIds)
                {
                    if (!await IsUserAssignedToStoreAsync(userId, storeId))
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            await AssignUserToStoreAsync(new UserStoreDto
                            {
                                UserId = userId,
                                StoreId = storeId,
                                UserName = user.UserName,
                                UserFullName = user.FullName,
                                IsPrimary = false
                            });
                        }
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("Bulk assigned {Count} users to store {StoreId}", userIds.Count, storeId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error bulk assigning users to store");
                throw;
            }
        }

        public async Task BulkRemoveUsersFromStoreAsync(int? storeId, List<string> userIds)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                foreach (var userId in userIds)
                {
                    await RemoveUserFromStoreAsync(userId, storeId);
                }

                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("Bulk removed {Count} users from store {StoreId}", userIds.Count, storeId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error bulk removing users from store");
                throw;
            }
        }


        public async Task<IEnumerable<UserStoreDto>> GetAssignmentHistoryAsync(string userId = null, int? storeId = null)
        {
            try
            {
                var query = await _unitOfWork.UserStores.GetAllAsync();

                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(us => us.UserId == userId);

                if (storeId.HasValue)
                    query = query.Where(us => us.StoreId == storeId.Value);

                var userStoreDtos = new List<UserStoreDto>();

                foreach (var userStore in query)
                {
                    var dto = await MapToDtoAsync(userStore);
                    userStoreDtos.Add(dto);
                }

                return userStoreDtos.OrderByDescending(us => us.AssignedDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assignment history");
                throw;
            }
        }

        private async Task ClearPrimaryStoreForUserAsync(string userId)
        {
            var primaryStores = await _unitOfWork.UserStores.FindAsync(us =>
                us.UserId == userId && us.IsPrimary && us.IsActive);

            foreach (var store in primaryStores)
            {
                store.IsPrimary = false;
                store.UpdatedAt = DateTime.UtcNow;
                store.UpdatedBy = "System";
                _unitOfWork.UserStores.Update(store);
            }
        }

        private async Task<UserStoreDto> MapToDtoAsync(UserStore userStore)
        {
            var dto = new UserStoreDto
            {
                Id = userStore.Id,
                UserId = userStore.UserId,
                StoreId = userStore.StoreId,
                IsPrimary = userStore.IsPrimary,
                AssignedDate = userStore.AssignedDate,
                UnassignedDate = userStore.UnassignedDate,
                IsActive = userStore.IsActive
            };

            // Get user details
            var user = await _userManager.FindByIdAsync(userStore.UserId);
            if (user != null)
            {
                dto.UserName = user.UserName;
                dto.UserFullName = user.FullName;
            }

            // Get store details
            var store = await _unitOfWork.Stores.GetByIdAsync(userStore.StoreId);
            if (store != null)
            {
                dto.StoreName = store.Name;
                dto.StoreCode = store.Code;

                // Get battalion and range names if available
                if (store.BattalionId.HasValue)
                {
                    var battalion = await _unitOfWork.Battalions.GetByIdAsync(store.BattalionId.Value);
                    dto.BattalionName = battalion?.Name;
                }

                if (store.RangeId.HasValue)
                {
                    var range = await _unitOfWork.Ranges.GetByIdAsync(store.RangeId.Value);
                    dto.RangeName = range?.Name;
                }
            }

            return dto;
        }










        public async Task<IEnumerable<UserStoreDto>> GetActiveAssignmentsAsync()
        {
            try
            {
                var assignments = await _unitOfWork.UserStores.FindAsync(us => us.IsActive);
                return await MapToUserStoreDtos(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active assignments");
                throw;
            }
        }


        public async Task<IEnumerable<UserStoreDto>> GetUserStoresByUserAsync(string userId)
        {
            try
            {
                var assignments = await _unitOfWork.UserStores.FindAsync(
                    us => us.UserId == userId && us.IsActive
                );
                return await MapToUserStoreDtos(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user store assignments");
                throw;
            }
        }

        public async Task<IEnumerable<UserStoreDto>> GetUserStoresByStoreAsync(int storeId)
        {
            try
            {
                var assignments = await _unitOfWork.UserStores.FindAsync(
                    us => us.StoreId == storeId && us.IsActive
                );
                return await MapToUserStoreDtos(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store user assignments");
                throw;
            }
        }

        public async Task<UserStoreDto> AssignUserToStoreAsync(string userId, int storeId, string assignedBy)
        {
            try
            {
                // Check if assignment already exists
                var existing = await _unitOfWork.UserStores.FirstOrDefaultAsync(
                    us => us.UserId == userId && us.StoreId == storeId
                );

                if (existing != null)
                {
                    if (!existing.IsActive)
                    {
                        existing.IsActive = true;
                        existing.UpdatedAt = DateTime.Now;
                        existing.UpdatedBy = assignedBy;
                        _unitOfWork.UserStores.Update(existing);
                    }
                    else
                    {
                        throw new InvalidOperationException("User is already assigned to this store");
                    }
                }
                else
                {
                    var userStore = new UserStore
                    {
                        UserId = userId,
                        StoreId = storeId,
                        AssignedAt = DateTime.Now,
                        AssignedBy = assignedBy,
                        CreatedAt = DateTime.Now,
                        CreatedBy = assignedBy,
                        IsActive = true
                    };

                    await _unitOfWork.UserStores.AddAsync(userStore);
                }

                await _unitOfWork.CompleteAsync();

                return await GetUserStoreByIdAsync(userId, storeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning user to store");
                throw;
            }
        }

        public async Task RemoveUserFromStoreAsync(string userId, int storeId)
        {
            try
            {
                var userStore = await _unitOfWork.UserStores.FirstOrDefaultAsync(
                    us => us.UserId == userId && us.StoreId == storeId && us.IsActive
                );

                if (userStore == null)
                    throw new InvalidOperationException("User assignment not found");

                userStore.IsActive = false;
                userStore.UpdatedAt = DateTime.Now;
                userStore.UpdatedBy = "System";

                _unitOfWork.UserStores.Update(userStore);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from store");
                throw;
            }
        }

        public async Task<bool> UserHasAccessToStoreAsync(string userId, int storeId)
        {
            return await _unitOfWork.UserStores.ExistsAsync(
                us => us.UserId == userId && us.StoreId == storeId && us.IsActive
            );
        }

        private async Task<UserStoreDto> GetUserStoreByIdAsync(string userId, int storeId)
        {
            var userStore = await _unitOfWork.UserStores.FirstOrDefaultAsync(
                us => us.UserId == userId && us.StoreId == storeId && us.IsActive
            );

            if (userStore == null) return null;

            var assignments = new List<UserStore> { userStore };
            var dtos = await MapToUserStoreDtos(assignments);
            return dtos.FirstOrDefault();
        }

        private async Task<IEnumerable<UserStoreDto>> MapToUserStoreDtos(IEnumerable<UserStore> userStores)
        {
            var dtos = new List<UserStoreDto>();

            foreach (var userStore in userStores)
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(userStore.StoreId);

                dtos.Add(new UserStoreDto
                {
                    UserId = userStore.UserId,
                    StoreId = userStore.StoreId,
                    StoreName = store?.Name,
                    StoreCode = store?.Code,
                    AssignedAt = userStore.AssignedAt,
                    AssignedBy = userStore.AssignedBy,
                    IsActive = userStore.IsActive
                });
            }

            return dtos;
        }
    }
}