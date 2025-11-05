using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class TemperatureController : Controller
    {
        private readonly ITemperatureLogService _temperatureLogService;
        private readonly IStoreService _storeService;
        private readonly ILogger<TemperatureController> _logger;

        public TemperatureController(
            ITemperatureLogService temperatureLogService,
            IStoreService storeService,
            ILogger<TemperatureController> logger)
        {
            _temperatureLogService = temperatureLogService;
            _storeService = storeService;
            _logger = logger;
        }

        [HttpGet]
        [HasPermission(Permission.ViewTemperatureLogs)]
        public async Task<IActionResult> Index(int? storeId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var logs = await _temperatureLogService.GetTemperatureLogsAsync(storeId, fromDate, toDate);
            var statistics = await _temperatureLogService.GetTemperatureStatisticsAsync(storeId, fromDate, toDate);

            ViewBag.Stores = await _storeService.GetActiveStoresAsync();
            ViewBag.Statistics = statistics;
            ViewBag.CurrentStoreId = storeId;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View(logs);
        }

        [HttpGet]
        [HasPermission(Permission.CreateTemperatureLog)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Stores = await _storeService.GetActiveStoresAsync();
            return View(new TemperatureLogDto
            {
                LogTime = DateTime.Now,
                Unit = "Celsius"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateTemperatureLog)]
        public async Task<IActionResult> Create(TemperatureLogDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _temperatureLogService.LogTemperatureAsync(dto);
                    TempData["Success"] = "Temperature logged successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error logging temperature");
                    ModelState.AddModelError("", "An error occurred while logging temperature.");
                }
            }

            ViewBag.Stores = await _storeService.GetActiveStoresAsync();
            return View(dto);
        }

        [HttpGet]
        [HasPermission(Permission.ViewTemperatureLogs)]
        public async Task<IActionResult> Alerts(int? storeId = null)
        {
            var alerts = await _temperatureLogService.GetAlertsAsync(storeId);

            ViewBag.Stores = await _storeService.GetActiveStoresAsync();
            ViewBag.CurrentStoreId = storeId;

            return View(alerts);
        }

        [HttpGet]
        [HasPermission(Permission.ExportReports)]
        public async Task<IActionResult> Export(int? storeId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var excel = await _temperatureLogService.GenerateTemperatureReportAsync(storeId, fromDate, toDate);
                return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"TemperatureReport_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting temperature report");
                TempData["Error"] = "An error occurred while generating the report.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
