using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Application.Helpers;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace IMS.Web.Controllers
{
    [Authorize]
    public class AllotmentLetterController : Controller
    {
        private readonly IAllotmentLetterService _allotmentLetterService;
        private readonly IStoreService _storeService;
        private readonly IBattalionService _battalionService;
        private readonly IRangeService _rangeService;
        private readonly IItemService _itemService;
        private readonly ILogger<AllotmentLetterController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingService _settingService;
        private readonly ISignatoryPresetService _signatoryPresetService;

        public AllotmentLetterController(
            IAllotmentLetterService allotmentLetterService,
            IStoreService storeService,
            IBattalionService battalionService,
            IRangeService rangeService,
            IItemService itemService,
            ILogger<AllotmentLetterController> logger,
            IUnitOfWork unitOfWork,
            ISettingService settingService,
            ISignatoryPresetService signatoryPresetService)
        {
            _allotmentLetterService = allotmentLetterService;
            _storeService = storeService;
            _battalionService = battalionService;
            _rangeService = rangeService;
            _itemService = itemService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _settingService = settingService;
            _signatoryPresetService = signatoryPresetService;
        }

        [HasPermission(Permission.ViewAllotmentLetter)]
        public async Task<IActionResult> Index(
            DateTime? dateFrom,
            DateTime? dateTo,
            string status,
            int? storeId,
            string allotmentNo,
            string issuedTo,
            string showExpired)
        {
            var allotments = await _allotmentLetterService.GetAllAllotmentLettersAsync();

            // Apply filters
            if (dateFrom.HasValue)
            {
                allotments = allotments.Where(a => a.AllotmentDate >= dateFrom.Value).ToList();
                ViewBag.DateFrom = dateFrom.Value.ToString("yyyy-MM-dd");
            }

            if (dateTo.HasValue)
            {
                allotments = allotments.Where(a => a.AllotmentDate <= dateTo.Value).ToList();
                ViewBag.DateTo = dateTo.Value.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrEmpty(status))
            {
                allotments = allotments.Where(a => a.Status == status).ToList();
                ViewBag.Status = status;
            }

            if (storeId.HasValue)
            {
                allotments = allotments.Where(a => a.FromStoreId == storeId.Value).ToList();
                ViewBag.StoreId = storeId.Value.ToString();
            }

            if (!string.IsNullOrEmpty(allotmentNo))
            {
                allotments = allotments.Where(a => a.AllotmentNo != null && a.AllotmentNo.Contains(allotmentNo, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.AllotmentNo = allotmentNo;
            }

            if (!string.IsNullOrEmpty(issuedTo))
            {
                allotments = allotments.Where(a =>
                    (a.IssuedTo != null && a.IssuedTo.Contains(issuedTo, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IssuedToBattalionName != null && a.IssuedToBattalionName.Contains(issuedTo, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IssuedToRangeName != null && a.IssuedToRangeName.Contains(issuedTo, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IssuedToZilaName != null && a.IssuedToZilaName.Contains(issuedTo, StringComparison.OrdinalIgnoreCase))
                ).ToList();
                ViewBag.IssuedTo = issuedTo;
            }

            if (!string.IsNullOrEmpty(showExpired))
            {
                if (showExpired == "true")
                {
                    allotments = allotments.Where(a => a.IsExpired).ToList();
                }
                else if (showExpired == "false")
                {
                    allotments = allotments.Where(a => !a.IsExpired).ToList();
                }
                ViewBag.ShowExpired = showExpired;
            }

            // Load stores for dropdown
            var stores = await _storeService.GetAllStoresAsync();
            ViewBag.Stores = new SelectList(stores.Select(s => new { Value = s.Id, Text = s.Name }), "Value", "Text");

            return View(allotments);
        }

        [HasPermission(Permission.CreateAllotmentLetter)]
        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();

            var model = new AllotmentLetterDto
            {
                AllotmentDate = DateTime.Now,
                ValidFrom = DateTime.Now,
                ValidUntil = DateTime.Now.AddMonths(3)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateAllotmentLetter)]
        public async Task<IActionResult> Create(AllotmentLetterDto dto, IFormFile documentFile)
        {
            Console.WriteLine("=== CREATE ALLOTMENT LETTER - START ===");
            Console.WriteLine($"ModelState Valid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== MODEL STATE ERRORS ===");
                foreach (var modelError in ModelState)
                {
                    if (modelError.Value.Errors.Any())
                    {
                        Console.WriteLine($"Key: {modelError.Key}");
                        foreach (var error in modelError.Value.Errors)
                        {
                            Console.WriteLine($"  Error: {error.ErrorMessage}");
                            if (error.Exception != null)
                            {
                                Console.WriteLine($"  Exception: {error.Exception.Message}");
                            }
                        }
                    }
                }

                await LoadViewBagData();
                return View(dto);
            }

            try
            {
                Console.WriteLine($"FromStoreId: {dto.FromStoreId}");
                Console.WriteLine($"ValidFrom: {dto.ValidFrom}");
                Console.WriteLine($"ValidUntil: {dto.ValidUntil}");
                Console.WriteLine($"Purpose: {dto.Purpose}");
                Console.WriteLine($"ReferenceNo (Before Trim): '{dto.ReferenceNo}'");
                Console.WriteLine($"Recipients Count: {dto.Recipients?.Count ?? 0}");
                Console.WriteLine($"DistributionList Count: {dto.DistributionList?.Count ?? 0}");

                // IMPORTANT: Remove all whitespace from ReferenceNo (স্মারক নং)
                if (!string.IsNullOrEmpty(dto.ReferenceNo))
                {
                    dto.ReferenceNo = System.Text.RegularExpressions.Regex.Replace(dto.ReferenceNo, @"\s+", "");
                    Console.WriteLine($"ReferenceNo (After Trim): '{dto.ReferenceNo}'");
                }

                // Validate Recipients
                if (dto.Recipients == null || !dto.Recipients.Any())
                {
                    Console.WriteLine("ERROR: No recipients provided!");
                    TempData["Error"] = "Please add at least one recipient with items.";
                    await LoadViewBagData();
                    return View(dto);
                }

                // IMPORTANT: Populate Items from Recipients for government format support
                // Items list represents table column headers, Recipients.Items contain actual quantities per recipient
                if (dto.Recipients != null && dto.Recipients.Any())
                {
                    var uniqueItems = new Dictionary<int, AllotmentLetterItemDto>();

                    foreach (var recipient in dto.Recipients)
                    {
                        Console.WriteLine($"Processing Recipient: {recipient.RecipientName}, Items: {recipient.Items?.Count ?? 0}");

                        if (recipient.Items != null)
                        {
                            foreach (var recipientItem in recipient.Items)
                            {
                                Console.WriteLine($"  Item ID: {recipientItem.ItemId}, Qty: {recipientItem.AllottedQuantity}");

                                if (!uniqueItems.ContainsKey(recipientItem.ItemId))
                                {
                                    uniqueItems[recipientItem.ItemId] = new AllotmentLetterItemDto
                                    {
                                        ItemId = recipientItem.ItemId,
                                        ItemName = recipientItem.ItemName,
                                        ItemNameBn = recipientItem.ItemNameBn,
                                        Unit = recipientItem.Unit,
                                        UnitBn = recipientItem.UnitBn,
                                        AllottedQuantity = 0 // Will be calculated from recipients
                                    };
                                }

                                // Sum up quantities from all recipients
                                uniqueItems[recipientItem.ItemId].AllottedQuantity += recipientItem.AllottedQuantity;
                            }
                        }
                    }

                    dto.Items = uniqueItems.Values.ToList();
                    Console.WriteLine($"Total unique items aggregated: {dto.Items.Count}");
                }

                // Log Distribution List
                if (dto.DistributionList != null && dto.DistributionList.Any())
                {
                    Console.WriteLine("=== DISTRIBUTION LIST ===");
                    foreach (var distEntry in dto.DistributionList)
                    {
                        Console.WriteLine($"  {distEntry.SerialNo}. {distEntry.RecipientTitleBn ?? distEntry.RecipientTitle}");
                    }
                }

                // Handle document upload
                if (documentFile != null && documentFile.Length > 0)
                {
                    Console.WriteLine($"Document upload: {documentFile.FileName}, Size: {documentFile.Length} bytes");
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "allotments");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{documentFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await documentFile.CopyToAsync(stream);
                    }

                    dto.DocumentPath = $"/uploads/allotments/{uniqueFileName}";
                    Console.WriteLine($"Document saved: {dto.DocumentPath}");
                }

                dto.CreatedBy = User.Identity.Name;
                Console.WriteLine($"CreatedBy: {dto.CreatedBy}");

                Console.WriteLine("Calling CreateAllotmentLetterAsync...");
                var result = await _allotmentLetterService.CreateAllotmentLetterAsync(dto);
                Console.WriteLine($"SUCCESS! Created Allotment Letter #{result.AllotmentNo}, ID: {result.Id}");

                TempData["Success"] = $"Allotment Letter #{result.AllotmentNo} created successfully!";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== EXCEPTION OCCURRED ===");
                Console.WriteLine($"Exception Type: {ex.GetType().FullName}");
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine("=== INNER EXCEPTION ===");
                    Console.WriteLine($"Inner Exception Type: {ex.InnerException.GetType().FullName}");
                    Console.WriteLine($"Inner Exception Message: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Stack Trace:\n{ex.InnerException.StackTrace}");
                }

                _logger.LogError(ex, "Error creating allotment letter: {Message}", ex.Message);
                TempData["Error"] = $"Failed to create allotment letter: {ex.Message}";
                await LoadViewBagData();
                return View(dto);
            }
            finally
            {
                Console.WriteLine("=== CREATE ALLOTMENT LETTER - END ===");
            }
        }

        [HasPermission(Permission.ViewAllotmentLetter)]
        public async Task<IActionResult> Details(int id)
        {
            var allotment = await _allotmentLetterService.GetAllotmentLetterByIdAsync(id);

            if (allotment == null)
            {
                TempData["Error"] = "Allotment Letter not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(allotment);
        }

        [HasPermission(Permission.EditAllotmentLetter)]
        public async Task<IActionResult> Edit(int id)
        {
            var allotment = await _allotmentLetterService.GetAllotmentLetterByIdAsync(id);

            if (allotment == null)
            {
                TempData["Error"] = "Allotment Letter not found.";
                return RedirectToAction(nameof(Index));
            }

            if (allotment.Status != "Draft")
            {
                TempData["Error"] = "Only draft allotment letters can be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await LoadViewBagData();
            return View(allotment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.EditAllotmentLetter)]
        public async Task<IActionResult> Edit(int id, AllotmentLetterDto dto, IFormFile documentFile)
        {
            Console.WriteLine("=== EDIT ALLOTMENT LETTER - START ===");
            Console.WriteLine($"Allotment ID: {id}");
            Console.WriteLine($"DTO ID: {dto.Id}");

            if (id != dto.Id)
            {
                Console.WriteLine("ERROR: ID mismatch!");
                return NotFound();
            }

            Console.WriteLine($"ModelState Valid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== MODEL STATE ERRORS ===");
                foreach (var modelError in ModelState)
                {
                    if (modelError.Value.Errors.Any())
                    {
                        Console.WriteLine($"Key: {modelError.Key}");
                        foreach (var error in modelError.Value.Errors)
                        {
                            Console.WriteLine($"  Error: {error.ErrorMessage}");
                            if (error.Exception != null)
                            {
                                Console.WriteLine($"  Exception: {error.Exception.Message}");
                            }
                        }
                    }
                }

                await LoadViewBagData();
                return View(dto);
            }

            try
            {
                Console.WriteLine($"FromStoreId: {dto.FromStoreId}");
                Console.WriteLine($"ReferenceNo (Before Trim): '{dto.ReferenceNo}'");
                Console.WriteLine($"Recipients Count: {dto.Recipients?.Count ?? 0}");
                Console.WriteLine($"DistributionList Count: {dto.DistributionList?.Count ?? 0}");

                // IMPORTANT: Remove all whitespace from ReferenceNo (স্মারক নং)
                if (!string.IsNullOrEmpty(dto.ReferenceNo))
                {
                    dto.ReferenceNo = System.Text.RegularExpressions.Regex.Replace(dto.ReferenceNo, @"\s+", "");
                    Console.WriteLine($"ReferenceNo (After Trim): '{dto.ReferenceNo}'");
                }

                // Validate Recipients
                if (dto.Recipients == null || !dto.Recipients.Any())
                {
                    Console.WriteLine("ERROR: No recipients provided!");
                    TempData["Error"] = "Please add at least one recipient with items.";
                    await LoadViewBagData();
                    return View(dto);
                }

                // IMPORTANT: Populate Items from Recipients for government format support
                // Items list represents table column headers, Recipients.Items contain actual quantities per recipient
                if (dto.Recipients != null && dto.Recipients.Any())
                {
                    var uniqueItems = new Dictionary<int, AllotmentLetterItemDto>();

                    foreach (var recipient in dto.Recipients)
                    {
                        Console.WriteLine($"Processing Recipient: {recipient.RecipientName}, Items: {recipient.Items?.Count ?? 0}");

                        if (recipient.Items != null)
                        {
                            foreach (var recipientItem in recipient.Items)
                            {
                                Console.WriteLine($"  Item ID: {recipientItem.ItemId}, Qty: {recipientItem.AllottedQuantity}");

                                if (!uniqueItems.ContainsKey(recipientItem.ItemId))
                                {
                                    uniqueItems[recipientItem.ItemId] = new AllotmentLetterItemDto
                                    {
                                        ItemId = recipientItem.ItemId,
                                        ItemName = recipientItem.ItemName,
                                        ItemNameBn = recipientItem.ItemNameBn,
                                        Unit = recipientItem.Unit,
                                        UnitBn = recipientItem.UnitBn,
                                        AllottedQuantity = 0 // Will be calculated from recipients
                                    };
                                }

                                // Sum up quantities from all recipients
                                uniqueItems[recipientItem.ItemId].AllottedQuantity += recipientItem.AllottedQuantity;
                            }
                        }
                    }

                    dto.Items = uniqueItems.Values.ToList();
                    Console.WriteLine($"Total unique items aggregated: {dto.Items.Count}");
                }

                // Log Distribution List
                if (dto.DistributionList != null && dto.DistributionList.Any())
                {
                    Console.WriteLine("=== DISTRIBUTION LIST ===");
                    foreach (var distEntry in dto.DistributionList)
                    {
                        Console.WriteLine($"  {distEntry.SerialNo}. {distEntry.RecipientTitleBn ?? distEntry.RecipientTitle}");
                    }
                }

                // Handle document upload
                if (documentFile != null && documentFile.Length > 0)
                {
                    Console.WriteLine($"Document upload: {documentFile.FileName}, Size: {documentFile.Length} bytes");
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "allotments");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{documentFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await documentFile.CopyToAsync(stream);
                    }

                    dto.DocumentPath = $"/uploads/allotments/{uniqueFileName}";
                    Console.WriteLine($"Document saved: {dto.DocumentPath}");
                }

                dto.UpdatedBy = User.Identity.Name;
                Console.WriteLine($"UpdatedBy: {dto.UpdatedBy}");

                Console.WriteLine("Calling UpdateAllotmentLetterAsync...");
                var result = await _allotmentLetterService.UpdateAllotmentLetterAsync(dto);
                Console.WriteLine($"SUCCESS! Updated Allotment Letter ID: {result.Id}");

                TempData["Success"] = "Allotment Letter updated successfully!";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== EXCEPTION OCCURRED ===");
                Console.WriteLine($"Exception Type: {ex.GetType().FullName}");
                Console.WriteLine($"Exception Message: {ex.Message}");
                Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine("=== INNER EXCEPTION ===");
                    Console.WriteLine($"Inner Exception Type: {ex.InnerException.GetType().FullName}");
                    Console.WriteLine($"Inner Exception Message: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Stack Trace:\n{ex.InnerException.StackTrace}");
                }

                _logger.LogError(ex, "Error updating allotment letter: {Message}", ex.Message);
                TempData["Error"] = $"Failed to update allotment letter: {ex.Message}";
                await LoadViewBagData();
                return View(dto);
            }
            finally
            {
                Console.WriteLine("=== EDIT ALLOTMENT LETTER - END ===");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateAllotmentLetter)]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            Console.WriteLine($"=== SUBMIT FOR APPROVAL - ID: {id} ===");

            try
            {
                var result = await _allotmentLetterService.SubmitForApprovalAsync(id, User.Identity.Name);
                Console.WriteLine($"Submit result - Success: {result.Success}, Message: {result.Message}");

                if (result.Success)
                {
                    TempData["Success"] = "Allotment Letter submitted for approval successfully!";
                    return Json(new { success = true, message = "Allotment Letter submitted for approval successfully!" });
                }
                else
                {
                    TempData["Error"] = result.Message ?? "Failed to submit allotment letter for approval.";
                    return Json(new { success = false, message = result.Message ?? "Failed to submit allotment letter for approval." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== EXCEPTION in SubmitForApproval ===");
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                _logger.LogError(ex, "Error submitting allotment letter {Id} for approval", id);
                TempData["Error"] = "An error occurred while submitting for approval.";
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        [HasPermission(Permission.ApproveAllotmentLetter)]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var result = await _allotmentLetterService.ApproveAllotmentLetterAsync(id, User.Identity.Name);

                if (result)
                {
                    TempData["Success"] = "Allotment Letter approved successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to approve allotment letter.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving allotment letter {Id}", id);
                TempData["Error"] = "An error occurred while approving.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        [HasPermission(Permission.ViewAllotmentLetter)]
        public async Task<IActionResult> Print(int id)
        {
            var allotment = await _allotmentLetterService.GetAllotmentLetterByIdAsync(id);
            if (allotment == null)
            {
                TempData["Error"] = "Allotment Letter not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("Print", allotment);
        }

        [HttpGet]
        [HasPermission(Permission.ViewAllotmentLetter)]
        public async Task<IActionResult> PrintGovernment(int id)
        {
            var allotment = await _allotmentLetterService.GetAllotmentLetterByIdAsync(id);
            if (allotment == null)
            {
                TempData["Error"] = "Allotment Letter not found.";
                return RedirectToAction(nameof(Index));
            }

            // Set default values if not already set
            if (string.IsNullOrEmpty(allotment.SignatoryName))
                allotment.SignatoryName = "এ.বি.এম. ফরহাদ, বিবিএম";

            if (string.IsNullOrEmpty(allotment.SignatoryDesignationBn))
                allotment.SignatoryDesignationBn = "উপপরিচালক (ভান্ডার)";

            if (string.IsNullOrEmpty(allotment.SignatoryId))
                allotment.SignatoryId = "বিএমভি-১২০২১৮";

            if (string.IsNullOrEmpty(allotment.SignatoryEmail))
                allotment.SignatoryEmail = "ddstore@ansarvdp.gov.bd";

            if (string.IsNullOrEmpty(allotment.SignatoryPhone))
                allotment.SignatoryPhone = "০২-৭২১৩৪০০";

            // Generate Bengali date if not set
            if (string.IsNullOrEmpty(allotment.BengaliDate))
            {
                allotment.BengaliDate = ConvertToBengaliDate(allotment.AllotmentDate);
            }

            return View("PrintGovernment", allotment);
        }

        [HttpGet]
        [HasPermission(Permission.ViewAllotmentLetter)]
        public async Task<IActionResult> ExportPDF(int id)
        {
            try
            {
                var allotment = await _allotmentLetterService.GetAllotmentLetterByIdAsync(id);
                if (allotment == null)
                {
                    TempData["Error"] = "Allotment Letter not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Auto-generate Bengali date if not set
                EnsureBengaliDate(allotment);

                // Return view that will trigger browser print dialog
                ViewBag.AutoPrint = true;
                ViewBag.FileName = $"Allotment_{allotment.AllotmentNo}_{DateTime.Now:yyyyMMdd}.pdf";
                return View("PrintPDF", allotment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting allotment letter {Id} to PDF", id);
                TempData["Error"] = "Failed to export PDF. " + ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // Helper method to auto-generate Bengali date if not provided
        private void EnsureBengaliDate(AllotmentLetterDto allotment)
        {
            if (string.IsNullOrEmpty(allotment.BengaliDate))
                allotment.BengaliDate = ConvertToBengaliDate(allotment.AllotmentDate);
        }

        // Helper method to convert English date to Bengali date
        // Returns both Bengali calendar and Gregorian date in Bengali format
        private string ConvertToBengaliDate(DateTime date)
        {
            return BengaliDateHelper.GetCompleteDateString(date);
        }

        // API Endpoints for cascading dropdowns
        [HttpGet]
        public async Task<IActionResult> GetBattalionsByRange(int rangeId)
        {
            var battalions = await _battalionService.GetActiveBattalionsAsync();
            var filtered = battalions.Where(b => b.RangeId == rangeId)
                .Select(b => new { id = b.Id, name = b.Name })
                .ToList();
            return Json(filtered);
        }

        [HttpGet]
        public async Task<IActionResult> GetUpazilasByZila(int zilaId)
        {
            var upazilas = await _unitOfWork.Upazilas.GetAllAsync();
            var filtered = upazilas.Where(u => u.ZilaId == zilaId)
                .Select(u => new { id = u.Id, name = u.Name })
                .ToList();
            return Json(filtered);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnionsByUpazila(int upazilaId)
        {
            var unions = await _unitOfWork.Unions.GetAllAsync();
            var filtered = unions.Where(u => u.UpazilaId == upazilaId)
                .Select(u => new { id = u.Id, name = u.Name })
                .ToList();
            return Json(filtered);
        }

        [HttpGet]
        public async Task<IActionResult> GetPresetsJson()
        {
            try
            {
                var presets = await _signatoryPresetService.GetActivePresetsAsync();
                var result = presets.Select(p => new
                {
                    id = p.Id,
                    presetName = p.PresetName,
                    presetNameBn = p.PresetNameBn,
                    signatoryName = p.SignatoryName,
                    signatoryDesignation = p.SignatoryDesignation,
                    signatoryDesignationBn = p.SignatoryDesignationBn,
                    signatoryId = p.SignatoryId,
                    signatoryPhone = p.SignatoryPhone,
                    signatoryEmail = p.SignatoryEmail,
                    isDefault = p.IsDefault
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching signatory presets");
                return Json(new List<object>());
            }
        }

        private async Task LoadViewBagData()
        {
            var stores = await _storeService.GetAllStoresAsync();

            // Try to filter Provision stores, but if none found, show all stores
            var provisionStores = stores.Where(s => s.StoreTypeName != null &&
                (s.StoreTypeName.Contains("Provision") || s.StoreTypeName.Contains("provision")));

            // If no provision stores found, show all stores
            var storesToShow = provisionStores.Any() ? provisionStores : stores;

            ViewBag.Stores = new SelectList(storesToShow, "Id", "Name");

            // Get data for dropdowns
            var battalions = await _battalionService.GetActiveBattalionsAsync();
            var ranges = await _rangeService.GetActiveRangesAsync();
            var zilas = await _unitOfWork.Zilas.GetAllAsync();
            var upazilas = await _unitOfWork.Upazilas.GetAllAsync();

            // SelectList for Edit form (backward compatibility)
            ViewBag.Battalions = new SelectList(battalions, "Id", "Name");
            ViewBag.Ranges = new SelectList(ranges, "Id", "Name");
            ViewBag.Zilas = new SelectList(zilas, "Id", "Name");
            ViewBag.Upazilas = new SelectList(upazilas, "Id", "Name");

            // Simple arrays for JavaScript (Create form) - all data for cascading
            ViewBag.BattalionsList = battalions.Select(b => new { id = b.Id, name = b.Name, rangeId = b.RangeId }).ToList();
            ViewBag.RangesList = ranges.Select(r => new { id = r.Id, name = r.Name }).ToList();
            ViewBag.ZilasList = zilas.Select(z => new { id = z.Id, name = z.Name, rangeId = z.RangeId }).ToList();
            ViewBag.UpazilasList = upazilas.Select(u => new { id = u.Id, name = u.Name, zilaId = u.ZilaId }).ToList();

            ViewBag.Items = await _itemService.GetAllItemsAsync();
        }

        // Export Methods
        [HttpGet]
        [HasPermission(Permission.ViewAllotmentLetter)]
        public async Task<IActionResult> ExportListPDF(
            DateTime? dateFrom,
            DateTime? dateTo,
            string status,
            int? storeId,
            string allotmentNo,
            string issuedTo,
            string showExpired)
        {
            try
            {
                var allotments = await GetFilteredAllotmentsAsync(dateFrom, dateTo, status, storeId, allotmentNo, issuedTo, showExpired);
                var pdfBytes = GenerateAllotmentListPDF(allotments);
                var fileName = $"AllotmentLetters_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF report");
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewAllotmentLetter)]
        public async Task<IActionResult> ExportCSV(
            DateTime? dateFrom,
            DateTime? dateTo,
            string status,
            int? storeId,
            string allotmentNo,
            string issuedTo,
            string showExpired)
        {
            var allotments = await GetFilteredAllotmentsAsync(dateFrom, dateTo, status, storeId, allotmentNo, issuedTo, showExpired);

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Allotment No,Date,Issued To,From Store,Total Items,Allotted Qty,Issued Qty,Remaining Qty,Valid Until,Status,Is Expired");

            foreach (var allotment in allotments)
            {
                var issuedToName = GetIssuedToName(allotment);
                csv.AppendLine($"\"{allotment.AllotmentNo}\",\"{allotment.AllotmentDate:dd-MMM-yyyy}\",\"{issuedToName}\",\"{allotment.FromStoreName}\",\"{allotment.TotalItems}\",\"{allotment.TotalQuantity}\",\"{allotment.TotalIssued}\",\"{allotment.TotalRemaining}\",\"{allotment.ValidUntil:dd-MMM-yyyy}\",\"{allotment.Status}\",\"{(allotment.IsExpired ? "Yes" : "No")}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"AllotmentLetters_{DateTime.Now:yyyyMMddHHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }

        [HttpGet]
        [HasPermission(Permission.ViewAllotmentLetter)]
        public async Task<IActionResult> ExportExcel(
            DateTime? dateFrom,
            DateTime? dateTo,
            string status,
            int? storeId,
            string allotmentNo,
            string issuedTo,
            string showExpired)
        {
            var allotments = await GetFilteredAllotmentsAsync(dateFrom, dateTo, status, storeId, allotmentNo, issuedTo, showExpired);

            // Set EPPlus license context
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Allotment Letters");

                // Header
                worksheet.Cells[1, 1].Value = "Allotment No";
                worksheet.Cells[1, 2].Value = "Date";
                worksheet.Cells[1, 3].Value = "Issued To";
                worksheet.Cells[1, 4].Value = "From Store";
                worksheet.Cells[1, 5].Value = "Total Items";
                worksheet.Cells[1, 6].Value = "Allotted Qty";
                worksheet.Cells[1, 7].Value = "Issued Qty";
                worksheet.Cells[1, 8].Value = "Remaining Qty";
                worksheet.Cells[1, 9].Value = "Valid Until";
                worksheet.Cells[1, 10].Value = "Status";
                worksheet.Cells[1, 11].Value = "Is Expired";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 11])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Data
                int row = 2;
                foreach (var allotment in allotments)
                {
                    var issuedToName = GetIssuedToName(allotment);
                    worksheet.Cells[row, 1].Value = allotment.AllotmentNo;
                    worksheet.Cells[row, 2].Value = allotment.AllotmentDate.ToString("dd-MMM-yyyy");
                    worksheet.Cells[row, 3].Value = issuedToName;
                    worksheet.Cells[row, 4].Value = allotment.FromStoreName;
                    worksheet.Cells[row, 5].Value = allotment.TotalItems;
                    worksheet.Cells[row, 6].Value = allotment.TotalQuantity;
                    worksheet.Cells[row, 7].Value = allotment.TotalIssued;
                    worksheet.Cells[row, 8].Value = allotment.TotalRemaining;
                    worksheet.Cells[row, 9].Value = allotment.ValidUntil.ToString("dd-MMM-yyyy");
                    worksheet.Cells[row, 10].Value = allotment.Status;
                    worksheet.Cells[row, 11].Value = allotment.IsExpired ? "Yes" : "No";
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var fileName = $"AllotmentLetters_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // Helper methods
        private async Task<IEnumerable<AllotmentLetterDto>> GetFilteredAllotmentsAsync(
            DateTime? dateFrom,
            DateTime? dateTo,
            string status,
            int? storeId,
            string allotmentNo,
            string issuedTo,
            string showExpired)
        {
            var allotments = await _allotmentLetterService.GetAllAllotmentLettersAsync();

            // Apply filters (same logic as Index)
            if (dateFrom.HasValue)
                allotments = allotments.Where(a => a.AllotmentDate >= dateFrom.Value).ToList();

            if (dateTo.HasValue)
                allotments = allotments.Where(a => a.AllotmentDate <= dateTo.Value).ToList();

            if (!string.IsNullOrEmpty(status))
                allotments = allotments.Where(a => a.Status == status).ToList();

            if (storeId.HasValue)
                allotments = allotments.Where(a => a.FromStoreId == storeId.Value).ToList();

            if (!string.IsNullOrEmpty(allotmentNo))
                allotments = allotments.Where(a => a.AllotmentNo != null && a.AllotmentNo.Contains(allotmentNo, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(issuedTo))
            {
                allotments = allotments.Where(a =>
                    (a.IssuedTo != null && a.IssuedTo.Contains(issuedTo, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IssuedToBattalionName != null && a.IssuedToBattalionName.Contains(issuedTo, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IssuedToRangeName != null && a.IssuedToRangeName.Contains(issuedTo, StringComparison.OrdinalIgnoreCase)) ||
                    (a.IssuedToZilaName != null && a.IssuedToZilaName.Contains(issuedTo, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrEmpty(showExpired))
            {
                if (showExpired == "true")
                    allotments = allotments.Where(a => a.IsExpired).ToList();
                else if (showExpired == "false")
                    allotments = allotments.Where(a => !a.IsExpired).ToList();
            }

            return allotments;
        }

        private string GetIssuedToName(AllotmentLetterDto allotment)
        {
            if (allotment.TotalRecipients > 0 && allotment.Recipients != null)
            {
                if (allotment.TotalRecipients == 1)
                {
                    var recipient = allotment.Recipients.FirstOrDefault();
                    return recipient?.RecipientName ?? recipient?.RecipientNameBn ?? "";
                }
                else
                {
                    return $"Multiple ({allotment.TotalRecipients})";
                }
            }
            else if (!string.IsNullOrEmpty(allotment.IssuedToBattalionName))
            {
                return allotment.IssuedToBattalionName;
            }
            else if (!string.IsNullOrEmpty(allotment.IssuedToRangeName))
            {
                return allotment.IssuedToRangeName;
            }
            else if (!string.IsNullOrEmpty(allotment.IssuedToZilaName))
            {
                return allotment.IssuedToZilaName;
            }
            else
            {
                return allotment.IssuedTo ?? "";
            }
        }

        private byte[] GenerateAllotmentListPDF(IEnumerable<AllotmentLetterDto> allotments)
        {
            using var ms = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 20, 20, 30, 30);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

            document.Open();

            // Organization Header
            var orgFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18, new iTextSharp.text.BaseColor(0, 106, 78));
            var orgName = new iTextSharp.text.Paragraph("Bangladesh Ansar & VDP", orgFont);
            orgName.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(orgName);

            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 14, new iTextSharp.text.BaseColor(64, 64, 64));
            var title = new iTextSharp.text.Paragraph("Allotment Letters Report", titleFont);
            title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            title.SpacingAfter = 10f;
            document.Add(title);

            var dateFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10, new iTextSharp.text.BaseColor(128, 128, 128));
            var dateInfo = new iTextSharp.text.Paragraph($"Generated: {DateTime.Now:dd MMMM yyyy HH:mm}", dateFont);
            dateInfo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            dateInfo.SpacingAfter = 15f;
            document.Add(dateInfo);

            // Create table
            var table = new iTextSharp.text.pdf.PdfPTable(10) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 10f, 8f, 15f, 12f, 5f, 7f, 7f, 8f, 8f, 8f });

            // Header font
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 9, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(255, 255, 255));
            var cellFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

            // Table headers
            var headers = new[] { "Allotment No", "Date", "Issued To", "From Store", "Items", "Allotted", "Issued", "Remaining", "Valid Until", "Status" };
            foreach (var header in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont))
                {
                    BackgroundColor = new iTextSharp.text.BaseColor(0, 106, 78),
                    HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER,
                    VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,
                    Padding = 5
                };
                table.AddCell(cell);
            }

            // Table data
            int totalItems = 0;
            decimal totalAllotted = 0, totalIssued = 0, totalRemaining = 0;

            foreach (var allotment in allotments)
            {
                var issuedToName = GetIssuedToName(allotment);

                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(allotment.AllotmentNo ?? "", cellFont)) { Padding = 4 });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(allotment.AllotmentDate.ToString("dd-MMM-yy"), cellFont)) { Padding = 4 });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(issuedToName, cellFont)) { Padding = 4 });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(allotment.FromStoreName ?? "", cellFont)) { Padding = 4 });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(allotment.TotalItems.ToString(), cellFont)) { Padding = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(allotment.TotalQuantity.ToString(), cellFont)) { Padding = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(allotment.TotalIssued.ToString(), cellFont)) { Padding = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(allotment.TotalRemaining.ToString(), cellFont)) { Padding = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });

                var validUntil = allotment.ValidUntil.ToString("dd-MMM-yy");
                if (allotment.IsExpired) validUntil += " (Expired)";
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(validUntil, cellFont)) { Padding = 4 });
                table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(allotment.Status ?? "", cellFont)) { Padding = 4, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER });

                totalItems += allotment.TotalItems;
                totalAllotted += allotment.TotalQuantity;
                totalIssued += allotment.TotalIssued;
                totalRemaining += allotment.TotalRemaining;
            }

            // Total row
            var totalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 9);
            table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase($"Total: {allotments.Count()} records", totalFont)) { Colspan = 4, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240) });
            table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(totalItems.ToString(), totalFont)) { Padding = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240) });
            table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(totalAllotted.ToString(), totalFont)) { Padding = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240) });
            table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(totalIssued.ToString(), totalFont)) { Padding = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240) });
            table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(totalRemaining.ToString(), totalFont)) { Padding = 5, HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER, BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240) });
            table.AddCell(new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("", totalFont)) { Colspan = 2, BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240) });

            document.Add(table);

            // Footer
            var footerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8, new iTextSharp.text.BaseColor(128, 128, 128));
            var footer = new iTextSharp.text.Paragraph($"\n\n© {DateTime.Now.Year} Bangladesh Ansar & VDP - Inventory Management System\nThis is a system-generated report", footerFont);
            footer.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            document.Add(footer);

            document.Close();
            return ms.ToArray();
        }
    }
}
