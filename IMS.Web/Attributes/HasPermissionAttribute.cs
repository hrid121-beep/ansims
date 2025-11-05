using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IMS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly Permission _permission;

        public HasPermissionAttribute(Permission permission)
        {
            _permission = permission;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
            var rolePermissionService = context.HttpContext.RequestServices.GetService<IRolePermissionService>();

            var currentUser = await userManager.GetUserAsync(user);
            if (currentUser == null || !currentUser.IsActive)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission = await rolePermissionService.UserHasPermissionAsync(currentUser.Id, _permission);

            if (!hasPermission)
            {
                // Check if user is Admin - Admins have all permissions
                var roles = await userManager.GetRolesAsync(currentUser);
                if (!roles.Contains("Admin"))
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}