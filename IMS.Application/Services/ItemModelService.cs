using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;

namespace IMS.Application.Services
{
    public class ItemModelService : IItemModelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ItemModelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ItemModelDto>> GetAllItemModelsAsync()
        {
            var itemModels = await _unitOfWork.ItemModels.GetAllAsync();
            var itemModelDtos = new List<ItemModelDto>();

            foreach (var model in itemModels.Where(m => m.IsActive))
            {
                var brand = await _unitOfWork.Brands.GetByIdAsync(model.BrandId);
                var itemCount = await _unitOfWork.Items.CountAsync(i => i.ItemModelId == model.Id && i.IsActive);

                itemModelDtos.Add(new ItemModelDto
                {
                    Id = model.Id,
                    Name = model.Name,
                    ModelNumber = model.ModelNumber,
                    BrandId = model.BrandId,
                    BrandName = brand?.Name,
                    CreatedAt = model.CreatedAt,
                    IsActive = model.IsActive,
                    ItemCount = itemCount
                });
            }

            return itemModelDtos.OrderBy(im => im.BrandName).ThenBy(im => im.Name);
        }

        public async Task<IEnumerable<ItemModelDto>> GetItemModelsByBrandIdAsync(int brandId)
        {
            var itemModels = await _unitOfWork.ItemModels.FindAsync(im => im.BrandId == brandId && im.IsActive);
            var itemModelDtos = new List<ItemModelDto>();
            var brand = await _unitOfWork.Brands.GetByIdAsync(brandId);

            foreach (var itemModel in itemModels)
            {
                var itemCount = await _unitOfWork.Items.CountAsync(i => i.ItemModelId == itemModel.Id && i.IsActive);

                itemModelDtos.Add(new ItemModelDto
                {
                    Id = itemModel.Id,
                    Name = itemModel.Name,
                    ModelNumber = itemModel.ModelNumber,
                    BrandId = itemModel.BrandId,
                    BrandName = brand?.Name,
                    CreatedAt = itemModel.CreatedAt,
                    IsActive = itemModel.IsActive,
                    ItemCount = itemCount
                });
            }

            return itemModelDtos.OrderBy(im => im.Name);
        }

        public async Task<ItemModelDto> GetItemModelByIdAsync(int id)
        {
            var itemModel = await _unitOfWork.ItemModels.GetByIdAsync(id);
            if (itemModel == null || !itemModel.IsActive) return null;

            var brand = await _unitOfWork.Brands.GetByIdAsync(itemModel.BrandId);
            var itemCount = await _unitOfWork.Items.CountAsync(i => i.ItemModelId == itemModel.Id && i.IsActive);

            return new ItemModelDto
            {
                Id = itemModel.Id,
                Name = itemModel.Name,
                ModelNumber = itemModel.ModelNumber,
                BrandId = itemModel.BrandId,
                BrandName = brand?.Name,
                CreatedAt = itemModel.CreatedAt,
                IsActive = itemModel.IsActive,
                ItemCount = itemCount
            };
        }

        public async Task<ItemModelDto> CreateItemModelAsync(ItemModelDto itemModelDto)
        {
            var itemModel = new ItemModel
            {
                Name = itemModelDto.Name,
                ModelNumber = itemModelDto.ModelNumber,
                BrandId = itemModelDto.BrandId,
                CreatedAt = DateTime.Now,
                CreatedBy = "System", // TODO: Get from current user
                IsActive = true
            };

            await _unitOfWork.ItemModels.AddAsync(itemModel);
            await _unitOfWork.CompleteAsync();

            itemModelDto.Id = itemModel.Id;
            return itemModelDto;
        }

        public async Task UpdateItemModelAsync(ItemModelDto itemModelDto)
        {
            var itemModel = await _unitOfWork.ItemModels.GetByIdAsync(itemModelDto.Id);
            if (itemModel != null)
            {
                itemModel.Name = itemModelDto.Name;
                itemModel.ModelNumber = itemModelDto.ModelNumber;
                itemModel.BrandId = itemModelDto.BrandId;
                itemModel.UpdatedAt = DateTime.Now;
                itemModel.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.ItemModels.Update(itemModel);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteItemModelAsync(int id)
        {
            var itemModel = await _unitOfWork.ItemModels.GetByIdAsync(id);
            if (itemModel != null)
            {
                itemModel.IsActive = false;
                itemModel.UpdatedAt = DateTime.Now;
                itemModel.UpdatedBy = "System"; // TODO: Get from current user

                _unitOfWork.ItemModels.Update(itemModel);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<ItemModelDto>> GetActiveItemModelsAsync()
        {
            return await GetAllItemModelsAsync();
        }

        public async Task<bool> ItemModelExistsAsync(string name, string modelNumber, int brandId, int? excludeId = null)
        {
            var itemModels = await _unitOfWork.ItemModels.FindAsync(im =>
                im.Name == name && im.ModelNumber == modelNumber && im.BrandId == brandId && im.IsActive);

            if (excludeId.HasValue)
            {
                itemModels = itemModels.Where(im => im.Id != excludeId.Value);
            }

            return itemModels.Any();
        }

        public async Task<int> GetItemModelItemCountAsync(int modelId)
        {
            return await _unitOfWork.Items.CountAsync(i => i.ItemModelId == modelId && i.IsActive);
        }

        public async Task<IEnumerable<ItemModelDto>> SearchItemModelsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllItemModelsAsync();

            var allModels = await GetAllItemModelsAsync();
            searchTerm = searchTerm.ToLower();

            return allModels.Where(im =>
                im.Name.ToLower().Contains(searchTerm) ||
                im.ModelNumber.ToLower().Contains(searchTerm) ||
                (im.BrandName != null && im.BrandName.ToLower().Contains(searchTerm))
            );
        }

        public async Task<IEnumerable<ItemModelDto>> GetPagedItemModelsAsync(int pageNumber, int pageSize)
        {
            var allModels = await GetAllItemModelsAsync();
            return allModels
                .OrderBy(im => im.BrandName)
                .ThenBy(im => im.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task<IEnumerable<ItemModelDto>> GetItemModelsByBrandAsync(int brandId)
        {
            return await GetItemModelsByBrandIdAsync(brandId);
        }
    }
}