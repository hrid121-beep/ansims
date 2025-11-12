using CsvHelper;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Range = IMS.Domain.Entities.Range;

namespace IMS.Application.Services
{
    public class RangeService : IRangeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RangeService> _logger;
        private readonly IActivityLogService _activityLogService;
        private readonly UserManager<User> _userManager;

        public RangeService(
            IUnitOfWork unitOfWork,
            ILogger<RangeService> logger,
            IActivityLogService activityLogService,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activityLogService = activityLogService ?? throw new ArgumentNullException(nameof(activityLogService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IEnumerable<RangeDto>> GetAllRangesAsync()
        {
            try
            {
                var ranges = await _unitOfWork.Ranges.GetAllWithIncludesAsync(
                    r => r.Battalions,
                    r => r.Zilas,
                    r => r.Stores
                );

                var rangeDtos = new List<RangeDto>();

                foreach (var range in ranges)
                {
                    var dto = MapToDto(range);
                    rangeDtos.Add(dto);
                }

                return rangeDtos.OrderBy(r => r.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all ranges");
                throw new InvalidOperationException("Failed to retrieve ranges", ex);
            }
        }

        public async Task<IEnumerable<RangeDto>> GetActiveRangesAsync()
        {
            try
            {
                var ranges = await _unitOfWork.Ranges.FindAsync(r => r.IsActive);
                var dtos = new List<RangeDto>();

                foreach (var range in ranges)
                {
                    dtos.Add(MapToDto(range));
                }

                return dtos.OrderBy(r => r.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active ranges");
                throw new InvalidOperationException("Failed to retrieve active ranges", ex);
            }
        }

        public async Task<RangeDto> GetRangeByIdAsync(int id)
        {
            try
            {
                var range = await _unitOfWork.Ranges.GetByIdWithIncludesAsync(
                    id,
                    r => r.Battalions,
                    r => r.Zilas,
                    r => r.Stores
                );

                if (range == null)
                {
                    _logger.LogWarning("Range with ID {Id} not found", id);
                    return null;
                }

                var dto = MapToDto(range);

                // Populate user names
                await PopulateUserNamesAsync(dto);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving range with ID {Id}", id);
                throw new InvalidOperationException($"Failed to retrieve range with ID {id}", ex);
            }
        }

        public async Task<RangeDto> GetRangeByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new ArgumentException("Range code cannot be empty", nameof(code));
                }

                var range = await _unitOfWork.Ranges.FirstOrDefaultAsync(r => r.Code == code);
                return range != null ? MapToDto(range) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving range with code {Code}", code);
                throw new InvalidOperationException($"Failed to retrieve range with code {code}", ex);
            }
        }

        public async Task<RangeDto> CreateRangeAsync(RangeDto rangeDto)
        {
            try
            {
                // Validate unique name and code
                if (await RangeExistsAsync(rangeDto.Name))
                {
                    throw new InvalidOperationException($"Range with name '{rangeDto.Name}' already exists");
                }

                if (string.IsNullOrWhiteSpace(rangeDto.Code))
                {
                    rangeDto.Code = await GenerateRangeCodeAsync();
                }
                else if (await RangeCodeExistsAsync(rangeDto.Code))
                {
                    throw new InvalidOperationException($"Range with code '{rangeDto.Code}' already exists");
                }

                var range = new Range
                {
                    Name = rangeDto.Name,
                    Code = rangeDto.Code,
                    HeadquarterLocation = rangeDto.HeadquarterLocation,
                    CommanderName = rangeDto.CommanderName,
                    CommanderRank = rangeDto.CommanderRank,
                    ContactNumber = rangeDto.ContactNumber,
                    Email = rangeDto.Email,
                    CoverageArea = rangeDto.CoverageArea,
                    IsActive = rangeDto.IsActive,
                    Remarks = rangeDto.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = rangeDto.CreatedBy ?? "System"
                };

                await _unitOfWork.Ranges.AddAsync(range);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Range",
                    range.Id,
                    "Create",
                    $"Created range: {range.Name} ({range.Code})",
                    rangeDto.CreatedBy ?? "System"
                );

                return MapToDto(range);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating range");
                throw;
            }
        }

        public async Task UpdateRangeAsync(RangeDto rangeDto)
        {
            try
            {
                var range = await _unitOfWork.Ranges.GetByIdAsync(rangeDto.Id);
                if (range == null)
                {
                    throw new InvalidOperationException($"Range with ID {rangeDto.Id} not found");
                }

                // Validate unique name and code
                if (await RangeExistsAsync(rangeDto.Name, rangeDto.Id))
                {
                    throw new InvalidOperationException($"Range with name '{rangeDto.Name}' already exists");
                }

                if (await RangeCodeExistsAsync(rangeDto.Code, rangeDto.Id))
                {
                    throw new InvalidOperationException($"Range with code '{rangeDto.Code}' already exists");
                }

                // Update properties
                range.Name = rangeDto.Name;
                range.Code = rangeDto.Code;
                range.HeadquarterLocation = rangeDto.HeadquarterLocation;
                range.CommanderName = rangeDto.CommanderName;
                range.CommanderRank = rangeDto.CommanderRank;
                range.ContactNumber = rangeDto.ContactNumber;
                range.Email = rangeDto.Email;
                range.CoverageArea = rangeDto.CoverageArea;
                range.IsActive = rangeDto.IsActive;
                range.Remarks = rangeDto.Remarks;
                range.UpdatedAt = DateTime.UtcNow;
                range.UpdatedBy = rangeDto.UpdatedBy ?? rangeDto.CreatedBy ?? "System";

                _unitOfWork.Ranges.Update(range);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Range",
                    range.Id,
                    "Update",
                    $"Updated range: {range.Name} ({range.Code})",
                    range.UpdatedBy
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range with ID {Id}", rangeDto.Id);
                throw;
            }
        }

        public async Task DeleteRangeAsync(int id)
        {
            try
            {
                var range = await _unitOfWork.Ranges.GetByIdAsync(id);
                if (range == null)
                {
                    throw new InvalidOperationException($"Range with ID {id} not found");
                }

                // Check dependencies
                var battalionCount = await GetRangeBattalionCountAsync(id);
                if (battalionCount > 0)
                {
                    throw new InvalidOperationException($"Cannot delete range with {battalionCount} assigned battalions");
                }

                var zilaCount = await GetRangeZilaCountAsync(id);
                if (zilaCount > 0)
                {
                    throw new InvalidOperationException($"Cannot delete range with {zilaCount} assigned districts");
                }

                var storeCount = await GetRangeStoreCountAsync(id);
                if (storeCount > 0)
                {
                    throw new InvalidOperationException($"Cannot delete range with {storeCount} assigned stores");
                }

                _unitOfWork.Ranges.Remove(range);
                await _unitOfWork.CompleteAsync();

                // Log activity
                await _activityLogService.LogActivityAsync(
                    "Range",
                    range.Id,
                    "Delete",
                    $"Deleted range: {range.Name} ({range.Code})",
                    "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> RangeExistsAsync(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return await _unitOfWork.Ranges.ExistsAsync(
                r => r.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || r.Id != excludeId.Value)
            );
        }

        public async Task<bool> RangeCodeExistsAsync(string code, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            return await _unitOfWork.Ranges.ExistsAsync(
                r => r.Code.ToLower() == code.ToLower() && (!excludeId.HasValue || r.Id != excludeId.Value)
            );
        }

        public async Task<int> GetRangeBattalionCountAsync(int rangeId)
        {
            try
            {
                return await _unitOfWork.Battalions.CountAsync(b => b.RangeId == rangeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting battalion count for range {Id}", rangeId);
                return 0;
            }
        }

        public async Task<int> GetRangeZilaCountAsync(int rangeId)
        {
            try
            {
                return await _unitOfWork.Zilas.CountAsync(z => z.RangeId == rangeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting zila count for range {Id}", rangeId);
                return 0;
            }
        }

        public async Task<int> GetRangeStoreCountAsync(int rangeId)
        {
            try
            {
                return await _unitOfWork.Stores.CountAsync(s => s.RangeId == rangeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store count for range {Id}", rangeId);
                return 0;
            }
        }

        public async Task<IEnumerable<BattalionDto>> GetRangeBattalionsAsync(int rangeId)
        {
            try
            {
                var battalions = await _unitOfWork.Battalions.FindAsync(b => b.RangeId == rangeId);
                return battalions.Select(b => new BattalionDto
                {
                    Id = b.Id,
                    Name = b.Name ?? string.Empty,
                    Code = b.Code ?? string.Empty,
                    Type = b.Type,
                    CommanderName = b.CommanderName ?? string.Empty,
                    ContactNumber = b.ContactNumber ?? string.Empty,
                    IsActive = b.IsActive,
                    PersonnelCount = b.TotalPersonnel
                }).OrderBy(b => b.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting battalions for range {Id}", rangeId);
                throw new InvalidOperationException($"Failed to retrieve battalions for range {rangeId}", ex);
            }
        }

        public async Task<IEnumerable<ZilaDto>> GetRangeZilasAsync(int rangeId)
        {
            try
            {
                var zilas = await _unitOfWork.Zilas.FindAsync(z => z.RangeId == rangeId);
                return zilas.Select(z => new ZilaDto
                {
                    Id = z.Id,
                    Name = z.Name ?? string.Empty,
                    Code = z.Code ?? string.Empty,
                    NameBangla = z.NameBangla ?? string.Empty,
                    Division = z.Division ?? string.Empty,
                    IsActive = z.IsActive
                }).OrderBy(z => z.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting zilas for range {Id}", rangeId);
                throw new InvalidOperationException($"Failed to retrieve zilas for range {rangeId}", ex);
            }
        }

        public async Task<RangeHierarchyDto> GetRangeHierarchyAsync(int rangeId)
        {
            try
            {
                var range = await GetRangeByIdAsync(rangeId);
                if (range == null)
                {
                    return null;
                }

                var battalions = await GetRangeBattalionsAsync(rangeId);
                var zilas = await GetRangeZilasAsync(rangeId);
                var statistics = await GetRangeStatisticsAsync(rangeId);

                return new RangeHierarchyDto
                {
                    Range = range,
                    Battalions = battalions,
                    Zilas = zilas,
                    Statistics = statistics
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hierarchy for range {Id}", rangeId);
                throw new InvalidOperationException($"Failed to retrieve hierarchy for range {rangeId}", ex);
            }
        }

        public async Task<string> GenerateRangeCodeAsync()
        {
            try
            {
                // Get all existing range codes
                var existingRanges = await _unitOfWork.Ranges.GetAllAsync();
                var existingCodes = existingRanges.Select(r => r.Code?.ToUpper()).Where(c => !string.IsNullOrEmpty(c)).ToList();

                // Default divisions/major areas in Bangladesh for Range codes
                var defaultAreas = new List<string>
                {
                    "DH", "CT", "KH", "RJ", "BS", "SY", "RG", "MY", "CM", "FN", "JH", "MD", "NK", "PT", "GP"
                };

                // Find first available area code
                foreach (var area in defaultAreas)
                {
                    var code = $"{area}-R";
                    if (!existingCodes.Contains(code))
                    {
                        return code;
                    }
                }

                // If all default areas are taken, generate with numbers
                var baseCode = "RNG-R";
                var counter = 1;

                while (existingCodes.Contains($"{baseCode}{counter:D2}"))
                {
                    counter++;
                    if (counter > 99) // Safety check
                    {
                        baseCode = $"R{DateTime.Now:yyMMdd}-";
                        counter = 1;
                    }
                }

                return $"{baseCode}{counter:D2}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating range code");
                // Fallback to timestamp-based code
                return $"R{DateTime.Now:yyMMddHHmm}";
            }
        }
        public async Task<string> GenerateRangeCodeFromNameAsync(string rangeName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rangeName))
                {
                    return await GenerateRangeCodeAsync();
                }

                // Extract area code from range name
                var cleanName = rangeName.Trim().ToUpper()
                    .Replace("RANGE", "")
                    .Replace("CANTONMENT", "")
                    .Replace("DIVISION", "")
                    .Trim();

                // Take first 2-3 characters based on name length
                string areaCode;
                if (cleanName.Length >= 3)
                {
                    areaCode = cleanName.Substring(0, Math.Min(3, cleanName.Length));
                }
                else
                {
                    areaCode = cleanName.PadRight(2, 'X');
                }

                // Remove vowels if more than 2 characters to make it 2 chars
                if (areaCode.Length > 2)
                {
                    var consonants = areaCode.Where(c => !"AEIOU".Contains(c)).ToArray();
                    if (consonants.Length >= 2)
                    {
                        areaCode = new string(consonants.Take(2).ToArray());
                    }
                    else
                    {
                        areaCode = areaCode.Substring(0, 2);
                    }
                }

                var baseCode = $"{areaCode}-R";

                // Check if this code already exists
                var existingRanges = await _unitOfWork.Ranges.GetAllAsync();
                var existingCodes = existingRanges.Select(r => r.Code?.ToUpper()).Where(c => !string.IsNullOrEmpty(c)).ToList();

                if (!existingCodes.Contains(baseCode))
                {
                    return baseCode;
                }

                // If exists, try with numbers
                var counter = 1;
                while (existingCodes.Contains($"{baseCode}{counter}") && counter <= 99)
                {
                    counter++;
                }

                return counter <= 99 ? $"{baseCode}{counter}" : await GenerateRangeCodeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating range code from name: {Name}", rangeName);
                return await GenerateRangeCodeAsync();
            }
        }


        public async Task<IEnumerable<RangeDto>> SearchRangesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllRangesAsync();
                }

                var ranges = await _unitOfWork.Ranges.FindAsync(
                    r => r.Name.Contains(searchTerm) ||
                         r.Code.Contains(searchTerm) ||
                         r.CommanderName.Contains(searchTerm) ||
                         r.HeadquarterLocation.Contains(searchTerm)
                );

                var dtos = new List<RangeDto>();
                foreach (var range in ranges)
                {
                    dtos.Add(MapToDto(range));
                }

                return dtos.OrderBy(r => r.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching ranges with term '{SearchTerm}'", searchTerm);
                throw new InvalidOperationException("Failed to search ranges", ex);
            }
        }

        public async Task<RangeStatisticsDto> GetRangeStatisticsAsync(int rangeId)
        {
            try
            {
                var battalions = await _unitOfWork.Battalions.FindAsync(b => b.RangeId == rangeId);
                var battalionList = battalions.ToList();

                return new RangeStatisticsDto
                {
                    TotalBattalions = battalionList.Count,
                    MaleBattalions = battalionList.Count(b => b.Type == BattalionType.Male),
                    FemaleBattalions = battalionList.Count(b => b.Type == BattalionType.Female),
                    TotalZilas = await GetRangeZilaCountAsync(rangeId),
                    TotalStores = await GetRangeStoreCountAsync(rangeId),
                    TotalPersonnel = battalionList.Sum(b => b.TotalPersonnel),
                    ActiveBattalions = battalionList.Count(b => b.IsActive),
                    InactiveBattalions = battalionList.Count(b => !b.IsActive)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for range {Id}", rangeId);
                throw new InvalidOperationException($"Failed to get range statistics", ex);
            }
        }

        public async Task<Dictionary<string, object>> GetRangeDashboardDataAsync(int rangeId)
        {
            try
            {
                var range = await _unitOfWork.Ranges.GetByIdAsync(rangeId);
                if (range == null)
                {
                    throw new InvalidOperationException($"Range with ID {rangeId} not found");
                }

                var stats = await GetRangeStatisticsAsync(rangeId);

                return new Dictionary<string, object>
                {
                    ["RangeName"] = range.Name,
                    ["RangeCode"] = range.Code,
                    ["Statistics"] = stats,
                    ["Commander"] = range.CommanderName ?? "Not Assigned",
                    ["Headquarter"] = range.HeadquarterLocation ?? "Not Set",
                    ["IsActive"] = range.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard data for range {Id}", rangeId);
                throw new InvalidOperationException("Failed to get range dashboard data", ex);
            }
        }

        private RangeDto MapToDto(Range range)
        {
            if (range == null)
            {
                return null;
            }

            return new RangeDto
            {
                Id = range.Id,
                Name = range.Name ?? string.Empty,
                Code = range.Code ?? string.Empty,
                HeadquarterLocation = range.HeadquarterLocation ?? string.Empty,
                CommanderName = range.CommanderName ?? string.Empty,
                CommanderRank = range.CommanderRank ?? string.Empty,
                ContactNumber = range.ContactNumber ?? string.Empty,
                Email = range.Email ?? string.Empty,
                CoverageArea = range.CoverageArea ?? string.Empty,
                IsActive = range.IsActive,
                Remarks = range.Remarks ?? string.Empty,
                BattalionCount = range.Battalions?.Count ?? 0,
                ZilaCount = range.Zilas?.Count ?? 0,
                StoreCount = range.Stores?.Count ?? 0,
                BattalionNames = range.Battalions?.Select(b => b.Name).Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>(),
                CreatedAt = range.CreatedAt,
                CreatedBy = range.CreatedBy ?? string.Empty,
                UpdatedAt = range.UpdatedAt,
                UpdatedBy = range.UpdatedBy ?? string.Empty
            };
        }

        private async Task PopulateUserNamesAsync(RangeDto dto)
        {
            if (dto == null) return;

            // Get Created By user name
            if (!string.IsNullOrEmpty(dto.CreatedBy))
            {
                var createdByUser = await _userManager.FindByIdAsync(dto.CreatedBy);
                dto.CreatedByName = createdByUser?.UserName ?? dto.CreatedBy;
            }

            // Get Updated By user name
            if (!string.IsNullOrEmpty(dto.UpdatedBy))
            {
                var updatedByUser = await _userManager.FindByIdAsync(dto.UpdatedBy);
                dto.UpdatedByName = updatedByUser?.UserName ?? dto.UpdatedBy;
            }
        }
    }
}