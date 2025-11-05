using IMS.Application.Interfaces;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IMS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequirePermissionAttribute : ActionFilterAttribute
    {
        private readonly Permission _permission;

        public RequirePermissionAttribute(Permission permission)
        {
            _permission = permission;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userContext = context.HttpContext.RequestServices.GetService<IUserContext>();

            if (userContext == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasPermission = await userContext.HasPermissionAsync(_permission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
