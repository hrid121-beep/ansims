using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StockAdjustmentController : Controller
    {
        private readonly IStockAdjustmentService _stockAdjustmentService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;

        public StockAdjustmentController(
            IStockAdjustmentService stockAdjustmentService,
            IItemService itemService,
            IStoreService storeService)
        {
            _stockAdjustmentService = stockAdjustmentService;
            _itemService = itemService;
            _storeService = storeService;
        }

        public async Task<IActionResult> Index()
        {
            var adjustments = await _stockAdjustmentService.GetAllStockAdjustmentsAsync();
            ViewBag.Statistics = await _stockAdjustmentService.GetAdjustmentStatisticsAsync();
            return View(adjustments);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.AdjustmentNo = await _stockAdjustmentService.GenerateAdjustmentNoAsync();
            await LoadViewBagData();
            return View(new StockAdjustmentDto { AdjustmentDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockAdjustmentDto adjustmentDto)
        {
            // Additional validation beyond DataAnnotations
            if (adjustmentDto.ItemId <= 0)
            {
                ModelState.AddModelError("ItemId", "Please select a valid item");
            }

            if (!adjustmentDto.StoreId.HasValue || adjustmentDto.StoreId.Value <= 0)
            {
                ModelState.AddModelError("StoreId", "Please select a valid store");
            }

            if (adjustmentDto.NewQuantity.HasValue && adjustmentDto.OldQuantity.HasValue
                && adjustmentDto.NewQuantity.Value == adjustmentDto.OldQuantity.Value)
            {
                ModelState.AddModelError("NewQuantity", "New quantity cannot be the same as current quantity");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _stockAdjustmentService.CreateStockAdjustmentAsync(adjustmentDto);
                    TempData["Success"] = "Stock adjustment created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException argEx)
                {
                    ModelState.AddModelError(string.Empty, argEx.Message);
                    TempData["Error"] = argEx.Message;
                }
                catch (InvalidOperationException invEx)
                {
                    ModelState.AddModelError(string.Empty, invEx.Message);
                    TempData["Error"] = invEx.Message;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the stock adjustment.");
                    TempData["Error"] = $"Error: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "Please correct the validation errors and try again.";
            }

            ViewBag.AdjustmentNo = adjustmentDto.AdjustmentNo;
            await LoadViewBagData();
            return View(adjustmentDto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var adjustment = await _stockAdjustmentService.GetStockAdjustmentByIdAsync(id);
            if (adjustment == null)
            {
                return NotFound();
            }
            return View(adjustment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _stockAdjustmentService.ApproveAdjustmentAsync(id, User.Identity.Name);
                TempData["Success"] = "Adjustment approved successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                await _stockAdjustmentService.RejectAdjustmentAsync(id, User.Identity.Name, reason);
                TempData["Success"] = "Adjustment rejected!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentStock(int itemId, int? storeId)
        {
            var stockLevel = await _stockAdjustmentService.GetCurrentStockLevelAsync(itemId, storeId);
            return Json(stockLevel);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemsByStore(int storeId)
        {
            var items = await _itemService.GetItemsByStoreAsync(storeId);
            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var pendingAdjustments = await _stockAdjustmentService.GetPendingAdjustmentsAsync();
            ViewBag.Title = "Pending Adjustments";
            return View("Index", pendingAdjustments);
        }

        [HttpGet]
        public async Task<IActionResult> Report(DateTime? fromDate, DateTime? toDate, int? storeId = null, string adjustmentType = null)
        {
            fromDate ??= DateTime.Today.AddMonths(-1);
            toDate ??= DateTime.Today;

            var adjustments = await _stockAdjustmentService.GetAdjustmentsByDateRangeAsync(fromDate.Value, toDate.Value);

            // Filter by store if provided
            if (storeId.HasValue)
            {
                adjustments = adjustments.Where(a => a.StoreId == storeId.Value).ToList();
            }

            // Filter by adjustment type if provided
            if (!string.IsNullOrEmpty(adjustmentType))
            {
                adjustments = adjustments.Where(a => a.AdjustmentType == adjustmentType).ToList();
            }

            ViewBag.Statistics = await _stockAdjustmentService.GetAdjustmentStatisticsAsync(fromDate, toDate);
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            // Load stores for dropdown
            var stores = await _storeService.GetAllStoresAsync();
            ViewBag.Stores = new SelectList(stores, "Id", "Name", storeId);

            return View(adjustments);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                fromDate ??= DateTime.Today.AddMonths(-1);
                toDate ??= DateTime.Today;

                var adjustments = await _stockAdjustmentService.GetAdjustmentsByDateRangeAsync(fromDate.Value, toDate.Value);

                using var ms = new System.IO.MemoryStream();
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

                document.Open();

                // Title
                var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
                var title = new iTextSharp.text.Paragraph("Stock Adjustment Report", titleFont);
                title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(title);

                // Date Range
                var dateFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
                var dateRange = new iTextSharp.text.Paragraph($"Period: {fromDate:dd MMM yyyy} - {toDate:dd MMM yyyy}", dateFont);
                dateRange.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                dateRange.SpacingAfter = 10f;
                document.Add(dateRange);

                // Table
                var table = new iTextSharp.text.pdf.PdfPTable(11);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 5f, 12f, 10f, 15f, 12f, 8f, 8f, 10f, 10f, 12f, 8f });

                // Headers
                var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 8);
                var headers = new[] { "#", "Adjustment No", "Date", "Item", "Store", "Old Qty", "New Qty", "Adjustment", "Type", "Reason", "Status" };
                foreach (var header in headers)
                {
                    var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(header, headerFont));
                    cell.BackgroundColor = iTextSharp.text.BaseColor.LightGray;
                    cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    table.AddCell(cell);
                }

                // Data
                var dataFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 7);
                int serialNo = 1;
                foreach (var item in adjustments.OrderByDescending(a => a.AdjustmentDate))
                {
                    table.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.AdjustmentNo ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.AdjustmentDate.ToString("dd-MMM-yyyy"), dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.ItemName ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.StoreName ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.OldQuantity?.ToString("N0") ?? "0", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.NewQuantity?.ToString("N0") ?? "0", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase((item.AdjustmentType == "Increase" ? "+" : "-") + item.AdjustmentQuantity?.ToString("N0"), dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.AdjustmentType ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.Reason?.Substring(0, Math.Min(20, item.Reason.Length)) ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.Status ?? "", dataFont));
                    serialNo++;
                }

                document.Add(table);
                document.Close();

                var fileName = $"StockAdjustment_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(ms.ToArray(), "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF: " + ex.Message;
                return RedirectToAction(nameof(Report), new { fromDate, toDate });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                fromDate ??= DateTime.Today.AddMonths(-1);
                toDate ??= DateTime.Today;

                var adjustments = await _stockAdjustmentService.GetAdjustmentsByDateRangeAsync(fromDate.Value, toDate.Value);

                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Stock Adjustments");

                // Title
                worksheet.Cell(1, 1).Value = "Stock Adjustment Report";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                worksheet.Range(1, 1, 1, 11).Merge();

                // Date Range
                worksheet.Cell(2, 1).Value = $"Period: {fromDate:dd MMM yyyy} - {toDate:dd MMM yyyy}";
                worksheet.Range(2, 1, 2, 11).Merge();

                // Headers
                var headerRow = 4;
                worksheet.Cell(headerRow, 1).Value = "#";
                worksheet.Cell(headerRow, 2).Value = "Adjustment No";
                worksheet.Cell(headerRow, 3).Value = "Date";
                worksheet.Cell(headerRow, 4).Value = "Item";
                worksheet.Cell(headerRow, 5).Value = "Store";
                worksheet.Cell(headerRow, 6).Value = "Old Qty";
                worksheet.Cell(headerRow, 7).Value = "New Qty";
                worksheet.Cell(headerRow, 8).Value = "Adjustment";
                worksheet.Cell(headerRow, 9).Value = "Type";
                worksheet.Cell(headerRow, 10).Value = "Reason";
                worksheet.Cell(headerRow, 11).Value = "Status";

                worksheet.Range(headerRow, 1, headerRow, 11).Style.Font.Bold = true;
                worksheet.Range(headerRow, 1, headerRow, 11).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;

                // Data
                int row = headerRow + 1;
                int serialNo = 1;
                foreach (var item in adjustments.OrderByDescending(a => a.AdjustmentDate))
                {
                    worksheet.Cell(row, 1).Value = serialNo;
                    worksheet.Cell(row, 2).Value = item.AdjustmentNo;
                    worksheet.Cell(row, 3).Value = item.AdjustmentDate.ToString("dd-MMM-yyyy");
                    worksheet.Cell(row, 4).Value = item.ItemName;
                    worksheet.Cell(row, 5).Value = item.StoreName;
                    worksheet.Cell(row, 6).Value = item.OldQuantity ?? 0;
                    worksheet.Cell(row, 7).Value = item.NewQuantity ?? 0;
                    worksheet.Cell(row, 8).Value = (item.AdjustmentType == "Increase" ? "+" : "-") + (item.AdjustmentQuantity ?? 0);
                    worksheet.Cell(row, 9).Value = item.AdjustmentType;
                    worksheet.Cell(row, 10).Value = item.Reason;
                    worksheet.Cell(row, 11).Value = item.Status;

                    row++;
                    serialNo++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using var ms = new System.IO.MemoryStream();
                workbook.SaveAs(ms);

                var fileName = $"StockAdjustment_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel: " + ex.Message;
                return RedirectToAction(nameof(Report), new { fromDate, toDate });
            }
        }

        private async Task LoadViewBagData()
        {
            var items = await _itemService.GetAllItemsAsync();
            var stores = await _storeService.GetAllStoresAsync();

            ViewBag.Items = new SelectList(items, "Id", "Name");
            ViewBag.Stores = new SelectList(stores, "Id", "Name");

            ViewBag.AdjustmentReasons = new SelectList(new[]
            {
                "Physical Count Variance",
                "Damaged Goods",
                "Expired Products",
                "System Error Correction",
                "Stock Take Adjustment",
                "Quality Issues",
                "Documentation Error",
                "Initial Stock Entry",
                "Other"
            });
        }
    }
}