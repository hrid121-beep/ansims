// SettingsController.cs
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly ISettingService _settingService;

        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _settingService.GetAllSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Dictionary<string, string> settings)
        {
            foreach (var setting in settings)
            {
                await _settingService.UpdateSettingAsync(setting.Key, setting.Value);
            }

            TempData["Success"] = "Settings updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SystemInfo()
        {
            ViewBag.Version = "1.0.0";
            ViewBag.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return View();
        }

        public IActionResult Backup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBackup()
        {
            // Implement backup logic here
            TempData["Success"] = "Backup created successfully!";
            return RedirectToAction(nameof(Backup));
        }

        public IActionResult ImportExport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportData(string dataType, string format)
        {
            try
            {
                // Implement export logic here
                TempData["Success"] = $"{dataType} exported successfully!";
                return RedirectToAction(nameof(ImportExport));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error exporting data: {ex.Message}";
                return RedirectToAction(nameof(ImportExport));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportData(IFormFile file, string dataType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    TempData["Error"] = "Please select a file to import";
                    return RedirectToAction(nameof(ImportExport));
                }

                // Implement import logic here
                TempData["Success"] = $"{dataType} imported successfully!";
                return RedirectToAction(nameof(ImportExport));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error importing data: {ex.Message}";
                return RedirectToAction(nameof(ImportExport));
            }
        }
    }
}
