using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IActivityLogService _activityLogService;

        public UserService(

           UserManager<User> userManager,
           RoleManager<IdentityRole> roleManager,
           IActivityLogService activityLogService)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _activityLogService = activityLogService; // Add this line
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.Where(u => u.IsActive).ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Designation = user.Designation,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });
            }

            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !user.IsActive) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Designation = user.Designation,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            };
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = new User
            {
                UserName = userCreateDto.Email,
                Email = userCreateDto.Email,
                FullName = userCreateDto.FullName,
                Designation = userCreateDto.Designation,
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, userCreateDto.Password);
            if (result.Succeeded)
            {
                foreach (var role in userCreateDto.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                return new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Designation = user.Designation,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = userCreateDto.Roles
                };
            }

            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user != null)
            {
                user.FullName = userDto.FullName;
                user.Designation = userDto.Designation;
                user.IsActive = userDto.IsActive;

                await _userManager.UpdateAsync(user);

                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, userDto.Roles);
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return await _userManager.GetRolesAsync(user);
            }
            return new List<string>();
        }

        public async Task AddUserToRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task RemoveUserFromRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.IsActive) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Designation = user.Designation,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            var userDtos = new List<UserDto>();

            foreach (var user in usersInRole.Where(u => u.IsActive))
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    Designation = user.Designation,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });
            }

            return userDtos;
        }

        public async Task<bool> EmailExistsAsync(string email, string excludeUserId = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(excludeUserId) && user.Id == excludeUserId)
                return false;

            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
        {
            return await GetAllUsersAsync();
        }

        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllUsersAsync();

            var users = await GetAllUsersAsync();
            searchTerm = searchTerm.ToLower();

            return users.Where(u =>
                u.UserName.ToLower().Contains(searchTerm) ||
                u.Email.ToLower().Contains(searchTerm) ||
                u.FullName.ToLower().Contains(searchTerm) ||
                (u.Designation != null && u.Designation.ToLower().Contains(searchTerm))
            );
        }

        public async Task<IEnumerable<UserDto>> GetPagedUsersAsync(int pageNumber, int pageSize)
        {
            var allUsers = await GetAllUsersAsync();
            return allUsers
                .OrderBy(u => u.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task AssignUserToBattalionAsync(string userId, int battalionId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return; // Changed from return false

            user.BattalionId = battalionId;
            await _userManager.UpdateAsync(user);

            await _activityLogService.LogActivityAsync(
                "User",
                0,
                "AssignToBattalion",
                $"User {userId} assigned to battalion {battalionId}",
                userId
            );
        }

        public async Task AssignUserToRangeAsync(string userId, int rangeId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.RangeId = rangeId;
            await _userManager.UpdateAsync(user);
        }

        public async Task AssignUserToZilaAsync(string userId, int zilaId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.ZilaId = zilaId;
            await _userManager.UpdateAsync(user);
        }

        public async Task AssignUserToUpazilaAsync(string userId, int upazilaId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.UpazilaId = upazilaId;
            await _userManager.UpdateAsync(user);
        }


        public async Task<IEnumerable<UserDto>> GetUsersByBattalionAsync(int battalionId)
        {
            var users = _userManager.Users.Where(u => u.BattalionId == battalionId);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                });
            }

            return userDtos;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRangeAsync(int rangeId)
        {
            var users = _userManager.Users.Where(u => u.RangeId == rangeId);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                });
            }

            return userDtos;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByZilaAsync(int zilaId)
        {
            var users = _userManager.Users.Where(u => u.ZilaId == zilaId);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive
                });
            }

            return userDtos;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByUpazilaAsync(int upazilaId)
        {
            var users = _userManager.Users.Where(u => u.UpazilaId == upazilaId);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive
                });
            }

            return userDtos;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByDesignationAsync(string designation)
        {
            var users = _userManager.Users.Where(u => u.Designation == designation);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive
                });
            }

            return userDtos;
        }

        public async Task<bool> BadgeNumberExistsAsync(string badgeNumber, string excludeUserId = null)
        {
            var query = _userManager.Users.Where(u => u.BadgeNumber == badgeNumber);

            if (!string.IsNullOrEmpty(excludeUserId))
                query = query.Where(u => u.Id != excludeUserId);

            return await query.AnyAsync();
        }

        public async Task UpdateUserBadgeNumberAsync(string userId, string badgeNumber)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            user.BadgeNumber = badgeNumber;
            await _userManager.UpdateAsync(user);
        }

    }
}