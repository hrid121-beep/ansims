using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class PersonnelLifeTrackingController : Controller
    {
        private readonly IPersonnelItemLifeService _lifeService;
        private readonly IItemService _itemService;
        private readonly IBattalionService _battalionService;
        private readonly IStoreService _storeService;

        public PersonnelLifeTrackingController(
            IPersonnelItemLifeService lifeService,
            IItemService itemService,
            IBattalionService battalionService,
            IStoreService storeService)
        {
            _lifeService = lifeService;
            _itemService = itemService;
            _battalionService = battalionService;
            _storeService = storeService;
        }

        public async Task<IActionResult> Index(int daysAhead = 30)
        {
            ViewBag.DaysAhead = daysAhead;
            var items = await _lifeService.GetExpiringItemsAsync(daysAhead);

            // Get dashboard stats
            ViewBag.Stats = await _lifeService.GetLifeSpanDashboardAsync();

            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Items = await _itemService.GetControlledItemsSelectListAsync();
            ViewBag.Battalions = await _battalionService.GetSelectListAsync();
            ViewBag.Stores = await _storeService.GetSelectListAsync();

            return View(new PersonnelItemIssueDto
            {
                IssueDate = DateTime.Today,
                PersonnelType = "Ansar"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonnelItemIssueDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _lifeService.CreatePersonnelIssueAsync(dto);
                    TempData["Success"] = "Personnel item issue created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.Items = await _itemService.GetControlledItemsSelectListAsync();
            ViewBag.Battalions = await _battalionService.GetSelectListAsync();
            ViewBag.Stores = await _storeService.GetSelectListAsync();

            return View(dto);
        }

        public async Task<IActionResult> PersonnelItems(string badgeNo)
        {
            if (string.IsNullOrEmpty(badgeNo))
            {
                return View();
            }

            var items = await _lifeService.GetByPersonnelAsync(badgeNo);
            ViewBag.BadgeNo = badgeNo;

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Replace(int id, string reason)
        {
            try
            {
                await _lifeService.ReplaceItemAsync(id, reason);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> ExpiryReport()
        {
            var expiredItems = await _lifeService.GetExpiringItemsAsync(0);
            return View(expiredItems);
        }

        [HttpPost]
        public async Task<IActionResult> SendAlerts()
        {
            var result = await _lifeService.ProcessExpiryAlertsAsync();

            if (result)
            {
                TempData["Success"] = "Alerts sent successfully";
            }
            else
            {
                TempData["Error"] = "Failed to send alerts";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}