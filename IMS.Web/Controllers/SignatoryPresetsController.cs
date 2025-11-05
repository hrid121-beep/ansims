using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class SignatoryPresetsController : Controller
    {
        private readonly ISignatoryPresetService _signatoryPresetService;
        private readonly IUserContext _userContext;
        private readonly ILogger<SignatoryPresetsController> _logger;

        public SignatoryPresetsController(
            ISignatoryPresetService signatoryPresetService,
            IUserContext userContext,
            ILogger<SignatoryPresetsController> logger)
        {
            _signatoryPresetService = signatoryPresetService;
            _userContext = userContext;
            _logger = logger;
        }

        // GET: SignatoryPresets
        public async Task<IActionResult> Index()
        {
            try
            {
                var presets = await _signatoryPresetService.GetAllPresetsAsync();
                return View(presets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading signatory presets");
                TempData["Error"] = "Failed to load signatory presets.";
                return View(new List<SignatoryPreset>());
            }
        }

        // GET: SignatoryPresets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SignatoryPresets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SignatoryPreset preset)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userName = _userContext.GetCurrentUserName();
                    preset.CreatedBy = userName;
                    preset.CreatedAt = DateTime.UtcNow;

                    await _signatoryPresetService.CreatePresetAsync(preset);

                    TempData["Success"] = "Signatory preset created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                return View(preset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating signatory preset");
                TempData["Error"] = "Failed to create preset. " + ex.Message;
                return View(preset);
            }
        }

        // GET: SignatoryPresets/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var preset = await _signatoryPresetService.GetPresetByIdAsync(id);
                if (preset == null)
                {
                    TempData["Error"] = "Preset not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(preset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading preset {Id}", id);
                TempData["Error"] = "Failed to load preset.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: SignatoryPresets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SignatoryPreset preset)
        {
            if (id != preset.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var userName = _userContext.GetCurrentUserName();
                    preset.UpdatedBy = userName;
                    preset.UpdatedAt = DateTime.UtcNow;

                    await _signatoryPresetService.UpdatePresetAsync(preset);

                    TempData["Success"] = "Preset updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                return View(preset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating preset {Id}", id);
                TempData["Error"] = "Failed to update preset. " + ex.Message;
                return View(preset);
            }
        }

        // POST: SignatoryPresets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _signatoryPresetService.DeletePresetAsync(id);

                if (result)
                {
                    TempData["Success"] = "Preset deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Preset not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting preset {Id}", id);
                TempData["Error"] = "Failed to delete preset. " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: SignatoryPresets/SetDefault/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            try
            {
                var result = await _signatoryPresetService.SetDefaultPresetAsync(id);

                if (result)
                {
                    TempData["Success"] = "Default preset updated successfully!";
                }
                else
                {
                    TempData["Error"] = "Preset not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default preset {Id}", id);
                TempData["Error"] = "Failed to set default preset. " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
