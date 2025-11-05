using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using IMS.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (field == null) return enumValue.ToString();

        var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute != null)
            return displayAttribute.Name ?? enumValue.ToString();

        var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute?.Description ?? enumValue.ToString();
    }
}

public static class ConversionHelpers
{
    public static int ConvertNullableToInt(int? value, int defaultValue = 0)
    {
        return value ?? defaultValue;
    }

    public static int? ConvertIntToNullable(int value)
    {
        return value == 0 ? (int?)null : value;
    }
}

public static class StoreTypeConversionHelper
{
    public static int? GetStoreTypeIdFromOldEnum(string oldEnumValue)
    {
        return oldEnumValue?.ToUpperInvariant() switch
        {
            "MAIN" => (int)StoreLevel.Main,
            "SUBSTORE" => (int)StoreLevel.SubStore,
            "TEMPORARY" => (int)StoreLevel.Temporary,
            "HQ" => (int)StoreLevel.HQ,
            "BATTALION" => (int)StoreLevel.Battalion,
            "RANGE" => (int)StoreLevel.Range,
            "ZILA" or "DISTRICT" => (int)StoreLevel.Zila,
            "UPAZILA" => (int)StoreLevel.Upazila,
            _ => null
        };
    }

    public static StoreLevel GetStoreLevelFromOldEnum(string oldEnumValue)
    {
        return oldEnumValue?.ToUpperInvariant() switch
        {
            "MAIN" => StoreLevel.Main,
            "SUBSTORE" => StoreLevel.SubStore,
            "TEMPORARY" => StoreLevel.Temporary,
            "HQ" => StoreLevel.HQ,
            "BATTALION" => StoreLevel.Battalion,
            "RANGE" => StoreLevel.Range,
            "ZILA" or "DISTRICT" => StoreLevel.Zila,
            "UPAZILA" => StoreLevel.Upazila,
            _ => StoreLevel.HQ
        };
    }
}

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
{
    private readonly ApplicationDbContext _context;

    public CustomUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        ApplicationDbContext context)
        : base(userManager, roleManager, optionsAccessor)
    {
        _context = context;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Add user profile claims
        if (!string.IsNullOrEmpty(user.FullName))
        {
            identity.AddClaim(new Claim("FullName", user.FullName));
        }

        if (!string.IsNullOrEmpty(user.Department))
        {
            identity.AddClaim(new Claim("Department", user.Department));
        }

        if (!string.IsNullOrEmpty(user.Designation))
        {
            identity.AddClaim(new Claim("Designation", user.Designation));
        }

        if (!string.IsNullOrEmpty(user.BadgeNumber))
        {
            identity.AddClaim(new Claim("BadgeNumber", user.BadgeNumber));
        }

        // Add StoreId claim from UserStores table
        try
        {
            var userStore = await _context.UserStores
                .Where(us => us.UserId == user.Id && us.IsActive)
                .OrderByDescending(us => us.IsPrimary)
                .ThenBy(us => us.Id)
                .Select(us => new { us.StoreId })
                .FirstOrDefaultAsync();

            if (userStore != null)
            {
                identity.AddClaim(new Claim("StoreId", userStore.StoreId.ToString()));

                // Get store name
                var storeName = await _context.Stores
                    .Where(s => s.Id == userStore.StoreId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(storeName))
                {
                    identity.AddClaim(new Claim("StoreName", storeName));
                }
            }
            else
            {
                identity.AddClaim(new Claim("StoreId", "0"));
            }
        }
        catch
        {
            // Fallback if UserStores query fails
            identity.AddClaim(new Claim("StoreId", "0"));
        }

        // Add organizational claims
        if (user.BattalionId.HasValue)
        {
            identity.AddClaim(new Claim("BattalionId", user.BattalionId.Value.ToString()));
        }

        if (user.RangeId.HasValue)
        {
            identity.AddClaim(new Claim("RangeId", user.RangeId.Value.ToString()));
        }

        if (user.ZilaId.HasValue)
        {
            identity.AddClaim(new Claim("ZilaId", user.ZilaId.Value.ToString()));
        }

        if (user.UpazilaId.HasValue)
        {
            identity.AddClaim(new Claim("UpazilaId", user.UpazilaId.Value.ToString()));
        }

        return identity;
    }
}