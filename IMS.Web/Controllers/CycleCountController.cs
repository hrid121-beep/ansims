using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class CycleCountController : Controller
    {
        private readonly IInventoryCycleCountService _cycleCountService;
        private readonly IStoreService _storeService;
        private readonly IItemService _itemService;
        private readonly ILogger<CycleCountController> _logger;

        public CycleCountController(
            IInventoryCycleCountService cycleCountService,
            IStoreService storeService,
            IItemService itemService,
            ILogger<CycleCountController> logger)
        {
            _cycleCountService = cycleCountService;
            _storeService = storeService;
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        [HasPermission(Permission.ViewCycleCount)]
        public async Task<IActionResult> Index()
        {
            var cycleCounts = await _cycleCountService.GetActiveCycleCountsAsync();
            var statistics = await _cycleCountService.GetCycleCountStatisticsAsync(
                DateTime.Now.AddMonths(-6), DateTime.Now);

            ViewBag.Statistics = statistics;
            return View(cycleCounts);
        }

        [HttpGet]
        [HasPermission(Permission.CreateCycleCount)]
        public async Task<IActionResult> Create()
        {
            ViewBag.Stores = await _storeService.GetActiveStoresAsync();
            ViewBag.CountTypes = new SelectList(new[]
            {
            "Full", "Partial", "ABC", "Random"
        });

            return View(new CycleCountDto
            {
                CountDate = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateCycleCount)]
        public async Task<IActionResult> Create(CycleCountDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _cycleCountService.CreateCycleCountAsync(dto);
                    TempData["Success"] = "Cycle count created successfully!";
                    return RedirectToAction(nameof(Details), new { id = result.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating cycle count");
                    ModelState.AddModelError("", "An error occurred while creating cycle count.");
                }
            }

            ViewBag.Stores = await _storeService.GetActiveStoresAsync();
            ViewBag.CountTypes = new SelectList(new[]
            {
            "Full", "Partial", "ABC", "Random"
        });
            return View(dto);
        }

        [HttpGet]
        [HasPermission(Permission.ViewCycleCount)]
        public async Task<IActionResult> Details(int id)
        {
            var cycleCount = await _cycleCountService.GetCycleCountByIdAsync(id);
            if (cycleCount == null)
                return NotFound();

            return View(cycleCount);
        }

        [HttpPost]
        [HasPermission(Permission.StartCycleCount)]
        public async Task<IActionResult> Start(int id)
        {
            try
            {
                var result = await _cycleCountService.StartCycleCountAsync(id);
                if (result)
                {
                    TempData["Success"] = "Cycle count started successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to start cycle count.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting cycle count");
                TempData["Error"] = "An error occurred while starting the count.";
            }

            return RedirectToAction(nameof(Count), new { id });
        }

        [HttpGet]
        [HasPermission(Permission.PerformCycleCount)]
        public async Task<IActionResult> Count(int id)
        {
            var cycleCount = await _cycleCountService.GetCycleCountByIdAsync(id);
            if (cycleCount == null || cycleCount.Status != "InProgress")
                return NotFound();

            return View(cycleCount);
        }

        [HttpPost]
        [HasPermission(Permission.PerformCycleCount)]
        public async Task<IActionResult> UpdateCount(int cycleCountId, CycleCountItemDto itemDto)
        {
            try
            {
                var result = await _cycleCountService.AddCountItemAsync(cycleCountId, itemDto);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating count");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [HasPermission(Permission.CompleteCycleCount)]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                var result = await _cycleCountService.CompleteCycleCountAsync(id);
                if (result)
                {
                    TempData["Success"] = "Cycle count completed successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to complete cycle count.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing cycle count");
                TempData["Error"] = "An error occurred while completing the count.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [HasPermission(Permission.ApproveCycleCount)]
        public async Task<IActionResult> ApproveAdjustments(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var result = await _cycleCountService.ApproveAdjustmentsAsync(id, userId);
                if (result)
                {
                    TempData["Success"] = "Adjustments approved and applied successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to approve adjustments.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving adjustments");
                TempData["Error"] = "An error occurred while approving adjustments.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
