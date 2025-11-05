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
    public class ZilaService : IZilaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ZilaService> _logger;
        private readonly IActivityLogService _activityLogService;

        public ZilaService(
            IUnitOfWork unitOfWork,
            ILogger<ZilaService> logger,
            IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
        }

        public async Task<IEnumerable<ZilaDto>> GetAllZilasAsync()
        {
            try
            {
                var zilas = await _unitOfWork.Zilas.GetAllAsync();
                var zilaDtos = new List<ZilaDto>();

                foreach (var zila in zilas)
                {
                    var dto = MapToDto(zila);
                    dto.UpazilaCount = await GetZilaUpazilaCountAsync(zila.Id);
                    dto.StoreCount = await GetZilaStoreCountAsync(zila.Id);
                    dto.VDPMemberCount = await GetZilaVDPMemberCountAsync(zila.Id);

                    if (zila.RangeId > 0)
                    {
                        var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                        dto.RangeName = range?.Name;
                    }

                    zilaDtos.Add(dto);
                }

                return zilaDtos.OrderBy(z => z.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all zilas");
                throw new InvalidOperationException("Failed to retrieve districts", ex);
            }
        }

        public async Task<IEnumerable<ZilaDto>> GetActiveZilasAsync()
        {
            try
            {
                var zilas = await _unitOfWork.Zilas.FindAsync(z => z.IsActive);
                var dtos = new List<ZilaDto>();

                foreach (var zila in zilas)
                {
                    var dto = MapToDto(zila);
                    if (zila.RangeId > 0)
                    {
                        var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                        dto.RangeName = range?.Name;
                    }
                    dtos.Add(dto);
                }

                return dtos.OrderBy(z => z.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active zilas");
                throw new InvalidOperationException("Failed to retrieve active districts", ex);
            }
        }

        public async Task<IEnumerable<ZilaDto>> GetZilasByRangeAsync(int rangeId)
        {
            try
            {
                var zilas = await _unitOfWork.Zilas.FindAsync(z => z.RangeId == rangeId);
                var dtos = new List<ZilaDto>();

                foreach (var zila in zilas)
                {
                    var dto = MapToDto(zila);
                    if (zila.RangeId > 0)
                    {
                        var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                        dto.RangeName = range?.Name;
                    }
                    dtos.Add(dto);
                }

                return dtos.OrderBy(z => z.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving zilas for range {RangeId}", rangeId);
                throw new InvalidOperationException($"Failed to retrieve districts for range {rangeId}", ex);
            }
        }

        public async Task<IEnumerable<ZilaDto>> GetZilasByDivisionAsync(string division)
        {
            try
            {
                var zilas = await _unitOfWork.Zilas.FindAsync(z => z.Division == division);
                var dtos = new List<ZilaDto>();

                foreach (var zila in zilas)
                {
                    var dto = MapToDto(zila);
                    dto.UpazilaCount = await GetZilaUpazilaCountAsync(zila.Id);
                    dto.StoreCount = await GetZilaStoreCountAsync(zila.Id);
                    dto.VDPMemberCount = await GetZilaVDPMemberCountAsync(zila.Id);

                    if (zila.RangeId > 0)
                    {
                        var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                        dto.RangeName = range?.Name;
                    }

                    dtos.Add(dto);
                }

                return dtos.OrderBy(z => z.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving zilas for division {Division}", division);
                throw new InvalidOperationException($"Failed to retrieve districts for division {division}", ex);
            }
        }

        public async Task<ZilaDto> GetZilaByIdAsync(int id)
        {
            try
            {
                var zila = await _unitOfWork.Zilas.GetByIdAsync(id);
                if (zila == null)
                {
                    _logger.LogWarning("Zila with ID {Id} not found", id);
                    return null;
                }

                var dto = MapToDto(zila);
                dto.UpazilaCount = await GetZilaUpazilaCountAsync(zila.Id);
                dto.StoreCount = await GetZilaStoreCountAsync(zila.Id);
                dto.VDPMemberCount = await GetZilaVDPMemberCountAsync(zila.Id);

                if (zila.RangeId > 0)
                {
                    var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                    dto.RangeName = range?.Name;
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving zila with ID {Id}", id);
                throw new InvalidOperationException($"Failed to retrieve district with ID {id}", ex);
            }
        }

        public async Task<ZilaDto> GetZilaByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new ArgumentException("District code cannot be empty", nameof(code));
                }

                var zila = await _unitOfWork.Zilas.FirstOrDefaultAsync(z => z.Code == code);
                if (zila == null)
                {
                    return null;
                }

                var dto = MapToDto(zila);
                if (zila.RangeId > 0)
                {
                    var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                    dto.RangeName = range?.Name;
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving zila with code {Code}", code);
                throw new InvalidOperationException($"Failed to retrieve district with code {code}", ex);
            }
        }

        public async Task<ZilaDto> CreateZilaAsync(ZilaDto zilaDto)
        {
            try
            {
                // Validate required fields
                ValidateZilaDto(zilaDto, isUpdate: false);

                // Validate unique name and code
                if (await ZilaExistsAsync(zilaDto.Name))
                {
                    throw new InvalidOperationException($"Zila with name '{zilaDto.Name}' already exists");
                }

                if (await ZilaCodeExistsAsync(zilaDto.Code))
                {
                    throw new InvalidOperationException($"Zila with code '{zilaDto.Code}' already exists");
                }

                // Validate Range exists if provided
                if (zilaDto.RangeId > 0)
                {
                    var range = await _unitOfWork.Ranges.GetByIdAsync(zilaDto.RangeId);
                    if (range == null)
                    {
                        throw new InvalidOperationException($"Range with ID {zilaDto.RangeId} not found");
                    }
                }

                var zila = new Zila
                {
                    Name = zilaDto.Name?.Trim(),
                    Code = zilaDto.Code?.Trim(),
                    NameBangla = zilaDto.NameBangla?.Trim(),
                    RangeId = zilaDto.RangeId,
                    Division = zilaDto.Division?.Trim(),
                    DistrictOfficerName = zilaDto.DistrictOfficerName?.Trim(),
                    ContactNumber = zilaDto.ContactNumber?.Trim(),
                    Email = zilaDto.Email?.Trim(),
                    OfficeAddress = zilaDto.OfficeAddress?.Trim(),
                    Area = zilaDto.Area,
                    Population = zilaDto.Population,
                    IsActive = zilaDto.IsActive,
                    Remarks = zilaDto.Remarks?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = zilaDto.CreatedBy ?? "System"
                };

                await _unitOfWork.Zilas.AddAsync(zila);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully created zila: {Name} ({Code}) by {User}",
                    zila.Name, zila.Code, zilaDto.CreatedBy ?? "System");

                // Log activity (non-blocking - don't fail the operation if this fails)
                try
                {
                    await _activityLogService.LogActivityAsync(
                        "Zila",
                        zila.Id,
                        "Create",
                        $"Created zila: {zila.Name} ({zila.Code})",
                        zilaDto.CreatedBy ?? "System"
                    );
                }
                catch (Exception logEx)
                {
                    _logger.LogWarning(logEx, "Failed to log activity for zila creation: {Name}", zila.Name);
                    // Don't throw - activity logging failure shouldn't fail the operation
                }

                return MapToDto(zila);
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
                _logger.LogError(ex, "Unexpected error creating zila: {Name}", zilaDto?.Name);
                throw new InvalidOperationException("An unexpected error occurred while creating the district. Please try again.", ex);
            }
        }

        public async Task UpdateZilaAsync(ZilaDto zilaDto)
        {
            try
            {
                // Validate required fields
                ValidateZilaDto(zilaDto, isUpdate: true);

                var zila = await _unitOfWork.Zilas.GetByIdAsync(zilaDto.Id);
                if (zila == null)
                {
                    throw new InvalidOperationException($"Zila with ID {zilaDto.Id} not found");
                }

                // Store original values for logging
                var originalName = zila.Name;
                var originalCode = zila.Code;

                // Validate unique name and code
                if (await ZilaExistsAsync(zilaDto.Name, zilaDto.Id))
                {
                    throw new InvalidOperationException($"Zila with name '{zilaDto.Name}' already exists");
                }

                if (await ZilaCodeExistsAsync(zilaDto.Code, zilaDto.Id))
                {
                    throw new InvalidOperationException($"Zila with code '{zilaDto.Code}' already exists");
                }

                // Validate Range exists if provided
                if (zilaDto.RangeId > 0)
                {
                    var range = await _unitOfWork.Ranges.GetByIdAsync(zilaDto.RangeId);
                    if (range == null)
                    {
                        throw new InvalidOperationException($"Range with ID {zilaDto.RangeId} not found");
                    }
                }

                // Update properties
                zila.Name = zilaDto.Name?.Trim();
                zila.Code = zilaDto.Code?.Trim();
                zila.NameBangla = zilaDto.NameBangla?.Trim();
                zila.RangeId = zilaDto.RangeId;
                zila.Division = zilaDto.Division?.Trim();
                zila.DistrictOfficerName = zilaDto.DistrictOfficerName?.Trim();
                zila.ContactNumber = zilaDto.ContactNumber?.Trim();
                zila.Email = zilaDto.Email?.Trim();
                zila.OfficeAddress = zilaDto.OfficeAddress?.Trim();
                zila.Area = zilaDto.Area;
                zila.Population = zilaDto.Population;
                zila.IsActive = zilaDto.IsActive;
                zila.Remarks = zilaDto.Remarks?.Trim();
                zila.UpdatedAt = DateTime.UtcNow;
                zila.UpdatedBy = zilaDto.UpdatedBy ?? zilaDto.CreatedBy ?? "System";

                _unitOfWork.Zilas.Update(zila);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully updated zila: {Name} ({Code}) by {User}",
                    zila.Name, zila.Code, zila.UpdatedBy);

                // Log activity (non-blocking - don't fail the operation if this fails)
                try
                {
                    await _activityLogService.LogActivityAsync(
                        "Zila",
                        zila.Id,
                        "Update",
                        $"Updated zila: {zila.Name} ({zila.Code})",
                        zila.UpdatedBy
                    );
                }
                catch (Exception logEx)
                {
                    _logger.LogWarning(logEx, "Failed to log activity for zila update: {Name}", zila.Name);
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
                _logger.LogError(ex, "Unexpected error updating zila with ID {Id}", zilaDto?.Id);
                throw new InvalidOperationException("An unexpected error occurred while updating the district. Please try again.", ex);
            }
        }

        public async Task DeleteZilaAsync(int id)
        {
            try
            {
                var zila = await _unitOfWork.Zilas.GetByIdAsync(id);
                if (zila == null)
                {
                    throw new InvalidOperationException($"Zila with ID {id} not found");
                }

                // Store details for logging
                var zilaName = zila.Name;
                var zilaCode = zila.Code;

                // Check if zila has any dependencies
                var upazilaCount = await GetZilaUpazilaCountAsync(id);
                if (upazilaCount > 0)
                {
                    throw new InvalidOperationException($"Cannot delete zila that has {upazilaCount} upazila{(upazilaCount > 1 ? "s" : "")} assigned");
                }

                var storeCount = await GetZilaStoreCountAsync(id);
                if (storeCount > 0)
                {
                    throw new InvalidOperationException($"Cannot delete zila that has {storeCount} store{(storeCount > 1 ? "s" : "")} assigned");
                }

                _unitOfWork.Zilas.Remove(zila);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Successfully deleted zila: {Name} ({Code})", zilaName, zilaCode);

                // Log activity (non-blocking - don't fail the operation if this fails)
                try
                {
                    await _activityLogService.LogActivityAsync(
                        "Zila",
                        zila.Id,
                        "Delete",
                        $"Deleted zila: {zilaName} ({zilaCode})",
                        "System"
                    );
                }
                catch (Exception logEx)
                {
                    _logger.LogWarning(logEx, "Failed to log activity for zila deletion: {Name}", zilaName);
                    // Don't throw - activity logging failure shouldn't fail the operation
                }
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting zila with ID {Id}", id);
                throw new InvalidOperationException("An unexpected error occurred while deleting the district. Please try again.", ex);
            }
        }

        public async Task<bool> ZilaExistsAsync(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var trimmedName = name.Trim();
            return await _unitOfWork.Zilas.ExistsAsync(
                z => z.Name.ToLower() == trimmedName.ToLower() && (!excludeId.HasValue || z.Id != excludeId.Value)
            );
        }

        public async Task<bool> ZilaCodeExistsAsync(string code, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var trimmedCode = code.Trim();
            return await _unitOfWork.Zilas.ExistsAsync(
                z => z.Code.ToLower() == trimmedCode.ToLower() && (!excludeId.HasValue || z.Id != excludeId.Value)
            );
        }

        public async Task<bool> CheckNameExistsAsync(string name, int? excludeId = null)
        {
            return await ZilaExistsAsync(name, excludeId);
        }

        public async Task<string> GenerateZilaCodeAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("District name is required to generate code", nameof(name));
                }

                var cleanName = name.Trim();

                // Create abbreviation from district name
                var abbreviation = CreateAbbreviation(cleanName);
                var prefix = $"ZILA-{abbreviation}";

                // Check if the basic code exists
                var basicCode = prefix;
                var exists = await ZilaCodeExistsAsync(basicCode);

                if (!exists)
                {
                    _logger.LogInformation("Generated zila code: {Code} for name: {Name}", basicCode, cleanName);
                    return basicCode;
                }

                // If basic code exists, try with numbers
                for (int i = 1; i <= 99; i++)
                {
                    var numberedCode = $"{prefix}-{i:D2}";
                    exists = await ZilaCodeExistsAsync(numberedCode);

                    if (!exists)
                    {
                        _logger.LogInformation("Generated zila code: {Code} for name: {Name}", numberedCode, cleanName);
                        return numberedCode;
                    }
                }

                throw new InvalidOperationException($"Unable to generate unique zila code for name '{cleanName}'. All codes may be exhausted.");
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error generating zila code for name {Name}", name);
                throw new InvalidOperationException($"Failed to generate district code for name '{name}'. Please try again.", ex);
            }
        }

        public async Task<IEnumerable<ZilaDto>> SearchZilasAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllZilasAsync();
                }

                var trimmedSearchTerm = searchTerm.Trim();
                var zilas = await _unitOfWork.Zilas.FindAsync(
                    z => z.Name.Contains(trimmedSearchTerm) ||
                         z.Code.Contains(trimmedSearchTerm) ||
                         z.NameBangla.Contains(trimmedSearchTerm) ||
                         z.Division.Contains(trimmedSearchTerm) ||
                         z.DistrictOfficerName.Contains(trimmedSearchTerm)
                );

                var dtos = new List<ZilaDto>();
                foreach (var zila in zilas)
                {
                    var dto = MapToDto(zila);
                    dto.UpazilaCount = await GetZilaUpazilaCountAsync(zila.Id);
                    dto.StoreCount = await GetZilaStoreCountAsync(zila.Id);
                    dto.VDPMemberCount = await GetZilaVDPMemberCountAsync(zila.Id);

                    if (zila.RangeId > 0)
                    {
                        var range = await _unitOfWork.Ranges.GetByIdAsync(zila.RangeId);
                        dto.RangeName = range?.Name;
                    }

                    dtos.Add(dto);
                }

                return dtos.OrderBy(z => z.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching zilas with term '{SearchTerm}'", searchTerm);
                throw new InvalidOperationException("Failed to search districts", ex);
            }
        }

        public async Task<IEnumerable<UpazilaDto>> GetZilaUpazilasAsync(int zilaId)
        {
            try
            {
                var upazilas = await _unitOfWork.Upazilas.FindAsync(u => u.ZilaId == zilaId);
                return upazilas.Select(u => new UpazilaDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Code = u.Code,
                    NameBangla = u.NameBangla,
                    HasVDPUnit = u.HasVDPUnit,
                    VDPMemberCount = u.VDPMemberCount,
                    IsActive = u.IsActive
                }).OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upazilas for zila {ZilaId}", zilaId);
                return new List<UpazilaDto>();
            }
        }

        public async Task<int> GetZilaUpazilaCountAsync(int zilaId)
        {
            try
            {
                var upazilas = await _unitOfWork.Upazilas.FindAsync(u => u.ZilaId == zilaId);
                return upazilas.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upazila count for zila {ZilaId}", zilaId);
                return 0;
            }
        }

        public async Task<int> GetZilaStoreCountAsync(int zilaId)
        {
            try
            {
                var stores = await _unitOfWork.Stores.FindAsync(s => s.ZilaId == zilaId);
                return stores.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store count for zila {ZilaId}", zilaId);
                return 0;
            }
        }

        public async Task<int> GetZilaVDPMemberCountAsync(int zilaId)
        {
            try
            {
                var upazilas = await _unitOfWork.Upazilas.FindAsync(u => u.ZilaId == zilaId);
                return upazilas.Sum(u => u.VDPMemberCount ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting VDP member count for zila {ZilaId}", zilaId);
                return 0;
            }
        }

        public async Task<Dictionary<string, object>> GetZilaStatisticsAsync(int zilaId)
        {
            try
            {
                var zila = await _unitOfWork.Zilas.GetByIdAsync(zilaId);
                if (zila == null)
                {
                    throw new InvalidOperationException($"Zila with ID {zilaId} not found");
                }

                var stats = new Dictionary<string, object>
                {
                    ["UpazilaCount"] = await GetZilaUpazilaCountAsync(zilaId),
                    ["StoreCount"] = await GetZilaStoreCountAsync(zilaId),
                    ["VDPMemberCount"] = await GetZilaVDPMemberCountAsync(zilaId),
                    ["Area"] = zila.Area?.ToString("N2") ?? "N/A",
                    ["Population"] = zila.Population?.ToString("N0") ?? "N/A",
                    ["Division"] = zila.Division ?? "N/A",
                    ["IsActive"] = zila.IsActive
                };

                return stats;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw business logic exceptions as-is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for zila {Id}", zilaId);
                throw new InvalidOperationException("Failed to get district statistics", ex);
            }
        }

        public async Task<byte[]> ExportZilasAsync()
        {
            try
            {
                var zilas = await GetAllZilasAsync();

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                // Write custom headers
                csv.WriteField("Code");
                csv.WriteField("Name");
                csv.WriteField("NameBangla");
                csv.WriteField("RangeId");
                csv.WriteField("RangeName");
                csv.WriteField("Division");
                csv.WriteField("DistrictOfficerName");
                csv.WriteField("ContactNumber");
                csv.WriteField("Email");
                csv.WriteField("OfficeAddress");
                csv.WriteField("Area");
                csv.WriteField("Population");
                csv.WriteField("UpazilaCount");
                csv.WriteField("StoreCount");
                csv.WriteField("VDPMemberCount");
                csv.WriteField("IsActive");
                csv.WriteField("CreatedAt");
                csv.WriteField("CreatedBy");
                csv.NextRecord();

                // Write data
                foreach (var zila in zilas)
                {
                    csv.WriteField(zila.Code);
                    csv.WriteField(zila.Name);
                    csv.WriteField(zila.NameBangla);
                    csv.WriteField(zila.RangeId);
                    csv.WriteField(zila.RangeName);
                    csv.WriteField(zila.Division);
                    csv.WriteField(zila.DistrictOfficerName);
                    csv.WriteField(zila.ContactNumber);
                    csv.WriteField(zila.Email);
                    csv.WriteField(zila.OfficeAddress);
                    csv.WriteField(zila.Area);
                    csv.WriteField(zila.Population);
                    csv.WriteField(zila.UpazilaCount);
                    csv.WriteField(zila.StoreCount);
                    csv.WriteField(zila.VDPMemberCount);
                    csv.WriteField(zila.IsActive);
                    csv.WriteField(zila.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    csv.WriteField(zila.CreatedBy);
                    csv.NextRecord();
                }

                writer.Flush();
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting zilas to CSV");
                throw new InvalidOperationException("Failed to export districts to CSV", ex);
            }
        }

        #region Private Helper Methods

        private void ValidateZilaDto(ZilaDto zilaDto, bool isUpdate)
        {
            if (zilaDto == null)
            {
                throw new ArgumentNullException(nameof(zilaDto), "District information is required");
            }

            if (string.IsNullOrWhiteSpace(zilaDto.Name))
            {
                throw new ArgumentException("District name is required and cannot be empty", nameof(zilaDto.Name));
            }

            if (zilaDto.Name.Trim().Length > 100)
            {
                throw new ArgumentException("District name cannot exceed 100 characters", nameof(zilaDto.Name));
            }

            if (string.IsNullOrWhiteSpace(zilaDto.Code))
            {
                throw new ArgumentException("District code is required and cannot be empty", nameof(zilaDto.Code));
            }

            if (zilaDto.Code.Trim().Length > 20)
            {
                throw new ArgumentException("District code cannot exceed 20 characters", nameof(zilaDto.Code));
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(zilaDto.Email))
            {
                try
                {
                    var email = new System.Net.Mail.MailAddress(zilaDto.Email);
                }
                catch
                {
                    throw new ArgumentException("Invalid email address format", nameof(zilaDto.Email));
                }
            }

            // Validate area
            if (zilaDto.Area.HasValue && zilaDto.Area.Value < 0)
            {
                throw new ArgumentException("Area must be a positive number", nameof(zilaDto.Area));
            }

            // Validate population
            if (zilaDto.Population.HasValue && zilaDto.Population.Value < 0)
            {
                throw new ArgumentException("Population must be a positive number", nameof(zilaDto.Population));
            }

            // Validate range ID
            if (zilaDto.RangeId <= 0)
            {
                throw new ArgumentException("Range selection is required", nameof(zilaDto.RangeId));
            }
        }

        private string CreateAbbreviation(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "UNK";
            }

            // Clean the name - remove special characters and extra spaces
            var cleanName = Regex.Replace(name.Trim(), @"[^a-zA-Z\s]", "");
            var words = cleanName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                return "UNK";
            }

            if (words.Length == 1)
            {
                // Single word - take first 3 characters
                var word = words[0].ToUpper();
                return word.Length >= 3 ? word.Substring(0, 3) : word;
            }

            // Multiple words - take first letter of each word, max 4 letters
            var abbreviation = string.Join("", words.Select(w => w[0]).Take(4)).ToUpper();

            // Ensure at least 2 characters
            if (abbreviation.Length < 2)
            {
                abbreviation = words[0].Substring(0, Math.Min(3, words[0].Length)).ToUpper();
            }

            return abbreviation;
        }

        private ZilaDto MapToDto(Zila zila)
        {
            if (zila == null)
            {
                return null;
            }

            return new ZilaDto
            {
                Id = zila.Id,
                Name = zila.Name ?? string.Empty,
                Code = zila.Code ?? string.Empty,
                NameBangla = zila.NameBangla ?? string.Empty,
                RangeId = zila.RangeId,
                RangeName = zila.Range?.Name ?? string.Empty,
                Division = zila.Division ?? string.Empty,
                DistrictOfficerName = zila.DistrictOfficerName ?? string.Empty,
                ContactNumber = zila.ContactNumber ?? string.Empty,
                Email = zila.Email ?? string.Empty,
                OfficeAddress = zila.OfficeAddress ?? string.Empty,
                Area = zila.Area,
                Population = zila.Population,
                IsActive = zila.IsActive,
                Remarks = zila.Remarks ?? string.Empty,
                CreatedAt = zila.CreatedAt,
                CreatedBy = zila.CreatedBy ?? string.Empty,
                UpdatedAt = zila.UpdatedAt,
                UpdatedBy = zila.UpdatedBy ?? string.Empty
            };
        }

        public Task<ImportResultDto> ImportZilasFromCsvAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}