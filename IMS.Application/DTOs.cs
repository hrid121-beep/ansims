using IMS.Application.DTOs;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTOs
{
    // Base DTO
    public class BaseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // User DTOs
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Organizational hierarchy - all nullable
        public int? BattalionId { get; set; }
        public string BattalionName { get; set; }

        public int? RangeId { get; set; }
        public string RangeName { get; set; }

        public int? ZilaId { get; set; }
        public string ZilaName { get; set; }

        public int? UpazilaId { get; set; }
        public string UpazilaName { get; set; }

        public int? UnionId { get; set; }
        public string UnionName { get; set; }

        public string Designation { get; set; }
        public string BadgeNumber { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
        public List<StoreUserDto> AssignedStores { get; set; } = new List<StoreUserDto>();
        public string PrimaryStoreName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string Department { get; set; }

    }

    public class UserCreateDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    // Login Log DTO
    public class LoginLogDto : BaseDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }

    // Category DTOs
    public class CategoryDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        // Count properties for display
        public int SubCategoryCount { get; set; } = 0;
        public int ItemCount { get; set; } = 0;
    }

    public class SubCategoryDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }


        // Count properties for display
        public int ItemCount { get; set; } = 0;
    }

    // Brand DTOs
    public class BrandDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int ModelCount { get; set; } = 0;
    }

    public class ItemModelDto : BaseDto
    {
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int ItemCount { get; set; } = 0;
    }

    // Item DTOs
    public class ItemDto : BaseDto
    {
        public string ItemCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }

        // ItemModel is optional
        public int? ItemModelId { get; set; }
        public string ModelName { get; set; }
        public string ItemModelName { get; set; } // Added missing property

        public ItemType Type { get; set; }
        public string Unit { get; set; }
        public decimal? MinimumStock { get; set; }
        public decimal? MaximumStock { get; set; }
        public decimal ReorderLevel { get; set; }

        // Brand is optional
        public int? BrandId { get; set; }
        public string BrandName { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.Available;

        // === Specifications - All optional ===
        public string Manufacturer { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool HasExpiry { get; set; }
        public int? ShelfLife { get; set; } // in days

        public string StorageRequirements { get; set; }
        public bool RequiresSpecialHandling { get; set; }
        public string SafetyInstructions { get; set; }

        public decimal? Weight { get; set; }
        public string WeightUnit { get; set; }

        public string Dimensions { get; set; }
        public string Color { get; set; }
        public string Material { get; set; }

        public bool IsHazardous { get; set; }
        public string HazardClass { get; set; }
        public decimal? UnitPrice { get; set; }

        public string ItemImage { get; set; }
        public string ImagePath { get; set; }
        public string BarcodePath { get; set; }
        public string QRCodeData { get; set; }

        // Additional properties for display
        public decimal? CurrentStock { get; set; }
        public string Location { get; set; }
        public string SerialNumber { get; set; }
        public decimal? TotalPurchased { get; set; }
        public decimal? TotalIssued { get; set; }
        public decimal? TotalReturned { get; set; }
        public decimal? TotalDamaged { get; set; }
        public decimal? TotalWrittenOff { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastIssueDate { get; set; }

        public string? ItemControlType { get; set; }
        public int? LifeSpanMonths { get; set; }
        public int? AlertBeforeDays { get; set; } = 30;

        // Separate lifespan for Ansar and VDP personnel
        public int? AnsarLifeSpanMonths { get; set; }
        public int? VDPLifeSpanMonths { get; set; }
        public int? AnsarAlertBeforeDays { get; set; }
        public int? VDPAlertBeforeDays { get; set; }

        public bool IsAnsarAuthorized { get; set; } = true;
        public bool IsVDPAuthorized { get; set; } = true;
        public bool RequiresPersonalIssue { get; set; } = false;

        public int CreatedYear { get; internal set; }
        public int CreatedMonth { get; internal set; }
        public decimal? UnitCost { get; set; }

        public List<BarcodeDto> Barcodes { get; set; } = new List<BarcodeDto>();
        public List<StoreStockDto> StoreStocks { get; set; } = new List<StoreStockDto>();
        public List<PurchaseHistoryDto> PurchaseHistory { get; set; } = new List<PurchaseHistoryDto>();
        public List<IssueHistoryDto> IssueHistory { get; set; } = new List<IssueHistoryDto>();
        public List<TransferHistoryDto> TransferHistory { get; set; } = new List<TransferHistoryDto>();
        public List<DamageHistoryDto> DamageHistory { get; set; } = new List<DamageHistoryDto>();

        public string NameBn { get; set; }
        public string Barcode { get; internal set; }
    }

    // Store DTOs
    public class StoreDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string InCharge { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ManagerName { get; set; }
        public string ManagerId { get; set; }
        public decimal? Capacity { get; set; }
        public string OperatingHours { get; set; }
        public string Remarks { get; set; }

        // Use StoreLevel enum for hierarchy
        public StoreLevel Level { get; set; }

        // Reference to StoreType entity - optional
        public int? StoreTypeId { get; set; }
        public string StoreTypeName { get; set; }
        public string Type { get; set; } // Added missing property
        public string StoreTypeCode { get; set; } // Added missing property

        // Organization hierarchy - all optional
        public int? BattalionId { get; set; }
        public string BattalionName { get; set; }
        public string BattalionCode { get; set; } // Added missing property

        public int? RangeId { get; set; }
        public string RangeName { get; set; }
        public string RangeCode { get; set; } // Added missing property

        public int? ZilaId { get; set; }
        public string ZilaName { get; set; }
        public string ZilaCode { get; set; } // Added missing property

        public int? UpazilaId { get; set; }
        public string UpazilaName { get; set; }
        public string UpazilaCode { get; set; } // Added missing property

        public int? UnionId { get; set; }
        public string UnionName { get; set; }

        public string Address { get; set; }

        public int? LocationId { get; set; }
        public string LocationDetailName { get; set; }

        // === Environmental Controls ===
        public bool RequiresTemperatureControl { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? MinTemperature { get; set; }
        public decimal? MaxTemperature { get; set; }

        // === Security ===
        public string SecurityLevel { get; set; }
        public string AccessRequirements { get; set; }

        // === Capacity ===
        public decimal? TotalCapacity { get; set; }
        public decimal? UsedCapacity { get; set; }
        public decimal? AvailableCapacity { get; set; }

        // === Store Keeper Info ===
        public string StoreKeeperName { get; set; }
        public string StoreKeeperContact { get; set; }
        public string Status { get; set; }

        // For display
        public int AssignedUserCount { get; set; }
        public List<string> AssignedUserNames { get; set; } = new List<string>();
        public int ItemCount { get; set; }
        public decimal UsedCapacityPercentage { get; set; }
        public int AssignedUsersCount { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalStockValue { get; set; }
        public string NameBn { get; set; }
        public string StoreKeeperId { get; set; }
    }

    public class StoreTypeDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool RequiresTemperatureControl { get; set; }
        public bool RequiresSecurityClearance { get; set; }
        public string DefaultManagerRole { get; set; }
        public bool IsMainStore { get; set; }
        public bool AllowDirectIssue { get; set; }
        public bool AllowTransfer { get; set; }
        public int MaxCapacity { get; set; }
        public int StoreCount { get; set; }
        public int CategoryCount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<CategoryDto> AllowedCategories { get; set; } = new List<CategoryDto>();
        public List<int> AllowedCategoryIds { get; set; } = new List<int>();
    }

    public class StoreTypeCategoryDto : BaseDto
    {
        public int StoreTypeId { get; set; }
        public int CategoryId { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsAllowed { get; set; }
        public string StoreTypeName { get; set; }
        public string CategoryName { get; set; }
    }

    public class StoreItemDto : BaseDto
    {
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal MaximumStock { get; set; }
        public decimal ReorderLevel { get; set; }
        public string Location { get; set; }
        public ItemStatus Status { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal? CurrentStock { get; set; }
        public string StockStatus { get; set; }

        public List<StockAlertDto> StockAlerts { get; set; } = new List<StockAlertDto>();
        public List<ExpiryTrackingDto> ExpiryAlerts { get; set; } = new List<ExpiryTrackingDto>();
        public List<PendingActionDto> PendingActions { get; set; } = new List<PendingActionDto>();

        public List<StockAlertDto> LowStockAlerts { get; set; } = new List<StockAlertDto>();
        public List<StockAlertDto> ReorderAlerts { get; set; } = new List<StockAlertDto>();
        public string Unit { get; internal set; }
        public bool IsLowStock { get; internal set; }
        public bool IsOutOfStock { get; internal set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; internal set; }
    }

    public class DashboardAlertDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActionUrl { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsRead { get; set; }
        public int? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
        public string Link { get; set; } // Added missing property
    }

    public class MonthlyTrendDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Value { get; set; }
        public string Label { get; set; }
        public string Trend { get; set; }
        public decimal? ChangeFromPrevious { get; set; }
        public decimal? PercentageChange { get; set; }

        // Added missing properties
        public int Count { get; set; }
        public string CategoryName { get; set; }
        public int ItemCount { get; set; }
    }

    public class BulkStockUploadDto
    {
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string EntryType { get; set; } = "Bulk";
        public DateTime EntryDate { get; set; }
        public string Remarks { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public int TotalItems => Items?.Count ?? 0;
        public decimal? TotalQuantity => Items?.Sum(i => i.Quantity) ?? 0;
        public decimal? TotalValue => Items?.Sum(i => i.TotalValue) ?? 0;
        public List<string> ValidationErrors { get; set; } = new List<string>();
        public bool IsValid => !ValidationErrors.Any();

        // Added missing properties
        public string FileName { get; set; }
        public List<BulkStockItemDto> Items { get; set; } = new List<BulkStockItemDto>();
        public List<ImportErrorDto> Errors { get; set; } = new List<ImportErrorDto>();
        public List<BulkStockItemDto> FailedRows { get; set; } = new List<BulkStockItemDto>();
        public List<BulkStockItemDto> SuccessfulRows { get; set; } = new List<BulkStockItemDto>();
        public int TotalRows { get; set; }

    }

    public class BulkStockItemDto
    {
        public int RowNumber { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string Location { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal UnitCost { get; set; }
        public decimal? TotalValue { get; set; }
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();

        // Added missing properties
        public string ValidationError { get; set; }
        public bool GenerateBarcode { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
    }

    public class StockLevelReportDto
    {
        public DateTime GeneratedDate { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public List<StockLevelDto> Items { get; set; } = new List<StockLevelDto>();
        public int TotalItems { get; set; }
        public int CriticalItems { get; set; }
        public int LowStockItems { get; set; }
        public int HealthyItems { get; set; }
        public decimal? TotalValue { get; set; }
        public string GeneratedBy { get; set; }
        public string ReportFormat { get; set; } = "Summary";

        // Added missing properties for individual item reports
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public decimal? TotalStock { get; set; }
        public decimal? MinimumStock { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? MaximumStock { get; set; }
        public Dictionary<int, decimal> StoreWiseStock { get; set; } = new Dictionary<int, decimal>();
        public decimal StockValue { get; set; }
        public string StockStatus { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public decimal CurrentStock { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public string StatusColor
        {
            get
            {
                return StockStatus switch
                {
                    "Out of Stock" => "danger",
                    "Critical" => "danger",
                    "Low Stock" => "warning",
                    "Overstocked" => "info",
                    _ => "success"
                };
            }
        }
    }

    public class ImportResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<ImportErrorDto> Errors { get; set; } = new List<ImportErrorDto>();
        public DateTime ImportDate { get; set; }
        public string ImportedBy { get; set; }
        public string FileName { get; set; }
        public TimeSpan ProcessingTime { get; set; }

        // Added missing properties
        public int TotalRows { get; set; }
        public int ImportedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }
        public int FailedCount { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public bool Success { get; set; }
        public List<ImportErrorDto> ImportErrors { get; set; } = new List<ImportErrorDto>();
        public List<int> SuccessRows { get; set; } = new List<int>();
        public List<int> ErrorRows { get; set; } = new List<int>();
        public List<int> ProcessedRows { get; set; } = new List<int>();
    }

    public class ImportErrorDto
    {
        public int RowNumber { get; set; }
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
        public string InvalidValue { get; set; }
        public string Suggestion { get; set; }

        // Added missing properties
        public int Row { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }

        public string Column { get; set; }
        public string Error { get; set; }
        public string Value { get; set; }
    }

    public class BarcodeTrackingDto
    {
        public int Id { get; set; }
        public int BarcodeId { get; set; }
        public string Action { get; set; }
        public string Location { get; set; }
        public string PerformedBy { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
        public string BarcodeNumber { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public DateTime LastScannedDate { get; set; }
        public string LastScannedBy { get; set; }
        public string LastScannedLocation { get; set; }
        public int ScanCount { get; set; }
        public string Status { get; set; }
        public List<BarcodeTrackingHistoryDto> History { get; set; } = new List<BarcodeTrackingHistoryDto>();

    }

    public class BarcodeTrackingHistoryDto
    {
        public DateTime ScannedDate { get; set; }
        public string ScannedBy { get; set; }
        public string Location { get; set; }
        public string Action { get; set; }
        public string Notes { get; set; }
    }

    public class OfflineScanDto
    {
        [Required]
        public string Barcode { get; set; }

        [Required]
        public string Action { get; set; } // "IN", "OUT", "MOVE", "COUNT"
        public int Quantity { get; set; } = 1;
        public int? StoreId { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        public string DeviceId { get; set; }
        public string BarcodeNumber { get; set; }
        public DateTime ScannedDate { get; set; }
        public string ScannedBy { get; set; }
        public bool IsSynced { get; set; }
        public DateTime? SyncedDate { get; set; }
        public string ValidationStatus { get; set; }
        public string ErrorMessage { get; set; }
        public int FromStoreId { get; set; }
        public int ToStoreId { get; set; }
        public string ScanId { get; set; } // Client-side generated ID
        public string BarcodeData { get; set; }
        public string ScanType { get; set; } // InventoryCount, Issue, Receive, Transfer
        public int? ReferenceId { get; set; } // Related document ID
        public DateTime ScannedAt { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; }
        public string UserId { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class BarcodeStatisticsDto
    {
        public int TotalBarcodes { get; set; }
        public int ActiveBarcodes { get; set; }
        public int BarcodesGeneratedToday { get; set; }
        public int BarcodesGeneratedThisWeek { get; set; }
        public int BarcodesGeneratedThisMonth { get; set; }
        public int BarcodesPrintedToday { get; set; }
        public int BarcodesScannedToday { get; set; }
        public DateTime LastGeneratedDate { get; set; }
        public DateTime LastPrintedDate { get; set; }
        public string MostActiveItem { get; set; }
        public int MostActiveItemScanCount { get; set; }
        public int ScannedToday { get; set; }
        public int ScannedThisWeek { get; set; }
        public int ScannedThisMonth { get; set; }
        public decimal? ScanRate { get; set; }
        public DateTime GeneratedDate { get; set; }
        public int InactiveBarcodes { get; set; }
        public int GeneratedToday { get; set; }
        public int PrintedToday { get; set; }
        public int TotalPrintCount { get; set; }
        public int BarcodesWithLocation { get; set; }
        public List<DailyBarcodeActivity> DailyActivity { get; set; } = new List<DailyBarcodeActivity>();
        public List<TopScannedItemDto> TopScannedItems { get; set; } = new List<TopScannedItemDto>();
        public Dictionary<string, int> BarcodesByCategory { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> BarcodesByStore { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> BarcodesByLocation { get; set; } = new Dictionary<string, int>();
    }

    public class DailyBarcodeActivity
    {
        public DateTime Date { get; set; }
        public int ScanCount { get; set; }
        public int UniqueBarcodes { get; set; }
        public int UniqueUsers { get; set; }

        // Added missing properties
        public int Generated { get; set; }
        public int Scanned { get; set; }
        public int Printed { get; set; }
        public int Deactivated { get; set; }
    }

    public class TopScannedItemDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ScanCount { get; set; }
        public DateTime LastScanned { get; set; }
    }

    public class BarcodeUsageDto
    {
        public int BarcodeId { get; set; }
        public string BarcodeNumber { get; set; }
        public string ItemName { get; set; }
        public string StoreName { get; set; }
        public int ScanCount { get; set; }
        public DateTime LastScanned { get; set; }
        public string LastAction { get; set; }
        public string LastScannedBy { get; set; }
        public List<BarcodeScanLogDto> RecentScans { get; set; } = new List<BarcodeScanLogDto>();
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalScans { get; set; }
        public int UniqueItems { get; set; }
        public int UniqueUsers { get; set; }
        public List<ItemUsageDto> ItemUsage { get; set; } = new List<ItemUsageDto>();
        public List<UserUsageDto> UserUsage { get; set; } = new List<UserUsageDto>();
        public DateTime LastUsed { get; set; }
        public string MostFrequentLocation { get; set; }
        public List<string> UsageTypes { get; set; } = new List<string>();
    }

    public class ItemUsageDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ScanCount { get; set; }
        public DateTime LastScanned { get; set; }
    }

    public class UserUsageDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int ScanCount { get; set; }
        public DateTime LastScan { get; set; }
    }

    public class QuickReceiveDto
    {
        public string ReceiveNo { get; set; }
        public string ReceiveType { get; set; }
        public string ReceivedFromName { get; set; }
        public string BadgeNo { get; set; }
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string ReceivedBy { get; set; }
        public string Notes { get; set; }

        public int? OriginalIssueId { get; set; }
        public string OriginalIssueNo { get; set; }
        public List<string> BarcodeNumbers { get; set; } = new List<string>();
        public string Condition { get; set; } = "Good";
        public string Remarks { get; set; }
        public List<QuickReceiveItemDto> Items { get; set; } = new List<QuickReceiveItemDto>();
    }

    public class PersonalizedDashboardDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DashboardStatsDto Stats { get; set; }
        public List<StockAlertDto> Alerts { get; set; } = new List<StockAlertDto>();
        public List<RecentActivityDto> RecentActivities { get; set; } = new List<RecentActivityDto>();
        public List<PendingActionDto> PendingActions { get; set; } = new List<PendingActionDto>();
        public DateTime LastUpdated { get; set; }
        public string PreferredStoreId { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>();

        // Added missing properties
        public List<StockAlertDto> CriticalAlerts { get; set; } = new List<StockAlertDto>();
        public List<StockAlertDto> WarningAlerts { get; set; } = new List<StockAlertDto>();
        public List<StockAlertDto> InfoAlerts { get; set; } = new List<StockAlertDto>();
        public int TotalAlerts { get; set; }
        public DateTime LastChecked { get; set; }
        public Dictionary<int, StoreStatisticsDto> StoreStatistics { get; set; } = new Dictionary<int, StoreStatisticsDto>();
    }

    public class StockAlertConfigDto
    {
        public int Id { get; set; }
        public string AlertType { get; set; }
        public decimal? LowStockThreshold { get; set; }
        public decimal? CriticalStockThreshold { get; set; }
        public bool IsEnabled { get; set; }
        public string NotificationMethod { get; set; }
        public List<string> Recipients { get; set; } = new List<string>();
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Added missing properties
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal? CurrentStock { get; set; }
        public decimal? MinimumStock { get; set; }
        public string AlertLevel { get; set; }
        public bool SendEmail { get; set; }
        public bool ShowInDashboard { get; set; }
        public List<string> EmailRecipients { get; set; } = new List<string>();
    }

    public class ApprovalRequirement
    {
        public string EntityType { get; set; }
        public decimal ThresholdAmount { get; set; }
        public string RequiredRole { get; set; }
        public string ApproverUserId { get; set; }
        public bool IsActive { get; set; } = true;
        public string Description { get; set; }

        // Added missing properties
        public bool RequiresApproval { get; set; }
        public string ApproverRole { get; set; }
    }

    public class BatchReportDto
    {
        public byte[] Data { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string GeneratedBy { get; set; }
        public int RecordCount { get; set; }

        public string ReportType { get; set; }
        public byte[] ReportData { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class PersonalizedAlertDashboard
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<DashboardAlertDto> HighPriorityAlerts { get; set; } = new List<DashboardAlertDto>();
        public List<DashboardAlertDto> MediumPriorityAlerts { get; set; } = new List<DashboardAlertDto>();
        public List<DashboardAlertDto> LowPriorityAlerts { get; set; } = new List<DashboardAlertDto>();
        public int TotalAlerts { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<StockAlertDto> StockAlerts { get; set; } = new List<StockAlertDto>();
        public List<ExpiryTrackingDto> ExpiryAlerts { get; set; } = new List<ExpiryTrackingDto>();
        public List<PendingActionDto> PendingActions { get; set; } = new List<PendingActionDto>();

        // Added missing properties
        public List<StockAlertDto> LowStockAlerts { get; set; } = new List<StockAlertDto>();
        public List<StockAlertDto> ReorderAlerts { get; set; } = new List<StockAlertDto>();
    }

    // Create DTOs
    public class StoreCreateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string InCharge { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
    }

    public class StoreUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string InCharge { get; set; }
        public string ContactNumber { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class AssignUserToStoreDto
    {
        public string UserId { get; set; }
        public List<int> StoreIds { get; set; } = new List<int>();
    }


    // Vendor DTOs
    public class VendorDto : BaseDto
    {
        [Required(ErrorMessage = "Vendor name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Vendor Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vendor type is required")]
        [Display(Name = "Vendor Type")]
        public string VendorType { get; set; }

        [StringLength(100, ErrorMessage = "Contact person name cannot exceed 100 characters")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [StringLength(20, ErrorMessage = "Mobile number cannot exceed 20 characters")]
        [Display(Name = "Mobile Number")]
        public string Mobile { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [StringLength(50, ErrorMessage = "TIN cannot exceed 50 characters")]
        [Display(Name = "TIN (Tax ID)")]
        public string TIN { get; set; }

        [StringLength(50, ErrorMessage = "BIN cannot exceed 50 characters")]
        [Display(Name = "BIN (Business ID)")]
        public string BIN { get; set; }
    }

    // Purchase DTOs
    public class PurchaseDto : BaseDto
    {
        public string PurchaseOrderNo { get; set; }
        //public string PurchaseNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorPhone { get; set; }
        public string VendorEmail { get; set; }

        public int? StoreId { get; set; }
        public string StoreName { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; } = 0;
        public string PurchaseType { get; set; } = "Vendor";
        public string Remarks { get; set; }
        public bool IsMarketplacePurchase { get; set; }

        public string Status { get; set; } = "Draft";
        public string MarketplaceUrl { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public int? RequisitionId { get; set; }
        public ProcurementType ProcurementType { get; set; }
        public string ProcurementTypeName => ProcurementType.ToString();
        public string ProcurementSource { get; set; }
        public string BudgetCode { get; set; }

        // Computed property
        public bool IsApproved => Status == "Approved";

        public List<PurchaseItemDto> Items { get; set; } = new List<PurchaseItemDto>();
        public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
        public List<ApprovalHistoryDto> ApprovalHistory { get; set; } = new List<ApprovalHistoryDto>();
    }

    public class PurchaseItemDto : BaseDto
    {
        public int PurchaseId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string Unit { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? AcceptedQuantity { get; set; }
        public decimal? RejectedQuantity { get; set; }
    }

    // Issue DTOs
    public class IssueDto : BaseDto
    {
        public string IssueNo { get; set; }
        public string IssueNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuedTo { get; set; }
        public string IssuedToName { get; set; }
        public string IssuedToType { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; } = "Draft";
        public string VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string ApprovalReferenceNo { get; set; }
        public string IssuedBy { get; set; }

        // Issued to organizational hierarchy - all optional
        public int? IssuedToBattalionId { get; set; }
        public string IssuedToBattalionName { get; set; }

        public int? IssuedToRangeId { get; set; }
        public string IssuedToRangeName { get; set; }

        public int? IssuedToZilaId { get; set; }
        public string IssuedToZilaName { get; set; }

        public int? IssuedToUpazilaId { get; set; }
        public string IssuedToUpazilaName { get; set; }

        public int? IssuedToUnionId { get; set; }
        public string IssuedToUnionName { get; set; }

        public string IssuedToIndividualName { get; set; }
        public string IssuedToIndividualBadgeNo { get; set; }

        public int? StoreId { get; set; }
        public string StoreName { get; set; }

        public int? BattalionId { get; set; }
        public int? RangeId { get; set; }
        public int? ZilaId { get; set; }
        public int? UpazilaId { get; set; }
        public int? ToEntityId { get; set; }
        public string ToEntityType { get; set; }
        public string FromStoreName { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovalComments { get; set; }
        public string RejectionReason { get; set; }

        // For partial issues
        public bool IsPartialIssue { get; set; }
        public int? ParentIssueId { get; set; }
        public string ParentIssueNo { get; set; }

        public string Remarks { get; set; }
        public string QRCode { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string ReceivedBy { get; set; }
        public string SignaturePath { get; set; }
        public string SignerName { get; set; }
        public string SignerBadgeId { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime? VoucherGeneratedDate { get; set; }
        public string DeliveryLocation { get; set; }
        public string IssuedToIndividualMobile { get; set; }
        public string SignerDesignation { get; set; }
        public DateTime? SignatureDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public string? ReceiverSignature { get; set; }
        public string? ReceiverBadgeId { get; set; }
        public string Department { get; set; }

        // Digital signature fields
        public string ApprovedByName { get; set; }
        public string ApprovedByBadgeNo { get; set; }
        public string ApprovalRemarks { get; set; }

        // NEW: Voucher document properties
        public string VoucherDocumentPath { get; set; }
        public string VoucherDocumentFileName { get; set; }

        // Navigation properties for display
        public BattalionDto? IssuedToBattalion { get; set; }
        public RangeDto? IssuedToRange { get; set; }
        public ZilaDto? IssuedToZila { get; set; }
        public UpazilaDto? IssuedToUpazila { get; set; }
        public int? AllotmentLetterId { get; set; }
        public string AllotmentLetterNo { get; set; }
        public string MemoNo { get; set; }
        public DateTime? MemoDate { get; set; }

        // Signature IDs
        public int? IssuerSignatureId { get; set; }
        public int? ApproverSignatureId { get; set; }
        public int? ReceiverSignatureId { get; set; }

        // Collections
        public List<IssueItemDto> Items { get; set; } = new List<IssueItemDto>();
        public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
        public List<ApprovalHistoryDto> ApprovalHistory { get; set; } = new List<ApprovalHistoryDto>();
        public List<DigitalSignatureDto> Signatures { get; set; } = new List<DigitalSignatureDto>();

        // Computed properties
        public int TotalItems => Items?.Count ?? 0;
        public decimal? TotalQuantity => Items?.Sum(i => i.Quantity) ?? 0;
        public decimal? TotalValue => Items?.Sum(i => i.Quantity * (i.UnitPrice ?? 0)) ?? 0;
        public bool CanApprove => Status == "Pending";
        public bool CanReceive => Status == "Approved";
        public bool CanEdit => Status == "Draft";

        // For partial issues
        public Dictionary<int, decimal> RequestedQuantities { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> ActualQuantities { get; set; } = new Dictionary<int, decimal>();
        public string Priority { get; internal set; }
        public string RequestedBy { get; internal set; }
        public int? FromStoreId { get; internal set; }
    }

    public class IssueItemDto : BaseDto
    {
        public int IssueId { get; set; }
        public int ItemId { get; set; }
        public int? StoreId { get; set; }
        public decimal Quantity { get; set; }

        // For display
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string StoreName { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Unit { get; set; }
        public decimal? AvailableStock { get; set; }
        public string Remarks { get; set; }
        public decimal? RequestedQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public string Condition { get; internal set; }
        public string BatchNumber { get; internal set; }
        public bool IsAvailable { get; set; }
        public decimal CurrentStock { get; set; }

        // Voucher fields
        public string LedgerNo { get; set; }
        public string PageNo { get; set; }
        public decimal? UsableQuantity { get; set; }
        public decimal? PartiallyUsableQuantity { get; set; }
        public decimal? UnusableQuantity { get; set; }
    }

    // Receive DTOs
    public class ReceiveDto : BaseDto
    {
        public string ReceiveNo { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string ReceivedFrom { get; set; }
        public string ReceivedFromType { get; set; }
        public string Remarks { get; set; }

        // Source organizational hierarchy - all optional
        public int? ReceivedFromBattalionId { get; set; }
        public string ReceivedFromBattalionName { get; set; }

        public int? ReceivedFromRangeId { get; set; }
        public string ReceivedFromRangeName { get; set; }

        public int? ReceivedFromZilaId { get; set; }
        public string ReceivedFromZilaName { get; set; }

        public int? ReceivedFromUpazilaId { get; set; }
        public string ReceivedFromUpazilaName { get; set; }

        public int? ReceivedFromUnionId { get; set; }
        public string ReceivedFromUnionName { get; set; }

        public string ReceivedFromIndividualName { get; set; }
        public string ReceivedFromIndividualBadgeNo { get; set; }
        public string ReceivedFromBadgeNo { get; set; }

        public string ReceiveNumber { get; set; }
        public string ReceivedBy { get; set; }

        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string Status { get; set; } = "Draft";

        // Link to original issue - optional
        public int? OriginalIssueId { get; set; }
        public string OriginalIssueNo { get; set; }

        public string ReceiveType { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string OriginalVoucherNo { get; set; }
        public string VoucherDocumentPath { get; set; }

        // Voucher fields for Receipt Voucher
        public string VoucherNo { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public DateTime? VoucherGeneratedDate { get; set; }
        public string VoucherQRCode { get; set; }

        public string ReceiverSignature { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverBadgeNo { get; set; }
        public string ReceiverDesignation { get; set; }

        // Condition assessment
        public string OverallCondition { get; set; }
        public string AssessmentNotes { get; set; }
        public string AssessedBy { get; set; }
        public DateTime? AssessmentDate { get; set; }

        // Collections
        public List<ReceiveItemDto> Items { get; set; } = new List<ReceiveItemDto>();

        // Computed properties
        public decimal? TotalQuantity => Items?.Sum(i => i.Quantity) ?? 0;
        public int TotalItems => Items?.Count ?? 0;
        public int DamagedItems => Items?.Count(i => i.Condition != "Good") ?? 0;

        // For partial issues
        public Dictionary<int, decimal> RequestedQuantities { get; set; } = new Dictionary<int, decimal>();
        public Dictionary<int, decimal> ActualQuantities { get; set; } = new Dictionary<int, decimal>();
        public string ReceivedFromName { get; set; }
        public List<string> DamagePhotos { get; set; } = new List<string>();
        public int IssueId { get; internal set; }
    }

    public class ReceiveItemDto : BaseDto
    {
        public decimal? IssuedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public string Condition { get; set; }
        public string DamageNotes { get; set; }
        public string DamagePhotoPath { get; set; }
        public string DamageDescription { get; set; }
        public bool IsScanned { get; set; }
        public int ReceiveId { get; set; }
        public int ItemId { get; set; }
        public int? StoreId { get; set; }
        public decimal? Quantity { get; set; }

        // For display
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string StoreName { get; set; }
        public string Unit { get; set; }
        public List<string> DamagePhotos { get; set; } = new List<string>();
        public string BarcodeNumber { get; set; }
        public decimal AcceptedQuantity { get; set; }
        public decimal? RejectedQuantity { get; set; }
        public string BatchNo { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Remarks { get; set; }

        // Voucher fields
        public string LedgerNo { get; set; }
        public string PageNo { get; set; }
        public decimal? UsableQuantity { get; set; }
        public decimal? PartiallyUsableQuantity { get; set; }
        public decimal? UnusableQuantity { get; set; }
    }

    // Transfer DTOs
    public class TransferDto : BaseDto
    {
        public string TransferNo { get; set; }
        public DateTime TransferDate { get; set; }
        public int FromStoreId { get; set; }
        public string FromStoreName { get; set; }
        public int ToStoreId { get; set; }
        public string ToStoreName { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; } = "Draft";
        public string TransferredBy { get; set; }
        public string TransferType { get; set; }
        public string Purpose { get; set; }
        public string RequestedBy { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string TransportMode { get; set; }
        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string DriverContact { get; set; }
        public List<TransferItemDto> Items { get; set; } = new List<TransferItemDto>();
    }

    public class TransferItemDto : BaseDto
    {
        public int TransferId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal? TransferQuantity { get; set; }
        public string BatchNo { get; set; }
        public string Remarks { get; set; }
    }

    // Write Off DTOs
    public class WriteOffDto : BaseDto
    {
        public string WriteOffNo { get; set; }
        public DateTime WriteOffDate { get; set; } = DateTime.Now;
        public string Reason { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }  // ✅ FIX: Make nullable
        public DateTime? ApprovedDate { get; set; }  // ✅ FIX: Keep for backwards compatibility (same as ApprovalDate)
        public decimal TotalValue { get; set; }
        public string Status { get; set; } = "Pending";
        public string RequiredApproverRole { get; set; }
        public int ApprovalThreshold { get; set; }
        public string ApprovalComments { get; set; }
        public string RejectionReason { get; set; }
        public bool CanEdit { get; set; } = true;  // ✅ FIX: Default to true

        public string AttachmentPaths { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectionDate { get; set; }

        public bool NotificationSent { get; set; }
        public DateTime? NotificationSentDate { get; set; }
        public string NotifiedUsers { get; set; }

        public int? StoreId { get; set; }
        public string StoreName { get; set; }

        // ✅ FIX: Keep only one Items property
        public List<WriteOffItemDto> Items { get; set; } = new List<WriteOffItemDto>();
    }

    public class WriteOffItemDto : BaseDto
    {
        public int WriteOffId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int? StoreId { get; set; }  // ✅ REVERT: Keep nullable to match entity
        public string StoreName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Unit { get; set; }
        public string Reason { get; set; }
    }

    // Damage DTOs
    public class DamageDto : BaseDto
    {
        public string DamageNo { get; set; }
        public DateTime DamageDate { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal? Quantity { get; set; }
        public string DamageType { get; set; }
        public string Cause { get; set; }
        public string ActionTaken { get; set; }
        public string PhotoPath { get; set; }
        public string ReportedBy { get; set; }
        public string Description { get; set; }
        public decimal EstimatedLoss { get; set; }
        public decimal TotalValue { get; set; }
        public string Status { get; set; } = "Pending";
        public string Remarks { get; set; }

        // Multi-item support
        public List<DamageItemDto> Items { get; set; } = new List<DamageItemDto>();
    }

    public class DamageItemDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string DamageType { get; set; }
        public string Description { get; set; }
        public decimal EstimatedValue { get; set; }
        public List<string> PhotoUrls { get; set; }
        public string BatchNo { get; set; }
        public string Remarks { get; set; }
    }

    // Return DTOs
    public class ReturnDto : BaseDto
    {
        public string ReturnNo { get; set; }
        public DateTime ReturnDate { get; set; }
        public string ReturnedBy { get; set; }
        public string ReturnedByType { get; set; }
        public string Reason { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal Quantity { get; set; }
        public bool IsRestocked { get; set; }
        public bool RestockApprovalRequired { get; set; }
        public string Remarks { get; set; }

        public int? OriginalIssueId { get; set; }
        public string OriginalIssueNo { get; set; }
        public string Condition { get; set; } // Item condition: Good, Damaged, Repairable, Unusable
        public bool RestockApproved { get; set; }
        public string ReturnType { get; set; } // Normal, Damaged, Expired
        public ReturnStatus Status { get; set; }
        public string RequestedBy { get; set; }
        public List<ReturnItemDto> Items { get; set; }
    }

    public class StockReturnDto : BaseDto
    {
        public int OriginalIssueId { get; set; }
        public string OriginalIssueNo { get; set; }
        public string ReturnNumber { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public string Condition { get; set; }
        public bool RestockApproved { get; set; }
        public DateTime ReturnDate { get; set; }
        public string ReturnedBy { get; set; }
    }

    public class StockOperationDto : BaseDto
    {
        public string OperationType { get; set; }
        public string ReferenceNumber { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int FromStoreId { get; set; }
        public string FromStoreName { get; set; }
        public int? ToStoreId { get; set; }
        public string ToStoreName { get; set; }
        public string Status { get; set; } = "Pending";
        public string Remarks { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }
    }

    // Issue Voucher DTOs
    public class IssueVoucherDto : BaseDto
    {
        public string VoucherNumber { get; set; }
        public int IssueId { get; set; }
        public string IssuedTo { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }
        public DateTime IssueDate { get; set; }
        public string AuthorizedBy { get; set; }
        public string ReceivedBy { get; set; }
        public byte[] ReceiverSignature { get; set; }
        public string VoucherBarcode { get; set; }

        // Added missing properties
        public string QRCodeImage { get; set; }
        public string SignatureImage { get; set; }
        public string ApprovedBy { get; set; }
        public string IssuedToDetails { get; set; }
        public string IssuedBy { get; set; }
        public List<IssueItemDto> Items { get; set; } = new List<IssueItemDto>();
        public DateTime? PrintedDate { get; set; }
        public string PrintedBy { get; set; }
        public string QRCodeData { get; set; }
        public string MemoNo { get; set; }
        public DateTime? MemoDate { get; set; }
        public string VoucherNo { get; set; }
        public string IssueNo { get; set; }
        public int? FromStoreId { get; set; }
        public string ToEntityType { get; set; }
        public int ToEntityId { get; set; }
        public string QRCode { get; set; }
    }

    // Barcode DTOs
    public class BarcodeDto : BaseDto
    {

        [Display(Name = "Barcode Number")]
        public string BarcodeNumber { get; set; }

        [Display(Name = "Barcode Type")]
        public string BarcodeType { get; set; } // Barcode, QRCode

        [Display(Name = "Item")]
        public int? ItemId { get; set; }
        public string ItemName { get; set; }

        [Display(Name = "Batch Number")]
        public string BatchNumber { get; set; }

        [Display(Name = "Reference Type")]
        public string ReferenceType { get; set; } // Issue, Receive, Transfer, etc.

        [Display(Name = "Reference ID")]
        public int? ReferenceId { get; set; }

        [Display(Name = "Generated Date")]
        public DateTime GeneratedDate { get; set; }

        [Display(Name = "Generated By")]
        public string GeneratedBy { get; set; }

        [Display(Name = "Data")]
        public string BarcodeData { get; set; }
        public string ItemCode { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string SerialNumber { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }

        public string PrintedBy { get; set; }
        public DateTime? PrintedDate { get; set; }
        public int PrintCount { get; set; } = 0;

        public string LastScannedLocation { get; set; }
        public DateTime? LastScannedDate { get; set; }
        public string LastScannedBy { get; set; }
        public int? ScanCount { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public int CategoryId { get; internal set; }
    }

    // Stock Movement DTOs
    public class StockMovementDto : BaseDto
    {
        public string MovementType { get; set; }
        public DateTime MovementDate { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public int? SourceStoreId { get; set; }
        public string SourceStoreName { get; set; }
        public int? DestinationStoreId { get; set; }
        public string DestinationStoreName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNo { get; set; }
        public int? ReferenceId { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public string Remarks { get; set; }
        public string MovedBy { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal? BalanceAfter { get; set; }
        public decimal OldQuantity { get; set; }
        public decimal NewQuantity { get; set; }
        public string ReferenceNumber { get; set; }
        public string FromStore { get; set; } // Add this
        public string ToStore { get; set; }   // Add this


    }

    // Approval Workflow DTOs
    public class ApprovalWorkflowDto : BaseDto
    {
        public string WorkflowName { get; set; }
        public string EntityType { get; set; }
        public string TriggerCondition { get; set; }
        public int StepOrder { get; set; }
        public string ApproverRole { get; set; }
        public string ApproverUserId { get; set; }
        public string ApproverUserName { get; set; }
        public bool IsRequired { get; set; }
        public decimal? ThresholdValue { get; set; }
        public string ThresholdField { get; set; }
        public string Name { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public List<WorkflowLevelDto> Levels { get; set; }
    }

    // Inventory Cycle Count DTOs
    public class InventoryCycleCountDto : BaseDto
    {
        public string CountNumber { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public DateTime CountDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string CountType { get; set; } = "Full";
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Notes { get; set; }
        public string CountedBy { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public int TotalItems { get; set; }
        public int CountedItems { get; set; }
        public int VarianceItems { get; set; }
        public decimal TotalVarianceValue { get; set; }

        public List<InventoryCycleCountItemDto> Items { get; set; } = new List<InventoryCycleCountItemDto>();
    }

    public class InventoryCycleCountItemDto : BaseDto
    {
        public int CycleCountId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal? SystemQuantity { get; set; }
        public decimal? CountedQuantity { get; set; }
        public decimal? Variance { get; set; }
        public string VarianceReason { get; set; }
        public string CountedById { get; set; }
        public string CountedByName { get; set; }
        public DateTime? CountedDate { get; set; }
        public bool IsRecounted { get; set; }
        public decimal VarianceQuantity { get; set; }
        public decimal VarianceValue { get; set; }
        public bool IsAdjusted { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public string AdjustedBy { get; set; }
        public string Comments { get; set; }
    }

    // Asset Tag DTOs
    public class AssetTagDto : BaseDto
    {
        public string TagNumber { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string SerialNumber { get; set; }
        public DateTime AssignedDate { get; set; }
        public string AssignedTo { get; set; }
        public string AssignmentType { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime? ReturnDate { get; set; }
        public string ReturnCondition { get; set; }
        public string Notes { get; set; }
    }

    // Temperature Log DTOs
    public class TemperatureLogDto : BaseDto
    {
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public DateTime LogTime { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public string Unit { get; set; } = "Celsius";
        public string Status { get; set; } = "Normal";
        public string RecordedBy { get; set; }
        public string Equipment { get; set; }
        public bool IsAlert { get; set; }
        public string AlertReason { get; set; }
    }

    // Expiry Tracking DTOs
    public class ExpiryTrackingDto : BaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Quantity { get; set; }
        public string Status { get; set; } = "Valid";
        public DateTime? DisposalDate { get; set; }
        public string DisposalReason { get; set; }
        public string DisposedBy { get; set; }
        public bool IsAlertSent { get; set; }
        public DateTime? AlertSentDate { get; set; }

        public string BatchNo { get; set; }
        public string Unit { get; set; }
        public int DaysToExpiry => (ExpiryDate - DateTime.Now).Days;
    }

    // Stock Alert DTOs
    public class StockAlertDto : BaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string AlertType { get; set; }
        public decimal? CurrentStock { get; set; }
        public decimal? MinimumStock { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime AlertDate { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedDate { get; set; }
        public decimal? CurrentQuantity { get; set; }
        public decimal? ThresholdQuantity { get; set; }
        public string Message { get; set; }
        public bool IsResolved { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string AlertLevel { get; set; }
        public string Unit { get; set; }
        public decimal? PercentageRemaining { get; set; }
        public StockAlertLevel Level { get; set; }
        public string LevelDisplay { get; set; }
        public string LevelColor { get; set; }
        public DateTime DetectedAt { get; set; }
        public bool IsAcknowledged { get; set; }
        public string Severity { get; set; }
        public decimal? ReorderLevel { get; set; }

        public string AlertMessage { get; set; }
    }

    // Stock Entry DTOs
    public class StockEntryDto : BaseDto
    {
        public string EntryNo { get; set; }
        public DateTime EntryDate { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string EntryType { get; set; } = "Direct";
        public string Remarks { get; set; }
        public string Status { get; set; } = "Draft";

        // Added missing property
        public int BarcodesGenerated { get; set; }

        public DateTime CreatedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Collections
        public List<StockEntryItemDto> Items { get; set; } = new List<StockEntryItemDto>();

        // Calculated Properties for display
        public int TotalItems => Items?.Count ?? 0;
        public decimal TotalQuantity => Items?.Sum(x => x.Quantity) ?? 0;
        public decimal TotalValue => Items?.Sum(x => x.Quantity * x.UnitCost) ?? 0;
    }

    public class StockEntryItemDto : BaseDto
    {
        public int StockEntryId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost => Quantity * UnitCost;
        public string Location { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool GenerateBarcodes { get; set; }
        public int BarcodesToGenerate { get; set; }
        public int BarcodesGenerated { get; set; }
        public List<string> GeneratedBarcodeNumbers { get; set; } = new List<string>();
        public string SubCategoryName { get; set; }
        public decimal? CurrentStock { get; set; }

    }

    // Stock Adjustment DTOs
    public class StockAdjustmentDto : BaseDto
    {
        public string AdjustmentNo { get; set; }

        [Required(ErrorMessage = "Adjustment date is required")]
        public DateTime AdjustmentDate { get; set; }

        [Required(ErrorMessage = "Store is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid store")]
        public int? StoreId { get; set; }

        public string StoreName { get; set; }
        public string AdjustmentType { get; set; } // Correction, Damage, Expiry, Loss

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Reason must be between 10 and 500 characters")]
        public string Reason { get; set; }

        public string Status { get; set; } // Pending, Approved, Rejected
        public DateTime CreatedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Calculated Properties
        public int TotalItems => Items?.Count ?? 0;
        public decimal TotalAdjustmentValue => Items?.Sum(x => Math.Abs(x.AdjustmentQuantity * x.UnitCost)) ?? 0;

        public string Remarks { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectionReason { get; set; }

        [Required(ErrorMessage = "Item is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid item")]
        public int ItemId { get; set; }

        public string ItemName { get; set; }
        public decimal? OldQuantity { get; set; }

        [Required(ErrorMessage = "New quantity is required")]
        [Range(0, double.MaxValue, ErrorMessage = "New quantity cannot be negative")]
        public decimal? NewQuantity { get; set; }

        public decimal? AdjustmentQuantity { get; set; }
        public bool IsApproved { get; set; }
        public string ReferenceDocument { get; set; }
        public string ItemCode { get; set; }

        public decimal? AdjustmentValue { get; set; }
        public string AdjustmentPercentage { get; set; }
        public string ReferenceNumber
        {
            get => ReferenceDocument;
            set => ReferenceDocument = value;
        }

        public decimal TotalValue
        {
            get => TotalAdjustmentValue;
            set { } // Read-only, calculated from Items
        }
        public List<StockAdjustmentItemDto> Items { get; set; } = new List<StockAdjustmentItemDto>();
    }

    public class StockAdjustmentItemDto : BaseDto
    {
        public int StockAdjustmentId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal AdjustmentQuantity { get; set; }
        public string AdjustmentType { get; set; }
        public string Reason { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue { get; set; }
        public string Notes { get; set; }
        public string Remarks { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal NewStock => CurrentStock + AdjustmentQuantity;
        public string AdjustmentReason { get; set; }
        public decimal Variance { get; set; }
        public decimal ActualQuantity { get; internal set; }
    }

    // Shipment Tracking DTOs
    public class ShipmentTrackingDto : BaseDto
    {
        public string ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public string TrackingCode { get; set; }
        public string QRCode { get; set; }
        public string Status { get; set; }
        public string LastLocation { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Carrier { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string TrackingUrl { get; set; }
    }

    // Activity Log DTOs
    public class ActivityLogDto : BaseDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public int? EntityId { get; set; }
        public string Description { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string UserAgent { get; set; }
        public string Entity { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime Timestamp { get; set; }
        public string EntityName { get; set; }
        public string Module { get; set; }
        public string Details { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Link { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IpAddress { get; set; }
        public string IPAddress { get; set; }
        public string FormattedTimestamp { get; set; }
        public string TimeAgo { get; set; }
        public string ActionIcon { get; set; }
        public string ActionColor { get; set; }
    }

    // Notification DTOs
    public class NotificationDto : BaseDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string Priority { get; set; } = "Normal";
        public string RelatedEntity { get; set; }
        public int? RelatedEntityId { get; set; }
        public string TargetRole { get; set; }

        public string TimeAgo { get; set; }  // Changed from { get; }
        public string Icon { get; set; }     // Changed from { get; }
        public string CssClass { get; set; } // Changed from { get; }

        public string TargetUserId { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string Category { get; internal set; }
        public string ReferenceId { get; internal set; }
        public string ActionUrl { get; internal set; }
        public string ReferenceType { get; set; }
        public string Metadata { get; set; }
    }

    // Setting DTOs
    public class SettingDto : BaseDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string DataType { get; set; } = "string";
        public bool IsReadOnly { get; set; }
    }

    // Usage Statistics DTOs
    public class UsageStatisticsDto : BaseDto
    {
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string MetricName { get; set; }
        public decimal MetricValue { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string Period { get; set; }
        public string CalculatedBy { get; set; }
        public DateTime CalculationDate { get; set; }
    }

    // System Configuration DTOs
    public class SystemConfigurationDto : BaseDto
    {
        public string ModuleName { get; set; }
        public string ConfigurationKey { get; set; }
        public string ConfigurationValue { get; set; }
        public string DataType { get; set; }
        public string Description { get; set; }
        public bool IsSystemLevel { get; set; }
        public bool RequiresRestart { get; set; }
        public string ValidValues { get; set; }
        public string DefaultValue { get; set; }
        public DateTime LastModified { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class RangeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Range name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Range name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [StringLength(20, ErrorMessage = "Range code cannot exceed 20 characters")]
        [RegularExpression(@"^[A-Z]{2,3}-R\d*$", ErrorMessage = "Code must be in format like 'DH-R' or 'DH-R1'")]
        public string Code { get; set; }

        [StringLength(200, ErrorMessage = "Headquarter location cannot exceed 200 characters")]
        public string HeadquarterLocation { get; set; }

        [StringLength(100, ErrorMessage = "Commander name cannot exceed 100 characters")]
        public string CommanderName { get; set; }

        [StringLength(50, ErrorMessage = "Commander rank cannot exceed 50 characters")]
        public string CommanderRank { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(50, ErrorMessage = "Contact number cannot exceed 50 characters")]
        public string ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; }

        [StringLength(500, ErrorMessage = "Coverage area cannot exceed 500 characters")]
        public string CoverageArea { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters")]
        public string Remarks { get; set; }

        // For display
        public int BattalionCount { get; set; } = 0;
        public int ZilaCount { get; set; } = 0;
        public int StoreCount { get; set; } = 0;
        public List<string> BattalionNames { get; set; } = new List<string>();

        // Audit
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class BattalionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Battalion name is required")]
        [StringLength(100, ErrorMessage = "Battalion name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Battalion code is required")]
        [StringLength(20, ErrorMessage = "Battalion code cannot exceed 20 characters")]
        [RegularExpression(@"^BN-[MF]-\d{2}$", ErrorMessage = "Code must be in format BN-M-01 or BN-F-01")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Battalion type is required")]
        public BattalionType Type { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(100)]
        public string CommanderName { get; set; }

        [StringLength(50)]
        public string CommanderRank { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(50)]
        public string ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string Email { get; set; }

        public int? RangeId { get; set; }
        public string RangeName { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string Remarks { get; set; }

        // Personnel Information
        public int TotalPersonnel { get; set; } = 0;
        public int OfficerCount { get; set; } = 0;
        public int EnlistedCount { get; set; } = 0;

        // Operational Information
        public DateTime? EstablishedDate { get; set; }
        public OperationalStatus? OperationalStatus { get; set; }

        // For display
        public int StoreCount { get; set; } = 0;
        public int PersonnelCount { get; set; } = 0;
        public int? ZilaId { get; set; }
        public string ZilaName { get; set; }
        public int UserCount { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class ZilaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "District name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "District code is required")]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(100)]
        public string NameBangla { get; set; }

        [Required(ErrorMessage = "Range is required")]
        public int RangeId { get; set; }
        public string RangeName { get; set; }

        [StringLength(100)]
        public string Division { get; set; }

        [StringLength(100)]
        public string DistrictOfficerName { get; set; }

        [Phone]
        [StringLength(50)]
        public string ContactNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(200)]
        public string OfficeAddress { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Area { get; set; }

        [Range(0, int.MaxValue)]
        public int? Population { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string Remarks { get; set; }

        // For display
        public int UpazilaCount { get; set; }
        public int StoreCount { get; set; }
        public int VDPMemberCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class UpazilaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Upazila name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Upazila code is required")]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(100)]
        public string NameBangla { get; set; }

        [Required(ErrorMessage = "District is required")]
        public int ZilaId { get; set; }
        public string ZilaName { get; set; }
        public string RangeName { get; set; }

        [StringLength(100)]
        public string UpazilaOfficerName { get; set; }

        [StringLength(50)]
        public string OfficerDesignation { get; set; }

        [Phone]
        [StringLength(50)]
        public string ContactNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(200)]
        public string OfficeAddress { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Area { get; set; }

        [Range(0, int.MaxValue)]
        public int? Population { get; set; }

        [Range(0, int.MaxValue)]
        public int? NumberOfUnions { get; set; }

        [Range(0, int.MaxValue)]
        public int? NumberOfVillages { get; set; }

        public bool HasVDPUnit { get; set; } = false;

        [Range(0, int.MaxValue)]
        public int? VDPMemberCount { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string Remarks { get; set; }

        // Additional fields
        [StringLength(100)]
        public string UpazilaChairmanName { get; set; }

        [StringLength(100)]
        public string VDPOfficerName { get; set; }

        // For display
        public int StoreCount { get; set; }
        public int UnionCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class UnionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Union name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Union code is required")]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(100)]
        public string NameBangla { get; set; }

        [Required(ErrorMessage = "Upazila is required")]
        public int UpazilaId { get; set; }
        public string UpazilaName { get; set; }

        [StringLength(100)]
        public string ChairmanName { get; set; }

        [StringLength(50)]
        public string ChairmanContact { get; set; }

        [StringLength(100)]
        public string SecretaryName { get; set; }

        [StringLength(50)]
        public string SecretaryContact { get; set; }

        [StringLength(100)]
        public string VDPOfficerName { get; set; }

        [StringLength(50)]
        public string VDPOfficerContact { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(200)]
        public string OfficeAddress { get; set; }

        public int? NumberOfWards { get; set; }
        public int? NumberOfVillages { get; set; }
        public int? NumberOfMouzas { get; set; }

        public decimal? Area { get; set; }
        public int? Population { get; set; }
        public int? NumberOfHouseholds { get; set; }

        public bool HasVDPUnit { get; set; } = false;

        public int? VDPMemberCountMale { get; set; }
        public int? VDPMemberCountFemale { get; set; }
        public int? AnsarMemberCount { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public bool IsRural { get; set; } = true;
        public bool IsBorderArea { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string Remarks { get; set; }

        // For display
        public int StoreCount { get; set; }
        public int TotalVDPMembers { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string ZilaName { get; internal set; }
        public string RangeName { get; internal set; }
    }

    public class LocationDto : BaseDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? ParentLocationId { get; set; }
        public string ParentLocationName { get; set; }
        public string LocationType { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
    }

    // User Store DTOs
    public class UserStoreDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? StoreId { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string AssignedBy { get; set; }
        public bool IsPrimary { get; set; } = false;
        public DateTime? UnassignedDate { get; set; }
        public DateTime AssignedAt { get; set; }

        // For display
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public bool IsActive { get; set; } = true;
        public string BattalionName { get; set; }
        public string RangeName { get; set; }
        public string FullName { get; set; }
        public string Notes { get; set; }

        public List<UserDto> Users { get; set; } = new List<UserDto>();
    }

    public class StoreUserDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "User is required")]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Store is required")]
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public bool IsPrimary { get; set; } = false;
        public DateTime AssignedDate { get; set; }
        public string AssignedBy { get; set; }
        public DateTime? UnassignedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string BattalionName { get; set; }
        public string RangeName { get; set; }
    }

    public class BattalionStoreDto
    {
        public int Id { get; set; }
        public int BattalionId { get; set; }
        public string BattalionName { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public bool IsPrimaryStore { get; set; } = false;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string BattalionCode { get; set; }
        public string StoreCode { get; set; }
        public string StoreLocation { get; set; }
    }

    public class StoreConfigurationDto
    {
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    // Quality Check DTOs
    public class QualityCheckDto : BaseDto
    {
        public string CheckNumber { get; set; }
        public DateTime CheckDate { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int PurchaseId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string CheckType { get; set; } = "Incoming";
        public string CheckedBy { get; set; }
        public string Status { get; set; } = "Pass";
        public string Comments { get; set; }
        public decimal CheckedQuantity { get; set; }
        public decimal PassedQuantity { get; set; }
        public decimal FailedQuantity { get; set; }
        public string FailureReasons { get; set; }
        public string CorrectiveActions { get; set; }
        public bool RequiresRetest { get; set; }
        public DateTime? RetestDate { get; set; }
        public List<QualityCheckItemDto> Items { get; set; }
    }

    public class DocumentDto : BaseDto
    {
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string DocumentType { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
    }

    // Supplier Evaluation DTOs
    public class SupplierEvaluationDto : BaseDto
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime EvaluationDate { get; set; }
        public string EvaluatedBy { get; set; }
        public string EvaluatedByName { get; set; }
        public decimal QualityRating { get; set; }
        public decimal DeliveryRating { get; set; }
        public decimal PriceRating { get; set; }
        public decimal ServiceRating { get; set; }
        public decimal OverallRating { get; set; }
        public string Comments { get; set; }
        public string Recommendations { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
    }

    public class SupplierPerformanceDto : BaseDto
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalOrders { get; set; }
        public int OnTimeDeliveries { get; set; }
        public decimal? OnTimePercentage { get; set; }
        public int QualityIssues { get; set; }
        public decimal? AverageLeadTime { get; set; }
        public decimal? TotalOrderValue { get; set; }
        public decimal? AverageOrderValue { get; set; }
        public int ReturnedItems { get; set; }
        public decimal? ReturnPercentage { get; set; }
        public string CalculatedBy { get; set; }
        public DateTime CalculationDate { get; set; }
    }

    // Contract DTOs
    public class ContractDto : BaseDto
    {
        public string ContractNumber { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string ContractType { get; set; } = "Purchase";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ContractValue { get; set; }
        public string Currency { get; set; } = "BDT";
        public string Status { get; set; } = "Draft";
        public string Terms { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string WarrantyTerms { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string FilePath { get; set; }

        public int DaysRemaining => (EndDate - DateTime.Now).Days;
    }

    // Item Specification DTOs
    public class ItemSpecificationDto : BaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string SpecificationName { get; set; }
        public string SpecificationValue { get; set; }
        public string Unit { get; set; }
        public string Category { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Warranty DTOs
    public class WarrantyDto : BaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string WarrantyNumber { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WarrantyType { get; set; }
        public string Terms { get; set; }
        public string ContactInfo { get; set; }
        public string Status { get; set; } = "Active";
        public decimal CoveredValue { get; set; }
        public string SerialNumber { get; set; }

        public int DaysRemaining => (EndDate - DateTime.Now).Days;
    }

    // Requisition DTOs
    public class RequisitionDto : BaseDto
    {
        public DateTime RequestDate { get; set; }
        public DateTime RequiredByDate { get; set; }
        public int? FromStoreId { get; set; }
        public string FromStoreName { get; set; }
        public int? ToStoreId { get; set; }
        public string ToStoreName { get; set; }
        public string FulfillmentStatus { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal ApprovedValue { get; set; }
        public bool AutoConvertToPO { get; set; }
        public int? PurchaseOrderId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string Level1ApprovedBy { get; set; }
        public DateTime? Level1ApprovedDate { get; set; }
        public string Level2ApprovedBy { get; set; }
        public DateTime? Level2ApprovedDate { get; set; }
        public string FinalApprovedBy { get; set; }
        public DateTime? FinalApprovedDate { get; set; }
        public int CurrentApprovalLevel { get; set; }
        public string RequisitionNumber { get; set; }
        public DateTime RequisitionDate { get; set; }
        public string RequestedBy { get; set; }
        public string RequestedByName { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Draft";
        public DateTime? RequiredDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovalComments { get; set; }
        public string RejectionReason { get; set; }
        public decimal TotalEstimatedCost { get; set; }
        public string Notes { get; set; }
        public List<RequisitionItemDto> Items { get; set; } = new List<RequisitionItemDto>();
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string PriorityClass
        {
            get
            {
                return Priority switch
                {
                    "Urgent" => "badge-danger",
                    "High" => "badge-warning",
                    "Normal" => "badge-primary",
                    "Low" => "badge-info",
                    _ => "badge-secondary"
                };
            }
        }
        public bool IsOverdue => RequiredByDate < DateTime.Now && FulfillmentStatus != "Complete";

    }

    public class RequisitionItemDto : BaseDto
    {
        public int RequisitionId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Unit { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public decimal EstimatedUnitPrice { get; set; }
        public decimal EstimatedTotalPrice { get; set; }
        public string Justification { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal? IssuedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Specification { get; set; }
    }

    // Compliance Check DTOs
    public class ComplianceCheckDto : BaseDto
    {
        public string CheckName { get; set; }
        public string CheckType { get; set; } = "Safety";
        public DateTime CheckDate { get; set; }
        public string CheckedBy { get; set; }
        public string Status { get; set; } = "Pending";
        public string RequirementReference { get; set; }
        public string Findings { get; set; }
        public string Recommendations { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ComplianceDate { get; set; }
        public bool IsRecurring { get; set; }
        public string Frequency { get; set; }
        public DateTime? NextCheckDate { get; set; }
    }

    // Approval Request DTOs
    public class ApprovalRequestDto : BaseDto
    {
        public string RequestType { get; set; }
        public int EntityId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Comments { get; set; }
        public string RejectionReason { get; set; }
        public string EntityData { get; set; }
        public decimal? ApprovalValue { get; set; }
        public string ApproverRole { get; set; }
        public decimal? Amount { get; set; }
        public DateTime RequestedDate { get; set; }
        public string EntityType { get; set; }
        public string ApprovalLevel { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevel { get; set; }
        public int WorkflowId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public virtual ICollection<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
    }

    public class ApprovalDto : BaseDto
    {
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string ApprovalType { get; set; }
        public string Status { get; set; } = "Pending";
        public string RequestedBy { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Comments { get; set; }
        public string RejectionReason { get; set; }
        public string EntityData { get; set; }
        public decimal? ApprovalValue { get; set; }
        public string ApproverRole { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestedDate { get; set; }
        public int Level { get; set; }
        public string Role { get; set; }
        public string RoleDescription { get; set; }
        public bool IsApproved { get; set; }
        public string Remarks { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> EntityDataDictionary { get; set; } = new Dictionary<string, object>();
    }

    // System Log DTOs
    public class SystemLogDto : BaseDto
    {
        public string Level { get; set; } = "Info";
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Source { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime LogDate { get; set; }
        public string AdditionalData { get; set; }
    }

    // File Upload DTOs
    public class FileUploadDto : BaseDto
    {
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadDate { get; set; }
    }

    // Report DTOs
    public class ReportDto : BaseDto
    {
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string Parameters { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; } = "Generating";
        public int RecordCount { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    // Alert DTOs
    public class AlertDto : BaseDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; } = "medium";
        public string Status { get; set; } = "active";
        public string ActionUrl { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ResolvedBy { get; set; }
        public int? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // User Session DTOs
    public class UserSessionDto : BaseDto
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime LastActivity { get; set; }
    }

    // Backup DTOs
    public class BackupDto : BaseDto
    {
        public string BackupName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string BackupType { get; set; } = "Full";
        public string Status { get; set; } = "InProgress";
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public int RecordCount { get; set; }
        public string InitiatedBy { get; set; }
    }

    // Email Log DTOs
    public class EmailLogDto : BaseDto
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? SentAt { get; set; }
        public string ErrorMessage { get; set; }
        public int RetryCount { get; set; }
        public string EmailType { get; set; }
        public string TriggeredBy { get; set; }
    }

    // Maintenance Schedule DTOs
    public class MaintenanceScheduleDto : BaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string MaintenanceType { get; set; } = "Preventive";
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public string CompletedBy { get; set; }
        public string CompletedByName { get; set; }
        public string Notes { get; set; }
        public decimal Cost { get; set; }
        public string Frequency { get; set; }
        public DateTime? NextDueDate { get; set; }

        public bool IsOverdue => ScheduledDate < DateTime.Now && Status != "Completed";
    }

    // Training Record DTOs
    public class TrainingRecordDto : BaseDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string TrainingType { get; set; }
        public string TrainingName { get; set; }
        public DateTime TrainingDate { get; set; }
        public string TrainerName { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; }
        public decimal? Score { get; set; }
        public bool IsCertified { get; set; }
        public DateTime? CertificationExpiry { get; set; }
        public string CertificateNumber { get; set; }
        public string Notes { get; set; }
    }

    // Audit DTOs
    public class AuditDto : BaseDto
    {
        public string Entity { get; set; }
        public int? EntityId { get; set; }
        public string Action { get; set; }
        public string Changes { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string EntityName { get; set; }
        public string IPAddress { get; set; }
    }

    // Role Permission DTOs
    public class RolePermissionDto : BaseDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public Permission Permission { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public bool IsGranted { get; set; } = true;
    }

    public class RolePermissionCreateDto
    {
        public string RoleId { get; set; }
        public List<Permission> Permissions { get; set; } = new List<Permission>();
    }

    public class RolePermissionUpdateDto
    {
        public string RoleId { get; set; }
        public Permission Permission { get; set; }
        public bool IsGranted { get; set; }
    }

    public class RoleWithPermissionsDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }

    public class PermissionDto
    {
        public Permission Permission { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool IsGranted { get; set; }
    }

    // Tracking History DTOs
    public class TrackingHistoryDto : BaseDto
    {
        public int ShipmentTrackingId { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Carrier { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string TrackingUrl { get; set; }
    }

    // Audit Log DTOs
    public class AuditLogDto : BaseDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalItems { get; set; }
        public decimal? TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int PendingOrders { get; set; }
        public int TodayTransactions { get; set; }
        public int TotalStores { get; set; }
        public int PendingIssues { get; set; }
        public int LowStockItems { get; set; }
        public int MonthlyPurchases { get; set; }
        public int MonthlyIssues { get; set; }
        public decimal? MonthlyPurchaseValue { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal? TotalInventoryValue { get; set; }
        public int TotalVendors { get; set; }
        public int TotalUsers { get; set; }
        public int PurchaseOrders { get; set; }
        public int PendingPurchases { get; set; }
        public int Issues { get; set; }
        public int OutOfStockItems { get; set; }
        public int TotalActiveItems { get; set; }
        public int ExpiredItems { get; set; }
        public int ExpiringIn7Days { get; set; }
        public int ExpiringIn30Days { get; set; }
        public int TotalPersonnel { get; set; }
        public int AnsarItems { get; set; }
        public int VDPItems { get; set; }
        public List<CategoryStockDto> CategoryStock { get; set; } = new List<CategoryStockDto>();

    }

    public class CategoryStockDto
    {
        public string CategoryName { get; set; }
        public int ItemCount { get; set; }
        public decimal? TotalValue { get; set; }
        public int CategoryId { get; set; }
        public decimal? TotalStock { get; set; }
    }

    public class InventoryMovementDto
    {
        public int Id { get; set; }
        public DateTime MovementDate { get; set; }
        public string MovementType { get; set; }
        public string ReferenceNo { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int? FromStoreId { get; set; }
        public string FromStoreName { get; set; }
        public int? ToStoreId { get; set; }
        public string ToStoreName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string MovedBy { get; set; }
        public string Remarks { get; set; }
    }

    public class StockLevelDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal? CurrentStock { get; set; }
        public decimal? AvailableStock { get; set; }
        public decimal? ReservedStock { get; set; }
        public decimal? MinimumStock { get; set; }
        public decimal? MaximumStock { get; set; }
        public decimal? ReorderLevel { get; set; }
        public string Status { get; set; }
        public string StockStatus { get; set; }
        public decimal? Percentage { get; set; }

        public string Unit { get; set; }
        public DateTime? LastRestockDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int? DaysUntilStockout { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsOutOfStock { get; set; }
        public string Location { get; set; }
        public decimal CurrentQuantity { get; set; } // Changed from { get; }

        public List<StoreStockInfo> StoreStocks { get; set; } = new List<StoreStockInfo>();
    }

    public class StoreStockDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int? StoreId { get; set; }
        public decimal? CurrentStock { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? MinimumStock { get; set; }
        public string StockStatus { get; set; }
        public string Status { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalValue { get; set; }
        public string Unit { get; set; }
        public DateTime? LastRestockDate { get; set; }
        public string CategoryName { get; set; }
        public string StoreName { get; set; }
        public decimal? MaximumStock { get; set; }
        public decimal? ReservedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
        public decimal? MinStockLevel { get; set; }
        public decimal? ReorderLevel { get; set; }
        public string Location { get; set; }


        public int Id { get; set; }
        public string StoreCode { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? LastIssueDate { get; internal set; }
        public DateTime? LastReceiveDate { get; internal set; }
        public string SubCategoryName { get; internal set; }
        public bool IsActive { get; internal set; }
    }

    public class StoreStockInfo
    {
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal? Quantity { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Value { get; set; }
    }

    // Search and Filter DTOs
    public class SearchFilterDto
    {
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? StoreId { get; set; }
        public ItemType? ItemType { get; set; }
        public ItemStatus? ItemStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinQuantity { get; set; }
        public decimal? MaxQuantity { get; set; }
        public bool? LowStock { get; set; }
        public string SortBy { get; set; } = "Name";
        public string SortOrder { get; set; } = "ASC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    // Bulk Operation DTOs
    public class BulkOperationDto
    {
        public List<int> ItemIds { get; set; } = new List<int>();
        public string Operation { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    // Additional DTOs
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
    }

    public class ApprovalHistoryDto
    {
        public string Action { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public string Comments { get; set; }
        public string ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public int Level { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }

        public int Id { get; set; }
        public string EntityType { get; set; }
        public string EntityReference { get; set; }
        public decimal Amount { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }
    }

    public class StoreUsersDto
    {
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string ManagerId { get; set; }
        public List<UserDto> AssignedUsers { get; set; } = new List<UserDto>();
        public List<UserDto> AvailableUsers { get; set; } = new List<UserDto>();
    }

    public class TransactionDto
    {
        public string Type { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    public class StoreSummaryDto
    {
        public int? StoreId { get; set; }
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public decimal? TotalValue { get; set; }
    }

    public class RecentActivityDto
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string User { get; set; }

        public string Icon { get; set; }
        public string Color { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string ActivityType { get; internal set; }
    }

    // Statistics DTOs
    public class RangeStatisticsDto
    {
        public int TotalBattalions { get; set; }
        public int MaleBattalions { get; set; }
        public int FemaleBattalions { get; set; }
        public int TotalZilas { get; set; }
        public int TotalStores { get; set; }
        public int TotalPersonnel { get; set; }
        public int ActiveBattalions { get; set; }
        public int InactiveBattalions { get; set; }
    }

    public class UpazilaStatisticsDto
    {
        public int TotalStores { get; set; }
        public int TotalUnions { get; set; }
        public int TotalVillages { get; set; }
        public int VDPMembers { get; set; }
        public int TotalIssues { get; set; }
        public int TotalReceives { get; set; }
        public decimal? TotalInventoryValue { get; set; }
        public List<MonthlyActivityDto> MonthlyActivities { get; set; } = new List<MonthlyActivityDto>();
    }

    public class UnionStatisticsDto
    {
        public int TotalStores { get; set; }
        public int TotalWards { get; set; }
        public int TotalVillages { get; set; }
        public int TotalMouzas { get; set; }
        public int TotalHouseholds { get; set; }
        public int Population { get; set; }
        public int VDPMembersMale { get; set; }
        public int VDPMembersFemale { get; set; }
        public int AnsarMembers { get; set; }
        public int TotalIssues { get; set; }
        public int TotalReceives { get; set; }
        public decimal? TotalInventoryValue { get; set; }
        public List<WardStatisticsDto> WardStatistics { get; set; } = new List<WardStatisticsDto>();
        public List<MonthlyActivityDto> MonthlyActivities { get; set; } = new List<MonthlyActivityDto>();
    }

    public class WardStatisticsDto
    {
        public int WardNumber { get; set; }
        public int Population { get; set; }
        public int VDPMembers { get; set; }
    }

    public class MonthlyActivityDto
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public int Issues { get; set; }
        public int IssueCount { get; set; }
        public int Receives { get; set; }
        public int ReceiveCount { get; set; }
        public decimal? Value { get; set; }
    }


    // Additional specialized DTOs
    public class PurchaseHistoryDto
    {
        public int PurchaseId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string VendorName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string StoreName { get; set; }
    }

    public class IssueHistoryDto
    {
        public int IssueId { get; set; }
        public string IssueNo { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuedTo { get; set; }
        public string IssuedToType { get; set; }
        public decimal? Quantity { get; set; }
        public string StoreName { get; set; }
        public string Purpose { get; set; }
    }

    public class TransferHistoryDto
    {
        public int TransferId { get; set; }
        public string TransferNo { get; set; }
        public DateTime TransferDate { get; set; }
        public string FromStoreName { get; set; }
        public string ToStoreName { get; set; }
        public decimal? Quantity { get; set; }
        public string Remarks { get; set; }
    }

    public class DamageHistoryDto
    {
        public int DamageId { get; set; }
        public string DamageNo { get; set; }
        public DateTime DamageDate { get; set; }
        public decimal? Quantity { get; set; }
        public string DamageType { get; set; }
        public string Cause { get; set; }
        public string ActionTaken { get; set; }
        public string StoreName { get; set; }
    }

    public class StoreDetailDto : StoreDto
    {
        public List<StoreStockDto> StockItems { get; set; } = new List<StoreStockDto>();
        public decimal? TotalValue { get; set; }
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public DateTime? LastStockUpdate { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = new List<RecentActivityDto>();
    }

    // Specialized DTOs for various operations
    public class IssueScanDto
    {
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string BarcodeNumber { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal? CurrentStock { get; set; }
        public string Unit { get; set; }
        public bool IsLowStock { get; set; }
        public decimal? MinimumStock { get; set; }
        public decimal? Quantity { get; set; }
    }

    public class ReceivePurchaseDto
    {
        public int PurchaseId { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string InvoiceNo { get; set; }
        public string ChallanNo { get; set; }
        public string ReceivedBy { get; set; }
        public string Remarks { get; set; }
        public List<ReceiveItemDto> Items { get; set; } = new List<ReceiveItemDto>();
    }

    public class TransferReceiptDto
    {
        public int TransferId { get; set; }
        public string ReceiptNo { get; set; }
        public string TransferNo { get; set; }
        public DateTime TransferDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string FromStore { get; set; }
        public string ToStore { get; set; }
        public List<TransferReceiptItemDto> Items { get; set; } = new List<TransferReceiptItemDto>();
    }

    public class TransferReceiptItemDto
    {
        public int ItemId { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public string Location { get; set; }
        public string Remarks { get; set; }
        public string ItemName { get; set; }
        public decimal? ShippedQuantity { get; set; }
        public decimal? Variance { get; set; }

    }

    public class ReturnRestockDto
    {
        public int ReturnId { get; set; }
        public List<ReturnRestockItemDto> Items { get; set; } = new List<ReturnRestockItemDto>();
    }

    public class ReturnRestockItemDto
    {
        public int ItemId { get; set; }
        public decimal? RestockQuantity { get; set; }
        public decimal? DamagedQuantity { get; set; }
        public string DamageType { get; set; }
        public string DamageDescription { get; set; }
        public decimal? EstimatedValue { get; set; }
    }

    public class DamageReportDto
    {
        public int Id { get; set; }
        public string ReportNo { get; set; }
        public int ReturnId { get; set; }
        public int ItemId { get; set; }
        public decimal? Quantity { get; set; }
        public string DamageType { get; set; }
        public string Description { get; set; }
        public decimal? EstimatedValue { get; set; }

        public int StoreId { get; set; }
        public DateTime ReportDate { get; set; }
        public string ReportedBy { get; set; }
        public DamageStatus Status { get; set; }
        public decimal TotalValue { get; set; }
        public List<DamageReportItemDto> Items { get; set; }
    }

    public class EmergencyRequestDto
    {
        public string IssuedToType { get; set; }
        public int? IssuedToBattalionId { get; set; }
        public int? IssuedToRangeId { get; set; }
        public int? IssuedToZilaId { get; set; }
        public int? IssuedToUpazilaId { get; set; }
        public string IssuedToIndividualName { get; set; }
        public string IssuedToIndividualBadgeNo { get; set; }
        public string Purpose { get; set; }
        public int? PreferredStoreId { get; set; }
        public List<EmergencyItemDto> Items { get; set; } = new List<EmergencyItemDto>();

        public string RequestNo { get; set; }
        public string Reason { get; set; }
        public int StoreId { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestDate { get; set; }
        public string VoucherDocumentPath { get; internal set; }
        public string MemoNo { get; internal set; }
        public DateTime? MemoDate { get; internal set; }
    }

    public class EmergencyItemDto
    {
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string Urgency { get; set; }
    }

    public class TrackingInfoDto
    {
        public string Type { get; set; }
        public int ReferenceId { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime Timestamp { get; set; }
        public string CurrentStatus { get; set; }
        public string LastLocation { get; set; }
        public List<TrackingHistoryDto> History { get; set; } = new List<TrackingHistoryDto>();
    }

    public class QuickReceiveItemDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? StoreId { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public string Condition { get; set; }
        public string Remarks { get; set; }
    }

    public class CreatePartialIssueDto
    {
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public int ParentIssueId { get; set; }
        public string IssueNo { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuedTo { get; set; }
        public string Purpose { get; set; }

        public Dictionary<int, decimal> ActualQuantities { get; set; } = new Dictionary<int, decimal>();
    }

    // System DTOs
    public class SystemHealthDto
    {
        public string Status { get; set; }
        public DateTime CheckTime { get; set; }
        public List<HealthCheckDto> Checks { get; set; } = new List<HealthCheckDto>();
        public Dictionary<string, object> SystemInfo { get; set; } = new Dictionary<string, object>();
    }

    public class HealthCheckDto
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }

    public class SystemConfigDto
    {
        public string OrganizationName { get; set; }
        public string OrganizationLogo { get; set; }
        public string TimeZone { get; set; }
        public string Currency { get; set; }
        public string DateFormat { get; set; }
        public string Language { get; set; }
        public bool AllowNegativeStock { get; set; }
        public bool RequireApprovalForWriteOff { get; set; }
        public decimal? ThresholdAmount { get; set; }
        public string Description { get; set; }
    }

    public class ExportDto
    {
        public string EntityType { get; set; }
        public string Format { get; set; }
        public SearchFilterDto Filter { get; set; }
        public List<string> Columns { get; set; } = new List<string>();
        public bool IncludeHeaders { get; set; } = true;
        public string FileName { get; set; }
    }

    public class ValidationErrorDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> FieldErrors { get; set; } = new Dictionary<string, string>();
        public string ErrorType { get; set; }
    }

    // Analytics DTOs
    public class MetricDto
    {
        public string Name { get; set; }
        public decimal? Value { get; set; }
        public decimal? PreviousValue { get; set; }
        public decimal? ChangePercentage { get; set; }
        public string Trend { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }

    public class TrendAnalysisDto
    {
        public string Category { get; set; }
        public List<DataPointDto> DataPoints { get; set; } = new List<DataPointDto>();
        public string TrendDirection { get; set; }
        public decimal? TrendStrength { get; set; }
        public List<string> Insights { get; set; } = new List<string>();
    }

    public class DataPointDto
    {
        public DateTime Date { get; set; }
        public decimal? Value { get; set; }
        public string Label { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    // Additional hierarchy DTOs
    public class RangeHierarchyDto
    {
        public RangeDto Range { get; set; }
        public IEnumerable<BattalionDto> Battalions { get; set; }
        public IEnumerable<ZilaDto> Zilas { get; set; }
        public RangeStatisticsDto Statistics { get; set; }
    }

    public class UnionSearchDto
    {
        public string SearchTerm { get; set; }
        public int? UpazilaId { get; set; }
        public int? ZilaId { get; set; }
        public int? RangeId { get; set; }
        public bool? HasVDPUnit { get; set; }
        public bool? IsRural { get; set; }
        public bool? IsBorderArea { get; set; }
        public bool? IsActive { get; set; }
    }

    public class StoreStatisticsDto
    {
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public int TotalItems { get; set; }
        public int CriticalItems { get; set; }
        public int LowStockItems { get; set; }
        public int HealthyItems { get; set; }
        public decimal? StockHealthPercentage { get; set; }
    }

    public class PendingActionDto
    {
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Priority { get; set; }
    }

    // Upload DTOs
    public class StockUploadItemDto
    {
        public int RowNumber { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Location { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? EstimatedValue { get; set; }
    }

    // Email Settings DTO (Note: EmailSettings entity doesn't inherit from BaseEntity)
    public class EmailSettingsDto
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool EnableSsl { get; set; }
        public int Timeout { get; set; } = 30000;
    }

    // Enums for DTOs
    public enum StockAlertLevel
    {
        Critical = 1,
        High = 2,
        Medium = 3,
        Normal = 4,
        Low = 5,
        Info = 6,

    }
    public class ApprovalThresholdDto
    {
        public int Id { get; set; }
        public decimal MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public int ApprovalLevel { get; set; }
        public string RequiredRole { get; set; }

        public string EntityType { get; set; }
        public decimal Amount { get; set; }
        public string ApproverRole { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        // Added missing properties
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Level { get; set; }
        public bool RequiresApproval { get; set; }
        public decimal ThresholdAmount { get; set; }
    }

    public class CycleCountDto : BaseDto
    {
        public string CountNumber { get; set; }
        public DateTime CountDate { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string CountType { get; set; }
        public string Status { get; set; }
        public string CountedBy { get; set; }
        public string CountedByName { get; set; }
        public string VerifiedBy { get; set; }
        public string VerifiedByName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public int TotalItems { get; set; }
        public int CountedItems { get; set; }
        public int VarianceItems { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public string Notes { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Added missing property
        public List<CycleCountItemDto> Items { get; set; } = new List<CycleCountItemDto>();
    }

    public class CycleCountItemDto : BaseDto
    {
        public int CycleCountId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public decimal? SystemQuantity { get; set; }
        public decimal? CountedQuantity { get; set; }
        public decimal VarianceQuantity { get; set; }
        public decimal VarianceValue { get; set; }
        public string VarianceReason { get; set; }
        public bool IsAdjusted { get; set; }
        public DateTime? AdjustmentDate { get; set; }
        public string AdjustedBy { get; set; }
        public decimal? Variance { get; set; }
        public string CountedById { get; set; }
        public string CountedByName { get; set; }
        public DateTime? CountedDate { get; set; }
        public bool IsRecounted { get; set; }
        public string Comments { get; set; }
    }

    public class DigitalSignatureDto
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Reference Type")]
        public string ReferenceType { get; set; }

        [Required]
        [Display(Name = "Reference ID")]
        public int ReferenceId { get; set; }

        [Required]
        [Display(Name = "Signature Type")]
        public string SignatureType { get; set; }

        [Display(Name = "Signed By")]
        public string SignedBy { get; set; }

        [Display(Name = "Signed At")]
        public DateTime SignedAt { get; set; }

        [Required]
        [Display(Name = "Signature")]
        public string SignatureData { get; set; }

        [Display(Name = "Device")]
        public string DeviceInfo { get; set; }

        [Display(Name = "IP Address")]
        public string IPAddress { get; set; }

        [Display(Name = "Location")]
        public string LocationInfo { get; set; }

        [Display(Name = "Verified")]
        public bool IsVerified { get; set; }

        [Display(Name = "Verified By")]
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }

        public string SignerName { get; set; }
        public string SignerBadgeNo { get; set; }
        public string SignerBadgeId { get; set; } // Added missing property
        public string SignerDesignation { get; set; }
        public DateTime SignatureDate { get; set; }
        public DateTime SignedDate { get; set; } // Added missing property
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; internal set; }
        public string DeviceId { get; internal set; }
        public string Location { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public string CreatedBy { get; internal set; }
    }

    public class ItemDetailDto : ItemDto
    {
        public List<WarrantyDto> Warranties { get; set; } = new List<WarrantyDto>();
        public List<MaintenanceScheduleDto> MaintenanceSchedules { get; set; } = new List<MaintenanceScheduleDto>();
    }

    public class StockReportDto
    {
        public DateTime GeneratedDate { get; set; }
        public string ReportType { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public List<StockReportItemDto> Items { get; set; } = new List<StockReportItemDto>();
        public List<StockReportItemDto> StockItems { get; set; } = new List<StockReportItemDto>(); // Added missing property
        public decimal? TotalValue { get; set; }
        public int TotalItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public string GeneratedBy { get; set; }
    }

    public class StockReportItemDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public decimal? CurrentStock { get; set; }
        public decimal? MinimumStock { get; set; }
        public decimal? MaximumStock { get; set; }
        public decimal? ReorderLevel { get; set; }
        public string Unit { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalValue { get; set; }
        public string StockStatus { get; set; }
        public DateTime? LastMovementDate { get; set; }
        public string StoreName { get; set; } // Added missing property
    }


    public class PersonalizedAlertDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActionLink { get; set; }
    }

    public class CountItemDto
    {
        public int InventoryId { get; set; }
        public int ItemId { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public string Notes { get; set; }
    }

    public class ScanCountDto
    {
        public int InventoryId { get; set; }
        public string Barcode { get; set; }
        public decimal Quantity { get; set; }
    }

    public class RecountDto
    {
        public int InventoryId { get; set; }
        public int ItemId { get; set; }
        public decimal RecountQuantity { get; set; }
    }

    public class PhysicalInventoryDto
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; }

        // Store & Organization
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int? BattalionId { get; set; }
        public string BattalionName { get; set; }
        public int? RangeId { get; set; }
        public string RangeName { get; set; }
        public int? ZilaId { get; set; }
        public string ZilaName { get; set; }
        public int? UpazilaId { get; set; }
        public string UpazilaName { get; set; }

        // Inventory Info
        public DateTime CountDate { get; set; }
        public string FiscalYear { get; set; }
        public PhysicalInventoryStatus Status { get; set; }
        public string StatusText { get; set; }
        public CountType CountType { get; set; }
        public string Remarks { get; set; }

        // Audit
        public bool IsAuditRequired { get; set; }
        public string AuditOfficer { get; set; }
        public string ApprovalReference { get; set; }
        public string ApprovalRemarks { get; set; }

        // Personnel
        public string InitiatedBy { get; set; }
        public DateTime? InitiatedDate { get; set; }
        public string CompletedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Totals
        public decimal TotalSystemQuantity { get; set; }
        public decimal TotalPhysicalQuantity { get; set; }
        public decimal TotalVariance { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public decimal TotalSystemValue { get; set; }
        public decimal TotalPhysicalValue { get; set; }

        // Collections
        public List<int> SelectedItemIds { get; set; }
        public List<PhysicalInventoryDetailDto> Details { get; set; }
        public List<ApprovalDto> ApprovalChain { get; set; }

        public PhysicalInventoryDto()
        {
            Details = new List<PhysicalInventoryDetailDto>();
            ApprovalChain = new List<ApprovalDto>();

        }

        public string CountedBy { get; set; }
        public string VerifiedBy { get; set; }

        // Compliance
        public string ComplianceStatus { get; set; }
        public List<string> AuditFindings { get; set; }
    }

    public class PhysicalInventoryItemDto
    {
        public int Id { get; set; }
        public int PhysicalInventoryId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemBarcode { get; set; }

        [Display(Name = "System Qty")]
        public decimal SystemQuantity { get; set; }

        [Display(Name = "Physical Qty")]
        [Required]
        public decimal PhysicalQuantity { get; set; }

        [Display(Name = "Variance")]
        public decimal Variance { get; set; }

        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }

        [Display(Name = "System Value")]
        public decimal SystemValue { get; set; }

        [Display(Name = "Physical Value")]
        public decimal PhysicalValue { get; set; }

        [Display(Name = "Variance Value")]
        public decimal VarianceValue { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Batch Number")]
        public string BatchNumber { get; set; }

        [Display(Name = "Counted At")]
        public DateTime? CountedAt { get; set; }

        [Display(Name = "Counted By")]
        public string CountedBy { get; set; }

        [Display(Name = "Is Recounted")]
        public bool IsRecounted { get; set; }

        [Display(Name = "Recount Qty")]
        public decimal? RecountQuantity { get; set; }

        [Display(Name = "Notes")]
        [MaxLength(500)]
        public string Notes { get; set; }

        [Display(Name = "Adjustment Status")]
        public string AdjustmentStatus { get; set; }

        // Computed properties
        public string VarianceClass => Variance < 0 ? "text-danger" : Variance > 0 ? "text-warning" : "text-success";
        public bool HasVariance => Variance != 0;
    }

    public class CycleCountScheduleDto
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Schedule Code")]
        public string ScheduleCode { get; set; }

        [Required]
        [Display(Name = "Schedule Name")]
        [MaxLength(100)]
        public string ScheduleName { get; set; }

        [Required]
        [Display(Name = "Store")]
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        [Required]
        [Display(Name = "Frequency")]
        public string Frequency { get; set; }

        [Required]
        [Display(Name = "Count Method")]
        public string CountMethod { get; set; }

        [Display(Name = "Next Scheduled")]
        public DateTime NextScheduledDate { get; set; }

        [Display(Name = "Last Executed")]
        public DateTime? LastExecutedDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "ABC Class")]
        public string ABCClass { get; set; }

        [Display(Name = "Min Value")]
        public decimal? MinimumValue { get; set; }

        [Display(Name = "Items Per Count")]
        public int? ItemsPerCount { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [Display(Name = "Notes")]
        [MaxLength(500)]
        public string Notes { get; set; }
    }
    public class QRCodeDto
    {
        public int QRCodeId { get; set; }
        public string VoucherType { get; set; }
        public int VoucherId { get; set; }
        public string VoucherNumber { get; set; }
        public string QRCodeData { get; set; }
        public string QRCodeImage { get; set; } // Base64 encoded image
        public DateTime GeneratedAt { get; set; }
        public Dictionary<string, object> DecodedData { get; set; }
    }
    public class BarcodeLabel
    {
        public string BarcodeNumber { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Size { get; set; } // Small, Medium, Large
        public string BarcodeImage { get; set; } // Base64 encoded
        public string QRCodeImage { get; set; } // Base64 encoded
        public Dictionary<string, string> CustomFields { get; set; }
    }
    public class BatchPrintRequestDto
    {
        [Required]
        public List<int> SelectedItemIds { get; set; } = new List<int>();

        [Range(1, 50, ErrorMessage = "Quantity must be between 1 and 50")]
        public int Quantity { get; set; } = 1;

        public string Format { get; set; } = "PDF"; // PDF, Labels

        public string Size { get; set; } = "Medium"; // Small, Medium, Large

        [Required]
        [Display(Name = "Label Size")]
        public string LabelSize { get; set; } // Small, Medium, Large

        [Display(Name = "Include QR Code")]
        public bool IncludeQRCode { get; set; }

        public string LabelFormat { get; set; }
        public int Copies { get; set; }

        public class BatchPrintItem
        {
            public int ItemId { get; set; }
            public string BatchNumber { get; set; }
            public int Quantity { get; set; }
        }
        public List<BatchPrintItemDto> Items { get; set; } = new List<BatchPrintItemDto>();
    }
    public class BatchPrintItemDto
    {
        [Required]
        public int ItemId { get; set; }
        public string ItemName { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Location { get; set; }
    }
    public class BatchPrintDto
    {
        public List<BarcodeLabel> Labels { get; set; }
        public int TotalLabels { get; set; }
        public DateTime PrintDate { get; set; }
        public string PrintedBy { get; set; }
        public byte[] PDFDocument { get; set; }
        public byte[] PdfBytes { get; set; }
        public string FileName { get; set; }
    }
    public class ScanResultDto
    {
        public bool Success { get; set; }
        public string Type { get; set; } // Barcode, ItemCode, QRCode
        public string BarcodeNumber { get; set; }
        public int? ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string BatchNumber { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public DateTime ScannedAt { get; set; } = DateTime.Now;
    }
    public class StockRotationDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Method { get; set; } // FIFO, LIFO, FEFO
        public List<StockRotationItemDto> SuggestedOrder { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
    public class StockRotationItemDto
    {
        public string BatchNumber { get; set; }
        public decimal Quantity { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Location { get; set; }
        public string Priority { get; set; } // High, Normal, Low
        public int DaysUntilExpiry => ExpiryDate.HasValue ?
            (ExpiryDate.Value - DateTime.Now).Days : int.MaxValue;
        public bool IsExpiringSoon => DaysUntilExpiry < 30;
    }
    public class BarcodeConfigurationDto
    {
        [Display(Name = "Barcode Format")]
        public string BarcodeFormat { get; set; } // CODE128, CODE39, QR

        [Display(Name = "Prefix")]
        public string Prefix { get; set; }

        [Display(Name = "Include Date")]
        public bool IncludeDate { get; set; }

        [Display(Name = "Include Batch")]
        public bool IncludeBatch { get; set; }

        [Display(Name = "Default Label Size")]
        public string DefaultLabelSize { get; set; }

        [Display(Name = "Auto Generate on Entry")]
        public bool AutoGenerateOnEntry { get; set; }

        [Display(Name = "Scanner Type")]
        public string ScannerType { get; set; } // USB, Bluetooth, Camera

        [Display(Name = "Offline Mode Enabled")]
        public bool OfflineModeEnabled { get; set; }

        [Display(Name = "Stock Rotation Method")]
        public string DefaultRotationMethod { get; set; } // FIFO, LIFO, FEFO
    }
    public class BatchDto
    {
        public int Id { get; set; }

        [Display(Name = "Batch Number")]
        public string BatchNumber { get; set; }

        [Required]
        [Display(Name = "Item")]
        public int ItemId { get; set; }
        public string ItemName { get; set; }

        [Required]
        [Display(Name = "Store")]
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "Initial Quantity")]
        public decimal InitialQuantity { get; set; }

        [Display(Name = "Manufacture Date")]
        [DataType(DataType.Date)]
        public DateTime? ManufactureDate { get; set; }

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Supplier Batch No")]
        public string SupplierBatchNo { get; set; }

        [Display(Name = "Cost Price")]
        public decimal CostPrice { get; set; }

        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        // Computed properties
        private int? _daysUntilExpiry;

        public int DaysUntilExpiry
        {
            get
            {
                if (_daysUntilExpiry.HasValue)
                    return _daysUntilExpiry.Value;

                return ExpiryDate.HasValue ?
                    (ExpiryDate.Value - DateTime.Now).Days : int.MaxValue;
            }
            set => _daysUntilExpiry = value;
        }

        public bool IsExpiring => DaysUntilExpiry <= 30;

        public bool IsExpired => DaysUntilExpiry < 0;

        public decimal UsedQuantity => InitialQuantity - Quantity;

        public decimal UsagePercentage => InitialQuantity > 0 ?
            (UsedQuantity / InitialQuantity * 100) : 0;
    }
    public class StockAllocationDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public string RotationMethod { get; set; }
        public List<BatchAllocationDto> Allocations { get; set; }
        public bool IsFullyAllocated { get; set; }
        public decimal ShortageQuantity { get; set; }
    }
    public class BatchAllocationDto
    {
        public int BatchId { get; set; }
        public string BatchNumber { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public decimal RemainingInBatch { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal CostPrice { get; set; }
        public string Location { get; set; }
        public string Priority { get; set; } // Critical, High, Medium, Normal
        public decimal AllocationValue => AllocatedQuantity * CostPrice;
    }
    public class BatchTraceabilityDto
    {
        public string BatchNumber { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal InitialQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SupplierBatchNo { get; set; }
        public List<BatchMovementDto> Movements { get; set; }
        public List<TransactionHistoryDto> IssueHistory { get; set; }
        public List<TransactionHistoryDto> ReceiveHistory { get; set; }
    }
    public class BatchMovementDto
    {
        public string MovementType { get; set; }
        public decimal Quantity { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime MovementDate { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public int BatchId { get; set; }
        public decimal BalanceBefore { get; set; }
        public string CreatedBy { get; set; }
        public decimal OldBalance { get; internal set; }
        public decimal NewBalance { get; internal set; }
    }

    public class TransactionHistoryDto
    {
        public string TransactionType { get; set; }
        public string TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal? Quantity { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
    }

    public class StockRotationReportDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string RotationMethod { get; set; }
        public DateTime GeneratedAt { get; set; }
        public List<ItemRotationAnalysisDto> ItemAnalysis { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalValue { get; set; }
        public int ItemsNeedingAttention { get; set; }
    }

    public class ItemRotationAnalysisDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int TotalBatches { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime? OldestBatchDate { get; set; }
        public int OldestBatchAge { get; set; } // Days
        public int ExpiringBatchCount { get; set; }
        public string RecommendedAction { get; set; }
    }
    public class CreateStoreDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string InCharge { get; set; }

        [StringLength(20)]
        public string ContactNumber { get; set; }

        public int? StoreTypeId { get; set; }
        public int? BattalionId { get; set; }
        public int? ZilaId { get; set; }
        public int? UpazilaId { get; set; }
        public decimal? Capacity { get; set; }
        public string OperatingHours { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AssignStoreKeeperDto
    {
        [Required]
        public int StoreId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public bool IsPrimary { get; set; } = false;
    }

    public class AddItemToStoreDto
    {
        [Required]
        public int StoreId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MinimumStock { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MaximumStock { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ReorderLevel { get; set; }

        public decimal InitialQuantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateStockLevelsDto
    {
        [Required]
        public int StoreId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MinimumStock { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MaximumStock { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ReorderLevel { get; set; }
    }

    public class AssignedUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string AssignedBy { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }

    public class AvailableUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
    }

    public class PhysicalInventoryDetailDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemNameBn { get; set; }
        public string ItemUnit { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Variance { get; set; }
        public decimal? VarianceValue { get; set; }

        public CountStatus Status { get; set; }
        public string StatusText { get; set; }
        public string CountedBy { get; set; }
        public DateTime? CountedDate { get; set; }

        public string LocationCode { get; set; }
        public string BatchNumbers { get; set; }
        public string Remarks { get; set; }
    }

    public class PhysicalCountUpdateDto
    {
        public int DetailId { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public string CountedBy { get; set; }
        public string LocationCode { get; set; }
        public string BatchNumbers { get; set; }
        public string Remarks { get; set; }
        public int ItemId { get; set; }
    }

    public class VarianceAnalysisDto
    {
        public int PhysicalInventoryId { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime CountDate { get; set; }
        public int InventoryId { get; set; }
        public int TotalItems { get; set; }
        public int ItemsWithOverage { get; set; }
        public int ItemsWithShortage { get; set; }
        public decimal TotalSystemValue { get; set; }
        public decimal TotalPhysicalValue { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public decimal OverageValue { get; set; }
        public decimal ShortageValue { get; set; }
        public decimal VariancePercentage { get; set; }
        public List<CategoryVarianceDto> VarianceByCategory { get; set; }
        public List<ItemVarianceDto> TopVarianceItems { get; set; }
        public bool RequiresApproval { get; set; }
        public List<string> RecommendedActions { get; set; }
        public int ItemsWithPositiveVariance { get; set; }
        public int ItemsWithNegativeVariance { get; set; }

        public decimal TotalPositiveVariance { get; set; }
        public decimal TotalNegativeVariance { get; set; }
        public decimal TotalSystemQuantity { get; set; }
        public decimal TotalPhysicalQuantity { get; set; }
        public decimal TotalVariance { get; set; }
        public List<VarianceDetailDto> VarianceDetails { get; set; }
        public List<VarianceItemDto> ItemsWithVariance { get; set; } = new List<VarianceItemDto>();
        public VarianceAnalysisDto()
        {
            VarianceDetails = new List<VarianceDetailDto>();
        }
    }

    public class VarianceDetailDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }

        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public decimal VarianceValue { get; set; }

        public string Remarks { get; set; }
    }

    public class PhysicalInventoryHistoryDto
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public DateTime CountDate { get; set; }
        public string FiscalYear { get; set; }
        public PhysicalInventoryStatus Status { get; set; }
        public CountType CountType { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalVariance { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }


    public class VarianceItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Variance { get; set; }
        public decimal VarianceValue { get; set; }
    }

    public class SystemActivityStatsDto
    {
        public int TotalActivities { get; internal set; }
        public int UniqueUsers { get; internal set; }
        public object TopActions { get; internal set; }
        public object TopEntities { get; internal set; }
        public Dictionary<int, int> HourlyDistribution { get; internal set; }
        public object DailyTrend { get; internal set; }
    }
    public class UserActivityStatsDto
    {
        public string UserId { get; set; }
        public int TotalActivities { get; set; }
        public Dictionary<string, int> ActivitiesByAction { get; set; }
        public Dictionary<string, int> ActivitiesByEntity { get; set; }
        public DateTime? FirstActivity { get; set; }
        public DateTime? LastActivity { get; set; }
        public int MostActiveHour { get; set; }
    }








    // ========== Store Management DTOs ==========
    public class StoreSetupStatusDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public bool IsStoreCreated { get; set; }
        public bool IsStoreKeeperAssigned { get; set; }
        public string StoreKeeperId { get; set; }
        public int ItemsCount { get; set; }
        public bool HasMinMaxLevels { get; set; }
        public DateTime? SetupCompletedDate { get; set; }
        public bool IsFullySetup { get; set; }
    }

    // ========== Purchase & Quality Check DTOs ==========
    public class ReceiveGoodsDto
    {
        public DateTime ReceiveDate { get; set; }
        public string ReceivedBy { get; set; }
        public string InvoiceNo { get; set; }
        public string ChallanNo { get; set; }
        public int VendorId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public List<ReceiveItemDto> Items { get; set; }
        public int PurchaseId { get; set; }
        public string Remarks { get; set; }
    }
    


    public class QualityCheckItemDto
    {
        public int ItemId { get; set; }
        public decimal CheckedQuantity { get; set; }
        public decimal PassedQuantity { get; set; }
        public decimal FailedQuantity { get; set; }
        public QualityCheckStatus Status { get; set; }
        public string Remarks { get; set; }
        public string CheckParameters { get; set; }
        public string ItemName { get; set; }

    }

    // ========== Issue & Handover DTOs ==========
    public class IssueApprovalDto
    {
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string Remarks { get; set; }
        public List<IssueApprovalItemDto> ApprovedItems { get; set; }
    }

    public class IssueApprovalItemDto
    {
        public int ItemId { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public string Remarks { get; set; }
    }

    public class IssueVoucherItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public string Unit { get; set; }
    }

    public class HandoverDto
    {
        public int IssueId { get; set; }
        public string IssueNo { get; set; }
        public string VoucherNo { get; set; }
        public string IssuedToName { get; set; }
        public string IssuedBy { get; set; }
        public string ReceivedBy { get; set; }

        // Receiver details
        public string ReceiverName { get; set; }
        public string ReceiverBadgeId { get; set; }
        public string ReceiverDesignation { get; set; }
        public string ReceiverContact { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverCode { get; set; }

        // Signature & handover details
        public string SignatureData { get; set; }
        public string ReceiverSignature { get; set; }
        public DateTime HandoverDate { get; set; }
        public string HandedOverBy { get; set; }
        public string HandoverNotes { get; set; }

        // Location & tracking
        public string Location { get; set; }
        public string IPAddress { get; set; }
        public string DeviceInfo { get; set; }

        public List<HandoverItemDto> HandoverItems { get; set; } = new List<HandoverItemDto>();
    }

    public class HandoverItemDto
    {
        public int ItemId { get; set; }
        public decimal IssuedQuantity { get; set; }
        public string Condition { get; set; }
        public string Remarks { get; set; }
    }

    public class IssueReceiptDto
    {
        public string ReceiptNo { get; set; }
        public string IssueNo { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime IssuedDate { get; set; }
        public string FromStore { get; set; }
        public string ToEntity { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceiverDesignation { get; set; }
        public List<IssueReceiptItemDto> Items { get; set; }
        public int SignatureId { get; set; }
        public string SignatureData { get; set; }
        public byte[] ReceiptPdf { get; set; }
    }

    public class IssueReceiptItemDto
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal IssuedQuantity { get; set; }
        public string Unit { get; set; }
        public string Condition { get; set; }
    }

    // ========== Return Flow DTOs ==========
    public class ReturnItemDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ReturnQuantity { get; set; }
        public string Condition { get; set; }
        public string ReturnReason { get; set; }
        public string BatchNo { get; set; }
        public string Remarks { get; set; }
    }

    public class ConditionCheckDto
    {
        public string CheckedBy { get; set; }
        public DateTime CheckedDate { get; set; }
        public string OverallCondition { get; set; }
        public List<ConditionCheckItemDto> Items { get; set; }
    }

    public class ConditionCheckItemDto
    {
        public int ItemId { get; set; }
        public decimal CheckedQuantity { get; set; }
        public decimal GoodQuantity { get; set; }
        public decimal DamagedQuantity { get; set; }
        public decimal ExpiredQuantity { get; set; }
        public string Condition { get; set; }
        public string Remarks { get; set; }
        public List<string> Photos { get; set; }
    }

    public class ReturnApprovalDto
    {
        public string ApprovedBy { get; set; }
        public string Remarks { get; set; }
        public List<ReturnApprovalItemDto> ApprovedItems { get; set; }
    }

    public class ReturnApprovalItemDto
    {
        public int ItemId { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public string Remarks { get; set; }
    }

    public class ReceiveReturnDto
    {
        public string ReceivedBy { get; set; }
        public string Remarks { get; set; }
        public List<ReceiveReturnItemDto> ReceivedItems { get; set; }
    }

    public class ReceiveReturnItemDto
    {
        public int ItemId { get; set; }
        public decimal ReceivedQuantity { get; set; }
    }

    public class ReturnReceiptDto
    {
        public string ReceiptNo { get; set; }
        public string ReturnNo { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public List<ReturnReceiptItemDto> Items { get; set; }
    }

    public class ReturnReceiptItemDto
    {
        public string ItemName { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public string Condition { get; set; }
    }

    public class DamageReportItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal DamagedQuantity { get; set; }
        public string DamageType { get; set; }
        public DateTime DamageDate { get; set; }
        public DateTime DiscoveredDate { get; set; }
        public string DamageDescription { get; set; }
        public decimal EstimatedValue { get; set; }
        public List<string> PhotoUrls { get; set; }
        public string BatchNo { get; set; }
        public string Remarks { get; set; }
    }

    public class WriteOffRequestDto
    {
        public int Id { get; set; }
        public string RequestNo { get; set; }
        public WriteOffStatus Status { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class WriteOffApprovalDto
    {
        public string ApprovedBy { get; set; }
        public string ApprovalLevel { get; set; }
        public string Remarks { get; set; }
        public string ApprovalReference { get; set; }
    }

    public class WriteOffExecutionDto
    {
        public string ExecutedBy { get; set; }
        public string DisposalMethod { get; set; }
        public DateTime DisposalDate { get; set; }
        public string DisposalLocation { get; set; }
        public string DisposalCompany { get; set; }
        public string CertificateNo { get; set; }
        public string WitnessedBy { get; set; }
        public List<string> PhotoUrls { get; set; }
        public string Remarks { get; set; }
    }

    public class DisposalCertificateDto
    {
        public string CertificateNo { get; set; }
        public string WriteOffRequestNo { get; set; }
        public DateTime DisposalDate { get; set; }
        public string DisposalMethod { get; set; }
        public decimal TotalValue { get; set; }
        public string ExecutedBy { get; set; }
        public string WitnessedBy { get; set; }
        public List<DisposalItemDto> Items { get; set; }
    }

    public class DisposalItemDto
    {
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
    }

    public class ExpiryAlertDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string BatchNo { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysToExpiry { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? EstimatedValue { get; set; }
        public string AlertLevel { get; set; }
    }


    public class ApprovalActionDto
    {
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovalLevel { get; set; }
        public string Remarks { get; set; }
    }

    public class BatchSignatureDto
    {
        public int Id { get; set; }
        public string BatchNo { get; set; }
        public string SignedBy { get; set; }
        public DateTime SignedDate { get; set; }
        public string Purpose { get; set; }
        public string SignatureData { get; set; }
        public string IPAddress { get; set; }
        public string DeviceInfo { get; set; }
        public List<BatchSignatureItemDto> Items { get; set; }
    }

    public class BatchSignatureItemDto
    {
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string Remarks { get; set; }
    }

    public class SignatureAuditTrailDto
    {
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public int TotalSignatures { get; set; }
        public int VerifiedSignatures { get; set; }
        public List<SignatureTimelineDto> Timeline { get; set; }
        public DateTime? FirstSignedDate { get; set; }
        public DateTime? LastSignedDate { get; set; }
    }

    public class SignatureTimelineDto
    {
        public int SignatureId { get; set; }
        public string SignedBy { get; set; }
        public DateTime SignedDate { get; set; }
        public string SignatureType { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string IPAddress { get; set; }
        public string DeviceInfo { get; set; }
    }

    public class StockReportRequestDto
    {
        public int? StoreId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public bool IncludeInactive { get; set; }
        public string ReportFormat { get; set; }
    }

    public class BatchTrackingDto : BaseDto
    {
        public string BatchNo { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public DateTime ReceivedDate { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string VendorId { get; set; }
        public string VendorBatchNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public decimal? UnitCost { get; set; }
        public string Status { get; set; }
        public string QualityCheckStatus { get; set; }
        public DateTime? QualityCheckDate { get; set; }
        public string QualityCheckBy { get; set; }
        public string StorageLocation { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal SuggestedQuantity { get; internal set; }
        public int DaysToExpiry { get; internal set; }
    }

    public class StockDto : BaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal MaximumStock { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
        public decimal? ReservedStock { get; set; }
        public decimal? AvailableStock { get; set; }
        public string ItemNameBn { get; internal set; }
        public string CategoryName { get; internal set; }
        public string SubCategoryName { get; internal set; }
        public string BrandName { get; internal set; }
        public decimal CurrentQuantity { get; internal set; }
        public decimal ReservedQuantity { get; internal set; }
        public decimal AvailableQuantity { get; internal set; }
        public decimal ReorderLevel { get; internal set; }
        public decimal UnitPrice { get; internal set; }
        public decimal TotalValue { get; internal set; }
        public DateTime LastCountDate { get; internal set; }
        public string Location { get; internal set; }
        public string StockStatus { get; internal set; }
        public string BatchNo { get; set; }
        public decimal Quantity { get; set; }
        public DateTime? LastIssueDate { get; set; }
        public DateTime? LastReceiveDate { get; set; }
    }

    // New DTO: AuditReportRequestDto
    public class AuditReportRequestDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EntityType { get; set; }
        public int? StoreId { get; set; }
        public string UserId { get; set; }
        public string ActionType { get; set; }
    }

    // New DTO: MovementReportRequestDto
    public class MovementReportRequestDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? ItemId { get; set; }
        public int? StoreId { get; set; }
        public string MovementType { get; set; }
    }
    public class StockSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    // ========== NEW REPORT DTOs ==========

    // Consumption Analysis Report DTOs
    public class ConsumptionAnalysisReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<ConsumptionItemDto> Items { get; set; } = new List<ConsumptionItemDto>();
        public decimal TotalConsumptionValue { get; set; }
        public int TotalItemsConsumed { get; set; }
        public List<CategoryConsumptionDto> CategoryBreakdown { get; set; } = new List<CategoryConsumptionDto>();
        public List<StoreConsumptionDto> StoreBreakdown { get; set; } = new List<StoreConsumptionDto>();
        public List<MonthlyConsumptionDto> MonthlyTrend { get; set; } = new List<MonthlyConsumptionDto>();
    }

    public class ConsumptionItemDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Unit { get; set; }
        public decimal AverageConsumption { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AverageMonthlyConsumption { get; set; }
        public int ConsumptionCount { get; set; }

        // Computed property for view compatibility
        public decimal AverageUnitPrice => TotalQuantity > 0 ? TotalValue / TotalQuantity : 0;
    }

    public class CategoryConsumptionDto
    {
        public string CategoryName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int ItemCount { get; set; }
    }

    public class StoreConsumptionDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int IssueCount { get; set; }
    }

    public class MonthlyConsumptionDto
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int IssueCount { get; set; }
    }

    // Expiry Report DTOs
    public class ExpiryReportDto
    {
        public DateTime AsOfDate { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public List<ExpiryItemDto> ExpiredItems { get; set; } = new List<ExpiryItemDto>();
        public List<ExpiryItemDto> ExpiringItems { get; set; } = new List<ExpiryItemDto>();
        public int TotalExpiredItems { get; set; }
        public int TotalExpiringItems { get; set; }
        public decimal TotalExpiredValue { get; set; }
        public decimal TotalExpiringValue { get; set; }
    }

    public class ExpiryItemDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysToExpiry { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal Value { get; set; }
        public string StoreName { get; set; }
        public string CategoryName { get; set; }
        public string Status { get; set; } // Expired, ExpiringInWeek, ExpiringInMonth, etc.
    }

    // Audit Trail Report DTOs
    public class AuditTrailReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<AuditTrailItemDto> AuditItems { get; set; } = new List<AuditTrailItemDto>();
        public int TotalTransactions { get; set; }
        public Dictionary<string, int> TransactionTypeBreakdown { get; set; } = new Dictionary<string, int>();

        // Alias property for view compatibility
        public List<AuditTrailItemDto> Transactions => AuditItems;
    }

    public class AuditTrailItemDto
    {
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string ReferenceNo { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string StoreName { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string PerformedBy { get; set; }
        public string Action { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Remarks { get; set; }

        // Alias properties for view compatibility
        public string ReferenceNumber => ReferenceNo;
        public string UserName => PerformedBy;
    }

    // ABC Analysis Report DTOs
    public class ABCAnalysisReportDto
    {
        public DateTime AnalysisDate { get; set; }
        public string AnalysisMethod { get; set; } // Value, Quantity, Movement
        public int Months { get; set; } = 12; // Analysis period in months (default 12 for annual)
        public List<ABCItemDto> ClassAItems { get; set; } = new List<ABCItemDto>();
        public List<ABCItemDto> ClassBItems { get; set; } = new List<ABCItemDto>();
        public List<ABCItemDto> ClassCItems { get; set; } = new List<ABCItemDto>();
        public ABCClassSummaryDto ClassASummary { get; set; }
        public ABCClassSummaryDto ClassBSummary { get; set; }
        public ABCClassSummaryDto ClassCSummary { get; set; }
    }

    public class ABCItemDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public decimal AnnualConsumptionValue { get; set; }
        public decimal AnnualConsumptionQuantity { get; set; }
        public decimal PercentageOfTotalValue { get; set; }
        public decimal CumulativePercentage { get; set; }
        public string ABCClass { get; set; } // A, B, or C
        public string Unit { get; set; }
        public int MovementFrequency { get; set; }
        public decimal CurrentStock { get; set; }

        // Computed properties for view compatibility
        public decimal AnalysisValue => AnnualConsumptionValue;
        public decimal PercentageContribution => PercentageOfTotalValue;
    }

    public class ABCClassSummaryDto
    {
        public string ClassName { get; set; } // A, B, or C
        public int ItemCount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PercentageOfTotalValue { get; set; }
        public decimal PercentageOfTotalItems { get; set; }
    }

    // Store Recent Transaction DTO
    public class StoreRecentTransactionDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public StockMovementType Type { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string Direction { get; set; } // "In" or "Out"
        public string ReferenceNumber { get; set; }
        public string User { get; set; }
    }

}