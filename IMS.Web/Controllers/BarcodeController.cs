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

namespace IMS.Web.Controllers
{
    // DTO for batch generate filters
    public class BatchFilterDto
    {
        public string StoreId { get; set; }
        public string CategoryId { get; set; }
        public string SubCategoryId { get; set; }
        public string Criteria { get; set; }
        public string Search { get; set; }
    }

    [Authorize]
    public class BarcodeController : Controller
    {
        private readonly IBarcodeService _barcodeService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<BarcodeController> _logger;

        public BarcodeController(
            IBarcodeService barcodeService,
            IItemService itemService,
            IStoreService storeService,
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            ILogger<BarcodeController> logger)
        {
            _barcodeService = barcodeService;
            _itemService = itemService;
            _storeService = storeService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        [HasPermission(Permission.ViewBarcode)]
        public async Task<IActionResult> Index(string search, int? storeId, string barcodeType,
            string referenceType, string scanStatus, DateTime? dateFrom, DateTime? dateTo,
            int? categoryId, string location, string sortBy = "GeneratedDate", string order = "desc")
        {
            try
            {
                ViewBag.SearchTerm = search;

                var filters = new BarcodeFilterDto
                {
                    Search = search,
                    StoreId = storeId,
                    BarcodeType = barcodeType,
                    ReferenceType = referenceType,
                    ScanStatus = scanStatus,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    CategoryId = categoryId,
                    Location = location,
                    SortBy = sortBy,
                    Order = order
                };

                var barcodes = await _barcodeService.GetFilteredBarcodesAsync(filters);

                var stats = await _barcodeService.GetBarcodeStatisticsAsync();
                ViewBag.TotalBarcodes = stats.TotalBarcodes;
                ViewBag.RecentlyScanned = stats.BarcodesScannedToday;
                ViewBag.MostPrinted = stats.BarcodesPrintedToday;
                ViewBag.Unscanned = stats.TotalBarcodes - stats.ActiveBarcodes;

                return View(barcodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading barcode index");
                TempData["Error"] = "An error occurred while loading barcodes.";
                return View(new List<BarcodeDto>());
            }
        }

        [HasPermission(Permission.ViewBarcode)]
        public async Task<IActionResult> Search(string barcodeNumber)
        {
            BarcodeDto barcode = null;

            if (!string.IsNullOrEmpty(barcodeNumber))
            {
                try
                {
                    barcode = await _barcodeService.GetBarcodeByNumberAsync(barcodeNumber);
                    if (barcode == null)
                    {
                        TempData["Warning"] = "Barcode not found.";
                    }
                    else
                    {
                        var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcode.BarcodeNumber);
                        ViewBag.QRCodeBase64 = qrCodeBase64;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error searching barcode: {BarcodeNumber}", barcodeNumber);
                    TempData["Error"] = "An error occurred while searching.";
                }
            }

            ViewBag.SearchTerm = barcodeNumber;
            return View(barcode);
        }

        [HasPermission(Permission.ViewBarcode)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var barcode = await _barcodeService.GetBarcodeByIdAsync(id);
                if (barcode == null)
                {
                    TempData["Error"] = "Barcode not found.";
                    return RedirectToAction(nameof(Index));
                }

                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcode.BarcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;

                return View(barcode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading barcode details: {Id}", id);
                TempData["Error"] = "An error occurred while loading barcode details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var barcode = await _barcodeService.GetBarcodeByIdAsync(id);
                if (barcode == null)
                {
                    TempData["Error"] = "Barcode not found.";
                    return RedirectToAction(nameof(Index));
                }

                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcode.BarcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;

                var stores = await _unitOfWork.Stores.GetAllAsync();
                ViewBag.Stores = new SelectList(stores.Where(s => s.IsActive), "Id", "Name");

                return View(barcode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading barcode for edit: {Id}", id);
                TempData["Error"] = "An error occurred while loading barcode.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> Edit(BarcodeDto barcodeDto, string changeReason, string changeNotes)
        {
            if (!ModelState.IsValid)
            {
                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcodeDto.BarcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;

                var stores = await _unitOfWork.Stores.GetAllAsync();
                ViewBag.Stores = new SelectList(stores.Where(s => s.IsActive), "Id", "Name");

                return View(barcodeDto);
            }

            if (string.IsNullOrWhiteSpace(changeReason))
            {
                ModelState.AddModelError("", "Change reason is required.");

                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcodeDto.BarcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;

                var stores = await _unitOfWork.Stores.GetAllAsync();
                ViewBag.Stores = new SelectList(stores.Where(s => s.IsActive), "Id", "Name");

                return View(barcodeDto);
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var userId = currentUser?.Id ?? User.Identity.Name;

                await _barcodeService.UpdateBarcodeAsync(barcodeDto);

                _logger.LogInformation(
                    "Barcode {BarcodeId} updated by {User}. Reason: {Reason}. Notes: {Notes}",
                    barcodeDto.Id, userId, changeReason, changeNotes);

                TempData["Success"] = "Barcode updated successfully!";
                return RedirectToAction(nameof(Details), new { id = barcodeDto.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating barcode: {BarcodeId}", barcodeDto.Id);
                TempData["Error"] = "An error occurred while updating barcode.";

                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcodeDto.BarcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;

                var stores = await _unitOfWork.Stores.GetAllAsync();
                ViewBag.Stores = new SelectList(stores.Where(s => s.IsActive), "Id", "Name");

                return View(barcodeDto);
            }
        }

        [HasPermission(Permission.PrintBarcode)]
        public async Task<IActionResult> Print(int id)
        {
            try
            {
                var barcode = await _barcodeService.GetBarcodeByIdAsync(id);
                if (barcode == null)
                {
                    TempData["Error"] = "Barcode not found.";
                    return RedirectToAction(nameof(Index));
                }

                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcode.BarcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;

                return View(barcode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading barcode for print: {Id}", id);
                TempData["Error"] = "An error occurred while loading barcode.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.PrintBarcode)]
        public async Task<IActionResult> GeneratePDF(int id)
        {
            try
            {
                var barcode = await _barcodeService.GetBarcodeByIdAsync(id);
                if (barcode == null)
                {
                    TempData["Error"] = "Barcode not found.";
                    return RedirectToAction(nameof(Index));
                }

                var barcodes = new List<BarcodeDto> { barcode };
                var pdfBytes = await _barcodeService.GenerateBatchBarcodePDF(barcodes);

                return File(pdfBytes, "application/pdf", $"Barcode_{barcode.BarcodeNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for barcode: {Id}", id);
                TempData["Error"] = "An error occurred while generating PDF.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> Generate(int? itemId)
        {
            if (!itemId.HasValue)
            {
                TempData["Error"] = "Please select an item to generate barcode.";
                return RedirectToAction("Index", "Item");
            }

            try
            {
                var item = await _itemService.GetItemByIdAsync(itemId.Value);
                if (item == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction("Index", "Item");
                }

                var barcodeNumber = await _barcodeService.GenerateUniqueBarcodeNumberAsync(itemId.Value);
                var serialNumber = await _barcodeService.GenerateSerialNumberAsync("SN");

                var barcode = new BarcodeDto
                {
                    ItemId = itemId.Value,
                    ItemName = item.Name,
                    ItemCode = item.Code,
                    CategoryName = item.CategoryName,
                    BarcodeNumber = barcodeNumber,
                    SerialNumber = serialNumber,
                    GeneratedDate = DateTime.Now,
                    BarcodeType = "QRCode"
                };

                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;

                var stores = await _unitOfWork.Stores.GetAllAsync();
                ViewBag.Stores = new SelectList(stores.Where(s => s.IsActive), "Id", "Name");

                return View(barcode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating barcode for item: {ItemId}", itemId);
                TempData["Error"] = "An error occurred while generating barcode.";
                return RedirectToAction("Index", "Item");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> Generate(BarcodeDto barcodeDto)
        {
            if (!ModelState.IsValid)
            {
                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcodeDto.BarcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;
                return View(barcodeDto);
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                barcodeDto.GeneratedBy = currentUser?.FullName ?? User.Identity.Name;

                var createdBarcode = await _barcodeService.CreateBarcodeAsync(barcodeDto);

                TempData["Success"] = "Barcode generated successfully!";
                return RedirectToAction(nameof(Print), new { id = createdBarcode.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving barcode");
                TempData["Error"] = "An error occurred while saving barcode.";

                var qrCodeBase64 = _barcodeService.GenerateQRCodeBase64(barcodeDto.BarcodeNumber);
                ViewBag.QRCodeBase64 = qrCodeBase64;

                return View(barcodeDto);
            }
        }

        public IActionResult GenerateQRImage(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest();
            }

            try
            {
                var qrCodeBytes = _barcodeService.GenerateQRCode(text);
                return File(qrCodeBytes, "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR image");
                return StatusCode(500);
            }
        }

        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> BatchGenerate()
        {
            try
            {
                var items = await _itemService.GetAllItemsAsync();
                ViewBag.Items = new SelectList(items.OrderBy(i => i.Name), "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading batch generate page");
                TempData["Error"] = "An error occurred while loading the page.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStores()
        {
            try
            {
                var stores = await _unitOfWork.Stores.GetAllAsync();
                var storeList = stores.Where(s => s.IsActive).Select(s => new
                {
                    value = s.Id.ToString(),
                    text = s.Name
                }).ToList();

                return Json(storeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stores for barcode filter");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var categoryList = categories.Where(c => c.IsActive).Select(c => new
                {
                    value = c.Id.ToString(),
                    text = c.Name
                }).ToList();

                return Json(categoryList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories for barcode filter");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubCategories(int categoryId)
        {
            try
            {
                var subCategories = await _unitOfWork.SubCategories
                    .FindAsync(sc => sc.CategoryId == categoryId && sc.IsActive);

                var subCategoryList = subCategories.Select(sc => new
                {
                    value = sc.Id.ToString(),
                    text = sc.Name
                }).ToList();

                return Json(subCategoryList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading subcategories for category {CategoryId}", categoryId);
                return Json(new List<object>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetFilteredItems([FromBody] BatchFilterDto filters)
        {
            try
            {
                int? storeId = !string.IsNullOrEmpty(filters.StoreId) ? int.Parse(filters.StoreId) : (int?)null;
                int? categoryId = !string.IsNullOrEmpty(filters.CategoryId) ? int.Parse(filters.CategoryId) : (int?)null;
                int? subCategoryId = !string.IsNullOrEmpty(filters.SubCategoryId) ? int.Parse(filters.SubCategoryId) : (int?)null;
                string criteria = filters.Criteria ?? "all";
                string search = filters.Search ?? "";

                var query = await _unitOfWork.Items.GetAllAsync();
                var items = query.Where(i => i.IsActive);

                // Get all StoreItems to filter items that are actually in stores
                var allStoreItems = await _unitOfWork.StoreItems.GetAllAsync();

                if (storeId.HasValue)
                {
                    // Filter by specific store
                    var storeItems = allStoreItems.Where(si => si.StoreId == storeId.Value);
                    var storeItemIds = storeItems.Select(si => si.ItemId).ToList();
                    items = items.Where(i => storeItemIds.Contains(i.Id));
                }
                else
                {
                    // When "All Stores" selected, show items that exist in ANY store
                    var itemsInStores = allStoreItems.Select(si => si.ItemId).Distinct().ToList();
                    items = items.Where(i => itemsInStores.Contains(i.Id));
                }

                if (categoryId.HasValue)
                {
                    items = items.Where(i => i.CategoryId == categoryId.Value);
                }

                if (subCategoryId.HasValue)
                {
                    items = items.Where(i => i.SubCategoryId == subCategoryId.Value);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    items = items.Where(i =>
                        i.Name.ToLower().Contains(search) ||
                        (i.Code != null && i.Code.ToLower().Contains(search)));
                }

                if (criteria == "no_barcode")
                {
                    var itemsWithBarcodes = (await _unitOfWork.Barcodes.GetAllAsync())
                        .Select(b => b.ItemId)
                        .Distinct()
                        .ToList();
                    items = items.Where(i => !itemsWithBarcodes.Contains(i.Id));
                }
                else if (criteria == "active_only")
                {
                    items = items.Where(i => i.IsActive);
                }

                var result = new List<object>();
                var allBarcodes = await _unitOfWork.Barcodes.GetAllAsync();

                foreach (var item in items.Take(100))
                {
                    var category = item.CategoryId > 0
                        ? await _unitOfWork.Categories.GetByIdAsync(item.CategoryId)
                        : null;

                    // Get store name(s) for this item
                    string storeName;
                    decimal currentStock = 0;

                    if (storeId.HasValue)
                    {
                        // Specific store selected
                        var store = await _unitOfWork.Stores.GetByIdAsync(storeId.Value);
                        storeName = store?.Name ?? "N/A";

                        var storeItem = allStoreItems.FirstOrDefault(si => si.StoreId == storeId.Value && si.ItemId == item.Id);
                        currentStock = storeItem?.CurrentStock ?? 0;
                    }
                    else
                    {
                        // All stores - show first store or count of stores
                        var itemStores = allStoreItems.Where(si => si.ItemId == item.Id).ToList();
                        if (itemStores.Any())
                        {
                            var firstStore = await _unitOfWork.Stores.GetByIdAsync(itemStores.First().StoreId);
                            if (itemStores.Count > 1)
                            {
                                storeName = $"{firstStore?.Name ?? "Store"} (+{itemStores.Count - 1} more)";
                            }
                            else
                            {
                                storeName = firstStore?.Name ?? "N/A";
                            }
                            currentStock = itemStores.Sum(si => si.CurrentStock);
                        }
                        else
                        {
                            storeName = "N/A";
                        }
                    }

                    var hasBarcodes = allBarcodes.Any(b => b.ItemId == item.Id);

                    result.Add(new
                    {
                        id = item.Id,
                        code = item.Code,
                        name = item.Name,
                        categoryName = category?.Name ?? "N/A",
                        storeName = storeName,
                        hasBarcodes = hasBarcodes,
                        currentStock = currentStock
                    });
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering items for batch barcode generation");
                return Json(new { error = true, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GenerateUniqueNumber()
        {
            try
            {
                var barcodeNumber = await _barcodeService.GenerateUniqueBarcodeNumberAsync(null);
                return Json(new { success = true, barcodeNumber = barcodeNumber });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating unique barcode number");
                return Json(new { success = false, message = "Failed to generate barcode number" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> RecordPrint(int barcodeId)
        {
            try
            {
                var barcode = await _barcodeService.GetBarcodeByIdAsync(barcodeId);
                if (barcode == null)
                {
                    return Json(new { success = false, message = "Barcode not found" });
                }

                var currentUser = await _userManager.GetUserAsync(User);
                var userId = currentUser?.Id ?? User.Identity.Name;

                await _barcodeService.UpdateBarcodeTrackingAsync(barcodeId, "Print", userId);

                _logger.LogInformation("Print recorded for barcode ID {BarcodeId}", barcodeId);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording barcode print: {BarcodeId}", barcodeId);
                return Json(new { success = false, message = "Failed to record print" });
            }
        }

        [HttpPost]
        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> GenerateBatchBarcodesAsync(int itemId, int quantity, string generatedBy)
        {
            try
            {
                if (quantity <= 0 || quantity > 100)
                {
                    return Json(new { success = false, message = "Quantity must be between 1 and 100" });
                }

                var barcodes = await _barcodeService.GenerateBatchBarcodesAsync(
                    itemId,
                    quantity,
                    generatedBy ?? User.Identity.Name
                );

                return Json(new
                {
                    success = true,
                    message = $"Generated {barcodes.Count} barcodes",
                    count = barcodes.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch barcode generation for item {ItemId}", itemId);
                return Json(new { success = false, message = "Failed to generate barcodes" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> BatchGenerate(List<BatchGenerateDto> batchItems)
        {
            if (batchItems == null || !batchItems.Any(b => b.Selected))
            {
                TempData["Error"] = "Please select at least one item.";
                return RedirectToAction(nameof(BatchGenerate));
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var allBarcodes = new List<BarcodeDto>();

                foreach (var batchItem in batchItems.Where(b => b.Selected))
                {
                    if (batchItem.Quantity > 0 && batchItem.Quantity <= 100)
                    {
                        var barcodes = await _barcodeService.GenerateBatchBarcodesAsync(
                            batchItem.ItemId,
                            batchItem.Quantity,
                            currentUser?.FullName ?? User.Identity.Name
                        );
                        allBarcodes.AddRange(barcodes);
                    }
                }

                if (allBarcodes.Any())
                {
                    var pdfBytes = await _barcodeService.GenerateBatchBarcodePDF(allBarcodes);

                    _logger.LogInformation("Batch generated {Count} barcodes by user {User}",
                        allBarcodes.Count, currentUser?.FullName);

                    TempData["Success"] = $"Successfully generated {allBarcodes.Count} barcodes.";
                    return File(pdfBytes, "application/pdf", $"Barcodes_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch barcode generation");
                TempData["Error"] = "An error occurred while generating barcodes.";
            }

            TempData["Error"] = "No barcodes were generated.";
            return RedirectToAction(nameof(BatchGenerate));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.PrintBarcode)]
        public async Task<IActionResult> BatchPrint([FromBody] BatchPrintRequestDto request)
        {
            if (request?.SelectedItemIds == null || !request.SelectedItemIds.Any())
            {
                return Json(new { success = false, message = "Please select items to print barcodes." });
            }

            if (request.Quantity < 1 || request.Quantity > 50)
            {
                return Json(new { success = false, message = "Quantity must be between 1 and 50." });
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var allBarcodes = new List<BarcodeDto>();

                foreach (var itemId in request.SelectedItemIds)
                {
                    var barcodes = await _barcodeService.GenerateBatchBarcodesAsync(
                        itemId,
                        request.Quantity,
                        currentUser?.FullName ?? User.Identity.Name
                    );
                    allBarcodes.AddRange(barcodes);
                }

                if (allBarcodes.Any())
                {
                    var pdfBytes = await _barcodeService.GenerateBatchBarcodePDF(allBarcodes);

                    HttpContext.Session.Set("BarcodePDF", pdfBytes);

                    _logger.LogInformation("Batch print {Count} barcodes by user {User}",
                        allBarcodes.Count, currentUser?.FullName);

                    return Json(new
                    {
                        success = true,
                        message = $"Generated {allBarcodes.Count} barcodes successfully.",
                        downloadUrl = Url.Action("DownloadBarcodes", "Barcode")
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch print");
                return Json(new { success = false, message = "An error occurred while generating barcodes." });
            }

            return Json(new { success = false, message = "Failed to generate barcodes." });
        }

        public IActionResult DownloadBarcodes()
        {
            var pdfBytes = HttpContext.Session.Get("BarcodePDF");
            if (pdfBytes == null)
            {
                TempData["Error"] = "No barcode PDF found. Please generate barcodes first.";
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.Remove("BarcodePDF");

            return File(pdfBytes, "application/pdf", $"Barcodes_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        [HttpPost]
        [HasPermission(Permission.GenerateBarcode)]
        public async Task<IActionResult> OfflineSync([FromBody] OfflineScanDto scanData)
        {
            if (scanData == null)
            {
                return Json(new { success = false, message = "Invalid scan data." });
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var userId = currentUser?.Id ?? User.Identity.Name;

                var barcode = await _barcodeService.ProcessOfflineScanAsync(scanData, userId);

                _logger.LogInformation("Offline scan processed: {Action} for barcode {Barcode} by user {User}",
                    scanData.Action, scanData.Barcode, userId);

                return Json(new
                {
                    success = true,
                    message = "Scan processed successfully",
                    data = new
                    {
                        barcode.Id,
                        barcode.BarcodeNumber,
                        barcode.ItemName,
                        action = scanData.Action,
                        quantity = scanData.Quantity,
                        timestamp = scanData.Timestamp
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid offline scan: {Message}", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing offline scan");
                return Json(new { success = false, message = "An error occurred while processing the scan." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term, int page = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                {
                    return Json(new { items = new List<object>(), hasMore = false });
                }

                var pageSize = 20;
                var suggestions = new List<object>();

                var barcodes = await _barcodeService.SearchBarcodesAsync(term);
                foreach (var barcode in barcodes.Take(10))
                {
                    suggestions.Add(new
                    {
                        value = barcode.BarcodeNumber,
                        text = barcode.BarcodeNumber,
                        subtitle = $"Item: {barcode.ItemName}",
                        type = "barcode"
                    });
                }

                var items = await _itemService.SearchItemsAsync(term);
                foreach (var item in items.Take(10))
                {
                    suggestions.Add(new
                    {
                        value = item.Name,
                        text = item.Name,
                        subtitle = $"Code: {item.Code} | Category: {item.CategoryName}",
                        type = "item"
                    });
                }

                var serialBarcodes = await _barcodeService.SearchBySerialNumberAsync(term);
                foreach (var barcode in serialBarcodes.Take(5))
                {
                    suggestions.Add(new
                    {
                        value = barcode.SerialNumber,
                        text = barcode.SerialNumber,
                        subtitle = $"Barcode: {barcode.BarcodeNumber} | Item: {barcode.ItemName}",
                        type = "serial"
                    });
                }

                var totalSuggestions = suggestions.Count;
                var pagedSuggestions = suggestions
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Json(new
                {
                    items = pagedSuggestions,
                    hasMore = totalSuggestions > (page * pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting search suggestions for term: {Term}", term);
                return Json(new { items = new List<object>(), hasMore = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            try
            {
                var locations = await _barcodeService.GetDistinctLocationsAsync();
                var locationList = locations.Select(l => new
                {
                    value = l,
                    text = l
                }).ToList();

                return Json(locationList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading locations");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBarcodeDetails(int id)
        {
            try
            {
                var barcode = await _barcodeService.GetBarcodeByIdAsync(id);
                if (barcode == null)
                {
                    return Json(new { success = false, message = "Barcode not found" });
                }

                return Json(new
                {
                    success = true,
                    barcodeType = barcode.BarcodeType ?? "CODE128",
                    generatedDate = barcode.GeneratedDate,
                    printCount = barcode.PrintCount,
                    scanCount = barcode.ScanCount,
                    lastScannedDate = barcode.LastScannedDate,
                    itemName = barcode.ItemName,
                    storeName = barcode.StoreName,
                    location = barcode.Location
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting barcode details for ID: {Id}", id);
                return Json(new { success = false, message = "Error loading barcode details" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RecordScan([FromBody] RecordScanDto scanDto)
        {
            try
            {
                if (scanDto == null || scanDto.BarcodeId <= 0 || string.IsNullOrWhiteSpace(scanDto.Location))
                {
                    return Json(new { success = false, message = "Invalid scan data" });
                }

                var currentUser = await _userManager.GetUserAsync(User);
                var userId = currentUser?.Id ?? User.Identity.Name;

                await _barcodeService.UpdateBarcodeTrackingAsync(
                    scanDto.BarcodeId,
                    scanDto.Action ?? "Manual",
                    userId
                );

                if (!string.IsNullOrWhiteSpace(scanDto.Location))
                {
                    await _barcodeService.UpdateBarcodeLocationAsync(
                        scanDto.BarcodeId,
                        null,
                        scanDto.Location,
                        scanDto.Notes
                    );
                }

                _logger.LogInformation("Manual scan recorded for barcode ID {BarcodeId} by user {UserId}",
                    scanDto.BarcodeId, userId);

                return Json(new
                {
                    success = true,
                    message = "Scan recorded successfully",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording manual scan for barcode ID: {BarcodeId}", scanDto?.BarcodeId);
                return Json(new { success = false, message = "Error recording scan" });
            }
        }

        [HttpGet]
        [HasPermission(Permission.ExportBarcode)]
        public async Task<IActionResult> ExportToExcel(string search, int? storeId, string barcodeType,
            string referenceType, string scanStatus, DateTime? dateFrom, DateTime? dateTo,
            int? categoryId, string location)
        {
            try
            {
                var filters = new BarcodeFilterDto
                {
                    Search = search,
                    StoreId = storeId,
                    BarcodeType = barcodeType,
                    ReferenceType = referenceType,
                    ScanStatus = scanStatus,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    CategoryId = categoryId,
                    Location = location
                };

                var barcodes = await _barcodeService.GetFilteredBarcodesAsync(filters);
                var excelBytes = await _barcodeService.ExportToExcelAsync(barcodes);

                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Barcodes_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting barcodes to Excel");
                TempData["Error"] = "Error exporting to Excel";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ExportBarcode)]
        public async Task<IActionResult> ExportToPDF(string search, int? storeId, string barcodeType,
            string referenceType, string scanStatus, DateTime? dateFrom, DateTime? dateTo,
            int? categoryId, string location)
        {
            try
            {
                var filters = new BarcodeFilterDto
                {
                    Search = search,
                    StoreId = storeId,
                    BarcodeType = barcodeType,
                    ReferenceType = referenceType,
                    ScanStatus = scanStatus,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    CategoryId = categoryId,
                    Location = location
                };

                var barcodes = await _barcodeService.GetFilteredBarcodesAsync(filters);
                var pdfBytes = await _barcodeService.ExportToPDFAsync(barcodes);

                return File(pdfBytes, "application/pdf", $"Barcodes_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting barcodes to PDF");
                TempData["Error"] = "Error exporting to PDF";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ExportBarcode)]
        public async Task<IActionResult> ExportToCSV(string search, int? storeId, string barcodeType,
            string referenceType, string scanStatus, DateTime? dateFrom, DateTime? dateTo,
            int? categoryId, string location)
        {
            try
            {
                var filters = new BarcodeFilterDto
                {
                    Search = search,
                    StoreId = storeId,
                    BarcodeType = barcodeType,
                    ReferenceType = referenceType,
                    ScanStatus = scanStatus,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    CategoryId = categoryId,
                    Location = location
                };

                var barcodes = await _barcodeService.GetFilteredBarcodesAsync(filters);
                var csvBytes = await _barcodeService.ExportToCSVAsync(barcodes);

                return File(csvBytes, "text/csv", $"Barcodes_{DateTime.Now:yyyyMMddHHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting barcodes to CSV");
                TempData["Error"] = "Error exporting to CSV";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Print all barcode labels in bulk (filtered or all)
        /// This generates actual printable barcode labels, not a report
        /// </summary>
        [HttpGet]
        [HasPermission(Permission.PrintBarcode)]
        public async Task<IActionResult> PrintAllLabels(string search, int? storeId, string barcodeType,
            string referenceType, string scanStatus, DateTime? dateFrom, DateTime? dateTo,
            int? categoryId, string location)
        {
            try
            {
                var filters = new BarcodeFilterDto
                {
                    Search = search,
                    StoreId = storeId,
                    BarcodeType = barcodeType,
                    ReferenceType = referenceType,
                    ScanStatus = scanStatus,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    CategoryId = categoryId,
                    Location = location
                };

                var barcodes = await _barcodeService.GetFilteredBarcodesAsync(filters);

                if (!barcodes.Any())
                {
                    TempData["Warning"] = "No barcodes found to print.";
                    return RedirectToAction(nameof(Index));
                }

                // Generate printable barcode labels PDF
                var pdfBytes = await _barcodeService.GenerateBatchBarcodePDF(barcodes.ToList());

                var currentUser = await _userManager.GetUserAsync(User);
                _logger.LogInformation("Bulk print {Count} barcode labels by user {User}",
                    barcodes.Count(), currentUser?.FullName ?? User.Identity.Name);

                return File(pdfBytes, "application/pdf", $"Barcode_Labels_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing all barcode labels");
                TempData["Error"] = "An error occurred while generating barcode labels PDF.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.PrintBarcode)]
        public async Task<IActionResult> PrintSelectedLabels([FromForm] List<int> selectedIds)
        {
            try
            {
                if (selectedIds == null || !selectedIds.Any())
                {
                    TempData["Warning"] = "No barcodes selected to print.";
                    return RedirectToAction(nameof(Index));
                }

                // Get selected barcodes
                var barcodes = new List<BarcodeDto>();
                foreach (var id in selectedIds)
                {
                    var barcode = await _barcodeService.GetBarcodeByIdAsync(id);
                    if (barcode != null)
                    {
                        barcodes.Add(barcode);
                    }
                }

                if (!barcodes.Any())
                {
                    TempData["Warning"] = "No valid barcodes found to print.";
                    return RedirectToAction(nameof(Index));
                }

                // Generate printable barcode labels PDF
                var pdfBytes = await _barcodeService.GenerateBatchBarcodePDF(barcodes);

                var currentUser = await _userManager.GetUserAsync(User);
                _logger.LogInformation("Printed {Count} selected barcode labels by user {User}",
                    barcodes.Count, currentUser?.FullName ?? User.Identity.Name);

                return File(pdfBytes, "application/pdf", $"Selected_Barcode_Labels_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing selected barcode labels");
                TempData["Error"] = "An error occurred while generating selected barcode labels PDF.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}