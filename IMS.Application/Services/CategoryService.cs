using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;

namespace IMS.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryDtos = new List<CategoryDto>();

            foreach (var category in categories.Where(c => c.IsActive))
            {
                // Count subcategories
                var subCategoryCount = await _unitOfWork.SubCategories.CountAsync(sc => sc.CategoryId == category.Id && sc.IsActive);

                // Count items in this category
                var itemCount = await _unitOfWork.Items.CountAsync(i => i.CategoryId == category.Id && i.IsActive);

                categoryDtos.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Code = category.Code,
                    CreatedAt = category.CreatedAt,
                    IsActive = category.IsActive,
                    SubCategoryCount = subCategoryCount,
                    ItemCount = itemCount
                });
            }

            return categoryDtos.OrderBy(c => c.Name);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null || !category.IsActive) return null;

            // Count subcategories
            var subCategoryCount = await _unitOfWork.SubCategories.CountAsync(sc => sc.CategoryId == category.Id && sc.IsActive);

            // Count items in this category
            var itemCount = await _unitOfWork.Items.CountAsync(i => i.CategoryId == category.Id && i.IsActive);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Code = category.Code,
                CreatedAt = category.CreatedAt,
                IsActive = category.IsActive,
                SubCategoryCount = subCategoryCount,
                ItemCount = itemCount
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                Code = categoryDto.Code,
                CreatedAt = DateTime.Now,
                CreatedBy = "System", // TODO: Get from current user
                IsActive = true
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            categoryDto.Id = category.Id;
            return categoryDto;
        }

        public async Task UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
            if (category != null)
            {
                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;
                category.Code = categoryDto.Code;
                category.UpdatedAt = DateTime.Now;
                category.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                category.IsActive = false;
                category.UpdatedAt = DateTime.Now;
                category.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> CategoryExistsAsync(string name, int? excludeId = null)
        {
            var categories = await _unitOfWork.Categories.FindAsync(c => c.Name == name && c.IsActive);
            if (excludeId.HasValue)
            {
                categories = categories.Where(c => c.Id != excludeId.Value);
            }
            return categories.Any();
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesWithSubCategoriesAsync()
        {
            return await GetAllCategoriesAsync();
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            return await GetAllCategoriesAsync();
        }

        public async Task<int> GetCategoryItemCountAsync(int categoryId)
        {
            return await _unitOfWork.Items.CountAsync(i => i.CategoryId == categoryId && i.IsActive);
        }
    }
}