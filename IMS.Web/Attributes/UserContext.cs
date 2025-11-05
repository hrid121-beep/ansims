using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;

    public UserContext(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public string UserId => GetCurrentUserId();

    public string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }

    public string CurrentUserName => GetCurrentUserName();

    public async Task<bool> HasPermissionAsync(Permission permission)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var roles = await _userManager.GetRolesAsync(user);
        return await Task.FromResult(true);
    }

    public string UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public string UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public string UserRole => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Task<string> GetCurrentUserIdAsync()
    {
        return Task.FromResult(UserId);
    }

    public Task<string> GetCurrentUserNameAsync()
    {
        return Task.FromResult(UserName);
    }

    public Task<List<string>> GetCurrentUserRolesAsync()
    {
        var roles = _httpContextAccessor.HttpContext?.User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();

        return Task.FromResult(roles);
    }

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public int? GetCurrentStoreId()
    {
        // Get from claims (set by CustomUserClaimsPrincipalFactory)
        var storeIdClaim = _httpContextAccessor.HttpContext?.User?
            .FindFirst("StoreId")?.Value;

        if (!string.IsNullOrEmpty(storeIdClaim) && int.TryParse(storeIdClaim, out int storeId))
        {
            // Return null if StoreId is 0 (means user has no store assigned)
            return storeId > 0 ? storeId : null;
        }

        return null;
    }
}