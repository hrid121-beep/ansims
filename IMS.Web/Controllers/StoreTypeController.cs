using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StoreTypeController : Controller
    {
        private readonly IStoreTypeService _storeTypeService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<StoreTypeController> _logger;

        public StoreTypeController(
            IStoreTypeService storeTypeService,
            ICategoryService categoryService,
            ILogger<StoreTypeController> logger)
        {
            _storeTypeService = storeTypeService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [HasPermission(Permission.ViewStore)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var storeTypes = await _storeTypeService.GetAllStoreTypesAsync();
                return View(storeTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading store types");
                TempData["Error"] = "An error occurred while loading store types.";
                return View(new List<StoreTypeDto>());
            }
        }

        [HasPermission(Permission.CreateStore)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(new StoreTypeDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateStore)]
        public async Task<IActionResult> Create(StoreTypeDto storeTypeDto, List<int> selectedCategories)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    storeTypeDto.CreatedBy = User.Identity.Name;
                    var createdType = await _storeTypeService.CreateStoreTypeAsync(storeTypeDto);

                    if (selectedCategories?.Any() == true)
                    {
                        await _storeTypeService.AssignCategoriesToStoreTypeAsync(createdType.Id, selectedCategories);
                    }

                    TempData["Success"] = "Store type created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store type");
                ModelState.AddModelError("", "An error occurred while creating the store type.");
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(storeTypeDto);
        }

        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var storeType = await _storeTypeService.GetStoreTypeByIdAsync(id);
                if (storeType == null)
                {
                    TempData["Error"] = "Store type not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(storeType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading store type for edit");
                TempData["Error"] = "An error occurred while loading the store type.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> Edit(int id, StoreTypeDto storeTypeDto, List<int> selectedCategories)
        {
            if (id != storeTypeDto.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    storeTypeDto.UpdatedBy = User.Identity.Name;
                    await _storeTypeService.UpdateStoreTypeAsync(storeTypeDto);

                    if (selectedCategories != null)
                    {
                        await _storeTypeService.AssignCategoriesToStoreTypeAsync(storeTypeDto.Id, selectedCategories);
                    }

                    TempData["Success"] = "Store type updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store type");
                ModelState.AddModelError("", "An error occurred while updating the store type.");
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(storeTypeDto);
        }

        [HasPermission(Permission.ViewStore)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var storeType = await _storeTypeService.GetStoreTypeByIdAsync(id);
                if (storeType == null)
                {
                    TempData["Error"] = "Store type not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(storeType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading store type details");
                TempData["Error"] = "An error occurred while loading store type details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteStore)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _storeTypeService.DeleteStoreTypeAsync(id);
                TempData["Success"] = "Store type deleted successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting store type");
                TempData["Error"] = "An error occurred while deleting the store type.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
