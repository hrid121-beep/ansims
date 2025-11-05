using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;

namespace IMS.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.Brands.GetAllAsync();
            var brandDtos = new List<BrandDto>();

            foreach (var brand in brands.Where(b => b.IsActive))
            {
                var modelCount = await _unitOfWork.ItemModels.CountAsync(m => m.BrandId == brand.Id && m.IsActive);

                brandDtos.Add(new BrandDto
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description,
                    CreatedAt = brand.CreatedAt,
                    IsActive = brand.IsActive,
                    ModelCount = modelCount
                });
            }

            return brandDtos;
        }

        public async Task<BrandDto> GetBrandByIdAsync(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null || !brand.IsActive) return null;

            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description
            };
        }

        public async Task<BrandDto> CreateBrandAsync(BrandDto brandDto)
        {
            var brand = new Brand
            {
                Name = brandDto.Name,
                Description = brandDto.Description,
                CreatedAt = DateTime.Now,
                CreatedBy = "System" // TODO: Get from current user
            };

            await _unitOfWork.Brands.AddAsync(brand);
            await _unitOfWork.CompleteAsync();

            brandDto.Id = brand.Id;
            return brandDto;
        }

        public async Task UpdateBrandAsync(BrandDto brandDto)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(brandDto.Id);
            if (brand != null)
            {
                brand.Name = brandDto.Name;
                brand.Description = brandDto.Description;
                brand.UpdatedAt = DateTime.Now;
                brand.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.Brands.Update(brand);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteBrandAsync(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand != null)
            {
                brand.IsActive = false;
                _unitOfWork.Brands.Update(brand);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<BrandDto>> GetActiveBrandsAsync()
        {
            return await GetAllBrandsAsync();
        }

        public async Task<bool> BrandExistsAsync(string name, int? excludeId = null)
        {
            var brands = await _unitOfWork.Brands.FindAsync(b => b.Name == name && b.IsActive);
            if (excludeId.HasValue)
            {
                brands = brands.Where(b => b.Id != excludeId.Value);
            }
            return brands.Any();
        }

        public async Task<int> GetBrandItemCountAsync(int brandId)
        {
            var itemModels = await _unitOfWork.ItemModels.FindAsync(im => im.BrandId == brandId && im.IsActive);
            var itemCount = 0;

            foreach (var model in itemModels)
            {
                var items = await _unitOfWork.Items.CountAsync(i => i.ItemModelId == model.Id && i.IsActive);
                itemCount += items;
            }

            return itemCount;
        }

        public async Task<IEnumerable<BrandDto>> SearchBrandsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllBrandsAsync();

            var brands = await GetAllBrandsAsync();
            searchTerm = searchTerm.ToLower();

            return brands.Where(b =>
                b.Name.ToLower().Contains(searchTerm) ||
                (b.Description != null && b.Description.ToLower().Contains(searchTerm))
            );
        }

        public async Task<IEnumerable<BrandDto>> GetPagedBrandsAsync(int pageNumber, int pageSize)
        {
            var allBrands = await GetAllBrandsAsync();
            return allBrands
                .OrderBy(b => b.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
    }
}