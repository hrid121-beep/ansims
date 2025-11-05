using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IMS.Web.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<User> _userManager;
        private readonly IRolePermissionService _rolePermissionService;

        public PermissionAuthorizationHandler(
            UserManager<User> userManager,
            IRolePermissionService rolePermissionService)
        {
            _userManager = userManager;
            _rolePermissionService = rolePermissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null || !user.IsActive)
            {
                context.Fail();
                return;
            }

            var hasPermission = await _rolePermissionService.UserHasPermissionAsync(user.Id, requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    public class MultiplePermissionsAuthorizationHandler : AuthorizationHandler<MultiplePermissionsRequirement>
    {
        private readonly UserManager<User> _userManager;
        private readonly IRolePermissionService _rolePermissionService;

        public MultiplePermissionsAuthorizationHandler(
            UserManager<User> userManager,
            IRolePermissionService rolePermissionService)
        {
            _userManager = userManager;
            _rolePermissionService = rolePermissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MultiplePermissionsRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null || !user.IsActive)
            {
                context.Fail();
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            var userPermissions = await _rolePermissionService.GetUserPermissionsAsync(user.Id);

            if (requirement.RequireAll)
            {
                if (requirement.Permissions.All(p => userPermissions.Contains(p)))
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                if (requirement.Permissions.Any(p => userPermissions.Contains(p)))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }

    public class StoreBasedPermissionAuthorizationHandler : AuthorizationHandler<StoreBasedPermissionRequirement>
    {
        private readonly UserManager<User> _userManager;
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IStoreService _storeService;

        public StoreBasedPermissionAuthorizationHandler(
            UserManager<User> userManager,
            IRolePermissionService rolePermissionService,
            IStoreService storeService)
        {
            _userManager = userManager;
            _rolePermissionService = rolePermissionService;
            _storeService = storeService;
        }

		protected override async Task HandleRequirementAsync(
			AuthorizationHandlerContext context,
			StoreBasedPermissionRequirement requirement)
		{
			var user = await _userManager.GetUserAsync(context.User);
			if (user == null || !user.IsActive)
			{
				context.Fail();
				return;
			}

			var hasPermission = await _rolePermissionService.UserHasPermissionAsync(user.Id, requirement.Permission);
			if (!hasPermission)
			{
				context.Fail();
				return;
			}

			if (requirement.AllowOwnStoreOnly)
			{
				var hasViewAllStores = await _rolePermissionService.UserHasPermissionAsync(user.Id, Domain.Enums.Permission.ViewAllStores);
				if (!hasViewAllStores)
				{
					var httpContext = context.Resource as Microsoft.AspNetCore.Http.HttpContext;
					if (httpContext != null)
					{
						var storeIdClaim = httpContext.Request.RouteValues["storeId"]?.ToString()
							?? httpContext.Request.Query["storeId"].ToString();

						if (!string.IsNullOrEmpty(storeIdClaim) && int.TryParse(storeIdClaim, out int storeId))
						{
							var userStores = await _storeService.GetStoresByUserAsync(user.Id);
							if (!userStores.Any(s => s.Id == storeId))
							{
								context.Fail();
								return;
							}
						}
					}
				}
			}

			context.Succeed(requirement);
		}
	}

    public class ValueBasedPermissionAuthorizationHandler : AuthorizationHandler<ValueBasedPermissionRequirement>
    {
        private readonly UserManager<User> _userManager;
        private readonly IRolePermissionService _rolePermissionService;

        public ValueBasedPermissionAuthorizationHandler(
            UserManager<User> userManager,
            IRolePermissionService rolePermissionService)
        {
            _userManager = userManager;
            _rolePermissionService = rolePermissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ValueBasedPermissionRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null || !user.IsActive)
            {
                context.Fail();
                return;
            }

            var httpContext = context.Resource as Microsoft.AspNetCore.Http.HttpContext;
            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            var valueStr = httpContext.Request.Form["TotalAmount"].ToString()
                ?? httpContext.Request.Query["amount"].ToString();

            if (decimal.TryParse(valueStr, out decimal value))
            {
                if (value <= requirement.ThresholdValue)
                {
                    var hasBasePermission = await _rolePermissionService.UserHasPermissionAsync(user.Id, requirement.BasePermission);
                    if (hasBasePermission)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
                else
                {
                    var hasElevatedPermission = await _rolePermissionService.UserHasPermissionAsync(user.Id, requirement.ElevatedPermission);
                    if (hasElevatedPermission)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                context.Succeed(requirement);
            }
        }
    }
}