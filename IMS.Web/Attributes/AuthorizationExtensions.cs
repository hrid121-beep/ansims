using IMS.Domain.Enums;
using IMS.Web.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace IMS.Web.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionBasedAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationCore(options =>
            {
                // Dashboard policies
                options.AddPolicy("ViewDashboard", policy =>
                    policy.Requirements.Add(new PermissionRequirement(Permission.ViewDashboard)));

                // Category policies
                options.AddPolicy("ManageCategories", policy =>
                    policy.Requirements.Add(new MultiplePermissionsRequirement(new[]
                    {
                        Permission.ViewCategory,
                        Permission.CreateCategory,
                        Permission.UpdateCategory,
                        Permission.DeleteCategory
                    }, requireAll: true)));

                // Store policies
                options.AddPolicy("ViewStores", policy =>
                    policy.Requirements.Add(new MultiplePermissionsRequirement(new[]
                    {
                        Permission.ViewStore,
                        Permission.ViewOwnStore
                    }, requireAll: false)));

                options.AddPolicy("ManageOwnStore", policy =>
                    policy.Requirements.Add(new StoreBasedPermissionRequirement(Permission.ViewStore, allowOwnStoreOnly: true)));

                // Purchase policies
                options.AddPolicy("CreatePurchase", policy =>
                    policy.Requirements.Add(new PermissionRequirement(Permission.CreatePurchase)));

                options.AddPolicy("ApprovePurchase", policy =>
                    policy.Requirements.Add(new PermissionRequirement(Permission.ApprovePurchase)));

                options.AddPolicy("CreateHighValuePurchase", policy =>
                    policy.Requirements.Add(new ValueBasedPermissionRequirement(
                        Permission.CreatePurchase,
                        Permission.ApprovePurchase,
                        100000m)));

                // Write-off policies
                options.AddPolicy("CreateWriteOff", policy =>
                    policy.Requirements.Add(new PermissionRequirement(Permission.CreateWriteOff)));

                options.AddPolicy("ApproveWriteOff", policy =>
                    policy.Requirements.Add(new PermissionRequirement(Permission.ApproveWriteOff)));

                options.AddPolicy("CreateHighValueWriteOff", policy =>
                    policy.Requirements.Add(new ValueBasedPermissionRequirement(
                        Permission.CreateWriteOff,
                        Permission.ApproveWriteOff,
                        50000m)));

                // Transfer policies
                options.AddPolicy("CrossBattalionTransfer", policy =>
                    policy.Requirements.Add(new MultiplePermissionsRequirement(new[]
                    {
                        Permission.CreateTransfer,
                        Permission.CrossBattalionTransfer
                    }, requireAll: true)));

                // Report policies
                options.AddPolicy("ViewReports", policy =>
                    policy.Requirements.Add(new MultiplePermissionsRequirement(new[]
                    {
                        Permission.ViewStockReport,
                        Permission.ViewPurchaseReport,
                        Permission.ViewIssueReport,
                        Permission.ViewTransferReport
                    }, requireAll: false)));

                options.AddPolicy("ViewAllReports", policy =>
                    policy.Requirements.Add(new PermissionRequirement(Permission.ViewAllReports)));

                // User management policies
                options.AddPolicy("ManageUsers", policy =>
                    policy.Requirements.Add(new MultiplePermissionsRequirement(new[]
                    {
                        Permission.ViewUser,
                        Permission.CreateUser,
                        Permission.UpdateUser,
                        Permission.DeleteUser
                    }, requireAll: true)));

                // Admin only policies
                options.AddPolicy("SystemAdmin", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("EmergencyOverride", policy =>
                    policy.Requirements.Add(new PermissionRequirement(Permission.EmergencyOverride)));
            });

            // Register authorization handlers
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, MultiplePermissionsAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, StoreBasedPermissionAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, ValueBasedPermissionAuthorizationHandler>();

            return services;
        }

        public static IServiceCollection AddRoleBasedPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireStoreManagerRole", policy => policy.RequireRole("StoreManager"));
                options.AddPolicy("RequireOperatorRole", policy => policy.RequireRole("Operator"));
                options.AddPolicy("RequireAuditorRole", policy => policy.RequireRole("Auditor"));
                options.AddPolicy("RequireViewerRole", policy => policy.RequireRole("Viewer"));

                options.AddPolicy("RequireElevatedRoles", policy =>
                    policy.RequireRole("Admin", "StoreManager"));

                options.AddPolicy("RequireOperationalRoles", policy =>
                    policy.RequireRole("Admin", "StoreManager", "Operator"));
            });

            return services;
        }
    }
}