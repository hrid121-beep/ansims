using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using IMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [HasPermission(Permission.ViewRole)]
    public class PermissionController : Controller
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionController(
            IRolePermissionService rolePermissionService,
            RoleManager<IdentityRole> roleManager)
        {
            _rolePermissionService = rolePermissionService;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var rolesWithPermissions = await _rolePermissionService.GetAllRolesWithPermissionsAsync();
            return View(rolesWithPermissions);
        }

        [HasPermission(Permission.UpdateRole)]
        public async Task<IActionResult> Edit(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            var roleWithPermissions = await _rolePermissionService.GetRoleWithPermissionsAsync(roleId);
            ViewBag.RoleName = role.Name;
            ViewBag.RoleId = role.Id;

            var permissionsByCategory = roleWithPermissions.Permissions
                .GroupBy(p => p.Category)
                .OrderBy(g => g.Key);

            return View(permissionsByCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.AssignPermission)]
        public async Task<IActionResult> UpdatePermissions(string roleId, List<int> selectedPermissions)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest();
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            if (role.Name == "Admin")
            {
                TempData["Error"] = "Cannot modify Admin role permissions!";
                return RedirectToAction(nameof(Edit), new { roleId });
            }

            await _rolePermissionService.RemoveAllPermissionsFromRoleAsync(roleId);

            if (selectedPermissions != null && selectedPermissions.Any())
            {
                var permissions = selectedPermissions.Select(p => (Permission)p).ToList();
                await _rolePermissionService.AssignPermissionsToRoleAsync(new RolePermissionCreateDto
                {
                    RoleId = roleId,
                    Permissions = permissions
                });
            }

            TempData["Success"] = $"Permissions updated successfully for role: {role.Name}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.AssignPermission)]
        public async Task<IActionResult> CopyPermissions(string sourceRoleId, string targetRoleId)
        {
            if (string.IsNullOrEmpty(sourceRoleId) || string.IsNullOrEmpty(targetRoleId))
            {
                return BadRequest();
            }

            var targetRole = await _roleManager.FindByIdAsync(targetRoleId);
            if (targetRole?.Name == "Admin")
            {
                return Json(new { success = false, message = "Cannot modify Admin role permissions!" });
            }

            try
            {
                await _rolePermissionService.CopyPermissionsAsync(sourceRoleId, targetRoleId);
                return Json(new { success = true, message = "Permissions copied successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRolePermissions(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest();
            }

            var permissions = await _rolePermissionService.GetPermissionsByRoleAsync(roleId);
            return Json(permissions.Where(p => p.IsGranted).Select(p => (int)p.Permission));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(Permission.AssignPermission)]
        public async Task<IActionResult> TogglePermission(string roleId, int permission, bool isGranted)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest();
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role?.Name == "Admin")
            {
                return Json(new { success = false, message = "Cannot modify Admin role permissions!" });
            }

            try
            {
                await _rolePermissionService.UpdateRolePermissionAsync(new RolePermissionUpdateDto
                {
                    RoleId = roleId,
                    Permission = (Permission)permission,
                    IsGranted = isGranted
                });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InitializeDefaultPermissions()
        {
            try
            {
                await _rolePermissionService.InitializeDefaultPermissionsAsync();
                return Json(new { success = true, message = "Default permissions initialized successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error initializing permissions: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckUserPermission(string userId, int permission)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var hasPermission = await _rolePermissionService.UserHasPermissionAsync(userId, (Permission)permission);
            return Json(new { hasPermission });
        }
    }
}