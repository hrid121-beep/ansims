using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StockAlertController : Controller
    {
        private readonly IStockAlertService _stockAlertService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;
        private readonly ILogger<StockAlertController> _logger;

        public StockAlertController(
            IStockAlertService stockAlertService,
            IStoreService storeService,
            IItemService itemService,
            ILogger<StockAlertController> logger)
        {
            _stockAlertService = stockAlertService;
            _storeService = storeService;
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string level = null, int? storeId = null, string search = null, int page = 1)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get all alerts for the user
                var dashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);

                var alerts = new List<StockAlertDto>();
                alerts.AddRange(dashboard.CriticalAlerts);
                alerts.AddRange(dashboard.WarningAlerts);
                alerts.AddRange(dashboard.InfoAlerts);

                // Apply filters
                if (!string.IsNullOrEmpty(level))
                {
                    alerts = alerts.Where(a => a.AlertLevel?.Equals(level, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }

                if (storeId.HasValue)
                {
                    alerts = alerts.Where(a => a.StoreId == storeId.Value).ToList();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    alerts = alerts.Where(a =>
                        a.ItemName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                        a.ItemCode?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                        a.StoreName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true
                    ).ToList();
                }

                // Populate ViewBag for filters
                ViewBag.Stores = await _storeService.GetAllStoresAsync();
                ViewBag.SelectedLevel = level;
                ViewBag.SelectedStoreId = storeId;
                ViewBag.SearchQuery = search;
                ViewBag.CurrentPage = page;

                // Pagination
                int pageSize = 20;
                var paginatedAlerts = alerts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.TotalPages = (int)Math.Ceiling(alerts.Count / (double)pageSize);

                return View(paginatedAlerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock alerts");
                TempData["Error"] = "Failed to load stock alerts. Please try again.";
                return View(new List<StockAlertDto>());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,LogisticsOfficer,StoreManager")]
        public async Task<IActionResult> AcknowledgeAlert(int itemId, int? storeId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _stockAlertService.AcknowledgeAlertAsync(itemId, storeId, userId);

                return Json(new { success = true, message = "Alert acknowledged successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acknowledging alert");
                return Json(new { success = false, message = "Failed to acknowledge alert: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,LogisticsOfficer")]
        public async Task<IActionResult> CheckAllStores()
        {
            try
            {
                var alerts = await _stockAlertService.CheckAllStoresForAlertsAsync();
                TempData["Success"] = $"Stock check completed. Found {alerts.Count} alerts.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking all stores");
                TempData["Error"] = "Failed to check all stores: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,LogisticsOfficer")]
        public async Task<IActionResult> SendStockAlerts()
        {
            try
            {
                await _stockAlertService.SendLowStockEmailsAsync();
                TempData["Success"] = "Stock alert emails sent successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending stock alerts");
                TempData["Error"] = "Failed to send stock alerts: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetAlertSummary()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var summary = await _stockAlertService.GetAlertSummaryAsync(userId);

                return Json(new
                {
                    success = true,
                    data = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alert summary");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetRecentAlerts(int count = 10)
        {
            try
            {
                // Return empty result if user is not authenticated
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = true, alerts = new List<object>(), count = 0 });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);

                var recentAlerts = dashboard.CriticalAlerts
                    .OrderByDescending(a => a.AlertDate)
                    .Take(count)
                    .Select(a => new
                    {
                        a.ItemName,
                        a.StoreName,
                        a.CurrentStock,
                        a.MinimumStock,
                        a.AlertLevel,
                        a.ItemId,
                        a.StoreId,
                        TimeAgo = GetTimeAgo(a.AlertDate)
                    })
                    .ToList();

                return Json(new
                {
                    success = true,
                    alerts = recentAlerts,
                    count = dashboard.CriticalAlerts.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent alerts");
                return Json(new { success = false, message = ex.Message, alerts = new List<object>(), count = 0 });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Export(string level = null, int? storeId = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);

                var alerts = new List<StockAlertDto>();
                alerts.AddRange(dashboard.CriticalAlerts);
                alerts.AddRange(dashboard.WarningAlerts);
                alerts.AddRange(dashboard.InfoAlerts);

                // Apply filters
                if (!string.IsNullOrEmpty(level))
                {
                    alerts = alerts.Where(a => a.AlertLevel?.Equals(level, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }

                if (storeId.HasValue)
                {
                    alerts = alerts.Where(a => a.StoreId == storeId.Value).ToList();
                }

                // Create CSV
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Stock Alerts Report");
                csv.AppendLine($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}");
                csv.AppendLine();
                csv.AppendLine("#,Item Code,Item Name,Store,Alert Level,Current Stock,Minimum Stock,Alert Date,Status");

                int serialNo = 1;
                foreach (var alert in alerts.OrderByDescending(a => a.AlertLevel))
                {
                    csv.AppendLine($"{serialNo}," +
                                  $"\"{alert.ItemCode}\"," +
                                  $"\"{EscapeCsv(alert.ItemName)}\"," +
                                  $"\"{EscapeCsv(alert.StoreName)}\"," +
                                  $"\"{alert.AlertLevel}\"," +
                                  $"{alert.CurrentStock}," +
                                  $"{alert.MinimumStock}," +
                                  $"\"{alert.AlertDate:yyyy-MM-dd}\"," +
                                  $"\"{alert.Status}\"");
                    serialNo++;
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"StockAlerts_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stock alerts");
                TempData["Error"] = "Failed to export alerts: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string level = null, int? storeId = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);

                var alerts = new List<StockAlertDto>();
                alerts.AddRange(dashboard.CriticalAlerts);
                alerts.AddRange(dashboard.WarningAlerts);
                alerts.AddRange(dashboard.InfoAlerts);

                // Apply filters
                if (!string.IsNullOrEmpty(level))
                {
                    alerts = alerts.Where(a => a.AlertLevel?.Equals(level, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }

                if (storeId.HasValue)
                {
                    alerts = alerts.Where(a => a.StoreId == storeId.Value).ToList();
                }

                // Generate Excel using ClosedXML
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Stock Alerts");

                // Title
                worksheet.Cell(1, 1).Value = "Stock Alerts Report";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                worksheet.Range(1, 1, 1, 9).Merge();

                // Date
                worksheet.Cell(2, 1).Value = $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}";
                worksheet.Range(2, 1, 2, 9).Merge();

                // Summary
                worksheet.Cell(3, 1).Value = $"Critical: {alerts.Count(a => a.AlertLevel == "Critical")} | " +
                    $"High: {alerts.Count(a => a.AlertLevel == "High")} | " +
                    $"Medium: {alerts.Count(a => a.AlertLevel == "Medium")} | " +
                    $"Low: {alerts.Count(a => a.AlertLevel == "Low")}";
                worksheet.Range(3, 1, 3, 9).Merge();

                // Headers
                var headerRow = 5;
                worksheet.Cell(headerRow, 1).Value = "#";
                worksheet.Cell(headerRow, 2).Value = "Item Code";
                worksheet.Cell(headerRow, 3).Value = "Item Name";
                worksheet.Cell(headerRow, 4).Value = "Store";
                worksheet.Cell(headerRow, 5).Value = "Alert Level";
                worksheet.Cell(headerRow, 6).Value = "Current Stock";
                worksheet.Cell(headerRow, 7).Value = "Minimum Stock";
                worksheet.Cell(headerRow, 8).Value = "Alert Date";
                worksheet.Cell(headerRow, 9).Value = "Status";

                worksheet.Range(headerRow, 1, headerRow, 9).Style.Font.Bold = true;
                worksheet.Range(headerRow, 1, headerRow, 9).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;

                // Data
                int row = headerRow + 1;
                int serialNo = 1;
                foreach (var alert in alerts.OrderByDescending(a => a.AlertLevel))
                {
                    worksheet.Cell(row, 1).Value = serialNo;
                    worksheet.Cell(row, 2).Value = alert.ItemCode;
                    worksheet.Cell(row, 3).Value = alert.ItemName;
                    worksheet.Cell(row, 4).Value = alert.StoreName;
                    worksheet.Cell(row, 5).Value = alert.AlertLevel;
                    worksheet.Cell(row, 6).Value = alert.CurrentStock;
                    worksheet.Cell(row, 7).Value = alert.MinimumStock;
                    worksheet.Cell(row, 8).Value = alert.AlertDate.ToString("dd-MMM-yyyy");
                    worksheet.Cell(row, 9).Value = alert.Status;

                    row++;
                    serialNo++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using var ms = new System.IO.MemoryStream();
                workbook.SaveAs(ms);

                var fileName = $"StockAlerts_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stock alerts to Excel");
                TempData["Error"] = "Failed to export to Excel: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf(string level = null, int? storeId = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dashboard = await _stockAlertService.GetPersonalizedAlertsAsync(userId);

                var alerts = new List<StockAlertDto>();
                alerts.AddRange(dashboard.CriticalAlerts);
                alerts.AddRange(dashboard.WarningAlerts);
                alerts.AddRange(dashboard.InfoAlerts);

                // Apply filters
                if (!string.IsNullOrEmpty(level))
                {
                    alerts = alerts.Where(a => a.AlertLevel?.Equals(level, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }

                if (storeId.HasValue)
                {
                    alerts = alerts.Where(a => a.StoreId == storeId.Value).ToList();
                }

                // Generate PDF using iTextSharp
                using var ms = new System.IO.MemoryStream();
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);

                document.Open();

                // Title
                var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
                var title = new iTextSharp.text.Paragraph("Stock Alerts Report", titleFont);
                title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(title);

                // Date
                var dateFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 10);
                var dateInfo = new iTextSharp.text.Paragraph($"Generated: {DateTime.Now:dd MMM yyyy HH:mm}", dateFont);
                dateInfo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                dateInfo.SpacingAfter = 10f;
                document.Add(dateInfo);

                // Summary
                var summaryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 9);
                var summary = new iTextSharp.text.Paragraph(
                    $"Critical: {alerts.Count(a => a.AlertLevel == "Critical")} | " +
                    $"High: {alerts.Count(a => a.AlertLevel == "High")} | " +
                    $"Medium: {alerts.Count(a => a.AlertLevel == "Medium")} | " +
                    $"Low: {alerts.Count(a => a.AlertLevel == "Low")}", summaryFont);
                summary.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                summary.SpacingAfter = 10f;
                document.Add(summary);

                // Table
                var table = new iTextSharp.text.pdf.PdfPTable(9);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 5f, 10f, 20f, 15f, 10f, 10f, 10f, 12f, 8f });

                // Headers
                var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 8);
                var headers = new[] { "#", "Item Code", "Item Name", "Store", "Alert Level", "Current", "Minimum", "Alert Date", "Status" };
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
                foreach (var alert in alerts.OrderByDescending(a => a.AlertLevel))
                {
                    table.AddCell(new iTextSharp.text.Phrase(serialNo.ToString(), dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(alert.ItemCode ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(alert.ItemName ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(alert.StoreName ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(alert.AlertLevel ?? "", dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(alert.CurrentStock.ToString(), dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(alert.MinimumStock.ToString(), dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(alert.AlertDate.ToString("dd-MMM-yyyy"), dataFont));
                    table.AddCell(new iTextSharp.text.Phrase(alert.Status ?? "", dataFont));
                    serialNo++;
                }

                document.Add(table);
                document.Close();

                var fileName = $"StockAlerts_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                return File(ms.ToArray(), "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stock alerts to PDF");
                TempData["Error"] = "Failed to export to PDF: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains("\""))
                value = value.Replace("\"", "\"\"");

            return value;
        }

        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.Now - date;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} min ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hr ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} days ago";

            return date.ToString("MMM dd, yyyy");
        }
    }
}
