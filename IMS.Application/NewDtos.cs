using IMS.Application.DTOs;
using IMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.DTOs
{
    public class TemperatureStatisticsDto
    {
        public decimal AverageTemperature { get; set; }
        public decimal MinTemperature { get; set; }
        public decimal MaxTemperature { get; set; }
        public decimal AverageHumidity { get; set; }
        public int TotalReadings { get; set; }
        public int AlertCount { get; set; }
        public decimal ComplianceRate { get; set; }
    }

    // Expiry ViewModels
    public class ExpiryStatisticsDto
    {
        public int TotalItems { get; set; }
        public int ExpiredItems { get; set; }
        public int ExpiringIn7Days { get; set; }
        public int ExpiringIn30Days { get; set; }
        public decimal TotalValue { get; set; }
    }

    // Cycle Count ViewModels
    public class CycleCountStatisticsDto
    {
        public int TotalCycleCounts { get; set; }
        public int CompletedCounts { get; set; }
        public int PendingCounts { get; set; }
        public decimal AverageAccuracy { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public int TotalItemsCounted { get; set; }
        public Dictionary<string, int> CountsByType { get; set; }
        public List<CycleCountTrendDto> Trends { get; set; }
        public int TotalCounts { get; set; }
        public int InProgressCounts { get; set; }
        public double AverageCountTime { get; set; }
        public double AverageVarianceItems { get; set; }
    }

    public class ItemValuationDto
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal? TotalValue { get; set; }
        public string Method { get; set; }
    }

    public class ValuationSummaryDto
    {
        public decimal? CurrentValue { get; set; }
        public decimal LastMonthValue { get; set; }
        public decimal? ValueChange { get; set; }
        public decimal? PercentageChange { get; set; }
        public int TotalItems { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ChartDataDto
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
        public decimal? Amount { get; set; }
        public int Count { get; internal set; }
    }
    public class StoreInventorySummary
    {
        public int TotalItems { get; set; }
        public decimal? TotalValue { get; set; }
        public int OutOfStockItems { get; set; }
        public int LowStockItems { get; set; }
        public List<CategorySummary> Categories { get; set; }
    }

    public class CategorySummary
    {
        public string CategoryName { get; set; }
        public int ItemCount { get; set; }
        public decimal? TotalValue { get; set; }
    }
    public class ActionStatDto
    {
        public string Action { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
    // Physical Count DTOs
    public class ScheduleCountDto
    {
        public int StoreId { get; set; }
        public DateTime CountDate { get; set; }
        public CountType CountType { get; set; }
        public bool FreezeStock { get; set; }
        public string InitiatedBy { get; set; }
        public int? BattalionId { get; set; }
        public int? RangeId { get; set; }
        public int? ZilaId { get; set; }
        public int? UpazilaId { get; set; }
        public string CountTeam { get; set; }
        public string SupervisorId { get; set; }
        public string Remarks { get; set; }
        public List<int> SelectedItemIds { get; set; }
    }

    public class CountRecordDto
    {
        public string CountedBy { get; set; }
        public bool IsBlindCount { get; set; }
        public List<CountedItemDto> CountedItems { get; set; }
    }

    public class CountedItemDto
    {
        public int ItemId { get; set; }
        public decimal CountedQuantity { get; set; }
        public string Location { get; set; }
        public string Remarks { get; set; }
    }

    public class CategoryVarianceDto
    {
        public string CategoryName { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalVariance { get; set; }
        public decimal? VariancePercentage { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Variance { get; set; }
        public decimal VarianceValue { get; set; }
    }

    public class ItemVarianceDto : BaseDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Variance { get; set; }
        public decimal VariancePercentage { get; set; }
        public decimal VarianceValue { get; set; }
        public string Reason { get; set; }
        public VarianceType VarianceType { get; set; }
    }

    public class AdjustmentCreationDto
    {
        public string CreatedBy { get; set; }
        public string Reason { get; set; }
        public List<ItemReasonDto> ItemReasons { get; set; }
    }

    public class ItemReasonDto
    {
        public int ItemId { get; set; }
        public string Reason { get; set; }
    }

    // Transfer DTOs
    public class TransferApprovalDto
    {
        public string ApprovedBy { get; set; }
        public string Remarks { get; set; }
        public List<TransferApprovalItemDto> ApprovedItems { get; set; }
    }

    public class TransferApprovalItemDto
    {
        public int ItemId { get; set; }
        public decimal ApprovedQuantity { get; set; }
    }

    public class ShipmentDto
    {
        public DateTime ShippedDate { get; set; }
        public string ShippedBy { get; set; }
        public string PackingListNo { get; set; }
        public string TransportCompany { get; set; }
        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string DriverContact { get; set; }
        public string SealNo { get; set; }
        public DateTime EstimatedArrival { get; set; }
        public List<ShippedItemDto> ShippedItems { get; set; }
    }

    public class ShippedItemDto
    {
        public int ItemId { get; set; }
        public decimal ShippedQuantity { get; set; }
        public int PackageCount { get; set; }
        public string PackageDetails { get; set; }
        public string BatchNo { get; set; }
        public string Condition { get; set; }
    }

    public class ReceiveTransferDto
    {
        public DateTime ReceivedDate { get; set; }
        public string ReceivedBy { get; set; }
        public string ScannedQRCode { get; set; }
        public string ReceiverSignature { get; set; }
        public string OverallCondition { get; set; }
        public string Remarks { get; set; }
        public List<ReceivedTransferItemDto> ReceivedItems { get; set; }
    }

    public class ReceivedTransferItemDto
    {
        public int ItemId { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public string Condition { get; set; }
        public string DiscrepancyReason { get; set; }
        public string Remarks { get; set; }
    }

    public class TransferTrackingDto
    {
        public string TransferNo { get; set; }
        public string Status { get; set; } // Change to string
        public string FromStore { get; set; }
        public string ToStore { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string VehicleNo { get; set; }
        public string DriverContact { get; set; }
        public List<TransferTimelineDto> Timeline { get; set; }
    }

    public class TransferTimelineDto
    {
        public string Event { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    // Batch Tracking DTOs
    public class TransferBatchDto
    {
        public int SourceBatchId { get; set; }
        public int DestinationStoreId { get; set; }
        public decimal TransferQuantity { get; set; }
        public string TransferReference { get; set; }
        public string TransferredBy { get; set; }
    }

    public class ExpiringBatchDto
    {
        public int BatchId { get; set; }
        public string BatchNo { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysToExpiry { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public decimal? EstimatedValue { get; set; }
        public string StorageLocation { get; set; }
        public string AlertLevel { get; set; }
        public string RecommendedAction { get; set; }
    }

    public class BatchHistoryDto
    {
        public string BatchNo { get; set; }
        public string ItemName { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal TotalRemaining { get; set; }
        public List<BatchLocationDto> Locations { get; set; }
        public List<BatchMovementHistoryDto> Movements { get; set; }
    }

    public class BatchLocationDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal Quantity { get; set; }
        public string Status { get; set; }
        public string StorageLocation { get; set; }
    }

    public class BatchMovementHistoryDto
    {
        public DateTime MovementDate { get; set; }
        public string MovementType { get; set; }
        public decimal Quantity { get; set; }
        public decimal BalanceAfter { get; set; }
        public string ReferenceNo { get; set; }
        public string CreatedBy { get; set; }
        public decimal NewBalance { get; internal set; }
    }

    public class BatchValuationDto
    {
        public int StoreId { get; set; }
        public DateTime ValuationDate { get; set; }
        public int TotalBatches { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal? FIFOValue { get; set; }
        public decimal? LIFOValue { get; set; }
        public decimal? WeightedAverageValue { get; set; }
        public List<CategoryBatchValuationDto> CategoryValuation { get; set; }
        public BatchAgingAnalysisDto AgingAnalysis { get; set; }
    }

    public class CategoryBatchValuationDto
    {
        public string CategoryName { get; set; }
        public int BatchCount { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class BatchAgingAnalysisDto
    {
        public int Under30Days { get; set; }
        public int Between30And60Days { get; set; }
        public int Between60And90Days { get; set; }
        public int Over90Days { get; set; }
    }

    public class StockAnalysisDto
    {
        public int StoreId { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalValue { get; set; }
        public int InStockItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public int OverstockItems { get; set; }
        public decimal LowStockValue { get; set; }
        public decimal OverstockValue { get; set; }
        public List<ItemMovementDto> FastMovingItems { get; set; }
        public List<ItemMovementDto> SlowMovingItems { get; set; }
        public List<ReorderSuggestionDto> ItemsToReorder { get; set; }
        public DateTime AnalysisDate { get; set; }
    }

    public class ItemMovementDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int MovementCount { get; set; }
        public decimal TotalQuantity { get; set; }
    }

    public class ReorderSuggestionDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal SuggestedQuantity { get; set; }
        public decimal EstimatedCost { get; set; }
    }

    public class EmailDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public List<string> Attachments { get; set; }
    }

    public class SmsDto
    {
        public string To { get; set; }
        public string Message { get; set; }
        public bool Priority { get; set; }
    }

    public class BatchNotificationDto
    {
        public List<string> UserIds { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
    }

    public class PendingApprovalDto
    {
        public int ApprovalRequestId { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Priority { get; set; }
        public int Level { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsEscalated { get; set; }

        public int Id { get; set; }
        public string Type { get; set; } // Add this property
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
    }

    public class DelegationDto
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
    }

    public class WorkflowLevelDto
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string ApproverRole { get; set; }
        public string SpecificApproverId { get; set; }
        public decimal? ThresholdAmount { get; set; }
        public bool CanEscalate { get; set; }
        public int? TimeoutHours { get; set; }
    }
    public class ExpiringItemDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; }
        public string ItemName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysRemaining { get; set; }
        public int Quantity { get; set; }
        public string StoreName { get; set; }
    }

    public class ActivityDto
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
        public string ActionUrl { get; set; }
        public string Link => ActionUrl; // Alias for view compatibility
        public DateTime ActivityDate { get; set; }
        public DateTime Timestamp => ActivityDate; // Alias for view compatibility
    }
    public class OrganizationStatsDto
    {
        public int RangeCount { get; set; }
        public int BattalionCount { get; set; }
        public int ZilaCount { get; set; }
        public int UpazilaCount { get; set; }
        public int UnionCount { get; set; }
    }


    public class BatchGenerateDto
    {
        public bool Selected { get; set; }

        [Required]
        public int ItemId { get; set; }

        public string ItemName { get; set; }

        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; } = 1;
    }

    public class BatchPrintRequest
    {
        public List<int> SelectedItemIds { get; set; }
        public int Quantity { get; set; }
    }

    public class ValidateBarcodeRequest
    {
        public string BarcodeNumber { get; set; }
    }

    public class ScanRequest
    {
        public string Barcode { get; set; }
        public string Action { get; set; }
        public decimal? Quantity { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public string DeviceId { get; set; }
        public int? StoreId { get; set; }
        public int? FromStoreId { get; set; }
        public int? ToStoreId { get; set; }
    }
    public class StoreDistributionDto
    {
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreCode { get; set; }
        public decimal? CurrentStock { get; set; }
        public decimal? MinimumStock { get; set; }
        public decimal? MaximumStock { get; set; }
        public string Status { get; set; }
    }
    // DTO for barcode validation results
    public class BarcodeValidationDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public BarcodeDto Barcode { get; set; }
        public string ValidationErrors { get; set; }
    }

    // DTO for individual barcode scan logs
    public class BarcodeScanLogDto
    {
        public int Id { get; set; }
        public string BarcodeNumber { get; set; }
        public string Action { get; set; }
        public int Quantity { get; set; }
        public DateTime ScanDateTime { get; set; }
        public string ScannedBy { get; set; }
        public string StoreName { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public string DeviceInfo { get; set; }
    }

    // DTO for barcode label printing configurations
    public class BarcodeLabelConfigDto
    {
        public string LabelSize { get; set; } = "Medium"; // Small, Medium, Large
        public string Format { get; set; } = "Standard"; // Standard, Compact, Detailed
        public bool IncludeQRCode { get; set; } = true;
        public bool IncludeItemName { get; set; } = true;
        public bool IncludeSerialNumber { get; set; } = true;
        public bool IncludePrice { get; set; } = false;
        public bool IncludeBarcode { get; set; } = true;
        public int LabelsPerRow { get; set; } = 2;
        public string PaperSize { get; set; } = "A4"; // A4, Letter, Label
    }

    // DTO for bulk barcode operations
    public class BulkBarcodeOperationDto
    {
        public List<int> BarcodeIds { get; set; } = new List<int>();
        public string Operation { get; set; } // "Print", "Export", "Delete", "Archive"
        public string Format { get; set; } = "PDF";
        public BarcodeLabelConfigDto LabelConfig { get; set; } = new BarcodeLabelConfigDto();
        public string ExportPath { get; set; }
        public bool IncludeHeaders { get; set; } = true;
    }

    // DTO for barcode import/export
    public class BarcodeImportDto
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BarcodeNumber { get; set; }
        public string SerialNumber { get; set; }
        public int Quantity { get; set; } = 1;
        public string BatchNumber { get; set; }
        public string Location { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Notes { get; set; }
    }

    // DTO for barcode template configuration
    public class BarcodeTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Format { get; set; } // CODE128, QR, PDF417
        public string Size { get; set; } // Small, Medium, Large
        public bool IncludeText { get; set; } = true;
        public bool IncludeQRCode { get; set; } = false;
        public string Layout { get; set; } = "Standard";
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }

    // DTO for barcode verification and quality check
    public class BarcodeQualityCheckDto
    {
        public string BarcodeNumber { get; set; }
        public bool IsReadable { get; set; }
        public int QualityScore { get; set; } // 0-100
        public string QualityIssues { get; set; }
        public byte[] BarcodeImage { get; set; }
        public string Recommendations { get; set; }
        public DateTime CheckedAt { get; set; }
        public string CheckedBy { get; set; }
    }

    public class RecordScanDto
    {
        public int BarcodeId { get; set; }
        public string Location { get; set; }
        public string Action { get; set; } = "Manual";
        public string Notes { get; set; }
    }

    public class BarcodeFilterDto
    {
        public string Search { get; set; }
        public int? StoreId { get; set; }
        public string BarcodeType { get; set; }
        public string ReferenceType { get; set; }
        public string ScanStatus { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? CategoryId { get; set; }
        public string Location { get; set; }
        public string SortBy { get; set; } = "GeneratedDate";
        public string Order { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class StockUpdateDto
    {
        public int StoreId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string Type { get; set; } // "IN" or "OUT"
        public string Reference { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string UpdatedBy { get; set; }

    }
    public class ReceivePurchaseItemDto
    {
        public int ItemId { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal AcceptedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }



    public class StockMovementSummaryDto
    {
        public DateTime Date { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal TotalInQuantity { get; set; }
        public decimal TotalOutQuantity { get; set; }
        public decimal TotalInValue { get; set; }
        public decimal TotalOutValue { get; set; }
        public int TransferInCount { get; set; }
        public int TransferOutCount { get; set; }
        public int AdjustmentCount { get; set; }
        public int ReturnCount { get; set; }
        public int WriteOffCount { get; set; }
        public int TotalMovements { get; set; }
        public List<MovementTypeStatDto> MovementTypeStats { get; set; }
    }

    public class MovementTypeStatDto
    {
        public string MovementType { get; set; }
        public int Count { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class StockMovementTrendDto
    {
        public DateTime Date { get; set; }
        public string Period { get; set; }
        public decimal InQuantity { get; set; }
        public decimal OutQuantity { get; set; }
        public decimal NetMovement { get; set; }
        public decimal ClosingBalance { get; set; }
    }

    public class StockCardDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<StockCardEntryDto> Entries { get; set; }
    }

    public class StockCardEntryDto
    {
        public DateTime Date { get; set; }
        public string DocumentNo { get; set; }
        public string Description { get; set; }
        public decimal InQuantity { get; set; }
        public decimal OutQuantity { get; set; }
        public decimal Balance { get; set; }
        public string Remarks { get; set; }
    }

    public class StockLedgerDto
    {
        public DateTime Date { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string TransactionType { get; set; }
        public string DocumentNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        public decimal RunningBalance { get; set; }
        public string StoreName { get; set; }
    }

    // Stock Reconciliation DTOs

    public class ReconciliationVarianceDto
    {
        public int ReconciliationId { get; set; }
        public string ReconciliationNo { get; set; }
        public int TotalItems { get; set; }
        public int ItemsWithVariance { get; set; }
        public int ItemsMatched { get; set; }
        public decimal TotalPositiveVariance { get; set; }
        public decimal TotalNegativeVariance { get; set; }
        public decimal NetVariance { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public decimal VariancePercentage { get; set; }
        public List<VarianceByReasonDto> VarianceByReason { get; set; }
        public List<ItemVarianceDto> TopVarianceItems { get; set; }
    }

    public class VarianceByReasonDto
    {
        public string Reason { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalVariance { get; set; }
        public decimal TotalValue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ReconciliationSummaryDto
    {
        public int Id { get; set; }
        public string ReconciliationNo { get; set; }
        public DateTime ReconciliationDate { get; set; }
        public string StoreName { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }
        public int ReconciledItems { get; set; }
        public int PendingItems { get; set; }
        public decimal SystemValue { get; set; }
        public decimal PhysicalValue { get; set; }
        public decimal VarianceValue { get; set; }
        public decimal AccuracyPercentage { get; set; }
        public string InitiatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class ReconciliationStatisticsDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalReconciliations { get; set; }
        public int PendingReconciliations { get; set; }
        public int CompletedReconciliations { get; set; }
        public int ApprovedReconciliations { get; set; }
        public int RejectedReconciliations { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public decimal AverageAccuracy { get; set; }
        public int TotalAdjustmentsCreated { get; set; }
        public List<ReconciliationTrendDto> Trends { get; set; }
    }

    public class ReconciliationTrendDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public decimal VarianceValue { get; set; }
        public decimal AccuracyPercentage { get; set; }
    }

    public class ReconciliationHistoryDto
    {
        public int Id { get; set; }
        public string ReconciliationNo { get; set; }
        public DateTime Date { get; set; }
        public string StoreName { get; set; }
        public string Status { get; set; }
        public decimal VarianceValue { get; set; }
        public string InitiatedBy { get; set; }
    }

    public class CycleCountTrendDto
    {
        public DateTime Date { get; set; }
        public int CountsPerformed { get; set; }
        public decimal AccuracyPercentage { get; set; }
        public decimal VarianceValue { get; set; }
    }
    public class AllotmentLetterDto : BaseDto
    {
        public string AllotmentNo { get; set; }
        public DateTime AllotmentDate { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }

        public string IssuedTo { get; set; }
        public string IssuedToType { get; set; }

        public int? IssuedToBattalionId { get; set; }
        public string IssuedToBattalionName { get; set; }

        public int? IssuedToRangeId { get; set; }
        public string IssuedToRangeName { get; set; }

        public int? IssuedToZilaId { get; set; }
        public string IssuedToZilaName { get; set; }

        public int? IssuedToUpazilaId { get; set; }
        public string IssuedToUpazilaName { get; set; }

        public int FromStoreId { get; set; }
        public string FromStoreName { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; }

        public string ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public string DocumentPath { get; set; }
        public string ReferenceNo { get; set; }

        // Bengali/Government Format Fields
        public string Subject { get; set; } // বিষয়
        public string SubjectBn { get; set; } // বিষয় (Bengali)
        public string BodyText { get; set; } // Main instructions
        public string BodyTextBn { get; set; } // Main instructions (Bengali)
        public DateTime? CollectionDeadline { get; set; } // Collection deadline

        [Required(ErrorMessage = "Signatory Name is required")]
        public string SignatoryName { get; set; } // Name of person signing

        [Required(ErrorMessage = "Signatory Designation (English) is required")]
        public string SignatoryDesignation { get; set; } // Designation (e.g., উপপরিচালক (ভান্ডার))

        [Required(ErrorMessage = "Signatory Designation (Bengali) is required")]
        public string SignatoryDesignationBn { get; set; }

        [Required(ErrorMessage = "Signatory ID is required")]
        public string SignatoryId { get; set; } // Staff ID (e.g., বিএমভি-১২০২১৮)

        [Required(ErrorMessage = "Signatory Phone is required")]
        public string SignatoryPhone { get; set; }

        [Required(ErrorMessage = "Signatory Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string SignatoryEmail { get; set; }

        public string BengaliDate { get; set; } // ভাদ্র ১৪৩২ বঙ্গাব্দ

        public List<AllotmentLetterItemDto> Items { get; set; } = new();

        // NEW: Multiple recipients support
        public List<AllotmentLetterRecipientDto> Recipients { get; set; } = new();

        // Computed
        public int TotalItems => Items?.Count ?? 0;
        public decimal TotalQuantity => Items?.Sum(i => i.AllottedQuantity) ?? 0;
        public decimal TotalIssued => Items?.Sum(i => i.IssuedQuantity) ?? 0;
        public decimal TotalRemaining => Items?.Sum(i => i.RemainingQuantity) ?? 0;
        public bool IsExpired => ValidUntil < DateTime.Now;
        public new bool IsActive => Status == "Active" && !IsExpired;
        public bool CanIssue => IsActive && TotalRemaining > 0;

        // NEW: Total recipients count
        public int TotalRecipients => Recipients?.Count ?? 0;

        public string Remarks { get; internal set; }
        public string RejectedBy { get; internal set; }
        public DateTime? RejectedDate { get; internal set; }
        public string RejectionReason { get; internal set; }
    }

    public class AllotmentLetterItemDto : BaseDto
    {
        public int AllotmentLetterId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemNameBn { get; set; } // Bengali name
        public string CategoryName { get; set; }
        public decimal AllottedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string Unit { get; set; }
        public string UnitBn { get; set; } // Bengali unit (টি, পিস, etc)
        public string Remarks { get; set; }

        public decimal AvailableStock { get; set; }
    }

    // NEW: DTO for multiple recipients
    public class AllotmentLetterRecipientDto : BaseDto
    {
        public int AllotmentLetterId { get; set; }
        public string RecipientType { get; set; } // "Range", "Battalion", "Zila", "Upazila", "Union"

        public int? RangeId { get; set; }
        public int? BattalionId { get; set; }
        public int? ZilaId { get; set; }
        public int? UpazilaId { get; set; }
        public int? UnionId { get; set; }

        public string RecipientName { get; set; }
        public string RecipientNameBn { get; set; } // Bengali name
        public string Remarks { get; set; }
        public int SerialNo { get; set; }
        public string StaffStrength { get; set; } // কর্মরত জনবল (optional)

        // Items for this recipient
        public List<AllotmentLetterRecipientItemDto> Items { get; set; } = new();

        // Computed
        public decimal TotalQuantity => Items?.Sum(i => i.AllottedQuantity) ?? 0;
    }

    // NEW: DTO for recipient items
    public class AllotmentLetterRecipientItemDto : BaseDto
    {
        public int AllotmentLetterRecipientId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemNameBn { get; set; } // Bengali name
        public string CategoryName { get; set; }
        public decimal AllottedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingQuantity => AllottedQuantity - IssuedQuantity;
        public string Unit { get; set; }
        public string UnitBn { get; set; } // Bengali unit
        public string Remarks { get; set; }
        public decimal AvailableStock { get; set; }
    }
}
