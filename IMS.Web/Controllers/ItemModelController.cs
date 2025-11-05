using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class ItemModelController : Controller
    {
        private readonly IItemModelService _itemModelService;
        private readonly IBrandService _brandService;

        public ItemModelController(
            IItemModelService itemModelService,
            IBrandService brandService)
        {
            _itemModelService = itemModelService;
            _brandService = brandService;
        }

        [HasPermission(Permission.ViewItemModel)]
        public async Task<IActionResult> Index()
        {
            var models = await _itemModelService.GetAllItemModelsAsync();
            return View(models);
        }

        [HasPermission(Permission.CreateItemModel)]
        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateItemModel)]
        public async Task<IActionResult> Create(ItemModelDto itemModelDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if model already exists
                    var exists = await _itemModelService.ItemModelExistsAsync(
                        itemModelDto.Name,
                        itemModelDto.ModelNumber,
                        itemModelDto.BrandId);

                    if (exists)
                    {
                        ModelState.AddModelError("", "A model with this name and number already exists for this brand.");
                    }
                    else
                    {
                        await _itemModelService.CreateItemModelAsync(itemModelDto);
                        TempData["Success"] = "Model created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to create model. " + ex.Message;
            }

            await LoadViewBagData();
            return View(itemModelDto);
        }

        [HasPermission(Permission.UpdateItemModel)]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _itemModelService.GetItemModelByIdAsync(id);
            if (model == null)
            {
                TempData["Error"] = "Model not found.";
                return RedirectToAction(nameof(Index));
            }

            await LoadViewBagData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateItemModel)]
        public async Task<IActionResult> Edit(int id, ItemModelDto itemModelDto)
        {
            if (id != itemModelDto.Id)
            {
                TempData["Error"] = "Invalid model ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Check if model already exists (excluding current)
                    var exists = await _itemModelService.ItemModelExistsAsync(
                        itemModelDto.Name,
                        itemModelDto.ModelNumber,
                        itemModelDto.BrandId,
                        itemModelDto.Id);

                    if (exists)
                    {
                        ModelState.AddModelError("", "A model with this name and number already exists for this brand.");
                    }
                    else
                    {
                        await _itemModelService.UpdateItemModelAsync(itemModelDto);
                        TempData["Success"] = "Model updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to update model. " + ex.Message;
            }

            await LoadViewBagData();
            return View(itemModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteItemModel)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if model has any items
                var itemCount = await _itemModelService.GetItemModelItemCountAsync(id);
                if (itemCount > 0)
                {
                    TempData["Error"] = $"Cannot delete model. It has {itemCount} associated items.";
                }
                else
                {
                    await _itemModelService.DeleteItemModelAsync(id);
                    TempData["Success"] = "Model deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to delete model. " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetModelsByBrand(int brandId)
        {
            var models = await _itemModelService.GetItemModelsByBrandIdAsync(brandId);
            return Json(models.Select(m => new { value = m.Id, text = m.Name }));
        }

        private async Task LoadViewBagData()
        {
            var brands = await _brandService.GetActiveBrandsAsync();
            ViewBag.Brands = new SelectList(brands, "Id", "Name");
        }
    }
}