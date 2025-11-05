using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;

        public CategoryController(ICategoryService categoryService, ISubCategoryService subCategoryService)
        {
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
        }

        [HasPermission(Permission.ViewCategory)]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(id);
            ViewBag.SubCategories = subCategories;

            return View(category);
        }

        [HasPermission(Permission.CreateCategory)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateCategory)]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryService.CreateCategoryAsync(categoryDto);
                    TempData["Success"] = "Category created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to create category. " + ex.Message;
            }

            return View(categoryDto);
        }

        [HasPermission(Permission.UpdateCategory)]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateCategory)]
        public async Task<IActionResult> Edit(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                TempData["Error"] = "Invalid category data.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryService.UpdateCategoryAsync(categoryDto);
                    TempData["Success"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to update category. " + ex.Message;
            }

            return View(categoryDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteCategory)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["Success"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to delete category. " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // API endpoint for getting subcategories by category
        [HttpGet]
        public async Task<JsonResult> GetSubCategories(int categoryId)
        {
            try
            {
                var subCategories = await _subCategoryService.GetSubCategoriesByCategoryIdAsync(categoryId);
                return Json(subCategories.Select(s => new { id = s.Id, name = s.Name }));
            }
            catch (Exception)
            {
                return Json(new List<object>());
            }
        }

        // Bulk actions endpoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> BulkAction(string action, int[] ids)
        {
            try
            {
                if (ids == null || ids.Length == 0)
                {
                    return Json(new { success = false, message = "No categories selected." });
                }

                switch (action.ToLower())
                {
                    case "activate":
                        // Implementation depends on your CategoryService having bulk operations
                        // For now, iterate through each
                        foreach (var id in ids)
                        {
                            var category = await _categoryService.GetCategoryByIdAsync(id);
                            if (category != null)
                            {
                                category.IsActive = true;
                                await _categoryService.UpdateCategoryAsync(category);
                            }
                        }
                        return Json(new { success = true, message = $"{ids.Length} categories activated successfully." });

                    case "deactivate":
                        foreach (var id in ids)
                        {
                            var category = await _categoryService.GetCategoryByIdAsync(id);
                            if (category != null)
                            {
                                category.IsActive = false;
                                await _categoryService.UpdateCategoryAsync(category);
                            }
                        }
                        return Json(new { success = true, message = $"{ids.Length} categories deactivated successfully." });

                    case "delete":
                        foreach (var id in ids)
                        {
                            await _categoryService.DeleteCategoryAsync(id);
                        }
                        return Json(new { success = true, message = $"{ids.Length} categories deleted successfully." });

                    default:
                        return Json(new { success = false, message = "Invalid action." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to perform bulk action: " + ex.Message });
            }
        }
    }
}