using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services
{
    public class UnionService : IUnionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UnionService> _logger;
        private readonly IActivityLogService _activityLogService;

        public UnionService(
            IUnitOfWork unitOfWork,
            ILogger<UnionService> logger,
            IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _activityLogService = activityLogService;
        }

        public async Task<IEnumerable<UnionDto>> GetAllUnionsAsync()
        {
            try
            {
                var unions = await _unitOfWork.Unions.GetAllAsync();
                var unionDtos = new List<UnionDto>();

                foreach (var union in unions)
                {
                    var dto = MapToDto(union);
                    dto.StoreCount = await GetUnionStoreCountAsync(union.Id);
                    dto.TotalVDPMembers = union.TotalVDPMembers;

                    // Get hierarchical names
                    if (union.UpazilaId > 0)
                    {
                        var upazila = await _unitOfWork.Upazilas.GetByIdAsync(union.UpazilaId);
                        if (upazila != null)
                        {
                            dto.UpazilaName = upazila.Name;
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
                        }
                    }

                    unionDtos.Add(dto);
                }

                return unionDtos.OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all unions");
                throw;
            }
        }

        public async Task<IEnumerable<UnionDto>> GetActiveUnionsAsync()
        {
            try
            {
                var unions = await _unitOfWork.Unions.FindAsync(u => u.IsActive);
                return unions.Select(MapToDto).OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active unions");
                throw;
            }
        }

        public async Task<IEnumerable<UnionDto>> GetUnionsByUpazilaAsync(int upazilaId)
        {
            try
            {
                var unions = await _unitOfWork.Unions.FindAsync(u => u.UpazilaId == upazilaId);
                var unionDtos = new List<UnionDto>();

                foreach (var union in unions)
                {
                    var dto = MapToDto(union);
                    dto.StoreCount = await GetUnionStoreCountAsync(union.Id);
                    dto.TotalVDPMembers = union.TotalVDPMembers;
                    unionDtos.Add(dto);
                }

                return unionDtos.OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unions for upazila {UpazilaId}", upazilaId);
                throw;
            }
        }

        public async Task<IEnumerable<UnionDto>> GetUnionsByZilaAsync(int zilaId)
        {
            try
            {
                var upazilas = await _unitOfWork.Upazilas.FindAsync(u => u.ZilaId == zilaId);
                var upazilaIds = upazilas.Select(u => u.Id).ToList();
                var unions = await _unitOfWork.Unions.FindAsync(u => upazilaIds.Contains(u.UpazilaId));

                return unions.Select(MapToDto).OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unions for zila {ZilaId}", zilaId);
                throw;
            }
        }

        public async Task<IEnumerable<UnionDto>> GetUnionsWithVDPUnitsAsync()
        {
            try
            {
                var unions = await _unitOfWork.Unions.FindAsync(u => u.HasVDPUnit);
                return unions.Select(MapToDto).OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unions with VDP units");
                throw;
            }
        }

        public async Task<IEnumerable<UnionDto>> GetBorderAreaUnionsAsync()
        {
            try
            {
                var unions = await _unitOfWork.Unions.FindAsync(u => u.IsBorderArea);
                return unions.Select(MapToDto).OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving border area unions");
                throw;
            }
        }

        public async Task<UnionDto> GetUnionByIdAsync(int id)
        {
            try
            {
                var union = await _unitOfWork.Unions.GetByIdAsync(id);
                if (union == null)
                    return null;

                var dto = MapToDto(union);
                dto.StoreCount = await GetUnionStoreCountAsync(union.Id);
                dto.TotalVDPMembers = union.TotalVDPMembers;

                // Get hierarchical names
                if (union.UpazilaId > 0)
                {
                    var upazila = await _unitOfWork.Upazilas.GetByIdAsync(union.UpazilaId);
                    if (upazila != null)
                    {
                        dto.UpazilaName = upazila.Name;
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
                    }
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving union with ID {Id}", id);
                throw;
            }
        }

        public async Task<UnionDto> GetUnionByCodeAsync(string code)
        {
            try
            {
                var union = await _unitOfWork.Unions.FirstOrDefaultAsync(u => u.Code == code);
                return union != null ? MapToDto(union) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving union with code {Code}", code);
                throw;
            }
        }

        public async Task<UnionDto> CreateUnionAsync(UnionDto unionDto)
        {
            try
            {
                if (await UnionExistsAsync(unionDto.Name, unionDto.UpazilaId))
                    throw new InvalidOperationException($"Union with name '{unionDto.Name}' already exists in this upazila");

                if (await UnionCodeExistsAsync(unionDto.Code))
                    throw new InvalidOperationException($"Union with code '{unionDto.Code}' already exists");

                var union = new Union
                {
                    Name = unionDto.Name,
                    Code = unionDto.Code,
                    NameBangla = unionDto.NameBangla,
                    UpazilaId = unionDto.UpazilaId,
                    ChairmanName = unionDto.ChairmanName,
                    ChairmanContact = unionDto.ChairmanContact,
                    SecretaryName = unionDto.SecretaryName,
                    SecretaryContact = unionDto.SecretaryContact,
                    VDPOfficerName = unionDto.VDPOfficerName,
                    VDPOfficerContact = unionDto.VDPOfficerContact,
                    Email = unionDto.Email,
                    OfficeAddress = unionDto.OfficeAddress,
                    NumberOfWards = unionDto.NumberOfWards,
                    NumberOfVillages = unionDto.NumberOfVillages,
                    NumberOfMouzas = unionDto.NumberOfMouzas,
                    Area = unionDto.Area,
                    Population = unionDto.Population,
                    NumberOfHouseholds = unionDto.NumberOfHouseholds,
                    HasVDPUnit = unionDto.HasVDPUnit,
                    VDPMemberCountMale = unionDto.VDPMemberCountMale,
                    VDPMemberCountFemale = unionDto.VDPMemberCountFemale,
                    AnsarMemberCount = unionDto.AnsarMemberCount,
                    Latitude = unionDto.Latitude,
                    Longitude = unionDto.Longitude,
                    IsRural = unionDto.IsRural,
                    IsBorderArea = unionDto.IsBorderArea,
                    IsActive = unionDto.IsActive,
                    Remarks = unionDto.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = unionDto.CreatedBy ?? "System"
                };

                await _unitOfWork.Unions.AddAsync(union);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "Union",
                    union.Id,
                    "Create",
                    $"Created union: {union.Name} ({union.Code})",
                    unionDto.CreatedBy ?? "System"
                );

                return MapToDto(union);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating union");
                throw;
            }
        }

        public async Task UpdateUnionAsync(UnionDto unionDto)
        {
            try
            {
                var union = await _unitOfWork.Unions.GetByIdAsync(unionDto.Id);
                if (union == null)
                    throw new InvalidOperationException($"Union with ID {unionDto.Id} not found");

                if (await UnionExistsAsync(unionDto.Name, unionDto.UpazilaId, unionDto.Id))
                    throw new InvalidOperationException($"Union with name '{unionDto.Name}' already exists in this upazila");

                if (await UnionCodeExistsAsync(unionDto.Code, unionDto.Id))
                    throw new InvalidOperationException($"Union with code '{unionDto.Code}' already exists");

                union.Name = unionDto.Name;
                union.Code = unionDto.Code;
                union.NameBangla = unionDto.NameBangla;
                union.UpazilaId = unionDto.UpazilaId;
                union.ChairmanName = unionDto.ChairmanName;
                union.ChairmanContact = unionDto.ChairmanContact;
                union.SecretaryName = unionDto.SecretaryName;
                union.SecretaryContact = unionDto.SecretaryContact;
                union.VDPOfficerName = unionDto.VDPOfficerName;
                union.VDPOfficerContact = unionDto.VDPOfficerContact;
                union.Email = unionDto.Email;
                union.OfficeAddress = unionDto.OfficeAddress;
                union.NumberOfWards = unionDto.NumberOfWards;
                union.NumberOfVillages = unionDto.NumberOfVillages;
                union.NumberOfMouzas = unionDto.NumberOfMouzas;
                union.Area = unionDto.Area;
                union.Population = unionDto.Population;
                union.NumberOfHouseholds = unionDto.NumberOfHouseholds;
                union.HasVDPUnit = unionDto.HasVDPUnit;
                union.VDPMemberCountMale = unionDto.VDPMemberCountMale;
                union.VDPMemberCountFemale = unionDto.VDPMemberCountFemale;
                union.AnsarMemberCount = unionDto.AnsarMemberCount;
                union.Latitude = unionDto.Latitude;
                union.Longitude = unionDto.Longitude;
                union.IsRural = unionDto.IsRural;
                union.IsBorderArea = unionDto.IsBorderArea;
                union.IsActive = unionDto.IsActive;
                union.Remarks = unionDto.Remarks;
                union.UpdatedAt = DateTime.UtcNow;
                union.UpdatedBy = unionDto.CreatedBy ?? "System";

                _unitOfWork.Unions.Update(union);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "Union",
                    union.Id,
                    "Update",
                    $"Updated union: {union.Name} ({union.Code})",
                    unionDto.CreatedBy ?? "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating union");
                throw;
            }
        }

        public async Task DeleteUnionAsync(int id)
        {
            try
            {
                var union = await _unitOfWork.Unions.GetByIdAsync(id);
                if (union == null)
                    throw new InvalidOperationException($"Union with ID {id} not found");

                var storeCount = await GetUnionStoreCountAsync(id);
                if (storeCount > 0)
                    throw new InvalidOperationException($"Cannot delete union with {storeCount} stores");

                // Check for issues
                var issues = await _unitOfWork.Issues.FindAsync(i => i.IssuedToUnionId == id);
                if (issues.Any())
                    throw new InvalidOperationException($"Cannot delete union with {issues.Count()} issues");

                // Check for receives
                var receives = await _unitOfWork.Receives.FindAsync(r => r.ReceivedFromUnionId == id);
                if (receives.Any())
                    throw new InvalidOperationException($"Cannot delete union with {receives.Count()} receives");

                _unitOfWork.Unions.Remove(union);
                await _unitOfWork.CompleteAsync();

                await _activityLogService.LogActivityAsync(
                    "Union",
                    union.Id,
                    "Delete",
                    $"Deleted union: {union.Name} ({union.Code})",
                    "System"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting union");
                throw;
            }
        }

        public async Task<bool> UnionExistsAsync(string name, int upazilaId, int? excludeId = null)
        {
            return await _unitOfWork.Unions.ExistsAsync(u =>
                u.Name.ToLower() == name.ToLower() &&
                u.UpazilaId == upazilaId &&
                (!excludeId.HasValue || u.Id != excludeId.Value));
        }

        public async Task<bool> UnionCodeExistsAsync(string code, int? excludeId = null)
        {
            return await _unitOfWork.Unions.ExistsAsync(u =>
                u.Code.ToLower() == code.ToLower() &&
                (!excludeId.HasValue || u.Id != excludeId.Value));
        }

        public async Task<int> GetUnionStoreCountAsync(int unionId)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.UnionId == unionId);
            return stores.Count();
        }

        public async Task<int> GetUnionVDPMemberCountAsync(int unionId)
        {
            var union = await _unitOfWork.Unions.GetByIdAsync(unionId);
            return union?.TotalVDPMembers ?? 0;
        }

        public async Task<Dictionary<string, object>> GetUnionDashboardDataAsync(int unionId)
        {
            try
            {
                var statistics = GetUnionStatisticsAsync(unionId);
                var union = await GetUnionByIdAsync(unionId);

                var dashboardData = new Dictionary<string, object>
                {
                    ["UnionInfo"] = union,
                    ["Statistics"] = statistics,
                    ["RecentIssues"] = await GetRecentIssuesAsync(unionId, 5),
                    ["RecentReceives"] = await GetRecentReceivesAsync(unionId, 5),
                    ["StockStatus"] = await GetStockStatusAsync(unionId),
                    ["Alerts"] = await GetAlertsAsync(unionId)
                };

                return dashboardData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting union dashboard data");
                throw;
            }
        }

        private Task GetUnionStatisticsAsync(int unionId)
        {
            throw new NotImplementedException();
        }

        public async Task ImportUnionsFromCsvAsync(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var record in records)
                {
                    var unionDto = new UnionDto
                    {
                        Code = record.Code,
                        Name = record.Name,
                        NameBangla = record.NameBangla,
                        UpazilaId = int.Parse(record.UpazilaId),
                        ChairmanName = record.ChairmanName,
                        ChairmanContact = record.ChairmanContact,
                        SecretaryName = record.SecretaryName,
                        SecretaryContact = record.SecretaryContact,
                        VDPOfficerName = record.VDPOfficerName,
                        VDPOfficerContact = record.VDPOfficerContact,
                        Email = record.Email,
                        OfficeAddress = record.OfficeAddress,
                        NumberOfWards = string.IsNullOrEmpty(record.NumberOfWards) ? null : int.Parse(record.NumberOfWards),
                        NumberOfVillages = string.IsNullOrEmpty(record.NumberOfVillages) ? null : int.Parse(record.NumberOfVillages),
                        NumberOfMouzas = string.IsNullOrEmpty(record.NumberOfMouzas) ? null : int.Parse(record.NumberOfMouzas),
                        Area = string.IsNullOrEmpty(record.Area) ? null : decimal.Parse(record.Area),
                        Population = string.IsNullOrEmpty(record.Population) ? null : int.Parse(record.Population),
                        NumberOfHouseholds = string.IsNullOrEmpty(record.NumberOfHouseholds) ? null : int.Parse(record.NumberOfHouseholds),
                        HasVDPUnit = bool.Parse(record.HasVDPUnit ?? "false"),
                        VDPMemberCountMale = string.IsNullOrEmpty(record.VDPMemberCountMale) ? null : int.Parse(record.VDPMemberCountMale),
                        VDPMemberCountFemale = string.IsNullOrEmpty(record.VDPMemberCountFemale) ? null : int.Parse(record.VDPMemberCountFemale),
                        AnsarMemberCount = string.IsNullOrEmpty(record.AnsarMemberCount) ? null : int.Parse(record.AnsarMemberCount),
                        IsRural = bool.Parse(record.IsRural ?? "true"),
                        IsBorderArea = bool.Parse(record.IsBorderArea ?? "false"),
                        IsActive = true,
                        CreatedBy = "Import"
                    };

                    if (!await UnionCodeExistsAsync(unionDto.Code))
                    {
                        await CreateUnionAsync(unionDto);
                    }
                }

                _logger.LogInformation("Successfully imported unions from CSV");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing unions from CSV");
                throw;
            }
        }

        public async Task<byte[]> ExportUnionsAsync()
        {
            try
            {
                var unions = await GetAllUnionsAsync();

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteRecords(unions);
                writer.Flush();

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting unions to CSV");
                throw;
            }
        }

        public async Task<byte[]> ExportUnionsByUpazilaAsync(int upazilaId)
        {
            try
            {
                var unions = await GetUnionsByUpazilaAsync(upazilaId);

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteRecords(unions);
                writer.Flush();

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting unions to CSV");
                throw;
            }
        }

        public async Task<IEnumerable<UnionDto>> SearchUnionsAsync(UnionSearchDto searchDto)
        {
            try
            {
                var query = await _unitOfWork.Unions.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
                {
                    var searchTerm = searchDto.SearchTerm.ToLower();
                    query = query.Where(u =>
                        u.Name.ToLower().Contains(searchTerm) ||
                        u.Code.ToLower().Contains(searchTerm) ||
                        u.NameBangla.ToLower().Contains(searchTerm) ||
                        u.ChairmanName.ToLower().Contains(searchTerm));
                }

                if (searchDto.UpazilaId.HasValue)
                    query = query.Where(u => u.UpazilaId == searchDto.UpazilaId.Value);

                if (searchDto.ZilaId.HasValue)
                {
                    var upazilas = await _unitOfWork.Upazilas.FindAsync(up => up.ZilaId == searchDto.ZilaId.Value);
                    var upazilaIds = upazilas.Select(up => up.Id).ToList();
                    query = query.Where(u => upazilaIds.Contains(u.UpazilaId));
                }

                if (searchDto.HasVDPUnit.HasValue)
                    query = query.Where(u => u.HasVDPUnit == searchDto.HasVDPUnit.Value);

                if (searchDto.IsRural.HasValue)
                    query = query.Where(u => u.IsRural == searchDto.IsRural.Value);

                if (searchDto.IsBorderArea.HasValue)
                    query = query.Where(u => u.IsBorderArea == searchDto.IsBorderArea.Value);

                if (searchDto.IsActive.HasValue)
                    query = query.Where(u => u.IsActive == searchDto.IsActive.Value);

                return query.Select(MapToDto).OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching unions");
                throw;
            }
        }

        public async Task<IEnumerable<UnionDto>> GetRuralUnionsAsync()
        {
            try
            {
                var unions = await _unitOfWork.Unions.FindAsync(u => u.IsRural);
                return unions.Select(MapToDto).OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rural unions");
                throw;
            }
        }

        public async Task<IEnumerable<UnionDto>> GetUrbanUnionsAsync()
        {
            try
            {
                var unions = await _unitOfWork.Unions.FindAsync(u => !u.IsRural);
                return unions.Select(MapToDto).OrderBy(u => u.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving urban unions");
                throw;
            }
        }

        public async Task UpdateVDPMemberCountAsync(int unionId, int maleCount, int femaleCount)
        {
            try
            {
                var union = await _unitOfWork.Unions.GetByIdAsync(unionId);
                if (union == null)
                    throw new InvalidOperationException($"Union with ID {unionId} not found");

                union.VDPMemberCountMale = maleCount;
                union.VDPMemberCountFemale = femaleCount;
                union.HasVDPUnit = (maleCount + femaleCount) > 0;
                union.UpdatedAt = DateTime.UtcNow;
                union.UpdatedBy = "System";

                _unitOfWork.Unions.Update(union);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated VDP member count for union {UnionId}: Male={Male}, Female={Female}",
                    unionId, maleCount, femaleCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating VDP member count");
                throw;
            }
        }

        public async Task UpdateAnsarMemberCountAsync(int unionId, int count)
        {
            try
            {
                var union = await _unitOfWork.Unions.GetByIdAsync(unionId);
                if (union == null)
                    throw new InvalidOperationException($"Union with ID {unionId} not found");

                union.AnsarMemberCount = count;
                union.UpdatedAt = DateTime.UtcNow;
                union.UpdatedBy = "System";

                _unitOfWork.Unions.Update(union);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated Ansar member count for union {UnionId} to {Count}", unionId, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Ansar member count");
                throw;
            }
        }

        public async Task<string> GenerateUnionCodeAsync(int upazilaId)
        {
            try
            {
                var upazila = await _unitOfWork.Upazilas.GetByIdAsync(upazilaId);
                if (upazila == null)
                    throw new InvalidOperationException($"Upazila with ID {upazilaId} not found");

                var upazilaCode = upazila.Code.Replace("UPZ-", "");
                var existingUnions = await _unitOfWork.Unions.FindAsync(u => u.UpazilaId == upazilaId);
                var count = existingUnions.Count() + 1;

                return $"UN-{upazilaCode}-{count:D3}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating union code");
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetUnionHierarchyAsync(int unionId)
        {
            try
            {
                var union = await GetUnionByIdAsync(unionId);
                if (union == null)
                    return null;

                var hierarchy = new List<object>
                {
                    new { Level = "Union", Id = union.Id, Name = union.Name, Code = union.Code },
                    new { Level = "Upazila", Id = union.UpazilaId, Name = union.UpazilaName },
                    new { Level = "Zila", Name = union.ZilaName },
                    new { Level = "Range", Name = union.RangeName }
                };

                return hierarchy;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting union hierarchy");
                throw;
            }
        }

        // Private helper methods
        private UnionDto MapToDto(Union union)
        {
            return new UnionDto
            {
                Id = union.Id,
                Name = union.Name,
                Code = union.Code,
                NameBangla = union.NameBangla,
                UpazilaId = union.UpazilaId,
                ChairmanName = union.ChairmanName,
                ChairmanContact = union.ChairmanContact,
                SecretaryName = union.SecretaryName,
                SecretaryContact = union.SecretaryContact,
                VDPOfficerName = union.VDPOfficerName,
                VDPOfficerContact = union.VDPOfficerContact,
                Email = union.Email,
                OfficeAddress = union.OfficeAddress,
                NumberOfWards = union.NumberOfWards,
                NumberOfVillages = union.NumberOfVillages,
                NumberOfMouzas = union.NumberOfMouzas,
                Area = union.Area,
                Population = union.Population,
                NumberOfHouseholds = union.NumberOfHouseholds,
                HasVDPUnit = union.HasVDPUnit,
                VDPMemberCountMale = union.VDPMemberCountMale,
                VDPMemberCountFemale = union.VDPMemberCountFemale,
                AnsarMemberCount = union.AnsarMemberCount,
                Latitude = union.Latitude,
                Longitude = union.Longitude,
                IsRural = union.IsRural,
                IsBorderArea = union.IsBorderArea,
                IsActive = union.IsActive,
                Remarks = union.Remarks,
                CreatedAt = union.CreatedAt,
                CreatedBy = union.CreatedBy,
                UpdatedAt = union.UpdatedAt,
                UpdatedBy = union.UpdatedBy,
                TotalVDPMembers = union.TotalVDPMembers
            };
        }

        private async Task<IEnumerable<object>> GetRecentIssuesAsync(int unionId, int count)
        {
            var issues = await _unitOfWork.Issues.FindAsync(i => i.IssuedToUnionId == unionId);
            return issues.OrderByDescending(i => i.IssueDate)
                .Take(count)
                .Select(i => new { i.Id, i.IssueNo, i.IssueDate, i.Status });
        }

        private async Task<IEnumerable<object>> GetRecentReceivesAsync(int unionId, int count)
        {
            var receives = await _unitOfWork.Receives.FindAsync(r => r.ReceivedFromUnionId == unionId);
            return receives.OrderByDescending(r => r.ReceiveDate)
                .Take(count)
                .Select(r => new { r.Id, r.ReceiveNo, r.ReceiveDate, r.Status });
        }

        private async Task<object> GetStockStatusAsync(int unionId)
        {
            var stores = await _unitOfWork.Stores.FindAsync(s => s.UnionId == unionId);
            var storeIds = stores.Select(s => s.Id).ToList();

            // Implement stock status logic based on your requirements
            return new
            {
                TotalStores = stores.Count(),
                TotalItems = 0, // Implement based on your stock tracking
                LowStockItems = 0,
                OutOfStockItems = 0
            };
        }

        private async Task<IEnumerable<object>> GetAlertsAsync(int unionId)
        {
            // Implement alerts logic based on your requirements
            var alerts = new List<object>();

            var union = await _unitOfWork.Unions.GetByIdAsync(unionId);
            if (union != null)
            {
                if (!union.HasVDPUnit)
                {
                    alerts.Add(new { Type = "Warning", Message = "No VDP unit established" });
                }

                if (union.TotalVDPMembers < 10)
                {
                    alerts.Add(new { Type = "Info", Message = "Low VDP member count" });
                }
            }

            return alerts;
        }

        Task<UnionStatisticsDto> IUnionService.GetUnionStatisticsAsync(int unionId)
        {
            throw new NotImplementedException();
        }
    }
}