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
    public class BattalionController : Controller
    {
        private readonly IBattalionService _battalionService;
        private readonly IRangeService _rangeService;
        private readonly IStoreService _storeService;
        private readonly ILogger<BattalionController> _logger;

        public BattalionController(
            IBattalionService battalionService,
            IRangeService rangeService,
            IStoreService storeService,
            ILogger<BattalionController> logger)
        {
            _battalionService = battalionService ?? throw new ArgumentNullException(nameof(battalionService));
            _rangeService = rangeService ?? throw new ArgumentNullException(nameof(rangeService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var battalions = await _battalionService.GetAllBattalionsAsync();
                return View(battalions ?? new List<BattalionDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading battalions");
                TempData["Error"] = "An error occurred while loading battalions.";
                return View(new List<BattalionDto>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var battalion = await _battalionService.GetBattalionByIdAsync(id);
                if (battalion == null)
                {
                    TempData["Error"] = "Battalion not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Stores = await _battalionService.GetBattalionStoresAsync(id) ?? new List<StoreDto>();
                ViewBag.Statistics = await _battalionService.GetBattalionStatisticsAsync(id);

                return View(battalion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading battalion details for ID {Id}", id);
                TempData["Error"] = "An error occurred while loading battalion details.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                await LoadRangeSelectList();
                LoadBattalionTypeSelectList();
                LoadOperationalStatusSelectList();

                var model = new BattalionDto
                {
                    IsActive = true,
                    Type = BattalionType.Male, // Default
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
        public async Task<IActionResult> Create(BattalionDto battalionDto)
        {
            try
            {
                // Auto-generate code if empty
                if (string.IsNullOrWhiteSpace(battalionDto.Code))
                {
                    battalionDto.Code = await _battalionService.GenerateBattalionCodeAsync(battalionDto.Type);
                }

                if (ModelState.IsValid)
                {
                    battalionDto.CreatedBy = User.Identity?.Name ?? "System";

                    try
                    {
                        var createdBattalion = await _battalionService.CreateBattalionAsync(battalionDto);
                        TempData["Success"] = $"Battalion '{battalionDto.Name}' created successfully with code '{createdBattalion.Code}'!";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Handle specific business logic errors with user-friendly messages
                        if (ex.Message.Contains("Battalion with name") && ex.Message.Contains("already exists"))
                        {
                            TempData["Error"] = $"A battalion with the name '{battalionDto.Name}' already exists. Please choose a different name.";
                            ModelState.AddModelError("Name", "This battalion name is already taken.");
                        }
                        else if (ex.Message.Contains("Battalion with code") && ex.Message.Contains("already exists"))
                        {
                            TempData["Error"] = $"The code '{battalionDto.Code}' is already in use. Please choose a different code.";
                            ModelState.AddModelError("Code", "This code is already in use.");
                        }
                        else if (ex.Message.Contains("Range with ID") && ex.Message.Contains("not found"))
                        {
                            TempData["Error"] = "The selected range is not valid. Please select a valid range.";
                            ModelState.AddModelError("RangeId", "Invalid range selection.");
                        }
                        else
                        {
                            // For any other InvalidOperationException, show the actual message as it's likely user-friendly
                            TempData["Error"] = ex.Message;
                        }

                        _logger.LogWarning("Battalion creation failed: {Message}", ex.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        // Handle validation errors with user-friendly messages
                        if (ex.Message.Contains("Battalion name"))
                        {
                            TempData["Error"] = "Battalion name is required and cannot be empty. Please enter a valid battalion name.";
                            ModelState.AddModelError("Name", "Battalion name is required.");
                        }
                        else if (ex.Message.Contains("Battalion code"))
                        {
                            TempData["Error"] = "Battalion code is required and cannot be empty. Please enter a valid battalion code.";
                            ModelState.AddModelError("Code", "Battalion code is required.");
                        }
                        else if (ex.Message.Contains("Officer count"))
                        {
                            TempData["Error"] = "Officer count cannot be negative. Please enter a valid number.";
                            ModelState.AddModelError("OfficerCount", "Invalid officer count.");
                        }
                        else if (ex.Message.Contains("Enlisted count"))
                        {
                            TempData["Error"] = "Enlisted count cannot be negative. Please enter a valid number.";
                            ModelState.AddModelError("EnlistedCount", "Invalid enlisted count.");
                        }
                        else if (ex.Message.Contains("Total personnel"))
                        {
                            TempData["Error"] = "Total personnel must equal the sum of officer and enlisted counts.";
                            ModelState.AddModelError("TotalPersonnel", "Personnel counts don't match.");
                        }
                        else if (ex.Message.Contains("email"))
                        {
                            TempData["Error"] = "Invalid email address format. Please enter a valid email.";
                            ModelState.AddModelError("Email", "Invalid email format.");
                        }
                        else if (ex.Message.Contains("Established date"))
                        {
                            TempData["Error"] = "Established date cannot be in the future. Please select a valid date.";
                            ModelState.AddModelError("EstablishedDate", "Invalid date.");
                        }
                        else
                        {
                            TempData["Error"] = "Some information you entered is not valid: " + ex.Message;
                        }

                        _logger.LogWarning("Battalion creation validation error: {Message}", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Unable to create battalion. Please check your information and try again. If the problem continues, contact support.";
                        _logger.LogError(ex, "Unexpected error creating battalion: {Name}", battalionDto?.Name);
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
            catch (InvalidOperationException ex)
            {
                // Handle specific business logic errors with user-friendly messages
                if (ex.Message.Contains("Battalion with name") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"A battalion with the name '{battalionDto.Name}' already exists. Please choose a different name.";
                    ModelState.AddModelError("Name", "This battalion name is already taken.");
                }
                else if (ex.Message.Contains("Battalion with code") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"The code '{battalionDto.Code}' is already in use. Please choose a different code.";
                    ModelState.AddModelError("Code", "This code is already in use.");
                }
                else if (ex.Message.Contains("Range with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The selected range is not valid. Please select a valid range.";
                    ModelState.AddModelError("RangeId", "Invalid range selection.");
                }
                else
                {
                    // For any other InvalidOperationException, show the actual message as it's likely user-friendly
                    TempData["Error"] = ex.Message;
                }

                _logger.LogWarning("Battalion creation failed: {Message}", ex.Message);
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors with user-friendly messages
                if (ex.Message.Contains("Battalion name"))
                {
                    TempData["Error"] = "Battalion name is required and cannot be empty. Please enter a valid battalion name.";
                    ModelState.AddModelError("Name", "Battalion name is required.");
                }
                else if (ex.Message.Contains("Battalion code"))
                {
                    TempData["Error"] = "Battalion code is required and cannot be empty. Please enter a valid battalion code.";
                    ModelState.AddModelError("Code", "Battalion code is required.");
                }
                else if (ex.Message.Contains("Officer count"))
                {
                    TempData["Error"] = "Officer count cannot be negative. Please enter a valid number.";
                    ModelState.AddModelError("OfficerCount", "Invalid officer count.");
                }
                else if (ex.Message.Contains("Enlisted count"))
                {
                    TempData["Error"] = "Enlisted count cannot be negative. Please enter a valid number.";
                    ModelState.AddModelError("EnlistedCount", "Invalid enlisted count.");
                }
                else if (ex.Message.Contains("Total personnel"))
                {
                    TempData["Error"] = "Total personnel must equal the sum of officer and enlisted counts.";
                    ModelState.AddModelError("TotalPersonnel", "Personnel counts don't match.");
                }
                else if (ex.Message.Contains("email"))
                {
                    TempData["Error"] = "Invalid email address format. Please enter a valid email.";
                    ModelState.AddModelError("Email", "Invalid email format.");
                }
                else if (ex.Message.Contains("Established date"))
                {
                    TempData["Error"] = "Established date cannot be in the future. Please select a valid date.";
                    ModelState.AddModelError("EstablishedDate", "Invalid date.");
                }
                else
                {
                    TempData["Error"] = "Some information you entered is not valid: " + ex.Message;
                }

                _logger.LogWarning("Battalion creation validation error: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to create battalion. Please check your information and try again. If the problem continues, contact support.";
                _logger.LogError(ex, "Error creating battalion: {Name}", battalionDto?.Name);
            }

            await LoadRangeSelectList(battalionDto?.RangeId);
            LoadBattalionTypeSelectList();
            LoadOperationalStatusSelectList();
            return View(battalionDto);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCodeFromType(BattalionType type)
        {
            try
            {
                var code = await _battalionService.GenerateBattalionCodeAsync(type);
                return Json(new { success = true, code = code });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating code for type: {Type}", type);
                return Json(new { success = false, message = "Could not generate code" });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var battalion = await _battalionService.GetBattalionByIdAsync(id);
                if (battalion == null)
                {
                    TempData["Error"] = "The battalion you're looking for was not found. It may have been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadRangeSelectList(battalion.RangeId);
                LoadBattalionTypeSelectList();
                LoadOperationalStatusSelectList();
                return View(battalion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading battalion for edit with ID {Id}", id);
                TempData["Error"] = "Unable to load the battalion for editing. Please try again or contact support.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BattalionDto battalionDto)
        {
            if (id != battalionDto.Id)
            {
                TempData["Error"] = "Invalid battalion information. Please try again or contact support if the problem persists.";
                return NotFound();
            }

            try
            {
                // Get original battalion for comparison
                var originalBattalion = await _battalionService.GetBattalionByIdAsync(id);
                if (originalBattalion == null)
                {
                    TempData["Error"] = "The battalion you're trying to edit was not found. It may have been deleted by another user.";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    battalionDto.UpdatedBy = userId ?? "System";

                    await _battalionService.UpdateBattalionAsync(battalionDto);

                    // Success message with specific details about what changed
                    var changes = new List<string>();
                    if (originalBattalion.Name != battalionDto.Name) changes.Add("name");
                    if (originalBattalion.CommanderName != battalionDto.CommanderName) changes.Add("commander");
                    if (originalBattalion.CommanderRank != battalionDto.CommanderRank) changes.Add("rank");
                    if (originalBattalion.Location != battalionDto.Location) changes.Add("location");
                    if (originalBattalion.IsActive != battalionDto.IsActive) changes.Add("status");
                    if (originalBattalion.TotalPersonnel != battalionDto.TotalPersonnel) changes.Add("personnel");

                    string changeText = changes.Any() ? $" ({string.Join(", ", changes)} updated)" : "";
                    TempData["Success"] = $"Battalion '{battalionDto.Name}' has been successfully updated{changeText}!";

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
                if (ex.Message.Contains("Battalion with name") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"Cannot update battalion because another battalion with the name '{battalionDto.Name}' already exists. Please choose a different name.";
                    ModelState.AddModelError("Name", "This battalion name is already taken by another battalion.");
                }
                else if (ex.Message.Contains("Battalion with code") && ex.Message.Contains("already exists"))
                {
                    TempData["Error"] = $"Cannot update battalion because the code '{battalionDto.Code}' is already in use by another battalion. Please choose a different code.";
                    ModelState.AddModelError("Code", "This code is already in use by another battalion.");
                }
                else if (ex.Message.Contains("Range with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The selected range is not valid. Please select a valid range.";
                    ModelState.AddModelError("RangeId", "Invalid range selection.");
                }
                else if (ex.Message.Contains("Battalion with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The battalion you're trying to update was not found. It may have been deleted by another user while you were editing.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // For any other InvalidOperationException, show the actual message as it's likely user-friendly
                    TempData["Error"] = "Unable to update the battalion: " + ex.Message;
                }

                _logger.LogWarning("Battalion update failed: {Message} (Battalion ID: {Id}, User: {User})", ex.Message, id, User.Identity?.Name);
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors with user-friendly messages
                if (ex.Message.Contains("Battalion name"))
                {
                    TempData["Error"] = "Battalion name is required and cannot be empty. Please enter a valid battalion name.";
                    ModelState.AddModelError("Name", "Battalion name is required.");
                }
                else if (ex.Message.Contains("Battalion code"))
                {
                    TempData["Error"] = "Battalion code is required and cannot be empty. Please enter a valid battalion code.";
                    ModelState.AddModelError("Code", "Battalion code is required.");
                }
                else if (ex.Message.Contains("Officer count"))
                {
                    TempData["Error"] = "Officer count cannot be negative. Please enter a valid number.";
                    ModelState.AddModelError("OfficerCount", "Invalid officer count.");
                }
                else if (ex.Message.Contains("Enlisted count"))
                {
                    TempData["Error"] = "Enlisted count cannot be negative. Please enter a valid number.";
                    ModelState.AddModelError("EnlistedCount", "Invalid enlisted count.");
                }
                else if (ex.Message.Contains("Total personnel"))
                {
                    TempData["Error"] = "Total personnel must equal the sum of officer and enlisted counts.";
                    ModelState.AddModelError("TotalPersonnel", "Personnel counts don't match.");
                }
                else if (ex.Message.Contains("email"))
                {
                    TempData["Error"] = "Invalid email address format. Please enter a valid email.";
                    ModelState.AddModelError("Email", "Invalid email format.");
                }
                else if (ex.Message.Contains("Established date"))
                {
                    TempData["Error"] = "Established date cannot be in the future. Please select a valid date.";
                    ModelState.AddModelError("EstablishedDate", "Invalid date.");
                }
                else
                {
                    TempData["Error"] = "Some information you entered is not valid: " + ex.Message;
                }

                _logger.LogWarning("Battalion update validation error: {Message} (Battalion ID: {Id})", ex.Message, id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "We're experiencing technical difficulties while saving your changes. Please try again in a few moments. If the problem continues, contact system administrator.";
                _logger.LogError(ex, "Unexpected error updating battalion: {Id} by user {User}", id, User.Identity?.Name);
            }

            await LoadRangeSelectList(battalionDto?.RangeId);
            LoadBattalionTypeSelectList();
            LoadOperationalStatusSelectList();
            return View(battalionDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Get battalion details first for better messaging
                var battalion = await _battalionService.GetBattalionByIdAsync(id);
                if (battalion == null)
                {
                    TempData["Error"] = "The battalion you're trying to delete was not found. It may have already been deleted.";
                    return RedirectToAction(nameof(Index));
                }

                var battalionName = battalion.Name;
                var battalionCode = battalion.Code;

                // Attempt to delete
                await _battalionService.DeleteBattalionAsync(id);

                TempData["Success"] = $"Battalion '{battalionName}' ({battalionCode}) has been successfully deleted.";
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("stores assigned"))
                {
                    TempData["Error"] = "This battalion cannot be deleted because it has stores assigned to it. " +
                                      "Please remove the store assignments first.";
                }
                else if (ex.Message.Contains("users assigned"))
                {
                    TempData["Error"] = "This battalion cannot be deleted because it has users assigned to it. " +
                                      "Please reassign the users to another battalion first.";
                }
                else if (ex.Message.Contains("Battalion with ID") && ex.Message.Contains("not found"))
                {
                    TempData["Error"] = "The battalion you're trying to delete was not found. It may have already been deleted by another user.";
                }
                else
                {
                    TempData["Error"] = "Unable to delete this battalion: " + ex.Message;
                }

                _logger.LogWarning("Battalion deletion failed: {Message} (Battalion ID: {Id})", ex.Message, id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "We encountered an unexpected error while trying to delete the battalion. " +
                                  "Please try again in a few moments. If the problem persists, contact system support.";

                _logger.LogError(ex, "Unexpected error deleting battalion with ID {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AssignStore(int id)
        {
            try
            {
                var battalion = await _battalionService.GetBattalionByIdAsync(id);
                if (battalion == null)
                {
                    return NotFound();
                }

                ViewBag.BattalionId = id;
                ViewBag.BattalionName = battalion.Name;

                // Get stores not already assigned to this battalion
                var allStores = await _storeService.GetAllStoresAsync();
                var battalionStores = await _battalionService.GetBattalionStoresAsync(id);
                var availableStores = allStores.Where(s => !battalionStores.Any(bs => bs.Id == s.Id));

                ViewBag.Stores = new SelectList(availableStores, "Id", "Name");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assign store view for battalion {Id}", id);
                TempData["Error"] = "An error occurred while loading the page.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStore(int battalionId, int? storeId, bool isPrimary)
        {
            try
            {
                await _battalionService.AssignStoreToBattalionAsync(battalionId, storeId, isPrimary);
                TempData["Success"] = "Store assigned successfully!";
                return RedirectToAction(nameof(Details), new { id = battalionId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id = battalionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning store {StoreId} to battalion {BattalionId}", storeId, battalionId);
                TempData["Error"] = "An error occurred while assigning the store.";
                return RedirectToAction(nameof(Details), new { id = battalionId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStore(int battalionId, int? storeId)
        {
            try
            {
                await _battalionService.RemoveStoreFromBattalionAsync(battalionId, storeId);
                TempData["Success"] = "Store removed successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing store {StoreId} from battalion {BattalionId}", storeId, battalionId);
                TempData["Error"] = "An error occurred while removing the store.";
            }

            return RedirectToAction(nameof(Details), new { id = battalionId });
        }

        // AJAX endpoints
        [HttpGet]
        public async Task<IActionResult> GetBattalionsByRange(int rangeId)
        {
            try
            {
                var battalions = await _battalionService.GetBattalionsByRangeAsync(rangeId);
                return Json(battalions.Select(b => new { value = b.Id, text = b.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting battalions by range {RangeId}", rangeId);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckCodeExists(string code, int? excludeId = null)
        {
            try
            {
                var exists = await _battalionService.BattalionCodeExistsAsync(code, excludeId);
                return Json(!exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if code exists: {Code}", code);
                return Json(false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                var battalions = await _battalionService.SearchBattalionsAsync(searchTerm);
                return PartialView("_BattalionList", battalions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching battalions with term '{SearchTerm}'", searchTerm);
                return PartialView("_BattalionList", new List<BattalionDto>());
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

        private void LoadBattalionTypeSelectList()
        {
            ViewBag.BattalionTypes = new SelectList(
                Enum.GetValues(typeof(BattalionType))
                    .Cast<BattalionType>()
                    .Select(t => new { Value = (int)t, Text = t.ToString() }),
                "Value",
                "Text"
            );
        }

        private void LoadOperationalStatusSelectList()
        {
            ViewBag.OperationalStatuses = new SelectList(
                Enum.GetValues(typeof(OperationalStatus))
                    .Cast<OperationalStatus>()
                    .Select(s => new { Value = (int)s, Text = s.ToString() }),
                "Value",
                "Text"
            );
        }

        // ==================== EXPORT OPERATIONS ====================

        [HttpGet]
        [HasPermission(Permission.ViewAllBattalions)]
        public async Task<IActionResult> ExportToCsv(string status = null)
        {
            try
            {
                var battalions = await _battalionService.GetAllBattalionsAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        battalions = battalions.Where(b => b.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        battalions = battalions.Where(b => !b.IsActive);
                }

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Code,Name,Range,Commander,Contact,Location,Strength,Status");

                foreach (var battalion in battalions)
                {
                    csv.AppendLine($"\"{EscapeCsv(battalion.Code)}\"," +
                        $"\"{EscapeCsv(battalion.Name)}\"," +
                        $"\"{EscapeCsv(battalion.RangeName)}\"," +
                        $"\"{EscapeCsv(battalion.CommanderName)}\"," +
                        $"\"{EscapeCsv(battalion.ContactNumber)}\"," +
                        $"\"{EscapeCsv(battalion.Location)}\"," +
                        $"{battalion.Strength ?? 0}," +
                        $"\"{(battalion.IsActive ? "Active" : "Inactive")}\"");
                }

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"Battalions_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting battalions to CSV");
                TempData["Error"] = "Error exporting data to CSV.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewAllBattalions)]
        public async Task<IActionResult> ExportToPdf(string status = null)
        {
            try
            {
                var battalions = await _battalionService.GetAllBattalionsAsync();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        battalions = battalions.Where(b => b.IsActive);
                    else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                        battalions = battalions.Where(b => !b.IsActive);
                }

                using (var memoryStream = new System.IO.MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
                    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18);
                    var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);

                    var titleParagraph = new iTextSharp.text.Paragraph("ANSAR & VDP - Battalions Report", titleFont);
                    titleParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    titleParagraph.SpacingAfter = 10f;
                    document.Add(titleParagraph);

                    var infoParagraph = new iTextSharp.text.Paragraph($"Report Generated: {DateTime.Now:dd-MMM-yyyy HH:mm} | Total: {battalions.Count()}", normalFont);
                    infoParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    infoParagraph.SpacingAfter = 15f;
                    document.Add(infoParagraph);

                    var mainTable = new iTextSharp.text.pdf.PdfPTable(7);
                    mainTable.WidthPercentage = 100;
                    mainTable.SetWidths(new float[] { 10f, 20f, 15f, 15f, 12f, 10f, 10f });

                    var headerTexts = new[] { "Code", "Name", "Range", "Commander", "Contact", "Strength", "Status" };
                    foreach (var headerText in headerTexts)
                    {
                        var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(headerText, headerFont));
                        cell.BackgroundColor = new iTextSharp.text.BaseColor(220, 220, 220);
                        cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                        cell.Padding = 5f;
                        mainTable.AddCell(cell);
                    }

                    foreach (var battalion in battalions)
                    {
                        mainTable.AddCell(new iTextSharp.text.Phrase(battalion.Code ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(battalion.Name ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(battalion.RangeName ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(battalion.CommanderName ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(battalion.ContactNumber ?? "", normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase((battalion.Strength ?? 0).ToString(), normalFont));
                        mainTable.AddCell(new iTextSharp.text.Phrase(battalion.IsActive ? "Active" : "Inactive", normalFont));
                    }

                    document.Add(mainTable);

                    var footerParagraph = new iTextSharp.text.Paragraph($"\nGenerated by: IMS System | Date: {DateTime.Now:dd-MMM-yyyy HH:mm}",
                        iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8));
                    footerParagraph.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    footerParagraph.SpacingBefore = 20f;
                    document.Add(footerParagraph);

                    document.Close();
                    return File(memoryStream.ToArray(), "application/pdf", $"Battalions_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting battalions to PDF");
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