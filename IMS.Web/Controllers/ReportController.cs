using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IMS.Web.Controllers
{
    [Authorize]
    [HasPermission(Permission.ViewReports)] // CRITICAL FIX: Require ViewReports permission for all report access
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IStoreService _storeService;
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(
            IReportService reportService,
            IStoreService storeService,
            ICategoryService categoryService,
            IUnitOfWork unitOfWork)
        {
            _reportService = reportService;
            _storeService = storeService;
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            // Get real-time statistics
            var totalPurchases = await _unitOfWork.Purchases.CountAsync(p => p.IsActive);
            var totalIssues = await _unitOfWork.Issues.CountAsync(i => i.IsActive);
            var totalTransfers = await _unitOfWork.Transfers.CountAsync(t => t.IsActive);

            // Calculate total losses (damages + write-offs)
            var totalDamages = await _unitOfWork.Damages.CountAsync(d => d.IsActive);
            var totalWriteOffs = await _unitOfWork.WriteOffs.CountAsync(w => w.IsActive);
            var totalLosses = totalDamages + totalWriteOffs;

            ViewBag.TotalPurchases = totalPurchases;
            ViewBag.TotalIssues = totalIssues;
            ViewBag.TotalTransfers = totalTransfers;
            ViewBag.TotalLosses = totalLosses;

            return View();
        }

        [HasPermission(Permission.ViewStockReport)]
        public async Task<IActionResult> StockReport(int? storeId, int? categoryId)
        {
            var report = await _reportService.GetStockReportAsync(storeId, categoryId);
            await LoadViewBagData();
            return View(report);
        }

        public async Task<IActionResult> IssueReport(DateTime? fromDate, DateTime? toDate, int? storeId = null)
        {
            // Set default date range if not provided
            fromDate ??= DateTime.Now.AddMonths(-1);
            toDate ??= DateTime.Now;

            var report = await _reportService.GetIssueReportAsync(fromDate, toDate, storeId);

            // Load stores for filter dropdown
            await LoadStoresViewBag();

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(report);
        }

        public async Task<IActionResult> PurchaseReport(DateTime? fromDate, DateTime? toDate, int? vendorId = null)
        {
            fromDate ??= DateTime.Now.AddMonths(-1);
            toDate ??= DateTime.Now;

            var report = await _reportService.GetPurchaseReportAsync(fromDate, toDate, vendorId);

            // Load vendors for filter dropdown
            await LoadVendorsViewBag();

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(report);
        }

        public async Task<IActionResult> TransferReport(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null)
        {
            fromDate ??= DateTime.Now.AddMonths(-1);
            toDate ??= DateTime.Now;

            var report = await _reportService.GetTransferReportAsync(fromDate, toDate, fromStoreId, toStoreId);

            // Load stores for filter dropdowns
            await LoadStoresViewBag();

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(report);
        }

        public async Task<IActionResult> LossReport(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null)
        {
            fromDate ??= DateTime.Now.AddMonths(-1);
            toDate ??= DateTime.Now;

            var report = await _reportService.GetLossReportAsync(fromDate, toDate, lossType, storeId);

            // Load filter data
            await LoadStoresViewBag();
            LoadLossTypesViewBag();

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");
            ViewBag.DateRange = $"{fromDate.Value:MM/dd/yyyy} - {toDate.Value:MM/dd/yyyy}";

            return View(report);
        }

        public async Task<IActionResult> MovementHistory(
            DateTime? fromDate,
            DateTime? toDate,
            int? itemId = null,
            string movementType = null,
            int? storeId = null)
        {
            fromDate ??= DateTime.Now.AddDays(-7); // Default to last 7 days
            toDate ??= DateTime.Now;

            var movements = await _reportService.GetMovementHistoryAsync(fromDate, toDate, itemId, movementType, storeId);

            // Load filter data
            await LoadStoresViewBag();
            await LoadItemsViewBag();
            LoadMovementTypesViewBag();

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(movements);
        }

        public async Task<IActionResult> Summary()
        {
            var summaryData = await _reportService.GetInventorySummaryAsync();
            return View(summaryData);
        }

        // Export Methods
        public async Task<IActionResult> ExportStockReport(int? storeId, int? categoryId)
        {
            try
            {
                var reportData = await _reportService.GenerateStockReportExcelAsync(storeId, categoryId);
                var fileName = $"StockReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(StockReport));
            }
        }

        public async Task<IActionResult> ExportIssueReport(DateTime? fromDate, DateTime? toDate, int? storeId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateIssueReportExcelAsync(fromDate, toDate, storeId);
                var fileName = $"IssueReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(IssueReport));
            }
        }

        public async Task<IActionResult> ExportPurchaseReport(DateTime? fromDate, DateTime? toDate, int? vendorId = null)
        {
            try
            {
                var reportData = await _reportService.GeneratePurchaseReportExcelAsync(fromDate, toDate, vendorId);
                var fileName = $"PurchaseReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(PurchaseReport));
            }
        }

        public async Task<IActionResult> ExportPurchaseReportCsv(DateTime? fromDate, DateTime? toDate, int? vendorId = null)
        {
            try
            {
                var reportData = await _reportService.GeneratePurchaseReportCsvAsync(fromDate, toDate, vendorId);
                var fileName = $"PurchaseReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(reportData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating CSV report: " + ex.Message;
                return RedirectToAction(nameof(PurchaseReport));
            }
        }

        public async Task<IActionResult> ExportPurchaseReportPdf(DateTime? fromDate, DateTime? toDate, int? vendorId = null)
        {
            try
            {
                var reportData = await _reportService.GeneratePurchaseReportPdfAsync(fromDate, toDate, vendorId);
                var fileName = $"PurchaseReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(PurchaseReport));
            }
        }

        public async Task<IActionResult> ExportTransferReport(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateTransferReportExcelAsync(fromDate, toDate, fromStoreId, toStoreId);
                var fileName = $"TransferReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(TransferReport));
            }
        }

        public async Task<IActionResult> ExportTransferReportCsv(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateTransferReportCsvAsync(fromDate, toDate, fromStoreId, toStoreId);
                var fileName = $"TransferReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(reportData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating CSV report: " + ex.Message;
                return RedirectToAction(nameof(TransferReport));
            }
        }

        public async Task<IActionResult> ExportTransferReportPdf(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateTransferReportPdfAsync(fromDate, toDate, fromStoreId, toStoreId);
                var fileName = $"TransferReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(TransferReport));
            }
        }

        public async Task<IActionResult> ExportLossReport(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateLossReportExcelAsync(fromDate, toDate, lossType, storeId);
                var fileName = $"LossReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(LossReport));
            }
        }

        public async Task<IActionResult> ExportLossReportCsv(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateLossReportCsvAsync(fromDate, toDate, lossType, storeId);
                var fileName = $"LossReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(reportData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating CSV report: " + ex.Message;
                return RedirectToAction(nameof(LossReport));
            }
        }

        public async Task<IActionResult> ExportLossReportPdf(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateLossReportPdfAsync(fromDate, toDate, lossType, storeId);
                var fileName = $"LossReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(LossReport));
            }
        }

        public async Task<IActionResult> ExportMovementHistory(DateTime? fromDate, DateTime? toDate, int? itemId = null, string movementType = null, int? storeId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateMovementHistoryExcelAsync(fromDate, toDate, itemId, movementType, storeId);
                var fileName = $"MovementHistory_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(MovementHistory));
            }
        }

        // Batch Report Generation
        [HttpPost]
        public async Task<IActionResult> GenerateBatchReport(string reportType, DateTime? fromDate, DateTime? toDate, int? storeId, int? categoryId)
        {
            try
            {
                var parameters = new Dictionary<string, object>();

                if (fromDate.HasValue) parameters["fromDate"] = fromDate.Value;
                if (toDate.HasValue) parameters["toDate"] = toDate.Value;
                if (storeId.HasValue) parameters["storeId"] = storeId.Value;
                if (categoryId.HasValue) parameters["categoryId"] = categoryId.Value;

                var batchReport = await _reportService.GenerateBatchReportAsync(reportType, parameters);

                return File(batchReport.Data, batchReport.MimeType, batchReport.FileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating report: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX Methods for Dashboard Widgets
        public async Task<IActionResult> GetStockSummary()
        {
            var report = await _reportService.GetStockReportAsync();
            var categoryStock = await _reportService.GetCategoryWiseStockAsync();

            return Json(new
            {
                totalItems = report.TotalItems,
                totalValue = report.TotalValue,
                lowStockItems = report.LowStockItems,
                outOfStockItems = report.OutOfStockItems,
                categoryData = categoryStock.Select(c => new
                {
                    name = c.CategoryName,
                    value = c.TotalValue,
                    count = c.ItemCount
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetReportStatistics()
        {
            var stats = await _reportService.GetReportStatisticsAsync();
            return Json(stats);
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyTrend(string reportType, int months = 6)
        {
            var trendData = await _reportService.GetMonthlyTrendAsync(reportType, months);
            return Json(trendData);
        }

        // Private helper methods
        private async Task LoadViewBagData()
        {
            var stores = await _storeService.GetAllStoresAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();

            var storeList = stores.Select(s => new { Id = s.Id.ToString(), Name = s.Name }).ToList();
            storeList.Insert(0, new { Id = "", Name = "All Stores" });

            var categoryList = categories.Select(c => new { Id = c.Id.ToString(), Name = c.Name }).ToList();
            categoryList.Insert(0, new { Id = "", Name = "All Categories" });

            ViewBag.Stores = new SelectList(storeList, "Id", "Name");
            ViewBag.Categories = new SelectList(categoryList, "Id", "Name");
        }

        private async Task LoadStoresViewBag()
        {
            var stores = await _storeService.GetAllStoresAsync();
            var storeList = stores.Select(s => new { Id = s.Id.ToString(), Name = s.Name }).ToList();
            storeList.Insert(0, new { Id = "", Name = "All Stores" });
            ViewBag.Stores = new SelectList(storeList, "Id", "Name");
        }

        private async Task LoadVendorsViewBag()
        {
            var vendors = await _reportService.GetVendorsAsync();
            var vendorList = vendors.Select(v => new { Id = v.Id.ToString(), Name = v.Name }).ToList();
            vendorList.Insert(0, new { Id = "", Name = "All Vendors" });
            ViewBag.Vendors = new SelectList(vendorList, "Id", "Name");
        }

        private async Task LoadItemsViewBag()
        {
            var items = await _reportService.GetItemsAsync();
            var itemList = items.Select(i => new { Id = i.Id.ToString(), Name = i.Name }).ToList();
            itemList.Insert(0, new { Id = "", Name = "All Items" });
            ViewBag.Items = new SelectList(itemList, "Id", "Name");
        }

        private void LoadLossTypesViewBag()
        {
            var lossTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Types" },
                new SelectListItem { Value = "Damage", Text = "Damage" },
                new SelectListItem { Value = "Theft", Text = "Theft" },
                new SelectListItem { Value = "Expiry", Text = "Expiry" },
                new SelectListItem { Value = "WriteOff", Text = "Write-off" }
            };
            ViewBag.LossTypes = lossTypes;
        }

        private void LoadMovementTypesViewBag()
        {
            var movementTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Types" },
                new SelectListItem { Value = "Purchase", Text = "Purchase" },
                new SelectListItem { Value = "Issue", Text = "Issue" },
                new SelectListItem { Value = "Transfer", Text = "Transfer" },
                new SelectListItem { Value = "Loss", Text = "Loss" },
                new SelectListItem { Value = "Return", Text = "Return" }
            };
            ViewBag.MovementTypes = movementTypes;
        }

        // ========== NEW REPORT ACTIONS ==========

        public async Task<IActionResult> ConsumptionReport(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null)
        {
            fromDate ??= DateTime.Now.AddMonths(-3);
            toDate ??= DateTime.Now;

            var report = await _reportService.GetConsumptionAnalysisReportAsync(fromDate, toDate, storeId, categoryId);

            // Load filter data
            await LoadStoresViewBag();
            await LoadViewBagData();

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(report);
        }

        public async Task<IActionResult> ExpiryReport(int? storeId = null, int? daysAhead = 90)
        {
            var report = await _reportService.GetExpiryReportAsync(storeId, daysAhead);

            // Load stores for filter dropdown
            await LoadStoresViewBag();

            ViewBag.DaysAheadOptions = new SelectList(new[]
            {
                new { Value = 7, Text = "Next 7 Days" },
                new { Value = 30, Text = "Next 30 Days" },
                new { Value = 60, Text = "Next 60 Days" },
                new { Value = 90, Text = "Next 90 Days" },
                new { Value = 180, Text = "Next 6 Months" }
            }, "Value", "Text", daysAhead);

            return View(report);
        }

        public async Task<IActionResult> AuditReport(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null)
        {
            fromDate ??= DateTime.Now.AddDays(-7);
            toDate ??= DateTime.Now;

            var report = await _reportService.GetAuditTrailReportAsync(fromDate, toDate, transactionType, storeId);

            // Load filter data
            await LoadStoresViewBag();

            ViewBag.TransactionTypes = new SelectList(new[]
            {
                new { Value = "", Text = "All Transaction Types" },
                new { Value = "Purchase", Text = "Purchase" },
                new { Value = "Issue", Text = "Issue" },
                new { Value = "Transfer", Text = "Transfer" },
                new { Value = "StockAdjustment", Text = "Stock Adjustment" },
                new { Value = "Return", Text = "Return" },
                new { Value = "Damage", Text = "Damage" },
                new { Value = "WriteOff", Text = "Write-off" }
            }, "Value", "Text", transactionType);

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View(report);
        }

        public async Task<IActionResult> VarianceReport(int? physicalInventoryId = null)
        {
            if (!physicalInventoryId.HasValue)
            {
                // Load list of recent physical inventories to choose from
                var recentInventories = await _unitOfWork.PhysicalInventories
                    .Query()
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CountDate)
                    .Take(10)
                    .Select(p => new
                    {
                        p.Id,
                        p.ReferenceNumber,
                        p.CountDate,
                        p.Status
                    })
                    .ToListAsync();

                ViewBag.PhysicalInventories = new SelectList(recentInventories, "Id", "ReferenceNumber");
                return View("SelectPhysicalInventory");
            }

            var report = await _reportService.GetVarianceReportAsync(physicalInventoryId.Value);
            return View(report);
        }

        public async Task<IActionResult> ABCAnalysis(string analysisMethod = "Value", int months = 12)
        {
            var report = await _reportService.GetABCAnalysisReportAsync(analysisMethod, months);

            ViewBag.AnalysisMethods = new SelectList(new[]
            {
                new { Value = "Value", Text = "By Value (Recommended)" },
                new { Value = "Quantity", Text = "By Quantity" },
                new { Value = "Movement", Text = "By Movement Frequency" }
            }, "Value", "Text", analysisMethod);

            ViewBag.MonthsOptions = new SelectList(new[]
            {
                new { Value = 3, Text = "Last 3 Months" },
                new { Value = 6, Text = "Last 6 Months" },
                new { Value = 12, Text = "Last 12 Months" },
                new { Value = 24, Text = "Last 24 Months" }
            }, "Value", "Text", months);

            return View(report);
        }

        // ========== NEW REPORT EXPORT ACTIONS ==========

        public async Task<IActionResult> ExportConsumptionReport(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateConsumptionReportCsvAsync(fromDate, toDate, storeId, categoryId);
                var fileName = $"ConsumptionReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(reportData,
                    "text/csv",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating CSV report: " + ex.Message;
                return RedirectToAction(nameof(ConsumptionReport));
            }
        }

        public async Task<IActionResult> ExportExpiryReport(int? storeId = null, int? daysAhead = 90)
        {
            try
            {
                var reportData = await _reportService.GenerateExpiryReportExcelAsync(storeId, daysAhead);
                var fileName = $"ExpiryReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(ExpiryReport));
            }
        }

        public async Task<IActionResult> ExportAuditReport(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateAuditTrailReportExcelAsync(fromDate, toDate, transactionType, storeId);
                var fileName = $"AuditTrailReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(AuditReport));
            }
        }

        public async Task<IActionResult> ExportVarianceReport(int physicalInventoryId)
        {
            try
            {
                var reportData = await _reportService.GenerateVarianceReportExcelAsync(physicalInventoryId);
                var fileName = $"VarianceReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(VarianceReport));
            }
        }

        public async Task<IActionResult> ExportABCAnalysis(string analysisMethod = "Value", int months = 12)
        {
            try
            {
                var reportData = await _reportService.GenerateABCAnalysisReportExcelAsync(analysisMethod, months);
                var fileName = $"ABCAnalysis_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(ABCAnalysis));
            }
        }

        // ========== NEW REPORT PDF EXPORT ACTIONS ==========

        public async Task<IActionResult> ExportStockReportPdf(int? storeId, int? categoryId)
        {
            try
            {
                var reportData = await _reportService.GenerateStockReportPdfAsync(storeId, categoryId);
                var fileName = $"StockReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(StockReport));
            }
        }

        public async Task<IActionResult> ExportIssueReportPdf(DateTime? fromDate, DateTime? toDate, int? storeId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateIssueReportPdfAsync(fromDate, toDate, storeId);
                var fileName = $"IssueReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(IssueReport));
            }
        }

        public async Task<IActionResult> ExportConsumptionReportPdf(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateConsumptionReportPdfAsync(fromDate, toDate, storeId, categoryId);
                var fileName = $"ConsumptionReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(ConsumptionReport));
            }
        }

        public async Task<IActionResult> ExportExpiryReportPdf(int? storeId = null, int? daysAhead = 90)
        {
            try
            {
                var reportData = await _reportService.GenerateExpiryReportPdfAsync(storeId, daysAhead);
                var fileName = $"ExpiryReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(ExpiryReport));
            }
        }

        public async Task<IActionResult> ExportAuditReportPdf(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null)
        {
            try
            {
                var reportData = await _reportService.GenerateAuditTrailReportPdfAsync(fromDate, toDate, transactionType, storeId);
                var fileName = $"AuditTrailReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(AuditReport));
            }
        }

        public async Task<IActionResult> ExportVarianceReportPdf(int physicalInventoryId)
        {
            try
            {
                var reportData = await _reportService.GenerateVarianceReportPdfAsync(physicalInventoryId);
                var fileName = $"VarianceReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(VarianceReport));
            }
        }

        public async Task<IActionResult> ExportABCAnalysisPdf(string analysisMethod = "Value", int months = 12)
        {
            try
            {
                var reportData = await _reportService.GenerateABCAnalysisReportPdfAsync(analysisMethod, months);
                var fileName = $"ABCAnalysis_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(ABCAnalysis));
            }
        }

        // ========== CENTRAL STORE REGISTER REPORT (কেন্দ্রীয় ভান্ডার মজুদ তালিকা) ==========

        [HasPermission(Permission.ViewStockReport)]
        public async Task<IActionResult> CentralStoreRegister(
            int? storeId,
            int? categoryId,
            string sortBy = "Ledger",
            string period = "",
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Calculate date range based on period
            DateTime? calculatedStartDate = startDate;
            DateTime? calculatedEndDate = endDate;

            if (!string.IsNullOrEmpty(period) && period != "Custom")
            {
                var today = DateTime.Today;
                switch (period)
                {
                    case "Today":
                        calculatedStartDate = today;
                        calculatedEndDate = today;
                        break;
                    case "Yesterday":
                        calculatedStartDate = today.AddDays(-1);
                        calculatedEndDate = today.AddDays(-1);
                        break;
                    case "ThisWeek":
                        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                        calculatedStartDate = startOfWeek;
                        calculatedEndDate = today;
                        break;
                    case "LastWeek":
                        var lastWeekStart = today.AddDays(-(int)today.DayOfWeek - 7);
                        var lastWeekEnd = lastWeekStart.AddDays(6);
                        calculatedStartDate = lastWeekStart;
                        calculatedEndDate = lastWeekEnd;
                        break;
                    case "ThisMonth":
                        calculatedStartDate = new DateTime(today.Year, today.Month, 1);
                        calculatedEndDate = today;
                        break;
                    case "LastMonth":
                        var lastMonth = today.AddMonths(-1);
                        calculatedStartDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                        calculatedEndDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                        break;
                }
            }

            var report = await _reportService.GetCentralStoreRegisterAsync(storeId, categoryId, sortBy, calculatedStartDate, calculatedEndDate);
            await LoadViewBagData();

            // Add sort options
            ViewBag.SortOptions = new SelectList(new[]
            {
                new { Value = "Ledger", Text = "Ledger Number (লেজার নং)" },
                new { Value = "Item", Text = "Item Name (নাম)" },
                new { Value = "Category", Text = "Category (শ্রেণী)" },
                new { Value = "Quantity", Text = "Quantity (পরিমাণ)" }
            }, "Value", "Text", sortBy);

            ViewBag.CurrentSort = sortBy;
            ViewBag.StartDate = calculatedStartDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = calculatedEndDate?.ToString("yyyy-MM-dd");

            return View(report);
        }

        [HasPermission(Permission.ViewStockReport)]
        public async Task<IActionResult> ExportCentralStoreRegisterPdf(
            int? storeId,
            int? categoryId,
            string sortBy = "Ledger",
            string period = "",
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                // Calculate date range based on period
                DateTime? calculatedStartDate = startDate;
                DateTime? calculatedEndDate = endDate;

                if (!string.IsNullOrEmpty(period) && period != "Custom")
                {
                    var today = DateTime.Today;
                    switch (period)
                    {
                        case "Today":
                            calculatedStartDate = today;
                            calculatedEndDate = today;
                            break;
                        case "Yesterday":
                            calculatedStartDate = today.AddDays(-1);
                            calculatedEndDate = today.AddDays(-1);
                            break;
                        case "ThisWeek":
                            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                            calculatedStartDate = startOfWeek;
                            calculatedEndDate = today;
                            break;
                        case "LastWeek":
                            var lastWeekStart = today.AddDays(-(int)today.DayOfWeek - 7);
                            var lastWeekEnd = lastWeekStart.AddDays(6);
                            calculatedStartDate = lastWeekStart;
                            calculatedEndDate = lastWeekEnd;
                            break;
                        case "ThisMonth":
                            calculatedStartDate = new DateTime(today.Year, today.Month, 1);
                            calculatedEndDate = today;
                            break;
                        case "LastMonth":
                            var lastMonth = today.AddMonths(-1);
                            calculatedStartDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                            calculatedEndDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                            break;
                    }
                }

                var reportData = await _reportService.GenerateCentralStoreRegisterPdfAsync(storeId, categoryId, sortBy, calculatedStartDate, calculatedEndDate);
                var fileName = $"CentralStoreRegister_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(reportData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating PDF report: " + ex.Message;
                return RedirectToAction(nameof(CentralStoreRegister));
            }
        }

        [HasPermission(Permission.ViewStockReport)]
        public async Task<IActionResult> ExportCentralStoreRegisterExcel(
            int? storeId,
            int? categoryId,
            string sortBy = "Ledger",
            string period = "",
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                // Calculate date range based on period
                DateTime? calculatedStartDate = startDate;
                DateTime? calculatedEndDate = endDate;

                if (!string.IsNullOrEmpty(period) && period != "Custom")
                {
                    var today = DateTime.Today;
                    switch (period)
                    {
                        case "Today":
                            calculatedStartDate = today;
                            calculatedEndDate = today;
                            break;
                        case "Yesterday":
                            calculatedStartDate = today.AddDays(-1);
                            calculatedEndDate = today.AddDays(-1);
                            break;
                        case "ThisWeek":
                            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                            calculatedStartDate = startOfWeek;
                            calculatedEndDate = today;
                            break;
                        case "LastWeek":
                            var lastWeekStart = today.AddDays(-(int)today.DayOfWeek - 7);
                            var lastWeekEnd = lastWeekStart.AddDays(6);
                            calculatedStartDate = lastWeekStart;
                            calculatedEndDate = lastWeekEnd;
                            break;
                        case "ThisMonth":
                            calculatedStartDate = new DateTime(today.Year, today.Month, 1);
                            calculatedEndDate = today;
                            break;
                        case "LastMonth":
                            var lastMonth = today.AddMonths(-1);
                            calculatedStartDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                            calculatedEndDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                            break;
                    }
                }

                var reportData = await _reportService.GenerateCentralStoreRegisterExcelAsync(storeId, categoryId, sortBy, calculatedStartDate, calculatedEndDate);
                var fileName = $"CentralStoreRegister_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(reportData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error generating Excel report: " + ex.Message;
                return RedirectToAction(nameof(CentralStoreRegister));
            }
        }
    }
}