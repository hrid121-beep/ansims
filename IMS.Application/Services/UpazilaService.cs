using CsvHelper;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace IMS.Application.Services
{
    public class UpazilaService : IUpazilaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpazilaService> _logger;
        private readonly IActivityLogService _activityLogService;

        public UpazilaService(
            IUnitOfWork unitOfWork,
            ILogger<UpazilaService> logger,
            IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
        }

        public async Task<IEnumerable<UpazilaDto>> GetAllUpazilasAsync()
        {
            try
            {
                var upazilas = await _unitOfWork.Upazilas.GetAllAsync();
                var upazilaDtos = new List<UpazilaDto>();

                foreach (var upazila in upazilas)
                {
                    var dto = MapToDto(upazila);
                    dto.StoreCount = await GetUpazilaStoreCountAsync(upazila.Id);

                    if (upazila.ZilaId > 0)
                    {
                        var zila = await _unitOfWork.Zilas.GetByIdAsync(upazila.ZilaId);
                        if (zila != null)
                        {
                            dto.ZilaName = zila.Name;
                            if (zila.RangeId > 0)
                            {
                                var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                                dto.RangeName = range?.Name;
                            }
                        }
                    }

                    upazilaDtos.Add(dto);
                }

                return upazilaDtos.OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all upazilas");
                throw new InvalidOperationException("Failed to retrieve sub-districts", ex);
            }
        }

        public async Task<IEnumerable<UpazilaDto>> GetActiveUpazilasAsync()
        {
            try
            {
                var upazilas = await _unitOfWork.Upazilas.FindAsync(u => u.IsActive);
                var dtos = new List<UpazilaDto>();

                foreach (var upazila in upazilas)
                {
                    var dto = MapToDto(upazila);
                    if (upazila.ZilaId > 0)
                    {
                        var zila = await _unitOfWork.Zilas.GetByIdAsync(upazila.ZilaId);
                        if (zila != null)
                        {
                            dto.ZilaName = zila.Name;
                            if (zila.RangeId > 0)
                            {
                                var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                                dto.RangeName = range?.Name;
                            }
                        }
                    }
                    dtos.Add(dto);
                }

                return dtos.OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active upazilas");
                throw new InvalidOperationException("Failed to retrieve active sub-districts", ex);
            }
        }

        public async Task<IEnumerable<UpazilaDto>> GetUpazilasByZilaAsync(int zilaId)
        {
            try
            {
                var upazilas = await _unitOfWork.Upazilas.FindAsync(u => u.ZilaId == zilaId);
                var upazilaDtos = new List<UpazilaDto>();

                foreach (var upazila in upazilas)
                {
                    var dto = MapToDto(upazila);
                    dto.StoreCount = await GetUpazilaStoreCountAsync(upazila.Id);

                    if (upazila.ZilaId > 0)
                    {
                        var zila = await _unitOfWork.Zilas.GetByIdAsync(upazila.ZilaId);
                        if (zila != null)
                        {
                            dto.ZilaName = zila.Name;
                            if (zila.RangeId > 0)
                            {
                                var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                                dto.RangeName = range?.Name;
                            }
                        }
                    }

                    upazilaDtos.Add(dto);
                }

                return upazilaDtos.OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upazilas for zila {ZilaId}", zilaId);
                throw new InvalidOperationException($"Failed to retrieve sub-districts for district {zilaId}", ex);
            }
        }

        public async Task<IEnumerable<UpazilaDto>> GetUpazilasWithVDPUnitsAsync()
        {
            try
            {
                var upazilas = await _unitOfWork.Upazilas.FindAsync(u => u.HasVDPUnit);
                var dtos = new List<UpazilaDto>();

                foreach (var upazila in upazilas)
                {
                    var dto = MapToDto(upazila);
                    if (upazila.ZilaId > 0)
                    {
                        var zila = await _unitOfWork.Zilas.GetByIdAsync(upazila.ZilaId);
                        dto.ZilaName = zila?.Name;
                    }
                    dtos.Add(dto);
                }

                return dtos.OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upazilas with VDP units");
                throw new InvalidOperationException("Failed to retrieve sub-districts with VDP units", ex);
            }
        }

        public async Task<UpazilaDto> GetUpazilaByIdAsync(int id)
        {
            try
            {
                var upazila = await _unitOfWork.Upazilas.GetByIdAsync(id);
                if (upazila == null)
                {
                    _logger.LogWarning("Upazila with ID {Id} not found", id);
                    return null;
                }

                var dto = MapToDto(upazila);
                dto.StoreCount = await GetUpazilaStoreCountAsync(upazila.Id);

                if (upazila.ZilaId > 0)
                {
                    var zila = await _unitOfWork.Zilas.GetByIdAsync(upazila.ZilaId);
                    if (zila != null)
                    {
                        dto.ZilaName = zila.Name;
                        if (zila.RangeId > 0)
                        {
                            var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                            dto.RangeName = range?.Name;
                        }
                    }
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upazila with ID {Id}", id);
                throw new InvalidOperationException($"Failed to retrieve sub-district with ID {id}", ex);
            }
        }

        public async Task<UpazilaDto> GetUpazilaByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new ArgumentException("Sub-district code cannot be empty", nameof(code));
                }

                var upazila = await _unitOfWork.Upazilas.FirstOrDefaultAsync(u => u.Code == code);
                if (upazila == null)
                {
                    return null;
                }

                var dto = MapToDto(upazila);
                if (upazila.ZilaId > 0)
                {
                    var zila = await _unitOfWork.Zilas.GetByIdAsync(upazila.ZilaId);
                    if (zila != null)
                    {
                        dto.ZilaName = zila.Name;
                        if (zila.RangeId > 0)
                        {
                            var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                            dto.RangeName = range?.Name;
                        }
                    }
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upazila with code {Code}", code);
                throw new InvalidOperationException($"Failed to retrieve sub-district with code {code}", ex);
            }
        }

        public async Task<UpazilaDto> CreateUpazilaAsync(UpazilaDto upazilaDto)
        {
            try
            {
                // Validate required fields
                ValidateUpazilaDto(upazilaDto, isUpdate: false);

                // Validate unique name and code
                if (await UpazilaExistsAsync(upazilaDto.Name, upazilaDto.ZilaId))
                {
                    throw new InvalidOperationException($"Upazila with name '{upazilaDto.Name}' already exists in this district");
                }

                if (await UpazilaCodeExistsAsync(upazilaDto.Code))
                {
                    throw new InvalidOperationException($"Upazila with code '{upazilaDto.Code}' already exists");
                }

                // Validate Zila exists
                if (upazilaDto.ZilaId > 0)
                {
                    var zila = await _unitOfWork.Zilas.GetByIdAsync(upazilaDto.ZilaId);
                    if (zila == null)
                    {
                        throw new InvalidOperationException($"District with ID {upazilaDto.ZilaId} not found");
                    }
                }

                var upazila = new Upazila
                {
                    Name = upazilaDto.Name?.Trim(),
                    Code = upazilaDto.Code?.Trim(),
                    NameBangla = upazilaDto.NameBangla?.Trim(),
                    ZilaId = upazilaDto.ZilaId,
                    UpazilaOfficerName = upazilaDto.UpazilaOfficerName?.Trim(),
                    OfficerDesignation = upazilaDto.OfficerDesignation?.Trim(),
                    ContactNumber = upazilaDto.ContactNumber?.Trim(),
                    Email = upazilaDto.Email?.Trim(),
                    OfficeAddress = upazilaDto.OfficeAddress?.Trim(),
                    Area = upazilaDto.Area,
                    Population = upazilaDto.Population,
                    NumberOfUnions = upazilaDto.NumberOfUnions,
                    NumberOfVillages = upazilaDto.NumberOfVillages,
                    HasVDPUnit = upazilaDto.HasVDPUnit,
                    VDPMemberCount = upazilaDto.VDPMemberCount,
                    UpazilaChairmanName = upazilaDto.UpazilaChairmanName?.Trim(),
                    VDPOfficerName = upazilaDto.VDPOfficerName?.Trim(),
                    IsActive = upazilaDto.IsActive,
                    Remarks = upazilaDto.Remarks?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = upazilaDto.CreatedBy ?? "System"
                };

                await _unitOfWork.Upazilas.AddAsync(upazila);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully created upazila: {Name} ({Code}) by {User}",
                    upazila.Name, upazila.Code, upazilaDto.CreatedBy ?? "System");

                // Log activity (non-blocking - don't fail the operation if this fails)
                try
                {
                    await _activityLogService.LogActivityAsync(
                        "Upazila",
                        upazila.Id,
                        "Create",
                        $"Created upazila: {upazila.Name} ({upazila.Code})",
                        upazilaDto.CreatedBy ?? "System"
                    );
                }
                catch (Exception logEx)
                {
                    _logger.LogWarning(logEx, "Failed to log activity for upazila creation: {Name}", upazila.Name);
                    // Don't throw - activity logging failure shouldn't fail the operation
                }

                return MapToDto(upazila);
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
                _logger.LogError(ex, "Unexpected error creating upazila: {Name}", upazilaDto?.Name);
                throw new InvalidOperationException("An unexpected error occurred while creating the sub-district. Please try again.", ex);
            }
        }

        public async Task UpdateUpazilaAsync(UpazilaDto upazilaDto)
        {
            try
            {
                // Validate required fields
                ValidateUpazilaDto(upazilaDto, isUpdate: true);

                var upazila = await _unitOfWork.Upazilas.GetByIdAsync(upazilaDto.Id);
                if (upazila == null)
                {
                    throw new InvalidOperationException($"Upazila with ID {upazilaDto.Id} not found");
                }

                // Store original values for logging
                var originalName = upazila.Name;
                var originalCode = upazila.Code;

                // Validate unique name and code
                if (await UpazilaExistsAsync(upazilaDto.Name, upazilaDto.ZilaId, upazilaDto.Id))
                {
                    throw new InvalidOperationException($"Upazila with name '{upazilaDto.Name}' already exists in this district");
                }

                if (await UpazilaCodeExistsAsync(upazilaDto.Code, upazilaDto.Id))
                {
                    throw new InvalidOperationException($"Upazila with code '{upazilaDto.Code}' already exists");
                }

                // Validate Zila exists
                if (upazilaDto.ZilaId > 0)
                {
                    var zila = await _unitOfWork.Zilas.GetByIdAsync(upazilaDto.ZilaId);
                    if (zila == null)
                    {
                        throw new InvalidOperationException($"District with ID {upazilaDto.ZilaId} not found");
                    }
                }

                // Update properties
                upazila.Name = upazilaDto.Name?.Trim();
                upazila.Code = upazilaDto.Code?.Trim();
                upazila.NameBangla = upazilaDto.NameBangla?.Trim();
                upazila.ZilaId = upazilaDto.ZilaId;
                upazila.UpazilaOfficerName = upazilaDto.UpazilaOfficerName?.Trim();
                upazila.OfficerDesignation = upazilaDto.OfficerDesignation?.Trim();
                upazila.ContactNumber = upazilaDto.ContactNumber?.Trim();
                upazila.Email = upazilaDto.Email?.Trim();
                upazila.OfficeAddress = upazilaDto.OfficeAddress?.Trim();
                upazila.Area = upazilaDto.Area;
                upazila.Population = upazilaDto.Population;
                upazila.NumberOfUnions = upazilaDto.NumberOfUnions;
                upazila.NumberOfVillages = upazilaDto.NumberOfVillages;
                upazila.HasVDPUnit = upazilaDto.HasVDPUnit;
                upazila.VDPMemberCount = upazilaDto.VDPMemberCount;
                upazila.UpazilaChairmanName = upazilaDto.UpazilaChairmanName?.Trim();
                upazila.VDPOfficerName = upazilaDto.VDPOfficerName?.Trim();
                upazila.IsActive = upazilaDto.IsActive;
                upazila.Remarks = upazilaDto.Remarks?.Trim();
                upazila.UpdatedAt = DateTime.UtcNow;
                upazila.UpdatedBy = upazilaDto.UpdatedBy ?? upazilaDto.CreatedBy ?? "System";

                _unitOfWork.Upazilas.Update(upazila);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully updated upazila: {Name} ({Code}) by {User}",
                    upazila.Name, upazila.Code, upazila.UpdatedBy);

                // Log activity (non-blocking - don't fail the operation if this fails)
                try
                {
                    await _activityLogService.LogActivityAsync(
                        "Upazila",
                        upazila.Id,
                        "Update",
                        $"Updated upazila: {upazila.Name} ({upazila.Code})",
                        upazila.UpdatedBy
                    );
                }
                catch (Exception logEx)
                {
                    _logger.LogWarning(logEx, "Failed to log activity for upazila update: {Name}", upazila.Name);
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
                _logger.LogError(ex, "Unexpected error updating upazila with ID {Id}", upazilaDto?.Id);
                throw new InvalidOperationException("An unexpected error occurred while updating the sub-district. Please try again.", ex);
            }
        }

        public async Task DeleteUpazilaAsync(int id)
        {
            try
            {
                var upazila = await _unitOfWork.Upazilas.GetByIdAsync(id);
                if (upazila == null)
                {
                    throw new InvalidOperationException($"Upazila with ID {id} not found");
                }

                // Store details for logging
                var upazilaName = upazila.Name;
                var upazilaCode = upazila.Code;

                // Check if upazila has any dependencies
                var storeCount = await GetUpazilaStoreCountAsync(id);
                if (storeCount > 0)
                {
                    throw new InvalidOperationException($"Cannot delete upazila that has {storeCount} store{(storeCount > 1 ? "s" : "")} assigned");
                }

                // Check for unions when implemented
                // var unionCount = await GetUpazilaUnionCountAsync(id);
                // if (unionCount > 0)
                //     throw new InvalidOperationException($"Cannot delete upazila that has {unionCount} union{(unionCount > 1 ? "s" : "")} assigned");

                _unitOfWork.Upazilas.Remove(upazila);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully deleted upazila: {Name} ({Code})", upazilaName, upazilaCode);

                // Log activity (non-blocking - don't fail the operation if this fails)
                try
                {
                    await _activityLogService.LogActivityAsync(
                        "Upazila",
                        upazila.Id,
                        "Delete",
                        $"Deleted upazila: {upazilaName} ({upazilaCode})",
                        "System"
                    );
                }
                catch (Exception logEx)
                {
                    _logger.LogWarning(logEx, "Failed to log activity for upazila deletion: {Name}", upazilaName);
                    // Don't throw - activity logging failure shouldn't fail the operation
                }
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting upazila with ID {Id}", id);
                throw new InvalidOperationException("An unexpected error occurred while deleting the sub-district. Please try again.", ex);
            }
        }

        public async Task<bool> UpazilaExistsAsync(string name, int zilaId, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var trimmedName = name.Trim();
            return await _unitOfWork.Upazilas.ExistsAsync(
                u => u.Name.ToLower() == trimmedName.ToLower() &&
                     u.ZilaId == zilaId &&
                     (!excludeId.HasValue || u.Id != excludeId.Value)
            );
        }

        public async Task<bool> UpazilaCodeExistsAsync(string code, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var trimmedCode = code.Trim();
            return await _unitOfWork.Upazilas.ExistsAsync(
                u => u.Code.ToLower() == trimmedCode.ToLower() &&
                     (!excludeId.HasValue || u.Id != excludeId.Value)
            );
        }

        public async Task<bool> CheckNameExistsAsync(string name, int zilaId, int? excludeId = null)
        {
            return await UpazilaExistsAsync(name, zilaId, excludeId);
        }

        public async Task<string> GenerateUpazilaCodeAsync(int zilaId)
        {
            try
            {
                var zila = await _unitOfWork.Zilas.GetByIdAsync(zilaId);
                if (zila == null)
                {
                    throw new ArgumentException("District selection is required to generate code", nameof(zilaId));
                }

                // Create code based on zila code
                var zilaCode = zila.Code.Replace("ZILA-", "");
                var existingUpazilas = await _unitOfWork.Upazilas.FindAsync(u => u.ZilaId == zilaId);
                var count = existingUpazilas.Count() + 1;

                // Basic code format: UPZ-{ZilaCode}-{Count}
                var basicCode = $"UPZ-{zilaCode}-{count:D3}";

                // Check if the basic code exists
                var exists = await UpazilaCodeExistsAsync(basicCode);

                if (!exists)
                {
                    _logger.LogInformation("Generated upazila code: {Code} for zila: {ZilaId}", basicCode, zilaId);
                    return basicCode;
                }

                // If basic code exists, try with incremental numbers
                for (int i = count + 1; i <= count + 99; i++)
                {
                    var numberedCode = $"UPZ-{zilaCode}-{i:D3}";
                    exists = await UpazilaCodeExistsAsync(numberedCode);

                    if (!exists)
                    {
                        _logger.LogInformation("Generated upazila code: {Code} for zila: {ZilaId}", numberedCode, zilaId);
                        return numberedCode;
                    }
                }

                throw new InvalidOperationException($"Unable to generate unique upazila code for district '{zila.Name}'. All codes may be exhausted.");
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error generating upazila code for zila {ZilaId}", zilaId);
                throw new InvalidOperationException($"Failed to generate sub-district code for district. Please try again.", ex);
            }
        }

        public async Task<IEnumerable<UpazilaDto>> SearchUpazilasAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllUpazilasAsync();
                }

                var trimmedSearchTerm = searchTerm.Trim();
                var upazilas = await _unitOfWork.Upazilas.FindAsync(
                    u => u.Name.Contains(trimmedSearchTerm) ||
                         u.Code.Contains(trimmedSearchTerm) ||
                         u.NameBangla.Contains(trimmedSearchTerm) ||
                         u.UpazilaOfficerName.Contains(trimmedSearchTerm) ||
                         u.UpazilaChairmanName.Contains(trimmedSearchTerm)
                );

                var dtos = new List<UpazilaDto>();
                foreach (var upazila in upazilas)
                {
                    var dto = MapToDto(upazila);
                    dto.StoreCount = await GetUpazilaStoreCountAsync(upazila.Id);

                    if (upazila.ZilaId > 0)
                    {
                        var zila = await _unitOfWork.Zilas.GetByIdAsync(upazila.ZilaId);
                        if (zila != null)
                        {
                            dto.ZilaName = zila.Name;
                            if (zila.RangeId > 0)
                            {
                                var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                                dto.RangeName = range?.Name;
                            }
                        }
                    }

                    dtos.Add(dto);
                }

                return dtos.OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching upazilas with term '{SearchTerm}'", searchTerm);
                throw new InvalidOperationException("Failed to search sub-districts", ex);
            }
        }

        public async Task<int> GetUpazilaStoreCountAsync(int upazilaId)
        {
            try
            {
                var stores = await _unitOfWork.Stores.FindAsync(s => s.UpazilaId == upazilaId);
                return stores.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store count for upazila {UpazilaId}", upazilaId);
                return 0;
            }
        }

        public async Task UpdateVDPMemberCountAsync(int upazilaId, int memberCount)
        {
            try
            {
                var upazila = await _unitOfWork.Upazilas.GetByIdAsync(upazilaId);
                if (upazila == null)
                {
                    throw new InvalidOperationException($"Upazila with ID {upazilaId} not found");
                }

                upazila.VDPMemberCount = memberCount;
                upazila.HasVDPUnit = memberCount > 0;
                upazila.UpdatedAt = DateTime.UtcNow;
                upazila.UpdatedBy = "System";

                _unitOfWork.Upazilas.Update(upazila);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated VDP member count for upazila {UpazilaId} to {Count}", upazilaId, memberCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating VDP member count for upazila {UpazilaId}", upazilaId);
                throw new InvalidOperationException("Failed to update VDP member count", ex);
            }
        }

        public async Task<UpazilaStatisticsDto> GetUpazilaStatisticsAsync(int upazilaId)
        {
            try
            {
                var upazila = await GetUpazilaByIdAsync(upazilaId);
                if (upazila == null)
                {
                    throw new InvalidOperationException($"Upazila with ID {upazilaId} not found");
                }

                var stores = await _unitOfWork.Stores.FindAsync(s => s.UpazilaId == upazilaId);
                var issues = await _unitOfWork.Issues.FindAsync(i => i.IssuedToUpazilaId == upazilaId);
                var receives = await _unitOfWork.Receives.FindAsync(r => r.ReceivedFromUpazilaId == upazilaId);

                // Calculate monthly activities for the last 6 months
                var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
                var monthlyActivities = new List<MonthlyActivityDto>();

                for (int i = 5; i >= 0; i--)
                {
                    var monthStart = DateTime.UtcNow.AddMonths(-i).Date;
                    var monthEnd = monthStart.AddMonths(1);
                    var monthName = monthStart.ToString("MMM yyyy");

                    var monthIssues = issues.Where(i => i.IssueDate >= monthStart && i.IssueDate < monthEnd).Count();
                    var monthReceives = receives.Where(r => r.ReceiveDate >= monthStart && r.ReceiveDate < monthEnd).Count();

                    monthlyActivities.Add(new MonthlyActivityDto
                    {
                        Month = monthName,
                        IssueCount = monthIssues,
                        ReceiveCount = monthReceives
                    });
                }

                return new UpazilaStatisticsDto
                {
                    TotalStores = stores.Count(),
                    TotalUnions = upazila.NumberOfUnions ?? 0,
                    TotalVillages = upazila.NumberOfVillages ?? 0,
                    VDPMembers = upazila.VDPMemberCount ?? 0,
                    TotalIssues = issues.Count(),
                    TotalReceives = receives.Count(),
                    MonthlyActivities = monthlyActivities
                };
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for upazila {Id}", upazilaId);
                throw new InvalidOperationException("Failed to get sub-district statistics", ex);
            }
        }

        public async Task<byte[]> ExportUpazilasAsync()
        {
            try
            {
                var upazilas = await GetAllUpazilasAsync();

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                // Write custom headers
                csv.WriteField("Code");
                csv.WriteField("Name");
                csv.WriteField("NameBangla");
                csv.WriteField("ZilaId");
                csv.WriteField("ZilaName");
                csv.WriteField("RangeName");
                csv.WriteField("UpazilaOfficerName");
                csv.WriteField("OfficerDesignation");
                csv.WriteField("ContactNumber");
                csv.WriteField("Email");
                csv.WriteField("OfficeAddress");
                csv.WriteField("Area");
                csv.WriteField("Population");
                csv.WriteField("NumberOfUnions");
                csv.WriteField("NumberOfVillages");
                csv.WriteField("HasVDPUnit");
                csv.WriteField("VDPMemberCount");
                csv.WriteField("UpazilaChairmanName");
                csv.WriteField("VDPOfficerName");
                csv.WriteField("StoreCount");
                csv.WriteField("IsActive");
                csv.WriteField("CreatedAt");
                csv.WriteField("CreatedBy");
                csv.NextRecord();

                // Write data
                foreach (var upazila in upazilas)
                {
                    csv.WriteField(upazila.Code);
                    csv.WriteField(upazila.Name);
                    csv.WriteField(upazila.NameBangla);
                    csv.WriteField(upazila.ZilaId);
                    csv.WriteField(upazila.ZilaName);
                    csv.WriteField(upazila.RangeName);
                    csv.WriteField(upazila.UpazilaOfficerName);
                    csv.WriteField(upazila.OfficerDesignation);
                    csv.WriteField(upazila.ContactNumber);
                    csv.WriteField(upazila.Email);
                    csv.WriteField(upazila.OfficeAddress);
                    csv.WriteField(upazila.Area);
                    csv.WriteField(upazila.Population);
                    csv.WriteField(upazila.NumberOfUnions);
                    csv.WriteField(upazila.NumberOfVillages);
                    csv.WriteField(upazila.HasVDPUnit);
                    csv.WriteField(upazila.VDPMemberCount);
                    csv.WriteField(upazila.UpazilaChairmanName);
                    csv.WriteField(upazila.VDPOfficerName);
                    csv.WriteField(upazila.StoreCount);
                    csv.WriteField(upazila.IsActive);
                    csv.WriteField(upazila.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    csv.WriteField(upazila.CreatedBy);
                    csv.NextRecord();
                }

                writer.Flush();
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting upazilas to CSV");
                throw new InvalidOperationException("Failed to export sub-districts to CSV", ex);
            }
        }

        #region Private Helper Methods

        private void ValidateUpazilaDto(UpazilaDto upazilaDto, bool isUpdate)
        {
            if (upazilaDto == null)
            {
                throw new ArgumentNullException(nameof(upazilaDto), "Sub-district information is required");
            }

            if (string.IsNullOrWhiteSpace(upazilaDto.Name))
            {
                throw new ArgumentException("Sub-district name is required and cannot be empty", nameof(upazilaDto.Name));
            }

            if (upazilaDto.Name.Trim().Length > 100)
            {
                throw new ArgumentException("Sub-district name cannot exceed 100 characters", nameof(upazilaDto.Name));
            }

            if (string.IsNullOrWhiteSpace(upazilaDto.Code))
            {
                throw new ArgumentException("Sub-district code is required and cannot be empty", nameof(upazilaDto.Code));
            }

            if (upazilaDto.Code.Trim().Length > 20)
            {
                throw new ArgumentException("Sub-district code cannot exceed 20 characters", nameof(upazilaDto.Code));
            }

            if (upazilaDto.ZilaId <= 0)
            {
                throw new ArgumentException("District selection is required", nameof(upazilaDto.ZilaId));
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(upazilaDto.Email))
            {
                try
                {
                    var email = new System.Net.Mail.MailAddress(upazilaDto.Email);
                }
                catch
                {
                    throw new ArgumentException("Invalid email address format", nameof(upazilaDto.Email));
                }
            }

            // Validate area
            if (upazilaDto.Area.HasValue && upazilaDto.Area.Value < 0)
            {
                throw new ArgumentException("Area must be a positive number", nameof(upazilaDto.Area));
            }

            // Validate population
            if (upazilaDto.Population.HasValue && upazilaDto.Population.Value < 0)
            {
                throw new ArgumentException("Population must be a positive number", nameof(upazilaDto.Population));
            }

            // Validate VDP member count
            if (upazilaDto.VDPMemberCount.HasValue && upazilaDto.VDPMemberCount.Value < 0)
            {
                throw new ArgumentException("VDP member count must be a positive number", nameof(upazilaDto.VDPMemberCount));
            }

            // Validate unions and villages
            if (upazilaDto.NumberOfUnions.HasValue && upazilaDto.NumberOfUnions.Value < 0)
            {
                throw new ArgumentException("Number of unions must be a positive number", nameof(upazilaDto.NumberOfUnions));
            }

            if (upazilaDto.NumberOfVillages.HasValue && upazilaDto.NumberOfVillages.Value < 0)
            {
                throw new ArgumentException("Number of villages must be a positive number", nameof(upazilaDto.NumberOfVillages));
            }
        }

        private UpazilaDto MapToDto(Upazila upazila)
        {
            if (upazila == null)
            {
                return null;
            }

            return new UpazilaDto
            {
                Id = upazila.Id,
                Name = upazila.Name ?? string.Empty,
                Code = upazila.Code ?? string.Empty,
                NameBangla = upazila.NameBangla ?? string.Empty,
                ZilaId = upazila.ZilaId,
                UpazilaOfficerName = upazila.UpazilaOfficerName ?? string.Empty,
                OfficerDesignation = upazila.OfficerDesignation ?? string.Empty,
                ContactNumber = upazila.ContactNumber ?? string.Empty,
                Email = upazila.Email ?? string.Empty,
                OfficeAddress = upazila.OfficeAddress ?? string.Empty,
                Area = upazila.Area,
                Population = upazila.Population,
                NumberOfUnions = upazila.NumberOfUnions,
                NumberOfVillages = upazila.NumberOfVillages,
                HasVDPUnit = upazila.HasVDPUnit,
                VDPMemberCount = upazila.VDPMemberCount,
                UpazilaChairmanName = upazila.UpazilaChairmanName ?? string.Empty,
                VDPOfficerName = upazila.VDPOfficerName ?? string.Empty,
                IsActive = upazila.IsActive,
                Remarks = upazila.Remarks ?? string.Empty,
                CreatedAt = upazila.CreatedAt,
                CreatedBy = upazila.CreatedBy ?? string.Empty,
                UpdatedAt = upazila.UpdatedAt,
                UpdatedBy = upazila.UpdatedBy ?? string.Empty
            };
        }

        public Task<ImportResultDto> ImportUpazilasFromCsvAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}