using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class RangeController : Controller
    {
        private readonly IRangeService _rangeService;
        private readonly IBattalionService _battalionService;
        private readonly IZilaService _zilaService;
        private readonly ILogger<RangeController> _logger;

        public RangeController(
            IRangeService rangeService,
            IBattalionService battalionService,
            IZilaService zilaService,
            ILogger<RangeController> logger)
        {
            _rangeService = rangeService ?? throw new ArgumentNullException(nameof(rangeService));
            _battalionService = battalionService ?? throw new ArgumentNullException(nameof(battalionService));
            _zilaService = zilaService ?? throw new ArgumentNullException(nameof(zilaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var ranges = await _rangeService.GetAllRangesAsync();
                return View(ranges ?? new List<RangeDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ranges");
                TempData["Error"] = "An error occurred while loading ranges.";
                return View(new List<RangeDto>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var range = await _rangeService.GetRangeByIdAsync(id);
                if (range == null)
                {
                    TempData["Error"] = "Range not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Battalions = await _rangeService.GetRangeBattalionsAsync(id) ?? new List<BattalionDto>();
                ViewBag.Zilas = await _rangeService.GetRangeZilasAsync(id) ?? new List<ZilaDto>();
                ViewBag.Statistics = await _rangeService.GetRangeStatisticsAsync(id);
                ViewBag.DashboardData = await _rangeService.GetRangeDashboardDataAsync(id);

                return View(range);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading range details for ID {Id}", id);
                TempData["Error"] = "An error occurred while loading range details.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            try
            {
                var model = new RangeDto
                {
                    IsActive = true,
                    Code = string.Empty // Will be generated
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
        public async Task<IActionResult> Create(RangeDto rangeDto)
        {
            try
            {
                // Auto-generate code if empty
                if (string.IsNullOrWhiteSpace(rangeDto.Code))
                {
                    if (!string.IsNullOrWhiteSpace(rangeDto.Name))
                    {
                        rangeDto.Code = GenerateCodeFromRangeName(rangeDto.Name);

                        // Check and handle duplicates
                        if (await _rangeService.RangeCodeExistsAsync(rangeDto.Code))
                        {
                            int counter = 1;
                            string baseCode = rangeDto.Code;
                            while (await _rangeService.RangeCodeExistsAsync($"{baseCode}{counter}") && counter <= 99)
                            {
                                counter++;
                            }
                            rangeDto.Code = $"{baseCode}{counter}";
                        }
                    }
                    else
                    {
                        rangeDto.Code = await GenerateRandomCodeAsync();
                    }
                }

                if (ModelState.IsValid)
                {
                    rangeDto.CreatedBy = User.Identity?.Name ?? "System";
                    await _rangeService.CreateRangeAsync(rangeDto);
                    TempData["Success"] = $"Range '{rangeDto.Name}' created successfully with code '{rangeDto.Code}'!";
                    return RedirectToAction(nameof(Index));
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
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("name") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"A range with the name '{rangeDto.Name}' already exists. Please choose a different name.";
                    ModelState.AddModelError("Name", "This range name is already taken.");
                }
                else if (ex.Message.Contains("code") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"The code '{rangeDto.Code}' is already in use. Please choose a different code.";
                    ModelState.AddModelError("Code", "This code is already in use.");
                }
                else
                {
                    TempData["Error"] = ex.Message;
                }

                _logger.LogWarning("Range creation failed: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to create range. Please check your information and try again. If the problem continues, contact support.";
                _logger.LogError(ex, "Error creating range: {Name}", rangeDto?.Name);
            }

            return View(rangeDto);
        }


        [HttpGet]
        public async Task<IActionResult> GenerateCodeFromName(string name)
        {
            try
            {
                string code;

                if (string.IsNullOrWhiteSpace(name))
                {
                    code = await GenerateRandomCodeAsync();
                }
                else
                {
                    code = GenerateCodeFromRangeName(name);

                    // Check if code already exists
                    var exists = await _rangeService.RangeCodeExistsAsync(code);
                    if (exists)
                    {
                        // Add number if exists
                        int counter = 1;
                        string baseCode = code;
                        while (await _rangeService.RangeCodeExistsAsync($"{baseCode}{counter}") && counter <= 99)
                        {
                            counter++;
                        }
                        code = $"{baseCode}{counter}";
                    }
                }

                return Json(new { success = true, code = code });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating code from name: {Name}", name);
                return Json(new { success = false, message = "Could not generate code" });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var range = await _rangeService.GetRangeByIdAsync(id);
                if (range == null)
                {
                    TempData["Error"] = "The range you're looking for was not found. It may have been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                return View(range);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading range for edit with ID {Id}", id);
                TempData["Error"] = "Unable to load the range for editing. Please try again or contact support.";
                return RedirectToAction(nameof(Index));
            }
        }

        // In RangeController.cs - Update the Edit method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RangeDto rangeDto)
        {
            if (id != rangeDto.Id)
            {
                TempData["Error"] = "Invalid range information. Please try again or contact support if the problem persists.";
                return NotFound();
            }

            try
            {
                // Get original range for comparison
                var originalRange = await _rangeService.GetRangeByIdAsync(id);
                if (originalRange == null)
                {
                    TempData["Error"] = "The range you're trying to edit was not found. It may have been deleted by another user.";
                    return RedirectToAction(nameof(Index));
                }

                // Code cannot be changed - always use original code
                rangeDto.Code = originalRange.Code;

                // Remove ALL Code-related validation errors (Code field is readonly and cannot be changed)
                ModelState.Remove("Code");
                var codeErrors = ModelState.Keys.Where(k => k.Contains("Code")).ToList();
                foreach (var key in codeErrors)
                {
                    ModelState.Remove(key);
                }

                if (ModelState.IsValid)
                {
                    // Fix: Use User ID instead of Username
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    rangeDto.UpdatedBy = userId ?? "System"; // Use actual user ID, fallback to "System"

                    await _rangeService.UpdateRangeAsync(rangeDto);

                    // Success message with specific details about what changed
                    var changes = new List<string>();
                    if (originalRange.Name != rangeDto.Name) changes.Add("name");
                    if (originalRange.CommanderName != rangeDto.CommanderName) changes.Add("commander");
                    if (originalRange.CommanderRank != rangeDto.CommanderRank) changes.Add("rank");
                    if (originalRange.HeadquarterLocation != rangeDto.HeadquarterLocation) changes.Add("location");
                    if (originalRange.IsActive != rangeDto.IsActive) changes.Add("status");

                    string changeText = changes.Any() ? $" ({string.Join(", ", changes)} updated)" : "";
                    TempData["Success"] = $"Range '{rangeDto.Name}' has been successfully updated{changeText}!";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Collect all validation errors for user-friendly display
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();

                    TempData["Error"] = "Please fix the following issues: " + string.Join(", ", errors);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Business logic errors - user-friendly messages
                if (ex.Message.Contains("name") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"Cannot update range because another range with the name '{rangeDto.Name}' already exists. Please choose a different name.";
                    ModelState.AddModelError("Name", "This range name is already taken by another range.");
                }
                else if (ex.Message.Contains("code") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"Cannot update range because the code '{rangeDto.Code}' is already in use by another range. Please choose a different code.";
                    ModelState.AddModelError("Code", "This code is already in use by another range.");
                }
                else if (ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The range you're trying to update was not found. It may have been deleted by another user while you were editing.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Unable to update the range: " + ex.Message;
                }

                _logger.LogWarning("Range update failed: {Message} (Range ID: {Id}, User: {User})", ex.Message, id, User.Identity?.Name);
            }
            catch (ArgumentException ex)
            {
                // Validation errors - user-friendly
                if (ex.Message.Contains("name"))
                {
                    TempData["Error"] = "Range name is required and cannot be empty. Please enter a valid range name.";
                    ModelState.AddModelError("Name", "Range name is required.");
                }
                else if (ex.Message.Contains("code"))
                {
                    TempData["Error"] = "Range code format is invalid. Please use the correct format (e.g., DH-R).";
                    ModelState.AddModelError("Code", "Invalid code format.");
                }
                else
                {
                    TempData["Error"] = "Some information you entered is not valid. Please check your entries and try again.";
                }

                _logger.LogWarning("Range update validation error: {Message} (Range ID: {Id})", ex.Message, id);
            }
            catch (Exception ex)
            {
                // System errors - user-friendly but log details
                TempData["Error"] = "We're experiencing technical difficulties while saving your changes. Please try again in a few moments. If the problem continues, contact system administrator.";
                _logger.LogError(ex, "Unexpected error updating range: {Id} by user {User}", id, User.Identity?.Name);
            }

            return View(rangeDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Get range details first for better messaging
                var range = await _rangeService.GetRangeByIdAsync(id);
                if (range == null)
                {
                    TempData["Error"] = "The range you're trying to delete was not found. It may have already been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                // Check for dependencies with detailed counts
                var battalionCount = await _rangeService.GetRangeBattalionCountAsync(id);
                var zilaCount = await _rangeService.GetRangeZilaCountAsync(id);
                var storeCount = await _rangeService.GetRangeStoreCountAsync(id);

                // Build user-friendly error message if there are dependencies
                if (battalionCount > 0 || zilaCount > 0 || storeCount > 0)
                {
                    var dependencies = new List<string>();
                    if (battalionCount > 0) dependencies.Add($"{battalionCount} Battalion{(battalionCount > 1 ? "s" : "")}");
                    if (zilaCount > 0) dependencies.Add($"{zilaCount} District{(zilaCount > 1 ? "s" : "")}");
                    if (storeCount > 0) dependencies.Add($"{storeCount} Store{(storeCount > 1 ? "s" : "")}");

                    var dependencyText = string.Join(", ", dependencies);
                    TempData["Error"] = $"Cannot delete '{range.Name}' because it still has {dependencyText} assigned to it. " +
                                      "Please reassign or remove these items first, then try deleting the range again.";
                    return RedirectToAction(nameof(Index));
                }

                // Attempt to delete
                await _rangeService.DeleteRangeAsync(id);

                // Success message
                TempData["Success"] = $"Range '{range.Name}' ({range.Code}) has been successfully deleted. " +
                                    "All associated data has been updated accordingly.";
            }
            catch (InvalidOperationException ex)
            {
                // Business logic errors - user-friendly messages
                if (ex.Message.Contains("assigned battalions"))
                {
                    TempData["Error"] = "This range cannot be deleted because it has battalions assigned to it. " +
                                      "Please reassign the battalions to another range first.";
                }
                else if (ex.Message.Contains("assigned districts"))
                {
                    TempData["Error"] = "This range cannot be deleted because it has districts assigned to it. " +
                                      "Please reassign the districts to another range first.";
                }
                else if (ex.Message.Contains("assigned stores"))
                {
                    TempData["Error"] = "This range cannot be deleted because it has stores assigned to it. " +
                                      "Please reassign the stores to another range first.";
                }
                else if (ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The range you're trying to delete was not found. It may have already been deleted by another user.";
                }
                else
                {
                    TempData["Error"] = "Unable to delete this range: " + ex.Message;
                }

                _logger.LogWarning("Range deletion failed: {Message} (Range ID: {Id})", ex.Message, id);
            }
            catch (Exception ex)
            {
                // System errors - user-friendly message
                TempData["Error"] = "We encountered an unexpected error while trying to delete the range. " +
                                  "Please try again in a few moments. If the problem persists, contact system support.";

                _logger.LogError(ex, "Unexpected error deleting range with ID {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Hierarchy(int id)
        {
            try
            {
                var hierarchy = await _rangeService.GetRangeHierarchyAsync(id);
                if (hierarchy == null)
                {
                    TempData["Error"] = "Range not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(hierarchy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading range hierarchy for ID {Id}", id);
                TempData["Error"] = "An error occurred while loading the hierarchy.";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX endpoints
        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code, int? excludeId = null)
        {
            try
            {
                var exists = await _rangeService.RangeCodeExistsAsync(code, excludeId);
                return Json(!exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if code exists: {Code}", code);
                return Json(false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRangeStatistics(int id)
        {
            try
            {
                var stats = await _rangeService.GetRangeStatisticsAsync(id);
                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for range {Id}", id);
                return Json(null);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                var ranges = await _rangeService.SearchRangesAsync(searchTerm);
                return PartialView("_RangeList", ranges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching ranges with term '{SearchTerm}'", searchTerm);
                return PartialView("_RangeList", new List<RangeDto>());
            }
        }



        private string GenerateCodeFromRangeName(string rangeName)
        {
            try
            {
                // Clean the name
                var cleanName = rangeName.Trim().ToUpper()
                    .Replace("RANGE", "")
                    .Replace("CANTONMENT", "")
                    .Replace("DIVISION", "")
                    .Replace("DISTRICT", "")
                    .Trim();

                // Predefined mapping for common areas
                var areaMapping = new Dictionary<string, string>
                {
                    {"DHAKA", "DH"},
                    {"CHITTAGONG", "CT"},
                    {"KHULNA", "KH"},
                    {"RAJSHAHI", "RJ"},
                    {"BARISAL", "BS"},
                    {"SYLHET", "SY"},
                    {"RANGPUR", "RG"},
                    {"MYMENSINGH", "MY"},
                    {"COMILLA", "CM"},
                    {"FARIDPUR", "FP"},
                    {"JESSORE", "JS"},
                    {"BOGRA", "BG"},
                    {"DINAJPUR", "DJ"},
                    {"KUSHTIA", "KS"},
                    {"PABNA", "PB"},
                    {"TANGAIL", "TG"}
                };

                // Check if we have a predefined mapping
                foreach (var mapping in areaMapping)
                {
                    if (cleanName.Contains(mapping.Key))
                    {
                        return $"{mapping.Value}-R";
                    }
                }

                // If no mapping found, generate from first letters
                if (cleanName.Length >= 2)
                {
                    // Take first 2 consonants if possible
                    var consonants = cleanName.Where(c => !"AEIOU ".Contains(c)).ToArray();
                    if (consonants.Length >= 2)
                    {
                        return $"{consonants[0]}{consonants[1]}-R";
                    }
                    else
                    {
                        return $"{cleanName.Substring(0, 2)}-R";
                    }
                }

                return "RG-R"; // Default fallback
            }
            catch
            {
                return "RG-R"; // Fallback
            }
        }

        private async Task<string> GenerateRandomCodeAsync()
        {
            var prefixes = new[] { "RG", "NK", "PT", "GP", "MD", "NT", "LK", "FN", "JH", "ST" };

            foreach (var prefix in prefixes)
            {
                var code = $"{prefix}-R";
                if (!await _rangeService.RangeCodeExistsAsync(code))
                {
                    return code;
                }
            }

            // If all taken, use timestamp
            return $"R{DateTime.Now:MMdd}-R";
        }

        // ==================== EXPORT OPERATIONS ====================

        [HttpGet]
        [HasPermission(Permission.ViewAllRanges)]
        public async Task<IActionResult> ExportToCsv(string status = null)
        {
            try
            {
                var ranges = await _rangeService.GetAllRangesAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        ranges = ranges.Where(r => r.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        ranges = ranges.Where(r => !r.IsActive);
                }

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Code,Name,Commander,Contact,Location,Status");

                foreach (var range in ranges)
                {
                    csv.AppendLine($"\"{EscapeCsv(range.Code)}\"," +
                        $"\"{EscapeCsv(range.Name)}\"," +
                        $"\"{EscapeCsv(range.CommanderName)}\"," +
                        $"\"{EscapeCsv(range.ContactNumber)}\"," +
                        $"\"{EscapeCsv(range.Location)}\"," +
                        $"\"{(range.IsActive ? "Active" : "Inactive")}\"");
                }

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"Ranges_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting ranges to CSV");
                TempData["Error"] = "Error exporting data to CSV.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewAllRanges)]
        public async Task<IActionResult> ExportToPdf(string status = null)
        {
            try
            {
                var ranges = await _rangeService.GetAllRangesAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        ranges = ranges.Where(r => r.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        ranges = ranges.Where(r => !r.IsActive);
                }

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30);
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18);
                    var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

                    var titleParagraph = new iTextSharp.text.Paragraph("ANSAR & VDP - Ranges Report", titleFont);
                    titleParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    titleParagraph.SpacingAfter = 10f;
                    document.Add(titleParagraph);

                    var infoParagraph = new iTextSharp.text.Paragraph($"Report Generated: {DateTime.Now:dd-MMM-yyyy HH:mm} | Total: {ranges.Count()}", normalFont);
                    infoParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    infoParagraph.SpacingAfter = 15f;
                    document.Add(infoParagraph);

                    var mainTable = new iTextSharp.text.pdf.PdfPTable(5);
                    mainTable.WidthPercentage = 100;
                    mainTable.SetWidths(new float[] { 15f, 30f, 25f, 20f, 10f });

                    var headerTexts = new[] { "Code", "Name", "Commander", "Contact", "Status" };
                    foreach (var headerText in headerTexts)
                    {
                        var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(headerText, headerFont));
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(220, 220, 220);
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.Padding = 5f;
                        mainTable.AddCell(cell);
                    }

                    foreach (var range in ranges)
                    {
                        mainTable.AddCell(new iTextSharp.text.Phrase(range.Code ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(range.Name ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(range.CommanderName ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(range.ContactNumber ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(range.IsActive ? "Active" : "Inactive", normalFont));
                    }

                    document.Add(mainTable);

                    var footerParagraph = new iTextSharp.text.Paragraph($"\nGenerated by: IMS System | Date: {DateTime.Now:dd-MMM-yyyy HH:mm}",
                        iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8));
                    footerParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    footerParagraph.SpacingBefore = 20f;
                    document.Add(footerParagraph);

                    document.Close();
                    return File(memoryStream.ToArray(), "application/pdf", $"Ranges_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting ranges to PDF");
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