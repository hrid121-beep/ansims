using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    [HasPermission(Permission.ViewAuditTrail)]
    public class AuditController : Controller
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditController> _logger;

        public AuditController(IAuditService auditService, ILogger<AuditController> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var from = fromDate ?? DateTime.Now.AddDays(-30);
            var to = toDate ?? DateTime.Now;

            var audits = await _auditService.GetAuditsByDateRangeAsync(from, to);

            ViewBag.FromDate = from;
            ViewBag.ToDate = to;

            return View(audits);
        }

        [HttpGet]
        public async Task<IActionResult> EntityTrail(string entity, int entityId)
        {
            var audits = await _auditService.GetAuditTrailAsync(entity, entityId);

            ViewBag.Entity = entity;
            ViewBag.EntityId = entityId;

            return View(audits);
        }

        [HttpGet]
        public async Task<IActionResult> UserActivity(string userId)
        {
            var audits = await _auditService.GetAuditsByUserAsync(userId);
            return View(audits);
        }

        [HttpGet]
        [HasPermission(Permission.ExportReports)]
        public async Task<IActionResult> Export(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var excel = await _auditService.GenerateAuditReportAsync(fromDate, toDate);
                return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"AuditReport_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting audit report");
                TempData["Error"] = "An error occurred while generating the report.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
