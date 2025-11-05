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
    public class UserStoreController : Controller
    {
        private readonly IUserStoreService _userStoreService;
        private readonly IStoreService _storeService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserStoreController> _logger;

        public UserStoreController(
            IUserStoreService userStoreService,
            IStoreService storeService,
            IUserService userService,
            UserManager<User> userManager,
            ILogger<UserStoreController> logger)
        {
            _userStoreService = userStoreService;
            _storeService = storeService;
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string userId = null, int? storeId = null)
        {
            try
            {
                IEnumerable<UserStoreDto> assignments;

                if (!string.IsNullOrEmpty(userId))
                {
                    assignments = await _userStoreService.GetUserStoresByUserAsync(userId);
                    var user = await _userManager.FindByIdAsync(userId);
                    ViewBag.UserName = user?.FullName ?? user?.UserName;
                    ViewBag.UserId = userId;
                }
                else if (storeId.HasValue)
                {
                    assignments = await _userStoreService.GetUserStoresByStoreAsync(storeId.Value);
                    var store = await _storeService.GetStoreByIdAsync(storeId.Value);
                    ViewBag.StoreName = store?.Name;
                    ViewBag.StoreId = storeId.Value;
                }
                else
                {
                    assignments = await _userStoreService.GetActiveAssignmentsAsync();
                }

                return View(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user store assignments");
                TempData["Error"] = "An error occurred while loading assignments.";
                return View(new List<UserStoreDto>());
            }
        }

        public async Task<IActionResult> Assign(string userId = null, int? storeId = null)
        {
            await LoadUserSelectList(userId);
            await LoadStoreSelectList(storeId);

            var model = new UserStoreDto();
            if (!string.IsNullOrEmpty(userId))
                model.UserId = userId;
            if (storeId.HasValue)
                model.StoreId = storeId.Value;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(UserStoreDto userStoreDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    userStoreDto.UserName = User.Identity.Name;
                    var result = await _userStoreService.AssignUserToStoreAsync(userStoreDto);
                    TempData["Success"] = "User assigned to store successfully!";

                    if (!string.IsNullOrEmpty(userStoreDto.UserId))
                        return RedirectToAction(nameof(Index), new { userId = userStoreDto.UserId });
                    else
                        return RedirectToAction(nameof(Index), new { storeId = userStoreDto.StoreId });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning user to store");
                ModelState.AddModelError("", "An error occurred while assigning user to store.");
            }

            await LoadUserSelectList(userStoreDto.UserId);
            await LoadStoreSelectList(userStoreDto.StoreId);
            return View(userStoreDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPrimary(string userId, int? storeId, string returnUrl = null)
        {
            try
            {
                await _userStoreService.SetPrimaryStoreAsync(userId, storeId);
                TempData["Success"] = "Primary store set successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting primary store");
                TempData["Error"] = "An error occurred while setting primary store.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index), new { userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id, string returnUrl = null)
        {
            try
            {
                await _userStoreService.RemoveUserFromStoreAsync(id);
                TempData["Success"] = "User removed from store successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user from store");
                TempData["Error"] = "An error occurred while removing user from store.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BulkAssign(int? storeId)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(storeId);
                if (store == null)
                {
                    TempData["Error"] = "Store not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get users not already assigned to this store
                var allUsers = await _userService.GetAllUsersAsync();
                var storeUsers = await _userStoreService.GetStoreUsersAsync(storeId);
                var availableUsers = allUsers.Where(u => !storeUsers.Any(su => su.Id == u.Id));

                ViewBag.StoreName = store.Name;
                ViewBag.StoreId = storeId;
                ViewBag.AvailableUsers = availableUsers;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bulk assign view");
                TempData["Error"] = "An error occurred while loading users.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkAssign(int? storeId, List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                TempData["Error"] = "Please select at least one user.";
                return RedirectToAction(nameof(BulkAssign), new { storeId });
            }

            try
            {
                await _userStoreService.BulkAssignUsersToStoreAsync(storeId, userIds);
                TempData["Success"] = $"{userIds.Count} users assigned successfully!";
                return RedirectToAction(nameof(Index), new { storeId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk assigning users");
                TempData["Error"] = "An error occurred while assigning users.";
                return RedirectToAction(nameof(BulkAssign), new { storeId });
            }
        }

        public async Task<IActionResult> History(string userId = null, int? storeId = null)
        {
            try
            {
                var history = await _userStoreService.GetAssignmentHistoryAsync(userId, storeId);

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    ViewBag.UserName = user?.FullName ?? user?.UserName;
                    ViewBag.UserId = userId;
                }

                if (storeId.HasValue)
                {
                    var store = await _storeService.GetStoreByIdAsync(storeId.Value);
                    ViewBag.StoreName = store?.Name;
                    ViewBag.StoreId = storeId.Value;
                }

                return View(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assignment history");
                TempData["Error"] = "An error occurred while loading history.";
                return View(new List<UserStoreDto>());
            }
        }

        // AJAX endpoints
        [HttpGet]
        public async Task<IActionResult> CheckAssignment(string userId, int? storeId)
        {
            try
            {
                var isAssigned = await _userStoreService.IsUserAssignedToStoreAsync(userId, storeId);
                return Json(new { isAssigned });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking assignment");
                return Json(new { isAssigned = false, error = "Error checking assignment." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPrimaryStore(string userId)
        {
            try
            {
                var store = await _userStoreService.GetUserPrimaryStoreAsync(userId);
                if (store != null)
                {
                    return Json(new { success = true, storeId = store.Id, storeName = store.Name });
                }
                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user primary store");
                return Json(new { success = false, error = "Error retrieving primary store." });
            }
        }

        private async Task LoadUserSelectList(string selectedUserId = null)
        {
            var users = await _userService.GetAllUsersAsync();
            var userList = users.Select(u => new {
                Id = u.Id,
                Name = $"{u.FullName} ({u.UserName})"
            }).OrderBy(u => u.Name);

            ViewBag.Users = new SelectList(userList, "Id", "Name", selectedUserId);
        }

        private async Task LoadStoreSelectList(int? selectedStoreId = null)
        {
            var stores = await _storeService.GetAllStoresAsync();
            ViewBag.Stores = new SelectList(stores.OrderBy(s => s.Name), "Id", "Name", selectedStoreId);
        }
    }
}