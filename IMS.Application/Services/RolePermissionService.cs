using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Reflection;

namespace IMS.Application.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserContext _userContext;

        public RolePermissionService(
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
            _userContext = userContext;
        }

        public async Task<IEnumerable<RolePermissionDto>> GetAllRolePermissionsAsync()
        {
            var rolePermissions = await _unitOfWork.RolePermissions.GetAllAsync();
            return rolePermissions.Where(rp => rp.IsActive).Select(MapToDto);
        }

        public async Task<IEnumerable<RolePermissionDto>> GetPermissionsByRoleAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return Enumerable.Empty<RolePermissionDto>();

            var permissions = await _unitOfWork.RolePermissions.FindAsync(rp => rp.RoleId == roleId && rp.IsActive);
            return permissions.Select(MapToDto);
        }

        public async Task<RoleWithPermissionsDto> GetRoleWithPermissionsAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return null;

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return null;

            var allPermissions = (await GetAllPermissionsAsync()).ToList(); // ✅ এখানে .ToList() add করুন
            var rolePermissions = await GetPermissionsByRoleAsync(roleId);

            foreach (var permission in allPermissions)
            {
                var rolePermission = rolePermissions.FirstOrDefault(rp => rp.Permission == permission.Permission);
                permission.IsGranted = rolePermission?.IsGranted ?? false;
            }

            return new RoleWithPermissionsDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Permissions = allPermissions
            };
        }

        public async Task<IEnumerable<RoleWithPermissionsDto>> GetAllRolesWithPermissionsAsync()
        {
            var roles = _roleManager.Roles.ToList();
            var result = new List<RoleWithPermissionsDto>();

            foreach (var role in roles)
            {
                var roleWithPermissions = await GetRoleWithPermissionsAsync(role.Id);
                if (roleWithPermissions != null)
                {
                    result.Add(roleWithPermissions);
                }
            }

            return result;
        }

        public async Task AssignPermissionsToRoleAsync(RolePermissionCreateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.RoleId))
                throw new ArgumentException("Role ID is required", nameof(dto.RoleId));

            if (dto.Permissions == null || !dto.Permissions.Any())
                throw new ArgumentException("At least one permission is required", nameof(dto.Permissions));

            var role = await _roleManager.FindByIdAsync(dto.RoleId);
            if (role == null)
                throw new InvalidOperationException("Role not found");

            var currentUser = _userContext.GetCurrentUserName() ?? "System";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var permission in dto.Permissions)
                {
                    var existingPermission = await _unitOfWork.RolePermissions.FirstOrDefaultAsync(
                        rp => rp.RoleId == dto.RoleId && rp.Permission == permission);

                    if (existingPermission == null)
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleId = dto.RoleId,
                            RoleName = role.Name,
                            Permission = permission,
                            PermissionName = permission.ToString(),
                            Description = GetPermissionDescription(permission),
                            IsGranted = true,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = currentUser
                        };

                        await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                    }
                    else if (!existingPermission.IsActive)
                    {
                        existingPermission.IsActive = true;
                        existingPermission.IsGranted = true;
                        existingPermission.UpdatedAt = DateTime.UtcNow;
                        existingPermission.UpdatedBy = currentUser;
                        _unitOfWork.RolePermissions.Update(existingPermission);
                    }
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateRolePermissionAsync(RolePermissionUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.RoleId))
                throw new ArgumentException("Role ID is required", nameof(dto.RoleId));

            var currentUser = _userContext.GetCurrentUserName() ?? "System";

            var rolePermission = await _unitOfWork.RolePermissions.FirstOrDefaultAsync(
                rp => rp.RoleId == dto.RoleId && rp.Permission == dto.Permission);

            if (rolePermission != null)
            {
                rolePermission.IsGranted = dto.IsGranted;
                rolePermission.UpdatedAt = DateTime.UtcNow;
                rolePermission.UpdatedBy = currentUser;
                _unitOfWork.RolePermissions.Update(rolePermission);
            }
            else
            {
                var role = await _roleManager.FindByIdAsync(dto.RoleId);
                if (role == null)
                    throw new InvalidOperationException("Role not found");

                rolePermission = new RolePermission
                {
                    RoleId = dto.RoleId,
                    RoleName = role.Name,
                    Permission = dto.Permission,
                    PermissionName = dto.Permission.ToString(),
                    Description = GetPermissionDescription(dto.Permission),
                    IsGranted = dto.IsGranted,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUser
                };
                await _unitOfWork.RolePermissions.AddAsync(rolePermission);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task RemovePermissionFromRoleAsync(string roleId, Permission permission)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentException("Role ID is required", nameof(roleId));

            var currentUser = _userContext.GetCurrentUserName() ?? "System";

            var rolePermission = await _unitOfWork.RolePermissions.FirstOrDefaultAsync(
                rp => rp.RoleId == roleId && rp.Permission == permission);

            if (rolePermission != null)
            {
                rolePermission.IsActive = false;
                rolePermission.UpdatedAt = DateTime.UtcNow;
                rolePermission.UpdatedBy = currentUser;
                _unitOfWork.RolePermissions.Update(rolePermission);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task RemoveAllPermissionsFromRoleAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentException("Role ID is required", nameof(roleId));

            var currentUser = _userContext.GetCurrentUserName() ?? "System";

            var permissions = await _unitOfWork.RolePermissions.FindAsync(rp => rp.RoleId == roleId && rp.IsActive);

            foreach (var permission in permissions)
            {
                permission.IsActive = false;
                permission.UpdatedAt = DateTime.UtcNow;
                permission.UpdatedBy = currentUser;
                _unitOfWork.RolePermissions.Update(permission);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> HasPermissionAsync(string roleId, Permission permission)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return false;

            var rolePermission = await _unitOfWork.RolePermissions.FirstOrDefaultAsync(
                rp => rp.RoleId == roleId && rp.Permission == permission && rp.IsActive);

            return rolePermission?.IsGranted ?? false;
        }

        public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<Permission>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<Permission>();

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new HashSet<Permission>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var rolePermissions = await _unitOfWork.RolePermissions.FindAsync(
                        rp => rp.RoleId == role.Id && rp.IsActive && rp.IsGranted);

                    foreach (var rp in rolePermissions)
                    {
                        permissions.Add(rp.Permission);
                    }
                }
            }

            return permissions;
        }

        public async Task<bool> UserHasPermissionAsync(string userId, Permission permission)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            var userPermissions = await GetUserPermissionsAsync(userId);
            return userPermissions.Contains(permission);
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = Enum.GetValues(typeof(Permission))
                .Cast<Permission>()
                .Select(p => new PermissionDto
                {
                    Permission = p,
                    Name = p.ToString(),
                    Category = GetPermissionCategory(p),
                    Description = GetPermissionDescription(p),
                    IsGranted = false
                })
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name);

            return await Task.FromResult(permissions);
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return Enumerable.Empty<PermissionDto>();

            var allPermissions = await GetAllPermissionsAsync();
            return allPermissions.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        public async Task CopyPermissionsAsync(string sourceRoleId, string targetRoleId)
        {
            if (string.IsNullOrWhiteSpace(sourceRoleId))
                throw new ArgumentException("Source role ID is required", nameof(sourceRoleId));

            if (string.IsNullOrWhiteSpace(targetRoleId))
                throw new ArgumentException("Target role ID is required", nameof(targetRoleId));

            if (sourceRoleId == targetRoleId)
                throw new InvalidOperationException("Source and target roles cannot be the same");

            var sourcePermissions = await GetPermissionsByRoleAsync(sourceRoleId);
            var targetRole = await _roleManager.FindByIdAsync(targetRoleId);

            if (targetRole == null)
                throw new InvalidOperationException("Target role not found");

            var currentUser = _userContext.GetCurrentUserName() ?? "System";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await RemoveAllPermissionsFromRoleAsync(targetRoleId);

                foreach (var permission in sourcePermissions.Where(p => p.IsGranted))
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = targetRoleId,
                        RoleName = targetRole.Name,
                        Permission = permission.Permission,
                        PermissionName = permission.PermissionName,
                        Description = permission.Description,
                        IsGranted = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = currentUser
                    };

                    await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task InitializeDefaultPermissionsAsync()
        {
            var defaultPermissions = new Dictionary<string, List<Permission>>
            {
                ["Admin"] = Enum.GetValues(typeof(Permission)).Cast<Permission>().ToList(),
                ["StoreManager"] = new List<Permission>
                {
                    Permission.ViewDashboard,
                    Permission.ViewCategory, Permission.ViewSubCategory, Permission.ViewBrand,
                    Permission.ViewItem, Permission.CreateItem, Permission.UpdateItem,
                    Permission.ViewStore, Permission.ViewOwnStore,
                    Permission.ViewVendor, Permission.CreateVendor,
                    Permission.ViewPurchase, Permission.CreatePurchase,
                    Permission.ViewIssue, Permission.CreateIssue,
                    Permission.ViewTransfer, Permission.CreateTransfer,
                    Permission.ViewDamage, Permission.CreateDamage,
                    Permission.ViewWriteOff, Permission.CreateWriteOff,
                    Permission.ViewReturn, Permission.CreateReturn,
                    Permission.ViewBarcode, Permission.GenerateBarcode, Permission.PrintBarcode,
                    Permission.ViewStockReport, Permission.ExportReports
                },
                ["Operator"] = new List<Permission>
                {
                    Permission.ViewDashboard,
                    Permission.ViewItem, Permission.ViewStore,
                    Permission.ViewIssue, Permission.CreateIssue,
                    Permission.ViewReceive, Permission.CreateReceive,
                    Permission.ViewBarcode, Permission.PrintBarcode
                },
                ["Auditor"] = new List<Permission>
                {
                    Permission.ViewDashboard,
                    Permission.ViewAllReports, Permission.ExportReports,
                    Permission.ViewAuditLog, Permission.ViewActivityLog,
                    Permission.ViewAllStores, Permission.ViewAllBattalions
                },
                ["Viewer"] = new List<Permission>
                {
                    Permission.ViewDashboard,
                    Permission.ViewItem, Permission.ViewStore,
                    Permission.ViewStockReport
                }
            };

            foreach (var rolePermissions in defaultPermissions)
            {
                var role = await _roleManager.FindByNameAsync(rolePermissions.Key);
                if (role != null)
                {
                    await AssignPermissionsToRoleAsync(new RolePermissionCreateDto
                    {
                        RoleId = role.Id,
                        Permissions = rolePermissions.Value
                    });
                }
            }
        }

        private RolePermissionDto MapToDto(RolePermission entity)
        {
            if (entity == null)
                return null;

            return new RolePermissionDto
            {
                Id = entity.Id,
                RoleId = entity.RoleId,
                RoleName = entity.RoleName,
                Permission = entity.Permission,
                PermissionName = entity.PermissionName,
                Description = entity.Description,
                IsGranted = entity.IsGranted
            };
        }

        private string GetPermissionCategory(Permission permission)
        {
            var value = (int)permission;

            if (value < 100) return "Dashboard";
            if (value < 200) return "Category";
            if (value < 300) return "SubCategory";
            if (value < 400) return "Brand";
            if (value < 500) return "Item";
            if (value < 600) return "Store";
            if (value < 700) return "Vendor";
            if (value < 800) return "Purchase";
            if (value < 900) return "Issue";
            if (value < 1000) return "Receive";
            if (value < 1100) return "Transfer";
            if (value < 1200) return "Damage";
            if (value < 1300) return "WriteOff";
            if (value < 1400) return "Return";
            if (value < 1500) return "Barcode";
            if (value < 1600) return "Report";
            if (value < 1700) return "User";
            if (value < 1800) return "Role";
            if (value < 1900) return "Settings";
            if (value < 2000) return "Audit";
            if (value < 2100) return "Notification";
            if (value < 2200) return "StockAdjustment";
            return "Special";
        }

        private string GetPermissionDescription(Permission permission)
        {
            var field = permission.GetType().GetField(permission.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? $"Permission to {permission}";
        }
    }
}