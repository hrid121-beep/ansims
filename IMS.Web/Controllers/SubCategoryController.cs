// SubCategoryController.cs
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
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly ICategoryService _categoryService;

        public SubCategoryController(
            ISubCategoryService subCategoryService,
            ICategoryService categoryService)
        {
            _subCategoryService = subCategoryService;
            _categoryService = categoryService;
        }

        [HasPermission(Permission.ViewSubCategory)]
        public async Task<IActionResult> Index()
        {
            var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
            return View(subCategories);
        }

        [HasPermission(Permission.CreateSubCategory)]
        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateSubCategory)]
        public async Task<IActionResult> Create(SubCategoryDto subCategoryDto)
        {
            if (ModelState.IsValid)
            {
                await _subCategoryService.CreateSubCategoryAsync(subCategoryDto);
                TempData["Success"] = "Sub-category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            await LoadViewBagData();
            return View(subCategoryDto);
        }

        [HasPermission(Permission.UpdateSubCategory)]
        public async Task<IActionResult> Edit(int id)
        {
            var subCategory = await _subCategoryService.GetSubCategoryByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            await LoadViewBagData();
            return View(subCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateSubCategory)]
        public async Task<IActionResult> Edit(int id, SubCategoryDto subCategoryDto)
        {
            if (id != subCategoryDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _subCategoryService.UpdateSubCategoryAsync(subCategoryDto);
                TempData["Success"] = "Sub-category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            await LoadViewBagData();
            return View(subCategoryDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteSubCategory)]
        public async Task<IActionResult> Delete(int id)
        {
            await _subCategoryService.DeleteSubCategoryAsync(id);
            TempData["Success"] = "Sub-category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetSubCategoriesByCategory(int categoryId)
        {
            var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
            return Json(subCategories.Select(s => new { value = s.Id, text = s.Name }));
        }

        private async Task LoadViewBagData()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
    }
}