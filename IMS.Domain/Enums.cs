using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace IMS.Domain.Enums
{
    public enum ItemType
    {
        Expendable = 1,
        NonExpendable = 2
    }

    public enum ItemStatus
    {
        Available = 1,
        Issued = 2,
        Damaged = 3,
        WrittenOff = 4,
        UnderRepair = 5
    }

    public enum Permission
    {
        // 0xxx - Dashboard
        [Description("Can view dashboard")]
        ViewDashboard = 1,

        // 1xxx - Category Management
        [Description("Can view categories")]
        ViewCategory = 100,
        [Description("Can create categories")]
        CreateCategory = 101,
        [Description("Can update categories")]
        UpdateCategory = 102,
        [Description("Can delete categories")]
        DeleteCategory = 103,

        // 2xxx - SubCategory Management
        [Description("Can view subcategories")]
        ViewSubCategory = 200,
        [Description("Can create subcategories")]
        CreateSubCategory = 201,
        [Description("Can update subcategories")]
        UpdateSubCategory = 202,
        [Description("Can delete subcategories")]
        DeleteSubCategory = 203,

        // 3xxx - Brand Management
        [Description("Can view brands")]
        ViewBrand = 300,
        [Description("Can create brands")]
        CreateBrand = 301,
        [Description("Can update brands")]
        UpdateBrand = 302,
        [Description("Can delete brands")]
        DeleteBrand = 303,

        // 4xxx - Item Management
        [Description("Can view items")]
        ViewItem = 400,
        [Description("Can create items")]
        CreateItem = 401,
        [Description("Can edit items")]
        EditItem = 402,
        [Description("Can delete items")]
        DeleteItem = 403,
        [Description("Can generate item codes")]
        GenerateItemCode = 404,
        [Description("Can update items")]
        UpdateItem = 405,
        [Description("Can view item models")]
        ViewItemModel = 406,
        [Description("Can create item models")]
        CreateItemModel = 407,
        [Description("Can update item models")]
        UpdateItemModel = 408,
        [Description("Can delete item models")]
        DeleteItemModel = 409,

        // 5xxx - Store Management
        [Description("Can view all stores")]
        ViewStore = 500,
        [Description("Can create stores")]
        CreateStore = 501,
        [Description("Can edit stores")]
        EditStore = 502,
        [Description("Can delete stores")]
        DeleteStore = 503,
        [Description("Can assign storekeepers")]
        AssignStoreKeeper = 504,
        [Description("Can view all stores")]
        ViewAllStores = 505,
        [Description("Can view own assigned store only")]
        ViewOwnStore = 506,
        [Description("Can update stores")]
        UpdateStore = 507,

        // 6xxx - Vendor Management
        [Description("Can view vendors")]
        ViewVendor = 600,
        [Description("Can create vendors")]
        CreateVendor = 601,
        [Description("Can update vendors")]
        UpdateVendor = 602,
        [Description("Can delete vendors")]
        DeleteVendor = 603,

        // 61xx - Expiry Tracking
        [Description("Can view expiry tracking")]
        ViewExpiryTracking = 610,
        [Description("Can create expiry records")]
        CreateExpiryTracking = 611,
        [Description("Can process expired items")]
        ProcessExpiredItems = 612,
        [Description("Can send expiry alerts")]
        SendAlerts = 613,

        // 62xx - Cycle Count
        [Description("Can view cycle counts")]
        ViewCycleCount = 620,
        [Description("Can create cycle counts")]
        CreateCycleCount = 621,
        [Description("Can start cycle counts")]
        StartCycleCount = 622,
        [Description("Can perform cycle counts")]
        PerformCycleCount = 623,
        [Description("Can complete cycle counts")]
        CompleteCycleCount = 624,
        [Description("Can approve cycle counts")]
        ApproveCycleCount = 625,

        // 63xx - Audit Trail
        [Description("Can view audit trail")]
        ViewAuditTrail = 630,

        // 66xx - Temperature Monitoring
        [Description("Can view temperature logs")]
        ViewTemperatureLogs = 660,
        [Description("Can create temperature logs")]
        CreateTemperatureLog = 661,
        [Description("Can export temperature reports")]
        ExportTemperatureReport = 662,

        // 7xxx - Purchase Management
        [Description("Can view purchases")]
        ViewPurchase = 700,
        [Description("Can create purchases")]
        CreatePurchase = 701,
        [Description("Can edit purchases")]
        EditPurchase = 702,
        [Description("Can delete purchases")]
        DeletePurchase = 703,
        [Description("Can approve purchases (Level 3 - DDG Admin)")]
        ApprovePurchase = 704,
        [Description("Can reject purchases")]
        RejectPurchase = 705,
        [Description("Can view all purchases")]
        ViewAllPurchases = 706,
        [Description("Can create marketplace purchases")]
        CreateMarketplacePurchase = 707,
        [Description("Can receive purchases in store")]
        ReceivePurchase = 708,
        [Description("Can update purchases")]
        UpdatePurchase = 709,
        [Description("Can inspect purchases (Level 2 - AD/DD Store)")]
        InspectPurchase = 710,

        // 8xxx - Issue Management
        [Description("Can view issues")]
        ViewIssue = 800,
        [Description("Can create issues")]
        CreateIssue = 801,
        [Description("Can edit issues")]
        EditIssue = 802,
        [Description("Can delete issues")]
        DeleteIssue = 803,
        [Description("Can approve issues")]
        ApproveIssue = 804,
        [Description("Can view all issues")]
        ViewAllIssues = 805,
        [Description("Can submit issues")]
        SubmitIssue = 806,
        [Description("Can process issues")]
        ProcessIssue = 807,
        [Description("Can update issues")]
        UpdateIssue = 808,
        [Description("Can process physical issues")]
        ProcessPhysicalIssue = 809,
        [Description("Can cancel issues")]
        CancelIssue = 810,
        [Description("Can export issues")]
        ExportIssues = 811,

        // 9xxx - Receive Management
        [Description("Can view receives")]
        ViewReceive = 900,
        [Description("Can create receives")]
        CreateReceive = 901,
        [Description("Can edit receives")]
        EditReceive = 902,
        [Description("Can delete receives")]
        DeleteReceive = 903,
        [Description("Can receive transfers")]
        ReceiveTransfer = 904,

        // 10xxx - Transfer Management
        [Description("Can view transfers")]
        ViewTransfer = 1000,
        [Description("Can create transfers")]
        CreateTransfer = 1001,
        [Description("Can edit transfers")]
        EditTransfer = 1002,
        [Description("Can delete transfers")]
        DeleteTransfer = 1003,
        [Description("Can approve transfers")]
        ApproveTransfer = 1004,
        [Description("Can process transfers")]
        ProcessTransfer = 1005,

        // 11xxx - Damage Management
        [Description("Can view damage reports")]
        ViewDamage = 1100,
        [Description("Can create damage reports")]
        CreateDamage = 1101,
        [Description("Can update damage reports")]
        UpdateDamage = 1102,
        [Description("Can delete damage reports")]
        DeleteDamage = 1103,
        [Description("Can approve damage reports")]
        ApproveDamage = 1104,

        // 12xxx - WriteOff Management
        [Description("Can view write-offs")]
        ViewWriteOff = 1200,
        [Description("Can create write-offs")]
        CreateWriteOff = 1201,
        [Description("Can update write-offs")]
        UpdateWriteOff = 1202,
        [Description("Can delete write-offs")]
        DeleteWriteOff = 1203,
        [Description("Can approve write-offs")]
        ApproveWriteOff = 1204,
        [Description("Can execute write-offs")]
        ExecuteWriteOff = 1205,
        [Description("Can write-off stock")]
        WriteOffStock = 1206,

        // 13xxx - Return Management
        [Description("Can view returns")]
        ViewReturn = 1300,
        [Description("Can create returns")]
        CreateReturn = 1301,
        [Description("Can edit returns")]
        EditReturn = 1302,
        [Description("Can delete returns")]
        DeleteReturn = 1303,
        [Description("Can approve returns")]
        ApproveReturn = 1304,
        [Description("Can update returns")]
        UpdateReturn = 1305,

        // 14xxx - Barcode Management
        [Description("Can view barcodes")]
        ViewBarcode = 1400,
        [Description("Can generate barcodes")]
        GenerateBarcode = 1401,
        [Description("Can print barcodes")]
        PrintBarcode = 1402,
        [Description("Can export barcodes")]
        ExportBarcode = 1403,

        // 15xx - Report Management
        [Description("Can view stock reports")]
        ViewStockReport = 1500,
        [Description("Can view purchase reports")]
        ViewPurchaseReport = 1501,
        [Description("Can view issue reports")]
        ViewIssueReport = 1502,
        [Description("Can view transfer reports")]
        ViewTransferReport = 1503,
        [Description("Can view damage reports")]
        ViewDamageReport = 1504,
        [Description("Can view write-off reports")]
        ViewWriteOffReport = 1505,
        [Description("Can view return reports")]
        ViewReturnReport = 1506,
        [Description("Can view inventory movement reports")]
        ViewInventoryMovementReport = 1507,
        [Description("Can export reports")]
        ExportReports = 1508,
        [Description("Can view all reports")]
        ViewAllReports = 1509,
        [Description("Can view reports")]
        ViewReports = 1510,

        // 16xx - User Management
        [Description("Can view all users")]
        ViewUsers = 1600,
        [Description("Can view user details")]
        ViewUser = 1601,
        [Description("Can create users")]
        CreateUser = 1602,
        [Description("Can edit users")]
        EditUser = 1603,
        [Description("Can delete users")]
        DeleteUser = 1604,
        [Description("Can assign roles to users")]
        AssignRoles = 1605,
        [Description("Can assign role")]
        AssignRole = 1606,
        [Description("Can reset user passwords")]
        ResetPassword = 1607,
        [Description("Can update users")]
        UpdateUser = 1608,

        // 17xx - Role Management
        [Description("Can view roles")]
        ViewRole = 1700,
        [Description("Can create roles")]
        CreateRole = 1701,
        [Description("Can update roles")]
        UpdateRole = 1702,
        [Description("Can delete roles")]
        DeleteRole = 1703,
        [Description("Can assign permissions to roles")]
        AssignPermission = 1704,

        // 18xx - Settings Management
        [Description("Can view settings")]
        ViewSettings = 1800,
        [Description("Can update settings")]
        UpdateSettings = 1801,
        [Description("Can view system info")]
        ViewSystemInfo = 1802,
        [Description("Can create backups")]
        CreateBackup = 1803,
        [Description("Can restore backups")]
        RestoreBackup = 1804,
        [Description("Can configure system")]
        SystemConfiguration = 1805,

        // 19xx - Audit Logs
        [Description("Can view audit logs")]
        ViewAuditLog = 1900,
        [Description("Can view activity logs")]
        ViewActivityLog = 1901,
        [Description("Can view login logs")]
        ViewLoginLog = 1902,
        [Description("Can export audit logs")]
        ExportAuditLog = 1903,

        // 20xx - Notifications
        [Description("Can view notifications")]
        ViewNotifications = 2000,
        [Description("Can create notifications")]
        CreateNotification = 2001,
        [Description("Can send notifications")]
        SendNotification = 2002,
        [Description("Can manage notifications")]
        ManageNotifications = 2003,
        [Description("Can delete notifications")]
        DeleteNotifications = 2004,

        // 21xx - Stock Adjustment
        [Description("Can view stock adjustments")]
        ViewStockAdjustment = 2100,
        [Description("Can create stock adjustments")]
        CreateStockAdjustment = 2101,
        [Description("Can approve stock adjustments")]
        ApproveStockAdjustment = 2102,
        [Description("Can adjust stock")]
        AdjustStock = 2103,
        [Description("Can create adjustments")]
        CreateAdjustment = 2104,

        // 22xx - Special Permissions
        [Description("Can view all battalions")]
        ViewAllBattalions = 2200,
        [Description("Can view own battalion only")]
        ViewOwnBattalion = 2201,
        [Description("Can view all ranges")]
        ViewAllRanges = 2202,
        [Description("Can view own range only")]
        ViewOwnRange = 2203,
        [Description("Can transfer across battalions")]
        CrossBattalionTransfer = 2204,
        [Description("Can transfer across ranges")]
        CrossRangeTransfer = 2205,
        [Description("Can override in emergencies")]
        EmergencyOverride = 2206,

        // 23xx - Quality Check
        [Description("Can perform quality checks")]
        PerformQualityCheck = 2300,
        [Description("Can approve quality checks")]
        ApproveQualityCheck = 2301,
        [Description("Can perform quality checks")]
        QualityCheck = 2302,

        // 30xx - Stock & Inventory Operations
        [Description("Can view stock")]
        ViewStock = 3000,
        [Description("Can create stock entries")]
        CreateStock = 3001,
        [Description("Can edit stock")]
        EditStock = 3002,
        [Description("Can delete stock")]
        DeleteStock = 3003,
        [Description("Can approve stock entries")]
        ApproveStock = 3004,
        [Description("Can export stock data")]
        ExportStock = 3005,
        [Description("Can view approvals")]
        ViewApproval = 3006,
        [Description("Can process approvals")]
        ProcessApproval = 3007,
        [Description("Can create emergency requests")]
        CreateEmergencyRequest = 3008,
        [Description("Can view inventory")]
        ViewInventory = 3009,
        [Description("Can create inventory")]
        CreateInventory = 3010,
        [Description("Can perform inventory counts")]
        PerformInventoryCount = 3011,
        [Description("Can export data")]
        ExportData = 3012,
        [Description("Can import data")]
        ImportData = 3013,
        [Description("Can bulk upload data")]
        BulkUpload = 3014,
        [Description("Can view physical inventory")]
        ViewPhysicalInventory = 3015,
        [Description("Can create physical inventory")]
        CreatePhysicalInventory = 3016,
        [Description("Can update physical inventory")]
        UpdatePhysicalInventory = 3017,
        [Description("Can approve physical inventory")]
        ApprovePhysicalInventory = 3018,
        [Description("Can cancel physical inventory")]
        CancelPhysicalInventory = 3019,
        [Description("Can export physical inventory")]
        ExportPhysicalInventory = 3020,
        [Description("Can review physical inventory")]
        ReviewPhysicalInventory = 3021,
        [Description("Can view reconciliation")]
        ViewReconciliation = 3033,
        [Description("Can create reconciliation")]
        CreateReconciliation = 3034,
        [Description("Can process reconciliation")]
        ProcessReconciliation = 3035,
        [Description("Can complete reconciliation")]
        CompleteReconciliation = 3036,
        [Description("Can approve reconciliation")]
        ApproveReconciliation = 3037,
        [Description("Can view stock movement")]
        ViewStockMovement = 3039,
        [Description("Can export stock movement")]
        ExportStockMovement = 3040,
        [Description("Can update stock")]
        UpdateStock = 3041,

        // 31xx - Allotment Letter (Provision Store)
        [Description("Can view allotment letters")]
        ViewAllotmentLetter = 3100,
        [Description("Can create allotment letters")]
        CreateAllotmentLetter = 3101,
        [Description("Can approve allotment letters (DD Provision)")]
        ApproveAllotmentLetter = 3102,
        [Description("Can update allotment letters")]
        UpdateAllotmentLetter = 3103,
        [Description("Can delete allotment letters")]
        DeleteAllotmentLetter = 3104,
        [Description("Can edit draft allotment letters")]
        EditAllotmentLetter = 3105,
    }

    public enum NotificationType
    {
        Info = 1,
        Warning = 2,
        Error = 3,
        Success = 4,
        StockAlert = 5,
        Approval = 6,
        UserAction = 7,
        SystemAlert = 8,
        PhysicalInventory = 9,
        StockAdjustment = 10,
        General,
        PurchaseOrder,
        StockTransfer,
        LowStock,
        ExpiryAlert,
        Information,
        Critical,
        Alert
    }

    public enum NotificationPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    // If StoreType is supposed to be an enum
    public enum StoreTypeEnum
    {
        Main = 1,
        SubStore = 2,
        Temporary = 3,
        Battalion = 4,
        Range = 5,
        District = 6,
        Central = 7,
        Regional = 8,
        Local = 9,
        Field = 10
    }

    public enum BattalionType
    {
        Male = 1,
        Female = 2
    }

    public enum OperationalStatus
    {
        Active = 1,
        Reserve = 2,
        Training = 3,
        StandDown = 4
    }


    public enum StoreLevel
    {
        Main = 1,
        SubStore = 2,
        Temporary = 3,
        HQ = 4,
        Battalion = 5,
        Range = 6,
        Zila = 7,
        Upazila = 8,
        Union = 9  // Added for future Union implementation
    }

    public enum IssuedToType
    {
        Battalion = 1,
        Range = 2,
        Zila = 3,
        Upazila = 4,
        Union = 5,      // Added for future Union implementation
        Individual = 6,
        AnsarHQ = 7
    }

    public enum ReceivedFromType
    {
        Battalion = 1,
        Range = 2,
        Zila = 3,
        Upazila = 4,
        Union = 5,      // Added for future Union implementation
        Individual = 6,
        Vendor = 7,
        AnserHQ = 8
    }

    public enum StockMovementType
    {
        Purchase = 1,
        Issue = 2,
        Transfer = 3,
        Return = 4,
        WriteOff = 5,
        Damage = 6,
        Adjustment = 7,
        PurchaseReceive,
        PhysicalCount,
        Opening,
        Production,
        Consumption
    }

    public enum ApprovalStatus
    {
        Draft = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4,
        Cancelled = 5,
        Completed = 6,
        Escalated,
        Expired,
        Awaiting
    }

    public enum TransactionStatus
    {
        Draft = 1,
        Pending = 2,
        Approved = 3,
        Processing = 4,
        InTransit = 5,
        Received = 6,
        Completed = 7,
        Cancelled = 8,
        Posted,
        Reversed,
        OnHold
    }
    public enum StockEntryType
    {
        Manual = 1,
        Purchase = 2,
        Return = 3,
        Transfer = 4,
        Adjustment = 5,
        BulkUpload = 6
    }

    public enum PhysicalInventoryStatus
    {
        Initiated,
        InProgress,
        Completed,
        UnderReview,
        Approved,
        Rejected,
        Cancelled,
        Verified,
        Scheduled,
        CountCompleted,
        Posted,
        Draft,
        PendingReview
    }

    public enum CountType
    {
        Full,
        Partial,
        Cycle,
        Spot,
        Annual,
        Quarterly,
        Random
    }

    public enum CountStatus
    {
        Pending,
        Counted,
        RecountRequested,
        Verified,
        NotStarted,
        InProgress,
        PendingRecount,  // ADD THIS
        Recounted,       // ADD THIS
        Completed,
        NotCounted,
        Disputed,
        Adjusted
    }

    public enum AdjustmentStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Approved
    }

    public enum AdjustmentType
    {
        PositiveAdjustment,
        NegativeAdjustment,
        WriteOff,
        Found
    }

    public enum Priority
    {
        Low,
        Normal,
        Medium,
        High,
        Urgent,
        Critical,
        Emergency
    }

    public enum PurchaseOrderStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected,
        PartiallyReceived,
        Received,
        Cancelled,
        Pending,
        Ordered
    }

    // ADD this new enum
    public enum ReconciliationStatus
    {
        Pending,
        InProgress,
        Completed,
        Approved,
        Rejected
    }

    // ========== Issue Related Enums ==========
    public enum IssueStatus
    {
        Pending,
        Approved,
        Rejected,
        Issued,
        PartiallyIssued,
        Completed,
        Cancelled
    }

    public enum IssueType
    {
        Regular,
        Urgent,
        Emergency,
        Scheduled
    }

    // ========== Purchase Related Enums ==========
    public enum PurchaseStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected,
        Ordered,
        PartiallyReceived,
        Received,
        Completed,
        Cancelled
    }

    public enum PurchaseType
    {
        Regular,
        Emergency,
        AutoGenerated,
        Manual
    }

    // ========== Receive Related Enums ==========
    public enum ReceiveStatus
    {
        Pending,
        InProcess,
        QualityCheckPending,
        QualityCheckInProgress,
        Completed,
        PartiallyCompleted,
        Rejected,
        Cancelled
    }

    // ========== Return Related Enums ==========
    public enum ReturnStatus
    {
        Pending,
        ConditionChecked,
        Approved,
        Rejected,
        InProcess,
        Completed,
        Cancelled
    }

    public enum ReturnType
    {
        Normal,
        Damaged,
        Expired,
        Unused,
        Defective
    }

    public enum ItemCondition
    {
        Good,           // Serviceable
        Damaged,        // Unserviceable
        Expired,        // Unserviceable
        Defective,      // Unserviceable
        Unknown         // Unserviceable
    }

    // Helper enum for simplified condition view
    public enum ServiceableStatus
    {
        Serviceable,    // Good condition only
        Unserviceable   // All other conditions
    }

    // ========== Transfer Related Enums ==========
    public enum TransferStatus
    {
        Pending,
        Approved,
        Rejected,
        InTransit,
        Received,
        Completed,
        Cancelled,
        Draft,
        ReceivedWithDiscrepancy
    }

    public enum TransferType
    {
        Regular,
        Emergency,
        Reallocation,
        Return
    }

    // ========== Quality Check Enums ==========
    public enum QualityCheckStatus
    {
        Pending,
        InProgress,
        Passed,
        Failed,
        PartiallyPassed,
        Skipped,
        Pass
    }

    // ========== Damage & Write-off Enums ==========
    public enum DamageStatus
    {
        Reported,
        Verified,
        Approved,
        Rejected,
        UnderReview,
        WrittenOff
    }

    public enum DamageType
    {
        Physical,
        Water,
        Fire,
        Expired,
        Contaminated,
        Theft,
        Natural,
        Unknown
    }

    public enum WriteOffStatus
    {
        Pending,
        UnderReview,
        Approved,
        Rejected,
        Executed,
        Cancelled,
        Draft,
        Completed
    }

    public enum DisposalMethod
    {
        Destroy,
        Recycle,
        Donate,
        Auction,
        Return,
        Other
    }

    public enum VarianceType
    {
        None,
        Shortage,
        Overage,
        Both
    }

    // ========== Stock Movement Enums ==========
    public enum ApprovalLevel
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Level5
    }

    // ========== Alert & Notification Enums ==========
    public enum AlertType
    {
        LowStock,
        OutOfStock,
        Expiry,
        Damage,
        PendingApproval,
        OrderDelay,
        SystemError
    }

    // ========== Organization Hierarchy Enums ==========
    public enum EntityType
    {
        Headquarters,
        Division,
        Battalion,
        Range,
        Zila,
        Upazila,
        Union,
        Store
    }

    // ========== Report Enums ==========
    public enum ReportType
    {
        StockStatus,
        Movement,
        Expiry,
        Damage,
        Purchase,
        Issue,
        Return,
        Transfer,
        PhysicalInventory,
        Variance,
        Audit,
        Performance,
        Custom
    }

    public enum ReportFormat
    {
        PDF,
        Excel,
        CSV,
        HTML,
        JSON,
        XML
    }

    // ========== Batch & Tracking Enums ==========
    public enum BatchStatus
    {
        Active,
        Inactive,
        Expired,
        Recalled,
        Quarantine,
        Consumed,
        Transferred,
        Quarantined,
        Rejected
    }

    public enum TrackingMethod
    {
        None,
        Batch,
        Serial,
        Both
    }

    public enum StockRotationMethod
    {
        FIFO, // First In First Out
        LIFO, // Last In First Out  
        FEFO, // First Expired First Out
        Manual
    }

    // ========== Fiscal Period Enums ==========
    public enum FiscalPeriodStatus
    {
        Open,
        Closed,
        Locked,
        Future
    }
    public enum VendorType
    {
        Supplier = 1,
        Manufacturer = 2,
        ServiceProvider = 3,
        Distributor = 4,
        Contractor = 5,
        Consultant = 6
    }
    public enum QualityStatus
    {
        Pass = 0,
        Fail = 1,
        Conditional = 2
    }

    public enum ProcurementType
    {
        GovernmentRevenue = 1,
        PrivateProcurement = 2
    }
}