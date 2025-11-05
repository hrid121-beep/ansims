using CsvHelper;
using DocumentFormat.OpenXml.InkML;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StoreService> _logger;
        private readonly IActivityLogService _activityLogService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;

        public StoreService(
            IUnitOfWork unitOfWork,
            ILogger<StoreService> logger,
            IActivityLogService activityLogService, 
            INotificationService notificationService,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _activityLogService = activityLogService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // Store Keeper Assignment
        public async Task<bool> AssignStoreKeeperAsync(int storeId, string userId)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                    throw new InvalidOperationException("Store not found");

                // Check if user exists
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    throw new InvalidOperationException("User not found");

                // Remove previous store keeper if exists
                if (!string.IsNullOrEmpty(store.StoreKeeperId))
                {
                    await RemoveStoreKeeperAsync(storeId);
                }

                // Assign new store keeper
                store.StoreKeeperId = userId;
                store.StoreKeeperAssignedDate = DateTime.Now;
                store.UpdatedAt = DateTime.Now;

                _unitOfWork.Stores.Update(store);
                await _unitOfWork.CompleteAsync();

                // Add user to store
                var storeUser = new UserStore
                {
                    StoreId = storeId,
                    UserId = userId,
                    AssignedDate = DateTime.Now,
                    IsActive = true,
                    Role = "StoreKeeper"
                };

                await _unitOfWork.UserStores.AddAsync(storeUser);
                await _unitOfWork.CompleteAsync();

                // Send notification
                await _notificationService.CreateNotificationAsync(new NotificationDto
                {
                    UserId = userId,
                    Title = "Store Keeper Assignment",
                    Message = $"You have been assigned as Store Keeper for {store.Name}",
                    Type = "Assignment",
                    CreatedAt = DateTime.Now
                });

                _logger.LogInformation($"Store keeper {userId} assigned to store {storeId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning store keeper");
                throw;
            }
        }
        public async Task<bool> RemoveStoreKeeperAsync(int storeId)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                    throw new InvalidOperationException("Store not found");

                var previousKeeperId = store.StoreKeeperId;
                store.StoreKeeperId = null;
                store.UpdatedAt = DateTime.Now;

                _unitOfWork.Stores.Update(store);

                // Remove from StoreUsers
                var storeUser = await _unitOfWork.UserStores
                    .FirstOrDefaultAsync(su => su.StoreId == storeId && su.UserId == previousKeeperId && su.Role == "StoreKeeper");

                if (storeUser != null)
                {
                    storeUser.IsActive = false;
                    storeUser.RemovedDate = DateTime.Now;
                    _unitOfWork.UserStores.Update(storeUser);
                }

                await _unitOfWork.CompleteAsync();

                // Send notification
                if (!string.IsNullOrEmpty(previousKeeperId))
                {
                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        UserId = previousKeeperId,
                        Title = "Store Keeper Removal",
                        Message = $"You have been removed as Store Keeper from {store.Name}",
                        Type = "Removal",
                        CreatedAt = DateTime.Now
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing store keeper");
                throw;
            }
        }
        // Set Min/Max Stock Levels for Items
        public async Task<bool> SetStockLevelsAsync(int storeId, int itemId, decimal minStock, decimal maxStock, decimal reorderLevel)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems
                    .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ItemId == itemId);

                if (storeItem == null)
                {
                    // Create new store item
                    storeItem = new StoreItem
                    {
                        StoreId = storeId,
                        ItemId = itemId,
                        MinimumStock = minStock,
                        MaximumStock = maxStock,
                        ReorderLevel = reorderLevel,
                        CurrentStock = 0,
                        LastStockUpdate = DateTime.Now,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.StoreItems.AddAsync(storeItem);
                }
                else
                {
                    // Update existing
                    storeItem.MinimumStock = minStock;
                    storeItem.MaximumStock = maxStock;
                    storeItem.ReorderLevel = reorderLevel;
                    storeItem.UpdatedAt = DateTime.Now;
                    _unitOfWork.StoreItems.Update(storeItem);
                }

                await _unitOfWork.CompleteAsync();

                // Check if current stock is below minimum
                if (storeItem.CurrentStock < minStock)
                {
                    await _notificationService.CreateLowStockAlertAsync(storeId, itemId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting stock levels");
                throw;
            }
        }
        // Get Store Keeper Details
        public async Task<UserDto> GetStoreKeeperAsync(int storeId)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);

                if (store?.StoreKeeperId == null)
                    return null;

                // Load the user separately
                var storeKeeper = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == store.StoreKeeperId);

                if (storeKeeper == null)
                    return null;

                return new UserDto
                {
                    Id = storeKeeper.Id,
                    FullName = storeKeeper.FullName,
                    Email = storeKeeper.Email,
                    PhoneNumber = storeKeeper.PhoneNumber,
                    Designation = storeKeeper.Designation,
                    Department = storeKeeper.Department
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store keeper");
                throw;
            }
        }
        // Check Low Stock Items
        public async Task<IEnumerable<StoreItemDto>> CheckLowStockAsync(int storeId)
        {
            try
            {
                var lowStockItems = await _unitOfWork.StoreItems.Query()
                    .Include(si => si.Item)
                    .Where(si => si.StoreId == storeId && si.CurrentStock <= si.MinimumStock)
                    .Select(si => new StoreItemDto
                    {
                        Id = si.Id,
                        ItemId = si.ItemId,
                        ItemName = si.Item.Name,
                        ItemCode = si.Item.ItemCode,
                        CurrentStock = si.CurrentStock,
                        MinimumStock = si.MinimumStock,
                        MaximumStock = si.MaximumStock,
                        ReorderLevel = si.ReorderLevel,
                        Unit = si.Item.Unit,
                        StockStatus = si.CurrentStock == 0 ? "Out of Stock" : "Low Stock"
                    })
                    .ToListAsync();

                // Send alerts for critical items
                foreach (var item in lowStockItems.Where(i => i.CurrentStock == 0))
                {
                    await _notificationService.CreateNotificationAsync(new NotificationDto
                    {
                        Title = "Out of Stock Alert",
                        Message = $"Item {item.ItemName} ({item.ItemCode}) is out of stock in store",
                        Type = "Critical",
                        Priority = "High",
                        CreatedAt = DateTime.Now
                    });
                }

                return lowStockItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking low stock");
                throw;
            }
        }
        // Store Setup Completion Check
        public async Task<StoreSetupStatusDto> GetStoreSetupStatusAsync(int storeId)
        {
            try
            {
                var store = await _unitOfWork.Stores.Query()
                    .Include(s => s.StoreItems)
                    .FirstOrDefaultAsync(s => s.Id == storeId);

                if (store == null)
                    throw new InvalidOperationException("Store not found");

                return new StoreSetupStatusDto
                {
                    StoreId = storeId,
                    StoreName = store.Name,
                    IsStoreCreated = true,
                    IsStoreKeeperAssigned = !string.IsNullOrEmpty(store.StoreKeeperId),
                    StoreKeeperId = store.StoreKeeperId,
                    ItemsCount = store.StoreItems?.Count ?? 0,
                    HasMinMaxLevels = store.StoreItems?.Any(si => si.MinimumStock > 0 && si.MaximumStock > 0) ?? false,
                    SetupCompletedDate = store.CreatedAt,
                    IsFullySetup = !string.IsNullOrEmpty(store.StoreKeeperId) &&
                                   store.StoreItems?.Any() == true &&
                                   store.StoreItems.All(si => si.MinimumStock > 0 && si.MaximumStock > 0)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store setup status");
                throw;
            }
        }
        public async Task<IEnumerable<StoreDto>> GetAllStoresAsync()
        {
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var storeDtos = new List<StoreDto>();

            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }

            return storeDtos.OrderBy(s => s.Name);
        }
        // Helper method
        private StoreDto MapToDto(Store store)
        {
            return new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                NameBn = store.NameBn,
                Code = store.Code,
                Location = store.Location,
                BattalionId = store.BattalionId,
                RangeId = store.RangeId,
                ZilaId = store.ZilaId,
                UpazilaId = store.UpazilaId,
                StoreKeeperId = store.StoreKeeperId,
                IsActive = store.IsActive,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt,
                StoreTypeId = store.StoreTypeId,
                StoreTypeName = store.StoreType?.Name,
                StoreTypeCode = store.StoreType?.Code,
                Level = store.Level,
                ContactNumber = store.ContactNumber,
                Email = store.Email,
                ManagerId = store.InCharge,
                ManagerName = store.InCharge,
                InCharge = store.InCharge, // For backward compatibility
                Remarks = store.Description,
                Capacity = store.TotalCapacity,
                UsedCapacity = store.UsedCapacity,
                OperatingHours = store.OperatingHours,
                CreatedBy = store.CreatedBy,
                UpdatedBy = store.UpdatedBy
            };
        }
        public async Task<IEnumerable<StoreDto>> GetActiveStoresAsync()
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.IsActive);
            var storeDtos = new List<StoreDto>();

            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }

            return storeDtos.OrderBy(s => s.Name);
        }
        public async Task<StoreDto> GetStoreByIdAsync(int? id)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(id);
                if (store == null)
                    return null;

                return await MapToDetailedDtoAsync(store);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving store with ID {Id}", id);
                throw;
            }
        }
        public async Task<StoreDto> CreateStoreAsync(StoreDto storeDto)
        {
            try
            {
                // Check if store with same code exists
                var existingStore = await _unitOfWork.Stores.FirstOrDefaultAsync(s => s.Code == storeDto.Code);
                if (existingStore != null)
                    throw new InvalidOperationException($"Store with code '{storeDto.Code}' already exists");

                var store = new Store
                {
                    Name = storeDto.Name,
                    Code = storeDto.Code ?? await GenerateStoreCodeAsync(),
                    StoreTypeId = storeDto.StoreTypeId,
                    Level = storeDto.Level,
                    Location = storeDto.Location,
                    ContactNumber = storeDto.ContactNumber,
                    Email = storeDto.Email,
                    InCharge = storeDto.ManagerId,
                    Description = storeDto.Remarks,
                    BattalionId = storeDto.BattalionId,
                    RangeId = storeDto.RangeId,
                    ZilaId = storeDto.ZilaId,
                    UpazilaId = storeDto.UpazilaId,
                    TotalCapacity = storeDto.Capacity,
                    OperatingHours = storeDto.OperatingHours,
                    IsActive = storeDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = storeDto.CreatedBy ?? "System"
                };

                await _unitOfWork.Stores.AddAsync(store);
                await _unitOfWork.CompleteAsync();

                // Assign manager to store
                if (!string.IsNullOrEmpty(storeDto.ManagerId))
                {
                    await AddUserToStoreAsync(store.Id, storeDto.ManagerId);
                }

                await _activityLogService.LogActivityAsync(
                    "Store",
                    store.Id,
                    "Create",
                    $"Created store: {store.Name} ({store.Code})",
                    store.CreatedBy
                );

                return await GetStoreByIdAsync(store.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store");
                throw;
            }
        }
        public async Task UpdateStoreAsync(StoreDto storeDto)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeDto.Id);
                if (store == null)
                    throw new InvalidOperationException($"Store with ID {storeDto.Id} not found");

                store.Name = storeDto.Name;
                store.StoreTypeId = storeDto.StoreTypeId;       
                store.Level = storeDto.Level;
                store.Location = storeDto.Location;
                store.ContactNumber = storeDto.ContactNumber;
                store.Email = storeDto.Email;
                store.InCharge = storeDto.ManagerId ?? storeDto.ManagerName;
                store.Description = storeDto.Remarks;
                store.BattalionId = storeDto.BattalionId;
                store.RangeId = storeDto.RangeId;
                store.ZilaId = storeDto.ZilaId;
                store.UpazilaId = storeDto.UpazilaId;
                store.TotalCapacity = storeDto.Capacity;
                store.OperatingHours = storeDto.OperatingHours;
                store.IsActive = storeDto.IsActive;
                store.UpdatedAt = DateTime.UtcNow;
                store.UpdatedBy = storeDto.UpdatedBy ?? "System";

                _unitOfWork.Stores.Update(store);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "Store",
                    store.Id,
                    "Update",
                    $"Updated store: {store.Name} ({store.Code})",
                    store.UpdatedBy
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store");
                throw;
            }
        }
        public async Task DeleteStoreAsync(int id)
        {
            try
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(id);
                if (store == null)
                    throw new InvalidOperationException($"Store with ID {id} not found");

                // Check if store can be deleted
                if (!await CanDeleteStoreAsync(id))
                    throw new InvalidOperationException("Cannot delete store with existing inventory or transactions");

                _unitOfWork.Stores.Remove(store);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "Store",
                    store.Id,
                    "Delete",
                    $"Deleted store: {store.Name} ({store.Code})",
                    "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting store");
                throw;
            }
        }
        // Query Methods
        public async Task<string> GenerateStoreCodeAsync()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var prefix = "ST";

                var stores = await _unitOfWork.Stores.GetAllAsync();
                var lastStore = stores
                    .Where(s => s.Code.StartsWith($"{prefix}-") && s.Code.Contains($"-{currentYear}"))
                    .OrderByDescending(s => s.Code)
                    .FirstOrDefault();

                int nextNumber = 1;
                if (lastStore != null && !string.IsNullOrEmpty(lastStore.Code))
                {
                    var parts = lastStore.Code.Split('-');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out var num))
                    {
                        nextNumber = num + 1;
                    }
                }

                return $"{prefix}-{nextNumber:D2}-{currentYear}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating store code");
                throw;
            }
        }
        public async Task<bool> StoreNameExistsAsync(string name, int? excludeId = null)
        {
            return await _unitOfWork.Stores.ExistsAsync(s =>
                s.Name.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || s.Id != excludeId.Value));
        }
        public async Task<bool> StoreExistsAsync(string name, int? excludeId = null)
        {
            return await StoreNameExistsAsync(name, excludeId);
        }
        public async Task<int> GetStoreCountAsync()
        {
            var stores = await _unitOfWork.Stores.GetAllAsync();
            return stores.Count();
        }
        public async Task<IEnumerable<StoreDto>> GetPagedStoresAsync(int page, int pageSize)
        {
            var stores = await _unitOfWork.Stores.GetAllAsync();
            var pagedStores = stores
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var storeDtos = new List<StoreDto>();
            foreach (var store in pagedStores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }

            return storeDtos;
        }
        public async Task<IEnumerable<StoreDto>> SearchStoresAsync(string searchTerm)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s =>
                s.Name.Contains(searchTerm) ||
                s.Code.Contains(searchTerm) ||
                s.Location.Contains(searchTerm));

            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }

            return storeDtos;
        }
        public async Task<IEnumerable<StoreDto>> GetStoresByTypeAsync(string type)
        {
            // Look up StoreType by code or name
            var storeType = await _unitOfWork.StoreTypes
                .FirstOrDefaultAsync(st => st.Code == type || st.Name == type);

            if (storeType == null)
            {
                return new List<StoreDto>();
            }

            // Use FindAsync instead of Include
            var stores = await _unitOfWork.Stores
                .FindAsync(s => s.StoreTypeId == storeType.Id && s.IsActive);

            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                // Load StoreType if needed
                if (store.StoreType == null && store.StoreTypeId.HasValue)
                {
                    store.StoreType = await _unitOfWork.StoreTypes.GetByIdAsync(store.StoreTypeId.Value);
                }

                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }
            return storeDtos;
        }
        public async Task<IEnumerable<StoreDto>> GetStoresByLocationAsync(string location)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.Location.Contains(location));
            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }
            return storeDtos;
        }
        public async Task<IEnumerable<StoreDto>> GetStoresByLevelAsync(StoreLevel level)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.Level == level && s.IsActive);
            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }
            return storeDtos.OrderBy(s => s.Name);
        }
        public async Task<IEnumerable<StoreDto>> GetStoresByBattalionAsync(int battalionId)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.BattalionId == battalionId);
            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }
            return storeDtos.OrderBy(s => s.Name);
        }
        public async Task<IEnumerable<StoreDto>> GetStoresByRangeAsync(int rangeId)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.RangeId == rangeId);
            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }
            return storeDtos.OrderBy(s => s.Name);
        }
        public async Task<IEnumerable<StoreDto>> GetStoresByZilaAsync(int zilaId)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.ZilaId == zilaId);
            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }
            return storeDtos.OrderBy(s => s.Name);
        }
        public async Task<IEnumerable<StoreDto>> GetStoresByUpazilaAsync(int upazilaId)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.UpazilaId == upazilaId);
            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }
            return storeDtos.OrderBy(s => s.Name);
        }
        // User Management
        public async Task<IEnumerable<StoreDto>> GetStoresByUserAsync(string userId)
        {
            var userStores = await _unitOfWork.UserStores.FindAsync(us => us.UserId == userId);
            var stores = new List<StoreDto>();

            foreach (var userStore in userStores)
            {
                var store = await GetStoreByIdAsync(userStore.StoreId);
                if (store != null)
                    stores.Add(store);
            }

            return stores;
        }
        public async Task<bool> AssignUserToStoreAsync(string userId, int? storeId)
        {
            try
            {
                var existing = await _unitOfWork.UserStores
                    .FirstOrDefaultAsync(us => us.UserId == userId && us.StoreId == storeId);

                if (existing != null)
                    return false;

                await AddUserToStoreAsync(storeId, userId);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task AddUserToStoreAsync(int? storeId, string userId)
        {
            try
            {
                var existing = await _unitOfWork.UserStores
                    .FirstOrDefaultAsync(us => us.StoreId == storeId && us.UserId == userId);

                if (existing != null)
                    throw new InvalidOperationException("User is already assigned to this store");

                var userStore = new UserStore
                {
                    UserId = userId,
                    StoreId = storeId,
                    AssignedDate = DateTime.UtcNow,
                    AssignedBy = "System",
                    IsActive = true
                };

                await _unitOfWork.UserStores.AddAsync(userStore);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogStorUsereActivityAsync(
                    "Store",
                    storeId,
                    "AddUser",
                    $"Added user {userId} to store",
                    "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user to store");
                throw;
            }
        }
        public async Task RemoveUserFromStoreAsync(string userId, int? storeId)
        {
            await RemoveUserFromStoreAsync(storeId, userId);
        }
        public async Task RemoveUserFromStoreAsync(int? storeId, string userId)
        {
            try
            {
                var userStore = await _unitOfWork.UserStores
                    .FirstOrDefaultAsync(us => us.StoreId == storeId && us.UserId == userId);

                if (userStore == null)
                    throw new InvalidOperationException("User is not assigned to this store");

                _unitOfWork.UserStores.Remove(userStore);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogRemoveUserActivityAsync(
                    "Store",
                    storeId,
                    "RemoveUser",
                    $"Removed user {userId} from store",
                    "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from store");
                throw;
            }
        }
        public async Task<bool> UserHasAccessToStoreAsync(string userId, int? storeId)
        {
            return await _unitOfWork.UserStores
                .ExistsAsync(us => us.UserId == userId && us.StoreId == storeId);
        }
        public async Task<IEnumerable<string>> GetStoreUsersAsync(int? storeId)
        {
            var userStores = await _unitOfWork.UserStores.FindAsync(us => us.StoreId == storeId);
            return userStores.Select(us => us.UserId);
        }
        public async Task<IEnumerable<UserDto>> GetStoreAssignedUsersAsync(int? storeId)
        {
            var userStores = await _unitOfWork.UserStores.FindAsync(us => us.StoreId == storeId);
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
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        IsActive = user.IsActive,
                        Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                    });
                }
            }

            return users;
        }

        // Stock Management
        public async Task<decimal> GetStoreTotalValueAsync(int? storeId)
        {
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId);
            decimal totalValue = 0;

            foreach (var storeItem in storeItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
                if (item != null)
                    totalValue += (decimal)(storeItem.Quantity * item.UnitPrice);
            }

            return totalValue;
        }
        public async Task<int> GetStoreItemCountAsync(int? storeId)
        {
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId);
            return storeItems.Count();
        }
        public async Task<IEnumerable<StoreStockDto>> GetStoreStockAsync(int? storeId)
        {
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId);
            var stockDtos = new List<StoreStockDto>();

            foreach (var storeItem in storeItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
                if (item != null)
                {
                    var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                    var category = subCategory != null ?
                        await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;

                    stockDtos.Add(new StoreStockDto
                    {
                        StoreId = storeItem.StoreId,
                        ItemId = storeItem.ItemId,
                        ItemName = item.Name,
                        ItemCode = item.ItemCode,
                        Quantity = storeItem.Quantity,
                        ReservedQuantity = storeItem.ReservedQuantity,
                        AvailableQuantity = storeItem.Quantity - (storeItem.ReservedQuantity),
                        MinStockLevel = item.MinimumStock,
                        MinimumStock = item.MinimumStock,
                        ReorderLevel = item.ReorderLevel,
                        CategoryName = category?.Name ?? "Uncategorized",
                        UnitPrice = item.UnitPrice,
                        TotalValue = storeItem.Quantity * item.UnitPrice,
                        Unit = item.Unit ?? "PCS"
                    });
                }
            }

            return stockDtos;
        }
        public async Task<decimal> GetStoreItemQuantityAsync(int? storeId, int itemId)
        {
            var storeItem = await _unitOfWork.StoreItems
                .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ItemId == itemId);
            return storeItem?.Quantity ?? 0;
        }
        public async Task<IEnumerable<StoreStockDto>> GetLowStockItemsByStoreAsync(int? storeId)
        {
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId);
            var lowStockItems = new List<StoreStockDto>();

            foreach (var storeItem in storeItems)
            {
                var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
                if (item != null && storeItem.Quantity <= item.MinimumStock)
                {
                    var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                    var category = subCategory != null ?
                        await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;

                    lowStockItems.Add(new StoreStockDto
                    {
                        StoreId = storeItem.StoreId,
                        ItemId = storeItem.ItemId,
                        ItemName = item.Name,
                        ItemCode = item.ItemCode,
                        Quantity = storeItem.Quantity,
                        MinStockLevel = item.MinimumStock,
                        MinimumStock = item.MinimumStock,
                        ReorderLevel = item.ReorderLevel,
                        CategoryName = category?.Name ?? "Uncategorized",
                        UnitPrice = item.UnitPrice,
                        TotalValue = storeItem.Quantity * item.UnitPrice,
                        Unit = item.Unit ?? "PCS"
                    });
                }
            }

            return lowStockItems;
        }

        // Statistics and Validation
        public async Task<Dictionary<string, int>> GetStoreStatisticsAsync(int? storeId)
        {
            var stats = new Dictionary<string, int>();

            stats["TotalItems"] = (await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId)).Count();
            stats["AssignedUsers"] = (await _unitOfWork.UserStores.FindAsync(us => us.StoreId == storeId)).Count();
            stats["ActiveIssues"] = (await _unitOfWork.Issues.FindAsync(i => i.FromStoreId == storeId && i.Status == "Active")).Count();
            stats["PendingReceives"] = (await _unitOfWork.Receives.FindAsync(r => r.StoreId == storeId && r.Status == "Pending")).Count();

            return stats;
        }
        public async Task<bool> CanDeleteStoreAsync(int? storeId)
        {
            var hasInventory = await _unitOfWork.StoreItems.ExistsAsync(si => si.StoreId == storeId && si.Quantity > 0);
            if (hasInventory) return false;

            var hasPendingIssues = await _unitOfWork.Issues.ExistsAsync(i => i.FromStoreId == storeId && i.Status != "Completed");
            if (hasPendingIssues) return false;

            var hasPendingReceives = await _unitOfWork.Receives.ExistsAsync(r => r.StoreId == storeId && r.Status != "Completed");
            if (hasPendingReceives) return false;

            return true;
        }
        public async Task UpdateStoreCapacityAsync(int? storeId, decimal usedCapacity)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            if (store != null)
            {
                store.UsedCapacity = usedCapacity;
                store.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Stores.Update(store);
                await _unitOfWork.CompleteAsync();
            }
        }
        public async Task<IEnumerable<StoreDto>> GetLowCapacityStoresAsync(decimal thresholdPercentage = 80)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s =>
                s.TotalCapacity.HasValue &&
                s.UsedCapacity.HasValue &&
                (s.UsedCapacity.Value / s.TotalCapacity.Value) * 100 >= thresholdPercentage);

            var storeDtos = new List<StoreDto>();
            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }

            return storeDtos.OrderByDescending(s => s.UsedCapacityPercentage);
        }
        public async Task<ImportResultDto> ImportStoresFromCsvAsync(Stream fileStream, bool updateExisting, string importedBy)
        {
            var result = new ImportResultDto { TotalRows = 0, Errors = new List<ImportErrorDto>() };

            try
            {
                using var reader = new StreamReader(fileStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<StoreCsvRecord>().ToList();
                result.TotalRows = records.Count;

                int rowNumber = 2;
                foreach (var record in records)
                {
                    try
                    {
                        await ProcessStoreImportRecord(record, updateExisting, importedBy, result, rowNumber);
                    }
                    catch (Exception ex)
                    {
                        result.FailedCount++;
                        result.Errors.Add(new ImportErrorDto
                        {
                            Row = rowNumber,
                            Message = ex.Message
                        });
                    }
                    rowNumber++;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing stores from CSV");
                throw;
            }
        }
        public async Task<byte[]> ExportStoresToCsvAsync()
        {
            var stores = await GetAllStoresAsync();
            return await ExportStoresToCsvAsync(stores);
        }
        public async Task<byte[]> ExportStoresToCsvAsync(IEnumerable<StoreDto> stores)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteRecords(stores.Select(s => new StoreCsvRecord
                {
                    Name = s.Name,
                    Type = s.Type,
                    Level = s.Level.ToString(),
                    ManagerUsername = s.ManagerName,
                    RangeCode = s.RangeCode,
                    BattalionCode = s.BattalionCode,
                    ZilaCode = s.ZilaCode,
                    UpazilaCode = s.UpazilaCode,
                    Location = s.Location,
                    ContactNumber = s.ContactNumber,
                    Email = s.Email,
                    Capacity = s.Capacity?.ToString(),
                    OperatingHours = s.OperatingHours,
                    IsActive = s.IsActive.ToString()
                }));

                writer.Flush();
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stores to CSV");
                throw;
            }
        }
        private async Task<StoreDto> MapToDetailedDtoAsync(Store store)
        {
            var dto = MapToDto(store);

            // Get counts
            dto.AssignedUserCount = (await _unitOfWork.UserStores.FindAsync(us => us.StoreId == store.Id)).Count();
            dto.ItemCount = (await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == store.Id)).Count();

            // Get names
            if (store.BattalionId.HasValue)
            {
                var battalion = await _unitOfWork.Battalions.GetByIdAsync(store.BattalionId.Value);
                dto.BattalionName = battalion?.Name;
                dto.BattalionCode = battalion?.Code;
            }

            if (store.RangeId.HasValue)
            {
                var range = await _unitOfWork.Ranges.GetByIdAsync(store.RangeId.Value);
                dto.RangeName = range?.Name;
                dto.RangeCode = range?.Code;
            }

            if (store.ZilaId.HasValue)
            {
                var zila = await _unitOfWork.Zilas.GetByIdAsync(store.ZilaId.Value);
                dto.ZilaName = zila?.Name;
                dto.ZilaCode = zila?.Code;
            }

            if (store.UpazilaId.HasValue)
            {
                var upazila = await _unitOfWork.Upazilas.GetByIdAsync(store.UpazilaId.Value);
                dto.UpazilaName = upazila?.Name;
                dto.UpazilaCode = upazila?.Code;
            }

            // Calculate capacity percentage
            if (store.TotalCapacity.HasValue && store.UsedCapacity.HasValue && store.TotalCapacity.Value > 0)
            {
                dto.UsedCapacityPercentage = (store.UsedCapacity.Value / store.TotalCapacity.Value) * 100;
            }

            // Get manager info
            if (!string.IsNullOrEmpty(store.InCharge))
            {
                var manager = await _userManager.FindByIdAsync(store.InCharge);
                if (manager != null)
                {
                    dto.ManagerId = manager.Id;
                    dto.ManagerName = manager.FullName;
                    dto.InCharge = manager.FullName; // For backward compatibility
                }
            }

            return dto;
        }
        private async Task ProcessStoreImportRecord(StoreCsvRecord record, bool updateExisting, string importedBy, ImportResultDto result, int rowNumber)
{
    // Validate required fields
    if (string.IsNullOrWhiteSpace(record.Name))
        throw new InvalidOperationException("Store name is required");

    if (string.IsNullOrWhiteSpace(record.Type))
        throw new InvalidOperationException("Store type is required");

    if (string.IsNullOrWhiteSpace(record.Level))
        throw new InvalidOperationException("Store level is required");

    // Check if store exists
    var existingStore = await _unitOfWork.Stores.FirstOrDefaultAsync(s => s.Name == record.Name);

    if (existingStore != null && !updateExisting)
    {
        result.SkippedCount++;
        return;
    }

    // Look up StoreType by code or name
    var storeType = await _unitOfWork.StoreTypes.FirstOrDefaultAsync(
        st => st.Code == record.Type || st.Name == record.Type
    );
    
    if (storeType == null)
        throw new InvalidOperationException($"Store type '{record.Type}' not found. Please use a valid store type code or name.");

    // Validate store level
    if (!Enum.TryParse<StoreLevel>(record.Level, out var storeLevel))
        throw new InvalidOperationException($"Invalid store level '{record.Level}'. Valid values are: {string.Join(", ", Enum.GetNames(typeof(StoreLevel)))}");

    // Validate and get related entities
    int? rangeId = null, battalionId = null, zilaId = null, upazilaId = null;

    if (!string.IsNullOrEmpty(record.RangeCode))
    {
        var range = await _unitOfWork.Ranges.FirstOrDefaultAsync(r => r.Code == record.RangeCode);
        if (range == null)
            throw new InvalidOperationException($"Range with code '{record.RangeCode}' not found");
        rangeId = range.Id;
    }

    if (!string.IsNullOrEmpty(record.BattalionCode))
    {
        var battalion = await _unitOfWork.Battalions.FirstOrDefaultAsync(b => b.Code == record.BattalionCode);
        if (battalion == null)
            throw new InvalidOperationException($"Battalion with code '{record.BattalionCode}' not found");
        battalionId = battalion.Id;
    }

    if (!string.IsNullOrEmpty(record.ZilaCode))
    {
        var zila = await _unitOfWork.Zilas.FirstOrDefaultAsync(z => z.Code == record.ZilaCode);
        if (zila == null)
            throw new InvalidOperationException($"Zila with code '{record.ZilaCode}' not found");
        zilaId = zila.Id;
    }

    if (!string.IsNullOrEmpty(record.UpazilaCode))
    {
        var upazila = await _unitOfWork.Upazilas.FirstOrDefaultAsync(u => u.Code == record.UpazilaCode);
        if (upazila == null)
            throw new InvalidOperationException($"Upazila with code '{record.UpazilaCode}' not found");
        upazilaId = upazila.Id;
    }

    // Get manager
    string managerId = null;
    if (!string.IsNullOrEmpty(record.ManagerUsername))
    {
        var manager = await _userManager.FindByNameAsync(record.ManagerUsername);
        if (manager == null)
            throw new InvalidOperationException($"User with username '{record.ManagerUsername}' not found");
        managerId = manager.Id;
    }

    if (existingStore != null)
    {
        // Update existing
        existingStore.StoreTypeId = storeType.Id;  // Use the looked-up storeType
        existingStore.Level = storeLevel;  // Use the parsed enum value
        existingStore.RangeId = rangeId;
        existingStore.BattalionId = battalionId;
        existingStore.ZilaId = zilaId;
        existingStore.UpazilaId = upazilaId;
        existingStore.ManagerId = managerId;  // Make sure this property name matches your entity
        existingStore.Location = record.Location;
        existingStore.ContactNumber = record.ContactNumber;
        existingStore.Email = record.Email;
        existingStore.Capacity = string.IsNullOrEmpty(record.Capacity) ? null : decimal.Parse(record.Capacity);
        existingStore.OperatingHours = record.OperatingHours;
        existingStore.IsActive = string.IsNullOrEmpty(record.IsActive) || bool.Parse(record.IsActive);
        existingStore.UpdatedAt = DateTime.UtcNow;
        existingStore.UpdatedBy = importedBy;

        _unitOfWork.Stores.Update(existingStore);
        result.UpdatedCount++;
    }
    else
    {
        // Create new
        var newStore = new Store
        {
            Name = record.Name,
            Code = await GenerateStoreCodeAsync(),
            StoreTypeId = storeType.Id,  // Set the foreign key
            Level = storeLevel,  // Use the parsed enum value
            RangeId = rangeId,
            BattalionId = battalionId,
            ZilaId = zilaId,
            UpazilaId = upazilaId,
            ManagerId = managerId,  // Make sure this property name matches your entity
            Location = record.Location,
            ContactNumber = record.ContactNumber,
            Email = record.Email,
            Capacity = string.IsNullOrEmpty(record.Capacity) ? null : decimal.Parse(record.Capacity),
            OperatingHours = record.OperatingHours,
            IsActive = string.IsNullOrEmpty(record.IsActive) || bool.Parse(record.IsActive),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = importedBy
        };

        await _unitOfWork.Stores.AddAsync(newStore);
        result.ImportedCount++;
    }

    await _unitOfWork.CompleteAsync();
}
        public async Task<IEnumerable<StoreDto>> GetStoresByUnionAsync(int unionId)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.UnionId == unionId);
            return stores.Select(MapToDto).OrderBy(s => s.Name);
        }
        public async Task<IEnumerable<StoreDto>> GetUserStoresAsync(string userId)
        {
            var userStores = await _unitOfWork.UserStores.GetAllAsync(
                us => us.UserId == userId && us.IsActive,
                includes: new[] { "Store" }
            );

            var stores = userStores.Select(us => us.Store).Distinct();
            var storeDtos = new List<StoreDto>();

            foreach (var store in stores)
            {
                // Load the StoreType if needed
                if (store.StoreType == null && store.StoreTypeId.HasValue)
                {
                    store.StoreType = await _unitOfWork.StoreTypes.GetByIdAsync(store.StoreTypeId.Value);
                }

                storeDtos.Add(new StoreDto
                {
                    Id = store.Id,
                    Name = store.Name,
                    Code = store.Code,
                    Location = store.Location,
                    Type = store.StoreType?.Name,  // Use StoreType.Name instead of store.Type
                    StoreTypeName = store.StoreType?.Name,  // Also set StoreTypeName
                    StoreTypeCode = store.StoreType?.Code,  // And StoreTypeCode
                    IsActive = store.IsActive
                });
            }

            return storeDtos;
        }
        public async Task<StockLevelDto> GetStockLevelAsync(int itemId, int? storeId)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems.SingleOrDefaultAsync(
                    si => si.ItemId == itemId && si.StoreId == storeId && si.IsActive
                );

                if (storeItem == null)
                {
                    return new StockLevelDto
                    {
                        ItemId = itemId,
                        StoreId = storeId,
                        CurrentStock = 0,
                        MinimumStock = 0,
                        IsOutOfStock = true,
                        StockStatus = "Out of Stock"
                    };
                }

                var item = await _unitOfWork.Items.GetByIdAsync(itemId);
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);

                var stockLevel = new StockLevelDto
                {
                    ItemId = itemId,
                    StoreId = storeId,
                    ItemCode = item?.Code,
                    ItemName = item?.Name ?? "Unknown Item",
                    StoreName = store?.Name ?? "Unknown Store",
                    CurrentStock = storeItem.Quantity,
                    MinimumStock = item?.MinimumStock ?? 0,
                    MaximumStock = item?.MaximumStock ?? 0,
                    Unit = item?.Unit,
                    IsLowStock = storeItem.Quantity <= (item?.MinimumStock ?? 0) && storeItem.Quantity > 0,
                    IsOutOfStock = storeItem.Quantity <= 0
                };

                // Set stock status
                if (stockLevel.IsOutOfStock)
                    stockLevel.StockStatus = "Out of Stock";
                else if (stockLevel.IsLowStock)
                    stockLevel.StockStatus = "Low Stock";
                else
                    stockLevel.StockStatus = "In Stock";

                return stockLevel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock level for item {ItemId} in store {StoreId}", itemId, storeId);
                throw;
            }
        }
        public async Task<ItemDto> GetItemDetailsAsync(int id)
        {
            return await GetItemByIdAsync(id);
        }
        public async Task<ItemDto> GetItemByIdAsync(int id)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(id);
            if (item == null) return null;

            return new ItemDto
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                Description = item.Description,
                Unit = item.Unit,
                UnitPrice = item.UnitPrice,
                MinimumStock = item.MinimumStock,
                MaximumStock = item.MaximumStock,
                ReorderLevel = item.ReorderLevel,
                Status = item.Status,
                CategoryId = item.CategoryId,
                BrandId = item.BrandId,
                IsActive = item.IsActive
            };
        }
        public async Task<IEnumerable<ItemDto>> GetAllItemsAsync()
        {
            var items = await _unitOfWork.Items.GetAllAsync();
            return items.Where(i => i.IsActive).Select(item => new ItemDto
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                Description = item.Description,
                Unit = item.Unit,
                UnitPrice = item.UnitPrice,
                MinimumStock = item.MinimumStock,
                MaximumStock = item.MaximumStock,
                ReorderLevel = item.ReorderLevel,
                Status = item.Status,
                CategoryId = item.CategoryId,
                BrandId = item.BrandId,
                IsActive = item.IsActive
            });
        }
        public async Task<IEnumerable<StockLevelDto>> GetStoreStockLevelsAsync(int? storeId)
        {
            try
            {
                var stockLevels = new List<StockLevelDto>();

                if (storeId.HasValue)
                {
                    // Get stock levels for a specific store
                    var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId && si.IsActive);

                    foreach (var storeItem in storeItems)
                    {
                        var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
                        if (item == null) continue;

                        var category = await _unitOfWork.Categories.GetByIdAsync(item.CategoryId);
                        var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);

                        var status = GetStockStatus(storeItem.Quantity, item.MinimumStock);

                        stockLevels.Add(new StockLevelDto
                        {
                            ItemId = item.Id,
                            ItemCode = item.ItemCode,
                            ItemName = item.Name,
                            CategoryName = category?.Name ?? "Uncategorized",
                            SubCategoryName = subCategory?.Name,
                            CurrentStock = storeItem.Quantity,
                            MinimumStock = item.MinimumStock ?? 0m,
                            ReorderLevel = item.ReorderLevel,
                            StockStatus = status,
                            Unit = item.Unit ?? "Piece",
                            Location = storeItem.Location,
                            LastUpdated = storeItem.UpdatedAt ?? storeItem.CreatedAt
                        });
                    }
                }
                else
                {
                    // Get stock levels for all stores - INCLUDES store distribution
                    var items = await _unitOfWork.Items.GetAllAsync();

                    foreach (var item in items.Where(i => i.IsActive))
                    {
                        var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);
                        var totalStock = storeItems.Sum(si => si.Quantity);

                        var category = await _unitOfWork.Categories.GetByIdAsync(item.CategoryId);
                        var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);

                        var status = GetStockStatus(totalStock, item.MinimumStock);

                        // ✅ Collect store distribution data
                        var storeStocks = new List<StoreStockInfo>();
                        foreach (var storeItem in storeItems)
                        {
                            var store = await _unitOfWork.Stores.GetByIdAsync(storeItem.StoreId);
                            storeStocks.Add(new StoreStockInfo
                            {
                                StoreId = storeItem.StoreId,
                                StoreName = store?.Name ?? "Unknown Store",
                                Quantity = storeItem.Quantity,
                                Location = storeItem.Location,
                                LastUpdated = storeItem.UpdatedAt ?? storeItem.CreatedAt,
                                ItemId = item.Id,
                                ItemName = item.Name,
                                Value = (decimal)(storeItem.Quantity * (item.UnitPrice ?? 0m))
                            });
                        }

                        stockLevels.Add(new StockLevelDto
                        {
                            ItemId = item.Id,
                            ItemCode = item.ItemCode,
                            ItemName = item.Name,
                            CategoryName = category?.Name ?? "Uncategorized",
                            SubCategoryName = subCategory?.Name,
                            CurrentStock = totalStock,
                            MinimumStock = item.MinimumStock ?? 0m,
                            ReorderLevel = item.ReorderLevel,
                            StockStatus = status,
                            Unit = item.Unit ?? "Piece",
                            StoreStocks = storeStocks, // ✅ Use collected data
                            LastUpdated = DateTime.Now
                        });
                    }
                }

                return stockLevels.OrderBy(s => s.StockStatus == "Out of Stock" ? 0 :
                                           s.StockStatus == "Critical" ? 1 :
                                           s.StockStatus == "Low" ? 2 : 3)
                                .ThenBy(s => s.ItemName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store stock levels");
                throw;
            }
        }
        private string GetStockStatus(decimal? currentStock, decimal? minimumStock)
        {
            if (currentStock == 0)
                return "Out of Stock";
            else if (currentStock < minimumStock * 0.5m)
                return "Critical";
            else if (currentStock < minimumStock)
                return "Low";
            else
                return "Good";
        }
        public async Task<object> GetStoreInventorySummaryAsync(int? storeId)
        {
            try
            {
                var summary = new StoreInventorySummary
                {
                    TotalItems = 0,
                    TotalValue = 0m,
                    OutOfStockItems = 0,
                    LowStockItems = 0,
                    Categories = new List<CategorySummary>()
                };

                if (!storeId.HasValue)
                    return summary;

                var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId && si.IsActive);
                var categorySummary = new Dictionary<int, CategorySummary>();

                foreach (var storeItem in storeItems)
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(storeItem.ItemId);
                    if (item == null) continue;

                    summary.TotalItems++;
                    summary.TotalValue += storeItem.Quantity * item.UnitPrice;

                    if (storeItem.Quantity == 0)
                        summary.OutOfStockItems++;
                    else if (storeItem.Quantity < item.MinimumStock)
                        summary.LowStockItems++;

                    // Category summary
                    if (!categorySummary.ContainsKey(item.CategoryId))
                    {
                        var category = await _unitOfWork.Categories.GetByIdAsync(item.CategoryId);
                        categorySummary[item.CategoryId] = new CategorySummary
                        {
                            CategoryName = category?.Name ?? "Unknown",
                            ItemCount = 0,
                            TotalValue = 0m
                        };
                    }

                    categorySummary[item.CategoryId].ItemCount++;
                    categorySummary[item.CategoryId].TotalValue += storeItem.Quantity * item.UnitPrice;
                }

                summary.Categories = categorySummary.Values.ToList();
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store inventory summary");
                throw;
            }
        }
        public async Task<IEnumerable<StoreRecentTransactionDto>> GetStoreRecentTransactionsAsync(int? storeId, int count = 20)
        {
            try
            {
                var transactions = new List<StoreRecentTransactionDto>();

                if (!storeId.HasValue)
                    return transactions;

                // Get recent stock movements
                var movements = await _unitOfWork.StockMovements.FindAsync(
                    m => (m.SourceStoreId == storeId || m.DestinationStoreId == storeId) && m.IsActive
                );

                foreach (var movement in movements.OrderByDescending(m => m.CreatedAt).Take(count))
                {
                    var item = await _unitOfWork.Items.GetByIdAsync(movement.ItemId);

                    transactions.Add(new StoreRecentTransactionDto
                    {
                        Id = movement.Id,
                        Date = movement.CreatedAt,
                        Type = movement.MovementType,
                        ItemCode = item?.ItemCode,
                        ItemName = item?.Name,
                        Quantity = movement.Quantity ?? 0,
                        Direction = movement.SourceStoreId == storeId ? "Out" : "In",
                        ReferenceNumber = movement.ReferenceNo,
                        User = movement.CreatedBy
                    });
                }

                return transactions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store recent transactions");
                throw;
            }
        }
        public Task<ImportResultDto> ImportStoresFromCsvAsync(Stream csvStream, string userId)
        {
            // Simply redirect to the fully implemented overload
            // Default updateExisting to true for backward compatibility
            return ImportStoresFromCsvAsync(csvStream, true, userId);
        }
        public async Task AddItemToStoreAsync(int storeId, int itemId,
            decimal minStock, decimal maxStock, decimal reorderLevel)
        {
            try
            {
                _logger.LogInformation(
                    "=== START AddItemToStoreAsync === StoreId={StoreId}, ItemId={ItemId}, Min={MinStock}, Max={MaxStock}, Reorder={ReorderLevel}",
                    storeId, itemId, minStock, maxStock, reorderLevel);

                // Validate input
                if (minStock < 0 || maxStock < 0 || reorderLevel < 0)
                {
                    var error = "Stock levels cannot be negative";
                    _logger.LogError(error);
                    throw new ArgumentException(error);
                }

                if (minStock > maxStock)
                {
                    var error = "Minimum stock cannot be greater than maximum stock";
                    _logger.LogError(error);
                    throw new ArgumentException(error);
                }

                if (reorderLevel < minStock || reorderLevel > maxStock)
                {
                    var error = "Reorder level must be between minimum and maximum stock";
                    _logger.LogError(error);
                    throw new ArgumentException(error);
                }

                // Verify store exists
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                {
                    var error = $"Store with ID {storeId} not found";
                    _logger.LogError(error);
                    throw new InvalidOperationException(error);
                }
                _logger.LogInformation("Store verified: {StoreName} (ID: {StoreId})", store.Name, storeId);

                // Verify item exists
                var item = await _unitOfWork.Items.GetByIdAsync(itemId);
                if (item == null)
                {
                    var error = $"Item with ID {itemId} not found";
                    _logger.LogError(error);
                    throw new InvalidOperationException(error);
                }
                _logger.LogInformation("Item verified: {ItemName} (ID: {ItemId})", item.Name, itemId);

                // Get ALL store items for this combination to check if exists
                var allStoreItems = await _unitOfWork.StoreItems.GetAllAsync();
                var existing = allStoreItems.FirstOrDefault(si =>
                    si.StoreId == storeId &&
                    si.ItemId == itemId);

                _logger.LogInformation("Existing StoreItem check: {Exists}", existing != null ? $"Found ID {existing.Id}" : "Not found");

                if (existing != null)
                {
                    // UPDATE existing
                    _logger.LogInformation(
                        "UPDATING existing StoreItem ID={StoreItemId}: OldMin={OldMin}, OldMax={OldMax}, OldReorder={OldReorder}",
                        existing.Id, existing.MinimumStock, existing.MaximumStock, existing.ReorderLevel);

                    existing.MinimumStock = minStock;
                    existing.MaximumStock = maxStock;
                    existing.ReorderLevel = reorderLevel;
                    existing.UpdatedAt = DateTime.Now;
                    existing.UpdatedBy = "System";
                    existing.IsActive = true;

                    _unitOfWork.StoreItems.Update(existing);

                    _logger.LogInformation(
                        "StoreItem updated in memory: ID={StoreItemId}, NewMin={MinStock}, NewMax={MaxStock}, NewReorder={ReorderLevel}",
                        existing.Id, minStock, maxStock, reorderLevel);
                }
                else
                {
                    // CREATE new
                    _logger.LogInformation("CREATING new StoreItem for StoreId={StoreId}, ItemId={ItemId}", storeId, itemId);

                    var newStoreItem = new StoreItem
                    {
                        StoreId = storeId,
                        ItemId = itemId,
                        Quantity = 0,
                        MinimumStock = minStock,
                        MaximumStock = maxStock,
                        ReorderLevel = reorderLevel,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System"
                    };

                    await _unitOfWork.StoreItems.AddAsync(newStoreItem);

                    _logger.LogInformation(
                        "New StoreItem added to context: StoreId={StoreId}, ItemId={ItemId}, Min={MinStock}, Max={MaxStock}, Reorder={ReorderLevel}",
                        storeId, itemId, minStock, maxStock, reorderLevel);
                }

                // CRITICAL: Save to database
                _logger.LogInformation("Calling CompleteAsync to save changes...");

                var saveResult = await _unitOfWork.CompleteAsync();

                _logger.LogInformation("CompleteAsync result: {SaveResult} row(s) affected", saveResult);

                if (saveResult <= 0)
                {
                    var error = "CompleteAsync returned 0 - No rows were saved to database!";
                    _logger.LogError(error);
                    throw new InvalidOperationException(error);
                }

                // Verify the save by querying again
                var verifyAll = await _unitOfWork.StoreItems.GetAllAsync();
                var verification = verifyAll.FirstOrDefault(si => si.StoreId == storeId && si.ItemId == itemId);

                if (verification != null)
                {
                    _logger.LogInformation(
                        "✓ VERIFICATION SUCCESSFUL - StoreItem exists in database: ID={StoreItemId}, StoreId={StoreId}, ItemId={ItemId}, Min={Min}, Max={Max}, Reorder={Reorder}",
                        verification.Id, verification.StoreId, verification.ItemId,
                        verification.MinimumStock, verification.MaximumStock, verification.ReorderLevel);
                }
                else
                {
                    _logger.LogError(
                        "✗ VERIFICATION FAILED - StoreItem NOT found in database after save! StoreId={StoreId}, ItemId={ItemId}",
                        storeId, itemId);
                    throw new InvalidOperationException("Item was not saved to database - verification failed");
                }

                _logger.LogInformation("=== END AddItemToStoreAsync SUCCESS ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "=== EXCEPTION in AddItemToStoreAsync === StoreId={StoreId}, ItemId={ItemId}, Message={Message}",
                    storeId, itemId, ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<StoreDto>> GetUserAccessibleStoresAsync(string userId)
        {
            // This method gets all stores that a user has access to
            var userStores = await _unitOfWork.UserStores
                .Query()
                .Include(us => us.Store)
                .Where(us => us.UserId == userId && us.IsActive)
                .ToListAsync();

            var stores = userStores.Select(us => us.Store).Where(s => s != null && s.IsActive);
            var storeDtos = new List<StoreDto>();

            foreach (var store in stores)
            {
                storeDtos.Add(await MapToDetailedDtoAsync(store));
            }

            return storeDtos.OrderBy(s => s.Name);
        }
        public async Task<StoreDto> GetStoreByIdAsync(int id)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(id);
            if (store == null) return null;

            // Fetch related entities to get names
            var storeType = store.StoreTypeId.HasValue
                ? await _unitOfWork.StoreTypes.GetByIdAsync(store.StoreTypeId.Value)
                : null;

            var range = store.RangeId.HasValue
                ? await _unitOfWork.Ranges.GetByIdAsync(store.RangeId.Value)
                : null;

            var battalion = store.BattalionId.HasValue
                ? await _unitOfWork.Battalions.GetByIdAsync(store.BattalionId.Value)
                : null;

            var zila = store.ZilaId.HasValue
                ? await _unitOfWork.Zilas.GetByIdAsync(store.ZilaId.Value)
                : null;

            var upazila = store.UpazilaId.HasValue
                ? await _unitOfWork.Upazilas.GetByIdAsync(store.UpazilaId.Value)
                : null;

            // Get manager details if InCharge contains userId
            string managerName = null;
            string managerId = null;
            if (!string.IsNullOrEmpty(store.InCharge))
            {
                // Try to get user by ID (assuming InCharge stores user ID)
                var manager = await _userManager.FindByIdAsync(store.InCharge);
                if (manager != null)
                {
                    managerName = manager.FullName ?? manager.UserName;
                    managerId = manager.Id;
                }
                else
                {
                    // If not a valid user ID, treat it as a name
                    managerName = store.InCharge;
                }
            }

            return new StoreDto
            {
                Id = store.Id,
                Name = store.Name,
                Code = store.Code,
                Location = store.Location,
                IsActive = store.IsActive,

                // Organization Assignment
                RangeId = store.RangeId,
                RangeName = range?.Name,
                RangeCode = range?.Code,

                BattalionId = store.BattalionId,
                BattalionName = battalion?.Name,
                BattalionCode = battalion?.Code,

                ZilaId = store.ZilaId,
                ZilaName = zila?.Name,
                ZilaCode = zila?.Code,

                UpazilaId = store.UpazilaId,
                UpazilaName = upazila?.Name,
                UpazilaCode = upazila?.Code,

                UnionId = store.UnionId,

                // Store Details
                Level = store.Level,
                StoreTypeId = store.StoreTypeId,
                StoreTypeName = storeType?.Name,
                StoreTypeCode = storeType?.Code,

                Capacity = store.TotalCapacity,
                UsedCapacity = store.UsedCapacity,
                AvailableCapacity = (store.TotalCapacity ?? 0) - (store.UsedCapacity ?? 0),
                UsedCapacityPercentage = store.TotalCapacity.HasValue && store.TotalCapacity > 0
                    ? (decimal)((store.UsedCapacity ?? 0) / store.TotalCapacity.Value * 100)
                    : 0,

                OperatingHours = store.OperatingHours,

                // Contact Information
                ContactNumber = store.ContactNumber,
                Email = store.Email,
                ManagerId = managerId,
                ManagerName = managerName,

                // Additional Info
                Remarks = store.Description,

                // Audit fields
                CreatedBy = store.CreatedBy,
                CreatedAt = store.CreatedAt,
                UpdatedBy = store.UpdatedBy,
                UpdatedAt = store.UpdatedAt
            };
        }
        public async Task<IEnumerable<StoreStockDto>> GetItemDistributionAcrossStoresAsync(int itemId)
        {
            var storeStocks = await _unitOfWork.StoreStocks.Query()
                .Include(ss => ss.Store)
                .Include(ss => ss.Item)
                .ThenInclude(i => i.SubCategory)
                    .ThenInclude(sc => sc.Category)
                .Where(ss => ss.ItemId == itemId)
                .ToListAsync();

            return storeStocks.Select(ss => new StoreStockDto
            {
                Id = ss.Id,
                StoreId = ss.StoreId,
                StoreName = ss.Store?.Name,
                StoreCode = ss.Store?.Code,
                ItemId = ss.ItemId,
                ItemName = ss.Item?.Name,
                ItemCode = ss.Item?.Code,
                // ✅ FIX: Use null-conditional operator for entire chain
                CategoryName = ss.Item?.SubCategory?.Category?.Name ?? "Uncategorized",
                Quantity = ss.Quantity,
                MinimumStock = ss.Item?.MinimumStock ?? 0m,
                MaximumStock = ss.Item?.MaximumStock ?? 0m,
                ReorderLevel = ss.ReorderLevel,
                Unit = ss.Item?.Unit ?? "Piece",
                UnitPrice = ss.Item?.UnitPrice ?? 0m,
                TotalValue = ss.Quantity * (ss.Item?.UnitPrice ?? 0m)
            }).ToList();
        }
        public async Task<(int successCount, List<string> errors)> AddUsersToStoreBatchAsync(int storeId, List<string> userIds)
        {
            var errors = new List<string>();
            int successCount = 0;

            try
            {
                // Validate store exists
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                {
                    throw new InvalidOperationException($"Store with ID {storeId} not found");
                }

                // Get existing assignments to avoid duplicates
                var existingAssignments = await _unitOfWork.UserStores
                    .FindAsync(us => us.StoreId == storeId && us.IsActive);
                var existingUserIds = existingAssignments.Select(us => us.UserId).ToList();

                // Prepare all user stores to add
                var userStoresToAdd = new List<UserStore>();

                foreach (var userId in userIds)
                {
                    // Check if user exists
                    var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user == null)
                    {
                        errors.Add($"User ID {userId}: Not found");
                        continue;
                    }

                    // Check if already assigned
                    if (existingUserIds.Contains(userId))
                    {
                        _logger.LogInformation("User {UserId} already assigned to store {StoreId}", userId, storeId);
                        continue; // Skip, not an error
                    }

                    // Prepare user store record
                    userStoresToAdd.Add(new UserStore
                    {
                        UserId = userId,
                        StoreId = storeId,
                        AssignedDate = DateTime.UtcNow,
                        AssignedBy = "System",
                        IsActive = true
                    });
                }

                // Add all users in a SINGLE transaction
                if (userStoresToAdd.Any())
                {
                    foreach (var userStore in userStoresToAdd)
                    {
                        await _unitOfWork.UserStores.AddAsync(userStore);
                    }

                    // Commit ALL at once
                    await _unitOfWork.CompleteAsync();
                    successCount = userStoresToAdd.Count;

                    // Log activity
                    await _activityLogService.LogActivityAsync(
                        "Store",
                        storeId,
                        "BulkAddUsers",
                        $"Added {successCount} users to store",
                        "System"
                    );

                    _logger.LogInformation("Successfully added {Count} users to store {StoreId} in batch", successCount, storeId);
                }

                return (successCount, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch adding users to store {StoreId}", storeId);
                throw;
            }
        }


        public async Task<StoreStockDto> GetStoreStockItemAsync(int storeId, int itemId)
        {
            var storeStock = await _unitOfWork.StoreStocks.Query()
                .Include(ss => ss.Store)
                .Include(ss => ss.Item)
                .ThenInclude(y => y.SubCategory)
                    .ThenInclude(i => i.Category)
                .FirstOrDefaultAsync(ss => ss.StoreId == storeId && ss.ItemId == itemId);

            if (storeStock == null)
                return null;

            return new StoreStockDto
            {
                Id = storeStock.Id,
                StoreId = storeStock.StoreId,
                StoreName = storeStock.Store?.Name,
                StoreCode = storeStock.Store?.Code,
                ItemId = storeStock.ItemId,
                ItemName = storeStock.Item?.Name,
                ItemCode = storeStock.Item?.Code,
                CategoryName = storeStock.Item?.SubCategory.Category?.Name,
                Quantity = storeStock.Quantity,
                MinimumStock = storeStock.Item.MinimumStock,
                MaximumStock = storeStock.Item.MaximumStock,
                ReorderLevel = storeStock.ReorderLevel,
                Unit = storeStock.Item?.Unit,
                UnitPrice = storeStock.Item?.UnitPrice,
                TotalValue = storeStock.Quantity * (storeStock.Item?.UnitPrice ?? 0)
            };
        }
        // Replace the GetStoreStockAsync method in your StoreService.cs with this fixed version

        public async Task<IEnumerable<StoreStockDto>> GetStoreStockAsync(int storeId)
        {
            try
            {
                var storeStocks = await _unitOfWork.StoreStocks.Query()
                    .Include(ss => ss.Item)
                        .ThenInclude(i => i.SubCategory)
                            .ThenInclude(sc => sc.Category)
                    .Include(ss => ss.Store)
                    .Where(ss => ss.StoreId == storeId && ss.Item != null && ss.Item.IsActive)
                    .ToListAsync();

                return storeStocks.Select(ss => new StoreStockDto
                {
                    Id = ss.Id,
                    StoreId = ss.StoreId,
                    StoreName = ss.Store?.Name ?? "Unknown Store",
                    ItemId = ss.ItemId,
                    ItemName = ss.Item?.Name ?? "Unknown Item",
                    ItemCode = ss.Item?.Code ?? ss.Item?.ItemCode ?? "N/A",
                    CategoryName = ss.Item?.SubCategory?.Category?.Name ?? "Uncategorized",
                    SubCategoryName = ss.Item?.SubCategory?.Name ?? "N/A",
                    Quantity = ss.Quantity,
                    MinimumStock = ss.Item?.MinimumStock ?? 0,
                    MaximumStock = ss.Item?.MaximumStock ?? 0,
                    ReorderLevel = ss.ReorderLevel,
                    Unit = ss.Item?.Unit ?? "Piece",
                    UnitPrice = ss.Item?.UnitPrice ?? 0,
                    TotalValue = ss.Quantity * (ss.Item?.UnitPrice ?? 0),
                    IsActive = ss.Item?.IsActive ?? false
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store stock for store {StoreId}", storeId);
                return new List<StoreStockDto>();
            }
        }
        public async Task UpdateStockAsync(StockUpdateDto dto)
        {
            try
            {
                // Find the store
                var store = await _unitOfWork.Stores.Query()
                    .Include(s => s.StoreItems)
                    .FirstOrDefaultAsync(s => s.Id == dto.StoreId);

                if (store == null)
                {
                    throw new InvalidOperationException($"Store with ID {dto.StoreId} not found");
                }

                // Find or create store item (stock) record
                var storeItem = store.StoreItems.FirstOrDefault(si => si.ItemId == dto.ItemId);

                decimal oldBalance = 0;
                decimal newBalance = 0;

                if (storeItem == null)
                {
                    // Create new store item record
                    storeItem = new StoreItem
                    {
                        StoreId = dto.StoreId,
                        ItemId = dto.ItemId,
                        CurrentStock = 0,
                        MinimumStock = 10, // Default
                        MaximumStock = 100, // Default
                        LastUpdated = DateTime.Now,
                        CreatedBy = dto.UpdatedBy,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    await _unitOfWork.StoreItems.AddAsync(storeItem);
                    oldBalance = 0;
                }
                else
                {
                    oldBalance = storeItem.CurrentStock;
                }

                // Update quantity based on type
                if (dto.Type == "IN")
                {
                    storeItem.CurrentStock += dto.Quantity;
                    newBalance = storeItem.CurrentStock;
                }
                else if (dto.Type == "OUT")
                {
                    if (storeItem.CurrentStock < dto.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock. Available: {storeItem.CurrentStock}, Requested: {dto.Quantity}");
                    }
                    storeItem.CurrentStock -= dto.Quantity;
                    newBalance = storeItem.CurrentStock;
                }

                storeItem.LastUpdated = DateTime.Now;
                storeItem.UpdatedBy = dto.UpdatedBy;
                storeItem.UpdatedAt = DateTime.Now;

                // Create stock movement record
                var movement = new StockMovement
                {
                    StoreId = dto.StoreId,
                    ItemId = dto.ItemId,
                    Quantity = dto.Quantity,
                    MovementType = dto.Type,
                    ReferenceType = "Purchase",
                    ReferenceNo = dto.Reference,
                    MovementDate = DateTime.Now,
                    OldBalance = oldBalance,
                    NewBalance = newBalance,
                    Reason = dto.Type == "IN" ? "Purchase Receive" : "Stock Issue",
                    Notes = $"Stock {dto.Type} for {dto.Reference}",
                    Remarks = $"Batch: {dto.BatchNo}, Expiry: {dto.ExpiryDate?.ToString("yyyy-MM-dd") ?? "N/A"}",
                    MovedBy = dto.UpdatedBy,
                    CreatedBy = dto.UpdatedBy,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.StockMovements.AddAsync(movement);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating stock: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<StoreItemDto>> GetStoreItemsAsync(int storeId)
        {
            try
            {
                var storeItems = await _unitOfWork.StoreItems.Query()
                    .Include(si => si.Item)
                        .ThenInclude(i => i.SubCategory)
                            .ThenInclude(sc => sc.Category)
                    .Include(si => si.Store)
                    .Where(si => si.StoreId == storeId && si.IsActive && si.Item != null && si.Item.IsActive)
                    .ToListAsync();

                return storeItems.Select(si => new StoreItemDto
                {
                    Id = si.Id,
                    StoreId = si.StoreId,
                    StoreName = si.Store?.Name ?? "Unknown Store",
                    ItemId = si.ItemId,
                    ItemName = si.Item?.Name ?? "Unknown Item",
                    ItemCode = si.Item?.Code ?? si.Item?.ItemCode ?? "N/A",
                    CategoryName = si.Item?.SubCategory?.Category?.Name ?? "Uncategorized",
                    SubCategoryName = si.Item?.SubCategory?.Name ?? "N/A",
                    Quantity = si.Quantity,
                    MinimumStock = si.MinimumStock,
                    MaximumStock = si.MaximumStock,
                    ReorderLevel = si.ReorderLevel,
                    Unit = si.Item?.Unit ?? "Piece",
                    Location = si.Location,
                    IsActive = si.IsActive
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store items for store {StoreId}", storeId);
                return new List<StoreItemDto>();
            }
        }

        public async Task<SelectList> GetSelectListAsync()
        {
            var stores = await _unitOfWork.Stores
                .Query()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Name} - {s.Code}"
                })
                .ToListAsync();

            return new SelectList(stores, "Value", "Text");
        }

        private class StoreCsvRecord
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Level { get; set; }
            public string ManagerUsername { get; set; }
            public string RangeCode { get; set; }
            public string BattalionCode { get; set; }
            public string ZilaCode { get; set; }
            public string UpazilaCode { get; set; }
            public string Location { get; set; }
            public string ContactNumber { get; set; }
            public string Email { get; set; }
            public string Capacity { get; set; }
            public string OperatingHours { get; set; }
            public string IsActive { get; set; }
        }
    }
}