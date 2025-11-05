using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Services
{
    public class StoreTypeService : IStoreTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StoreTypeService> _logger;

        public StoreTypeService(IUnitOfWork unitOfWork, ILogger<StoreTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<IEnumerable<StoreTypeDto>> GetAllStoreTypesAsync()
        {
            try
            {
                var storeTypes = await _unitOfWork.StoreTypes.GetAllAsync();
                var storeTypeDtos = new List<StoreTypeDto>();

                foreach (var storeType in storeTypes.Where(st => st.IsActive))
                {
                    var stores = await _unitOfWork.Stores.FindAsync(s => s.StoreTypeId == storeType.Id && s.IsActive);
                    var categories = await GetStoreTypeCategoriesAsync(storeType.Id);

                    storeTypeDtos.Add(new StoreTypeDto
                    {
                        Id = storeType.Id,
                        Code = storeType.Code,
                        Name = storeType.Name,
                        Description = storeType.Description,
                        Icon = storeType.Icon,
                        Color = storeType.Color,
                        IsMainStore = storeType.IsMainStore,
                        AllowDirectIssue = storeType.AllowDirectIssue,
                        AllowTransfer = storeType.AllowTransfer,
                        MaxCapacity = storeType.MaxCapacity,
                        StoreCount = stores.Count(),
                        CategoryCount = categories.Count(),
                        AllowedCategories = categories.ToList(),
                        IsActive = storeType.IsActive
                    });
                }

                return storeTypeDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving store types");
                throw;
            }
        }


        public async Task<StoreTypeDto> GetStoreTypeByIdAsync(int id)
        {
            var storeType = await _unitOfWork.StoreTypes.GetByIdAsync(id);
            if (storeType == null || !storeType.IsActive) return null;

            var stores = await _unitOfWork.Stores.FindAsync(s => s.StoreTypeId == storeType.Id && s.IsActive);
            var categories = await GetStoreTypeCategoriesAsync(storeType.Id);

            return new StoreTypeDto
            {
                Id = storeType.Id,
                Code = storeType.Code,
                Name = storeType.Name,
                Description = storeType.Description,
                Icon = storeType.Icon,
                Color = storeType.Color,
                IsMainStore = storeType.IsMainStore,
                AllowDirectIssue = storeType.AllowDirectIssue,
                AllowTransfer = storeType.AllowTransfer,
                MaxCapacity = storeType.MaxCapacity,
                StoreCount = stores.Count(),
                CategoryCount = categories.Count(),
                AllowedCategories = categories.ToList(),
                AllowedCategoryIds = categories.Select(c => c.Id).ToList(),
                IsActive = storeType.IsActive
            };
        }

        public async Task<StoreTypeDto> GetStoreTypeByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            var storeType = await _unitOfWork.StoreTypes.FirstOrDefaultAsync(st => st.Code == code && st.IsActive);
            if (storeType == null) return null;

            return await GetStoreTypeByIdAsync(storeType.Id);
        }

        public async Task<StoreTypeDto> CreateStoreTypeAsync(StoreTypeDto storeTypeDto)
        {
            var storeType = new StoreType
            {
                Code = storeTypeDto.Code,
                Name = storeTypeDto.Name,
                Description = storeTypeDto.Description,
                Icon = storeTypeDto.Icon,
                Color = storeTypeDto.Color,
                IsMainStore = storeTypeDto.IsMainStore,
                AllowDirectIssue = storeTypeDto.AllowDirectIssue,
                AllowTransfer = storeTypeDto.AllowTransfer,
                MaxCapacity = storeTypeDto.MaxCapacity,
                CreatedAt = DateTime.Now,
                CreatedBy = storeTypeDto.CreatedBy ?? "System",
                IsActive = true
            };

            await _unitOfWork.StoreTypes.AddAsync(storeType);
            await _unitOfWork.CompleteAsync();

            storeTypeDto.Id = storeType.Id;
            return storeTypeDto;
        }

        public async Task UpdateStoreTypeAsync(StoreTypeDto storeTypeDto)
        {
            var storeType = await _unitOfWork.StoreTypes.GetByIdAsync(storeTypeDto.Id);
            if (storeType == null) throw new InvalidOperationException("Store type not found");

            storeType.Code = storeTypeDto.Code;
            storeType.Name = storeTypeDto.Name;
            storeType.Description = storeTypeDto.Description;
            storeType.Icon = storeTypeDto.Icon;
            storeType.Color = storeTypeDto.Color;
            storeType.IsMainStore = storeTypeDto.IsMainStore;
            storeType.AllowDirectIssue = storeTypeDto.AllowDirectIssue;
            storeType.AllowTransfer = storeTypeDto.AllowTransfer;
            storeType.MaxCapacity = storeTypeDto.MaxCapacity;
            storeType.UpdatedAt = DateTime.Now;
            storeType.UpdatedBy = storeTypeDto.UpdatedBy ?? "System";

            _unitOfWork.StoreTypes.Update(storeType);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStoreTypeAsync(int id)
        {
            var storeType = await _unitOfWork.StoreTypes.GetByIdAsync(id);
            if (storeType == null) throw new InvalidOperationException("Store type not found");

            var hasStores = await _unitOfWork.Stores.ExistsAsync(s => s.StoreTypeId == id && s.IsActive);
            if (hasStores) throw new InvalidOperationException("Cannot delete store type with associated stores");

            storeType.IsActive = false;
            storeType.UpdatedAt = DateTime.Now;
            storeType.UpdatedBy = "System";

            _unitOfWork.StoreTypes.Update(storeType);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> CanStoreTypeHoldCategoryAsync(int storeTypeId, int categoryId)
        {
            var mapping = await _unitOfWork.StoreTypeCategories.FirstOrDefaultAsync(
                stc => stc.StoreTypeId == storeTypeId && stc.CategoryId == categoryId && stc.IsActive
            );

            return mapping != null;
        }

        public async Task AssignCategoriesToStoreTypeAsync(int storeTypeId, List<int> categoryIds)
        {
            // Remove existing assignments
            var existingAssignments = await _unitOfWork.StoreTypeCategories.FindAsync(
                stc => stc.StoreTypeId == storeTypeId
            );

            foreach (var assignment in existingAssignments)
            {
                assignment.IsActive = false;
                _unitOfWork.StoreTypeCategories.Update(assignment);
            }

            // Add new assignments
            foreach (var categoryId in categoryIds)
            {
                var existing = existingAssignments.FirstOrDefault(a => a.CategoryId == categoryId);
                if (existing != null)
                {
                    existing.IsActive = true;
                    _unitOfWork.StoreTypeCategories.Update(existing);
                }
                else
                {
                    var newAssignment = new StoreTypeCategory
                    {
                        StoreTypeId = storeTypeId,
                        CategoryId = categoryId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "System",
                        IsActive = true
                    };
                    await _unitOfWork.StoreTypeCategories.AddAsync(newAssignment);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        private async Task<IEnumerable<CategoryDto>> GetStoreTypeCategoriesAsync(int storeTypeId)
        {
            var mappings = await _unitOfWork.StoreTypeCategories.FindAsync(
                stc => stc.StoreTypeId == storeTypeId && stc.IsActive
            );

            var categories = new List<CategoryDto>();
            foreach (var mapping in mappings)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(mapping.CategoryId);
                if (category != null && category.IsActive)
                {
                    categories.Add(new CategoryDto
                    {
                        Id = category.Id,
                        Code = category.Code,
                        Name = category.Name,
                        Description = category.Description
                    });
                }
            }

            return categories;
        }
    }
}
