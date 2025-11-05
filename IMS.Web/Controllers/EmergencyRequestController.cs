using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class EmergencyRequestController : Controller
    {
        private readonly IIssueService _issueService;
        private readonly IStoreService _storeService;

        public EmergencyRequestController(
            IIssueService issueService,
            IStoreService storeService)
        {
            _issueService = issueService;
            _storeService = storeService;
        }

        [HttpGet]
        [HasPermission(Permission.CreateEmergencyRequest)]
        public IActionResult Create()
        {
            return View(new EmergencyRequestViewModel());
        }

        [HttpPost]
        [HasPermission(Permission.CreateEmergencyRequest)]
        public async Task<IActionResult> Create(EmergencyRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new EmergencyRequestDto
            {
                IssuedToType = model.IssuedToType,
                IssuedToIndividualName = model.RecipientName,
                IssuedToIndividualBadgeNo = model.BadgeNo,
                Purpose = model.Purpose,
                PreferredStoreId = model.PreferredStoreId,
                Items = model.Items.Select(i => new EmergencyItemDto
                {
                    ItemId = i.ItemId,
                    Quantity = i.Quantity,
                    Urgency = "Critical"
                }).ToList()
            };

            var result = await _issueService.CreateEmergencyRequestAsync(dto);

            if (result.Success)
            {
                TempData["Success"] = "Emergency request created and fast-tracked!";
                return RedirectToAction("Details", "Issue", new { id = result.Data });
            }

            TempData["Error"] = result.Message;
            return View(model);
        }
    }
}
