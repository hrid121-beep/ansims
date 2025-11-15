using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ISubCategoryService subCategoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _logger = logger;
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

        // ==================== EXPORT OPERATIONS ====================

        [HttpGet]
        [HasPermission(Permission.ViewCategory)]
        public async Task<IActionResult> ExportToCsv(string status = null)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        categories = categories.Where(c => c.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        categories = categories.Where(c => !c.IsActive);
                }

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Code,Name,Description,Item Count,Status,Created Date");

                foreach (var category in categories)
                {
                    csv.AppendLine($"\"{EscapeCsv(category.Code)}\"," +
                        $"\"{EscapeCsv(category.Name)}\"," +
                        $"\"{EscapeCsv(category.Description)}\"," +
                        $"{category.ItemCount}," +
                        $"\"{(category.IsActive ? "Active" : "Inactive")}\"," +
                        $"\"{category.CreatedAt:dd-MMM-yyyy}\"");
                }

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"Categories_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting categories to CSV");
                TempData["Error"] = "Error exporting data to CSV.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewCategory)]
        public async Task<IActionResult> ExportToPdf(string status = null)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        categories = categories.Where(c => c.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        categories = categories.Where(c => !c.IsActive);
                }

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30);
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18);
                    var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

                    var titleParagraph = new iTextSharp.text.Paragraph("ANSAR & VDP - Categories Report", titleFont);
                    titleParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    titleParagraph.SpacingAfter = 10f;
                    document.Add(titleParagraph);

                    var infoParagraph = new iTextSharp.text.Paragraph($"Report Generated: {DateTime.Now:dd-MMM-yyyy HH:mm} | Total: {categories.Count()}", normalFont);
                    infoParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    infoParagraph.SpacingAfter = 15f;
                    document.Add(infoParagraph);

                    var mainTable = new iTextSharp.text.pdf.PdfPTable(5);
                    mainTable.WidthPercentage = 100;
                    mainTable.SetWidths(new float[] { 15f, 25f, 35f, 12f, 13f });

                    var headerTexts = new[] { "Code", "Name", "Description", "Items", "Status" };
                    foreach (var headerText in headerTexts)
                    {
                        var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(headerText, headerFont));
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(220, 220, 220);
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.Padding = 5f;
                        mainTable.AddCell(cell);
                    }

                    foreach (var category in categories)
                    {
                        mainTable.AddCell(new iTextSharp.text.Phrase(category.Code ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(category.Name ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(category.Description ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(category.ItemCount.ToString(), normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(category.IsActive ? "Active" : "Inactive", normalFont));
                    }

                    document.Add(mainTable);

                    var footerParagraph = new iTextSharp.text.Paragraph($"\nGenerated by: IMS System | Date: {DateTime.Now:dd-MMM-yyyy HH:mm}",
                        iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8));
                    footerParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    footerParagraph.SpacingBefore = 20f;
                    document.Add(footerParagraph);

                    document.Close();
                    return File(memoryStream.ToArray(), "application/pdf", $"Categories_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting categories to PDF");
                TempData["Error"] = "Error exporting data to PDF.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains("\"")) value = value.Replace("\"", "\"\"");
            return value;
        }
    }
}