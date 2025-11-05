using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class ExpiryTrackingController : Controller
    {
        private readonly IExpiryTrackingService _expiryTrackingService;
        private readonly IItemService _itemService;
        private readonly IStoreService _storeService;
        private readonly ILogger<ExpiryTrackingController> _logger;

        public ExpiryTrackingController(
            IExpiryTrackingService expiryTrackingService,
            IItemService itemService,
            IStoreService storeService,
            ILogger<ExpiryTrackingController> logger)
        {
            _expiryTrackingService = expiryTrackingService;
            _itemService = itemService;
            _storeService = storeService;
            _logger = logger;
        }

        [HttpGet]
        [HasPermission(Permission.ViewExpiryTracking)]
        public async Task<IActionResult> Index(int daysBeforeExpiry = 30)
        {
            var expiringItems = await _expiryTrackingService.GetExpiringItemsAsync(daysBeforeExpiry);
            var statistics = await _expiryTrackingService.GetExpiryStatisticsAsync(null);

            ViewBag.Statistics = statistics;
            ViewBag.DaysBeforeExpiry = daysBeforeExpiry;

            return View(expiringItems);
        }

        [HttpGet]
        [HasPermission(Permission.ViewExpiryTracking)]
        public async Task<IActionResult> Expired()
        {
            var expiredItems = await _expiryTrackingService.GetExpiredItemsAsync();
            return View(expiredItems);
        }

        [HttpGet]
        [HasPermission(Permission.CreateExpiryTracking)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Items = await _itemService.GetAllItemsAsync();
            ViewBag.Stores = await _storeService.GetActiveStoresAsync();

            return View(new ExpiryTrackingDto
            {
                ExpiryDate = DateTime.Now.AddMonths(6)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateExpiryTracking)]
        public async Task<IActionResult> Create(ExpiryTrackingDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _expiryTrackingService.AddExpiryTrackingAsync(dto);
                    TempData["Success"] = "Expiry tracking added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding expiry tracking");
                    ModelState.AddModelError("", "An error occurred while adding expiry tracking.");
                }
            }

            ViewBag.Items = await _itemService.GetAllItemsAsync();
            ViewBag.Stores = await _storeService.GetActiveStoresAsync();
            return View(dto);
        }

        [HttpPost]
        [HasPermission(Permission.ProcessExpiredItems)]
        public async Task<IActionResult> ProcessExpired(int id, string disposalReason)
        {
            try
            {
                var result = await _expiryTrackingService.ProcessExpiredItemAsync(id, disposalReason);
                if (result)
                {
                    TempData["Success"] = "Expired item processed successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to process expired item.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expired item");
                TempData["Error"] = "An error occurred while processing the item.";
            }

            return RedirectToAction(nameof(Expired));
        }

        [HttpPost]
        [HasPermission(Permission.SendAlerts)]
        public async Task<IActionResult> SendAlerts()
        {
            try
            {
                await _expiryTrackingService.SendExpiryAlertsAsync();
                TempData["Success"] = "Expiry alerts sent successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending expiry alerts");
                TempData["Error"] = "An error occurred while sending alerts.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
