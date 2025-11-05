
// AdminController.cs
using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using IMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IItemService _itemService;

        public AdminController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IItemService itemService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _itemService = itemService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SettingsDashboard()
        {
            return View();
        }

        [HasPermission(Permission.ViewUser)]
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,  // NEW
                    Email = user.Email,
                    FullName = user.FullName,
                    Designation = user.Designation,
                    Department = user.Department,  // NEW
                    PhoneNumber = user.PhoneNumber,  // NEW
                    Roles = roles.ToList(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt  // NEW
                });
            }

            return View(userViewModels);
        }

        [HasPermission(Permission.CreateUser)]
        public async Task<IActionResult> CreateUser()
        {
            await LoadRoles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateUser)]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check username exists
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Username already exists.");
                    await LoadRoles();
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.UserName,  // CHANGED - now separate
                    Email = model.Email,  // CHANGED - can be null/optional
                    FullName = model.FullName,
                    Designation = model.Designation,
                    Department = model.Department,  // NEW
                    BadgeNumber = model.BadgeNumber,  // NEW
                    PhoneNumber = model.PhoneNumber,  // NEW
                    EmailConfirmed = true,
                    IsActive = model.IsActive  // NEW
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    TempData["Success"] = $"User '{model.UserName}' created successfully!";
                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            await LoadRoles();
            return View(model);
        }

        // REPLACE existing EditUser GET method:
        [HasPermission(Permission.EditUser)]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,  // NEW
                Email = user.Email,
                FullName = user.FullName,
                Designation = user.Designation,
                Department = user.Department,  // NEW
                BadgeNumber = user.BadgeNumber,  // NEW
                PhoneNumber = user.PhoneNumber,  // NEW
                CurrentRole = roles.FirstOrDefault(),
                IsActive = user.IsActive  // NEW
            };

            await LoadRoles();
            return View(model);
        }

        // REPLACE existing EditUser POST method:
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.EditUser)]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Users));
                }

                // Check if username changed and exists
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("UserName", "Username already exists.");
                    await LoadRoles();
                    return View(model);
                }

                user.UserName = model.UserName;  // NEW
                user.Email = model.Email;
                user.FullName = model.FullName;
                user.Designation = model.Designation;
                user.Department = model.Department;  // NEW
                user.BadgeNumber = model.BadgeNumber;  // NEW
                user.PhoneNumber = model.PhoneNumber;  // NEW
                user.IsActive = model.IsActive;  // NEW

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    if (!string.IsNullOrEmpty(model.CurrentRole))
                    {
                        await _userManager.AddToRoleAsync(user, model.CurrentRole);
                    }

                    TempData["Success"] = "User updated successfully!";
                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            await LoadRoles();
            return View(model);
        }

        public IActionResult Roles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        public IActionResult Logs()
        {
            // Implement activity logs view
            return View();
        }

        private async Task LoadRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.Roles = new SelectList(roles);
        }

        // ADD these methods to existing AdminController class

        // ==================== USER STATUS TOGGLE ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Json(new
                {
                    success = true,
                    isActive = user.IsActive,
                    message = user.IsActive ? "User activated successfully!" : "User deactivated successfully!"
                });
            }

            return Json(new { success = false, message = "Failed to update user status." });
        }

        // ==================== PASSWORD RESET ====================
        [HasPermission(Permission.ResetPassword)]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            return View(new ResetPasswordViewModel
            {
                UserId = user.Id,
                UserName = user.UserName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.ResetPassword)]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Users));
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Password reset successfully!";
                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // ==================== DELETE USER ====================
        [HasPermission(Permission.DeleteUser)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (user.Id == currentUser.Id)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Users));
            }

            return View(new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteUser)]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = "User deactivated successfully!";
            }

            return RedirectToAction(nameof(Users));
        }

        // ==================== ROLE MANAGEMENT ====================
        [HasPermission(Permission.CreateRole)]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.CreateRole)]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.Name);
                if (roleExists)
                {
                    ModelState.AddModelError("Name", "Role already exists.");
                    return View(model);
                }

                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));

                if (result.Succeeded)
                {
                    TempData["Success"] = $"Role '{model.Name}' created successfully!";
                    return RedirectToAction(nameof(Roles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HasPermission(Permission.UpdateRole)]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "Role not found.";
                return RedirectToAction(nameof(Roles));
            }

            return View(new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.UpdateRole)]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    TempData["Error"] = "Role not found.";
                    return RedirectToAction(nameof(Roles));
                }

                if (role.Name == "Admin")
                {
                    TempData["Error"] = "Cannot modify Admin role.";
                    return RedirectToAction(nameof(Roles));
                }

                role.Name = model.Name;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Role updated successfully!";
                    return RedirectToAction(nameof(Roles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HasPermission(Permission.DeleteRole)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "Role not found.";
                return RedirectToAction(nameof(Roles));
            }

            if (role.Name == "Admin")
            {
                TempData["Error"] = "Cannot delete Admin role.";
                return RedirectToAction(nameof(Roles));
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            ViewBag.UsersCount = usersInRole.Count;

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.DeleteRole)]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["Error"] = "Role not found.";
                return RedirectToAction(nameof(Roles));
            }

            if (role.Name == "Admin")
            {
                TempData["Error"] = "Cannot delete Admin role.";
                return RedirectToAction(nameof(Roles));
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                TempData["Error"] = "Cannot delete role. Users are assigned to this role.";
                return RedirectToAction(nameof(Roles));
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["Success"] = "Role deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete role.";
            }

            return RedirectToAction(nameof(Roles));
        }

        // Data Management Actions

        [HttpGet]
        public IActionResult DataManagement()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpdateItems()
        {
            try
            {
                int updatedCount = await _itemService.BulkUpdateItemsBanglaAndStockAsync();

                TempData["Success"] = $"Successfully updated {updatedCount} items with Bangla names and stock values!";
                return RedirectToAction(nameof(DataManagement));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating items: {ex.Message}";
                return RedirectToAction(nameof(DataManagement));
            }
        }
    }
}
