using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IBrandService _brandService;
        private readonly IItemModelService _itemModelService;
        private readonly IBarcodeService _barcodeService;
        private readonly IFileService _fileService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(
            IItemService itemService,
            ICategoryService categoryService,
            ISubCategoryService subCategoryService,
            IBrandService brandService,
            IItemModelService itemModelService,
            IBarcodeService barcodeService,
            IFileService fileService,
            ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _brandService = brandService;
            _itemModelService = itemModelService;
            _barcodeService = barcodeService;
            _fileService = fileService;
            _logger = logger;
        }

        [HasPermission(Permission.ViewItem)]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var pagedItems = await _itemService.GetPagedItemsAsync(pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;

                // ADD THESE LINES FOR FILTERS
                var categories = await _categoryService.GetAllCategoriesAsync();
                var brands = await _brandService.GetAllBrandsAsync();
                ViewBag.FilterCategories = categories.Select(c => c.Name).Distinct().ToList();
                ViewBag.FilterBrands = brands.Select(b => b.Name).Distinct().ToList();

                return View(pagedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading items");
                TempData["Error"] = "An error occurred while loading items.";
                return View(new PagedResult<ItemDto>
                {
                    Items = new List<ItemDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });
            }
        }
        [HasPermission(Permission.ViewItem)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var item = await _itemService.GetItemDetailAsync(id);
                if (item == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading item details for id: {Id}", id);
                TempData["Error"] = "An error occurred while loading item details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HasPermission(Permission.CreateItem)]
        public async Task<IActionResult> Create()
        {
            try
            {
                await LoadViewBagData();
                var model = new ItemDto
                {
                    ItemCode = await _itemService.GenerateItemCodeAsync(0) // Generate default code
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create item form");
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }


        // OLD EDIT METHOD - REPLACED BY NEW IMPLEMENTATION BELOW (Line 592+)
        // [HasPermission(Permission.UpdateItem)]
        // public async Task<IActionResult> Edit(int id)
        // {
        //     try
        //     {
        //         var item = await _itemService.GetItemByIdAsync(id);
        //         if (item == null)
        //         {
        //             TempData["Error"] = "Item not found.";
        //             return RedirectToAction(nameof(Index));
        //         }
        //         await LoadViewBagData(item);
        //         return View(item);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error loading edit form for item id: {Id}", id);
        //         TempData["Error"] = "An error occurred while loading the item.";
        //         return RedirectToAction(nameof(Index));
        //     }
        // }

        // OLD EDIT POST METHOD - REPLACED BY NEW IMPLEMENTATION BELOW (Line ~617+)
        // Renamed to EditOld to avoid conflicts - can be deleted after testing
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateItem)]
        public async Task<IActionResult> EditOld(int id, ItemDto itemDto, IFormFile ItemImageFile)
        {
            if (id != itemDto.Id)
            {
                return NotFound();
            }

            try
            {
                // Get existing item to preserve values
                var existingItem = await _itemService.GetItemByIdAsync(id);
                if (existingItem == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Preserve SubCategoryId if not provided (0 or null)
                if (!itemDto.SubCategoryId.HasValue || itemDto.SubCategoryId.Value == 0)
                {
                    itemDto.SubCategoryId = existingItem.SubCategoryId;
                    ModelState.Remove("SubCategoryId");
                }

                // Preserve CategoryId if not provided
                if (itemDto.CategoryId == 0)
                {
                    itemDto.CategoryId = existingItem.CategoryId;
                    ModelState.Remove("CategoryId");
                }

                // Preserve BrandId if not provided (optional field)
                if (!itemDto.BrandId.HasValue || itemDto.BrandId.Value == 0)
                {
                    itemDto.BrandId = existingItem.BrandId;
                    ModelState.Remove("BrandId");
                }

                // Preserve ItemModelId if not provided (optional field)
                if (!itemDto.ItemModelId.HasValue || itemDto.ItemModelId.Value == 0)
                {
                    itemDto.ItemModelId = existingItem.ItemModelId;
                    ModelState.Remove("ItemModelId");
                }

                // Preserve Type if Type is 0
                if ((int)itemDto.Type == 0)
                {
                    itemDto.Type = existingItem.Type;
                    ModelState.Remove("Type");
                }

                // Preserve Status if Status is 0 (invalid enum value)
                if ((int)itemDto.Status == 0)
                {
                    itemDto.Status = existingItem.Status;
                    ModelState.Remove("Status");
                }

                // Preserve ImagePath if empty (not uploading new image)
                if (string.IsNullOrWhiteSpace(itemDto.ImagePath))
                {
                    itemDto.ImagePath = existingItem.ImagePath;
                }

                // Preserve other optional string fields if empty
                // Note: Barcode has internal set, so can't be set here - it's auto-generated

                if (string.IsNullOrWhiteSpace(itemDto.BarcodePath))
                {
                    itemDto.BarcodePath = existingItem.BarcodePath;
                }

                if (string.IsNullOrWhiteSpace(itemDto.QRCodeData))
                {
                    itemDto.QRCodeData = existingItem.QRCodeData;
                }

                if (string.IsNullOrWhiteSpace(itemDto.ItemImage))
                {
                    itemDto.ItemImage = existingItem.ItemImage;
                }

                // Debug logging - Log ALL properties
                _logger.LogInformation("=== EDIT ITEM DEBUG - Item ID: {ItemId} ===", id);
                _logger.LogInformation("CategoryId: {CategoryId}, SubCategoryId: {SubCategoryId}", itemDto.CategoryId, itemDto.SubCategoryId);
                _logger.LogInformation("BrandId: {BrandId}, ItemModelId: {ItemModelId}", itemDto.BrandId, itemDto.ItemModelId);
                _logger.LogInformation("Type: {Type} (int value: {TypeInt})", itemDto.Type, (int)itemDto.Type);
                _logger.LogInformation("Unit: {Unit}, MinimumStock: {MinStock}", itemDto.Unit, itemDto.MinimumStock);
                _logger.LogInformation("Name: {Name}, Status: {Status} (int value: {StatusInt})", itemDto.Name, itemDto.Status, (int)itemDto.Status);

                // Log ModelState errors BEFORE validation
                _logger.LogInformation("ModelState.IsValid: {IsValid}, Error Count: {ErrorCount}",
                    ModelState.IsValid, ModelState.ErrorCount);

                if (ModelState.ErrorCount > 0)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var state = ModelState[key];
                        if (state.Errors.Count > 0)
                        {
                            _logger.LogWarning("Field '{Field}' has errors: {Errors}, AttemptedValue: '{AttemptedValue}'",
                                key,
                                string.Join("; ", state.Errors.Select(e => e.ErrorMessage)),
                                state.AttemptedValue);
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    // Log validation errors for debugging
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new {
                            Field = x.Key,
                            Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList(),
                            AttemptedValue = x.Value.AttemptedValue
                        })
                        .ToList();

                    _logger.LogError("Item Edit validation failed for ID {ItemId}. Errors: {@ValidationErrors}", id, errors);

                    // Show detailed error to user - use ; separator instead of \n to avoid JavaScript issues
                    var errorMessage = "Validation errors: " + string.Join("; ", errors.Select(e => $"{e.Field}: {string.Join(", ", e.Errors)}"));
                    TempData["Error"] = errorMessage;

                    await LoadViewBagData(itemDto);
                    return View(itemDto);
                }

                // Additional validation
                if (string.IsNullOrWhiteSpace(itemDto.Name))
                {
                    ModelState.AddModelError("Name", "Item name is required");
                    await LoadViewBagData(itemDto);
                    return View(itemDto);
                }

                if (itemDto.MinimumStock < 0)
                {
                    ModelState.AddModelError("MinimumStock", "Minimum stock cannot be negative");
                    await LoadViewBagData(itemDto);
                    return View(itemDto);
                }

                // Handle image upload
                if (ItemImageFile != null && ItemImageFile.Length > 0)
                {
                    // Validate image file
                    if (!_fileService.IsValidImageFile(ItemImageFile))
                    {
                        ModelState.AddModelError("", "Invalid image file. Please upload a valid image (JPG, PNG, GIF)");
                        await LoadViewBagData(itemDto);
                        TempData["Error"] = "Invalid image file format.";
                        return View(itemDto);
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(itemDto.ImagePath))
                    {
                        await _fileService.DeleteFileAsync(itemDto.ImagePath);
                    }

                    // Save new image
                    itemDto.ImagePath = await _fileService.SaveFileAsync(ItemImageFile, "items");
                    _logger.LogInformation("Item image uploaded: {ImagePath} for item {ItemId}", itemDto.ImagePath, id);
                }

                // Set update audit fields
                itemDto.UpdatedAt = DateTime.Now;
                itemDto.UpdatedBy = User.Identity?.Name;

                await _itemService.UpdateItemAsync(itemDto);

                TempData["Success"] = "Item updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Validation error updating item with id: {Id}", id);
                ModelState.AddModelError("", ex.Message);
                await LoadViewBagData(itemDto);
                TempData["Error"] = ex.Message;
                return View(itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item with id: {Id}", id);
                TempData["Error"] = "An error occurred while updating the item. Please try again.";
                await LoadViewBagData(itemDto);
                return View(itemDto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteItem)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var canDelete = await _itemService.CanDeleteItemAsync(id);
                if (!canDelete)
                {
                    TempData["Error"] = "Cannot delete item. It may have associated transactions or stock.";
                    return RedirectToAction(nameof(Index));
                }

                await _itemService.DeleteItemAsync(id);
                TempData["Success"] = "Item deleted successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item with id: {Id}", id);
                TempData["Error"] = "An error occurred while deleting the item.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> GenerateBarcode(int id, int quantity = 1)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                if (quantity < 1 || quantity > 100)
                {
                    return BadRequest("Quantity must be between 1 and 100");
                }

                var barcodes = await _barcodeService.GenerateBatchBarcodesAsync(id, quantity);
                var pdf = await _barcodeService.GenerateBatchBarcodePDF(barcodes);

                return File(pdf, "application/pdf", $"{item.ItemCode}_barcodes_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating barcode for item id: {Id}", id);
                TempData["Error"] = "An error occurred while generating barcode.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpGet]
        [HasPermission(Permission.GenerateItemCode)]
        public async Task<JsonResult> GenerateItemCode(int subCategoryId)
        {
            try
            {
                var itemCode = await _itemService.GenerateItemCodeAsync(subCategoryId);
                return Json(new { success = true, itemCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating item code for subcategory id: {SubCategoryId}", subCategoryId);
                return Json(new { success = false, message = "Failed to generate item code" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                ViewBag.SearchTerm = searchTerm;
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return RedirectToAction(nameof(Index), new { pageNumber, pageSize });
                }

                var searchResult = await _itemService.SearchItemsAsync(searchTerm, pageNumber, pageSize);
                return View("Index", searchResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching items with term: {SearchTerm}", searchTerm);
                TempData["Error"] = "An error occurred while searching items.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSubCategories(int categoryId)
        {
            try
            {
                var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);

                // Ensure proper JSON format
                var result = subCategories.Select(sc => new
                {
                    id = sc.Id,
                    name = sc.Name
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading subcategories for category id: {CategoryId}", categoryId);
                return Json(new List<object>()); // Return empty list instead of error
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetItemModels(int brandId)
        {
            try
            {
                var itemModels = await _itemModelService.GetItemModelsByBrandIdAsync(brandId);

                // Ensure proper JSON format
                var result = itemModels.Select(im => new
                {
                    id = im.Id,
                    name = im.Name
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading item models for brand id: {BrandId}", brandId);
                return Json(new List<object>()); // Return empty list instead of error
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateItem)]
        public async Task<IActionResult> Create(ItemDto itemDto)
        {
            try
            {
                // For AJAX requests
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    if (!ModelState.IsValid)
                    {
                        var errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .Select(x => new { x.Key, x.Value.Errors })
                            .ToList();

                        return Json(new { success = false, message = "Validation failed", errors });
                    }

                    // Validation
                    if (string.IsNullOrWhiteSpace(itemDto.Name))
                    {
                        return Json(new { success = false, message = "Item name is required" });
                    }

                    if (itemDto.MinimumStock < 0)
                    {
                        return Json(new { success = false, message = "Minimum stock cannot be negative" });
                    }

                    // Generate item code if not provided
                    if (string.IsNullOrWhiteSpace(itemDto.ItemCode))
                    {
                        itemDto.ItemCode = await _itemService.GenerateItemCodeAsync(itemDto.SubCategoryId);
                    }

                    // Set default values
                    itemDto.CurrentStock = 0; // New items start with 0 stock
                    itemDto.CreatedAt = DateTime.Now;
                    itemDto.CreatedBy = User.Identity.Name;

                    await _itemService.CreateItemAsync(itemDto);

                    return Json(new { success = true, message = "Item created successfully!" });
                }

                // For regular form submission
                if (!ModelState.IsValid)
                {
                    await LoadViewBagData(itemDto);
                    return View(itemDto);
                }

                // Generate item code if not provided
                if (string.IsNullOrWhiteSpace(itemDto.ItemCode))
                {
                    itemDto.ItemCode = await _itemService.GenerateItemCodeAsync(itemDto.SubCategoryId);
                }

                await _itemService.CreateItemAsync(itemDto);
                TempData["Success"] = "Item created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }

                ModelState.AddModelError("", ex.Message);
                await LoadViewBagData(itemDto);
                return View(itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "An error occurred while creating the item" });
                }

                TempData["Error"] = "An error occurred while creating the item.";
                await LoadViewBagData(itemDto);
                return View(itemDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubCategoriesByCategory(int categoryId)
        {
            var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
            return Json(subCategories.Select(s => new { id = s.Id, name = s.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetModelsByBrand(int brandId)
        {
            var models = await _itemModelService.GetItemModelsByBrandIdAsync(brandId);
            return Json(models.Select(m => new { id = m.Id, name = m.Name }));
        }

        // ==================== NEW CLEAN EDIT IMPLEMENTATION ====================

        [HasPermission(Permission.UpdateItem)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                if (item == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadViewBagData(item);
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for item id: {Id}", id);
                TempData["Error"] = "An error occurred while loading the item.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateItem)]
        public async Task<IActionResult> Edit(int id, ItemDto itemDto, IFormFile ItemImageFile)
        {
            _logger.LogInformation("=== EDIT - Starting edit for Item ID: {ItemId} ===", id);

            if (id != itemDto.Id)
            {
                _logger.LogWarning("ID mismatch: URL id={UrlId}, DTO id={DtoId}", id, itemDto.Id);
                return NotFound();
            }

            try
            {
                // Get existing item
                var existingItem = await _itemService.GetItemByIdAsync(id);
                if (existingItem == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation("Existing item loaded: {ItemName}, CategoryId={CategoryId}",
                    existingItem.Name, existingItem.CategoryId);

                // Handle image upload FIRST
                if (ItemImageFile != null && ItemImageFile.Length > 0)
                {
                    _logger.LogInformation("Image file uploaded: {FileName}, Size: {Size}",
                        ItemImageFile.FileName, ItemImageFile.Length);

                    if (!_fileService.IsValidImageFile(ItemImageFile))
                    {
                        ModelState.AddModelError("", "Invalid image file. Please upload JPG, PNG, or GIF.");
                        TempData["Error"] = "Invalid image file format.";
                        await LoadViewBagData(itemDto);
                        return View(itemDto);
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingItem.ImagePath))
                    {
                        await _fileService.DeleteFileAsync(existingItem.ImagePath);
                        _logger.LogInformation("Deleted old image: {OldImage}", existingItem.ImagePath);
                    }

                    // Save new image
                    itemDto.ImagePath = await _fileService.SaveFileAsync(ItemImageFile, "items");
                    _logger.LogInformation("New image saved: {NewImage}", itemDto.ImagePath);
                }
                else
                {
                    // Keep existing image path
                    itemDto.ImagePath = existingItem.ImagePath;
                }

                // Preserve fields that should not change
                itemDto.CurrentStock = existingItem.CurrentStock;
                itemDto.CreatedAt = existingItem.CreatedAt;
                itemDto.CreatedBy = existingItem.CreatedBy;
                itemDto.ItemCode = existingItem.ItemCode;

                // Preserve barcode-related fields (auto-generated)
                itemDto.BarcodePath = existingItem.BarcodePath;
                itemDto.QRCodeData = existingItem.QRCodeData;
                itemDto.ItemImage = existingItem.ItemImage;

                // Set update info
                itemDto.UpdatedAt = DateTime.Now;
                itemDto.UpdatedBy = User.Identity?.Name;

                // Log what we're about to save
                _logger.LogInformation("Updating item: Name={Name}, CategoryId={CategoryId}, Type={Type}, Status={Status}",
                    itemDto.Name, itemDto.CategoryId, itemDto.Type, itemDto.Status);

                // Validate ModelState
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new {
                            Field = x.Key,
                            Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        })
                        .ToList();

                    _logger.LogWarning("Validation failed: {@Errors}", errors);

                    TempData["Error"] = "Validation failed: " +
                        string.Join("; ", errors.Select(e => $"{e.Field}: {string.Join(", ", e.Errors)}"));

                    await LoadViewBagData(itemDto);
                    return View(itemDto);
                }

                // Update the item
                await _itemService.UpdateItemAsync(itemDto);
                _logger.LogInformation("Item {ItemId} updated successfully", id);

                TempData["Success"] = "Item updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Validation error updating item {ItemId}", id);
                TempData["Error"] = ex.Message;
                await LoadViewBagData(itemDto);
                return View(itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item {ItemId}", id);
                TempData["Error"] = "An error occurred while updating the item. Please try again.";
                await LoadViewBagData(itemDto);
                return View(itemDto);
            }
        }

        private async Task LoadViewBagData(ItemDto currentItem = null)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                var brands = await _brandService.GetAllBrandsAsync();

                // Debug the CategoryId
                if (currentItem != null)
                {
                    Console.WriteLine($"LoadViewBagData - CategoryId: {currentItem.CategoryId}");
                }

                ViewBag.Categories = new SelectList(categories, "Id", "Name", currentItem?.CategoryId);
                ViewBag.Brands = new SelectList(brands, "Id", "Name", currentItem?.BrandId);

                // Create SelectList for ItemType enum - properly configure for asp-for binding
                var itemTypesData = Enum.GetValues(typeof(ItemType))
                    .Cast<ItemType>()
                    .Select(e => new
                    {
                        Value = (int)e,
                        Text = e.ToString()
                    })
                    .ToList();

                // Determine selected value - handle invalid Type values by defaulting to Expendable
                int selectedType = currentItem != null && Enum.IsDefined(typeof(ItemType), currentItem.Type)
                    ? (int)currentItem.Type
                    : (int)ItemType.Expendable;

                // Debug logging
                if (currentItem != null)
                {
                    _logger.LogInformation("Item Type: {Type} (raw value: {RawValue}), Selected: {Selected}",
                        currentItem.Type, (int)currentItem.Type, selectedType);
                }

                ViewBag.ItemTypes = new SelectList(itemTypesData, "Value", "Text", selectedType);

                // Load subcategories if CategoryId exists
                if (currentItem?.CategoryId > 0)
                {
                    var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(currentItem.CategoryId);
                    ViewBag.SubCategories = new SelectList(subCategories, "Id", "Name", currentItem.SubCategoryId);
                }
                else
                {
                    ViewBag.SubCategories = new SelectList(new List<SubCategoryDto>(), "Id", "Name");
                }

                // Load models if BrandId exists
                if (currentItem?.BrandId > 0)
                {
                    var itemModels = await _itemModelService.GetItemModelsByBrandIdAsync(currentItem.BrandId.Value);
                    ViewBag.ItemModels = new SelectList(itemModels, "Id", "Name", currentItem.ItemModelId);
                }
                else
                {
                    ViewBag.ItemModels = new SelectList(new List<ItemModelDto>(), "Id", "Name");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading view bag data");
                // Provide empty lists to prevent view errors
                ViewBag.Categories = new SelectList(new List<object>());
                ViewBag.SubCategories = new SelectList(new List<object>());
                ViewBag.Brands = new SelectList(new List<object>());
                ViewBag.ItemModels = new SelectList(new List<object>());
                ViewBag.ItemTypes = new SelectList(new List<object>());
            }
        }

        /// <summary>
        /// Get all items for Receive modal (not store-dependent)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllItemsForReceive(string searchTerm = "")
        {
            try
            {
                var items = await _itemService.GetAllItemsAsync();

                // Filter by search term if provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var search = searchTerm.ToLower();
                    items = items.Where(i =>
                        (i.ItemCode != null && i.ItemCode.ToLower().Contains(search)) ||
                        (i.Code != null && i.Code.ToLower().Contains(search)) ||
                        (i.Name != null && i.Name.ToLower().Contains(search)) ||
                        (i.CategoryName != null && i.CategoryName.ToLower().Contains(search)) ||
                        (i.Barcode != null && i.Barcode.ToLower().Contains(search))
                    ).ToList();
                }

                // Map to the format expected by the modal
                var result = items.Select(i => new
                {
                    itemId = i.Id,
                    itemCode = i.ItemCode ?? i.Code ?? "",
                    code = i.ItemCode ?? i.Code ?? "",
                    itemName = i.Name ?? "",
                    name = i.Name ?? "",
                    categoryName = i.CategoryName ?? "N/A",
                    quantity = 0, // No stock info for receive
                    currentStock = 0,
                    minimumStock = i.MinimumStock ?? 0,
                    unit = i.Unit ?? "Piece",
                    barcode = i.Barcode ?? "",
                    isLowStock = false,
                    isOutOfStock = false
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all items for receive");
                return Json(new List<object>());
            }
        }

        // ==================== EXPORT OPERATIONS ====================

        [HttpGet]
        [HasPermission(Permission.ViewItem)]
        public async Task<IActionResult> ExportToCsv(string category = null, string brand = null, string status = null)
        {
            try
            {
                var items = await _itemService.GetAllItemsAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(category))
                {
                    items = items.Where(i => i.CategoryName != null && i.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(brand))
                {
                    items = items.Where(i => i.BrandName != null && i.BrandName.Equals(brand, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    {
                        items = items.Where(i => i.IsActive);
                    }
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                    {
                        items = items.Where(i => !i.IsActive);
                    }
                }

                var csv = new System.Text.StringBuilder();

                // Add headers
                csv.AppendLine("#,Item Code,Name,Category,Sub Category,Brand,Model,Unit,Unit Price,Min Stock,Max Stock,Reorder Level,Status");

                // Add data
                int serialNo = 1;
                foreach (var item in items)
                {
                    csv.AppendLine($"{serialNo}," +
                        $"\"{EscapeCsv(item.ItemCode ?? item.Code)}\"," +
                        $"\"{EscapeCsv(item.Name)}\"," +
                        $"\"{EscapeCsv(item.CategoryName)}\"," +
                        $"\"{EscapeCsv(item.SubCategoryName)}\"," +
                        $"\"{EscapeCsv(item.BrandName)}\"," +
                        $"\"{EscapeCsv(item.ModelName)}\"," +
                        $"\"{EscapeCsv(item.Unit)}\"," +
                        $"{item.UnitPrice ?? 0m}," +
                        $"{item.MinimumStock ?? 0m}," +
                        $"{item.MaximumStock ?? 0m}," +
                        $"{item.ReorderLevel}," +
                        $"\"{(item.IsActive ? "Active" : "Inactive")}\"");
                    serialNo++;
                }

                var fileContent = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"Items_{DateTime.Now:yyyyMMddHHmmss}.csv";
                return File(fileContent, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting items to CSV");
                TempData["Error"] = "Error exporting data to CSV.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewItem)]
        public async Task<IActionResult> ExportToPdf(string category = null, string brand = null, string status = null)
        {
            try
            {
                var items = await _itemService.GetAllItemsAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(category))
                {
                    items = items.Where(i => i.CategoryName != null && i.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(brand))
                {
                    items = items.Where(i => i.BrandName != null && i.BrandName.Equals(brand, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    {
                        items = items.Where(i => i.IsActive);
                    }
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                    {
                        items = items.Where(i => !i.IsActive);
                    }
                }

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Define fonts
                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18);
                    var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

                    // Add title
                    var titleParagraph = new iTextSharp.text.Paragraph("ANSAR & VDP - Items Report", titleFont);
                    titleParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    titleParagraph.SpacingAfter = 10f;
                    document.Add(titleParagraph);

                    // Add report info
                    var infoParagraph = new iTextSharp.text.Paragraph($"Report Generated: {DateTime.Now:dd-MMM-yyyy HH:mm} | Total Items: {items.Count()}", normalFont);
                    infoParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    infoParagraph.SpacingAfter = 15f;
                    document.Add(infoParagraph);

                    // Create table
                    var mainTable = new iTextSharp.text.pdf.PdfPTable(10);
                    mainTable.WidthPercentage = 100;
                    mainTable.SetWidths(new float[] { 5f, 10f, 15f, 12f, 12f, 10f, 8f, 8f, 8f, 8f });

                    // Add headers
                    var headerTexts = new[] { "#", "Code", "Name", "Category", "Brand", "Model", "Unit", "Price", "Min Stock", "Status" };
                    foreach (var headerText in headerTexts)
                    {
                        var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(headerText, headerFont));
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(220, 220, 220);
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.Padding = 5f;
                        mainTable.AddCell(cell);
                    }

                    // Add data
                    int serialNo = 1;
                    foreach (var item in items)
                    {
                        mainTable.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(item.ItemCode ?? item.Code ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(item.Name ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(item.CategoryName ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(item.BrandName ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(item.ModelName ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(item.Unit ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase($"৳{(item.UnitPrice ?? 0m):N2}", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase((item.MinimumStock ?? 0m).ToString(), normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(item.IsActive ? "Active" : "Inactive", normalFont));
                        serialNo++;
                    }

                    document.Add(mainTable);

                    // Footer
                    var footerParagraph = new iTextSharp.text.Paragraph($"\nGenerated by: IMS System | Date: {DateTime.Now:dd-MMM-yyyy HH:mm}",
                        iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8));
                    footerParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    footerParagraph.SpacingBefore = 20f;
                    document.Add(footerParagraph);

                    document.Close();

                    var fileContent = memoryStream.ToArray();
                    var fileName = $"Items_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    return File(fileContent, "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting items to PDF");
                TempData["Error"] = "Error exporting data to PDF.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains("\""))
                value = value.Replace("\"", "\"\"");

            return value;
        }
    }
}
