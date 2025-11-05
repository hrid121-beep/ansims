using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StoreConfigurationController : Controller
    {
        private readonly IStoreConfigurationService _configService;
        private readonly IStoreService _storeService;
        private readonly ILogger<StoreConfigurationController> _logger;

        public StoreConfigurationController(
            IStoreConfigurationService configService,
            IStoreService storeService,
            ILogger<StoreConfigurationController> logger)
        {
            _configService = configService;
            _storeService = storeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? storeId)
        {
            try
            {
                IEnumerable<StoreConfigurationDto> configurations;

                if (storeId.HasValue)
                {
                    configurations = await _configService.GetStoreConfigurationsAsync(storeId.Value);
                    var store = await _storeService.GetStoreByIdAsync(storeId.Value);
                    ViewBag.StoreName = store?.Name;
                    ViewBag.StoreId = storeId.Value;
                }
                else
                {
                    configurations = await _configService.GetAllConfigurationsAsync();
                }

                await LoadStoreSelectList(storeId);
                return View(configurations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading store configurations");
                TempData["Error"] = "An error occurred while loading configurations.";
                return View(new List<StoreConfigurationDto>());
            }
        }

        public async Task<IActionResult> Create(int? storeId)
        {
            await LoadStoreSelectList(storeId);
            await LoadConfigKeySelectList();

            var model = new StoreConfigurationDto();
            if (storeId.HasValue)
            {
                model.StoreId = storeId.Value;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StoreConfigurationDto configDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    configDto.CreatedBy = User.Identity.Name;
                    await _configService.CreateConfigurationAsync(configDto);
                    TempData["Success"] = "Configuration created successfully!";
                    return RedirectToAction(nameof(Index), new { storeId = configDto.StoreId });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating configuration");
                ModelState.AddModelError("", "An error occurred while creating the configuration.");
            }

            await LoadStoreSelectList(configDto.StoreId);
            await LoadConfigKeySelectList(configDto.ConfigKey);
            return View(configDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var config = await _configService.GetConfigurationByIdAsync(id);
                if (config == null)
                {
                    TempData["Error"] = "Configuration not found.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadStoreSelectList(config.StoreId);
                return View(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading configuration for edit");
                TempData["Error"] = "An error occurred while loading the configuration.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StoreConfigurationDto configDto)
        {
            if (id != configDto.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    configDto.CreatedBy = User.Identity.Name; // Used as UpdatedBy
                    await _configService.UpdateConfigurationAsync(configDto);
                    TempData["Success"] = "Configuration updated successfully!";
                    return RedirectToAction(nameof(Index), new { storeId = configDto.StoreId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating configuration");
                ModelState.AddModelError("", "An error occurred while updating the configuration.");
            }

            await LoadStoreSelectList(configDto.StoreId);
            return View(configDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int? returnStoreId)
        {
            try
            {
                await _configService.DeleteConfigurationAsync(id);
                TempData["Success"] = "Configuration deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting configuration");
                TempData["Error"] = "An error occurred while deleting the configuration.";
            }

            return RedirectToAction(nameof(Index), new { storeId = returnStoreId });
        }

        public async Task<IActionResult> BulkEdit(int? storeId)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(storeId);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                var configs = await _configService.GetAllStoreConfigsAsync(storeId);
                var availableKeys = await _configService.GetAvailableConfigKeysAsync();

                ViewBag.StoreName = store.Name;
                ViewBag.StoreId = storeId;
                ViewBag.AvailableKeys = availableKeys;

                return View(configs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bulk edit view");
                TempData["Error"] = "An error occurred while loading configurations.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkEdit(int? storeId, Dictionary<string, string> configs)
        {
            try
            {
                await _configService.BulkUpdateConfigurationsAsync(storeId, configs);
                TempData["Success"] = "Configurations updated successfully!";
                return RedirectToAction(nameof(Index), new { storeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating configurations");
                TempData["Error"] = "An error occurred while updating configurations.";
                return RedirectToAction(nameof(BulkEdit), new { storeId });
            }
        }

        public async Task<IActionResult> Copy()
        {
            await LoadStoreSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Copy(int sourceStoreId, int targetStoreId)
        {
            if (sourceStoreId == targetStoreId)
            {
                ModelState.AddModelError("", "Source and target stores cannot be the same.");
                await LoadStoreSelectList();
                return View();
            }

            try
            {
                await _configService.CopyConfigurationsAsync(sourceStoreId, targetStoreId);
                TempData["Success"] = "Configurations copied successfully!";
                return RedirectToAction(nameof(Index), new { storeId = targetStoreId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying configurations");
                ModelState.AddModelError("", "An error occurred while copying configurations.");
                await LoadStoreSelectList();
                return View();
            }
        }

        // AJAX endpoint
        [HttpGet]
        public async Task<IActionResult> GetConfigValue(int? storeId, string configKey)
        {
            try
            {
                var value = await _configService.GetConfigValueAsync(storeId, configKey);
                return Json(new { success = true, value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting config value");
                return Json(new { success = false, error = "Error retrieving configuration value." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreConfigs(int storeId)
        {
            try
            {
                var configs = await _configService.GetStoreConfigurationsAsync(storeId);
                return Json(configs.Select(c => new
                {
                    configKey = c.ConfigKey,
                    configValue = c.ConfigValue
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store configs");
                return Json(new List<object>());
            }
        }

        private async Task LoadStoreSelectList(int? selectedStoreId = null)
        {
            var stores = await _storeService.GetAllStoresAsync();
            ViewBag.Stores = new SelectList(stores.OrderBy(s => s.Name), "Id", "Name", selectedStoreId);
        }

        private async Task LoadConfigKeySelectList(string selectedKey = null)
        {
            var keys = await _configService.GetAvailableConfigKeysAsync();
            ViewBag.ConfigKeys = new SelectList(keys, selectedKey);
        }
    }
}