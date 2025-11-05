namespace IMS.Application.Constants
{
    public static class DataConstants
    {
        // Maximum value for SQL Server decimal(18,2)
        // This is 16 digits before decimal, 2 after
        public const decimal MAX_DECIMAL_VALUE = 9999999999999999.99m;

        // Maximum value for approval amounts (use a reasonable business limit)
        public const decimal MAX_APPROVAL_AMOUNT = 999999999999.99m; // 999 billion

        // Maximum value for quantities decimal(18,3)
        public const decimal MAX_QUANTITY_VALUE = 999999999999999.999m;

        // Maximum value for stock levels
        public const decimal MAX_STOCK_LEVEL = 999999999.999m;

        // Use double for Range attributes (this is safe as it's just validation)
        public const double MAX_DOUBLE_FOR_RANGE = 999999999999.99d;
    }
    public static class ApprovalConstants
    {
        // Maximum values for SQL Server decimal(18,2) columns
        public const decimal MAX_AMOUNT = 999999999999.99m; // 999 billion - reasonable business limit

        // Approval thresholds
        public static class Thresholds
        {
            public const decimal LOW_VALUE_MAX = 50000m;
            public const decimal MEDIUM_VALUE_MAX = 100000m;
            public const decimal HIGH_VALUE_MAX = 500000m;
            public const decimal WRITEOFF_LOW_MAX = 10000m;
        }

        // Approval levels
        public static class Levels
        {
            public const decimal STORE_MANAGER_LIMIT = 50000m;
            public const decimal DEPARTMENT_HEAD_LIMIT = 100000m;
            public const decimal FINANCE_MANAGER_LIMIT = 500000m;
            public const decimal DIRECTOR_LIMIT = MAX_AMOUNT;
        }
    }
}