using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace IMS.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ItemService> _logger;
        public ItemService(IUnitOfWork unitOfWork, ILogger<ItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  
        }
        public async Task<string> GenerateItemCodeAsync(int? subCategoryId)
        {
            try
            {
                // Handle case when called from Create form with subCategoryId = 0 or null
                if (!subCategoryId.HasValue || subCategoryId.Value <= 0)
                {
                    // Generate a temporary code that will be replaced when actual subcategory is selected
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    return $"TEMP-{timestamp}";
                }

                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(subCategoryId.Value);

                // Check if subcategory exists
                if (subCategory == null)
                {
                    // Fallback to generic code if subcategory not found
                    var itemCount = await _unitOfWork.Items.CountAsync(i => i.IsActive) + 1;
                    return $"ITEM-{DateTime.Now.Year}-{itemCount:D4}";
                }

                var category = await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId);

                // Generate prefix based on available data
                string prefix;
                if (category != null && !string.IsNullOrWhiteSpace(category.Name) && !string.IsNullOrWhiteSpace(subCategory.Name))
                {
                    // Both category and subcategory exist - take first 2 chars from each
                    var catPrefix = category.Name.Length >= 2 ? category.Name.Substring(0, 2) : category.Name;
                    var subCatPrefix = subCategory.Name.Length >= 2 ? subCategory.Name.Substring(0, 2) : subCategory.Name;
                    prefix = $"{catPrefix}{subCatPrefix}".ToUpper();
                }
                else if (!string.IsNullOrWhiteSpace(subCategory.Name))
                {
                    // Only subcategory name available
                    prefix = subCategory.Name.Length >= 4 ? subCategory.Name.Substring(0, 4).ToUpper() : subCategory.Name.ToUpper();
                }
                else
                {
                    // Fallback prefix
                    prefix = "ITEM";
                }

                // Remove any special characters from prefix
                prefix = System.Text.RegularExpressions.Regex.Replace(prefix, @"[^A-Z0-9]", "");

                // Ensure prefix has at least 2 characters
                if (string.IsNullOrWhiteSpace(prefix) || prefix.Length < 2)
                {
                    prefix = "IT";
                }

                // Get count for this subcategory
                var count = await _unitOfWork.Items.CountAsync(i => i.SubCategoryId == subCategoryId) + 1;

                // Generate final code: PREFIX-YEAR-COUNT (e.g., WEAP-2025-0001)
                return $"{prefix}-{DateTime.Now.Year}-{count:D4}";
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error generating item code for subcategory {SubCategoryId}", subCategoryId);

                // Ultimate fallback - always return a valid code
                try
                {
                    var fallbackCount = await _unitOfWork.Items.CountAsync(i => i.IsActive) + 1;
                    return $"ITEM-{DateTime.Now.Year}-{fallbackCount:D4}";
                }
                catch
                {
                    // If even counting fails, use timestamp
                    return $"ITEM-{DateTime.Now:yyyyMMddHHmmss}";
                }
            }
        }

        public async Task<PagedResult<ItemDto>> GetPagedItemsAsync(int pageNumber, int pageSize)
        {
            try
            {
                // Get total count of active items
                var totalCount = await _unitOfWork.Items.CountAsync(i => i.IsActive);

                // Get items for the current page
                var skip = (pageNumber - 1) * pageSize;
                var items = await _unitOfWork.Items.FindAsync(i => i.IsActive);
                var pagedItems = items.OrderByDescending(i => i.CreatedAt)
                                      .Skip(skip)
                                      .Take(pageSize)
                                      .ToList();

                // Convert to DTOs
                var itemDtos = new List<ItemDto>();
                foreach (var item in pagedItems)
                {
                    // Map to DTO using existing logic
                    var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                    var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;
                    var model = item.ItemModelId.HasValue ? await _unitOfWork.ItemModels.GetByIdAsync(item.ItemModelId.Value) : null;
                    var brand = item.BrandId.HasValue ? await _unitOfWork.Brands.GetByIdAsync(item.BrandId.Value) : null;  // ✅ FIX: Get brand from item.BrandId
                    var currentStock = await CalculateCurrentStockAsync(item.Id);

                    itemDtos.Add(new ItemDto
                    {
                        Id = item.Id,
                        ItemCode = item.ItemCode,
                        Name = item.Name,
                        Description = item.Description,
                        SubCategoryId = item.SubCategoryId,
                        SubCategoryName = subCategory?.Name,
                        CategoryId = subCategory?.CategoryId ?? 0,
                        CategoryName = category?.Name,
                        ItemModelId = item.ItemModelId,
                        ModelName = model?.Name,
                        BrandId = item.BrandId,  // ✅ FIX: Use item.BrandId instead of model?.BrandId
                        BrandName = brand?.Name,
                        Type = item.Type,
                        Unit = item.Unit,
                        MinimumStock = item.MinimumStock,
                        CurrentStock = currentStock,
                        ReorderLevel = item.ReorderLevel,
                        Status = item.Status,
                        Weight = item.Weight,
                        ItemImage = item.ItemImage,
                        IsActive = item.IsActive,
                        CreatedAt = item.CreatedAt,
                        UpdatedAt = item.UpdatedAt,
                        CreatedBy = item.CreatedBy,
                        UpdatedBy = item.UpdatedBy
                    });
                }

                return new PagedResult<ItemDto>
                {
                    Items = itemDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting paged items");
                return new PagedResult<ItemDto>
                {
                    Items = new List<ItemDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        private string GetSafePrefix(string text, int length)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new string('X', length);

            // Remove special characters and spaces
            var cleanText = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z0-9]", "");

            if (string.IsNullOrWhiteSpace(cleanText))
                return new string('X', length);

            if (cleanText.Length >= length)
                return cleanText.Substring(0, length).ToUpper();

            return cleanText.ToUpper().PadRight(length, 'X');
        }

        public async Task<IEnumerable<ItemDto>> GetAllItemsAsync()
        {
            var items = await _unitOfWork.Items.GetAllAsync();
            var itemDtos = new List<ItemDto>();

            foreach (var item in items.Where(i => i.IsActive))
            {
                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;
                var model = item.ItemModelId.HasValue ? await _unitOfWork.ItemModels.GetByIdAsync(item.ItemModelId.Value) : null;
                var brand = item.BrandId.HasValue ? await _unitOfWork.Brands.GetByIdAsync(item.BrandId.Value) : null;  // ✅ FIX

                var currentStock = await CalculateCurrentStockAsync(item.Id);

                itemDtos.Add(new ItemDto
                {
                    Id = item.Id,
                    ItemCode = item.ItemCode,
                    Name = item.Name,
                    Description = item.Description,
                    SubCategoryId = item.SubCategoryId,
                    SubCategoryName = subCategory?.Name,
                    CategoryId = subCategory?.CategoryId ?? 0,
                    CategoryName = category?.Name,
                    ItemModelId = item.ItemModelId,
                    ModelName = model?.Name,
                    BrandId = item.BrandId,
                    BrandName = brand?.Name,
                    Type = item.Type,
                    Unit = item.Unit,
                    MinimumStock = item.MinimumStock,
                    CurrentStock = currentStock,
                    ReorderLevel = item.ReorderLevel,
                    IsActive = item.IsActive,
                    CreatedAt = item.CreatedAt,
                    UnitCost = item.UnitCost,  // ✅ ADDED - This is the fix!

                    // Image and Barcode fields - ADDED
                    ImagePath = item.ImagePath,
                    ItemImage = item.ItemImage,
                    BarcodePath = item.BarcodePath,
                    QRCodeData = item.QRCodeData
                });
            }

            return itemDtos;
        }

        public async Task<ItemDto> GetItemByIdAsync(int id)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(id);
            if (item == null || !item.IsActive) return null;

            var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
            var category = subCategory != null ? await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;
            var model = item.ItemModelId.HasValue ? await _unitOfWork.ItemModels.GetByIdAsync(item.ItemModelId.Value) : null;
            var brand = item.BrandId.HasValue ? await _unitOfWork.Brands.GetByIdAsync(item.BrandId.Value) : null;  // ✅ FIX

            // Get location from StoreItems table
            var storeItem = await _unitOfWork.StoreItems
                .FirstOrDefaultAsync(si => si.ItemId == id);

            var currentStock = await CalculateCurrentStockAsync(item.Id);

            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                NameBn = item.NameBn,
                ItemCode = item.ItemCode,
                Code = item.Code,
                Description = item.Description,
                Unit = item.Unit,
                Type = item.Type,
                Status = item.Status,
                SubCategoryId = item.SubCategoryId,
                SubCategoryName = subCategory?.Name,
                CategoryId = subCategory?.CategoryId ?? 0,  // Fix this issue
                CategoryName = category?.Name,
                ItemModelId = item.ItemModelId,
                ModelName = model?.Name,
                BrandId = item.BrandId,  // ✅ FIX: Use item.BrandId
                BrandName = brand?.Name,

                // Stock fields
                MinimumStock = item.MinimumStock,
                MaximumStock = item.MaximumStock,
                ReorderLevel = item.ReorderLevel,
                UnitPrice = item.UnitPrice,
                UnitCost = item.UnitCost,  // ✅ Map from DB

                // Product details - ALL from DB
                Manufacturer = item.Manufacturer,  // ✅ Map from DB
                ManufactureDate = item.ManufactureDate,  // ✅ Map from DB
                ExpiryDate = item.ExpiryDate,  // ✅ Map from DB
                HasExpiry = item.HasExpiry,  // ✅ Map from DB
                ShelfLife = item.ShelfLife,  // ✅ Map from DB
                StorageRequirements = item.StorageRequirements,  // ✅ Map from DB
                RequiresSpecialHandling = item.RequiresSpecialHandling,  // ✅ Map from DB
                SafetyInstructions = item.SafetyInstructions,  // ✅ Map from DB

                // Physical properties - ALL from DB
                Weight = item.Weight,  // ✅ Map from DB
                WeightUnit = item.WeightUnit,  // ✅ Map from DB
                Dimensions = item.Dimensions,  // ✅ Map from DB
                Color = item.Color,  // ✅ Map from DB
                Material = item.Material,  // ✅ Map from DB

                // Safety - ALL from DB
                IsHazardous = item.IsHazardous,  // ✅ Map from DB
                HazardClass = item.HazardClass,  // ✅ Map from DB

                // Media - ALL from DB
                ItemImage = item.ItemImage,  // ✅ Map from DB
                ImagePath = item.ImagePath,  // ✅ Map from DB
                Barcode = item.Barcode,  // ✅ Map from DB
                BarcodePath = item.BarcodePath,  // ✅ Map from DB
                QRCodeData = item.QRCodeData,  // ✅ Map from DB

                // From other tables
                Location = storeItem?.Location,  // From StoreItems
                CurrentStock = currentStock,  // Calculated

                // Master Catalogue fields
                CatalogueLedgerNo = item.CatalogueLedgerNo,
                CataloguePageNo = item.CataloguePageNo,
                CatalogueEntryDate = item.CatalogueEntryDate,
                FirstReceivedDate = item.FirstReceivedDate,
                CatalogueRemarks = item.CatalogueRemarks,

                // Audit
                CreatedAt = item.CreatedAt,
                CreatedBy = item.CreatedBy,
                UpdatedAt = item.UpdatedAt,
                UpdatedBy = item.UpdatedBy,
                IsActive = item.IsActive
            };
        }

        public async Task<ItemDto> CreateItemAsync(ItemDto itemDto)
        {
            // Validate ItemType
            if (!Enum.IsDefined(typeof(ItemType), itemDto.Type))
            {
                throw new InvalidOperationException("Invalid Item Type. Please select Expendable or NonExpendable.");
            }

            var item = new Item
            {
                ItemCode = await GenerateItemCodeAsync(itemDto.SubCategoryId),
                Name = itemDto.Name,
                Description = itemDto.Description,
                SubCategoryId = itemDto.SubCategoryId ?? 0,
                ItemModelId = itemDto.ItemModelId,
                Type = itemDto.Type,
                Unit = itemDto.Unit,
                MinimumStock = itemDto.MinimumStock,
                CreatedAt = DateTime.Now,
                CreatedBy = "System" // TODO: Get from current user
            };

            await _unitOfWork.Items.AddAsync(item);
            await _unitOfWork.CompleteAsync();

            itemDto.Id = item.Id;
            itemDto.ItemCode = item.ItemCode;
            return itemDto;
        }

        public async Task UpdateItemAsync(ItemDto itemDto)
        {
            // ✅ 1. Validation
            if (itemDto == null)
                throw new ArgumentNullException(nameof(itemDto), "Item data is required");

            if (string.IsNullOrWhiteSpace(itemDto.Name))
                throw new InvalidOperationException("Item name is required");

            if (itemDto.CategoryId <= 0)
                throw new InvalidOperationException("Valid category is required");

            if (itemDto.MinimumStock < 0)
                throw new InvalidOperationException("Minimum stock cannot be negative");

            if (itemDto.MaximumStock < itemDto.MinimumStock)
                throw new InvalidOperationException("Maximum stock cannot be less than minimum stock");

            if (itemDto.ReorderLevel < 0)
                throw new InvalidOperationException("Reorder level cannot be negative");

            if (!Enum.IsDefined(typeof(ItemType), itemDto.Type))
                throw new InvalidOperationException("Invalid Item Type. Please select Expendable or NonExpendable.");

            // ✅ 2. Fetch existing item
            var item = await _unitOfWork.Items.GetByIdAsync(itemDto.Id);
            if (item == null)
                throw new InvalidOperationException($"Item with ID {itemDto.Id} not found");

            // ✅ 3. Duplicate name check
            var duplicate = await _unitOfWork.Items.FirstOrDefaultAsync(
                i => i.Name == itemDto.Name && i.Id != itemDto.Id && i.IsActive);
            if (duplicate != null)
                throw new InvalidOperationException($"An item with name '{itemDto.Name}' already exists");

            // ✅ 4. Map DTO → Entity
            item.Name = itemDto.Name;
            item.NameBn = itemDto.NameBn;
            item.Description = itemDto.Description;
            item.Code = itemDto.Code;
            item.CategoryId = itemDto.CategoryId;
            item.SubCategoryId = itemDto.SubCategoryId ?? item.SubCategoryId;
            item.BrandId = itemDto.BrandId;
            item.ItemModelId = itemDto.ItemModelId;
            item.Type = itemDto.Type;
            item.Unit = itemDto.Unit;
            item.MinimumStock = itemDto.MinimumStock;
            item.MaximumStock = itemDto.MaximumStock ?? 0;
            item.ReorderLevel = itemDto.ReorderLevel;
            item.UnitPrice = itemDto.UnitPrice;
            item.UnitCost = itemDto.UnitCost;
            item.Manufacturer = itemDto.Manufacturer;
            item.ShelfLife = itemDto.ShelfLife;
            item.Status = itemDto.Status;

            // Dates & Special Handling
            item.ManufactureDate = itemDto.ManufactureDate;
            item.ExpiryDate = itemDto.ExpiryDate;
            item.HasExpiry = itemDto.HasExpiry;
            item.StorageRequirements = itemDto.StorageRequirements;
            item.RequiresSpecialHandling = itemDto.RequiresSpecialHandling;
            item.SafetyInstructions = itemDto.SafetyInstructions;

            // Physical Properties
            item.Weight = itemDto.Weight;
            item.WeightUnit = itemDto.WeightUnit;
            item.Dimensions = itemDto.Dimensions;
            item.Color = itemDto.Color;
            item.Material = itemDto.Material;
            item.IsHazardous = itemDto.IsHazardous;
            item.HazardClass = itemDto.HazardClass;

            // Media & Codes
            item.ItemImage = itemDto.ItemImage;
            item.ImagePath = itemDto.ImagePath;
            item.Barcode = itemDto.Barcode;
            item.BarcodePath = itemDto.BarcodePath;
            item.QRCodeData = itemDto.QRCodeData;

            // Master Catalogue fields
            item.CatalogueLedgerNo = itemDto.CatalogueLedgerNo;
            item.CataloguePageNo = itemDto.CataloguePageNo;
            item.CatalogueEntryDate = itemDto.CatalogueEntryDate;
            item.FirstReceivedDate = itemDto.FirstReceivedDate;
            item.CatalogueRemarks = itemDto.CatalogueRemarks;

            // Status & Audit
            item.IsActive = itemDto.IsActive;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = itemDto.UpdatedBy ?? "System";

            // ✅ 5. Save Changes
            _unitOfWork.Items.Update(item);
            await _unitOfWork.CompleteAsync();

            // ✅ 6. Log
            _logger.LogInformation(
                "Item {ItemId} '{ItemName}' updated successfully by {UpdatedBy}",
                item.Id, item.Name, item.UpdatedBy);
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(id);
            if (item != null)
            {
                item.IsActive = false;
                _unitOfWork.Items.Update(item);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<ItemDto>> GetLowStockItemsAsync()
        {
            var items = await GetAllItemsAsync();
            return items.Where(i => i.CurrentStock < i.MinimumStock);
        }

        public async Task<IEnumerable<ItemDto>> GetItemsByCategoryAsync(int categoryId)
        {
            var items = await GetAllItemsAsync();
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            var categoryName = category?.Name;

            return items.Where(i => i.CategoryName == categoryName);
        }

        public async Task<IEnumerable<ItemDto>> GetItemsBySubCategoryAsync(int subCategoryId)
        {
            var items = await GetAllItemsAsync();
            return items.Where(i => i.SubCategoryId == subCategoryId);
        }

        public async Task<IEnumerable<ItemDto>> GetItemsByStoreAsync(int storeId)
        {
            // Get all store items for this store that have stock
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.StoreId == storeId && si.Quantity > 0 && si.IsActive);
            var itemIds = storeItems.Select(si => si.ItemId).Distinct().ToList();

            // Get all items
            var allItems = await GetAllItemsAsync();

            // Filter items that exist in the store
            return allItems.Where(i => itemIds.Contains(i.Id));
        }

        public async Task<IEnumerable<ItemDto>> SearchItemsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllItemsAsync();

            var items = await GetAllItemsAsync();
            searchTerm = searchTerm.ToLower();

            return items.Where(i =>
                i.ItemCode.ToLower().Contains(searchTerm) ||
                i.Name.ToLower().Contains(searchTerm) ||
                (i.Description != null && i.Description.ToLower().Contains(searchTerm)) ||
                (i.CategoryName != null && i.CategoryName.ToLower().Contains(searchTerm)) ||
                (i.SubCategoryName != null && i.SubCategoryName.ToLower().Contains(searchTerm))
            );
        }

        public async Task<bool> ItemCodeExistsAsync(string itemCode, int? excludeId = null)
        {
            var items = await _unitOfWork.Items.FindAsync(i => i.ItemCode == itemCode && i.IsActive);
            if (excludeId.HasValue)
            {
                items = items.Where(i => i.Id != excludeId.Value);
            }
            return items.Any();
        }

        public async Task<decimal> GetTotalStockValueAsync()
        {
            var purchaseItems = await _unitOfWork.PurchaseItems.GetAllAsync();
            var storeItems = await _unitOfWork.StoreItems.GetAllAsync();

            decimal totalValue = 0;

            foreach (var storeItem in storeItems.Where(si => si.IsActive && si.Quantity > 0))
            {
                // Get the last purchase price for this item
                var lastPurchase = purchaseItems
                    .Where(pi => pi.ItemId == storeItem.ItemId)
                    .OrderByDescending(pi => pi.CreatedAt)
                    .FirstOrDefault();

                if (lastPurchase != null)
                {
                    totalValue += (decimal)(storeItem.Quantity * lastPurchase.UnitPrice);
                }
            }

            return totalValue;
        }

        public async Task<IEnumerable<ItemDto>> GetPagedItemsAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var allItems = string.IsNullOrWhiteSpace(searchTerm)
                ? await GetAllItemsAsync()
                : await SearchItemsAsync(searchTerm);

            return allItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task<IEnumerable<ItemDto>> GetExpendableItemsAsync()
        {
            var items = await GetAllItemsAsync();
            return items.Where(i => i.Type == Domain.Enums.ItemType.Expendable);
        }

        public async Task<IEnumerable<ItemDto>> GetNonExpendableItemsAsync()
        {
            var items = await GetAllItemsAsync();
            return items.Where(i => i.Type == Domain.Enums.ItemType.NonExpendable);
        }

        public async Task<ItemDto> GetItemByCodeAsync(string itemCode)
        {
            var item = await _unitOfWork.Items.FirstOrDefaultAsync(i => i.ItemCode == itemCode && i.IsActive);
            if (item == null) return null;

            return await GetItemByIdAsync(item.Id);
        }

        private async Task<decimal> CalculateCurrentStockAsync(int? itemId)
        {
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == itemId && si.IsActive);
            return (decimal)storeItems.Sum(si => si.Quantity);
        }

        public async Task<PagedResult<ItemDto>> GetPagedItemsAsync(PaginationParams paginationParams)
        {
            // Get all items first (you might want to optimize this with a more efficient query)
            var query = await _unitOfWork.Items.FindAsync(i => i.IsActive);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
            {
                var searchTerm = paginationParams.SearchTerm.ToLower();
                query = query.Where(i =>
                    i.Name.ToLower().Contains(searchTerm) ||
                    i.ItemCode.ToLower().Contains(searchTerm) ||
                    i.Description.ToLower().Contains(searchTerm));
            }

            // Get total count before pagination
            var totalCount = query.Count();

            // Apply sorting
            query = paginationParams.SortBy?.ToLower() switch
            {
                "name" => paginationParams.SortDescending ?
                    query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name),
                "itemcode" => paginationParams.SortDescending ?
                    query.OrderByDescending(i => i.ItemCode) : query.OrderBy(i => i.ItemCode),
                "category" => paginationParams.SortDescending ?
                    query.OrderByDescending(i => i.SubCategory.Category.Name) :
                    query.OrderBy(i => i.SubCategory.Category.Name),
                _ => query.OrderByDescending(i => i.CreatedAt) // Default sort
            };

            // Apply pagination
            var items = query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToList();

            // Map to DTOs
            var itemDtos = new List<ItemDto>();
            foreach (var item in items)
            {
                var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
                var category = subCategory != null ?
                    await _unitOfWork.Categories.GetByIdAsync(subCategory.CategoryId) : null;
                var model = item.ItemModelId.HasValue ?
                    await _unitOfWork.ItemModels.GetByIdAsync(item.ItemModelId.Value) : null;
                var brand = model?.Brand ?? (model?.BrandId != null ?
                    await _unitOfWork.Brands.GetByIdAsync(model.BrandId) : null);

                // Get current stock
                var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id);
                var currentStock = storeItems.Sum(si => si.Quantity);

                itemDtos.Add(new ItemDto
                {
                    Id = item.Id,
                    ItemCode = item.ItemCode,
                    Name = item.Name,
                    Description = item.Description,
                    SubCategoryId = item.SubCategoryId,
                    SubCategoryName = subCategory?.Name,
                    CategoryName = category?.Name,
                    ItemModelId = item.ItemModelId,
                    ModelName = model?.Name,
                    BrandName = brand?.Name,
                    Type = item.Type,
                    Unit = item.Unit,
                    MinimumStock = item.MinimumStock,
                    CurrentStock = currentStock,
                    CreatedAt = item.CreatedAt,
                    IsActive = item.IsActive
                });
            }

            return new PagedResult<ItemDto>(itemDtos, totalCount,
                paginationParams.PageNumber, paginationParams.PageSize);
        }

        public async Task<bool> CanDeleteItemAsync(int id)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(id);
            if (item == null) return false;

            // Check if item has any stock
            var hasStock = await _unitOfWork.StoreItems.ExistsAsync(si => si.ItemId == id && si.Quantity > 0);
            if (hasStock) return false;

            // Check if item has any transactions
            var hasIssues = await _unitOfWork.IssueItems.ExistsAsync(ii => ii.ItemId == id);
            if (hasIssues) return false;

            var hasPurchases = await _unitOfWork.PurchaseItems.ExistsAsync(pi => pi.ItemId == id);
            if (hasPurchases) return false;

            var hasTransfers = await _unitOfWork.TransferItems.ExistsAsync(ti => ti.ItemId == id);
            if (hasTransfers) return false;

            return true;
        }

        public async Task<PagedResult<ItemDto>> SearchItemsAsync(string searchTerm, int pageNumber = 1, int pageSize = 50)
        {
            Expression<Func<Item, bool>> predicate = i => i.IsActive;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                predicate = i => i.IsActive && (
                    i.Name.ToLower().Contains(searchTerm) ||
                    i.ItemCode.ToLower().Contains(searchTerm) ||
                    i.Description.ToLower().Contains(searchTerm)
                );
            }

            var totalCount = await _unitOfWork.Items.CountAsync(predicate);
            var items = await _unitOfWork.Items.GetPagedAsync(pageNumber, pageSize, predicate);

            var itemDtos = new List<ItemDto>();
            foreach (var item in items)
            {
                itemDtos.Add(await MapToItemDto(item));
            }

            return new PagedResult<ItemDto>(itemDtos, totalCount, pageNumber, pageSize);
        }

        private async Task<ItemDto> MapToItemDto(Item item)
        {
            var subCategory = await _unitOfWork.SubCategories.GetByIdAsync(item.SubCategoryId);
            var category = subCategory != null ? await _unitOfWork.SubCategories.GetByIdAsync(subCategory.CategoryId) : null;

            // Handle nullable BrandId
            Brand brand = null;
            if (item.BrandId.HasValue)
            {
                brand = await _unitOfWork.Brands.GetByIdAsync(item.BrandId.Value);
            }

            var model = await _unitOfWork.ItemModels.GetByIdAsync(item.ItemModelId);

            // Get current stock across all stores
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == item.Id && si.IsActive);
            var currentStock = storeItems.Sum(si => si.Quantity);

            return new ItemDto
            {
                Id = item.Id,
                ItemCode = item.ItemCode,
                Name = item.Name,
                Description = item.Description,
                CategoryId = category?.Id ?? 0,
                CategoryName = category?.Name,
                SubCategoryId = item.SubCategoryId,
                SubCategoryName = subCategory?.Name,
                BrandId = item.BrandId,
                BrandName = brand?.Name,
                ItemModelId = item.ItemModelId,
                ItemModelName = model?.Name,
                Type = item.Type,
                Status = item.Status,
                MinimumStock = item.MinimumStock,
                ReorderLevel = item.ReorderLevel,
                Unit = item.Unit,
                Weight = item.Weight,
                ItemImage = item.ItemImage,
                CurrentStock = currentStock,
                CreatedAt = item.CreatedAt,
                IsActive = item.IsActive
            };
        }

        public async Task<ItemDetailDto> GetItemDetailAsync(int id)
        {
            var item = await _unitOfWork.Items.GetByIdAsync(id);
            if (item == null) return null;

            var itemDto = await MapToItemDto(item);
            if (itemDto == null) return null;

            // Calculate current stock from StoreItems
            var storeItems = await _unitOfWork.StoreItems.FindAsync(si => si.ItemId == id && si.IsActive);
            var currentStock = storeItems.Sum(si => si.Quantity);

            var itemDetail = new ItemDetailDto
            {
                // Copy all properties from ItemDto
                Id = itemDto.Id,
                ItemCode = itemDto.ItemCode,
                Name = itemDto.Name,
                NameBn = item.NameBn,
                Description = itemDto.Description,
                CategoryId = itemDto.CategoryId,
                CategoryName = itemDto.CategoryName,
                SubCategoryId = itemDto.SubCategoryId,
                SubCategoryName = itemDto.SubCategoryName,
                BrandId = itemDto.BrandId,
                BrandName = itemDto.BrandName,
                ItemModelId = itemDto.ItemModelId,
                ItemModelName = itemDto.ItemModelName,
                Type = itemDto.Type,
                Status = itemDto.Status,
                MinimumStock = itemDto.MinimumStock,
                MaximumStock = item.MaximumStock,
                ReorderLevel = itemDto.ReorderLevel,
                Unit = itemDto.Unit,
                Weight = itemDto.Weight,
                WeightUnit = item.WeightUnit,
                ItemImage = itemDto.ItemImage,
                ImagePath = item.ImagePath,
                CreatedAt = itemDto.CreatedAt,
                CreatedBy = item.CreatedBy,
                UpdatedAt = item.UpdatedAt,
                UpdatedBy = item.UpdatedBy,
                IsActive = itemDto.IsActive,

                // ✅ ADD MISSING CRITICAL FIELDS
                CurrentStock = currentStock,
                UnitPrice = item.UnitPrice,
                UnitCost = item.UnitCost,
                Manufacturer = item.Manufacturer,
                ManufactureDate = item.ManufactureDate,
                ExpiryDate = item.ExpiryDate,
                HasExpiry = item.HasExpiry,
                ShelfLife = item.ShelfLife,
                Material = item.Material,
                Color = item.Color,
                Dimensions = item.Dimensions,
                IsHazardous = item.IsHazardous,
                HazardClass = item.HazardClass,
                StorageRequirements = item.StorageRequirements,
                RequiresSpecialHandling = item.RequiresSpecialHandling,
                SafetyInstructions = item.SafetyInstructions,
                Barcode = item.Barcode,
                BarcodePath = item.BarcodePath,
                QRCodeData = item.QRCodeData,
                Code = item.Code
            };

            // Get store stocks (already fetched above for CurrentStock calculation)
            itemDetail.StoreStocks = new List<StoreStockDto>();

            foreach (var storeItem in storeItems)
            {
                var store = await _unitOfWork.Stores.GetByIdAsync(storeItem.StoreId);
                itemDetail.StoreStocks.Add(new StoreStockDto
                {
                    StoreId = storeItem.StoreId,
                    StoreName = store?.Name ?? "Unknown",
                    Quantity = storeItem.Quantity,
                    LastUpdated = storeItem.UpdatedAt ?? storeItem.CreatedAt
                });
            }

            // Get purchase history
            var purchaseItems = await _unitOfWork.PurchaseItems.FindAsync(pi => pi.ItemId == id);
            itemDetail.PurchaseHistory = new List<PurchaseHistoryDto>();

            foreach (var pi in purchaseItems.Take(10))
            {
                var purchase = await _unitOfWork.Purchases.GetByIdAsync(pi.PurchaseId);
                var vendor = purchase != null ? await _unitOfWork.Vendors.GetByIdAsync(purchase.VendorId) : null;
                var store = await _unitOfWork.Stores.GetByIdAsync(pi.StoreId);

                itemDetail.PurchaseHistory.Add(new PurchaseHistoryDto
                {
                    PurchaseId = pi.PurchaseId,
                    PurchaseOrderNo = purchase?.PurchaseOrderNo ?? "Unknown",
                    PurchaseDate = purchase?.PurchaseDate ?? DateTime.MinValue,
                    VendorName = vendor?.Name ?? "Unknown",
                    Quantity = pi.Quantity,
                    UnitPrice = pi.UnitPrice,
                    TotalPrice = pi.TotalPrice,
                    StoreName = store?.Name ?? "Unknown"
                });
            }

            // Get issue history
            var issueItems = await _unitOfWork.IssueItems.FindAsync(ii => ii.ItemId == id);
            itemDetail.IssueHistory = new List<IssueHistoryDto>();

            foreach (var ii in issueItems.Take(10))
            {
                var issue = await _unitOfWork.Issues.GetByIdAsync(ii.IssueId);
                var store = await _unitOfWork.Stores.GetByIdAsync(ii.StoreId);

                itemDetail.IssueHistory.Add(new IssueHistoryDto
                {
                    IssueId = ii.IssueId,
                    IssueNo = issue?.IssueNo ?? "Unknown",
                    IssueDate = issue?.IssueDate ?? DateTime.MinValue,
                    IssuedTo = issue?.IssuedTo ?? "Unknown",
                    IssuedToType = issue?.IssuedToType ?? "Unknown",
                    Quantity = ii.Quantity,
                    StoreName = store?.Name ?? "Unknown",
                    Purpose = issue?.Purpose ?? "Unknown"
                });
            }

            // Calculate totals
            itemDetail.TotalPurchased = purchaseItems.Sum(pi => pi.Quantity);
            itemDetail.TotalIssued = issueItems.Sum(ii => ii.Quantity);
            itemDetail.LastPurchaseDate = itemDetail.PurchaseHistory.Any()
                ? itemDetail.PurchaseHistory.Max(ph => ph.PurchaseDate)
                : (DateTime?)null;
            itemDetail.LastIssueDate = itemDetail.IssueHistory.Any()
                ? itemDetail.IssueHistory.Max(ih => ih.IssueDate)
                : (DateTime?)null;

            // Get barcodes
            var barcodes = await _unitOfWork.Barcodes.FindAsync(b => b.ItemId == id && b.IsActive);
            itemDetail.Barcodes = barcodes.Select(b => new BarcodeDto
            {
                Id = b.Id,
                BarcodeNumber = b.BarcodeNumber,
                ItemId = b.ItemId,
                SerialNumber = b.SerialNumber,
                GeneratedDate = b.GeneratedDate,
                IsActive = b.IsActive
            }).ToList();

            return itemDetail;
        }
        public async Task<decimal> GetStoreItemQuantityAsync(int? storeId, int itemId)
        {
            try
            {
                var storeItem = await _unitOfWork.StoreItems.FirstOrDefaultAsync(si =>
                    si.StoreId == storeId &&
                    si.ItemId == itemId &&
                    si.IsActive);

                return storeItem?.Quantity ?? 0;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error getting store item quantity");
                return 0;
            }
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description,
                IsActive = c.IsActive
            });
        }

        public async Task<int> GetTotalItemsCount()
        {
            return await _unitOfWork.Items.CountAsync(i => i.IsActive);
        }

        public async Task<int> GetTotalStoresCount()
        {
            return await _unitOfWork.Stores.CountAsync(s => s.IsActive);
        }

        public async Task<List<ExpiryTrackingDto>> GetExpiringItems(int daysToExpiry)
        {
            var expiryDate = DateTime.Now.AddDays(daysToExpiry);

            // If StoreItems doesn't have ExpiryDate, return empty list
            // Otherwise, implement the logic
            return new List<ExpiryTrackingDto>();
        }

        public async Task<List<CategoryStockDto>> GetCategoryStocks()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(c => c.IsActive);
            var categoryStocks = new List<CategoryStockDto>();

            foreach (var category in categories)
            {
                categoryStocks.Add(new CategoryStockDto
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    ItemCount = 10, // Placeholder
                    TotalStock = 100, // Placeholder
                    TotalValue = 1000 // Placeholder
                });
            }

            return categoryStocks;
        }

        public async Task<List<MonthlyTrendDto>> GetItemDistributionByCategory()
        {
            return new List<MonthlyTrendDto>
            {
                
            };
        }

        public Task<bool> GetItemByBarcodeAsync(string barcode)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ItemDto>> GetCriticalStockItemsAsync()
        {
            var items = await GetLowStockItemsAsync();
            return items.Where(i => i.CurrentStock <= i.MinimumStock * 0.2m).ToList();
        }

        public async Task<int> GetItemCountByCategoryAsync(int categoryId)
        {
            var items = await GetItemsByCategoryAsync(categoryId);
            return items.Count();
        }

        public async Task<int> GetLowStockCountAsync()
        {
            var items = await GetLowStockItemsAsync();
            return items.Count();
        }
        public async Task<IEnumerable<ItemDto>> GetActiveItemsAsync()
        {
            var items = await _unitOfWork.Items
                .GetAllAsync(i => i.IsActive,
                    includes: new[] { "SubCategory.Category", "Brand", "ItemModel" });

            return items.Select(i => new ItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Code = i.Code ?? i.ItemCode,
                ItemCode = i.ItemCode ?? i.Code,
                Unit = i.Unit ?? "Piece",
                CategoryId = i.SubCategory.CategoryId,
                CategoryName = i.SubCategory?.Category?.Name,
                SubCategoryId = i.SubCategoryId,
                SubCategoryName = i.SubCategory?.Name,
                BrandId = i.BrandId,
                BrandName = i.Brand?.Name,
                MinimumStock = i.MinimumStock,
                MaximumStock = i.MaximumStock,
                IsActive = i.IsActive
            }).OrderBy(i => i.Name);
        }
        public async Task<SelectList> GetControlledItemsSelectListAsync()
        {
            var items = await _unitOfWork.Items
                .Query()
                .Where(i => i.IsActive && i.ItemControlType == "Controlled")
                .OrderBy(i => i.Name)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Name} ({i.ItemCode})"
                })
                .ToListAsync();

            return new SelectList(items, "Value", "Text");
        }

        public async Task<int> BulkUpdateItemsBanglaAndStockAsync()
        {
            try
            {
                // Common English to Bangla item name mappings
                var nameTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    // Uniforms & Clothing
                    { "Uniform", "ইউনিফর্ম" },
                    { "Shirt", "শার্ট" },
                    { "Pant", "প্যান্ট" },
                    { "Boot", "বুট" },
                    { "Shoes", "জুতা" },
                    { "Belt", "বেল্ট" },
                    { "Cap", "ক্যাপ" },
                    { "Jacket", "জ্যাকেট" },
                    { "Raincoat", "রেইনকোট" },
                    { "Sweater", "সোয়েটার" },

                    // Stationery
                    { "Pen", "কলম" },
                    { "Pencil", "পেন্সিল" },
                    { "Paper", "কাগজ" },
                    { "Notebook", "খাতা" },
                    { "File", "ফাইল" },
                    { "Folder", "ফোল্ডার" },
                    { "Stapler", "স্ট্যাপলার" },
                    { "Clip", "ক্লিপ" },
                    { "Envelope", "খাম" },
                    { "Stamp Pad", "স্ট্যাম্প প্যাড" },

                    // Electronics
                    { "Computer", "কম্পিউটার" },
                    { "Laptop", "ল্যাপটপ" },
                    { "Printer", "প্রিন্টার" },
                    { "Scanner", "স্ক্যানার" },
                    { "Mouse", "মাউস" },
                    { "Keyboard", "কীবোর্ড" },
                    { "Monitor", "মনিটর" },
                    { "Cable", "ক্যাবল" },

                    // Furniture
                    { "Table", "টেবিল" },
                    { "Chair", "চেয়ার" },
                    { "Desk", "ডেস্ক" },
                    { "Cabinet", "ক্যাবিনেট" },
                    { "Shelf", "সেলফ" },
                    { "Almirah", "আলমারি" },

                    // Weapons & Security
                    { "Rifle", "রাইফেল" },
                    { "Pistol", "পিস্তল" },
                    { "Baton", "লাঠি" },
                    { "Handcuff", "হাতকড়া" },
                    { "Whistle", "হুইসেল" },
                    { "Torch", "টর্চ" },
                    { "Flashlight", "টর্চ লাইট" },

                    // Medical
                    { "First Aid Box", "প্রাথমিক চিকিৎসা বাক্স" },
                    { "Bandage", "ব্যান্ডেজ" },
                    { "Medicine", "ঔষধ" },
                    { "Thermometer", "থার্মোমিটার" },

                    // Miscellaneous
                    { "Flag", "পতাকা" },
                    { "Badge", "ব্যাজ" },
                    { "ID Card", "পরিচয়পত্র" },
                    { "Book", "বই" },
                    { "Register", "রেজিস্টার" },
                    { "Blanket", "কম্বল" },
                    { "Pillow", "বালিশ" },
                    { "Mosquito Net", "মশারি" }
                };

                // Fetch all items
                var allItems = await _unitOfWork.Items
                    .Query()
                    .Where(i => i.IsActive)
                    .ToListAsync();

                int updatedCount = 0;

                foreach (var item in allItems)
                {
                    bool updated = false;

                    // Update Bangla name if empty or not set
                    if (string.IsNullOrWhiteSpace(item.NameBn))
                    {
                        // Try exact match first
                        if (nameTranslations.TryGetValue(item.Name, out string banglaName))
                        {
                            item.NameBn = banglaName;
                            updated = true;
                        }
                        else
                        {
                            // Try to find partial match
                            foreach (var translation in nameTranslations)
                            {
                                if (item.Name.Contains(translation.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    item.NameBn = translation.Value;
                                    updated = true;
                                    break;
                                }
                            }
                        }
                    }

                    // Update stock values if not set properly
                    if (item.MinimumStock == null || item.MinimumStock == 0)
                    {
                        // Set reasonable defaults based on item type or category
                        item.MinimumStock = 10;
                        updated = true;
                    }

                    if (item.MaximumStock == null || item.MaximumStock == 0)
                    {
                        item.MaximumStock = 1000;
                        updated = true;
                    }

                    if (item.ReorderLevel == 0)
                    {
                        item.ReorderLevel = item.MinimumStock.HasValue ? item.MinimumStock.Value * 2 : 20;
                        updated = true;
                    }

                    if (updated)
                    {
                        _unitOfWork.Items.Update(item);
                        updatedCount++;
                    }
                }

                // Save all changes
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Bulk updated {updatedCount} items with Bangla names and stock values");

                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk update of items");
                throw;
            }
        }

    }
}