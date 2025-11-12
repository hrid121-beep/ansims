using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class UnionController : Controller
    {
        private readonly IUnionService _unionService;
        private readonly IUpazilaService _upazilaService;
        private readonly IZilaService _zilaService;
        private readonly IRangeService _rangeService;
        private readonly IStoreService _storeService;
        private readonly ILogger<UnionController> _logger;

        public UnionController(
            IUnionService unionService,
            IUpazilaService upazilaService,
            IZilaService zilaService,
            IRangeService rangeService,
            IStoreService storeService,
            ILogger<UnionController> logger)
        {
            _unionService = unionService;
            _upazilaService = upazilaService;
            _zilaService = zilaService;
            _rangeService = rangeService;
            _storeService = storeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? upazilaId = null, string filter = null)
        {
            try
            {
                IEnumerable<UnionDto> unions;

                if (upazilaId.HasValue)
                {
                    unions = await _unionService.GetUnionsByUpazilaAsync(upazilaId.Value);
                    var upazila = await _upazilaService.GetUpazilaByIdAsync(upazilaId.Value);
                    ViewBag.UpazilaId = upazilaId;
                    ViewBag.UpazilaName = upazila?.Name;
                    ViewBag.ZilaName = upazila?.ZilaName;
                }
                else
                {
                    switch (filter?.ToLower())
                    {
                        case "vdp":
                            unions = await _unionService.GetUnionsWithVDPUnitsAsync();
                            ViewBag.Filter = "VDP Units";
                            break;
                        case "border":
                            unions = await _unionService.GetBorderAreaUnionsAsync();
                            ViewBag.Filter = "Border Areas";
                            break;
                        case "rural":
                            unions = await _unionService.GetRuralUnionsAsync();
                            ViewBag.Filter = "Rural";
                            break;
                        case "urban":
                            unions = await _unionService.GetUrbanUnionsAsync();
                            ViewBag.Filter = "Urban";
                            break;
                        default:
                            unions = await _unionService.GetAllUnionsAsync();
                            break;
                    }
                }

                // Load filter data
                ViewBag.Upazilas = await _upazilaService.GetActiveUpazilasAsync();
                ViewBag.Zilas = await _zilaService.GetActiveZilasAsync();

                return View(unions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading unions");
                TempData["Error"] = "An error occurred while loading unions.";
                return View(new List<UnionDto>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var union = await _unionService.GetUnionByIdAsync(id);
                if (union == null)
                {
                    TempData["Error"] = "Union not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Load optional data - don't fail if these throw exceptions
                try
                {
                    ViewBag.Statistics = await _unionService.GetUnionStatisticsAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error loading union statistics for ID {Id}", id);
                    ViewBag.Statistics = null;
                }

                try
                {
                    ViewBag.Stores = await _storeService.GetStoresByUnionAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error loading stores for union ID {Id}", id);
                    ViewBag.Stores = new List<IMS.Application.DTOs.StoreDto>();
                }

                try
                {
                    ViewBag.DashboardData = await _unionService.GetUnionDashboardDataAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error loading dashboard data for union ID {Id}", id);
                    ViewBag.DashboardData = new Dictionary<string, object>();
                }

                return View(union);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading union details for ID {Id}", id);
                TempData["Error"] = $"An error occurred while loading union details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create(int? upazilaId = null)
        {
            await LoadUpazilaSelectList(upazilaId);

            var model = new UnionDto();
            if (upazilaId.HasValue)
            {
                model.UpazilaId = upazilaId.Value;
                model.Code = await _unionService.GenerateUnionCodeAsync(upazilaId.Value);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnionDto unionDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unionDto.CreatedBy = User.Identity.Name;
                    await _unionService.CreateUnionAsync(unionDto);
                    TempData["Success"] = "Union created successfully!";
                    return RedirectToAction(nameof(Index), new { upazilaId = unionDto.UpazilaId });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating union");
                ModelState.AddModelError("", "An error occurred while creating the union.");
            }

            await LoadUpazilaSelectList(unionDto.UpazilaId);
            return View(unionDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var union = await _unionService.GetUnionByIdAsync(id);
                if (union == null)
                {
                    TempData["Error"] = "Union not found.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadUpazilaSelectList(union.UpazilaId);
                return View(union);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading union for edit");
                TempData["Error"] = "An error occurred while loading the union.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UnionDto unionDto)
        {
            if (id != unionDto.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    unionDto.CreatedBy = User.Identity.Name; // Used as UpdatedBy
                    await _unionService.UpdateUnionAsync(unionDto);
                    TempData["Success"] = "Union updated successfully!";
                    return RedirectToAction(nameof(Index), new { upazilaId = unionDto.UpazilaId });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating union");
                ModelState.AddModelError("", "An error occurred while updating the union.");
            }

            await LoadUpazilaSelectList(unionDto.UpazilaId);
            return View(unionDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var union = await _unionService.GetUnionByIdAsync(id);
                await _unionService.DeleteUnionAsync(id);
                TempData["Success"] = "Union deleted successfully!";

                return RedirectToAction(nameof(Index), new { upazilaId = union?.UpazilaId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting union");
                TempData["Error"] = "An error occurred while deleting the union.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search()
        {
            await LoadSearchFilters();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(UnionSearchDto searchDto)
        {
            try
            {
                var unions = await _unionService.SearchUnionsAsync(searchDto);
                ViewBag.SearchResults = true;
                await LoadSearchFilters();
                return View("Index", unions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching unions");
                TempData["Error"] = "An error occurred while searching unions.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a CSV file to import.");
                return View();
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Please select a valid CSV file.");
                return View();
            }

            try
            {
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                await _unionService.ImportUnionsFromCsvAsync(filePath);

                // Clean up temp file
                System.IO.File.Delete(filePath);

                TempData["Success"] = "Unions imported successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing unions");
                ModelState.AddModelError("", "An error occurred while importing unions. Please check the CSV format.");
                return View();
            }
        }

        public async Task<IActionResult> Export(int? upazilaId = null)
        {
            try
            {
                byte[] csvData;
                string fileName;

                if (upazilaId.HasValue)
                {
                    csvData = await _unionService.ExportUnionsByUpazilaAsync(upazilaId.Value);
                    var upazila = await _upazilaService.GetUpazilaByIdAsync(upazilaId.Value);
                    fileName = $"Unions_{upazila?.Name}_{DateTime.Now:yyyyMMdd}.csv";
                }
                else
                {
                    csvData = await _unionService.ExportUnionsAsync();
                    fileName = $"Unions_{DateTime.Now:yyyyMMdd}.csv";
                }

                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting unions");
                TempData["Error"] = "An error occurred while exporting unions.";
                return RedirectToAction(nameof(Index));
            }
        }

        // VDP Management
        public async Task<IActionResult> VDPManagement(int id)
        {
            try
            {
                var union = await _unionService.GetUnionByIdAsync(id);
                if (union == null)
                {
                    TempData["Error"] = "Union not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(union);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading VDP management");
                TempData["Error"] = "An error occurred while loading VDP management.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVDPCount(int id, int maleCount, int femaleCount)
        {
            try
            {
                await _unionService.UpdateVDPMemberCountAsync(id, maleCount, femaleCount);
                TempData["Success"] = "VDP member count updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating VDP count");
                TempData["Error"] = "An error occurred while updating VDP member count.";
                return RedirectToAction(nameof(VDPManagement), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAnsarCount(int id, int count)
        {
            try
            {
                await _unionService.UpdateAnsarMemberCountAsync(id, count);
                TempData["Success"] = "Ansar member count updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Ansar count");
                TempData["Error"] = "An error occurred while updating Ansar member count.";
                return RedirectToAction(nameof(VDPManagement), new { id });
            }
        }

        // AJAX endpoints
        [HttpGet]
        public async Task<IActionResult> GetUnionsByUpazila(int upazilaId)
        {
            try
            {
                var unions = await _unionService.GetUnionsByUpazilaAsync(upazilaId);
                return Json(unions.Select(u => new { value = u.Id, text = u.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unions by upazila");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnionsByZila(int zilaId)
        {
            try
            {
                var unions = await _unionService.GetUnionsByZilaAsync(zilaId);
                return Json(unions.Select(u => new { value = u.Id, text = u.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unions by zila");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code, int? excludeId = null)
        {
            var exists = await _unionService.UnionCodeExistsAsync(code, excludeId);
            return Json(!exists);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCode(int upazilaId)
        {
            try
            {
                var code = await _unionService.GenerateUnionCodeAsync(upazilaId);
                return Json(new { success = true, code });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating union code");
                return Json(new { success = false, message = "Error generating code" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnionHierarchy(int id)
        {
            try
            {
                var hierarchy = await _unionService.GetUnionHierarchyAsync(id);
                return Json(hierarchy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting union hierarchy");
                return Json(null);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnionStatistics(int id)
        {
            try
            {
                var statistics = await _unionService.GetUnionStatisticsAsync(id);
                return Json(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting union statistics");
                return Json(null);
            }
        }

        // Private helper methods
        private async Task LoadUpazilaSelectList(int? selectedUpazilaId = null)
        {
            var upazilas = await _upazilaService.GetActiveUpazilasAsync();
            ViewBag.Upazilas = new SelectList(upazilas, "Id", "Name", selectedUpazilaId);

            // Also load zilas for cascading dropdown
            var zilas = await _zilaService.GetActiveZilasAsync();
            ViewBag.Zilas = new SelectList(zilas, "Id", "Name");

            // Load ranges for reference
            var ranges = await _rangeService.GetActiveRangesAsync();
            ViewBag.Ranges = ranges.ToDictionary(r => r.Id, r => r.Name);
        }

        private async Task LoadSearchFilters()
        {
            ViewBag.Upazilas = await _upazilaService.GetActiveUpazilasAsync();
            ViewBag.Zilas = await _zilaService.GetActiveZilasAsync();
            ViewBag.Ranges = await _rangeService.GetActiveRangesAsync();
        }
    }
}