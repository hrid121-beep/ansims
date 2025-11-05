using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly IItemModelService _itemModelService;

        public BrandController(IBrandService brandService, IItemModelService itemModelService)
        {
            _brandService = brandService;
            _itemModelService = itemModelService;
        }

        [HasPermission(Permission.ViewBrand)]
        public async Task<IActionResult> Index()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return View(brands);
        }

        [HasPermission(Permission.CreateBrand)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateBrand)]
        public async Task<IActionResult> Create(BrandDto brandDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if brand already exists
                    var exists = await _brandService.BrandExistsAsync(brandDto.Name);
                    if (exists)
                    {
                        ModelState.AddModelError("Name", "A brand with this name already exists.");
                    }
                    else
                    {
                        await _brandService.CreateBrandAsync(brandDto);
                        TempData["Success"] = $"Brand '{brandDto.Name}' created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to create brand. " + ex.Message;
            }

            return View(brandDto);
        }

        [HasPermission(Permission.UpdateBrand)]
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
            {
                TempData["Error"] = "Brand not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateBrand)]
        public async Task<IActionResult> Edit(int id, BrandDto brandDto)
        {
            if (id != brandDto.Id)
            {
                TempData["Error"] = "Invalid brand ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Check if brand name already exists (excluding current)
                    var exists = await _brandService.BrandExistsAsync(brandDto.Name, brandDto.Id);
                    if (exists)
                    {
                        ModelState.AddModelError("Name", "A brand with this name already exists.");
                    }
                    else
                    {
                        await _brandService.UpdateBrandAsync(brandDto);
                        TempData["Success"] = $"Brand '{brandDto.Name}' updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to update brand. " + ex.Message;
            }

            return View(brandDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteBrand)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var brand = await _brandService.GetBrandByIdAsync(id);
                if (brand == null)
                {
                    TempData["Error"] = "Brand not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if brand has any models
                var modelCount = await _brandService.GetBrandItemCountAsync(id);
                if (modelCount > 0)
                {
                    TempData["Error"] = $"Cannot delete brand '{brand.Name}'. It has {modelCount} associated models.";
                }
                else
                {
                    await _brandService.DeleteBrandAsync(id);
                    TempData["Success"] = $"Brand '{brand.Name}' deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to delete brand. " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> CheckBrandExists(string name, int? excludeId = null)
        {
            var exists = await _brandService.BrandExistsAsync(name, excludeId);
            return Json(new { exists });
        }

        [HttpGet]
        public async Task<JsonResult> GetBrandModels(int brandId)
        {
            var models = await _itemModelService.GetItemModelsByBrandIdAsync(brandId);
            return Json(models.Select(m => new {
                id = m.Id,
                name = m.Name,
                modelNumber = m.ModelNumber,
                itemCount = m.ItemCount
            }));
        }

        [HttpGet]
        public async Task<JsonResult> GetBrandStatistics(int brandId)
        {
            var brand = await _brandService.GetBrandByIdAsync(brandId);
            if (brand == null)
            {
                return Json(new { error = "Brand not found" });
            }

            var modelCount = brand.ModelCount;
            var itemCount = await _brandService.GetBrandItemCountAsync(brandId);

            return Json(new
            {
                brandName = brand.Name,
                modelCount,
                itemCount,
                canDelete = modelCount == 0
            });
        }
    }
}