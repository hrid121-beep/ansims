using ClosedXML.Excel;
using CsvHelper;
using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Text;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StockEntryController : Controller
    {
        private readonly IStockEntryService _stockEntryService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;
        private readonly IBarcodeService _barcodeService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork; // ADD THIS
        private readonly ILogger<StockEntryController> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public StockEntryController(
            IStockEntryService stockEntryService,
            IStoreService storeService,
            IItemService itemService,
            IBarcodeService barcodeService,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IHostEnvironment hostEnvironment, // ADD THIS
            ILogger<StockEntryController> logger)
        {
            _stockEntryService = stockEntryService;
            _storeService = storeService;
            _itemService = itemService;
            _barcodeService = barcodeService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment; // ADD THIS
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetItemByBarcode(string barcode, int? storeId)
        {
            try
            {
                _logger.LogInformation($"Searching for item with barcode: {barcode}");

                // First try to find the barcode
                var barcodeEntity = await _unitOfWork.Barcodes
                    .FirstOrDefaultAsync(b => b.BarcodeNumber == barcode && b.IsActive);

                if (barcodeEntity == null)
                {
                    _logger.LogWarning($"Barcode not found: {barcode}");

                    // Try to find item by ItemCode (fallback)
                    var itemByCode = await _unitOfWork.Items
                        .FirstOrDefaultAsync(i => i.ItemCode == barcode && i.IsActive);

                    if (itemByCode == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Item not found with this barcode or item code"
                        });
                    }

                    // Found by item code
                    barcodeEntity = new Barcode { ItemId = itemByCode.Id };
                }

                // Get the item details
                var item = await _unitOfWork.Items.Query()
                    .Include(i => i.SubCategory)
                        .ThenInclude(sc => sc.Category)
                    .FirstOrDefaultAsync(i => i.Id == barcodeEntity.ItemId);

                if (item == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Item not found"
                    });
                }

                // Get current stock if store is specified
                decimal currentStock = 0;
                if (storeId.HasValue)
                {
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.ItemId == item.Id && si.StoreId == storeId.Value);
                    currentStock = storeItem?.Quantity ?? 0;
                }

                return Json(new
                {
                    success = true,
                    item = new
                    {
                        id = item.Id,
                        name = item.Name,
                        itemCode = item.ItemCode,
                        categoryId = item.SubCategory?.CategoryId,
                        categoryName = item.SubCategory?.Category?.Name,
                        subCategoryId = item.SubCategoryId,
                        subCategoryName = item.SubCategory?.Name,
                        unitPrice = item.UnitPrice ?? 0,
                        unit = item.Unit ?? "Unit",
                        currentStock = currentStock,
                        minimumStock = item.MinimumStock,
                        reorderLevel = item.ReorderLevel,
                        barcodeNumber = barcode,
                        location = barcodeEntity.Location,
                        batchNumber = barcodeEntity.BatchNumber
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting item by barcode: {barcode}");
                return Json(new
                {
                    success = false,
                    message = "Error retrieving item information"
                });
            }
        }

        // Add this method to search items by name or code (for autocomplete)
        [HttpGet]
        public async Task<IActionResult> SearchItems(string term, int? storeId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                {
                    return Json(new List<object>());
                }

                var items = await _unitOfWork.Items.Query()
                    .Include(i => i.SubCategory)
                        .ThenInclude(sc => sc.Category)
                    .Where(i => i.IsActive &&
                               (i.Name.Contains(term) ||
                                i.ItemCode.Contains(term) ||
                                i.Barcode.Contains(term)))
                    .Take(20)
                    .Select(i => new
                    {
                        id = i.Id,
                        label = $"{i.Name} ({i.ItemCode})",
                        value = i.ItemCode,
                        itemId = i.Id,
                        name = i.Name,
                        itemCode = i.ItemCode,
                        categoryId = i.SubCategory.CategoryId,
                        categoryName = i.SubCategory.Category.Name,
                        unitPrice = i.UnitPrice ?? 0,
                        unit = i.Unit ?? "Unit"
                    })
                    .ToListAsync();

                return Json(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching items with term: {term}");
                return Json(new List<object>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.EditStock)]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to complete stock entry ID: {id}");

                var result = await _stockEntryService.CompleteStockEntryAsync(id);

                if (result)
                {
                    _logger.LogInformation($"Stock entry ID {id} completed successfully");
                    return Json(new
                    {
                        success = true,
                        message = "Stock entry completed successfully!"
                    });
                }
                else
                {
                    _logger.LogWarning($"Failed to complete stock entry ID {id} - may already be completed");
                    return Json(new
                    {
                        success = false,
                        message = "Unable to complete stock entry. It may already be completed."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing stock entry ID: {id}");
                return Json(new
                {
                    success = false,
                    message = $"An error occurred while completing the stock entry: {ex.Message}"
                });
            }
        }

        // Delete stock entry - NEW method for AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteStock)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete stock entry ID: {id}");

                // Check if entry exists and is in Draft status
                var entry = await _stockEntryService.GetStockEntryByIdAsync(id);
                if (entry == null)
                {
                    _logger.LogWarning($"Stock entry ID {id} not found");
                    return Json(new
                    {
                        success = false,
                        message = "Stock entry not found."
                    });
                }

                if (entry.Status != "Draft")
                {
                    _logger.LogWarning($"Cannot delete stock entry ID {id} - status is {entry.Status}");
                    return Json(new
                    {
                        success = false,
                        message = "Only Draft entries can be deleted."
                    });
                }

                var result = await _stockEntryService.DeleteStockEntryAsync(id);

                if (result)
                {
                    _logger.LogInformation($"Stock entry ID {id} deleted successfully");
                    return Json(new
                    {
                        success = true,
                        message = "Stock entry deleted successfully!"
                    });
                }
                else
                {
                    _logger.LogWarning($"Failed to delete stock entry ID {id}");
                    return Json(new
                    {
                        success = false,
                        message = "Unable to delete stock entry."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting stock entry ID: {id}");
                return Json(new
                {
                    success = false,
                    message = $"An error occurred while deleting the stock entry: {ex.Message}"
                });
            }
        }

        // Edit action - for navigating to edit page
        [HasPermission(Permission.EditStock)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var entry = await _stockEntryService.GetStockEntryByIdAsync(id);
                if (entry == null)
                {
                    TempData["Error"] = "Stock entry not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (entry.Status != "Draft")
                {
                    TempData["Error"] = "Only Draft entries can be edited.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                ViewBag.EntryNo = entry.EntryNo;
                await LoadViewBagData();

                return View(entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock entry for edit");
                TempData["Error"] = "An error occurred while loading the stock entry.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Edit POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.EditStock)]
        public async Task<IActionResult> Edit(int id, StockEntryDto model)
        {
            try
            {
                if (model.Items == null || !model.Items.Any())
                {
                    ModelState.AddModelError("", "Please add at least one item to the stock entry.");
                }

                if (ModelState.IsValid)
                {
                    var result = await _stockEntryService.UpdateStockEntryAsync(id, model);

                    TempData["Success"] = $"Stock entry {result.EntryNo} updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock entry");
                ModelState.AddModelError("", "An error occurred while updating the stock entry.");
            }

            await LoadViewBagData();
            return View(model);
        }

        // Index - List stock entries
        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> Index(int? storeId = null, string status = null, int pageNumber = 1)
        {
            try
            {
                var pageSize = 20;
                var entries = await _stockEntryService.GetStockEntriesAsync(pageNumber, pageSize, storeId, status);

                ViewBag.Stores = await _storeService.GetActiveStoresAsync();
                ViewBag.CurrentStoreId = storeId;
                ViewBag.CurrentStatus = status;

                return View(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock entries");
                TempData["Error"] = "An error occurred while loading stock entries.";
                return View(new PagedResult<StockEntryDto>());
            }
        }

        // Create stock entry
        [HasPermission(Permission.CreateStock)]
        public async Task<IActionResult> Create()
        {
            ViewBag.EntryNo = await _stockEntryService.GenerateEntryNoAsync();
            await LoadViewBagData();

            var model = new StockEntryDto
            {
                EntryDate = DateTime.Now,
                EntryType = "Direct",
                Items = new List<StockEntryItemDto>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateStock)]
        public async Task<IActionResult> Create(StockEntryDto model)
        {
            try
            {
                // Log the incoming model for debugging
                _logger.LogInformation($"Creating stock entry for Store: {model.StoreId}, Items Count: {model.Items?.Count ?? 0}");

                // Validate model
                if (model.Items == null || !model.Items.Any())
                {
                    ModelState.AddModelError("", "Please add at least one item to the stock entry.");
                    _logger.LogWarning("Stock entry creation failed: No items provided");
                }

                // Additional validation
                foreach (var item in model.Items ?? new List<StockEntryItemDto>())
                {
                    if (item.ItemId <= 0)
                    {
                        ModelState.AddModelError("", $"Invalid item selected");
                        _logger.LogWarning($"Invalid ItemId: {item.ItemId}");
                    }
                    if (item.Quantity <= 0)
                    {
                        ModelState.AddModelError("", $"Quantity must be greater than 0 for all items");
                        _logger.LogWarning($"Invalid Quantity for ItemId {item.ItemId}: {item.Quantity}");
                    }
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var result = await _stockEntryService.CreateStockEntryAsync(model);

                        _logger.LogInformation($"Stock entry created successfully: {result.EntryNo}");
                        TempData["Success"] = $"Stock entry {result.EntryNo} created successfully!";

                        // If barcodes were generated, show option to print
                        if (result.BarcodesGenerated > 0)
                        {
                            TempData["Info"] = $"{result.BarcodesGenerated} barcodes generated. You can print them from the details page.";
                        }

                        return RedirectToAction(nameof(Details), new { id = result.Id });
                    }
                    catch (InvalidOperationException ioEx)
                    {
                        _logger.LogError(ioEx, "Invalid operation while creating stock entry");
                        ModelState.AddModelError("", $"Operation Error: {ioEx.Message}");
                        TempData["Error"] = $"Operation Error: {ioEx.Message}";
                    }
                    catch (DbUpdateException dbEx)
                    {
                        _logger.LogError(dbEx, "Database error while creating stock entry");
                        var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                        ModelState.AddModelError("", $"Database Error: {innerMessage}");
                        TempData["Error"] = $"Database Error: {innerMessage}";
                    }
                }
                else
                {
                    // Log validation errors
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                        .ToList();

                    _logger.LogWarning($"Model validation failed: {System.Text.Json.JsonSerializer.Serialize(errors)}");
                    TempData["Error"] = "Please check the form for validation errors.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating stock entry");
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                TempData["Error"] = $"Unexpected Error: {ex.Message}";

                // In development, show more details
                if (_hostEnvironment.IsDevelopment())
                {
                    ModelState.AddModelError("", $"Stack Trace: {ex.StackTrace}");
                }
            }

            // Reload ViewBag data before returning to view
            await LoadViewBagData();

            // Log the final model state
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogError($"Returning to view with errors: {string.Join("; ", allErrors)}");
            }

            return View(model);
        }

        // View details
        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var entry = await _stockEntryService.GetStockEntryByIdAsync(id);
                if (entry == null)
                {
                    TempData["Error"] = "Stock entry not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get generated barcodes
                var barcodes = new List<BarcodeDto>();
                foreach (var item in entry.Items.Where(i => i.BarcodesGenerated > 0))
                {
                    var itemBarcodes = await _barcodeService.GetBarcodesByItemAsync(item.ItemId);
                    barcodes.AddRange(itemBarcodes.Take(item.BarcodesGenerated));
                }
                ViewBag.GeneratedBarcodes = barcodes;

                return View(entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock entry details");
                TempData["Error"] = "An error occurred while loading stock entry details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Print barcodes
        [HasPermission(Permission.PrintBarcode)]
        public async Task<IActionResult> PrintBarcodes(int id)
        {
            try
            {
                var entry = await _stockEntryService.GetStockEntryByIdAsync(id);
                if (entry == null)
                {
                    TempData["Error"] = "Stock entry not found.";
                    return RedirectToAction(nameof(Index));
                }

                var barcodes = new List<BarcodeDto>();
                foreach (var item in entry.Items.Where(i => i.GeneratedBarcodeNumbers.Any()))
                {
                    foreach (var barcodeNumber in item.GeneratedBarcodeNumbers)
                    {
                        var barcode = await _barcodeService.GetBarcodeByNumberAsync(barcodeNumber);
                        if (barcode != null)
                            barcodes.Add(barcode);
                    }
                }

                ViewBag.StockEntry = entry;
                return View(barcodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing barcodes for printing");
                TempData["Error"] = "An error occurred while preparing barcodes.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }


        [HttpPost]
        [HasPermission(Permission.CreateStock)]
        public async Task<IActionResult> BulkUpload(IFormFile file, int? storeId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "Please select a file to upload.";
                    return RedirectToAction(nameof(BulkUpload));
                }

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "Please upload a CSV file.";
                    return RedirectToAction(nameof(BulkUpload));
                }

                using var stream = file.OpenReadStream();
                var validation = await _stockEntryService.ValidateBulkUploadAsync(stream, storeId);
                validation.FileName = file.FileName;

                if (validation.Errors.Any() || validation.FailedRows.Count() > 0)
                {
                    ViewBag.ValidationResult = validation;
                    ViewBag.Stores = await _storeService.GetActiveStoresAsync();
                    return View("BulkUploadValidation", validation);
                }

                var result = await _stockEntryService.ProcessBulkUploadAsync(validation);

                TempData["Success"] = $"Bulk upload completed! {result.SuccessfulRows} items processed successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing bulk upload");
                TempData["Error"] = "An error occurred while processing the bulk upload.";
                return RedirectToAction(nameof(BulkUpload));
            }
        }

        // Stock adjustment
        [HasPermission(Permission.AdjustStock)]
        public async Task<IActionResult> Adjustment()
        {
            ViewBag.AdjustmentNo = await _stockEntryService.GenerateAdjustmentNoAsync();
            await LoadViewBagData();

            var model = new StockAdjustmentDto
            {
                AdjustmentDate = DateTime.Now,
                AdjustmentType = "Correction",
                Items = new List<StockAdjustmentItemDto>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.AdjustStock)]
        public async Task<IActionResult> Adjustment(StockAdjustmentDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _stockEntryService.CreateStockAdjustmentAsync(model);

                    TempData["Success"] = $"Stock adjustment {result.AdjustmentNo} created successfully!";
                    TempData["Info"] = "The adjustment is pending approval.";

                    return RedirectToAction("AdjustmentDetails", new { id = result.Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock adjustment");
                ModelState.AddModelError("", "An error occurred while creating the stock adjustment.");
            }

            await LoadViewBagData();
            return View(model);
        }

        // Adjustment details
        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> AdjustmentDetails(int id)
        {
            try
            {
                var adjustment = await _stockEntryService.GetStockAdjustmentByIdAsync(id);
                if (adjustment == null)
                {
                    TempData["Error"] = "Stock adjustment not found.";
                    return RedirectToAction("AdjustmentList");
                }

                return View(adjustment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading adjustment details");
                TempData["Error"] = "An error occurred while loading adjustment details.";
                return RedirectToAction("AdjustmentList");
            }
        }

        // Approve adjustment
        [HttpPost]
        [HasPermission(Permission.ApproveStock)]
        public async Task<IActionResult> ApproveAdjustment(int id)
        {
            try
            {
                var result = await _stockEntryService.ApproveStockAdjustmentAsync(id, User.Identity.Name);
                if (result)
                {
                    TempData["Success"] = "Stock adjustment approved successfully!";
                }
                else
                {
                    TempData["Error"] = "Unable to approve stock adjustment.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving stock adjustment");
                TempData["Error"] = "An error occurred while approving the adjustment.";
            }

            return RedirectToAction("AdjustmentDetails", new { id });
        }


        [HttpGet]
        public async Task<IActionResult> GetItemStock(int itemId, int? storeId)
        {
            try
            {
                var item = await _unitOfWork.Items.GetByIdAsync(itemId);
                if (item == null)
                {
                    return Json(new { success = false, message = "Item not found" });
                }

                decimal currentStock = 0;
                if (storeId.HasValue)
                {
                    var storeItem = await _unitOfWork.StoreItems
                        .FirstOrDefaultAsync(si => si.ItemId == itemId && si.StoreId == storeId.Value && si.IsActive);
                    currentStock = storeItem?.Quantity ?? 0;
                }

                return Json(new
                {
                    success = true,
                    currentStock = currentStock,
                    minimumStock = item.MinimumStock,
                    reorderLevel = item.ReorderLevel,
                    itemName = item.Name,
                    itemCode = item.ItemCode,
                    unit = item.Unit ?? "Unit",
                    unitPrice = item.UnitPrice ?? 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item stock");
                return Json(new { success = false, message = "Error retrieving stock information" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemsByCategory(int categoryId)
        {
            try
            {
                // First get all subcategories for this category
                var subCategories = await _unitOfWork.SubCategories
                    .FindAsync(sc => sc.CategoryId == categoryId && sc.IsActive);

                var subCategoryIds = subCategories.Select(sc => sc.Id).ToList();

                // Then get all items for these subcategories
                var items = await _unitOfWork.Items.Query()
                    .Include(i => i.SubCategory)
                    .Where(i => subCategoryIds.Contains(i.SubCategoryId) && i.IsActive)
                    .Select(i => new
                    {
                        id = i.Id,
                        name = i.Name,
                        itemCode = i.ItemCode,
                        unitPrice = i.UnitPrice ?? 0,
                        unit = i.Unit ?? "Unit"
                    })
                    .ToListAsync();

                return Json(new { success = true, items = items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items by category");
                return Json(new { success = false, message = "Error retrieving items" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLowStockItems(int? storeId)
        {
            try
            {
                var items = await _stockEntryService.GetLowStockItemsAsync(storeId);
                return Json(new { success = true, items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock items");
                return Json(new { success = false, message = "Error retrieving low stock items" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckMultipleItemsStock([FromBody] CheckStockRequest request)
        {
            try
            {
                var stocks = await _stockEntryService.GetMultipleItemStocksAsync(request.StoreId, request.ItemIds);
                return Json(new { success = true, stocks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking multiple items stock");
                return Json(new { success = false, message = "Error checking stock" });
            }
        }

        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> ExportStock(string format, int? storeId = null, int? categoryId = null, string stockStatus = null)
        {
            try
            {
                _logger.LogInformation($"Exporting stock data in {format} format");

                // Get filtered stock data
                var stockData = await _stockEntryService.GetStockLevelsAsync(storeId, stockStatus == "LowStock");

                // Apply category filter if provided (filter by CategoryName instead of CategoryId)
                if (categoryId.HasValue)
                {
                    // Get category name from database
                    var category = await _unitOfWork.Categories.GetByIdAsync(categoryId.Value);
                    if (category != null)
                    {
                        stockData = stockData.Where(s => s.CategoryName == category.Name).ToList();
                    }
                }

                // Apply stock status filter if provided
                if (!string.IsNullOrEmpty(stockStatus))
                {
                    stockData = stockData.Where(s => s.StockStatus == stockStatus).ToList();
                }

                // Export based on format
                switch (format?.ToLower())
                {
                    case "csv":
                        return ExportToCSV(stockData);
                    case "excel":
                        return ExportToExcel(stockData);
                    case "pdf":
                        return ExportToPDF(stockData);
                    default:
                        TempData["Error"] = "Invalid export format";
                        return RedirectToAction(nameof(StockLevels));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stock data");
                TempData["Error"] = "An error occurred while exporting stock data.";
                return RedirectToAction(nameof(StockLevels));
            }
        }

        // Export to CSV
        private IActionResult ExportToCSV(IEnumerable<StockLevelReportDto> stockData)
        {
            var csv = new StringBuilder();

            // Headers
            csv.AppendLine("Item Code,Item Name,Category,Store,Current Stock,Min Stock,Max Stock,Unit,Unit Price,Stock Value,Status");

            // Data rows
            foreach (var item in stockData)
            {
                csv.AppendLine($"\"{item.ItemCode}\",\"{item.ItemName}\",\"{item.CategoryName}\",\"{item.StoreName}\"," +
                              $"{item.CurrentStock},{item.MinimumStock},{item.MaximumStock},\"{item.Unit}\"," +
                              $"{item.UnitPrice},{item.StockValue},\"{item.StockStatus}\"");
            }

            var fileName = $"StockLevels_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", fileName);
        }

        // Export to Excel
        private IActionResult ExportToExcel(IEnumerable<StockLevelReportDto> stockData)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Stock Levels");

                // Headers with styling
                var headers = new[] { "Item Code", "Item Name", "Category", "Store", "Current Stock",
                             "Min Stock", "Max Stock", "Unit", "Unit Price", "Stock Value", "Status" };

                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Data rows
                int row = 2;
                foreach (var item in stockData)
                {
                    worksheet.Cell(row, 1).Value = item.ItemCode;
                    worksheet.Cell(row, 2).Value = item.ItemName;
                    worksheet.Cell(row, 3).Value = item.CategoryName;
                    worksheet.Cell(row, 4).Value = item.StoreName;
                    worksheet.Cell(row, 5).Value = item.CurrentStock;
                    worksheet.Cell(row, 6).Value = item.MinimumStock ?? 0;
                    worksheet.Cell(row, 7).Value = item.MaximumStock ?? 0;
                    worksheet.Cell(row, 8).Value = item.Unit;
                    worksheet.Cell(row, 9).Value = item.UnitPrice;
                    worksheet.Cell(row, 10).Value = item.StockValue;
                    worksheet.Cell(row, 11).Value = item.StockStatus;

                    // Color code based on stock status
                    switch (item.StockStatus)
                    {
                        case "OutOfStock":
                            worksheet.Cell(row, 11).Style.Fill.BackgroundColor = XLColor.Red;
                            worksheet.Cell(row, 11).Style.Font.FontColor = XLColor.White;
                            break;
                        case "LowStock":
                            worksheet.Cell(row, 11).Style.Fill.BackgroundColor = XLColor.Yellow;
                            break;
                        case "InStock":
                            worksheet.Cell(row, 11).Style.Fill.BackgroundColor = XLColor.LightGreen;
                            break;
                    }

                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save to memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var fileName = $"StockLevels_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // Export to PDF (HTML format that can be printed to PDF)
        private IActionResult ExportToPDF(IEnumerable<StockLevelReportDto> stockData)
        {
            var html = new StringBuilder();
            html.AppendLine("<html><head><style>");
            html.AppendLine("table { border-collapse: collapse; width: 100%; font-size: 10px; }");
            html.AppendLine("th, td { border: 1px solid black; padding: 5px; text-align: left; }");
            html.AppendLine("th { background-color: #4CAF50; color: white; }");
            html.AppendLine(".out-of-stock { background-color: #ffcccc; }");
            html.AppendLine(".low-stock { background-color: #ffffcc; }");
            html.AppendLine("</style></head><body>");
            html.AppendLine($"<h2>Stock Levels Report - {DateTime.Now:dd/MM/yyyy HH:mm}</h2>");
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Item Code</th><th>Item Name</th><th>Category</th><th>Store</th>" +
                           "<th>Current Stock</th><th>Min Stock</th><th>Max Stock</th>" +
                           "<th>Unit</th><th>Unit Price</th><th>Stock Value</th><th>Status</th></tr>");

            foreach (var item in stockData)
            {
                var rowClass = item.StockStatus == "OutOfStock" ? "out-of-stock" :
                              item.StockStatus == "LowStock" ? "low-stock" : "";

                html.AppendLine($"<tr class='{rowClass}'>");
                html.AppendLine($"<td>{item.ItemCode}</td>");
                html.AppendLine($"<td>{item.ItemName}</td>");
                html.AppendLine($"<td>{item.CategoryName}</td>");
                html.AppendLine($"<td>{item.StoreName}</td>");
                html.AppendLine($"<td style='text-align:right'>{item.CurrentStock:N2}</td>");
                html.AppendLine($"<td style='text-align:right'>{item.MinimumStock:N2}</td>");
                html.AppendLine($"<td style='text-align:right'>{item.MaximumStock:N2}</td>");
                html.AppendLine($"<td>{item.Unit}</td>");
                html.AppendLine($"<td style='text-align:right'>{item.UnitPrice:N2}</td>");
                html.AppendLine($"<td style='text-align:right'>{item.StockValue:N2}</td>");
                html.AppendLine($"<td>{item.StockStatus}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table></body></html>");

            var fileName = $"StockLevels_{DateTime.Now:yyyyMMdd_HHmmss}.html";
            var bytes = Encoding.UTF8.GetBytes(html.ToString());

            return File(bytes, "text/html", fileName);
        }

        // Get Stock History for item
        [HttpGet]
        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> GetStockHistory(int itemId, int storeId)
        {
            try
            {
                // Get stock movements from database
                var history = await _unitOfWork.StockMovements
                    .Query()
                    .Where(sm => sm.ItemId == itemId && sm.StoreId == storeId)
                    .OrderByDescending(sm => sm.MovementDate)
                    .Take(20)
                    .Select(sm => new
                    {
                        date = sm.MovementDate,
                        type = sm.MovementType,
                        quantity = sm.Quantity,
                        reference = sm.ReferenceNo
                    })
                    .ToListAsync();

                return Json(new { success = true, history });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock history");
                return Json(new { success = false, message = "Failed to load stock history" });
            }
        }

        // UPDATE your existing StockLevels action with this corrected version
        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> StockLevels(int? storeId = null, int? categoryId = null, string stockStatus = null)
        {
            try
            {
                var stockLevels = await _stockEntryService.GetStockLevelsAsync(storeId, stockStatus == "LowStock");

                // Apply category filter if provided (filter by CategoryName)
                if (categoryId.HasValue)
                {
                    var category = await _unitOfWork.Categories.GetByIdAsync(categoryId.Value);
                    if (category != null)
                    {
                        stockLevels = stockLevels.Where(s => s.CategoryName == category.Name).ToList();
                    }
                }

                // Apply stock status filter if provided
                if (!string.IsNullOrEmpty(stockStatus))
                {
                    stockLevels = stockLevels.Where(s => s.StockStatus == stockStatus).ToList();
                }

                // Calculate summary statistics
                ViewBag.TotalItems = stockLevels.Count();
                ViewBag.TotalValue = stockLevels.Sum(s => s.StockValue);
                ViewBag.LowStockCount = stockLevels.Count(s => s.StockStatus == "LowStock");
                ViewBag.OutOfStockCount = stockLevels.Count(s => s.StockStatus == "OutOfStock");

                // Populate dropdowns
                ViewBag.Stores = await _storeService.GetActiveStoresAsync();

                // Get categories from database directly
                var categories = await _unitOfWork.Categories.GetAllAsync();
                ViewBag.Categories = categories.Where(c => c.IsActive).ToList();

                ViewBag.CurrentStoreId = storeId;
                ViewBag.CurrentCategoryId = categoryId;
                ViewBag.CurrentStatus = stockStatus;

                return View(stockLevels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock levels");
                TempData["Error"] = "An error occurred while loading stock levels.";
                return View(new List<StockLevelReportDto>());
            }
        }

        [HttpGet]
        [HasPermission(Permission.CreateStock)]
        public async Task<IActionResult> BulkUpload()
        {
            var stores = await _storeService.GetActiveStoresAsync();
            ViewBag.Stores = new SelectList(stores, "Id", "Name");
            return View();
        }


        [HttpPost]
        [HasPermission(Permission.CreateStock)]
        public async Task<IActionResult> ProcessBulkUpload(IFormFile file, int storeId)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload";
                return RedirectToAction(nameof(BulkUpload));
            }

            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<BulkStockItemDto>().ToList();

                var bulkUpload = new BulkStockUploadDto
                {
                    StoreId = storeId,
                    Items = records
                };

                var result = await _stockEntryService.BulkUploadStockAsync(bulkUpload);

                if (result.Success)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(BulkUpload));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error processing file: {ex.Message}";
                return RedirectToAction(nameof(BulkUpload));
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> DownloadTemplate()
        {
            var stream = await _stockEntryService.DownloadBulkUploadTemplateAsync();
            return File(stream, "text/csv", "stock_upload_template.csv");
        }

        private async Task LoadViewBagData()
        {
            ViewBag.Stores = await _storeService.GetActiveStoresAsync();
            ViewBag.Categories = await _itemService.GetAllCategoriesAsync();
        }

        // Request models
        public class CheckStockRequest
        {
            public int? StoreId { get; set; }
            public List<int> ItemIds { get; set; }
        }
    }
}