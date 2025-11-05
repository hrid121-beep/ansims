using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;

namespace IMS.Application.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubCategoryDto>> GetAllSubCategoriesAsync()
        {
            var subCategories = await _unitOfWork.SubCategories.GetAllAsync();
            var subCategoryDtos = new List<SubCategoryDto>();

            foreach (var subCategory in subCategories.Where(sc => sc.IsActive))
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId);
                var itemCount = await _unitOfWork.Items.CountAsync(i => i.SubCategoryId == subCategory.Id && i.IsActive);

                subCategoryDtos.Add(new SubCategoryDto
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    Description = subCategory.Description,
                    Code = subCategory.Code,
                    CategoryId = subCategory.CategoryId,
                    CategoryName = category?.Name,
                    CreatedAt = subCategory.CreatedAt,
                    IsActive = subCategory.IsActive,
                    ItemCount = itemCount
                });
            }

            return subCategoryDtos.OrderBy(sc => sc.CategoryName).ThenBy(sc => sc.Name);
        }

        public async Task<IEnumerable<SubCategoryDto>> GetSubCategoriesByCategoryIdAsync(int categoryId)
        {
            var subCategories = await _unitOfWork.SubCategories.FindAsync(sc => sc.CategoryId == categoryId && sc.IsActive);
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            var subCategoryDtos = new List<SubCategoryDto>();

            foreach (var subCategory in subCategories)
            {
                var itemCount = await _unitOfWork.Items.CountAsync(i => i.SubCategoryId == subCategory.Id && i.IsActive);

                subCategoryDtos.Add(new SubCategoryDto
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    Description = subCategory.Description,
                    Code = subCategory.Code,
                    CategoryId = subCategory.CategoryId,
                    CategoryName = category?.Name,
                    CreatedAt = subCategory.CreatedAt,
                    IsActive = subCategory.IsActive,
                    ItemCount = itemCount
                });
            }

            return subCategoryDtos.OrderBy(sc => sc.Name);
        }

        public async Task<SubCategoryDto> GetSubCategoryByIdAsync(int id)
        {
            var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(id);
            if (subCategory == null || !subCategory.IsActive) return null;

            var category = await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId);
            var itemCount = await _unitOfWork.Items.CountAsync(i => i.SubCategoryId == subCategory.Id && i.IsActive);

            return new SubCategoryDto
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                Description = subCategory.Description,
                Code = subCategory.Code,
                CategoryId = subCategory.CategoryId,
                CategoryName = category?.Name,
                CreatedAt = subCategory.CreatedAt,
                IsActive = subCategory.IsActive,
                ItemCount = itemCount
            };
        }

        public async Task<SubCategoryDto> CreateSubCategoryAsync(SubCategoryDto subCategoryDto)
        {
            var subCategory = new SubCategory
            {
                Name = subCategoryDto.Name,
                Description = subCategoryDto.Description,
                Code = subCategoryDto.Code,
                CategoryId = subCategoryDto.CategoryId,
                CreatedAt = DateTime.Now,
                CreatedBy = "System", // TODO: Get from current user
                IsActive = true
            };

            await _unitOfWork.SubCategories.AddAsync(subCategory);
            await _unitOfWork.CompleteAsync();

            subCategoryDto.Id = subCategory.Id;
            return subCategoryDto;
        }

        public async Task UpdateSubCategoryAsync(SubCategoryDto subCategoryDto)
        {
            var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(subCategoryDto.Id);
            if (subCategory != null)
            {
                subCategory.Name = subCategoryDto.Name;
                subCategory.Description = subCategoryDto.Description;
                subCategory.Code = subCategoryDto.Code;
                subCategory.CategoryId = subCategoryDto.CategoryId;
                subCategory.UpdatedAt = DateTime.Now;
                subCategory.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.SubCategories.Update(subCategory);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteSubCategoryAsync(int id)
        {
            var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(id);
            if (subCategory != null)
            {
                subCategory.IsActive = false;
                subCategory.UpdatedAt = DateTime.Now;
                subCategory.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.SubCategories.Update(subCategory);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> SubCategoryExistsAsync(string name, int categoryId, int? excludeId = null)
        {
            var subCategories = await _unitOfWork.SubCategories.FindAsync(sc =>
                sc.Name == name && sc.CategoryId == categoryId && sc.IsActive);

            if (excludeId.HasValue)
            {
                subCategories = subCategories.Where(sc => sc.Id != excludeId.Value);
            }
            return subCategories.Any();
        }

        public async Task<IEnumerable<SubCategoryDto>> GetActiveSubCategoriesAsync()
        {
            return await GetAllSubCategoriesAsync();
        }

        public async Task<int> GetSubCategoryItemCountAsync(int subCategoryId)
        {
            return await _unitOfWork.Items.CountAsync(i => i.SubCategoryId == subCategoryId && i.IsActive);
        }
    }
}