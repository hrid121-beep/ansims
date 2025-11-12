
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
        private readonly IApprovalService _approvalService;
        private readonly IConfiguration _configuration;
        private readonly IActivityLogService _activityLogService;
        private readonly IStoreService _storeService;

        public AdminController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IItemService itemService,
            IApprovalService approvalService,
            IConfiguration configuration,
            IActivityLogService activityLogService,
            IStoreService storeService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _itemService = itemService;
            _approvalService = approvalService;
            _configuration = configuration;
            _activityLogService = activityLogService;
            _storeService = storeService;
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

        [HasPermission(Permission.ViewRole)]
        public IActionResult Roles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HasPermission(Permission.ViewActivityLog)]
        public async Task<IActionResult> Logs(string activityType = null, string module = null,
            string userName = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1)
        {
            try
            {
                // Get today's date range
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                // Get statistics for today
                var stats = await _activityLogService.GetSystemActivityStatsAsync(today, tomorrow);

                ViewBag.TotalActivitiesToday = stats.TotalActivities;
                ViewBag.ActiveUsers = stats.UniqueUsers;

                // Get all logs with filters
                var allLogs = await _activityLogService.GetAllActivityLogsAsync(
                    userId: string.IsNullOrEmpty(userName) ? null : userName,
                    entityName: module,
                    fromDate: fromDate,
                    toDate: toDate);

                // Filter by activity type if specified
                if (!string.IsNullOrEmpty(activityType))
                {
                    allLogs = allLogs.Where(l => l.Action?.Equals(activityType, StringComparison.OrdinalIgnoreCase) == true);
                }

                // Get total count
                var totalCount = allLogs.Count();

                // Implement pagination
                int pageSize = 20;
                var paginatedLogs = allLogs
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Get all users for activity stats
                var allUsers = _userManager.Users.ToList();
                foreach (var log in paginatedLogs)
                {
                    if (!string.IsNullOrEmpty(log.UserId))
                    {
                        var user = allUsers.FirstOrDefault(u => u.Id == log.UserId);
                        log.UserName = user?.UserName ?? "Unknown";
                    }
                }

                // Calculate failed logins and critical events
                var failedLogins = allLogs.Count(l => l.Action?.Contains("Login", StringComparison.OrdinalIgnoreCase) == true
                    && l.Description?.Contains("Failed", StringComparison.OrdinalIgnoreCase) == true);

                var criticalEvents = allLogs.Count(l => l.Action?.Contains("Delete", StringComparison.OrdinalIgnoreCase) == true);

                // Set ViewBag data
                ViewBag.FailedLogins = failedLogins;
                ViewBag.CriticalEvents = criticalEvents;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                ViewBag.TotalLogs = totalCount;
                ViewBag.PageSize = pageSize;

                // Keep filter values
                ViewBag.CurrentActivityType = activityType;
                ViewBag.CurrentModule = module;
                ViewBag.CurrentUserName = userName;
                ViewBag.CurrentFromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.CurrentToDate = toDate?.ToString("yyyy-MM-dd");

                // Get database size and oldest entry
                var allLogsForStats = await _activityLogService.GetAllActivityLogsAsync();
                ViewBag.TotalLogEntries = allLogsForStats.Count();
                ViewBag.OldestEntry = allLogsForStats.Any() ? allLogsForStats.Min(l => l.Timestamp).ToString("MMM dd, yyyy") : "N/A";

                return View(paginatedLogs);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading activity logs: {ex.Message}";
                return View(new List<ActivityLogDto>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  // CRITICAL: Only Admin can delete logs
        public async Task<IActionResult> ClearOldLogs(int days)
        {
            try
            {
                var deletedCount = await _activityLogService.DeleteArchivedLogsAsync(days);
                TempData["Success"] = $"Successfully deleted {deletedCount} log entries older than {days} days.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error clearing logs: {ex.Message}";
            }

            return RedirectToAction(nameof(Logs));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]  // CRITICAL: Only Admin can export logs
        public async Task<IActionResult> ExportLogs(string activityType = null, string module = null,
            string userName = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // Get all logs with filters
                var logs = await _activityLogService.GetAllActivityLogsAsync(
                    userId: string.IsNullOrEmpty(userName) ? null : userName,
                    entityName: module,
                    fromDate: fromDate,
                    toDate: toDate);

                // Filter by activity type if specified
                if (!string.IsNullOrEmpty(activityType))
                {
                    logs = logs.Where(l => l.Action?.Equals(activityType, StringComparison.OrdinalIgnoreCase) == true);
                }

                // Get all users for mapping
                var allUsers = _userManager.Users.ToList();

                // Create CSV content
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Timestamp,User,Activity,Module,Description,IP Address");

                foreach (var log in logs)
                {
                    var user = allUsers.FirstOrDefault(u => u.Id == log.UserId);
                    var logUserName = user?.UserName ?? "Unknown";
                    var timestamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                    var description = log.Description?.Replace(",", ";").Replace("\"", "'") ?? "";

                    csv.AppendLine($"{timestamp},{logUserName},{log.Action},{log.Module ?? log.Entity},{description},{log.IPAddress}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"ActivityLogs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error exporting logs: {ex.Message}";
                return RedirectToAction(nameof(Logs));
            }
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
        [HasPermission(Permission.EditUser)]
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
        [Authorize(Roles = "Admin")]  // CRITICAL: Only Admin can manage data
        public IActionResult DataManagement()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]  // CRITICAL: Only Admin can configure approvals
        public async Task<IActionResult> InitializeDefaultSettings()
        {
            try
            {
                var result = await _approvalService.InitializeDefaultApprovalSettingsAsync();

                if (result.Success)
                {
                    TempData["Success"] = result.Message;
                }
                else
                {
                    TempData["Error"] = result.Message;
                }

                return RedirectToAction(nameof(DataManagement));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error initializing settings: {ex.Message}";
                return RedirectToAction(nameof(DataManagement));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]  // CRITICAL: Only Admin can backup database
        public async Task<IActionResult> BackupDatabase(string format = "bak")
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var sqlConnectionString = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                var databaseName = sqlConnectionString.InitialCatalog;
                var currentUser = User?.Identity?.Name ?? "System";

                if (format.ToLower() == "bak")
                {
                    using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        // Call stored procedure for backup
                        using (var command = new Microsoft.Data.SqlClient.SqlCommand("sp_BackupDatabase", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.CommandTimeout = 600; // 10 minutes timeout

                            // Input parameters
                            command.Parameters.AddWithValue("@BackupType", "FULL");
                            command.Parameters.AddWithValue("@BackupPath", DBNull.Value);

                            // Output parameters
                            var backupFileNameParam = new Microsoft.Data.SqlClient.SqlParameter("@BackupFileName", System.Data.SqlDbType.NVarChar, 255)
                            {
                                Direction = System.Data.ParameterDirection.Output
                            };
                            command.Parameters.Add(backupFileNameParam);

                            var errorMessageParam = new Microsoft.Data.SqlClient.SqlParameter("@ErrorMessage", System.Data.SqlDbType.NVarChar, 4000)
                            {
                                Direction = System.Data.ParameterDirection.Output
                            };
                            command.Parameters.Add(errorMessageParam);

                            var successParam = new Microsoft.Data.SqlClient.SqlParameter("@Success", System.Data.SqlDbType.Bit)
                            {
                                Direction = System.Data.ParameterDirection.Output
                            };
                            command.Parameters.Add(successParam);

                            // Execute stored procedure
                            await command.ExecuteNonQueryAsync();

                            // Get output values
                            var success = (bool)successParam.Value;
                            var backupFileName = backupFileNameParam.Value?.ToString();
                            var errorMessage = errorMessageParam.Value?.ToString();

                            if (!success)
                            {
                                TempData["Error"] = $"Backup failed: {errorMessage}";
                                return RedirectToAction(nameof(DataManagement));
                            }

                            // Extract path from error message (contains full path)
                            var backupPath = errorMessage?.Replace("Backup completed successfully: ", "");

                            // Get file size for logging
                            long fileSize = 0;
                            if (!string.IsNullOrEmpty(backupPath) && System.IO.File.Exists(backupPath))
                            {
                                fileSize = new FileInfo(backupPath).Length;
                            }

                            // Log the backup operation
                            using (var logCommand = new Microsoft.Data.SqlClient.SqlCommand("sp_LogBackup", connection))
                            {
                                logCommand.CommandType = System.Data.CommandType.StoredProcedure;
                                logCommand.Parameters.AddWithValue("@BackupType", "FULL");
                                logCommand.Parameters.AddWithValue("@BackupFileName", backupFileName ?? "Unknown");
                                logCommand.Parameters.AddWithValue("@BackupPath", backupPath ?? "Unknown");
                                logCommand.Parameters.AddWithValue("@BackupSize", fileSize);
                                logCommand.Parameters.AddWithValue("@Success", success);
                                logCommand.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);
                                logCommand.Parameters.AddWithValue("@CreatedBy", currentUser);
                                await logCommand.ExecuteNonQueryAsync();
                            }

                            // Read the backup file and return it
                            if (!string.IsNullOrEmpty(backupPath) && System.IO.File.Exists(backupPath))
                            {
                                var memory = new MemoryStream();
                                using (var stream = new FileStream(backupPath, FileMode.Open, FileAccess.Read))
                                {
                                    await stream.CopyToAsync(memory);
                                }
                                memory.Position = 0;

                                // Clean up the backup file
                                try
                                {
                                    System.IO.File.Delete(backupPath);
                                }
                                catch { /* Ignore cleanup errors */ }

                                return File(memory, "application/octet-stream", backupFileName);
                            }
                            else
                            {
                                TempData["Error"] = "Backup file not found.";
                                return RedirectToAction(nameof(DataManagement));
                            }
                        }
                    }
                }
                else if (format.ToLower() == "sql")
                {
                    // Generate SQL script
                    var scriptFileName = $"{databaseName}_Script_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                    var script = await GenerateDatabaseScriptAsync(connectionString, databaseName);

                    var bytes = System.Text.Encoding.UTF8.GetBytes(script);

                    // Log the SQL script export
                    using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using (var logCommand = new Microsoft.Data.SqlClient.SqlCommand("sp_LogBackup", connection))
                        {
                            logCommand.CommandType = System.Data.CommandType.StoredProcedure;
                            logCommand.Parameters.AddWithValue("@BackupType", "SQL");
                            logCommand.Parameters.AddWithValue("@BackupFileName", scriptFileName);
                            logCommand.Parameters.AddWithValue("@BackupPath", "SQL Script Export");
                            logCommand.Parameters.AddWithValue("@BackupSize", bytes.Length);
                            logCommand.Parameters.AddWithValue("@Success", true);
                            logCommand.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);
                            logCommand.Parameters.AddWithValue("@CreatedBy", currentUser);
                            await logCommand.ExecuteNonQueryAsync();
                        }
                    }

                    return File(bytes, "text/plain", scriptFileName);
                }
                else
                {
                    TempData["Error"] = "Invalid backup format specified.";
                    return RedirectToAction(nameof(DataManagement));
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Backup failed: {ex.Message}";
                return RedirectToAction(nameof(DataManagement));
            }
        }

        private async Task<string> GenerateDatabaseScriptAsync(string connectionString, string databaseName)
        {
            var script = new System.Text.StringBuilder();

            script.AppendLine($"-- Database Script for {databaseName}");
            script.AppendLine($"-- Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            script.AppendLine($"-- WARNING: This script will drop and recreate the database!");
            script.AppendLine();
            script.AppendLine($"USE master;");
            script.AppendLine($"GO");
            script.AppendLine();
            script.AppendLine($"IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')");
            script.AppendLine($"BEGIN");
            script.AppendLine($"    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
            script.AppendLine($"    DROP DATABASE [{databaseName}];");
            script.AppendLine($"END");
            script.AppendLine($"GO");
            script.AppendLine();
            script.AppendLine($"CREATE DATABASE [{databaseName}];");
            script.AppendLine($"GO");
            script.AppendLine();
            script.AppendLine($"USE [{databaseName}];");
            script.AppendLine($"GO");
            script.AppendLine();

            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Get all tables
                var tablesCommand = new Microsoft.Data.SqlClient.SqlCommand(
                    "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME",
                    connection);

                var tables = new List<(string Schema, string Name)>();
                using (var reader = await tablesCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tables.Add((reader["TABLE_SCHEMA"].ToString(), reader["TABLE_NAME"].ToString()));
                    }
                }

                // Generate CREATE TABLE statements
                script.AppendLine("-- =============================================");
                script.AppendLine("-- CREATE TABLES");
                script.AppendLine("-- =============================================");
                script.AppendLine();

                foreach (var table in tables)
                {
                    script.AppendLine($"-- Table: [{table.Schema}].[{table.Name}]");

                    var createTableCmd = new Microsoft.Data.SqlClient.SqlCommand($@"
                        SELECT
                            c.COLUMN_NAME,
                            c.DATA_TYPE,
                            c.CHARACTER_MAXIMUM_LENGTH,
                            c.NUMERIC_PRECISION,
                            c.NUMERIC_SCALE,
                            c.IS_NULLABLE,
                            c.COLUMN_DEFAULT
                        FROM INFORMATION_SCHEMA.COLUMNS c
                        WHERE c.TABLE_SCHEMA = @Schema AND c.TABLE_NAME = @Table
                        ORDER BY c.ORDINAL_POSITION", connection);

                    createTableCmd.Parameters.AddWithValue("@Schema", table.Schema);
                    createTableCmd.Parameters.AddWithValue("@Table", table.Name);

                    script.AppendLine($"CREATE TABLE [{table.Schema}].[{table.Name}] (");

                    var columns = new List<string>();
                    using (var reader = await createTableCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var columnName = reader["COLUMN_NAME"].ToString();
                            var dataType = reader["DATA_TYPE"].ToString().ToUpper();
                            var maxLength = reader["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value ? "" : reader["CHARACTER_MAXIMUM_LENGTH"].ToString();
                            var isNullable = reader["IS_NULLABLE"].ToString() == "YES" ? "NULL" : "NOT NULL";
                            var defaultValue = reader["COLUMN_DEFAULT"] == DBNull.Value ? "" : $"DEFAULT {reader["COLUMN_DEFAULT"]}";

                            var columnDef = $"    [{columnName}] [{dataType}]";

                            if (dataType == "NVARCHAR" || dataType == "VARCHAR")
                            {
                                columnDef += maxLength == "-1" ? "(MAX)" : $"({maxLength})";
                            }
                            else if (dataType == "DECIMAL" || dataType == "NUMERIC")
                            {
                                var precision = reader["NUMERIC_PRECISION"];
                                var scale = reader["NUMERIC_SCALE"];
                                columnDef += $"({precision},{scale})";
                            }

                            columnDef += $" {isNullable}";

                            if (!string.IsNullOrEmpty(defaultValue))
                            {
                                columnDef += $" {defaultValue}";
                            }

                            columns.Add(columnDef);
                        }
                    }

                    script.AppendLine(string.Join(",\n", columns));
                    script.AppendLine(");");
                    script.AppendLine("GO");
                    script.AppendLine();
                }

                // Generate INSERT statements for data
                script.AppendLine("-- =============================================");
                script.AppendLine("-- INSERT DATA");
                script.AppendLine("-- =============================================");
                script.AppendLine();

                foreach (var table in tables)
                {
                    var countCmd = new Microsoft.Data.SqlClient.SqlCommand($"SELECT COUNT(*) FROM [{table.Schema}].[{table.Name}]", connection);
                    var rowCount = (int)await countCmd.ExecuteScalarAsync();

                    if (rowCount > 0)
                    {
                        script.AppendLine($"-- Data for table [{table.Schema}].[{table.Name}] ({rowCount} rows)");
                        script.AppendLine($"SET IDENTITY_INSERT [{table.Schema}].[{table.Name}] ON;");

                        var selectCmd = new Microsoft.Data.SqlClient.SqlCommand($"SELECT * FROM [{table.Schema}].[{table.Name}]", connection);
                        using (var reader = await selectCmd.ExecuteReaderAsync())
                        {
                            var columnNames = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columnNames.Add(reader.GetName(i));
                            }

                            while (await reader.ReadAsync())
                            {
                                var values = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (reader.IsDBNull(i))
                                    {
                                        values.Add("NULL");
                                    }
                                    else
                                    {
                                        var value = reader.GetValue(i);
                                        if (value is string || value is DateTime || value is Guid)
                                        {
                                            values.Add($"N'{value.ToString().Replace("'", "''")}'");
                                        }
                                        else if (value is bool)
                                        {
                                            values.Add((bool)value ? "1" : "0");
                                        }
                                        else
                                        {
                                            values.Add(value.ToString());
                                        }
                                    }
                                }

                                script.AppendLine($"INSERT INTO [{table.Schema}].[{table.Name}] ([{string.Join("], [", columnNames)}]) VALUES ({string.Join(", ", values)});");
                            }
                        }

                        script.AppendLine($"SET IDENTITY_INSERT [{table.Schema}].[{table.Name}] OFF;");
                        script.AppendLine("GO");
                        script.AppendLine();
                    }
                }
            }

            return script.ToString();
        }
    }
}
