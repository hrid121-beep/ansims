using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class DamageController : Controller
    {
        private readonly IDamageService _damageService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly IUserContext _userContext;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly INotificationService _notificationService;
        private readonly ILogger<DamageController> _logger;

        public DamageController(
            IDamageService damageService,
            IItemService itemService,
            IStoreService storeService,
            IUserContext userContext,
            UserManager<User> userManager,
            IWebHostEnvironment environment,
            INotificationService notificationService,
            ILogger<DamageController> logger)
        {
            _damageService = damageService ?? throw new ArgumentNullException(nameof(damageService));
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region List and Search Actions

        /// <summary>
        /// Display list of damage records
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.ViewDamage)]
        public async Task<IActionResult> Index(string status = null, string damageType = null, int page = 1, int pageSize = 20)
        {
            try
            {
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.Status = status;
                ViewBag.DamageType = damageType;

                IEnumerable<DamageDto> damages;

                // Apply filters
                if (!string.IsNullOrWhiteSpace(damageType))
                {
                    damages = await _damageService.GetDamagesByTypeAsync(damageType);
                }
                else
                {
                    damages = await _damageService.GetAllDamagesAsync();
                }

                // Filter by status if specified
                if (!string.IsNullOrWhiteSpace(status))
                {
                    damages = damages.Where(d => d.Status == status);
                }

                // Get status counts
                var allDamages = await _damageService.GetAllDamagesAsync();
                ViewBag.TotalCount = allDamages.Count();
                ViewBag.PendingCount = allDamages.Count(d => d.Status == "Pending");
                ViewBag.ApprovedCount = allDamages.Count(d => d.Status == "Approved");
                ViewBag.UnderInvestigationCount = allDamages.Count(d => d.ActionTaken == "Under Investigation");
                ViewBag.WrittenOffCount = allDamages.Count(d => d.ActionTaken == "Write-off Recommended");

                // Get current user info
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(currentUser);
                    ViewBag.UserRoles = userRoles;
                    ViewBag.CurrentUserId = currentUser.Id;
                }

                // Apply pagination
                var totalItems = damages.Count();
                var pagedDamages = damages
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                return View(pagedDamages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading damages list");
                TempData["Error"] = "An error occurred while loading damage records.";
                return View(new List<DamageDto>());
            }
        }

        #endregion

        #region Create Actions

        /// <summary>
        /// Display create damage form
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.CreateDamage)]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.DamageNo = await _damageService.GenerateDamageNoAsync();
                await LoadViewBagData();

                var model = new DamageDto
                {
                    DamageDate = DateTime.Now,
                    Status = "Pending"
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create damage form");
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Process damage creation
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateDamage)]
        public async Task<IActionResult> Create(DamageDto damageDto, IFormFile photoFile)
        {
            try
            {
                // Remove validation for server-set fields
                ModelState.Remove("DamageNo");
                ModelState.Remove("Status");
                ModelState.Remove("CreatedBy");
                ModelState.Remove("CreatedAt");

                if (!ModelState.IsValid)
                {
                    await LoadViewBagData();
                    return View(damageDto);
                }

                // Handle photo upload
                if (photoFile != null && photoFile.Length > 0)
                {
                    var maxFileSize = 5 * 1024 * 1024; // 5MB
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                    if (photoFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("", "Photo file size must be less than 5MB.");
                        await LoadViewBagData();
                        return View(damageDto);
                    }

                    var extension = Path.GetExtension(photoFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Only JPG and PNG images are allowed.");
                        await LoadViewBagData();
                        return View(damageDto);
                    }

                    var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "damages");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photoFile.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(stream);
                    }

                    damageDto.PhotoPath = $"/uploads/damages/{fileName}";
                }

                var result = await _damageService.CreateDamageAsync(damageDto);

                if (result != null)
                {
                    TempData["Success"] = $"Damage record {result.DamageNo} created successfully.";
                    return RedirectToAction(nameof(Details), new { id = result.Id });
                }

                TempData["Error"] = "Failed to create damage record.";
                await LoadViewBagData();
                return View(damageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating damage record");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                await LoadViewBagData();
                return View(damageDto);
            }
        }

        /// <summary>
        /// Display multi-item damage creation form
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.CreateDamage)]
        public async Task<IActionResult> CreateMultiItem()
        {
            try
            {
                ViewBag.DamageNo = await _damageService.GenerateDamageNoAsync();
                await LoadViewBagData();

                var model = new DamageDto
                {
                    DamageDate = DateTime.Now,
                    Status = DamageStatus.Reported.ToString(),
                    Items = new List<DamageItemDto>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading multi-item damage form");
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Process multi-item damage creation
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateDamage)]
        public async Task<IActionResult> CreateMultiItem(DamageDto damageDto, List<IFormFile> itemPhotos)
        {
            try
            {
                // Remove validation for server-set fields
                ModelState.Remove("DamageNo");
                ModelState.Remove("ReportNo");
                ModelState.Remove("Status");
                ModelState.Remove("CreatedBy");
                ModelState.Remove("CreatedAt");

                if (!ModelState.IsValid)
                {
                    await LoadViewBagData();
                    return View(damageDto);
                }

                // Validate items
                if (damageDto.Items == null || !damageDto.Items.Any())
                {
                    ModelState.AddModelError("", "At least one damage item is required.");
                    await LoadViewBagData();
                    return View(damageDto);
                }

                // Handle photo uploads for items
                if (itemPhotos != null && itemPhotos.Any())
                {
                    var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "damages");
                    Directory.CreateDirectory(uploadsPath);

                    var maxFileSize = 5 * 1024 * 1024; // 5MB
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

                    for (int i = 0; i < itemPhotos.Count && i < damageDto.Items.Count; i++)
                    {
                        var photoFile = itemPhotos[i];
                        if (photoFile != null && photoFile.Length > 0)
                        {
                            if (photoFile.Length > maxFileSize)
                            {
                                ModelState.AddModelError("", $"Photo file size for item {i + 1} must be less than 5MB.");
                                await LoadViewBagData();
                                return View(damageDto);
                            }

                            var extension = Path.GetExtension(photoFile.FileName).ToLowerInvariant();
                            if (!allowedExtensions.Contains(extension))
                            {
                                ModelState.AddModelError("", $"Only JPG, PNG, and PDF files are allowed for item {i + 1}.");
                                await LoadViewBagData();
                                return View(damageDto);
                            }

                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photoFile.FileName)}";
                            var filePath = Path.Combine(uploadsPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await photoFile.CopyToAsync(stream);
                            }

                            // Initialize PhotoUrls list if null
                            if (damageDto.Items[i].PhotoUrls == null)
                            {
                                damageDto.Items[i].PhotoUrls = new List<string>();
                            }
                            damageDto.Items[i].PhotoUrls.Add($"/uploads/damages/{fileName}");
                        }
                    }
                }

                var result = await _damageService.CreateMultiItemDamageAsync(damageDto, damageDto.Items);

                if (result != null)
                {
                    if (result.TotalValue >= 10000)
                    {
                        TempData["Success"] = $"Damage report {result.DamageNo} created successfully. A write-off request has been automatically generated due to high value (৳{result.TotalValue:N2}).";
                    }
                    else
                    {
                        TempData["Success"] = $"Damage report {result.DamageNo} created successfully.";
                    }
                    return RedirectToAction(nameof(Details), new { id = result.Id });
                }

                TempData["Error"] = "Failed to create damage report.";
                await LoadViewBagData();
                return View(damageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating multi-item damage report");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                await LoadViewBagData();
                return View(damageDto);
            }
        }

        #endregion

        #region Details Actions

        /// <summary>
        /// Display damage details
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.ViewDamage)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var damage = await _damageService.GetDamageByIdAsync(id);
                if (damage == null)
                {
                    TempData["Error"] = "Damage record not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get current user info
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    ViewBag.CurrentUserId = currentUser.Id;
                    ViewBag.CurrentUserName = currentUser.UserName;
                    ViewBag.IsCreator = damage.CreatedBy == currentUser.UserName;
                    ViewBag.IsAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                    ViewBag.CanApprove = damage.Status == "Pending" && (
                        await _userManager.IsInRoleAsync(currentUser, "Admin") ||
                        await _userManager.IsInRoleAsync(currentUser, "Manager")
                    );
                }

                return View(damage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading damage details: {DamageId}", id);
                TempData["Error"] = "An error occurred while loading damage details.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Report Actions

        /// <summary>
        /// Generate damage report
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.ViewDamageReport)]
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate, string status = null, string damageType = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                // Get damages by date range
                var damages = await _damageService.GetDamagesByDateRangeAsync(start, end);

                // Filter by status if specified
                if (!string.IsNullOrEmpty(status))
                {
                    damages = damages.Where(d => d.Status == status);
                }

                // Filter by damage type if specified
                if (!string.IsNullOrEmpty(damageType))
                {
                    damages = damages.Where(d => d.DamageType == damageType);
                }

                var reportData = new DamageReportViewModel
                {
                    StartDate = start,
                    EndDate = end,
                    Status = status,
                    DamageType = damageType,
                    Damages = damages,
                    TotalCount = damages.Count(),
                    ApprovedCount = damages.Count(d => d.Status == "Approved"),
                    PendingCount = damages.Count(d => d.Status == "Pending"),
                    RejectedCount = damages.Count(d => d.Status == "Rejected"),
                    UnderInvestigationCount = damages.Count(d => d.ActionTaken == "Under Investigation"),
                    WriteOffRecommendedCount = damages.Count(d => d.ActionTaken == "Write-off Recommended"),

                    // Group by damage type
                    DamageTypeSummary = damages
                        .GroupBy(d => d.DamageType)
                        .Select(g => new DamageTypeSummaryDto
                        {
                            DamageType = g.Key,
                            Count = g.Count(),
                            TotalQuantity = g.Sum(d => d.Quantity ?? 0)
                        })
                        .OrderByDescending(d => d.Count)
                        .ToList(),

                    // Group by action taken
                    ActionSummary = damages
                        .GroupBy(d => d.ActionTaken)
                        .Select(g => new ActionSummaryDto
                        {
                            ActionTaken = g.Key,
                            Count = g.Count(),
                            TotalQuantity = g.Sum(d => d.Quantity ?? 0)
                        })
                        .OrderByDescending(a => a.Count)
                        .ToList(),

                    // Monthly summary
                    MonthlySummary = damages
                        .GroupBy(d => new { d.DamageDate.Year, d.DamageDate.Month })
                        .Select(g => new DamageMonthlySummaryDto
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            Count = g.Count(),
                            TotalQuantity = g.Sum(d => d.Quantity ?? 0)
                        })
                        .OrderBy(m => m.Year).ThenBy(m => m.Month)
                        .ToList()
                };

                ViewBag.StatusList = new SelectList(new[]
                {
                    new { Value = "", Text = "All Status" },
                    new { Value = "Pending", Text = "Pending" },
                    new { Value = "Approved", Text = "Approved" },
                    new { Value = "Rejected", Text = "Rejected" }
                }, "Value", "Text", status);

                ViewBag.DamageTypeList = new SelectList(new[]
                {
                    new { Value = "", Text = "All Types" },
                    new { Value = "Physical Damage", Text = "Physical Damage" },
                    new { Value = "Weather Damage", Text = "Weather Damage" },
                    new { Value = "Water Damage", Text = "Water Damage" },
                    new { Value = "Fire Damage", Text = "Fire Damage" },
                    new { Value = "Wear and Tear", Text = "Wear and Tear" },
                    new { Value = "Technical Failure", Text = "Technical Failure" },
                    new { Value = "Accident", Text = "Accident" },
                    new { Value = "Misuse", Text = "Misuse" },
                    new { Value = "Vandalism", Text = "Vandalism" },
                    new { Value = "Other", Text = "Other" }
                }, "Value", "Text", damageType);

                return View(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating damage report");
                TempData["Error"] = "An error occurred while generating the report.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Export damage report to PDF
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.ViewDamageReport)]
        public async Task<IActionResult> ExportToPdf(DateTime? startDate, DateTime? endDate, string status = null, string damageType = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                var damages = await _damageService.GetDamagesByDateRangeAsync(start, end);

                if (!string.IsNullOrEmpty(status))
                {
                    damages = damages.Where(d => d.Status == status);
                }

                if (!string.IsNullOrEmpty(damageType))
                {
                    damages = damages.Where(d => d.DamageType == damageType);
                }

                // Generate PDF report
                using (var memoryStream = new MemoryStream())
                {
                    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
                    iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Title
                    var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 18);
                    var title = new iTextSharp.text.Paragraph("Damage Report\n\n", titleFont);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    document.Add(title);

                    // Report info
                    var normalFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
                    var boldFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 10);

                    var reportInfo = new iTextSharp.text.Paragraph();
                    reportInfo.Add(new iTextSharp.text.Chunk("Report Period: ", boldFont));
                    reportInfo.Add(new iTextSharp.text.Chunk($"{start:dd-MMM-yyyy} to {end:dd-MMM-yyyy}\n", normalFont));
                    reportInfo.Add(new iTextSharp.text.Chunk("Generated: ", boldFont));
                    reportInfo.Add(new iTextSharp.text.Chunk($"{DateTime.Now:dd-MMM-yyyy HH:mm}\n\n", normalFont));
                    document.Add(reportInfo);

                    // Summary table
                    var summaryTable = new iTextSharp.text.pdf.PdfPTable(5);
                    summaryTable.WidthPercentage = 100;
                    summaryTable.AddCell(new iTextSharp.text.Phrase("Total", boldFont));
                    summaryTable.AddCell(new iTextSharp.text.Phrase("Approved", boldFont));
                    summaryTable.AddCell(new iTextSharp.text.Phrase("Pending", boldFont));
                    summaryTable.AddCell(new iTextSharp.text.Phrase("Rejected", boldFont));
                    summaryTable.AddCell(new iTextSharp.text.Phrase("Under Investigation", boldFont));

                    summaryTable.AddCell(damages.Count().ToString());
                    summaryTable.AddCell(damages.Count(d => d.Status == "Approved").ToString());
                    summaryTable.AddCell(damages.Count(d => d.Status == "Pending").ToString());
                    summaryTable.AddCell(damages.Count(d => d.Status == "Rejected").ToString());
                    summaryTable.AddCell(damages.Count(d => d.ActionTaken == "Under Investigation").ToString());

                    document.Add(summaryTable);
                    document.Add(new iTextSharp.text.Paragraph("\n"));

                    // Damage details table
                    var table = new iTextSharp.text.pdf.PdfPTable(8);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 5, 12, 15, 15, 10, 15, 13, 15 });

                    // Headers
                    table.AddCell(new iTextSharp.text.Phrase("#", boldFont));
                    table.AddCell(new iTextSharp.text.Phrase("Damage No", boldFont));
                    table.AddCell(new iTextSharp.text.Phrase("Date", boldFont));
                    table.AddCell(new iTextSharp.text.Phrase("Item", boldFont));
                    table.AddCell(new iTextSharp.text.Phrase("Quantity", boldFont));
                    table.AddCell(new iTextSharp.text.Phrase("Type", boldFont));
                    table.AddCell(new iTextSharp.text.Phrase("Status", boldFont));
                    table.AddCell(new iTextSharp.text.Phrase("Description", boldFont));

                    // Data rows
                    int serialNo = 1;
                    foreach (var damage in damages)
                    {
                        table.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(damage.DamageNo ?? "", normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(damage.DamageDate.ToString("dd-MMM-yyyy"), normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(damage.ItemName ?? "", normalFont));
                        table.AddCell(new iTextSharp.text.Phrase((damage.Quantity ?? 0).ToString(), normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(damage.DamageType ?? "", normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(damage.Status ?? "", normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(damage.Description ?? "", normalFont));
                        serialNo++;
                    }

                    document.Add(table);
                    document.Close();

                    var fileName = $"DamageReport_{start:yyyyMMdd}_{end:yyyyMMdd}.pdf";
                    return File(memoryStream.ToArray(), "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting damage report to PDF");
                TempData["Error"] = "An error occurred while exporting to PDF.";
                return RedirectToAction(nameof(Report), new { startDate, endDate, status, damageType });
            }
        }

        /// <summary>
        /// Export damage report to Excel
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.ViewDamageReport)]
        public async Task<IActionResult> ExportToExcel(DateTime? startDate, DateTime? endDate, string status = null, string damageType = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now.Date.AddDays(1).AddSeconds(-1);

                var damages = await _damageService.GetDamagesByDateRangeAsync(start, end);

                if (!string.IsNullOrEmpty(status))
                {
                    damages = damages.Where(d => d.Status == status);
                }

                if (!string.IsNullOrEmpty(damageType))
                {
                    damages = damages.Where(d => d.DamageType == damageType);
                }

                // Generate Excel report using ClosedXML
                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Damage Report");

                    // Title
                    worksheet.Cell(1, 1).Value = "Damage Report";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                    worksheet.Range(1, 1, 1, 8).Merge();
                    worksheet.Range(1, 1, 1, 8).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                    // Report info
                    worksheet.Cell(2, 1).Value = $"Period: {start:dd-MMM-yyyy} to {end:dd-MMM-yyyy}";
                    worksheet.Cell(3, 1).Value = $"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}";

                    // Summary section
                    int row = 5;
                    worksheet.Cell(row, 1).Value = "Summary";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    row++;

                    worksheet.Cell(row, 1).Value = "Total Damages:";
                    worksheet.Cell(row, 2).Value = damages.Count();
                    row++;
                    worksheet.Cell(row, 1).Value = "Approved:";
                    worksheet.Cell(row, 2).Value = damages.Count(d => d.Status == "Approved");
                    row++;
                    worksheet.Cell(row, 1).Value = "Pending:";
                    worksheet.Cell(row, 2).Value = damages.Count(d => d.Status == "Pending");
                    row++;
                    worksheet.Cell(row, 1).Value = "Rejected:";
                    worksheet.Cell(row, 2).Value = damages.Count(d => d.Status == "Rejected");
                    row++;
                    worksheet.Cell(row, 1).Value = "Under Investigation:";
                    worksheet.Cell(row, 2).Value = damages.Count(d => d.ActionTaken == "Under Investigation");

                    // Details section
                    row += 2;
                    worksheet.Cell(row, 1).Value = "Damage Details";
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    row++;

                    // Headers
                    var headers = new[] { "#", "Damage No", "Date", "Store", "Item", "Quantity", "Type", "Status", "Description", "Action Taken" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(row, i + 1).Value = headers[i];
                        worksheet.Cell(row, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(row, i + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                        worksheet.Cell(row, i + 1).Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                    }
                    row++;

                    // Data rows
                    int serialNo = 1;
                    foreach (var damage in damages)
                    {
                        worksheet.Cell(row, 1).Value = serialNo;
                        worksheet.Cell(row, 2).Value = damage.DamageNo ?? "";
                        worksheet.Cell(row, 3).Value = damage.DamageDate.ToString("dd-MMM-yyyy");
                        worksheet.Cell(row, 4).Value = damage.StoreName ?? "";
                        worksheet.Cell(row, 5).Value = damage.ItemName ?? "";
                        worksheet.Cell(row, 6).Value = damage.Quantity ?? 0;
                        worksheet.Cell(row, 7).Value = damage.DamageType ?? "";
                        worksheet.Cell(row, 8).Value = damage.Status ?? "";
                        worksheet.Cell(row, 9).Value = damage.Description ?? "";
                        worksheet.Cell(row, 10).Value = damage.ActionTaken ?? "";

                        // Apply borders
                        for (int i = 1; i <= 10; i++)
                        {
                            worksheet.Cell(row, i).Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                        }
                        row++;
                        serialNo++;
                    }

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Save to memory stream
                    using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        var fileName = $"DamageReport_{start:yyyyMMdd}_{end:yyyyMMdd}.xlsx";
                        return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting damage report to Excel");
                TempData["Error"] = "An error occurred while exporting to Excel.";
                return RedirectToAction(nameof(Report), new { startDate, endDate, status, damageType });
            }
        }

        #endregion

        #region Dashboard and Statistics

        /// <summary>
        /// Display damage dashboard with statistics
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.ViewDamage)]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Get all damages for statistics
                var allDamages = await _damageService.GetAllDamagesAsync();

                // Get count by status
                var statusCounts = await _damageService.GetDamageCountByStatusAsync();

                // Get count by type for last 30 days
                var last30Days = DateTime.Now.AddDays(-30);
                var typeCounts = await _damageService.GetDamageCountByTypeAsync(last30Days, DateTime.Now);

                // Calculate total value
                var totalValue = allDamages.Sum(d => d.TotalValue);
                var highValueDamages = allDamages.Where(d => d.TotalValue >= 10000).Count();

                // Recent damages (last 10)
                var recentDamages = allDamages
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(10)
                    .ToList();

                // Monthly trend (last 6 months)
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                var monthlyTrend = allDamages
                    .Where(d => d.DamageDate >= sixMonthsAgo)
                    .GroupBy(d => new { d.DamageDate.Year, d.DamageDate.Month })
                    .Select(g => new MonthlyDamageDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count(),
                        TotalValue = g.Sum(d => d.TotalValue)
                    })
                    .OrderBy(m => m.Year).ThenBy(m => m.Month)
                    .ToList();

                var model = new DamageDashboardViewModel
                {
                    StatusCounts = statusCounts,
                    TypeCounts = typeCounts,
                    TotalDamages = allDamages.Count(),
                    TotalValue = totalValue,
                    HighValueDamages = highValueDamages,
                    RecentDamages = recentDamages,
                    MonthlyTrend = monthlyTrend,
                    PendingApproval = statusCounts.ContainsKey(DamageStatus.Reported) ? statusCounts[DamageStatus.Reported] : 0,
                    UnderReview = statusCounts.ContainsKey(DamageStatus.UnderReview) ? statusCounts[DamageStatus.UnderReview] : 0
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading damage dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Status Management

        /// <summary>
        /// Update damage status
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ApproveDamage)]
        public async Task<IActionResult> UpdateStatus(int id, string status, string remarks)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    TempData["Error"] = "Status is required.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Parse status string to DamageStatus enum
                if (!Enum.TryParse<DamageStatus>(status, out var damageStatus))
                {
                    TempData["Error"] = "Invalid status value.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var result = await _damageService.UpdateDamageStatusAsync(id, damageStatus, remarks);

                if (result)
                {
                    TempData["Success"] = "Damage status updated successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to update damage status.";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating damage status: {DamageId}", id);
                TempData["Error"] = "An error occurred while updating status.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Load dropdown data
        /// </summary>
        private async Task LoadViewBagData()
        {
            try
            {
                var items = await _itemService.GetAllItemsAsync();
                var stores = await _storeService.GetAllStoresAsync();

                ViewBag.Items = new SelectList(items.Where(i => i.IsActive), "Id", "Name");
                ViewBag.Stores = new SelectList(stores.Where(s => s.IsActive), "Id", "Name");

                ViewBag.DamageTypes = new SelectList(new[] {
                    "Physical Damage",
                    "Weather Damage",
                    "Water Damage",
                    "Fire Damage",
                    "Wear and Tear",
                    "Technical Failure",
                    "Accident",
                    "Misuse",
                    "Vandalism",
                    "Other"
                });

                ViewBag.ActionTypes = new SelectList(new[] {
                    "Under Investigation",
                    "Repair Required",
                    "Replacement Ordered",
                    "Remove from Stock",
                    "Write-off Recommended",
                    "No Action Required"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading view data");
            }
        }

        // POST: Damage/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,StoreManager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var damage = await _damageService.GetDamageByIdAsync(id);
                if (damage == null)
                {
                    TempData["Error"] = "Damage report not found.";
                    return RedirectToAction(nameof(Index));
                }

                await _damageService.DeleteDamageAsync(id, User.Identity.Name);

                TempData["Success"] = $"Damage report '{damage.DamageNo}' has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Business logic validation error (status, WriteOff, StockMovement, etc.)
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting damage report {DamageId}", id);
                TempData["Error"] = "An error occurred while deleting the damage report.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion
    }

    #region View Models

    public class DamageReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string DamageType { get; set; }
        public IEnumerable<DamageDto> Damages { get; set; }
        public int TotalCount { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int RejectedCount { get; set; }
        public int UnderInvestigationCount { get; set; }
        public int WriteOffRecommendedCount { get; set; }
        public List<DamageTypeSummaryDto> DamageTypeSummary { get; set; }
        public List<ActionSummaryDto> ActionSummary { get; set; }
        public List<DamageMonthlySummaryDto> MonthlySummary { get; set; }
    }

    public class DamageTypeSummaryDto
    {
        public string DamageType { get; set; }
        public int Count { get; set; }
        public decimal TotalQuantity { get; set; }
    }

    public class ActionSummaryDto
    {
        public string ActionTaken { get; set; }
        public int Count { get; set; }
        public decimal TotalQuantity { get; set; }
    }

    public class DamageMonthlySummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
        public decimal TotalQuantity { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
    }

    public class DamageDashboardViewModel
    {
        public Dictionary<DamageStatus, int> StatusCounts { get; set; }
        public Dictionary<string, int> TypeCounts { get; set; }
        public int TotalDamages { get; set; }
        public decimal TotalValue { get; set; }
        public int HighValueDamages { get; set; }
        public List<DamageDto> RecentDamages { get; set; }
        public List<MonthlyDamageDto> MonthlyTrend { get; set; }
        public int PendingApproval { get; set; }
        public int UnderReview { get; set; }
    }

    public class MonthlyDamageDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
        public decimal TotalValue { get; set; }
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
    }

    #endregion
}
