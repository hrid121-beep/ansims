using CsvHelper;
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Application.Services;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Infrastructure.Repositories;
using IMS.Web.Attributes;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Web.Controllers
{
    [Authorize]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IBattalionService _battalionService;
        private readonly IRangeService _rangeService;
        private readonly IItemService _itemService;
        private readonly IZilaService _zilaService;
        private readonly IUpazilaService _upazilaService;
        private readonly UserManager<User> _userManager;
        private readonly IStoreTypeService _storeTypeService;
        private readonly ILogger<StoreController> _logger;

        public StoreController(
            IStoreService storeService,
            IBattalionService battalionService,
            IRangeService rangeService,
            IZilaService zilaService,
            IUpazilaService upazilaService,
            IStoreTypeService storeTypeService,
            UserManager<User> userManager,
            IItemService itemService,
            ILogger<StoreController> logger)
        {
            _storeService = storeService;
            _battalionService = battalionService;
            _rangeService = rangeService;
            _zilaService = zilaService;
            _storeTypeService = storeTypeService;
            _itemService = itemService;
            _upazilaService = upazilaService;
            _userManager = userManager;
            _logger = logger;
        }

        // ==================== MAIN STORE OPERATIONS ====================

        [HasPermission(Permission.ViewStore)]
        public async Task<IActionResult> Index(int? battalionId = null, int? rangeId = null)
        {
            try
            {
                IEnumerable<StoreDto> stores;

                if (battalionId.HasValue)
                {
                    stores = await _storeService.GetStoresByBattalionAsync(battalionId.Value);
                    var battalion = await _battalionService.GetBattalionByIdAsync(battalionId.Value);
                    ViewBag.BattalionId = battalionId;
                    ViewBag.BattalionName = battalion?.Name;
                }
                else if (rangeId.HasValue)
                {
                    stores = await _storeService.GetStoresByRangeAsync(rangeId.Value);
                    var range = await _rangeService.GetRangeByIdAsync(rangeId.Value);
                    ViewBag.RangeId = rangeId;
                    ViewBag.RangeName = range?.Name;
                }
                else
                {
                    stores = await _storeService.GetAllStoresAsync();
                }

                // Load filter options
                ViewBag.Zilas = new SelectList(await _zilaService.GetActiveZilasAsync(), "Id", "Name");

                return View(stores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stores");
                TempData["Error"] = "An error occurred while loading stores.";
                return View(new List<StoreDto>());
            }
        }

        [HasPermission(Permission.ViewStore)]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                // ✅ FIXED: Use GetStoreItemsAsync instead of GetStoreStockAsync
                var storeItems = await _storeService.GetStoreItemsAsync(id);

                // Get item details with prices for calculations
                var storeStock = new List<StoreStockDto>();
                decimal totalValue = 0;

                foreach (var si in storeItems)
                {
                    // Get full item details to get unit price
                    var itemDetails = await _itemService.GetItemByIdAsync(si.ItemId);
                    var unitPrice = itemDetails?.UnitPrice ?? 0;
                    var itemTotalValue = si.Quantity * unitPrice;

                    storeStock.Add(new StoreStockDto
                    {
                        StoreId = si.StoreId,
                        ItemId = si.ItemId,
                        ItemCode = si.ItemCode,
                        ItemName = si.ItemName,
                        CategoryName = si.CategoryName,
                        Quantity = si.Quantity,
                        MinimumStock = si.MinimumStock,
                        MaximumStock = si.MaximumStock,
                        ReorderLevel = si.ReorderLevel,
                        Unit = si.Unit,
                        UnitPrice = unitPrice,
                        TotalValue = itemTotalValue
                    });

                    totalValue += (decimal)itemTotalValue;
                }

                ViewBag.StoreStock = storeStock;

                // ✅ UPDATE: Override store counts with actual values from StoreItems
                store.ItemCount = storeItems.Count();
                ViewBag.TotalValue = totalValue;

                // Load assigned users
                var assignedUsers = await _storeService.GetStoreAssignedUsersAsync(id);
                ViewBag.AssignedUsers = assignedUsers;
                store.AssignedUserCount = assignedUsers.Count();

                // Load recent transactions
                ViewBag.RecentTransactions = await _storeService.GetStoreRecentTransactionsAsync(id);

                // Load inventory summary
                ViewBag.InventorySummary = await _storeService.GetStoreInventorySummaryAsync(id);

                return View(store);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading store details for ID: {StoreId}", id);
                TempData["Error"] = "An error occurred while loading store details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HasPermission(Permission.CreateStore)]
        public async Task<IActionResult> Create(int? battalionId = null, int? rangeId = null)
        {
            try
            {
                var model = new StoreDto();

                // Generate store code
                model.Code = await _storeService.GenerateStoreCodeAsync();

                // Set organization if coming from specific page
                if (battalionId.HasValue)
                {
                    model.BattalionId = battalionId;
                    model.Level = StoreLevel.Battalion;
                }
                else if (rangeId.HasValue)
                {
                    model.RangeId = rangeId;
                    model.Level = StoreLevel.Range;
                }

                await LoadSelectLists(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create store view");
                TempData["Error"] = "An error occurred while loading the page.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateStore)]
        public async Task<IActionResult> Create(StoreDto storeDto, bool redirectToAddItems = false)
        {
            try
            {
                // Validate organization assignment based on level
                if (!ValidateOrganizationAssignment(storeDto))
                {
                    ModelState.AddModelError("", "Please select the required organization assignment based on store level.");
                }

                if (ModelState.IsValid)
                {
                    storeDto.CreatedBy = User.Identity.Name;
                    var createdStore = await _storeService.CreateStoreAsync(storeDto);

                    TempData["Success"] = "Store created successfully!";

                    if (redirectToAddItems)
                    {
                        return RedirectToAction(nameof(AddItemsToStore), new { id = createdStore.Id });
                    }

                    return RedirectToAction(nameof(Details), new { id = createdStore.Id });
                }

                await LoadSelectLists(storeDto);
                return View(storeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store");
                TempData["Error"] = "An error occurred while creating the store.";
                await LoadSelectLists(storeDto);
                return View(storeDto);
            }
        }

        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadSelectLists(store);
                return View(store);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit store view");
                TempData["Error"] = "An error occurred while loading the store.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> Edit(int id, StoreDto storeDto)
        {
            if (id != storeDto.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    storeDto.UpdatedBy = User.Identity.Name;
                    await _storeService.UpdateStoreAsync(storeDto);

                    TempData["Success"] = "Store updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await LoadSelectLists(storeDto);
                return View(storeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store");
                TempData["Error"] = "An error occurred while updating the store.";
                await LoadSelectLists(storeDto);
                return View(storeDto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteStore)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if store has stock or active users
                var storeStock = await _storeService.GetStoreStockAsync(id);
                if (storeStock != null && storeStock.Any(s => s.Quantity > 0))
                {
                    return Json(new { success = false, message = "Cannot delete store with existing stock. Please transfer or remove all stock first." });
                }

                await _storeService.DeleteStoreAsync(id);
                return Json(new { success = true, message = "Store deleted successfully!" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to delete store {StoreId}", id);
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting store {StoreId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the store." });
            }
        }

        // ==================== USER MANAGEMENT ====================

        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> ManageUsers(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                var assignedUsers = await _storeService.GetStoreAssignedUsersAsync(id);
                var allUsers = await _userManager.Users.Where(u => u.IsActive).ToListAsync();

                var assignedUserIds = assignedUsers.Select(u => u.Id).ToList();
                var availableUsers = allUsers.Where(u => !assignedUserIds.Contains(u.Id)).ToList();

                var model = new StoreUsersViewModel
                {
                    StoreId = store.Id,
                    StoreName = store.Name,
                    StoreCode = store.Code,
                    PrimaryKeeper = store.ManagerName,
                    AssignedUsers = assignedUsers.Select(u => new AssignedUserDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        IsPrimary = u.Id == store.ManagerId,
                        AssignedDate = DateTime.Now
                    }).ToList(),
                    AvailableUsers = availableUsers.Select(u => new AvailableUserDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        FullName = u.FullName,
                        Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault()
                    }).ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading manage users view");
                TempData["Error"] = "An error occurred while loading users.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> AddUsersToStore(int storeId, List<string> userIds, bool setPrimary = false)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    await _storeService.AddUserToStoreAsync(storeId, userId);
                }

                if (setPrimary && userIds.Any())
                {
                    var store = await _storeService.GetStoreByIdAsync(storeId);
                    store.ManagerId = userIds.First();
                    await _storeService.UpdateStoreAsync(store);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding users to store");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> RemoveUserFromStore(int storeId, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || storeId <= 0)
                {
                    return Json(new { success = false, message = "Invalid parameters." });
                }

                // Check if user is the primary store keeper
                var store = await _storeService.GetStoreByIdAsync(storeId);
                if (store != null && store.ManagerId == userId)
                {
                    store.ManagerId = null;
                    store.UpdatedBy = User.Identity.Name;
                    await _storeService.UpdateStoreAsync(store);
                }

                // Remove user from store assignment
                await _storeService.RemoveUserFromStoreAsync(userId, storeId);

                return Json(new { success = true, message = "User removed successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserId} from store {StoreId}", userId, storeId);
                return Json(new { success = false, message = "Failed to remove user from store." });
            }
        }

        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> UserAssignment(int? storeId = null)
        {
            try
            {
                ViewBag.Stores = await _storeService.GetActiveStoresAsync();

                var model = new StoreUserAssignmentViewModel();

                if (storeId.HasValue)
                {
                    var store = await _storeService.GetStoreByIdAsync(storeId.Value);
                    model.StoreId = storeId.Value;
                    model.StoreName = store?.Name;
                    model.AssignedUsers = await _storeService.GetStoreAssignedUsersAsync(storeId.Value);
                }

                // Get all users for assignment dropdown
                var allUsers = await _userManager.Users.Where(u => u.IsActive).ToListAsync();
                model.AvailableUsers = allUsers.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.FullName} ({u.UserName})"
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user assignment view");
                TempData["Error"] = "An error occurred while loading user assignments.";
                return View(new StoreUserAssignmentViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> AssignUsers(int storeId, List<string> userIds)
        {
            try
            {
                // Validate input
                if (userIds == null || !userIds.Any())
                {
                    TempData["Warning"] = "No users were selected to assign.";
                    return RedirectToAction(nameof(UserAssignment), new { storeId });
                }

                // Validate store exists
                var store = await _storeService.GetStoreByIdAsync(storeId);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(UserAssignment));
                }

                // Use batch operation instead of loop
                var (successCount, errors) = await _storeService.AddUsersToStoreBatchAsync(storeId, userIds);

                // Get user names for better error messages
                var errorMessages = new List<string>();
                foreach (var error in errors)
                {
                    errorMessages.Add(error);
                }

                // Set appropriate messages
                if (successCount > 0 && errors.Count == 0)
                {
                    TempData["Success"] = $"Successfully assigned {successCount} user(s) to the store!";
                }
                else if (successCount > 0 && errors.Count > 0)
                {
                    TempData["Warning"] = $"Assigned {successCount} user(s) successfully. {errors.Count} failed: {string.Join("; ", errorMessages)}";
                }
                else if (successCount == 0 && errors.Count > 0)
                {
                    TempData["Error"] = $"Failed to assign users: {string.Join("; ", errorMessages)}";
                }
                else
                {
                    TempData["Info"] = "All selected users are already assigned to this store.";
                }

                return RedirectToAction(nameof(UserAssignment), new { storeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AssignUsers for store {StoreId}", storeId);
                TempData["Error"] = "An unexpected error occurred while assigning users.";
                return RedirectToAction(nameof(UserAssignment), new { storeId });
            }
        }

        // ==================== STOCK MANAGEMENT ====================

        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> StockLevels(int? storeId = null)
        {
            try
            {
                var stockLevels = await _storeService.GetStoreStockLevelsAsync(storeId);

                ViewBag.Stores = await _storeService.GetActiveStoresAsync();
                ViewBag.CurrentStoreId = storeId;

                if (storeId.HasValue)
                {
                    var store = await _storeService.GetStoreByIdAsync(storeId.Value);
                    ViewBag.StoreName = store?.Name;
                }

                return View(stockLevels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock levels");
                TempData["Error"] = "An error occurred while loading stock levels.";
                return View(new List<StockLevelDto>());
            }
        }

        [HttpGet]
        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> AddItemsToStore(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null) return NotFound();

            ViewBag.Store = store;
            ViewBag.Items = new SelectList(await _itemService.GetAllItemsAsync(), "Id", "Name");

            return View(new AddItemsToStoreViewModel { StoreId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> AddItemsToStore(AddItemsToStoreViewModel model)
        {
            try
            {
                _logger.LogInformation(
                    "AddItemsToStore POST called: StoreId={StoreId}, SelectedItems={ItemCount}, MinStock={MinStock}, MaxStock={MaxStock}, ReorderLevel={ReorderLevel}",
                    model.StoreId, model.SelectedItemIds?.Count ?? 0, model.MinStock, model.MaxStock, model.ReorderLevel);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid for AddItemsToStore");

                    ViewBag.Store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    ViewBag.Items = new SelectList(await _itemService.GetAllItemsAsync(), "Id", "Name");

                    TempData["Error"] = "Please correct the validation errors and try again.";
                    return View(model);
                }

                // Validate that items are selected
                if (model.SelectedItemIds == null || !model.SelectedItemIds.Any())
                {
                    _logger.LogWarning("No items selected for StoreId={StoreId}", model.StoreId);

                    ViewBag.Store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    ViewBag.Items = new SelectList(await _itemService.GetAllItemsAsync(), "Id", "Name");

                    TempData["Error"] = "Please select at least one item to add to the store.";
                    return View(model);
                }

                // Validate stock levels
                if (model.MinStock > model.MaxStock)
                {
                    _logger.LogWarning(
                        "Invalid stock levels: MinStock={MinStock} > MaxStock={MaxStock}",
                        model.MinStock, model.MaxStock);

                    ViewBag.Store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    ViewBag.Items = new SelectList(await _itemService.GetAllItemsAsync(), "Id", "Name");

                    TempData["Error"] = "Minimum stock cannot be greater than maximum stock.";
                    return View(model);
                }

                if (model.ReorderLevel < model.MinStock || model.ReorderLevel > model.MaxStock)
                {
                    _logger.LogWarning(
                        "Invalid reorder level: ReorderLevel={ReorderLevel} not between MinStock={MinStock} and MaxStock={MaxStock}",
                        model.ReorderLevel, model.MinStock, model.MaxStock);

                    ViewBag.Store = await _storeService.GetStoreByIdAsync(model.StoreId);
                    ViewBag.Items = new SelectList(await _itemService.GetAllItemsAsync(), "Id", "Name");

                    TempData["Error"] = "Reorder level must be between minimum and maximum stock levels.";
                    return View(model);
                }

                // Get store for logging
                var store = await _storeService.GetStoreByIdAsync(model.StoreId);
                if (store == null)
                {
                    _logger.LogError("Store not found: StoreId={StoreId}", model.StoreId);
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                int successCount = 0;
                int failCount = 0;
                List<string> errorMessages = new List<string>();

                // Add each selected item to the store
                foreach (var itemId in model.SelectedItemIds)
                {
                    try
                    {
                        _logger.LogInformation(
                            "Adding ItemId={ItemId} to StoreId={StoreId}",
                            itemId, model.StoreId);

                        await _storeService.AddItemToStoreAsync(
                            model.StoreId,
                            itemId,
                            model.MinStock,
                            model.MaxStock,
                            model.ReorderLevel);

                        successCount++;

                        _logger.LogInformation(
                            "Successfully added ItemId={ItemId} to StoreId={StoreId}",
                            itemId, model.StoreId);
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        var item = await _itemService.GetItemByIdAsync(itemId);
                        var itemName = item?.Name ?? $"Item {itemId}";

                        _logger.LogError(ex,
                            "Failed to add ItemId={ItemId} to StoreId={StoreId}: {ErrorMessage}",
                            itemId, model.StoreId, ex.Message);

                        errorMessages.Add($"{itemName}: {ex.Message}");
                    }
                }

                // Set appropriate success/error messages
                if (successCount > 0 && failCount == 0)
                {
                    TempData["Success"] = $"Successfully added {successCount} item(s) to {store.Name}!";
                    _logger.LogInformation(
                        "Successfully added {SuccessCount} items to Store {StoreId}",
                        successCount, model.StoreId);
                }
                else if (successCount > 0 && failCount > 0)
                {
                    TempData["Warning"] = $"Added {successCount} item(s) successfully, but {failCount} failed. Errors: {string.Join("; ", errorMessages)}";
                    _logger.LogWarning(
                        "Partially added items to Store {StoreId}: Success={SuccessCount}, Failed={FailCount}",
                        model.StoreId, successCount, failCount);
                }
                else
                {
                    TempData["Error"] = $"Failed to add items. Errors: {string.Join("; ", errorMessages)}";
                    _logger.LogError(
                        "Failed to add all items to Store {StoreId}",
                        model.StoreId);
                }

                return RedirectToAction(nameof(Details), new { id = model.StoreId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddItemsToStore POST action");
                TempData["Error"] = $"An error occurred while adding items to the store: {ex.Message}";

                ViewBag.Store = await _storeService.GetStoreByIdAsync(model.StoreId);
                ViewBag.Items = new SelectList(await _itemService.GetAllItemsAsync(), "Id", "Name");

                return View(model);
            }
        }

        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> UpdateStockLevels(int storeId, int itemId)
        {
            try
            {
                var storeStock = await _storeService.GetStoreStockItemAsync(storeId, itemId);
                if (storeStock == null)
                {
                    TempData["Error"] = "Stock item not found.";
                    return RedirectToAction(nameof(Details), new { id = storeId });
                }

                ViewBag.Store = await _storeService.GetStoreByIdAsync(storeId);
                ViewBag.Item = await _itemService.GetItemByIdAsync(itemId);

                return View(storeStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock levels for update");
                TempData["Error"] = "Failed to load stock levels.";
                return RedirectToAction(nameof(Details), new { id = storeId });
            }
        }

        // ==================== BULK OPERATIONS ====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateStore)]
        public async Task<IActionResult> BulkUpdateStatus(List<int> ids, bool isActive)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "No stores selected." });
                }

                int updatedCount = 0;
                foreach (var id in ids)
                {
                    var store = await _storeService.GetStoreByIdAsync(id);
                    if (store != null)
                    {
                        store.IsActive = isActive;
                        store.UpdatedBy = User.Identity.Name;
                        await _storeService.UpdateStoreAsync(store);
                        updatedCount++;
                    }
                }

                return Json(new
                {
                    success = true,
                    count = updatedCount,
                    message = $"Successfully updated {updatedCount} stores."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store status in bulk");
                return Json(new { success = false, message = "An error occurred while updating stores." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteStore)]
        public async Task<IActionResult> BulkDelete(List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "No stores selected." });
                }

                int deletedCount = 0;
                var errors = new List<string>();

                foreach (var id in ids)
                {
                    try
                    {
                        // Check if store has stock
                        var storeStock = await _storeService.GetStoreStockAsync(id);
                        if (storeStock != null && storeStock.Any(s => s.Quantity > 0))
                        {
                            var store = await _storeService.GetStoreByIdAsync(id);
                            errors.Add($"Store '{store?.Name}' has existing stock");
                            continue;
                        }

                        await _storeService.DeleteStoreAsync(id);
                        deletedCount++;
                    }
                    catch (Exception)
                    {
                        errors.Add($"Failed to delete store ID: {id}");
                    }
                }

                if (errors.Any())
                {
                    return Json(new
                    {
                        success = false,
                        count = deletedCount,
                        message = $"Deleted {deletedCount} stores. Errors: {string.Join(", ", errors)}"
                    });
                }

                return Json(new
                {
                    success = true,
                    count = deletedCount,
                    message = $"Successfully deleted {deletedCount} stores."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stores in bulk");
                return Json(new { success = false, message = "An error occurred while deleting stores." });
            }
        }

        // ==================== EXPORT OPERATIONS ====================

        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> ExportStoreStock(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                var storeStock = await _storeService.GetStoreStockAsync(id);

                // Generate CSV content
                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Write headers
                    csv.WriteField("Item Code");
                    csv.WriteField("Item Name");
                    csv.WriteField("Category");
                    csv.WriteField("Current Stock");
                    csv.WriteField("Min Stock");
                    csv.WriteField("Max Stock");
                    csv.WriteField("Reorder Level");
                    csv.WriteField("Unit");
                    csv.WriteField("Unit Price");
                    csv.WriteField("Total Value");
                    csv.WriteField("Status");
                    csv.NextRecord();

                    // Write data
                    foreach (var item in storeStock)
                    {
                        csv.WriteField(item.ItemCode);
                        csv.WriteField(item.ItemName);
                        csv.WriteField(item.CategoryName);
                        csv.WriteField(item.Quantity);
                        csv.WriteField(item.MinimumStock);
                        csv.WriteField(item.MaximumStock);
                        csv.WriteField(item.ReorderLevel);
                        csv.WriteField(item.Unit);
                        csv.WriteField(item.UnitPrice);
                        csv.WriteField(item.TotalValue);
                        csv.WriteField(item.Quantity == 0 ? "Out of Stock" :
                                      item.Quantity <= item.MinimumStock ? "Low Stock" : "In Stock");
                        csv.NextRecord();
                    }

                    writer.Flush();
                    var result = memoryStream.ToArray();

                    return File(result, "text/csv", $"{store.Code}_Stock_{DateTime.Now:yyyyMMdd}.csv");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting store stock for store {StoreId}", id);
                TempData["Error"] = "Failed to export store stock.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> ExportStockLevels(int? storeId = null, string format = "excel")
        {
            try
            {
                var stockLevels = await _storeService.GetStoreStockLevelsAsync(storeId);

                if (format.ToLower() == "excel")
                {
                    // Generate Excel file
                    var excelData = GenerateStockLevelsExcel(stockLevels);
                    return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                               $"StockLevels_{DateTime.Now:yyyyMMdd}.xlsx");
                }

                // Default to CSV
                var csvData = GenerateStockLevelsCsv(stockLevels);
                return File(csvData, "text/csv", $"StockLevels_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stock levels");
                TempData["Error"] = "Failed to export stock levels.";
                return RedirectToAction(nameof(StockLevels));
            }
        }

        // ==================== AJAX ENDPOINTS ====================

        [HttpGet]
        public async Task<IActionResult> GenerateStoreCode()
        {
            try
            {
                var code = await _storeService.GenerateStoreCodeAsync();
                return Json(code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating store code");
                return Json("");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBattalionsByRange(int rangeId)
        {
            try
            {
                var battalions = await _battalionService.GetBattalionsByRangeAsync(rangeId);
                return Json(battalions.Select(b => new { value = b.Id, text = b.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting battalions by range");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUpazilasByZila(int zilaId)
        {
            try
            {
                var upazilas = await _upazilaService.GetUpazilasByZilaAsync(zilaId);
                return Json(upazilas.Select(u => new { value = u.Id, text = u.Name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upazilas by zila");
                return Json(new List<object>());
            }
        }

        [HttpGet]
        [HasPermission(Permission.ViewStock)]
        public async Task<IActionResult> GetStoreDistribution(int itemId)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(itemId);
                if (item == null)
                {
                    return PartialView("_StoreDistributionPartial", null);
                }

                // Get all stores that have this item
                var storeStocks = await _storeService.GetItemDistributionAcrossStoresAsync(itemId);

                var model = new StoreDistributionViewModel
                {
                    ItemId = itemId,
                    ItemName = item.Name,
                    ItemCode = item.Code,
                    StoreDistributions = storeStocks.Select(s => new StoreDistributionDto
                    {
                        StoreId = s.StoreId,
                        StoreName = s.StoreName,
                        StoreCode = s.StoreCode,
                        CurrentStock = s.Quantity,
                        MinimumStock = s.MinimumStock,
                        MaximumStock = s.MaximumStock,
                        Status = s.Quantity == 0 ? "Out of Stock" :
                                s.Quantity <= s.MinimumStock ? "Low Stock" : "Good"
                    }).ToList()
                };

                return PartialView("_StoreDistributionPartial", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store distribution for item {ItemId}", itemId);
                return PartialView("_StoreDistributionPartial", null);
            }
        }

        // ==================== PRIVATE HELPER METHODS ====================

        private async Task LoadSelectLists(StoreDto model = null)
        {
            // Load Ranges
            ViewBag.Ranges = new SelectList(
                await _rangeService.GetActiveRangesAsync(),
                "Id",
                "Name",
                model?.RangeId
            );

            // Load Battalions - if Range is selected, load only battalions for that range
            if (model?.RangeId.HasValue == true)
            {
                ViewBag.Battalions = new SelectList(
                    await _battalionService.GetBattalionsByRangeAsync(model.RangeId.Value),
                    "Id",
                    "Name",
                    model?.BattalionId
                );
            }
            else
            {
                ViewBag.Battalions = new SelectList(
                    await _battalionService.GetActiveBattalionsAsync(),
                    "Id",
                    "Name",
                    model?.BattalionId
                );
            }

            // Load Zilas
            ViewBag.Zilas = new SelectList(
                await _zilaService.GetActiveZilasAsync(),
                "Id",
                "Name",
                model?.ZilaId
            );

            // Load Upazilas - if Zila is selected, load only upazilas for that zila
            if (model?.ZilaId.HasValue == true)
            {
                ViewBag.Upazilas = new SelectList(
                    await _upazilaService.GetUpazilasByZilaAsync(model.ZilaId.Value),
                    "Id",
                    "Name",
                    model?.UpazilaId
                );
            }
            else
            {
                ViewBag.Upazilas = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            // Load Active Users for Manager selection
            var activeUsers = _userManager.Users.Where(u => u.IsActive).ToList();
            ViewBag.Users = new SelectList(
                activeUsers,
                "Id",
                "FullName",
                model?.ManagerId
            );

            // Load Store Types
            var storeTypes = await _storeTypeService.GetAllStoreTypesAsync();
            ViewBag.StoreTypes = new SelectList(
                storeTypes,
                "Id",
                "Name",
                model?.StoreTypeId
            );

            // Store Levels enum
            ViewBag.StoreLevels = new SelectList(
                Enum.GetValues(typeof(StoreLevel))
                    .Cast<StoreLevel>()
                    .Select(l => new {
                        Value = l.ToString(),
                        Text = l.ToString().Replace("_", " ")
                    }),
                "Value",
                "Text",
                model?.Level.ToString()
            );
        }

        private bool ValidateOrganizationAssignment(StoreDto store)
        {
            switch (store.Level)
            {
                case StoreLevel.Range:
                    return store.RangeId.HasValue;
                case StoreLevel.Battalion:
                    return store.BattalionId.HasValue;
                case StoreLevel.Zila:
                    return store.ZilaId.HasValue;
                case StoreLevel.Upazila:
                    return store.UpazilaId.HasValue && store.ZilaId.HasValue;
                case StoreLevel.HQ:
                default:
                    return true;
            }
        }

        private byte[] GenerateStockLevelsExcel(IEnumerable<StockLevelDto> stockLevels)
        {
            // For now, return CSV as Excel functionality requires additional packages
            // You can implement Excel generation using EPPlus or similar library later
            return GenerateStockLevelsCsv(stockLevels);
        }

        private byte[] GenerateStockLevelsCsv(IEnumerable<StockLevelDto> stockLevels)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteField("Item Code");
                csv.WriteField("Item Name");
                csv.WriteField("Category");
                csv.WriteField("Current Stock");
                csv.WriteField("Minimum Stock");
                csv.WriteField("Reorder Level");
                csv.WriteField("Unit");
                csv.WriteField("Status");
                csv.NextRecord();

                foreach (var item in stockLevels)
                {
                    csv.WriteField(item.ItemCode);
                    csv.WriteField(item.ItemName);
                    csv.WriteField(item.CategoryName);
                    csv.WriteField(item.CurrentStock);
                    csv.WriteField(item.MinimumStock);
                    csv.WriteField(item.ReorderLevel);
                    csv.WriteField(item.Unit);
                    csv.WriteField(item.StockStatus);
                    csv.NextRecord();
                }

                writer.Flush();
                return memoryStream.ToArray();
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchStoreItems(int storeId, string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // If no search term, return all items in the store
                    return await GetStoreItems(storeId);
                }

                // Get all items that have stock in this store
                var storeStocks = await _storeService.GetStoreStockAsync(storeId);

                if (storeStocks == null || !storeStocks.Any())
                {
                    return Json(new List<object>());
                }

                // Filter by search term (search in both item code and item name)
                searchTerm = searchTerm.ToLower();
                var filteredItems = storeStocks.Where(s =>
                    (s.ItemCode != null && s.ItemCode.ToLower().Contains(searchTerm)) ||
                    (s.ItemName != null && s.ItemName.ToLower().Contains(searchTerm))
                );

                // Map to the format expected by the view
                var items = filteredItems.Select(s => new
                {
                    itemId = s.ItemId,
                    itemCode = s.ItemCode,
                    code = s.ItemCode,
                    itemName = s.ItemName,
                    name = s.ItemName,
                    categoryName = s.CategoryName,
                    quantity = s.Quantity,
                    currentStock = s.Quantity,
                    minimumStock = s.MinimumStock,
                    unit = s.Unit,
                    storeId = s.StoreId,
                    isLowStock = s.Quantity <= s.MinimumStock && s.Quantity > 0,
                    isOutOfStock = s.Quantity <= 0
                }).ToList();

                return Json(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching store items for store {StoreId} with term {SearchTerm}",
                    storeId, searchTerm);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreItems(int storeId)
        {
            try
            {
                // First try to get from StoreStock
                var storeStocks = await _storeService.GetStoreStockAsync(storeId);

                // If no stocks found, try StoreItems
                if (storeStocks == null || !storeStocks.Any())
                {
                    // Get from StoreItems table instead
                    var storeItems = await _storeService.GetStoreItemsAsync(storeId);

                    if (storeItems != null && storeItems.Any())
                    {
                        var items = storeItems.Select(si => new
                        {
                            itemId = si.ItemId,
                            itemCode = si.ItemCode,
                            code = si.ItemCode,
                            itemName = si.ItemName,
                            name = si.ItemName,
                            categoryName = si.CategoryName ?? "N/A",
                            quantity = si.Quantity,
                            currentStock = si.Quantity,
                            minimumStock = si.MinimumStock,
                            unit = si.Unit ?? "Piece",
                            storeId = storeId,
                            isLowStock = si.Quantity <= si.MinimumStock && si.Quantity > 0,
                            isOutOfStock = si.Quantity <= 0
                        }).ToList();

                        return Json(items);
                    }

                    return Json(new List<object>());
                }

                // Map StoreStock to expected format
                var stockItems = storeStocks.Select(s => new
                {
                    itemId = s.ItemId,
                    itemCode = s.ItemCode,
                    code = s.ItemCode,
                    itemName = s.ItemName,
                    name = s.ItemName,
                    categoryName = s.CategoryName ?? "N/A",
                    quantity = s.Quantity,
                    currentStock = s.Quantity,
                    minimumStock = s.MinimumStock,
                    unit = s.Unit ?? "Piece",
                    storeId = s.StoreId,
                    isLowStock = s.Quantity <= s.MinimumStock && s.Quantity > 0,
                    isOutOfStock = s.Quantity <= 0
                }).ToList();

                return Json(stockItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store items for store {StoreId}", storeId);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItemsWithStoreStock(int storeId)
        {
            try
            {
                // Get ALL items from the Items table
                var allItems = await _itemService.GetAllItemsAsync();

                if (allItems == null || !allItems.Any())
                {
                    return Json(new List<object>());
                }

                // Get store stocks for this specific store
                var storeStocks = await _storeService.GetStoreStockAsync(storeId);
                var storeStockDict = storeStocks?.ToDictionary(s => s.ItemId, s => s) ?? new Dictionary<int, StoreStockDto>();

                // Also check StoreItems table
                var storeItems = await _storeService.GetStoreItemsAsync(storeId);
                var storeItemDict = storeItems?.ToDictionary(si => si.ItemId, si => si) ?? new Dictionary<int, StoreItemDto>();

                // Map all items, showing which ones are in the selected store
                var items = allItems.Select(item =>
                {
                    // Check both StoreStock and StoreItems
                    var hasInStock = storeStockDict.TryGetValue(item.Id, out var storeStock);
                    var hasInStoreItems = storeItemDict.TryGetValue(item.Id, out var storeItem);

                    // Use whichever has the item
                    decimal? quantity = 0;
                    if (hasInStock && storeStock != null)
                    {
                        quantity = storeStock.Quantity;
                    }
                    else if (hasInStoreItems && storeItem != null)
                    {
                        quantity = storeItem.Quantity;
                    }

                    return new
                    {
                        itemId = item.Id,
                        itemCode = item.ItemCode ?? item.Code ?? "N/A",
                        code = item.Code ?? item.ItemCode ?? "N/A",
                        itemName = item.Name ?? "Unknown",
                        name = item.Name ?? "Unknown",
                        categoryName = item.CategoryName ?? "Uncategorized",
                        quantity = quantity,
                        currentStock = quantity,
                        minimumStock = item.MinimumStock ?? 0,
                        unit = item.Unit ?? "Piece",
                        storeId = storeId,
                        hasStock = hasInStock || hasInStoreItems,
                        isLowStock = quantity > 0 && quantity <= (item.MinimumStock ?? 0),
                        isOutOfStock = quantity <= 0,
                        isAvailable = quantity > 0
                    };
                })
                .OrderByDescending(i => i.quantity > 0) // Show items with stock first
                .ThenBy(i => i.itemCode)
                .ToList();

                return Json(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all items with store stock for store {StoreId}", storeId);
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchItems(int storeId, string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllItemsWithStoreStock(storeId);
                }

                // Get all items and filter by search term
                var allItems = await _itemService.GetAllItemsAsync();

                searchTerm = searchTerm.ToLower().Trim();

                // Filter items by code or name
                var filteredItems = allItems.Where(item =>
                    (item.Code != null && item.Code.ToLower().Contains(searchTerm)) ||
                    (item.ItemCode != null && item.ItemCode.ToLower().Contains(searchTerm)) ||
                    (item.Name != null && item.Name.ToLower().Contains(searchTerm))
                ).ToList();

                // Get store stocks
                var storeStocks = await _storeService.GetStoreStockAsync(storeId);
                var storeStockDict = storeStocks?.ToDictionary(s => s.ItemId, s => s) ?? new Dictionary<int, StoreStockDto>();

                // Get store items
                var storeItems = await _storeService.GetStoreItemsAsync(storeId);
                var storeItemDict = storeItems?.ToDictionary(si => si.ItemId, si => si) ?? new Dictionary<int, StoreItemDto>();

                // Map filtered items
                var items = filteredItems.Select(item =>
                {
                    var hasInStock = storeStockDict.TryGetValue(item.Id, out var storeStock);
                    var hasInStoreItems = storeItemDict.TryGetValue(item.Id, out var storeItem);

                    decimal? quantity = 0;
                    if (hasInStock && storeStock != null)
                    {
                        quantity = storeStock.Quantity;
                    }
                    else if (hasInStoreItems && storeItem != null)
                    {
                        quantity = storeItem.Quantity;
                    }

                    return new
                    {
                        itemId = item.Id,
                        itemCode = item.ItemCode ?? item.Code ?? "N/A",
                        code = item.Code ?? item.ItemCode ?? "N/A",
                        itemName = item.Name ?? "Unknown",
                        name = item.Name ?? "Unknown",
                        categoryName = item.CategoryName ?? "Uncategorized",
                        quantity = quantity,
                        currentStock = quantity,
                        minimumStock = item.MinimumStock ?? 0,
                        unit = item.Unit ?? "Piece",
                        storeId = storeId,
                        hasStock = quantity > 0,
                        isLowStock = quantity > 0 && quantity <= (item.MinimumStock ?? 0),
                        isOutOfStock = quantity <= 0,
                        isAvailable = quantity > 0
                    };
                }).ToList();

                return Json(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching items for store {StoreId}", storeId);
                return Json(new List<object>());
            }
        }
    }
}