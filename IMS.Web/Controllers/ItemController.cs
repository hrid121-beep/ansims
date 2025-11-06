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
            if (id != itemDto.Id)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBagData(itemDto);
                    TempData["Error"] = "Please check the form and fix any errors.";
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

                // Create SelectList for ItemType enum - use enum values directly for proper asp-for binding
                var itemTypes = Enum.GetValues(typeof(ItemType))
                    .Cast<ItemType>()
                    .Select(e => new SelectListItem
                    {
                        Value = ((int)e).ToString(), // Convert enum to int, then to string for HTML value
                        Text = e.ToString(),
                        Selected = currentItem != null && currentItem.Type == e
                    })
                    .ToList();
                ViewBag.ItemTypes = itemTypes;

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
    }
}