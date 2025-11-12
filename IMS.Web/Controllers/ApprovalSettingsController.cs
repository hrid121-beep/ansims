using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize(Roles = "Admin,DirectorGeneral")]
    public class ApprovalSettingsController : Controller
    {
        private readonly IApprovalService _approvalService;
        private readonly ILogger<ApprovalSettingsController> _logger;

        public ApprovalSettingsController(
            IApprovalService approvalService,
            ILogger<ApprovalSettingsController> logger)
        {
            _approvalService = approvalService;
            _logger = logger;
        }

        // GET: ApprovalSettings
        public async Task<IActionResult> Index()
        {
            try
            {
                var thresholds = await _approvalService.GetAllThresholdsAsync();
                var workflows = await _approvalService.GetAllWorkflowsAsync();
                var entityTypes = await _approvalService.GetConfiguredEntityTypesAsync();

                // Add all possible entity types
                var allEntityTypes = new List<string>
                {
                    "PURCHASE", "REQUISITION", "ISSUE", "TRANSFER",
                    "WRITEOFF", "STOCK_ADJUSTMENT", "PHYSICAL_INVENTORY",
                    "ALLOTMENT_LETTER", "STOCK_ENTRY"
                };

                // Add any configured types that aren't in the list
                foreach (var type in entityTypes)
                {
                    if (!string.IsNullOrEmpty(type) && !allEntityTypes.Contains(type.ToUpper()))
                    {
                        allEntityTypes.Add(type.ToUpper());
                    }
                }

                ViewBag.Thresholds = thresholds;
                ViewBag.Workflows = workflows;
                ViewBag.EntityTypes = allEntityTypes.OrderBy(e => e).ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading approval settings");
                TempData["Error"] = "Error loading approval settings: " + ex.Message;
                return View();
            }
        }

        // GET: ApprovalSettings/CreateThreshold
        public IActionResult CreateThreshold()
        {
            PrepareViewBags();
            return View(new ApprovalThresholdDto());
        }

        // POST: ApprovalSettings/CreateThreshold
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThreshold(ApprovalThresholdDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    PrepareViewBags();
                    return View(dto);
                }

                // Validate min/max amounts
                if (dto.MaxAmount.HasValue && dto.MaxAmount <= dto.MinAmount)
                {
                    ModelState.AddModelError("MaxAmount", "Maximum amount must be greater than minimum amount");
                    PrepareViewBags();
                    return View(dto);
                }

                await _approvalService.CreateThresholdAsync(dto);
                TempData["Success"] = "Approval threshold created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval threshold");
                TempData["Error"] = "Error creating threshold: " + ex.Message;
                PrepareViewBags();
                return View(dto);
            }
        }

        // GET: ApprovalSettings/EditThreshold/5
        public async Task<IActionResult> EditThreshold(int id)
        {
            try
            {
                var threshold = await _approvalService.GetThresholdByIdAsync(id);
                if (threshold == null)
                {
                    TempData["Error"] = "Threshold not found";
                    return RedirectToAction(nameof(Index));
                }

                PrepareViewBags();
                return View(threshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading threshold {id}");
                TempData["Error"] = "Error loading threshold: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ApprovalSettings/EditThreshold/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditThreshold(int id, ApprovalThresholdDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    PrepareViewBags();
                    return View(dto);
                }

                // Validate min/max amounts
                if (dto.MaxAmount.HasValue && dto.MaxAmount <= dto.MinAmount)
                {
                    ModelState.AddModelError("MaxAmount", "Maximum amount must be greater than minimum amount");
                    PrepareViewBags();
                    return View(dto);
                }

                await _approvalService.UpdateThresholdAsync(dto);
                TempData["Success"] = "Approval threshold updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating threshold {id}");
                TempData["Error"] = "Error updating threshold: " + ex.Message;
                PrepareViewBags();
                return View(dto);
            }
        }

        // POST: ApprovalSettings/DeleteThreshold/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteThreshold(int id)
        {
            try
            {
                var result = await _approvalService.DeleteThresholdAsync(id);
                if (result)
                {
                    TempData["Success"] = "Approval threshold deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Threshold not found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting threshold {id}");
                TempData["Error"] = "Error deleting threshold: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ApprovalSettings/ToggleThreshold/5
        [HttpPost]
        public async Task<IActionResult> ToggleThreshold(int id)
        {
            try
            {
                var result = await _approvalService.ToggleThresholdStatusAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Status updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Threshold not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling threshold {id}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: ApprovalSettings/CreateWorkflow
        public IActionResult CreateWorkflow()
        {
            PrepareViewBags();
            var dto = new ApprovalWorkflowDto
            {
                Levels = new List<WorkflowLevelDto>
                {
                    new WorkflowLevelDto { Level = 1, CanEscalate = true, TimeoutHours = 24 }
                }
            };
            return View(dto);
        }

        // POST: ApprovalSettings/CreateWorkflow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkflow(ApprovalWorkflowDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    PrepareViewBags();
                    return View(dto);
                }

                // Validate workflow levels
                if (dto.Levels == null || !dto.Levels.Any())
                {
                    ModelState.AddModelError("", "At least one approval level is required");
                    PrepareViewBags();
                    return View(dto);
                }

                await _approvalService.CreateWorkflowAsync(dto);
                TempData["Success"] = "Approval workflow created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow");
                TempData["Error"] = "Error creating workflow: " + ex.Message;
                PrepareViewBags();
                return View(dto);
            }
        }

        // GET: ApprovalSettings/EditWorkflow/5
        public async Task<IActionResult> EditWorkflow(int id)
        {
            try
            {
                var workflow = await _approvalService.GetWorkflowByIdAsync(id);
                if (workflow == null)
                {
                    TempData["Error"] = "Workflow not found";
                    return RedirectToAction(nameof(Index));
                }

                PrepareViewBags();
                return View(workflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading workflow {id}");
                TempData["Error"] = "Error loading workflow: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ApprovalSettings/EditWorkflow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkflow(int id, ApprovalWorkflowDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    PrepareViewBags();
                    return View(dto);
                }

                // Validate workflow levels
                if (dto.Levels == null || !dto.Levels.Any())
                {
                    ModelState.AddModelError("", "At least one approval level is required");
                    PrepareViewBags();
                    return View(dto);
                }

                await _approvalService.UpdateWorkflowAsync(dto);
                TempData["Success"] = "Approval workflow updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating workflow {id}");
                TempData["Error"] = "Error updating workflow: " + ex.Message;
                PrepareViewBags();
                return View(dto);
            }
        }

        // POST: ApprovalSettings/DeleteWorkflow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorkflow(int id)
        {
            try
            {
                var result = await _approvalService.DeleteWorkflowAsync(id);
                if (result)
                {
                    TempData["Success"] = "Approval workflow deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Workflow not found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting workflow {id}");
                TempData["Error"] = "Error deleting workflow: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ApprovalSettings/ToggleWorkflow/5
        [HttpPost]
        public async Task<IActionResult> ToggleWorkflow(int id)
        {
            try
            {
                var result = await _approvalService.ToggleWorkflowStatusAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Status updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Workflow not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling workflow {id}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: ApprovalSettings/ToggleEntity
        [HttpPost]
        public async Task<IActionResult> ToggleEntity(string entityType, bool isRequired)
        {
            try
            {
                await _approvalService.ToggleEntityApprovalAsync(entityType, isRequired);
                return Json(new { success = true, message = $"Approval {(isRequired ? "enabled" : "disabled")} for {entityType}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling entity {entityType}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void PrepareViewBags()
        {
            ViewBag.EntityTypes = new SelectList(new[]
            {
                "PURCHASE",
                "REQUISITION",
                "ISSUE",
                "TRANSFER",
                "WRITEOFF",
                "STOCK_ADJUSTMENT",
                "PHYSICAL_INVENTORY",
                "ALLOTMENT_LETTER",
                "STOCK_ENTRY"
            });

            ViewBag.Roles = new SelectList(new[]
            {
                "StoreManager",
                "StoreKeeper",
                "UpazilaCommander",
                "ZilaCommander",
                "RangeDIG",
                "DirectorOps",
                "DirectorGeneral",
                "DD Provision",
                "AD/DD Store",
                "DDG Admin"
            });

            ViewBag.ApprovalLevels = new SelectList(Enumerable.Range(1, 5));
        }
    }
}
