using IMS.Domain.Enums;

namespace IMS.Application.Helpers
{
    public static class ConditionHelper
    {
        /// <summary>
        /// Determines if an item condition is serviceable or unserviceable
        /// </summary>
        public static ServiceableStatus GetServiceableStatus(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition))
                return ServiceableStatus.Unserviceable;

            return condition.Equals("Good", StringComparison.OrdinalIgnoreCase)
                ? ServiceableStatus.Serviceable
                : ServiceableStatus.Unserviceable;
        }

        /// <summary>
        /// Determines if an item condition is serviceable or unserviceable from enum
        /// </summary>
        public static ServiceableStatus GetServiceableStatus(ItemCondition condition)
        {
            return condition == ItemCondition.Good
                ? ServiceableStatus.Serviceable
                : ServiceableStatus.Unserviceable;
        }

        /// <summary>
        /// Gets the display text for serviceable status
        /// </summary>
        public static string GetServiceableStatusText(string condition)
        {
            var status = GetServiceableStatus(condition);
            return status == ServiceableStatus.Serviceable ? "Serviceable" : "Unserviceable";
        }

        /// <summary>
        /// Gets the CSS badge class for serviceable status
        /// </summary>
        public static string GetServiceableStatusBadgeClass(string condition)
        {
            var status = GetServiceableStatus(condition);
            return status == ServiceableStatus.Serviceable ? "badge-success" : "badge-danger";
        }

        /// <summary>
        /// Gets the icon for serviceable status
        /// </summary>
        public static string GetServiceableStatusIcon(string condition)
        {
            var status = GetServiceableStatus(condition);
            return status == ServiceableStatus.Serviceable ? "fa-check-circle" : "fa-times-circle";
        }

        /// <summary>
        /// Checks if condition is serviceable
        /// </summary>
        public static bool IsServiceable(string condition)
        {
            return GetServiceableStatus(condition) == ServiceableStatus.Serviceable;
        }
    }
}
