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
    public class UpazilaController : Controller
    {
        private readonly IUpazilaService _upazilaService;
        private readonly IZilaService _zilaService;
        private readonly IRangeService _rangeService;
        private readonly IStoreService _storeService;
        private readonly ILogger<UpazilaController> _logger;

        public UpazilaController(
            IUpazilaService upazilaService,
            IZilaService zilaService,
            IRangeService rangeService,
            IStoreService storeService,
            ILogger<UpazilaController> logger)
        {
            _upazilaService = upazilaService ?? throw new ArgumentNullException(nameof(upazilaService));
            _zilaService = zilaService ?? throw new ArgumentNullException(nameof(zilaService));
            _rangeService = rangeService ?? throw new ArgumentNullException(nameof(rangeService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index(int? zilaId = null)
        {
            try
            {
                IEnumerable<UpazilaDto> upazilas;

                if (zilaId.HasValue)
                {
                    upazilas = await _upazilaService.GetUpazilasByZilaAsync(zilaId.Value);
                    var zila = await _zilaService.GetZilaByIdAsync(zilaId.Value);
                    ViewBag.ZilaId = zilaId;
                    ViewBag.ZilaName = zila?.Name;
                }
                else
                {
                    upazilas = await _upazilaService.GetAllUpazilasAsync();
                }

                await LoadZilaSelectList(); // For filter dropdown
                return View(upazilas ?? new List<UpazilaDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading upazilas");
                TempData["Error"] = "An error occurred while loading sub-districts. Please try again or contact support.";
                return View(new List<UpazilaDto>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var upazila = await _upazilaService.GetUpazilaByIdAsync(id);
                if (upazila == null)
                {
                    TempData["Error"] = "The sub-district you're looking for was not found. It may have been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Statistics = await _upazilaService.GetUpazilaStatisticsAsync(id);
                ViewBag.Stores = await _storeService.GetStoresByUpazilaAsync(id) ?? new List<StoreDto>();

                return View(upazila);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading upazila details for ID {Id}", id);
                TempData["Error"] = "An error occurred while loading sub-district details. Please try again or contact support.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create(int? zilaId = null)
        {
            try
            {
                await LoadZilaSelectList(zilaId);

                var model = new UpazilaDto
                {
                    IsActive = true,
                    Code = string.Empty // Will be auto-generated
                };

                if (zilaId.HasValue)
                {
                    model.ZilaId = zilaId.Value;
                    // Auto-generate code when zila is pre-selected
                    try
                    {
                        model.Code = await _upazilaService.GenerateUpazilaCodeAsync(zilaId.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not auto-generate code for zila {ZilaId}", zilaId.Value);
                        // Don't fail the page load, just leave code empty
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create view");
                TempData["Error"] = "Unable to load the create form. Please try again.";
                return RedirectToAction(nameof(Index), new { zilaId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UpazilaDto upazilaDto)
        {
            try
            {
                // Auto-generate code if empty
                if (string.IsNullOrWhiteSpace(upazilaDto.Code) && upazilaDto.ZilaId > 0)
                {
                    try
                    {
                        upazilaDto.Code = await _upazilaService.GenerateUpazilaCodeAsync(upazilaDto.ZilaId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to auto-generate code for upazila");
                        ModelState.AddModelError("Code", "Unable to generate code automatically. Please enter a code manually.");
                    }
                }

                if (ModelState.IsValid)
                {
                    upazilaDto.CreatedBy = User.Identity?.Name ?? "System";

                    try
                    {
                        var createdUpazila = await _upazilaService.CreateUpazilaAsync(upazilaDto);
                        TempData["Success"] = $"Sub-district '{upazilaDto.Name}' created successfully with code '{createdUpazila.Code}'! " +
                                            (upazilaDto.HasVDPUnit ? $"VDP unit with {upazilaDto.VDPMemberCount} members has been registered." : "You can now add stores to this sub-district.");

                        return RedirectToAction(nameof(Index), new { zilaId = upazilaDto.ZilaId });
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Handle specific business logic errors with user-friendly messages
                        if (ex.Message.Contains("Upazila with name") && ex.Message.Contains("already exists"))
                        {
                            TempData["Error"] = $"A sub-district with the name '{upazilaDto.Name}' already exists in this district. Please choose a different name.";
                            ModelState.AddModelError("Name", "This sub-district name is already taken in this district.");
                        }
                        else if (ex.Message.Contains("Upazila with code") && ex.Message.Contains("already exists"))
                        {
                            TempData["Error"] = $"The code '{upazilaDto.Code}' is already in use. Please choose a different code.";
                            ModelState.AddModelError("Code", "This code is already in use.");
                        }
                        else if (ex.Message.Contains("Zila with ID") && ex.Message.Contains("not found"))
                        {
                            TempData["Error"] = "The selected district is not valid. Please select a valid district.";
                            ModelState.AddModelError("ZilaId", "Invalid district selection.");
                        }
                        else
                        {
                            TempData["Error"] = "Unable to create the sub-district: " + ex.Message;
                        }

                        _logger.LogWarning("Upazila creation failed: {Message}", ex.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        // Handle validation errors with user-friendly messages
                        if (ex.Message.Contains("Upazila name"))
                        {
                            TempData["Error"] = "Sub-district name is required and cannot be empty. Please enter a valid sub-district name.";
                            ModelState.AddModelError("Name", "Sub-district name is required.");
                        }
                        else if (ex.Message.Contains("Upazila code"))
                        {
                            TempData["Error"] = "Sub-district code is required and cannot be empty. Please enter a valid sub-district code.";
                            ModelState.AddModelError("Code", "Sub-district code is required.");
                        }
                        else if (ex.Message.Contains("email"))
                        {
                            TempData["Error"] = "Invalid email address format. Please enter a valid email.";
                            ModelState.AddModelError("Email", "Invalid email format.");
                        }
                        else if (ex.Message.Contains("area"))
                        {
                            TempData["Error"] = "Area must be a positive number. Please enter a valid area.";
                            ModelState.AddModelError("Area", "Invalid area value.");
                        }
                        else if (ex.Message.Contains("population"))
                        {
                            TempData["Error"] = "Population must be a positive number. Please enter a valid population.";
                            ModelState.AddModelError("Population", "Invalid population value.");
                        }
                        else if (ex.Message.Contains("District"))
                        {
                            TempData["Error"] = "Please select a valid district.";
                            ModelState.AddModelError("ZilaId", "District selection is required.");
                        }
                        else
                        {
                            TempData["Error"] = "Some information you entered is not valid: " + ex.Message;
                        }

                        _logger.LogWarning("Upazila creation validation error: {Message}", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Unable to create sub-district. Please check your information and try again. If the problem continues, contact support.";
                        _logger.LogError(ex, "Unexpected error creating upazila: {Name}", upazilaDto?.Name);
                    }
                }
                else
                {
                    // Collect validation errors
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();

                    TempData["Error"] = "Please fix the following errors: " + string.Join(", ", errors);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to create sub-district. Please check your information and try again. If the problem continues, contact support.";
                _logger.LogError(ex, "Error creating upazila: {Name}", upazilaDto?.Name);
            }

            await LoadZilaSelectList(upazilaDto?.ZilaId);
            return View(upazilaDto);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCodeFromZila(int zilaId)
        {
            try
            {
                if (zilaId <= 0)
                {
                    return Json(new { success = false, message = "District selection is required" });
                }

                var code = await _upazilaService.GenerateUpazilaCodeAsync(zilaId);
                return Json(new { success = true, code = code });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating code for zila: {ZilaId}", zilaId);
                return Json(new { success = false, message = "Could not generate code" });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var upazila = await _upazilaService.GetUpazilaByIdAsync(id);
                if (upazila == null)
                {
                    TempData["Error"] = "The sub-district you're looking for was not found. It may have been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadZilaSelectList(upazila.ZilaId);
                return View(upazila);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading upazila for edit with ID {Id}", id);
                TempData["Error"] = "Unable to load the sub-district for editing. Please try again or contact support.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpazilaDto upazilaDto)
        {
            if (id != upazilaDto.Id)
            {
                TempData["Error"] = "Invalid sub-district information. Please try again or contact support if the problem persists.";
                return NotFound();
            }

            try
            {
                // Get original upazila for comparison
                var originalUpazila = await _upazilaService.GetUpazilaByIdAsync(id);
                if (originalUpazila == null)
                {
                    TempData["Error"] = "The sub-district you're trying to edit was not found. It may have been deleted by another user.";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    upazilaDto.UpdatedBy = userId ?? User.Identity?.Name ?? "System";

                    await _upazilaService.UpdateUpazilaAsync(upazilaDto);

                    // Success message with specific details about what changed
                    var changes = new List<string>();
                    if (originalUpazila.Name != upazilaDto.Name) changes.Add("name");
                    if (originalUpazila.UpazilaOfficerName != upazilaDto.UpazilaOfficerName) changes.Add("officer");
                    if (originalUpazila.ContactNumber != upazilaDto.ContactNumber) changes.Add("contact");
                    if (originalUpazila.HasVDPUnit != upazilaDto.HasVDPUnit) changes.Add("VDP unit status");
                    if (originalUpazila.VDPMemberCount != upazilaDto.VDPMemberCount) changes.Add("VDP member count");
                    if (originalUpazila.IsActive != upazilaDto.IsActive) changes.Add("status");
                    if (originalUpazila.Population != upazilaDto.Population) changes.Add("population");
                    if (originalUpazila.Area != upazilaDto.Area) changes.Add("area");

                    string changeText = changes.Any() ? $" ({string.Join(", ", changes)} updated)" : "";
                    TempData["Success"] = $"Sub-district '{upazilaDto.Name}' has been successfully updated{changeText}!";

                    return RedirectToAction(nameof(Index), new { zilaId = upazilaDto.ZilaId });
                }
                else
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();

                    TempData["Error"] = "Please fix the following issues: " + string.Join(", ", errors);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Handle specific business logic errors with user-friendly messages
                if (ex.Message.Contains("Upazila with name") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"Cannot update sub-district because another sub-district with the name '{upazilaDto.Name}' already exists in this district. Please choose a different name.";
                    ModelState.AddModelError("Name", "This sub-district name is already taken by another sub-district in this district.");
                }
                else if (ex.Message.Contains("Upazila with code") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"Cannot update sub-district because the code '{upazilaDto.Code}' is already in use by another sub-district. Please choose a different code.";
                    ModelState.AddModelError("Code", "This code is already in use by another sub-district.");
                }
                else if (ex.Message.Contains("Upazila with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The sub-district you're trying to update was not found. It may have been deleted by another user while you were editing.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Unable to update the sub-district: " + ex.Message;
                }

                _logger.LogWarning("Upazila update failed: {Message} (Upazila ID: {Id}, User: {User})", ex.Message, id, User.Identity?.Name);
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors with user-friendly messages
                if (ex.Message.Contains("Upazila name"))
                {
                    TempData["Error"] = "Sub-district name is required and cannot be empty. Please enter a valid sub-district name.";
                    ModelState.AddModelError("Name", "Sub-district name is required.");
                }
                else if (ex.Message.Contains("email"))
                {
                    TempData["Error"] = "Invalid email address format. Please enter a valid email.";
                    ModelState.AddModelError("Email", "Invalid email format.");
                }
                else
                {
                    TempData["Error"] = "Some information you entered is not valid: " + ex.Message;
                }

                _logger.LogWarning("Upazila update validation error: {Message} (Upazila ID: {Id})", ex.Message, id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "We're experiencing technical difficulties while saving your changes. Please try again in a few moments. If the problem continues, contact system administrator.";
                _logger.LogError(ex, "Unexpected error updating upazila: {Id} by user {User}", id, User.Identity?.Name);
            }

            await LoadZilaSelectList(upazilaDto?.ZilaId);
            return View(upazilaDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Get upazila details first for better messaging
                var upazila = await _upazilaService.GetUpazilaByIdAsync(id);
                if (upazila == null)
                {
                    TempData["Error"] = "The sub-district you're trying to delete was not found. It may have already been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                var upazilaName = upazila.Name;
                var upazilaCode = upazila.Code;
                var zilaId = upazila.ZilaId;

                // Attempt to delete
                await _upazilaService.DeleteUpazilaAsync(id);

                TempData["Success"] = $"Sub-district '{upazilaName}' ({upazilaCode}) has been successfully deleted.";
                return RedirectToAction(nameof(Index), new { zilaId });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("stores"))
                {
                    var storeCount = await _upazilaService.GetUpazilaStoreCountAsync(id);
                    TempData["Error"] = $"This sub-district cannot be deleted because it has {storeCount} store(s) assigned to it. " +
                                      "Please remove the store assignments first.";
                }
                else if (ex.Message.Contains("unions"))
                {
                    TempData["Error"] = "This sub-district cannot be deleted because it has unions assigned to it. " +
                                      "Please remove the union assignments first.";
                }
                else if (ex.Message.Contains("users assigned"))
                {
                    TempData["Error"] = "This sub-district cannot be deleted because it has users assigned to it. " +
                                      "Please reassign the users to another sub-district first.";
                }
                else if (ex.Message.Contains("Upazila with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The sub-district you're trying to delete was not found. It may have already been deleted by another user.";
                }
                else
                {
                    TempData["Error"] = "Unable to delete this sub-district: " + ex.Message;
                }

                _logger.LogWarning("Upazila deletion failed: {Message} (Upazila ID: {Id})", ex.Message, id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "We encountered an unexpected error while trying to delete the sub-district. " +
                                  "Please try again in a few moments. If the problem persists, contact system support.";

                _logger.LogError(ex, "Unexpected error deleting upazila with ID {Id}", id);
            }

            return RedirectToAction(nameof(Index));
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
                TempData["Error"] = "Please select a CSV file to import. The file cannot be empty.";
                return View();
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Please select a valid CSV file. Only .csv files are supported.";
                return View();
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
            {
                TempData["Error"] = "File size too large. Please select a file smaller than 5MB.";
                return View();
            }

            try
            {
                var filePath = Path.GetTempFileName();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var result = await _upazilaService.ImportUpazilasFromCsvAsync(filePath);

                // Clean up temp file
                System.IO.File.Delete(filePath);

                if (result.SuccessCount > 0)
                {
                    TempData["Success"] = $"Successfully imported {result.SuccessCount} sub-district(s)! " +
                                        (result.ErrorCount > 0 ? $"{result.ErrorCount} record(s) had errors and were skipped." : "");
                }
                else
                {
                    TempData["Error"] = "No sub-districts were imported. Please check your CSV format and data.";
                }

                if (result.Errors?.Any() == true)
                {
                    TempData["ImportErrors"] = string.Join("; ", result.Errors.Select(e => e.Message).Take(5)) +
                                             (result.Errors.Count > 5 ? $" and {result.Errors.Count - 5} more errors." : "");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing upazilas from file: {FileName}", file.FileName);
                TempData["Error"] = "An error occurred while importing sub-districts. Please check the CSV format and try again. " +
                                  "Make sure all required columns are present and data is in the correct format.";
                return View();
            }
        }

        public async Task<IActionResult> Export()
        {
            try
            {
                var csvData = await _upazilaService.ExportUpazilasAsync();
                var fileName = $"SubDistricts_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                TempData["Success"] = $"Successfully exported {(await _upazilaService.GetAllUpazilasAsync()).Count()} sub-district(s) to CSV file.";

                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting upazilas");
                TempData["Error"] = "An error occurred while exporting sub-districts. Please try again or contact support.";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX endpoints
        [HttpGet]
        public async Task<IActionResult> GetUpazilasByZila(int zilaId)
        {
            try
            {
                var upazilas = await _upazilaService.GetUpazilasByZilaAsync(zilaId);
                return Json(upazilas.Select(u => new { value = u.Id, text = u.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upazilas by zila {ZilaId}", zilaId);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code, int? excludeId = null)
        {
            try
            {
                var exists = await _upazilaService.UpazilaCodeExistsAsync(code, excludeId);
                return Json(!exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if code exists: {Code}", code);
                return Json(false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckNameExists(string name, int zilaId, int? excludeId = null)
        {
            try
            {
                var exists = await _upazilaService.UpazilaExistsAsync(name, zilaId, excludeId);
                return Json(!exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if name exists: {Name}", name);
                return Json(false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCode(int zilaId)
        {
            try
            {
                var code = await _upazilaService.GenerateUpazilaCodeAsync(zilaId);
                return Json(new { success = true, code });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating upazila code for zila {ZilaId}", zilaId);
                return Json(new { success = false, message = "Error generating code" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                var upazilas = await _upazilaService.SearchUpazilasAsync(searchTerm);
                return PartialView("_UpazilaList", upazilas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching upazilas with term '{SearchTerm}'", searchTerm);
                return PartialView("_UpazilaList", new List<UpazilaDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUpazilaStatistics(int id)
        {
            try
            {
                var statistics = await _upazilaService.GetUpazilaStatisticsAsync(id);
                return Json(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for upazila {Id}", id);
                return Json(new Dictionary<string, object>());
            }
        }

        // Helper methods
        private async Task LoadZilaSelectList(int? selectedZilaId = null)
        {
            try
            {
                var zilas = await _zilaService.GetActiveZilasAsync();
                ViewBag.Zilas = new SelectList(zilas ?? new List<ZilaDto>(), "Id", "Name", selectedZilaId);

                // Also provide the full list for filtering and other uses
                ViewBag.ZilasList = zilas ?? new List<ZilaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading zila select list");
                ViewBag.Zilas = new SelectList(new List<ZilaDto>(), "Id", "Name");
                ViewBag.ZilasList = new List<ZilaDto>();
            }
        }
    }
}