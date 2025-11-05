using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class BattalionService : IBattalionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BattalionService> _logger;
    private readonly IActivityLogService _activityLogService;

    public BattalionService(
        IUnitOfWork unitOfWork,
        ILogger<BattalionService> logger,
        IActivityLogService activityLogService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
    }

    public async Task<IEnumerable<BattalionDto>> GetAllBattalionsAsync()
    {
        try
        {
            var battalions = await _unitOfWork.Battalions.GetAllWithIncludesAsync(
                b => b.Range,
                b => b.BattalionStores,
                b => b.Users
            );

            var battalionDtos = new List<BattalionDto>();

            foreach (var battalion in battalions)
            {
                var dto = MapToDto(battalion);
                battalionDtos.Add(dto);
            }

            return battalionDtos.OrderBy(b => b.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all battalions");
            throw new InvalidOperationException("Failed to retrieve battalions", ex);
        }
    }

    public async Task<IEnumerable<BattalionDto>> GetActiveBattalionsAsync()
    {
        try
        {
            var battalions = await _unitOfWork.Battalions.FindAsync(b => b.IsActive);
            var dtos = new List<BattalionDto>();

            foreach (var battalion in battalions)
            {
                dtos.Add(MapToDto(battalion));
            }

            return dtos.OrderBy(b => b.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active battalions");
            throw new InvalidOperationException("Failed to retrieve active battalions", ex);
        }
    }

    public async Task<IEnumerable<BattalionDto>> GetBattalionsByRangeAsync(int rangeId)
    {
        try
        {
            var battalions = await _unitOfWork.Battalions.FindAsync(b => b.RangeId == rangeId);
            var dtos = new List<BattalionDto>();

            foreach (var battalion in battalions)
            {
                dtos.Add(MapToDto(battalion));
            }

            return dtos.OrderBy(b => b.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving battalions for range {RangeId}", rangeId);
            throw new InvalidOperationException($"Failed to retrieve battalions for range {rangeId}", ex);
        }
    }

    public async Task<IEnumerable<BattalionDto>> GetBattalionsByTypeAsync(BattalionType type)
    {
        try
        {
            var battalions = await _unitOfWork.Battalions.FindAsync(b => b.Type == type);
            var dtos = new List<BattalionDto>();

            foreach (var battalion in battalions)
            {
                dtos.Add(MapToDto(battalion));
            }

            return dtos.OrderBy(b => b.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving battalions by type {Type}", type);
            throw new InvalidOperationException($"Failed to retrieve battalions by type {type}", ex);
        }
    }

    public async Task<BattalionDto> GetBattalionByIdAsync(int id)
    {
        try
        {
            var battalion = await _unitOfWork.Battalions.GetByIdWithIncludesAsync(
                id,
                b => b.Range,
                b => b.BattalionStores,
                b => b.Users
            );

            if (battalion == null)
            {
                _logger.LogWarning("Battalion with ID {Id} not found", id);
                return null;
            }

            return MapToDto(battalion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving battalion with ID {Id}", id);
            throw new InvalidOperationException($"Failed to retrieve battalion with ID {id}", ex);
        }
    }

    public async Task<BattalionDto> GetBattalionByCodeAsync(string code)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Battalion code cannot be empty", nameof(code));
            }

            var battalion = await _unitOfWork.Battalions.FirstOrDefaultAsync(b => b.Code == code);
            return battalion != null ? MapToDto(battalion) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving battalion with code {Code}", code);
            throw new InvalidOperationException($"Failed to retrieve battalion with code {code}", ex);
        }
    }

    public async Task<BattalionDto> CreateBattalionAsync(BattalionDto battalionDto)
    {
        try
        {
            // Validate required fields
            ValidateBattalionDto(battalionDto, isUpdate: false);

            // Validate unique name and code
            if (await BattalionExistsAsync(battalionDto.Name))
            {
                throw new InvalidOperationException($"Battalion with name '{battalionDto.Name}' already exists");
            }

            if (await BattalionCodeExistsAsync(battalionDto.Code))
            {
                throw new InvalidOperationException($"Battalion with code '{battalionDto.Code}' already exists");
            }

            // Validate Range exists if provided
            if (battalionDto.RangeId.HasValue)
            {
                var range = await _unitOfWork.Ranges.GetByIdAsync(battalionDto.RangeId.Value);
                if (range == null)
                {
                    throw new InvalidOperationException($"Range with ID {battalionDto.RangeId} not found");
                }
            }

            var battalion = new Battalion
            {
                Name = battalionDto.Name?.Trim(),
                Code = battalionDto.Code?.Trim(),
                Type = battalionDto.Type,
                Location = battalionDto.Location?.Trim(),
                CommanderName = battalionDto.CommanderName?.Trim(),
                CommanderRank = battalionDto.CommanderRank?.Trim(),
                ContactNumber = battalionDto.ContactNumber?.Trim(),
                Email = battalionDto.Email?.Trim(),
                RangeId = battalionDto.RangeId,
                IsActive = battalionDto.IsActive,
                Remarks = battalionDto.Remarks?.Trim(),
                TotalPersonnel = battalionDto.TotalPersonnel,
                OfficerCount = battalionDto.OfficerCount,
                EnlistedCount = battalionDto.EnlistedCount,
                EstablishedDate = battalionDto.EstablishedDate,
                OperationalStatus = battalionDto.OperationalStatus,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = battalionDto.CreatedBy ?? "System"
            };

            await _unitOfWork.Battalions.AddAsync(battalion);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Successfully created battalion: {Name} ({Code}) by {User}",
                battalion.Name, battalion.Code, battalionDto.CreatedBy ?? "System");

            // Log activity (non-blocking - don't fail the operation if this fails)
            try
            {
                await _activityLogService.LogActivityAsync(
                    "Battalion",
                    battalion.Id,
                    "Create",
                    $"Created battalion: {battalion.Name} ({battalion.Code})",
                    battalionDto.CreatedBy ?? "System"
                );
            }
            catch (Exception logEx)
            {
                _logger.LogWarning(logEx, "Failed to log activity for battalion creation: {Name}", battalion.Name);
                // Don't throw - activity logging failure shouldn't fail the operation
            }

            return MapToDto(battalion);
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions as-is
        }
        catch (ArgumentException)
        {
            throw; // Re-throw validation exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating battalion: {Name}", battalionDto?.Name);
            throw new InvalidOperationException("An unexpected error occurred while creating the battalion. Please try again.", ex);
        }
    }

    public async Task UpdateBattalionAsync(BattalionDto battalionDto)
    {
        try
        {
            // Validate required fields
            ValidateBattalionDto(battalionDto, isUpdate: true);

            var battalion = await _unitOfWork.Battalions.GetByIdAsync(battalionDto.Id);
            if (battalion == null)
            {
                throw new InvalidOperationException($"Battalion with ID {battalionDto.Id} not found");
            }

            // Store original values for logging
            var originalName = battalion.Name;
            var originalCode = battalion.Code;

            // Validate unique name and code
            if (await BattalionExistsAsync(battalionDto.Name, battalionDto.Id))
            {
                throw new InvalidOperationException($"Battalion with name '{battalionDto.Name}' already exists");
            }

            if (await BattalionCodeExistsAsync(battalionDto.Code, battalionDto.Id))
            {
                throw new InvalidOperationException($"Battalion with code '{battalionDto.Code}' already exists");
            }

            // Validate Range exists if provided
            if (battalionDto.RangeId.HasValue)
            {
                var range = await _unitOfWork.Ranges.GetByIdAsync(battalionDto.RangeId.Value);
                if (range == null)
                {
                    throw new InvalidOperationException($"Range with ID {battalionDto.RangeId} not found");
                }
            }

            // Update properties
            battalion.Name = battalionDto.Name?.Trim();
            battalion.Code = battalionDto.Code?.Trim();
            battalion.Type = battalionDto.Type;
            battalion.Location = battalionDto.Location?.Trim();
            battalion.CommanderName = battalionDto.CommanderName?.Trim();
            battalion.CommanderRank = battalionDto.CommanderRank?.Trim();
            battalion.ContactNumber = battalionDto.ContactNumber?.Trim();
            battalion.Email = battalionDto.Email?.Trim();
            battalion.RangeId = battalionDto.RangeId;
            battalion.IsActive = battalionDto.IsActive;
            battalion.Remarks = battalionDto.Remarks?.Trim();
            battalion.TotalPersonnel = battalionDto.TotalPersonnel;
            battalion.OfficerCount = battalionDto.OfficerCount;
            battalion.EnlistedCount = battalionDto.EnlistedCount;
            battalion.EstablishedDate = battalionDto.EstablishedDate;
            battalion.OperationalStatus = battalionDto.OperationalStatus;
            battalion.UpdatedAt = DateTime.UtcNow;
            battalion.UpdatedBy = battalionDto.UpdatedBy ?? battalionDto.CreatedBy ?? "System";

            _unitOfWork.Battalions.Update(battalion);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Successfully updated battalion: {Name} ({Code}) by {User}",
                battalion.Name, battalion.Code, battalion.UpdatedBy);

            // Log activity (non-blocking - don't fail the operation if this fails)
            try
            {
                await _activityLogService.LogActivityAsync(
                    "Battalion",
                    battalion.Id,
                    "Update",
                    $"Updated battalion: {battalion.Name} ({battalion.Code})",
                    battalion.UpdatedBy
                );
            }
            catch (Exception logEx)
            {
                _logger.LogWarning(logEx, "Failed to log activity for battalion update: {Name}", battalion.Name);
                // Don't throw - activity logging failure shouldn't fail the operation
            }
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions as-is
        }
        catch (ArgumentException)
        {
            throw; // Re-throw validation exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating battalion with ID {Id}", battalionDto?.Id);
            throw new InvalidOperationException("An unexpected error occurred while updating the battalion. Please try again.", ex);
        }
    }

    public async Task DeleteBattalionAsync(int id)
    {
        try
        {
            var battalion = await _unitOfWork.Battalions.GetByIdAsync(id);
            if (battalion == null)
            {
                throw new InvalidOperationException($"Battalion with ID {id} not found");
            }

            // Store details for logging
            var battalionName = battalion.Name;
            var battalionCode = battalion.Code;

            // Check if battalion has any dependencies
            var hasStores = await _unitOfWork.BattalionStores.ExistsAsync(bs => bs.BattalionId == id);
            if (hasStores)
            {
                var storeCount = await GetBattalionStoreCountAsync(id);
                throw new InvalidOperationException($"Cannot delete battalion that has {storeCount} store{(storeCount > 1 ? "s" : "")} assigned");
            }

            // Check for users - Note: This may need adjustment based on your actual user entity structure
            var hasUsers = await _unitOfWork.Categories.ExistsAsync(u => u.Id == id); // This should be updated to actual user check
            if (hasUsers)
            {
                throw new InvalidOperationException("Cannot delete battalion that has users assigned");
            }

            _unitOfWork.Battalions.Remove(battalion);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Successfully deleted battalion: {Name} ({Code})", battalionName, battalionCode);

            // Log activity (non-blocking - don't fail the operation if this fails)
            try
            {
                await _activityLogService.LogActivityAsync(
                    "Battalion",
                    battalion.Id,
                    "Delete",
                    $"Deleted battalion: {battalionName} ({battalionCode})",
                    "System"
                );
            }
            catch (Exception logEx)
            {
                _logger.LogWarning(logEx, "Failed to log activity for battalion deletion: {Name}", battalionName);
                // Don't throw - activity logging failure shouldn't fail the operation
            }
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting battalion with ID {Id}", id);
            throw new InvalidOperationException("An unexpected error occurred while deleting the battalion. Please try again.", ex);
        }
    }

    public async Task<bool> BattalionExistsAsync(string name, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var trimmedName = name.Trim();
        return await _unitOfWork.Battalions.ExistsAsync(
            b => b.Name.ToLower() == trimmedName.ToLower() && (!excludeId.HasValue || b.Id != excludeId.Value)
        );
    }

    public async Task<bool> BattalionCodeExistsAsync(string code, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var trimmedCode = code.Trim();
        return await _unitOfWork.Battalions.ExistsAsync(
            b => b.Code.ToLower() == trimmedCode.ToLower() && (!excludeId.HasValue || b.Id != excludeId.Value)
        );
    }

    public async Task<string> GenerateBattalionCodeAsync(BattalionType type)
    {
        try
        {
            var prefix = type == BattalionType.Male ? "BN-M-" : "BN-F-";

            // Get all existing battalion codes with this prefix
            var existingBattalions = await _unitOfWork.Battalions.FindAsync(
                b => b.Type == type && !string.IsNullOrEmpty(b.Code) && b.Code.StartsWith(prefix)
            );

            var maxNumber = 0;
            foreach (var battalion in existingBattalions)
            {
                if (battalion.Code.Length >= prefix.Length + 2) // prefix + at least 2 digits
                {
                    var numberPart = battalion.Code.Substring(prefix.Length);
                    if (int.TryParse(numberPart, out var number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }
            }

            var newCode = $"{prefix}{(maxNumber + 1):D2}";

            // Double-check the generated code doesn't exist (safety check)
            var exists = await BattalionCodeExistsAsync(newCode);
            if (exists)
            {
                // If somehow it exists, try with a higher number
                for (int i = maxNumber + 2; i <= 999; i++)
                {
                    newCode = $"{prefix}{i:D2}";
                    exists = await BattalionCodeExistsAsync(newCode);
                    if (!exists)
                    {
                        break;
                    }
                }

                if (exists)
                {
                    throw new InvalidOperationException($"Unable to generate unique battalion code for type {type}. All codes may be exhausted.");
                }
            }

            _logger.LogInformation("Generated battalion code: {Code} for type: {Type}", newCode, type);
            return newCode;
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error generating battalion code for type {Type}", type);
            throw new InvalidOperationException($"Failed to generate battalion code for type {type}. Please try again.", ex);
        }
    }

    public async Task<IEnumerable<BattalionDto>> SearchBattalionsAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllBattalionsAsync();
            }

            var trimmedSearchTerm = searchTerm.Trim();
            var battalions = await _unitOfWork.Battalions.FindAsync(
                b => b.Name.Contains(trimmedSearchTerm) ||
                     b.Code.Contains(trimmedSearchTerm) ||
                     b.CommanderName.Contains(trimmedSearchTerm) ||
                     b.Location.Contains(trimmedSearchTerm)
            );

            var dtos = new List<BattalionDto>();
            foreach (var battalion in battalions)
            {
                dtos.Add(MapToDto(battalion));
            }

            return dtos.OrderBy(b => b.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching battalions with term '{SearchTerm}'", searchTerm);
            throw new InvalidOperationException("Failed to search battalions", ex);
        }
    }

    public async Task<int> GetBattalionPersonnelCountAsync(int battalionId)
    {
        try
        {
            var battalion = await _unitOfWork.Battalions.GetByIdAsync(battalionId);
            return battalion?.TotalPersonnel ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting personnel count for battalion {Id}", battalionId);
            return 0;
        }
    }

    public async Task<int> GetBattalionStoreCountAsync(int battalionId)
    {
        try
        {
            return await _unitOfWork.BattalionStores.CountAsync(bs => bs.BattalionId == battalionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store count for battalion {Id}", battalionId);
            return 0;
        }
    }

    public async Task<IEnumerable<StoreDto>> GetBattalionStoresAsync(int battalionId)
    {
        try
        {
            var battalionStores = await _unitOfWork.BattalionStores.FindAsync(bs => bs.BattalionId == battalionId);
            var stores = new List<StoreDto>();

            foreach (var bs in battalionStores)
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(bs.StoreId);
                if (store != null)
                {
                    stores.Add(new StoreDto
                    {
                        Id = store.Id,
                        Code = store.Code,
                        Name = store.Name,
                        Location = store.Location,
                        IsActive = store.IsActive
                    });
                }
            }

            return stores;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stores for battalion {Id}", battalionId);
            throw new InvalidOperationException($"Failed to retrieve stores for battalion {battalionId}", ex);
        }
    }

    public async Task AssignStoreToBattalionAsync(int battalionId, int? storeId, bool isPrimary = false)
    {
        try
        {
            // Validate battalion exists
            var battalion = await _unitOfWork.Battalions.GetByIdAsync(battalionId);
            if (battalion == null)
            {
                throw new InvalidOperationException($"Battalion with ID {battalionId} not found");
            }

            // Validate store exists
            var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
            if (store == null)
            {
                throw new InvalidOperationException($"Store with ID {storeId} not found");
            }

            // Check if already assigned
            var existing = await _unitOfWork.BattalionStores.FirstOrDefaultAsync(
                bs => bs.BattalionId == battalionId && bs.StoreId == storeId
            );

            if (existing != null)
            {
                throw new InvalidOperationException("Store is already assigned to this battalion");
            }

            // If setting as primary, unset other primary stores
            if (isPrimary)
            {
                var primaryStores = await _unitOfWork.BattalionStores.FindAsync(
                    bs => bs.BattalionId == battalionId && bs.IsPrimaryStore
                );

                foreach (var ps in primaryStores)
                {
                    ps.IsPrimaryStore = false;
                    _unitOfWork.BattalionStores.Update(ps);
                }
            }

            var battalionStore = new BattalionStore
            {
                BattalionId = battalionId,
                StoreId = storeId,
                IsPrimaryStore = isPrimary,
                EffectiveFrom = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await _unitOfWork.BattalionStores.AddAsync(battalionStore);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Successfully assigned store {StoreName} to battalion {BattalionName} (Primary: {IsPrimary})",
                store.Name, battalion.Name, isPrimary);

            // Log activity (non-blocking - don't fail the operation if this fails)
            try
            {
                await _activityLogService.LogActivityAsync(
                    "Battalion",
                    battalionId,
                    "AssignStore",
                    $"Assigned store {store.Name} to battalion {battalion.Name} (Primary: {isPrimary})",
                    "System"
                );
            }
            catch (Exception logEx)
            {
                _logger.LogWarning(logEx, "Failed to log activity for store assignment: Battalion {BattalionName}, Store {StoreName}",
                    battalion.Name, store.Name);
                // Don't throw - activity logging failure shouldn't fail the operation
            }
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error assigning store {StoreId} to battalion {BattalionId}", storeId, battalionId);
            throw new InvalidOperationException("An unexpected error occurred while assigning the store. Please try again.", ex);
        }
    }

    public async Task RemoveStoreFromBattalionAsync(int battalionId, int? storeId)
    {
        try
        {
            var battalionStore = await _unitOfWork.BattalionStores.FirstOrDefaultAsync(
                bs => bs.BattalionId == battalionId && bs.StoreId == storeId
            );

            if (battalionStore == null)
            {
                throw new InvalidOperationException("Store is not assigned to this battalion");
            }

            battalionStore.EffectiveTo = DateTime.UtcNow;
            battalionStore.UpdatedAt = DateTime.UtcNow;
            battalionStore.UpdatedBy = "System";

            _unitOfWork.BattalionStores.Update(battalionStore);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Successfully removed store {StoreId} from battalion {BattalionId}", storeId, battalionId);

            // Log activity (non-blocking - don't fail the operation if this fails)
            try
            {
                await _activityLogService.LogActivityAsync(
                    "Battalion",
                    battalionId,
                    "RemoveStore",
                    $"Removed store {storeId} from battalion {battalionId}",
                    "System"
                );
            }
            catch (Exception logEx)
            {
                _logger.LogWarning(logEx, "Failed to log activity for store removal: Battalion {BattalionId}, Store {StoreId}",
                    battalionId, storeId);
                // Don't throw - activity logging failure shouldn't fail the operation
            }
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error removing store {StoreId} from battalion {BattalionId}", storeId, battalionId);
            throw new InvalidOperationException("An unexpected error occurred while removing the store. Please try again.", ex);
        }
    }

    public async Task<Dictionary<string, object>> GetBattalionStatisticsAsync(int battalionId)
    {
        try
        {
            var battalion = await _unitOfWork.Battalions.GetByIdAsync(battalionId);
            if (battalion == null)
            {
                throw new InvalidOperationException($"Battalion with ID {battalionId} not found");
            }

            var stats = new Dictionary<string, object>
            {
                ["TotalPersonnel"] = battalion.TotalPersonnel,
                ["OfficerCount"] = battalion.OfficerCount,
                ["EnlistedCount"] = battalion.EnlistedCount,
                ["StoreCount"] = await GetBattalionStoreCountAsync(battalionId),
                ["ActiveSince"] = battalion.EstablishedDate?.ToString("yyyy-MM-dd") ?? "N/A",
                ["OperationalStatus"] = battalion.OperationalStatus?.ToString() ?? "Unknown"
            };

            return stats;
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions as-is
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for battalion {Id}", battalionId);
            throw new InvalidOperationException("Failed to get battalion statistics", ex);
        }
    }

    public async Task<SelectList> GetSelectListAsync()
    {
        var battalions = await _unitOfWork.Battalions
            .Query()
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = $"{b.Name} - {b.Code}"
            })
            .ToListAsync();

        return new SelectList(battalions, "Value", "Text");
    }

    #region Private Helper Methods

    private void ValidateBattalionDto(BattalionDto battalionDto, bool isUpdate)
    {
        if (battalionDto == null)
        {
            throw new ArgumentNullException(nameof(battalionDto), "Battalion information is required");
        }

        if (string.IsNullOrWhiteSpace(battalionDto.Name))
        {
            throw new ArgumentException("Battalion name is required and cannot be empty", nameof(battalionDto.Name));
        }

        if (battalionDto.Name.Trim().Length > 100) // Adjust max length as needed
        {
            throw new ArgumentException("Battalion name cannot exceed 100 characters", nameof(battalionDto.Name));
        }

        if (string.IsNullOrWhiteSpace(battalionDto.Code))
        {
            throw new ArgumentException("Battalion code is required and cannot be empty", nameof(battalionDto.Code));
        }

        if (battalionDto.Code.Trim().Length > 20) // Adjust max length as needed
        {
            throw new ArgumentException("Battalion code cannot exceed 20 characters", nameof(battalionDto.Code));
        }

        // Validate personnel counts
        if (battalionDto.OfficerCount < 0)
        {
            throw new ArgumentException("Officer count cannot be negative", nameof(battalionDto.OfficerCount));
        }

        if (battalionDto.EnlistedCount < 0)
        {
            throw new ArgumentException("Enlisted count cannot be negative", nameof(battalionDto.EnlistedCount));
        }

        if (battalionDto.TotalPersonnel != (battalionDto.OfficerCount + battalionDto.EnlistedCount))
        {
            throw new ArgumentException("Total personnel must equal the sum of officer and enlisted counts", nameof(battalionDto.TotalPersonnel));
        }

        // Validate email format if provided
        if (!string.IsNullOrWhiteSpace(battalionDto.Email))
        {
            try
            {
                var email = new System.Net.Mail.MailAddress(battalionDto.Email);
            }
            catch
            {
                throw new ArgumentException("Invalid email address format", nameof(battalionDto.Email));
            }
        }

        // Validate established date
        if (battalionDto.EstablishedDate.HasValue && battalionDto.EstablishedDate.Value > DateTime.Now)
        {
            throw new ArgumentException("Established date cannot be in the future", nameof(battalionDto.EstablishedDate));
        }
    }

    private BattalionDto MapToDto(Battalion battalion)
    {
        if (battalion == null)
        {
            return null;
        }

        return new BattalionDto
        {
            Id = battalion.Id,
            Name = battalion.Name ?? string.Empty,
            Code = battalion.Code ?? string.Empty,
            Type = battalion.Type,
            Location = battalion.Location ?? string.Empty,
            CommanderName = battalion.CommanderName ?? string.Empty,
            CommanderRank = battalion.CommanderRank ?? string.Empty,
            ContactNumber = battalion.ContactNumber ?? string.Empty,
            Email = battalion.Email ?? string.Empty,
            RangeId = battalion.RangeId,
            RangeName = battalion.Range?.Name ?? string.Empty,
            IsActive = battalion.IsActive,
            Remarks = battalion.Remarks ?? string.Empty,
            TotalPersonnel = battalion.TotalPersonnel,
            OfficerCount = battalion.OfficerCount,
            EnlistedCount = battalion.EnlistedCount,
            EstablishedDate = battalion.EstablishedDate,
            OperationalStatus = battalion.OperationalStatus,
            StoreCount = battalion.BattalionStores?.Count(bs => bs.EffectiveTo == null) ?? 0,
            PersonnelCount = battalion.TotalPersonnel,
            CreatedAt = battalion.CreatedAt,
            CreatedBy = battalion.CreatedBy ?? string.Empty,
            UpdatedAt = battalion.UpdatedAt,
            UpdatedBy = battalion.UpdatedBy ?? string.Empty
        };
    }

    #endregion
}