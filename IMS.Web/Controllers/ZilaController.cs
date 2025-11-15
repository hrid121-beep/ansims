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
    public class ZilaController : Controller
    {
        private readonly IZilaService _zilaService;
        private readonly IRangeService _rangeService;
        private readonly IUpazilaService _upazilaService;
        private readonly IStoreService _storeService;
        private readonly ILogger<ZilaController> _logger;

        public ZilaController(
            IZilaService zilaService,
            IRangeService rangeService,
            IUpazilaService upazilaService,
            IStoreService storeService,
            ILogger<ZilaController> logger)
        {
            _zilaService = zilaService ?? throw new ArgumentNullException(nameof(zilaService));
            _rangeService = rangeService ?? throw new ArgumentNullException(nameof(rangeService));
            _upazilaService = upazilaService ?? throw new ArgumentNullException(nameof(upazilaService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var zilas = await _zilaService.GetAllZilasAsync();
                await LoadRangeSelectList(); // For filter dropdown
                return View(zilas ?? new List<ZilaDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading zilas");
                TempData["Error"] = "An error occurred while loading districts. Please try again or contact support.";
                return View(new List<ZilaDto>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var zila = await _zilaService.GetZilaByIdAsync(id);
                if (zila == null)
                {
                    TempData["Error"] = "The district you're looking for was not found. It may have been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Upazilas = await _zilaService.GetZilaUpazilasAsync(id) ?? new List<UpazilaDto>();
                ViewBag.VDPMemberCount = await _zilaService.GetZilaVDPMemberCountAsync(id);
                ViewBag.Stores = await _storeService.GetStoresByZilaAsync(id) ?? new List<StoreDto>();
                ViewBag.Statistics = await _zilaService.GetZilaStatisticsAsync(id);

                return View(zila);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading zila details for ID {Id}", id);
                TempData["Error"] = "An error occurred while loading district details. Please try again or contact support.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                await LoadRangeSelectList();
                LoadDivisionSelectList();

                var model = new ZilaDto
                {
                    IsActive = true,
                    Code = string.Empty // Will be auto-generated
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create view");
                TempData["Error"] = "Unable to load the create form. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZilaDto zilaDto)
        {
            try
            {
                // Auto-generate code if empty
                if (string.IsNullOrWhiteSpace(zilaDto.Code))
                {
                    zilaDto.Code = await _zilaService.GenerateZilaCodeAsync(zilaDto.Name);
                }

                if (ModelState.IsValid)
                {
                    zilaDto.CreatedBy = User.Identity?.Name ?? "System";

                    try
                    {
                        var createdZila = await _zilaService.CreateZilaAsync(zilaDto);
                        TempData["Success"] = $"District '{zilaDto.Name}' created successfully with code '{createdZila.Code}'! You can now add upazilas to this district.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Handle specific business logic errors with user-friendly messages
                        if (ex.Message.Contains("Zila with name") && ex.Message.Contains("already exists"))
                        {
                            TempData["Error"] = $"A district with the name '{zilaDto.Name}' already exists. Please choose a different name.";
                            ModelState.AddModelError("Name", "This district name is already taken.");
                        }
                        else if (ex.Message.Contains("Zila with code") && ex.Message.Contains("already exists"))
                        {
                            TempData["Error"] = $"The code '{zilaDto.Code}' is already in use. Please choose a different code.";
                            ModelState.AddModelError("Code", "This code is already in use.");
                        }
                        else if (ex.Message.Contains("Range with ID") && ex.Message.Contains("not found"))
                        {
                            TempData["Error"] = "The selected range is not valid. Please select a valid range.";
                            ModelState.AddModelError("RangeId", "Invalid range selection.");
                        }
                        else
                        {
                            TempData["Error"] = "Unable to create the district: " + ex.Message;
                        }

                        _logger.LogWarning("Zila creation failed: {Message}", ex.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        // Handle validation errors with user-friendly messages
                        if (ex.Message.Contains("District name"))
                        {
                            TempData["Error"] = "District name is required and cannot be empty. Please enter a valid district name.";
                            ModelState.AddModelError("Name", "District name is required.");
                        }
                        else if (ex.Message.Contains("District code"))
                        {
                            TempData["Error"] = "District code is required and cannot be empty. Please enter a valid district code.";
                            ModelState.AddModelError("Code", "District code is required.");
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
                        else
                        {
                            TempData["Error"] = "Some information you entered is not valid: " + ex.Message;
                        }

                        _logger.LogWarning("Zila creation validation error: {Message}", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Unable to create district. Please check your information and try again. If the problem continues, contact support.";
                        _logger.LogError(ex, "Unexpected error creating zila: {Name}", zilaDto?.Name);
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
                TempData["Error"] = "Unable to create district. Please check your information and try again. If the problem continues, contact support.";
                _logger.LogError(ex, "Error creating zila: {Name}", zilaDto?.Name);
            }

            await LoadRangeSelectList(zilaDto?.RangeId);
            LoadDivisionSelectList(zilaDto?.Division);
            return View(zilaDto);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCodeFromName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return Json(new { success = false, message = "District name is required" });
                }

                var code = await _zilaService.GenerateZilaCodeAsync(name);
                return Json(new { success = true, code = code });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating code for name: {Name}", name);
                return Json(new { success = false, message = "Could not generate code" });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var zila = await _zilaService.GetZilaByIdAsync(id);
                if (zila == null)
                {
                    TempData["Error"] = "The district you're looking for was not found. It may have been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadRangeSelectList(zila.RangeId);
                LoadDivisionSelectList(zila.Division);
                return View(zila);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading zila for edit with ID {Id}", id);
                TempData["Error"] = "Unable to load the district for editing. Please try again or contact support.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ZilaDto zilaDto)
        {
            if (id != zilaDto.Id)
            {
                TempData["Error"] = "Invalid district information. Please try again or contact support if the problem persists.";
                return NotFound();
            }

            try
            {
                // Get original district for comparison
                var originalZila = await _zilaService.GetZilaByIdAsync(id);
                if (originalZila == null)
                {
                    TempData["Error"] = "The district you're trying to edit was not found. It may have been deleted by another user.";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    zilaDto.UpdatedBy = userId ?? User.Identity?.Name ?? "System";

                    await _zilaService.UpdateZilaAsync(zilaDto);

                    // Success message with specific details about what changed
                    var changes = new List<string>();
                    if (originalZila.Name != zilaDto.Name) changes.Add("name");
                    if (originalZila.DistrictOfficerName != zilaDto.DistrictOfficerName) changes.Add("district officer");
                    if (originalZila.Division != zilaDto.Division) changes.Add("division");
                    if (originalZila.ContactNumber != zilaDto.ContactNumber) changes.Add("contact");
                    if (originalZila.IsActive != zilaDto.IsActive) changes.Add("status");
                    if (originalZila.Population != zilaDto.Population) changes.Add("population");
                    if (originalZila.Area != zilaDto.Area) changes.Add("area");

                    string changeText = changes.Any() ? $" ({string.Join(", ", changes)} updated)" : "";
                    TempData["Success"] = $"District '{zilaDto.Name}' has been successfully updated{changeText}!";

                    return RedirectToAction(nameof(Index));
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
                if (ex.Message.Contains("Zila with name") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"Cannot update district because another district with the name '{zilaDto.Name}' already exists. Please choose a different name.";
                    ModelState.AddModelError("Name", "This district name is already taken by another district.");
                }
                else if (ex.Message.Contains("Zila with code") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"Cannot update district because the code '{zilaDto.Code}' is already in use by another district. Please choose a different code.";
                    ModelState.AddModelError("Code", "This code is already in use by another district.");
                }
                else if (ex.Message.Contains("Range with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The selected range is not valid. Please select a valid range.";
                    ModelState.AddModelError("RangeId", "Invalid range selection.");
                }
                else if (ex.Message.Contains("Zila with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The district you're trying to update was not found. It may have been deleted by another user while you were editing.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Unable to update the district: " + ex.Message;
                }

                _logger.LogWarning("Zila update failed: {Message} (Zila ID: {Id}, User: {User})", ex.Message, id, User.Identity?.Name);
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors with user-friendly messages
                if (ex.Message.Contains("District name"))
                {
                    TempData["Error"] = "District name is required and cannot be empty. Please enter a valid district name.";
                    ModelState.AddModelError("Name", "District name is required.");
                }
                else if (ex.Message.Contains("District code"))
                {
                    TempData["Error"] = "District code is required and cannot be empty. Please enter a valid district code.";
                    ModelState.AddModelError("Code", "District code is required.");
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
                else
                {
                    TempData["Error"] = "Some information you entered is not valid: " + ex.Message;
                }

                _logger.LogWarning("Zila update validation error: {Message} (Zila ID: {Id})", ex.Message, id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "We're experiencing technical difficulties while saving your changes. Please try again in a few moments. If the problem continues, contact system administrator.";
                _logger.LogError(ex, "Unexpected error updating zila: {Id} by user {User}", id, User.Identity?.Name);
            }

            await LoadRangeSelectList(zilaDto?.RangeId);
            LoadDivisionSelectList(zilaDto?.Division);
            return View(zilaDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Get district details first for better messaging
                var zila = await _zilaService.GetZilaByIdAsync(id);
                if (zila == null)
                {
                    TempData["Error"] = "The district you're trying to delete was not found. It may have already been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                var zilaName = zila.Name;
                var zilaCode = zila.Code;

                // Attempt to delete
                await _zilaService.DeleteZilaAsync(id);

                TempData["Success"] = $"District '{zilaName}' ({zilaCode}) has been successfully deleted.";
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("upazilas"))
                {
                    var upazilaCount = await _zilaService.GetZilaUpazilaCountAsync(id);
                    TempData["Error"] = $"This district cannot be deleted because it has {upazilaCount} upazila(s) assigned to it. " +
                                      "Please remove the upazila assignments first.";
                }
                else if (ex.Message.Contains("stores"))
                {
                    var storeCount = await _zilaService.GetZilaStoreCountAsync(id);
                    TempData["Error"] = $"This district cannot be deleted because it has {storeCount} store(s) assigned to it. " +
                                      "Please remove the store assignments first.";
                }
                else if (ex.Message.Contains("users assigned"))
                {
                    TempData["Error"] = "This district cannot be deleted because it has users assigned to it. " +
                                      "Please reassign the users to another district first.";
                }
                else if (ex.Message.Contains("Zila with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The district you're trying to delete was not found. It may have already been deleted by another user.";
                }
                else
                {
                    TempData["Error"] = "Unable to delete this district: " + ex.Message;
                }

                _logger.LogWarning("Zila deletion failed: {Message} (Zila ID: {Id})", ex.Message, id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "We encountered an unexpected error while trying to delete the district. " +
                                  "Please try again in a few moments. If the problem persists, contact system support.";

                _logger.LogError(ex, "Unexpected error deleting zila with ID {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ByDivision(string division)
        {
            try
            {
                var zilas = await _zilaService.GetZilasByDivisionAsync(division);
                ViewBag.Division = division;
                await LoadRangeSelectList(); // For filter dropdown
                return View("Index", zilas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading zilas by division: {Division}", division);
                TempData["Error"] = "An error occurred while loading districts for the selected division.";
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

                var result = await _zilaService.ImportZilasFromCsvAsync(filePath);

                // Clean up temp file
                System.IO.File.Delete(filePath);

                if (result.SuccessCount > 0)
                {
                    TempData["Success"] = $"Successfully imported {result.SuccessCount} district(s)! " +
                                        (result.ErrorCount > 0 ? $"{result.ErrorCount} record(s) had errors and were skipped." : "");
                }
                else
                {
                    TempData["Error"] = "No districts were imported. Please check your CSV format and data.";
                }

                if (result.Errors?.Any() == true)
                {
                    TempData["ImportErrors"] = string.Join("; ", result.Errors.Take(5)) +
                                             (result.Errors.Count > 5 ? $" and {result.Errors.Count - 5} more errors." : "");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing zilas from file: {FileName}", file.FileName);
                TempData["Error"] = "An error occurred while importing districts. Please check the CSV format and try again. " +
                                  "Make sure all required columns are present and data is in the correct format.";
                return View();
            }
        }

        public async Task<IActionResult> Export()
        {
            try
            {
                var csvData = await _zilaService.ExportZilasAsync();
                var fileName = $"Districts_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                TempData["Success"] = $"Successfully exported {(await _zilaService.GetAllZilasAsync()).Count()} district(s) to CSV file.";

                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting zilas");
                TempData["Error"] = "An error occurred while exporting districts. Please try again or contact support.";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX endpoints
        [HttpGet]
        public async Task<IActionResult> GetZilasByRange(int rangeId)
        {
            try
            {
                var zilas = await _zilaService.GetZilasByRangeAsync(rangeId);
                return Json(zilas.Select(z => new { value = z.Id, text = z.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting zilas by range {RangeId}", rangeId);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code, int? excludeId = null)
        {
            try
            {
                var exists = await _zilaService.ZilaCodeExistsAsync(code, excludeId);
                return Json(!exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if code exists: {Code}", code);
                return Json(false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckNameExists(string name, int? excludeId = null)
        {
            try
            {
                var exists = await _zilaService.ZilaExistsAsync(name, excludeId);
                return Json(!exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if name exists: {Name}", name);
                return Json(false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                var zilas = await _zilaService.SearchZilasAsync(searchTerm);
                return PartialView("_ZilaList", zilas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching zilas with term '{SearchTerm}'", searchTerm);
                return PartialView("_ZilaList", new List<ZilaDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetZilaStatistics(int id)
        {
            try
            {
                var statistics = await _zilaService.GetZilaStatisticsAsync(id);
                return Json(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for zila {Id}", id);
                return Json(new Dictionary<string, object>());
            }
        }

        // Helper methods
        private async Task LoadRangeSelectList(int? selectedRangeId = null)
        {
            try
            {
                var ranges = await _rangeService.GetActiveRangesAsync();
                ViewBag.Ranges = new SelectList(ranges ?? new List<RangeDto>(), "Id", "Name", selectedRangeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading range select list");
                ViewBag.Ranges = new SelectList(new List<RangeDto>(), "Id", "Name");
            }
        }

        private void LoadDivisionSelectList(string selectedDivision = null)
        {
            var divisions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Dhaka Division", Text = "Dhaka Division" },
                new SelectListItem { Value = "Chittagong Division", Text = "Chittagong Division" },
                new SelectListItem { Value = "Rajshahi Division", Text = "Rajshahi Division" },
                new SelectListItem { Value = "Khulna Division", Text = "Khulna Division" },
                new SelectListItem { Value = "Barisal Division", Text = "Barisal Division" },
                new SelectListItem { Value = "Sylhet Division", Text = "Sylhet Division" },
                new SelectListItem { Value = "Rangpur Division", Text = "Rangpur Division" },
                new SelectListItem { Value = "Mymensingh Division", Text = "Mymensingh Division" }
            };

            ViewBag.Divisions = new SelectList(divisions, "Value", "Text", selectedDivision);
        }

        // ==================== EXPORT OPERATIONS ====================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportToCsv(string status = null)
        {
            try
            {
                var zilas = await _zilaService.GetAllZilasAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        zilas = zilas.Where(z => z.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        zilas = zilas.Where(z => !z.IsActive);
                }

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Code,Name,Division,Status");

                foreach (var zila in zilas)
                {
                    csv.AppendLine($"\"{EscapeCsv(zila.Code)}\"," +
                        $"\"{EscapeCsv(zila.Name)}\"," +
                        $"\"{EscapeCsv(zila.Division)}\"," +
                        $"\"{(zila.IsActive ? "Active" : "Inactive")}\"");
                }

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"Zilas_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting zilas to CSV");
                TempData["Error"] = "Error exporting data to CSV.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportToPdf(string status = null)
        {
            try
            {
                var zilas = await _zilaService.GetAllZilasAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        zilas = zilas.Where(z => z.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        zilas = zilas.Where(z => !z.IsActive);
                }

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30);
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18);
                    var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

                    var titleParagraph = new iTextSharp.text.Paragraph("ANSAR & VDP - Zilas Report", titleFont);
                    titleParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    titleParagraph.SpacingAfter = 10f;
                    document.Add(titleParagraph);

                    var infoParagraph = new iTextSharp.text.Paragraph($"Report Generated: {DateTime.Now:dd-MMM-yyyy HH:mm} | Total: {zilas.Count()}", normalFont);
                    infoParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    infoParagraph.SpacingAfter = 15f;
                    document.Add(infoParagraph);

                    var mainTable = new iTextSharp.text.pdf.PdfPTable(4);
                    mainTable.WidthPercentage = 100;
                    mainTable.SetWidths(new float[] { 20f, 40f, 30f, 10f });

                    var headerTexts = new[] { "Code", "Name", "Division", "Status" };
                    foreach (var headerText in headerTexts)
                    {
                        var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(headerText, headerFont));
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(220, 220, 220);
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.Padding = 5f;
                        mainTable.AddCell(cell);
                    }

                    foreach (var zila in zilas)
                    {
                        mainTable.AddCell(new iTextSharp.text.Phrase(zila.Code ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(zila.Name ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(zila.Division ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(zila.IsActive ? "Active" : "Inactive", normalFont));
                    }

                    document.Add(mainTable);

                    var footerParagraph = new iTextSharp.text.Paragraph($"\nGenerated by: IMS System | Date: {DateTime.Now:dd-MMM-yyyy HH:mm}",
                        iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8));
                    footerParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    footerParagraph.SpacingBefore = 20f;
                    document.Add(footerParagraph);

                    document.Close();
                    return File(memoryStream.ToArray(), "application/pdf", $"Zilas_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting zilas to PDF");
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