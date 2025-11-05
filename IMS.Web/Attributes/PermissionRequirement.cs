using IMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace IMS.Web.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Permission Permission { get; }

        public PermissionRequirement(Permission permission)
        {
            Permission = permission;
        }
    }

    public class MultiplePermissionsRequirement : IAuthorizationRequirement
    {
        public Permission[] Permissions { get; }
        public bool RequireAll { get; }

        public MultiplePermissionsRequirement(Permission[] permissions, bool requireAll = false)
        {
            Permissions = permissions;
            RequireAll = requireAll;
        }
    }

    public class StoreBasedPermissionRequirement : IAuthorizationRequirement
    {
        public Permission Permission { get; }
        public bool AllowOwnStoreOnly { get; }

        public StoreBasedPermissionRequirement(Permission permission, bool allowOwnStoreOnly = true)
        {
            Permission = permission;
            AllowOwnStoreOnly = allowOwnStoreOnly;
        }
    }

    public class BatchPermissionRequirement : IAuthorizationRequirement
    {
        public Permission Permission { get; }
        public int BatchSize { get; }
        public int MaxBatchSize { get; }

        public BatchPermissionRequirement(Permission permission, int batchSize, int maxBatchSize = 100)
        {
            Permission = permission;
            BatchSize = batchSize;
            MaxBatchSize = maxBatchSize;
        }
    }

    public class ValueBasedPermissionRequirement : IAuthorizationRequirement
    {
        public Permission BasePermission { get; }
        public Permission ElevatedPermission { get; }
        public decimal ThresholdValue { get; }

        public ValueBasedPermissionRequirement(Permission basePermission, Permission elevatedPermission, decimal thresholdValue)
        {
            BasePermission = basePermission;
            ElevatedPermission = elevatedPermission;
            ThresholdValue = thresholdValue;
        }
    }
}