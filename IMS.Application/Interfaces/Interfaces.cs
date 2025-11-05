using IMS.Application.DTOs;
using IMS.Application.Helpers;
using IMS.Application.Services;
using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using ApprovalLevel = IMS.Domain.Entities.ApprovalLevel;
using Range = IMS.Domain.Entities.Range;
using ValidationResult = IMS.Application.Services.ValidationResult;

namespace IMS.Application.Interfaces
{
    // Base Repository Interface
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int? id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate = null);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);

        Task<T> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate,
           string[] includes = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        Task<IEnumerable<T>> GetAllAsync(
            System.Linq.Expressions.Expression<Func<T, bool>> predicate = null,
            string[] includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        void Delete(T entity);
        IQueryable<T> Query();
        IQueryable<T> GetQueryable();
        Task<T> GetLastAsync(Expression<Func<T, bool>> predicate);
    }

    // Unit of Work Interface
    public interface IUnitOfWork : IDisposable
    {
        // Existing repositories
        IRepository<Category> Categories { get; }
        IRepository<SubCategory> SubCategories { get; }
        IRepository<Brand> Brands { get; }
        IRepository<ItemModel> ItemModels { get; }
        IRepository<Item> Items { get; }
        IRepository<Store> Stores { get; }
        IRepository<StoreItem> StoreItems { get; }
        IRepository<LedgerBook> LedgerBooks { get; }
        IRepository<Vendor> Vendors { get; }
        IRepository<Purchase> Purchases { get; }
        IRepository<PurchaseItem> PurchaseItems { get; }
        IRepository<Issue> Issues { get; }
        IRepository<IssueItem> IssueItems { get; }
        IRepository<Receive> Receives { get; }
        IRepository<ReceiveItem> ReceiveItems { get; }
        IRepository<Transfer> Transfers { get; }
        IRepository<TransferItem> TransferItems { get; }
        IRepository<WriteOff> WriteOffs { get; }
        IRepository<WriteOffItem> WriteOffItems { get; }
        IRepository<Damage> Damages { get; }
        IRepository<Return> Returns { get; }
        IRepository<Barcode> Barcodes { get; }
        IRepository<LoginLog> LoginLogs { get; }
        IRepository<ActivityLog> ActivityLogs { get; }
        IRepository<StockAdjustment> StockAdjustments { get; }
        IRepository<Notification> Notifications { get; }
        IRepository<Setting> Settings { get; }
        IRepository<Audit> Audits { get; }
        IRepository<RolePermission> RolePermissions { get; }
        IRepository<UserStore> UserStores { get; }
        IRepository<Battalion> Battalions { get; }
        IRepository<Range> Ranges { get; }
        IRepository<Zila> Zilas { get; }
        IRepository<Upazila> Upazilas { get; }
        IRepository<StoreConfiguration> StoreConfigurations { get; }
        IRepository<BattalionStore> BattalionStores { get; }
        IRepository<User> Users { get; }
        IRepository<StoreType> StoreTypes { get; }
        IRepository<StoreTypeCategory> StoreTypeCategories { get; }
        IRepository<ApprovalRequest> ApprovalRequests { get; }
        IRepository<Union> Unions { get; }
        IRepository<StockOperation> StockOperations { get; }
        IRepository<IssueVoucher> IssueVouchers { get; }
        IRepository<StockReturn> StockReturns { get; }
        IRepository<StockAlert> StockAlerts { get; }  // Add this
        IRepository<StockMovement> StockMovements { get; }
        IRepository<ExpiryTracking> ExpiryTrackings { get; }
        IRepository<StockEntry> StockEntries { get; }
        IRepository<StockEntryItem> StockEntryItems { get; }
        IRepository<StockAdjustmentItem> StockAdjustmentItems { get; }
        IRepository<ShipmentTracking> ShipmentTrackings { get; }
        IRepository<TrackingHistory> TrackingHistories { get; }
        IRepository<AuditLog> AuditLogs { get; }
        IRepository<InventoryCycleCount> InventoryCycleCounts { get; }
        IRepository<InventoryCycleCountItem> InventoryCycleCountItems { get; }
        IRepository<InventoryValuation> InventoryValuations { get; }
        IRepository<PhysicalInventory> PhysicalInventories { get; }
        IRepository<PhysicalInventoryItem> PhysicalInventoryItems { get; }
        //IRepository<StockReconciliation> StockReconciliations { get; }
        //IRepository<StockReconciliationItem> StockReconciliationItems { get; }
        IRepository<CycleCountSchedule> CycleCountSchedules { get; }
        IRepository<DigitalSignature> DigitalSignatures { get; }
        IRepository<BatchTracking> BatchTrackings { get; }
        IRepository<BatchMovement> BatchMovements { get; }
        IRepository<Requisition> Requisitions { get; }
        IRepository<RequisitionItem> RequisitionItems { get; }
        IRepository<ApprovalThreshold> ApprovalThresholds { get; }
        IRepository<ApprovalHistory> ApprovalHistories { get; }
        IRepository<StoreStock> StoreStocks { get; }
        IRepository<AuditReport> AuditReports { get; }
        IRepository<PurchaseOrder> PurchaseOrders { get; }
        IRepository<PhysicalInventoryDetail> PhysicalInventoryDetails { get; }
        IRepository<PurchaseOrderItem> PurchaseOrderItems { get; }
        IRepository<ApprovalStep> ApprovalSteps { get; }
        IRepository<ApprovalWorkflow> ApprovalWorkflows { get; }
        IRepository<ApprovalDelegation> ApprovalDelegations { get; }
        IRepository<WriteOffRequest> WriteOffRequests { get; }
        IRepository<UserNotificationPreferences> UserNotificationPreferences { get; }
        IRepository<TransferShipment> TransferShipments { get; }
        IRepository<TransferDiscrepancy> TransferDiscrepancies { get; }
        IRepository<DamageReport> DamageReports { get; }
        IRepository<DamageReportItem> DamageReportItems { get; }
        IRepository<DamageRecord> DamageRecords { get; }
        IRepository<ExpiredRecord> ExpiredRecords { get; }
        IRepository<DisposalRecord> DisposalRecords { get; }
        IRepository<ConditionCheck> ConditionChecks { get; }
        IRepository<SignatureOTP> SignatureOTPs { get; }
        IRepository<BatchSignature> BatchSignatures { get; }
        IRepository<Signature> Signatures { get; }
        IRepository<PersonnelItemIssue> PersonnelItemIssues { get; }
        IRepository<AllotmentLetter> AllotmentLetters { get; }
        IRepository<AllotmentLetterItem> AllotmentLetterItems { get; }

        // NEW: Multiple recipients support
        IRepository<AllotmentLetterRecipient> AllotmentLetterRecipients { get; }
        IRepository<AllotmentLetterRecipientItem> AllotmentLetterRecipientItems { get; }
        IRepository<SignatoryPreset> SignatoryPresets { get; }

        IRepository<Document> Documents { get; }

        // Transaction methods
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync();
    }

    // Category Service Interface
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
        Task UpdateCategoryAsync(CategoryDto categoryDto);
        Task DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(string name, int? excludeId = null);
        Task<IEnumerable<CategoryDto>> GetCategoriesWithSubCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
        Task<int> GetCategoryItemCountAsync(int categoryId);
    }

    // SubCategory Service Interface
    public interface ISubCategoryService
    {
        Task<IEnumerable<SubCategoryDto>> GetAllSubCategoriesAsync();
        Task<IEnumerable<SubCategoryDto>> GetSubCategoriesByCategoryIdAsync(int categoryId);
        Task<SubCategoryDto> GetSubCategoryByIdAsync(int id);
        Task<SubCategoryDto> CreateSubCategoryAsync(SubCategoryDto subCategoryDto);
        Task UpdateSubCategoryAsync(SubCategoryDto subCategoryDto);
        Task DeleteSubCategoryAsync(int id);
        Task<bool> SubCategoryExistsAsync(string name, int categoryId, int? excludeId = null);
        Task<IEnumerable<SubCategoryDto>> GetActiveSubCategoriesAsync();
        Task<int> GetSubCategoryItemCountAsync(int subCategoryId);
    }

    // Item Service Interface
    public interface IItemService
    {
        Task<IEnumerable<ItemDto>> GetAllItemsAsync();
        Task<ItemDto> GetItemByIdAsync(int id);
        Task<ItemDto> CreateItemAsync(ItemDto itemDto);
        Task UpdateItemAsync(ItemDto itemDto);
        Task DeleteItemAsync(int id);
        Task<string> GenerateItemCodeAsync(int subCategoryId);
        Task<IEnumerable<ItemDto>> GetLowStockItemsAsync();
        Task<IEnumerable<ItemDto>> GetItemsByCategoryAsync(int categoryId);
        Task<IEnumerable<ItemDto>> GetItemsBySubCategoryAsync(int subCategoryId);
        Task<IEnumerable<ItemDto>> GetItemsByStoreAsync(int storeId);
        Task<IEnumerable<ItemDto>> SearchItemsAsync(string searchTerm);
        Task<bool> ItemCodeExistsAsync(string itemCode, int? excludeId = null);
        Task<decimal> GetTotalStockValueAsync();
        Task<IEnumerable<ItemDto>> GetPagedItemsAsync(int pageNumber, int pageSize, string searchTerm = null);
        Task<IEnumerable<ItemDto>> GetExpendableItemsAsync();
        Task<IEnumerable<ItemDto>> GetNonExpendableItemsAsync();
        Task<ItemDto> GetItemByCodeAsync(string itemCode);
        Task<PagedResult<ItemDto>> GetPagedItemsAsync(PaginationParams paginationParams);

        Task<bool> CanDeleteItemAsync(int id);
        Task<ItemDetailDto> GetItemDetailAsync(int id);
        Task<PagedResult<ItemDto>> SearchItemsAsync(string searchTerm, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

        // Add these dashboard methods
        Task<int> GetTotalItemsCount();
        Task<int> GetTotalStoresCount();
        Task<List<ExpiryTrackingDto>> GetExpiringItems(int daysToExpiry);
        Task<List<CategoryStockDto>> GetCategoryStocks();
        Task<List<MonthlyTrendDto>> GetItemDistributionByCategory();
        Task<bool> GetItemByBarcodeAsync(string barcode);

        Task<int> GetItemCountByCategoryAsync(int categoryId);
        Task<List<ItemDto>> GetCriticalStockItemsAsync();
        Task<int> GetLowStockCountAsync();
        Task<IEnumerable<ItemDto>> GetActiveItemsAsync();
        Task<SelectList> GetControlledItemsSelectListAsync();
        Task<int> BulkUpdateItemsBanglaAndStockAsync();

    }

    public interface IStoreService
    {
        Task<bool> AssignStoreKeeperAsync(int storeId, string userId);
        Task<bool> RemoveStoreKeeperAsync(int storeId);
        Task<bool> SetStockLevelsAsync(int storeId, int itemId, decimal minStock, decimal maxStock, decimal reorderLevel);
        Task<UserDto> GetStoreKeeperAsync(int storeId);
        Task<IEnumerable<StoreItemDto>> CheckLowStockAsync(int storeId);
        Task<StoreSetupStatusDto> GetStoreSetupStatusAsync(int storeId);
        Task<(int successCount, List<string> errors)> AddUsersToStoreBatchAsync(int storeId, List<string> userIds);
        Task<IEnumerable<StoreDto>> GetAllStoresAsync();
        Task<StoreDto> GetStoreByIdAsync(int id);
        Task<StoreDto> CreateStoreAsync(StoreDto storeDto);
        Task UpdateStoreAsync(StoreDto storeDto);
        Task DeleteStoreAsync(int id);

        Task<decimal> GetStoreItemQuantityAsync(int? storeId, int itemId);
        Task<StockLevelDto> GetStockLevelAsync(int itemId, int? storeId);
        Task<StoreDto> GetStoreByIdAsync(int? id);
        Task<IEnumerable<StoreDto>> GetActiveStoresAsync();
        Task<bool> StoreNameExistsAsync(string name, int? excludeId = null);
        Task<decimal> GetStoreTotalValueAsync(int? storeId);
        Task<int> GetStoreItemCountAsync(int? storeId);
        Task<bool> StoreExistsAsync(string name, int? excludeId = null);
        Task<IEnumerable<StoreDto>> GetStoresByTypeAsync(string storeType);
        Task<IEnumerable<StoreDto>> GetStoresByLocationAsync(string location);
        Task<IEnumerable<StoreDto>> GetPagedStoresAsync(int pageNumber, int pageSize);
        Task<int> GetStoreCountAsync();
        Task<IEnumerable<StoreDto>> SearchStoresAsync(string searchTerm);

        // User-Store management
        Task<IEnumerable<StoreDto>> GetStoresByUserAsync(string userId);
        Task<bool> AssignUserToStoreAsync(string userId, int? storeId);
        Task RemoveUserFromStoreAsync(string userId, int? storeId);
        Task<bool> UserHasAccessToStoreAsync(string userId, int? storeId);

        // Hierarchy queries
        Task<IEnumerable<StoreDto>> GetStoresByBattalionAsync(int battalionId);
        Task<IEnumerable<StoreDto>> GetStoresByRangeAsync(int rangeId);
        Task<IEnumerable<StoreDto>> GetStoresByZilaAsync(int zilaId);
        Task<IEnumerable<StoreDto>> GetStoresByUpazilaAsync(int upazilaId);
        Task<IEnumerable<StoreDto>> GetStoresByLevelAsync(StoreLevel level);
        Task<string> GenerateStoreCodeAsync();
        Task<Dictionary<string, int>> GetStoreStatisticsAsync(int? storeId);
        Task<bool> CanDeleteStoreAsync(int? storeId);
        Task UpdateStoreCapacityAsync(int? storeId, decimal usedCapacity);
        Task<IEnumerable<StoreDto>> GetLowCapacityStoresAsync(decimal thresholdPercentage = 80);

        // Import/Export
        Task<ImportResultDto> ImportStoresFromCsvAsync(Stream csvStream, string userId);
        Task<byte[]> ExportStoresToCsvAsync();

        // Stock management
        Task<IEnumerable<StoreStockDto>> GetStoreStockAsync(int? storeId);
        Task<IEnumerable<StoreStockDto>> GetLowStockItemsByStoreAsync(int? storeId);
        Task<IEnumerable<StockLevelDto>> GetStoreStockLevelsAsync(int? storeId);

        // User-Store assignment
        Task AddUserToStoreAsync(int? storeId, string userId);
        Task<IEnumerable<string>> GetStoreUsersAsync(int? storeId);
        Task RemoveUserFromStoreAsync(int? storeId, string userId);

        // Additional methods
        Task<IEnumerable<UserDto>> GetStoreAssignedUsersAsync(int? storeId);
        Task<object> GetStoreInventorySummaryAsync(int? storeId);
        Task<IEnumerable<StoreRecentTransactionDto>> GetStoreRecentTransactionsAsync(int? storeId, int count = 20);
        Task<IEnumerable<StoreDto>> GetStoresByUnionAsync(int unionId);
        Task<byte[]> ExportStoresToCsvAsync(IEnumerable<StoreDto> stores);
        Task<ImportResultDto> ImportStoresFromCsvAsync(Stream fileStream, bool updateExisting, string importedBy);
        Task<IEnumerable<StoreDto>> GetUserStoresAsync(string userId);

        // Item management
        Task<ItemDto> GetItemDetailsAsync(int id);
        Task<IEnumerable<ItemDto>> GetAllItemsAsync();
        Task<ItemDto> GetItemByIdAsync(int id);
        Task AddItemToStoreAsync(int storeId, int itemId, decimal minStock, decimal maxStock, decimal reorderLevel);

        // Add this method that's used in controller
        Task<IEnumerable<StoreDto>> GetUserAccessibleStoresAsync(string userId);

        Task<IEnumerable<StoreStockDto>> GetItemDistributionAcrossStoresAsync(int itemId);
        Task<StoreStockDto> GetStoreStockItemAsync(int storeId, int itemId);

        Task<IEnumerable<StoreStockDto>> GetStoreStockAsync(int storeId);
        Task UpdateStockAsync(StockUpdateDto dto);
        Task<IEnumerable<StoreItemDto>> GetStoreItemsAsync(int storeId);
        Task<SelectList> GetSelectListAsync();

    }

    // Purchase Service Interface
    public interface IPurchaseService
    {
        Task<PurchaseDto> CreatePurchaseFromLowStockAsync(int storeId);
        Task<bool> ReceiveGoodsAsync(int purchaseId, ReceiveGoodsDto dto);
        Task<bool> PerformQualityCheckAsync(int receiveId, QualityCheckDto dto);
        // Existing methods...
        Task<PurchaseDto> CreatePurchaseAsync(PurchaseDto purchaseDto);
        Task<IEnumerable<PurchaseDto>> GetAllPurchasesAsync();
        Task<PurchaseDto> GetPurchaseByIdAsync(int id);
        Task<string> GeneratePurchaseOrderNoAsync();

        Task UpdatePurchaseAsync(PurchaseDto purchaseDto);
        Task DeletePurchaseAsync(int id);
        Task<IEnumerable<PurchaseDto>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PurchaseDto>> GetPurchasesByVendorAsync(int vendorId);
        Task<decimal> GetTotalPurchaseValueAsync(DateTime? startDate, DateTime? endDate);
        Task<int> GetPurchaseCountAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<PurchaseDto>> GetMarketplacePurchasesAsync();
        Task<IEnumerable<PurchaseDto>> GetVendorPurchasesAsync();
        Task<bool> PurchaseOrderNoExistsAsync(string purchaseOrderNo);
        Task<(IEnumerable<PurchaseDto> Items, int TotalCount)> GetPagedPurchasesAsync(int pageNumber, int pageSize);
        Task UpdateStatusAsync(int id, string status, string updatedBy);
        Task ApprovePurchaseAsync(int id, string approvedBy, string comments);
        Task RejectPurchaseAsync(int id, string rejectedBy, string reason);
        Task<List<ApprovalHistoryDto>> GetApprovalHistoryAsync(int purchaseId);
        Task<List<User>> GetApproversAsync();

        Task<ServiceResult> SubmitForApprovalAsync(int purchaseId);
        Task<ServiceResult> ReceivePurchaseAsync(ReceivePurchaseDto dto);
        Task<bool> CanUserApproveAsync(string userId, decimal amount);

        // Add these dashboard methods
        Task<int> GetPurchaseOrdersCount();
        Task<List<PurchaseDto>> GetRecentPurchases(int count);
        Task<List<MonthlyTrendDto>> GetMonthlyPurchaseTrend(int months);
        Task<bool> ProcessQualityCheckAsync(QualityCheckDto dto); // Add this line
    }

    // Issue Service Interface
    public interface IIssueService
    {
        Task<IssueDto> CreateIssueRequestAsync(IssueDto dto);
        Task<bool> ApproveIssueAsync(int issueId, IssueApprovalDto approvalDto);
        Task<bool> CompletePhysicalHandoverAsync(int issueId, HandoverDto handoverDto);
        Task<IssueDto> UpdateIssueAsync(IssueDto dto);

        Task<string> GenerateIssueNoAsync();
        Task<IEnumerable<IssueDto>> GetAllIssuesAsync();
        Task<IssueDto> GetIssueByIdAsync(int id);

        // Barcode scanning
        Task<IssueScanDto> ScanItemForIssueAsync(string barcodeNumber, int? storeId);
        Task<IEnumerable<IssueScanDto>> BulkScanItemsAsync(List<string> barcodeNumbers, int? storeId);

        // Voucher generation
        Task<IssueVoucherDto> GenerateIssueVoucherAsync(int issueId);
        Task<string> GenerateVoucherQRCodeAsync(int issueId);
        Task<byte[]> PrintIssueVoucherAsync(int issueId);

        // Digital signature
        Task<bool> CaptureDigitalSignatureAsync(int issueId, DigitalSignatureDto signatureDto);
        Task<DigitalSignatureDto> GetDigitalSignatureAsync(int issueId);

        // Enhanced approval
        Task<IssueDto> ApproveWithSignatureAsync(int issueId, string approvedBy, string comments, DigitalSignatureDto signature);

        // Tracking
        Task<IEnumerable<IssueDto>> GetIssuesByBarcodeAsync(string barcodeNumber);
        Task<Dictionary<string, object>> GetIssueAnalyticsAsync(DateTime fromDate, DateTime toDate);
        Task<IssueDto> CreateIssueAsync(IssueDto issueDto);
        Task<IEnumerable<IssueDto>> GetIssuesByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<IssueDto>> GetIssuesByRecipientAsync(string recipient);
        Task<IEnumerable<IssueDto>> GetIssuesByTypeAsync(string recipientType);
        Task<int> GetIssueCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetTotalIssuedValueAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> IssueNoExistsAsync(string issueNo);
        Task<IEnumerable<IssueDto>> GetPendingIssuesAsync();
        Task<IEnumerable<IssueDto>> GetPagedIssuesAsync(int pageNumber, int pageSize);
        Task<PagedResult<IssueDto>> GetAllIssuesAsync(int pageNumber = 1, int pageSize = 50);

        // Approval workflow methods - FIXED SIGNATURES
        Task<bool> SubmitForApprovalAsync(int issueId, string submittedBy);
        Task<IssueDto> ApproveIssueAsync(int issueId, string approvedBy, string comments = null);
        Task<IssueDto> RejectIssueAsync(int issueId, string rejectedBy, string reason);
        Task<bool> AddDigitalSignatureAsync(int issueId, DigitalSignatureDto signatureDto);
        Task<IssueDto> CreatePartialIssueAsync(int parentIssueId, Dictionary<int, decimal> actualQuantities, string createdBy);
        Task<IssueVoucherDto> CreateIssueWithVoucherAsync(int issueId);
        Task<IEnumerable<IssueDto>> GetApprovedIssuesWithoutReceivesAsync();
        Task<IssueDto> SubmitForApprovalAsync(int issueId);
        Task<bool> AddDigitalSignatureAsync(int issueId, string signaturePath, string signerName, string signerBadgeId);
        Task<IssueDto> CreatePartialIssueAsync(int parentIssueId, CreatePartialIssueDto dto);

        Task<ServiceResult> CreateIssueVoucherAsync(IssueDto dto);
        Task<ServiceResult> ProcessIssueAsync(int issueId);
        Task<ServiceResult> ConfirmReceiptAsync(string trackingCode);
        Task<IssueDto> GetIssueByTrackingCode(string trackingCode);

        Task<ServiceResult> CreateEmergencyRequestAsync(EmergencyRequestDto dto);
        Task<ServiceResult> ProcessEmergencyApprovalAsync(int issueId);

        // Add these dashboard methods
        Task<int> GetPendingIssuesCount();
        Task<List<IssueDto>> GetRecentIssues(int count);
        Task<int> GetPendingApprovalsCount();

        // Add these missing methods:
        Task<bool> DeleteIssueAsync(int issueId);
        Task<bool> CanDeleteIssueAsync(int issueId);
        Task<ServiceResult> CancelIssueAsync(int issueId, string reason);
        Task<IEnumerable<IssueDto>> SearchIssuesAsync(string searchTerm, string status, string issueType, DateTime? fromDate, DateTime? toDate);
        Task<byte[]> ExportIssuesToExcelAsync(string searchTerm, string status, string issueType, DateTime? fromDate, DateTime? toDate);

    }

    // Receive Service Interface
    public interface IReceiveService
    {
        Task<ReceiveDto> CreateReceiveFromIssueAsync(int issueId, string receivedBy);
        Task<bool> VerifyVoucherAsync(string voucherQRCode);
        Task<bool> CompleteReceiveAsync(int receiveId, string signature);
        Task<string> GenerateReceiveNoAsync();
        Task<IEnumerable<ReceiveDto>> GetAllReceivesAsync();
        Task<ReceiveDto> GetReceiveByIdAsync(int id);
        Task<IssueDto> ScanIssueVoucherAsync(string qrCode);
        Task<ReceiveDto> CreateReceiveFromVoucherAsync(string voucherNumber);
        Task<bool> AddDamagePhotoAsync(int receiveId, int itemId, string photoBase64);
        Task<ReceiveDto> AssessItemConditionAsync(int receiveId, int itemId, string condition, string notes);
        Task<ReceiveDto> BulkReceiveByBarcodeAsync(List<string> barcodeNumbers, string receivedBy);
        Task<Dictionary<string, object>> GetReceiveAnalyticsAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<dynamic>> GetDamageReportAsync(DateTime fromDate, DateTime toDate);
        Task<ReceiveDto> CreateReceiveAsync(ReceiveDto receiveDto);
        Task<IEnumerable<ReceiveDto>> GetReceivesByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<ReceiveDto>> GetReceivesBySourceAsync(string source);
        Task<IEnumerable<ReceiveDto>> GetReceivesByTypeAsync(string sourceType);
        Task<int> GetReceiveCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetTotalReceivedValueAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> ReceiveNoExistsAsync(string receiveNo);
        Task<IEnumerable<ReceiveDto>> GetPagedReceivesAsync(int pageNumber, int pageSize);
        Task<ReceiveDto> CreateReceiveFromIssueAsync(int issueId, ReceiveDto receiveDto);
        Task<IEnumerable<ReceiveDto>> GetReceivesByIssueAsync(int issueId);
        Task<StockLevelDto> GetStockLevelAsync(int itemId, int? storeId);
        byte[] ExportToExcel(IEnumerable<ReceiveDto> receives);
        byte[] ExportToPdf(IEnumerable<ReceiveDto> receives);
        Task<IEnumerable<ReceiveDto>> GetReceivesByIssueIdAsync(int issueId);
        Task<bool> UpdateReceivedQuantityAsync(int receiveId, int itemId, decimal quantity);
        Task<ReceiveDto> CreateQuickReceiveAsync(QuickReceiveDto dto);

        Task<ReceiveDto> UpdateReceiveAsync(ReceiveDto receiveDto);
        Task<bool> DeleteReceiveAsync(int id);
        Task<IEnumerable<ReceiveDto>> GetActiveReceivesAsync();
        Task<IEnumerable<ReceiveDto>> GetPendingReceivesAsync();
        Task<IEnumerable<ReceiveDto>> GetReceivesByStatusAsync(string status);
        Task<bool> ValidateReceiveAsync(int receiveId);
        Task<bool> CancelReceiveAsync(int id, string reason);
        Task<Dictionary<string, object>> GetReceiveSummaryAsync(int id);

        // Batch operations
        Task<bool> BulkUpdateItemConditionsAsync(int receiveId, Dictionary<int, string> itemConditions);
        Task<bool> BulkVerifyItemsAsync(int receiveId, List<ReceiveItemDto> items);

        // Reporting
        Task<byte[]> GenerateReceiveReportAsync(int id);
        Task<IEnumerable<ReceiveDto>> GetReceivesForReportAsync(DateTime fromDate, DateTime toDate, string status = null);

        // Validation
        Task<bool> CanEditReceiveAsync(int id);
        Task<bool> CanDeleteReceiveAsync(int id);
        Task<bool> IsReceiveCompletedAsync(int id);

        // Stock update methods
        Task<bool> UpdateStockFromReceiveAsync(int receiveId);
        Task<bool> ReverseStockFromReceiveAsync(int receiveId);
    }

    public interface ITransferService
    {
        Task<TransferDto> CreateTransferAsync(TransferDto dto);
        Task<bool> ApproveTransferAsync(int transferId, TransferApprovalDto approvalDto);
        Task<bool> ShipTransferAsync(int transferId, ShipmentDto shipmentDto);
        Task<bool> ReceiveTransferAsync(int transferId, ReceiveTransferDto receiveDto);
        Task<TransferTrackingDto> TrackTransferAsync(string transferNo);
        Task<IEnumerable<TransferDto>> GetAllTransfersAsync();
        Task<TransferDto> GetTransferByIdAsync(int id);
        Task<string> GenerateTransferNoAsync();
        Task<IEnumerable<TransferDto>> GetTransfersByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<TransferDto>> GetTransfersByStoreAsync(int? storeId);
        Task<IEnumerable<TransferDto>> GetTransfersFromStoreAsync(int? fromStoreId);
        Task<IEnumerable<TransferDto>> GetTransfersToStoreAsync(int? toStoreId);
        Task<int> GetTransferCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> TransferNoExistsAsync(string transferNo);
        Task<IEnumerable<TransferDto>> GetPagedTransfersAsync(int pageNumber, int pageSize);
        Task<bool> ValidateTransferAsync(TransferDto transferDto);
        Task<PagedResult<TransferDto>> GetAllTransfersAsync(int pageNumber = 1, int pageSize = 50);
        Task<ServiceResult> CreateTransferRequestAsync(TransferDto dto);
        Task<ServiceResult> ApproveTransferAsync(int transferId, string approvedBy);
        Task<ServiceResult> ProcessTransferDispatchAsync(int transferId);
        Task<ServiceResult> ConfirmTransferReceiptAsync(int transferId, TransferReceiptDto receiptDto);
        Task<TransferDto> GetTransferByTrackingCodeAsync(string trackingCode);
    }

    // Damage Service Interface
    public interface IDamageService
    {
        // Get Methods
        Task<IEnumerable<DamageDto>> GetAllDamagesAsync();
        Task<DamageDto> GetDamageByIdAsync(int id);
        Task<IEnumerable<DamageDto>> GetDamagesByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<DamageDto>> GetDamagesByItemAsync(int? itemId);
        Task<IEnumerable<DamageDto>> GetDamagesByStoreAsync(int? storeId);
        Task<IEnumerable<DamageDto>> GetDamagesByTypeAsync(string damageType);
        Task<IEnumerable<DamageDto>> GetDamagesByStatusAsync(DamageStatus status);
        Task<IEnumerable<DamageDto>> GetPagedDamagesAsync(int pageNumber, int pageSize);

        // Create/Update Methods
        Task<DamageDto> CreateDamageAsync(DamageDto damageDto);
        Task<DamageDto> CreateMultiItemDamageAsync(DamageDto damageDto, List<DamageItemDto> items);
        Task<bool> UpdateDamageStatusAsync(int id, DamageStatus status, string remarks);

        // Statistics & Calculations
        Task<int> GetDamageCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<decimal> GetTotalDamageValueAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<string, int>> GetDamageCountByTypeAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<Dictionary<DamageStatus, int>> GetDamageCountByStatusAsync();

        // Helper Methods
        Task<string> GenerateDamageNoAsync();
        Task<bool> DamageNoExistsAsync(string damageNo);
    }

    // Return Service Interface
    public interface IReturnService
    {
        Task<ReturnDto> CreateReturnRequestAsync(ReturnDto dto);
        Task<bool> CheckReturnConditionAsync(int returnId, ConditionCheckDto checkDto);
        Task<bool> ApproveReturnAsync(int returnId, ReturnApprovalDto approvalDto);
        Task<bool> ReceiveReturnAsync(int returnId, ReceiveReturnDto receiveDto);
        Task<string> GenerateReturnNoAsync();
        Task<IEnumerable<ReturnDto>> GetAllReturnsAsync();
        Task<ReturnDto> GetReturnByIdAsync(int id);
        Task<ReturnDto> CreateReturnAsync(ReturnDto returnDto);
        Task<IEnumerable<ReturnDto>> GetReturnsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<ReturnDto>> GetReturnsBySourceAsync(string returnedBy);
        Task<IEnumerable<ReturnDto>> GetReturnsByTypeAsync(string returnedByType);
        Task<IEnumerable<ReturnDto>> GetRestockedReturnsAsync();
        Task<IEnumerable<ReturnDto>> GetPendingRestockReturnsAsync();
        Task<int> GetReturnCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> ReturnNoExistsAsync(string returnNo);
        Task<IEnumerable<ReturnDto>> GetPagedReturnsAsync(int pageNumber, int pageSize);
        Task MarkAsRestockedAsync(int returnId);
        Task<IEnumerable<ReturnDto>> GetReturnsByIssueAsync(int issueId);
        Task<ServiceResult> AuthorizeReturnAsync(int returnId, bool isApproved, string reason = null);
        Task<ServiceResult> ProcessReturnRestockingAsync(int returnId, ReturnRestockDto restockDto);
        Task<ServiceResult> CreateDamageReportAsync(int returnId, DamageReportDto damageDto);
    }

    // WriteOff Service Interface
    public interface IWriteOffService
    {
        Task<DamageReportDto> ReportDamagedItemsAsync(DamageReportDto dto);
        Task<WriteOffRequestDto> CreateWriteOffRequestAsync(DamageReport damageReport);
        Task<bool> ApproveWriteOffAsync(int writeOffId, WriteOffApprovalDto approvalDto);
        Task<bool> ExecuteWriteOffAsync(int writeOffId, WriteOffExecutionDto executionDto);
        Task<IEnumerable<ExpiryAlertDto>> GetExpiryAlertsAsync(int storeId, int daysBeforeExpiry = 30);
        Task<string> GenerateDamageReportNoAsync();
        Task<string> GenerateWriteOffRequestNoAsync();
        Task<IEnumerable<WriteOffDto>> GetAllWriteOffsAsync();
        Task<WriteOffDto> GetWriteOffByIdAsync(int id);
        Task<WriteOffDto> CreateWriteOffAsync(WriteOffDto dto); // Overload without attachments
        Task<WriteOffDto> CreateWriteOffAsync(WriteOffDto dto, List<IFormFile> attachments);
        Task<WriteOffDto> CreateWriteOffWithApprovalAsync(WriteOffDto dto, string approvedBy);
        Task<WriteOffDto> UpdateWriteOffAsync(int id, WriteOffDto dto);
        Task<bool> DeleteWriteOffAsync(int id);
        Task<bool> SubmitForApprovalAsync(int writeOffId);
        Task<bool> ApproveWriteOffAsync(int writeOffId, string approvedBy); // Overload without comments
        Task<bool> ApproveWriteOffAsync(int writeOffId, string approvedBy, string comments);
        Task<bool> RejectWriteOffAsync(int writeOffId, string rejectedBy, string reason);
        Task<IEnumerable<WriteOffDto>> SearchWriteOffsAsync(string searchTerm);
        Task<IEnumerable<WriteOffDto>> GetWriteOffsByStatusAsync(string status);
        Task<IEnumerable<WriteOffDto>> GetWriteOffsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<WriteOffDto>> GetPendingApprovalsAsync(string approverRole);
        Task<IEnumerable<WriteOffDto>> GetWriteOffsByApproverAsync(string approverName);
        Task<IEnumerable<WriteOffDto>> GetWriteOffsByReasonAsync(string reason);
        Task<IEnumerable<WriteOffDto>> GetPagedWriteOffsAsync(int page, int pageSize);
        Task<ApprovalThresholdDto> GetApprovalRequirementAsync(decimal amount);
        Task<bool> CanUserApproveAsync(string userId, decimal totalValue);
        Task<string> GenerateWriteOffNoAsync();
        Task<bool> WriteOffNoExistsAsync(string writeOffNo);
        Task<int> GetWriteOffCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalWriteOffValueAsync(DateTime? startDate = null, DateTime? endDate = null);
    }

    public interface IVendorService
    {
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto> GetVendorByIdAsync(int id);
        Task<VendorDto> CreateVendorAsync(VendorDto vendorDto);
        Task UpdateVendorAsync(VendorDto vendorDto);
        Task DeleteVendorAsync(int id);
        Task<IEnumerable<VendorDto>> GetActiveVendorsAsync();
        Task<bool> VendorExistsAsync(string name, string email, int? excludeId = null);
        Task<IEnumerable<PurchaseDto>> GetVendorPurchasesAsync(int vendorId);
        Task<decimal> GetVendorTotalPurchaseValueAsync(int vendorId);
        Task<IEnumerable<VendorDto>> SearchVendorsAsync(string searchTerm);
        Task<IEnumerable<VendorDto>> GetPagedVendorsAsync(int pageNumber, int pageSize);
    }

    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto> GetBrandByIdAsync(int id);
        Task<BrandDto> CreateBrandAsync(BrandDto brandDto);
        Task UpdateBrandAsync(BrandDto brandDto);
        Task DeleteBrandAsync(int id);
        Task<IEnumerable<BrandDto>> GetActiveBrandsAsync();
        Task<bool> BrandExistsAsync(string name, int? excludeId = null);
        Task<int> GetBrandItemCountAsync(int brandId);
        Task<IEnumerable<BrandDto>> SearchBrandsAsync(string searchTerm);
        Task<IEnumerable<BrandDto>> GetPagedBrandsAsync(int pageNumber, int pageSize);
    }

    public interface IItemModelService
    {
        Task<IEnumerable<ItemModelDto>> GetAllItemModelsAsync();
        Task<IEnumerable<ItemModelDto>> GetItemModelsByBrandIdAsync(int brandId);
        Task<ItemModelDto> GetItemModelByIdAsync(int id);
        Task<ItemModelDto> CreateItemModelAsync(ItemModelDto itemModelDto);
        Task UpdateItemModelAsync(ItemModelDto itemModelDto);
        Task DeleteItemModelAsync(int id);
        Task<IEnumerable<ItemModelDto>> GetActiveItemModelsAsync();
        Task<bool> ItemModelExistsAsync(string name, string modelNumber, int brandId, int? excludeId = null);
        Task<int> GetItemModelItemCountAsync(int modelId);
        Task<IEnumerable<ItemModelDto>> SearchItemModelsAsync(string searchTerm);
        Task<IEnumerable<ItemModelDto>> GetPagedItemModelsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<ItemModelDto>> GetItemModelsByBrandAsync(int brandId);
    }

    public interface IBarcodeService
    {
        Task<string> GenerateBarcodeAsync(string data);
        Task<string> GenerateQRCodeAsync(string data);
        Task<BarcodeDto> CreateItemBarcodeAsync(int itemId);
        Task<BarcodeDto> CreateBatchBarcodeAsync(int batchId);
        //Task<string> ScanBarcodeAsync(string barcodeImage);
        Task<byte[]> GenerateBarcodeLabelsAsync(List<int> itemIds);
        Task<PagedResult<BarcodeDto>> GetAllBarcodesAsync(int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<BarcodeDto>> GetAllBarcodesAsync();
        Task<BarcodeDto> GetBarcodeByIdAsync(int id);
        Task<BarcodeDto> GetBarcodeByNumberAsync(string barcodeNumber);
        Task<BarcodeDto> GetBarcodeBySerialNumberAsync(string serialNumber);
        Task<BarcodeDto> CreateBarcodeAsync(BarcodeDto barcodeDto);
        Task<BarcodeDto> UpdateBarcodeAsync(BarcodeDto barcodeDto);
        Task<bool> DeleteBarcodeAsync(int id);
        Task<IEnumerable<BarcodeDto>> GetBarcodesByItemAsync(int? itemId);
        Task<IEnumerable<BarcodeDto>> GetBarcodesByStoreAsync(int? storeId);
        Task<IEnumerable<BarcodeDto>> GetBarcodesByLocationAsync(int? storeId, string location);
        Task<IEnumerable<BarcodeDto>> GetPagedBarcodesAsync(int pageNumber, int pageSize);
        Task<IEnumerable<BarcodeDto>> SearchBarcodesAsync(string searchTerm);
        Task<bool> BarcodeExistsAsync(string barcodeNumber);
        Task<bool> SerialNumberExistsAsync(string serialNumber);
        Task<string> GenerateUniqueBarcodeNumberAsync(int? itemId);
        Task<string> GenerateSerialNumberAsync(string prefix = "SN");
        Task<string> GenerateBatchNumberAsync(string prefix = "BN");
        byte[] GenerateQRCode(string text);
        string GenerateQRCodeBase64(string text);
        byte[] GenerateCode128Barcode(string text);
        string GenerateCode128BarcodeBase64(string text);
        Task<string> GenerateBarcodeAsync(int itemId, string batchNumber = null);
        Task<List<BarcodeDto>> GenerateBatchBarcodesAsync(int itemId, int quantity, string createdBy = null);
        Task<List<BarcodeDto>> GenerateBatchBarcodesForStoreAsync(int itemId, int? storeId, int quantity, string location = null);
        Task<byte[]> GenerateBatchBarcodePDF(List<BarcodeDto> barcodes);
        Task<byte[]> GenerateBarcodeLabelsAsync(List<int> barcodeIds, BarcodeLabel labelFormat);
        Task<BatchPrintDto> GenerateBatchLabelsAsync(BatchPrintRequestDto request);
        Task<(bool isValid, string message, BarcodeDto barcode)> ValidateBarcodeAsync(string barcodeNumber);
        Task<bool> ValidateBarcodeFormatAsync(string barcode);
        Task<ValidationResult> ValidateBarcodeChecksum(string barcode);
        Task UpdateBarcodeTrackingAsync(int barcodeId, string action, string userId);
        Task UpdatePrintInformationAsync(int barcodeId, string printedBy);
        Task UpdateBarcodeLocationAsync(int barcodeId, int? storeId, string location, string notes = null);
        Task<IEnumerable<BarcodeTrackingDto>> GetBarcodeHistoryAsync(int barcodeId);
        Task<BarcodeDto> ProcessOfflineScanAsync(OfflineScanDto scanData, string userId);
        Task<BarcodeStatisticsDto> GetBarcodeStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<BarcodeUsageDto>> GetBarcodeUsageReportAsync(int? storeId = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<int> GetActiveBarcodeCountAsync();
        Task<int> GetBarcodeCountByItemAsync(int? itemId);
        Task<TrackingInfoDto> DecodeTrackingQRAsync(string qrCode);
        Task<string> GenerateShipmentQRAsync(string referenceType, int referenceId, Dictionary<string, object> additionalData);
        Task<ServiceResult> UpdateTrackingStatusAsync(string qrCode, string status, string location = null);
        Task<QRCodeDto> GenerateQRCodeForVoucherAsync(string voucherType, int voucherId);
        Task<bool> SyncOfflineScansAsync(List<OfflineScanDto> offlineScans);
        Task<StockRotationDto> GetStockRotationSuggestionAsync(int itemId, int storeId, string method = "FIFO");
        Task<IEnumerable<BarcodeDto>> SearchBySerialNumberAsync(string term);
        Task<IEnumerable<string>> GetDistinctLocationsAsync();
        Task<IEnumerable<BarcodeDto>> GetFilteredBarcodesAsync(BarcodeFilterDto filters);
        Task<byte[]> ExportToExcelAsync(IEnumerable<BarcodeDto> barcodes);
        Task<byte[]> ExportToPDFAsync(IEnumerable<BarcodeDto> barcodes);
        Task<byte[]> ExportToCSVAsync(IEnumerable<BarcodeDto> barcodes);
    }

    public interface IReportService
    {
        Task<byte[]> GenerateStockReportAsync(StockReportRequestDto request);
        Task<byte[]> GenerateMovementReportAsync(MovementReportRequestDto request);
        Task<byte[]> GenerateVarianceReportAsync(int physicalInventoryId);
        Task<byte[]> GenerateExpiryReportAsync(int storeId, DateTime? asOfDate);
        Task<byte[]> GeneratePurchaseReportAsync(DateTime fromDate, DateTime toDate);
        Task<byte[]> GenerateIssueReportAsync(DateTime fromDate, DateTime toDate);
        Task<byte[]> GenerateAuditReportAsync(AuditReportRequestDto request);
        Task<DashboardStatsDto> GetDashboardStatsAsync(int? storeId = null);
        Task<StockReportDto> GetStockReportAsync(int? storeId = null, int? categoryId = null);
        Task<IEnumerable<IssueDto>> GetIssueReportAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null);
        Task<IEnumerable<PurchaseDto>> GetPurchaseReportAsync(DateTime? fromDate, DateTime? toDate, int? vendorId = null);
        Task<IEnumerable<DamageDto>> GetDamageReportAsync(DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<ReturnDto>> GetReturnReportAsync(DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<WriteOffDto>> GetWriteOffReportAsync(DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<TransferDto>> GetTransferReportAsync(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null);
        Task<IEnumerable<InventoryMovementDto>> GetInventoryMovementReportAsync(DateTime? fromDate, DateTime? toDate, int? itemId = null);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<IEnumerable<StockLevelDto>> GetStockLevelsAsync();
        Task<IEnumerable<CategoryStockDto>> GetCategoryWiseStockAsync();
        Task<byte[]> GenerateStockReportExcelAsync(int? storeId = null, int? categoryId = null);
        Task<byte[]> GeneratePurchaseReportExcelAsync(DateTime? fromDate, DateTime? toDate, int? vendorId = null);
        Task<byte[]> GeneratePurchaseReportCsvAsync(DateTime? fromDate, DateTime? toDate, int? vendorId = null);
        Task<byte[]> GenerateIssueReportExcelAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null);
        Task<byte[]> GenerateTransferReportExcelAsync(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null);
        Task<byte[]> GenerateTransferReportCsvAsync(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null);
        Task<byte[]> GenerateTransferReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? fromStoreId = null, int? toStoreId = null);
        Task<byte[]> GenerateLossReportExcelAsync(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null);
        Task<byte[]> GenerateLossReportCsvAsync(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null);
        Task<byte[]> GenerateLossReportPdfAsync(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null);
        Task<byte[]> GenerateMovementHistoryExcelAsync(DateTime? fromDate, DateTime? toDate, int? itemId = null, string movementType = null, int? storeId = null);
        Task<BatchReportDto> GenerateBatchReportAsync(string reportType, Dictionary<string, object> parameters);
        Task<object> GetInventorySummaryAsync();
        Task<IEnumerable<VendorDto>> GetVendorsAsync();
        Task<IEnumerable<ItemDto>> GetItemsAsync();
        Task<object> GetReportStatisticsAsync();
        Task<object> GetMonthlyTrendAsync(string reportType, int months);
        Task<IEnumerable<object>> GetLossReportAsync(DateTime? fromDate, DateTime? toDate, string lossType = null, int? storeId = null);
        Task<IEnumerable<object>> GetMovementHistoryAsync(DateTime? fromDate, DateTime? toDate, int? itemId = null, string movementType = null, int? storeId = null);

        // New Report Methods
        Task<ConsumptionAnalysisReportDto> GetConsumptionAnalysisReportAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null);
        Task<ExpiryReportDto> GetExpiryReportAsync(int? storeId = null, int? daysAhead = 90);
        Task<AuditTrailReportDto> GetAuditTrailReportAsync(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null);
        Task<VarianceAnalysisDto> GetVarianceReportAsync(int physicalInventoryId);
        Task<ABCAnalysisReportDto> GetABCAnalysisReportAsync(string analysisMethod = "Value", int months = 12);

        // New Report Export Methods - Excel
        Task<byte[]> GenerateConsumptionReportExcelAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null);
        Task<byte[]> GenerateExpiryReportExcelAsync(int? storeId = null, int? daysAhead = 90);
        Task<byte[]> GenerateAuditTrailReportExcelAsync(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null);
        Task<byte[]> GenerateVarianceReportExcelAsync(int physicalInventoryId);
        Task<byte[]> GenerateABCAnalysisReportExcelAsync(string analysisMethod = "Value", int months = 12);

        // New Report Export Methods - PDF
        Task<byte[]> GenerateStockReportPdfAsync(int? storeId = null, int? categoryId = null);
        Task<byte[]> GeneratePurchaseReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? vendorId = null);
        Task<byte[]> GenerateIssueReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null);
        Task<byte[]> GenerateConsumptionReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? storeId = null, int? categoryId = null);
        Task<byte[]> GenerateExpiryReportPdfAsync(int? storeId = null, int? daysAhead = 90);
        Task<byte[]> GenerateAuditTrailReportPdfAsync(DateTime? fromDate, DateTime? toDate, string transactionType = null, int? storeId = null);
        Task<byte[]> GenerateVarianceReportPdfAsync(int physicalInventoryId);
        Task<byte[]> GenerateABCAnalysisReportPdfAsync(string analysisMethod = "Value", int months = 12);
    }

    // User Service Interface
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string id);
        Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task UpdateUserAsync(UserDto userDto);
        Task DeleteUserAsync(string id);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task AddUserToRoleAsync(string userId, string role);
        Task RemoveUserFromRoleAsync(string userId, string role);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role);
        Task<bool> EmailExistsAsync(string email, string excludeUserId = null);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string userId, string newPassword);
        Task<IEnumerable<UserDto>> GetActiveUsersAsync();
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<UserDto>> GetPagedUsersAsync(int pageNumber, int pageSize);

        // Add new methods for organizational assignment
        Task AssignUserToBattalionAsync(string userId, int battalionId);
        Task AssignUserToRangeAsync(string userId, int rangeId);
        Task AssignUserToZilaAsync(string userId, int zilaId);
        Task AssignUserToUpazilaAsync(string userId, int upazilaId);
        Task<IEnumerable<UserDto>> GetUsersByBattalionAsync(int battalionId);
        Task<IEnumerable<UserDto>> GetUsersByRangeAsync(int rangeId);
        Task<IEnumerable<UserDto>> GetUsersByZilaAsync(int zilaId);
        Task<IEnumerable<UserDto>> GetUsersByUpazilaAsync(int upazilaId);
        Task<IEnumerable<UserDto>> GetUsersByDesignationAsync(string designation);
        Task UpdateUserBadgeNumberAsync(string userId, string badgeNumber);
        Task<bool> BadgeNumberExistsAsync(string badgeNumber, string excludeUserId = null);
    }

    // Activity Log Service Interface
    public interface IActivityLogService
    {
        // Main logging methods - REQUIRED by interface
        Task LogAsync(string entityName, string action, string description,
            int? entityId, object oldValues, object newValues, string userId, string ipAddress);

        // Overloaded logging methods for convenience
        Task LogActivityAsync(string entityName, int? entityId, string action,
            string description, string userId = null);

        Task LogActivityAsync(string userId, string action, string entityType,
            int? entityId, object oldValues, object newValues, string ipAddress = null);

        Task LogActivityAsync(string userId, string action, string entityType,
            int entityId, string oldValue, string newValue, string additionalInfo);

        // Query methods
        Task<IEnumerable<ActivityLogDto>> GetAllActivityLogsAsync(
            string userId = null, string entityName = null,
            DateTime? fromDate = null, DateTime? toDate = null);

        Task<IEnumerable<ActivityLogDto>> GetEntityActivityLogsAsync(
            string entityName, int entityId);

        Task<IEnumerable<ActivityLogDto>> GetUserActivityLogsAsync(
            string userId, int? limit = null);

        Task<ActivityLogDto> GetActivityLogByIdAsync(int id);

        Task<SystemActivityStatsDto> GetSystemActivityStatsAsync(
            DateTime? fromDate = null, DateTime? toDate = null);

        // Maintenance methods
        Task<int> ArchiveOldLogsAsync(int daysToKeep = 90);
        Task<int> DeleteArchivedLogsAsync(int daysToKeep = 365);
        Task LogStorUsereActivityAsync(string v1, int? storeId, string v2, string v3, string v4);
        Task LogRemoveUserActivityAsync(string v1, int? storeId, string v2, string v3, string v4);
        Task LogAsync(string v1, int id, string v2, string v3);

        Task<IEnumerable<ActivityLogDto>> GetRecentActivitiesAsync(int count = 10);
        Task<IEnumerable<ActivityLogDto>> GetActivityLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<NotificationDto>> GetUnreadNotificationsAsync(string userId);
    }

    // Login Log Service Interface
    public interface ILoginLogService
    {
        Task<IEnumerable<LoginLogDto>> GetAllLoginLogsAsync();
        Task<IEnumerable<LoginLogDto>> GetLoginLogsByUserAsync(string userId);
        Task<IEnumerable<LoginLogDto>> GetLoginLogsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<LoginLogDto>> GetActiveSessionsAsync();
        Task LogLoginAsync(string userId, string ipAddress);
        Task LogLogoutAsync(string userId);
        Task<int> GetLoginCountAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<LoginLogDto>> GetFailedLoginAttemptsAsync(DateTime? fromDate = null);
        Task<IEnumerable<LoginLogDto>> GetPagedLoginLogsAsync(int pageNumber, int pageSize);
        Task<TimeSpan> GetAverageSessionDurationAsync(string userId = null);
        Task CleanupOldLogsAsync(int daysToKeep = 90);
    }

    // Stock Adjustment Service Interface
    public interface IStockAdjustmentService
    {
        Task<IEnumerable<StockAdjustmentDto>> GetAllStockAdjustmentsAsync();
        Task<StockAdjustmentDto> GetStockAdjustmentByIdAsync(int id);
        Task<StockAdjustmentDto> CreateStockAdjustmentAsync(StockAdjustmentDto adjustmentDto);
        Task<string> GenerateAdjustmentNoAsync();
        Task<IEnumerable<StockAdjustmentDto>> GetAdjustmentsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<StockAdjustmentDto>> GetAdjustmentsByItemAsync(int? itemId);
        Task<IEnumerable<StockAdjustmentDto>> GetAdjustmentsByStoreAsync(int? storeId);
        Task<IEnumerable<StockAdjustmentDto>> GetAdjustmentsByTypeAsync(string adjustmentType);
        Task<bool> AdjustmentNoExistsAsync(string adjustmentNo);
        Task<IEnumerable<StockAdjustmentDto>> GetPagedAdjustmentsAsync(int pageNumber, int pageSize);
        Task ApproveAdjustmentAsync(int adjustmentId, string approvedBy);

        Task<IEnumerable<StockAdjustmentDto>> GetPendingAdjustmentsAsync();
        Task<IEnumerable<StockAdjustmentDto>> GetApprovedAdjustmentsAsync();
        Task RejectAdjustmentAsync(int id, string rejectedBy, string reason);
        Task<decimal> GetTotalAdjustmentValueAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<StockLevelDto> GetCurrentStockLevelAsync(int itemId, int? storeId);
        Task<Dictionary<string, int>> GetAdjustmentStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<StockAdjustmentDto> CreateAdjustmentAsync(StockAdjustmentDto adjustment);
        Task<StockAdjustmentDto> GetAdjustmentAsync(int id);
        Task<List<StockAdjustmentDto>> GetAdjustmentsAsync();
        Task<bool> ProcessAdjustmentAsync(int id);

    }

    // File Service Interface (for handling uploads)
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        Task<byte[]> GetFileAsync(string filePath);
        Task<string> GetFileUrlAsync(string filePath);
        bool IsValidImageFile(IFormFile file);
        bool IsValidDocumentFile(IFormFile file);
        Task<string> SaveBase64ImageAsync(string base64String, string folder, string fileName);
    }

    // Email Service Interface
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml);
        Task SendBulkEmailAsync(List<string> recipients, string subject, string body);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);

        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string fileName);
    }

    // Notification Service Interface
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(NotificationDto dto);
        Task CreateLowStockAlertAsync(int storeId, int itemId);
        Task CreateExpiryAlertAsync(BatchTracking batch);
        Task CreateApprovalNotificationAsync(ApprovalRequest approval);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task SendBatchNotificationsAsync(BatchNotificationDto dto);
        Task CreateDailySummaryAsync();

        // Existing methods
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(string userId);
        Task<NotificationDto> GetNotificationByIdAsync(int id);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteNotificationAsync(int notificationId);
        Task<int> GetUnreadCountAsync(string userId);
        Task SendBroadcastNotificationAsync(string title, string message, string type = "info");
        Task SendNotificationAsync(string userId, string title, string message, string type = "info");

        // ADD THESE MISSING METHODS:
        Task<IEnumerable<NotificationDto>> GetRecentNotificationsAsync(string userId, int count = 5);
        Task DeleteAllReadNotificationsAsync(string userId);
        Task SendToRoleAsync(string roleName, string title, string message, string type = "info");
        Task SendNotificationAsync(NotificationDto notification);
    }
    // Cache Service Interface
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task ClearAsync();
        Task RemovePatternAsync(string pattern);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null);
    }

    // Configuration Service Interface
    public interface IConfigurationService
    {
        Task<string> GetSettingAsync(string key);
        Task SetSettingAsync(string key, string value);
        Task<T> GetSettingAsync<T>(string key);
        Task SetSettingAsync<T>(string key, T value);
        Task<IEnumerable<SettingDto>> GetAllSettingsAsync();
        Task UpdateSettingsAsync(Dictionary<string, string> settings);
        Task<bool> SettingExistsAsync(string key);
    }

    // Audit Service Interface
    public interface IAuditService
    {
        Task<IEnumerable<AuditDto>> GetAuditTrailAsync(string entity, int entityId);
        Task<IEnumerable<AuditDto>> GetAuditsByUserAsync(string userId);
        Task<IEnumerable<AuditDto>> GetAuditsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task LogAuditAsync(string entity, int entityId, string action, string changes, string userId);
        Task<byte[]> GenerateAuditReportAsync(DateTime fromDate, DateTime toDate);
    }

    // Dashboard Service Interface
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<IEnumerable<RecentActivityDto>> GetRecentActivitiesAsync(int count = 10);
        Task<IEnumerable<AlertDto>> GetAlertsAsync();
        Task<IEnumerable<ChartDataDto>> GetPurchaseTrendAsync(int months = 12);
        Task<IEnumerable<ChartDataDto>> GetIssueTrendAsync(int months = 12);
        Task<IEnumerable<ChartDataDto>> GetCategoryDistributionAsync();
        Task<IEnumerable<ChartDataDto>> GetStoreWiseStockAsync();
        Task RefreshCacheAsync();
    }

    public interface ISettingService
    {
        Task<IEnumerable<SettingDto>> GetAllSettingsAsync();
        Task<SettingDto> GetSettingByKeyAsync(string key);
        Task UpdateSettingAsync(string key, string value);
        Task<string> GetSettingValueAsync(string key);
        Task<Dictionary<string, string>> GetSettingsAsDictionaryAsync();
    }

    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermissionDto>> GetAllRolePermissionsAsync();
        Task<IEnumerable<RolePermissionDto>> GetPermissionsByRoleAsync(string roleId);
        Task<RoleWithPermissionsDto> GetRoleWithPermissionsAsync(string roleId);
        Task<IEnumerable<RoleWithPermissionsDto>> GetAllRolesWithPermissionsAsync();
        Task AssignPermissionsToRoleAsync(RolePermissionCreateDto dto);
        Task UpdateRolePermissionAsync(RolePermissionUpdateDto dto);
        Task RemovePermissionFromRoleAsync(string roleId, Permission permission);
        Task RemoveAllPermissionsFromRoleAsync(string roleId);
        Task<bool> HasPermissionAsync(string roleId, Permission permission);
        Task<IEnumerable<Permission>> GetUserPermissionsAsync(string userId);
        Task<bool> UserHasPermissionAsync(string userId, Permission permission);
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetPermissionsByCategoryAsync(string category);
        Task CopyPermissionsAsync(string sourceRoleId, string targetRoleId);
        Task InitializeDefaultPermissionsAsync();
    }

    // Battalion Service Interface
    public interface IBattalionService
    {
        Task<IEnumerable<BattalionDto>> GetAllBattalionsAsync();
        Task<IEnumerable<BattalionDto>> GetActiveBattalionsAsync();
        Task<IEnumerable<BattalionDto>> GetBattalionsByRangeAsync(int rangeId);
        Task<IEnumerable<BattalionDto>> GetBattalionsByTypeAsync(BattalionType type);
        Task<BattalionDto> GetBattalionByIdAsync(int id);
        Task<BattalionDto> GetBattalionByCodeAsync(string code);
        Task<BattalionDto> CreateBattalionAsync(BattalionDto battalionDto);
        Task UpdateBattalionAsync(BattalionDto battalionDto);
        Task DeleteBattalionAsync(int id);
        Task<bool> BattalionExistsAsync(string name, int? excludeId = null);
        Task<bool> BattalionCodeExistsAsync(string code, int? excludeId = null);
        Task<int> GetBattalionPersonnelCountAsync(int battalionId);
        Task<int> GetBattalionStoreCountAsync(int battalionId);
        Task<IEnumerable<StoreDto>> GetBattalionStoresAsync(int battalionId);
        Task AssignStoreToBattalionAsync(int battalionId, int? storeId, bool isPrimary = false);
        Task RemoveStoreFromBattalionAsync(int battalionId, int? storeId);
        Task<string> GenerateBattalionCodeAsync(BattalionType type);
        Task<IEnumerable<BattalionDto>> SearchBattalionsAsync(string searchTerm);
        Task<Dictionary<string, object>> GetBattalionStatisticsAsync(int battalionId);
        Task<SelectList> GetSelectListAsync();
    }

    // Range Service Interface
    public interface IRangeService
    {
        Task<IEnumerable<RangeDto>> GetAllRangesAsync();
        Task<IEnumerable<RangeDto>> GetActiveRangesAsync();
        Task<RangeDto> GetRangeByIdAsync(int id);
        Task<RangeDto> GetRangeByCodeAsync(string code);
        Task<RangeDto> CreateRangeAsync(RangeDto rangeDto);
        Task UpdateRangeAsync(RangeDto rangeDto);
        Task DeleteRangeAsync(int id);
        Task<bool> RangeExistsAsync(string name, int? excludeId = null);
        Task<bool> RangeCodeExistsAsync(string code, int? excludeId = null);
        Task<int> GetRangeBattalionCountAsync(int rangeId);
        Task<int> GetRangeZilaCountAsync(int rangeId);
        Task<int> GetRangeStoreCountAsync(int rangeId);
        Task<IEnumerable<BattalionDto>> GetRangeBattalionsAsync(int rangeId);
        Task<IEnumerable<ZilaDto>> GetRangeZilasAsync(int rangeId);
        Task<RangeHierarchyDto> GetRangeHierarchyAsync(int rangeId);
        Task<string> GenerateRangeCodeAsync();
        Task<IEnumerable<RangeDto>> SearchRangesAsync(string searchTerm);
        Task<RangeStatisticsDto> GetRangeStatisticsAsync(int rangeId);
        Task<Dictionary<string, object>> GetRangeDashboardDataAsync(int rangeId);
        Task<string> GenerateRangeCodeFromNameAsync(string rangeName);
    }

    // Zila Service Interface
    public interface IZilaService
    {
        Task<IEnumerable<ZilaDto>> GetAllZilasAsync();
        Task<IEnumerable<ZilaDto>> GetActiveZilasAsync();
        Task<IEnumerable<ZilaDto>> GetZilasByRangeAsync(int rangeId);
        Task<IEnumerable<ZilaDto>> GetZilasByDivisionAsync(string division);
        Task<ZilaDto> GetZilaByIdAsync(int id);
        Task<ZilaDto> GetZilaByCodeAsync(string code);
        Task<ZilaDto> CreateZilaAsync(ZilaDto zilaDto);
        Task UpdateZilaAsync(ZilaDto zilaDto);
        Task DeleteZilaAsync(int id);
        Task<bool> ZilaExistsAsync(string name, int? excludeId = null);
        Task<bool> ZilaCodeExistsAsync(string code, int? excludeId = null);
        Task<IEnumerable<UpazilaDto>> GetZilaUpazilasAsync(int zilaId);
        Task<int> GetZilaUpazilaCountAsync(int zilaId);
        Task<int> GetZilaStoreCountAsync(int zilaId);
        Task<int> GetZilaVDPMemberCountAsync(int zilaId);
        //Task ImportZilasFromCsvAsync(string filePath);
        Task<byte[]> ExportZilasAsync(); // Added for export functionality

        // NEW METHODS - Add these:
        Task<string> GenerateZilaCodeAsync(string name);
        Task<IEnumerable<ZilaDto>> SearchZilasAsync(string searchTerm);
        Task<Dictionary<string, object>> GetZilaStatisticsAsync(int zilaId);
        Task<ImportResultDto> ImportZilasFromCsvAsync(string filePath);
        Task<bool> CheckNameExistsAsync(string name, int? excludeId = null);
    }

    // Upazila Service Interface
    public interface IUpazilaService
    {
        Task<IEnumerable<UpazilaDto>> GetAllUpazilasAsync();
        Task<IEnumerable<UpazilaDto>> GetActiveUpazilasAsync();
        Task<IEnumerable<UpazilaDto>> GetUpazilasByZilaAsync(int zilaId);
        Task<IEnumerable<UpazilaDto>> GetUpazilasWithVDPUnitsAsync();
        Task<UpazilaDto> GetUpazilaByIdAsync(int id);
        Task<UpazilaDto> GetUpazilaByCodeAsync(string code);
        Task<UpazilaDto> CreateUpazilaAsync(UpazilaDto upazilaDto);
        Task UpdateUpazilaAsync(UpazilaDto upazilaDto);
        Task DeleteUpazilaAsync(int id);
        Task<bool> UpazilaExistsAsync(string name, int zilaId, int? excludeId = null);
        Task<bool> UpazilaCodeExistsAsync(string code, int? excludeId = null);
        Task<bool> CheckNameExistsAsync(string name, int zilaId, int? excludeId = null); // Added for validation
        Task<int> GetUpazilaStoreCountAsync(int upazilaId);
        Task UpdateVDPMemberCountAsync(int upazilaId, int memberCount);
        Task<UpazilaStatisticsDto> GetUpazilaStatisticsAsync(int upazilaId);
        Task<ImportResultDto> ImportUpazilasFromCsvAsync(string filePath); // Fixed return type
        Task<byte[]> ExportUpazilasAsync();
        Task<string> GenerateUpazilaCodeAsync(int zilaId);
        Task<IEnumerable<UpazilaDto>> SearchUpazilasAsync(string searchTerm); // Added search method
    }

    // UserStore Service Interface
    public interface IUserStoreService
    {
        Task<IEnumerable<UserStoreDto>> GetAllUserStoresAsync();
        Task<IEnumerable<UserStoreDto>> GetUserStoresByUserAsync(string userId);
        Task<IEnumerable<UserStoreDto>> GetUserStoresByStoreAsync(int? storeId);
        Task<IEnumerable<UserDto>> GetStoreUsersAsync(int? storeId);
        Task<IEnumerable<StoreDto>> GetUserStoresAsync(string userId);
        Task<UserStoreDto> AssignUserToStoreAsync(UserStoreDto userStoreDto);
        Task UpdateUserStoreAsync(UserStoreDto userStoreDto);
        Task RemoveUserFromStoreAsync(int id);
        Task RemoveUserFromStoreAsync(string userId, int? storeId);
        Task<bool> IsUserAssignedToStoreAsync(string userId, int? storeId);
        Task SetPrimaryStoreAsync(string userId, int? storeId);
        Task<StoreDto> GetUserPrimaryStoreAsync(string userId);
        Task BulkAssignUsersToStoreAsync(int? storeId, List<string> userIds);
        Task BulkRemoveUsersFromStoreAsync(int? storeId, List<string> userIds);
        Task<IEnumerable<UserStoreDto>> GetActiveAssignmentsAsync();
        Task<IEnumerable<UserStoreDto>> GetAssignmentHistoryAsync(string userId = null, int? storeId = null);

        Task<IEnumerable<UserStoreDto>> GetUserStoresByStoreAsync(int storeId);
        Task<UserStoreDto> AssignUserToStoreAsync(string userId, int storeId, string assignedBy);
        Task RemoveUserFromStoreAsync(string userId, int storeId);
        Task<bool> UserHasAccessToStoreAsync(string userId, int storeId);

    }


    public interface IStoreConfigurationService
    {
        Task<IEnumerable<StoreConfigurationDto>> GetAllConfigurationsAsync();
        Task<IEnumerable<StoreConfigurationDto>> GetStoreConfigurationsAsync(int storeId);
        Task<StoreConfigurationDto> GetConfigurationByIdAsync(int id);
        Task<StoreConfigurationDto> CreateConfigurationAsync(StoreConfigurationDto dto);
        Task UpdateConfigurationAsync(StoreConfigurationDto dto);
        Task DeleteConfigurationAsync(int id);
        Task<string> GetConfigValueAsync(int? storeId, string configKey);
        Task<IEnumerable<string>> GetAvailableConfigKeysAsync();
        Task<Dictionary<string, string>> GetAllStoreConfigsAsync(int? storeId);
        Task BulkUpdateConfigurationsAsync(int? storeId, Dictionary<string, string> configs);
        Task CopyConfigurationsAsync(int sourceStoreId, int targetStoreId);
    }

    // BattalionStore Service Interface
    public interface IBattalionStoreService
    {
        Task<IEnumerable<BattalionStoreDto>> GetAllBattalionStoresAsync();
        Task<IEnumerable<BattalionStoreDto>> GetBattalionStoresByBattalionAsync(int battalionId);
        Task<IEnumerable<BattalionStoreDto>> GetBattalionStoresByStoreAsync(int? storeId);
        Task<BattalionStoreDto> GetBattalionStoreByIdAsync(int id);
        Task<BattalionStoreDto> AssignBattalionToStoreAsync(BattalionStoreDto battalionStoreDto);
        Task UpdateBattalionStoreAsync(BattalionStoreDto battalionStoreDto);
        Task RemoveBattalionFromStoreAsync(int id);
        Task<bool> IsBattalionAssignedToStoreAsync(int battalionId, int? storeId);
        Task SetPrimaryStoreForBattalionAsync(int battalionId, int? storeId);
        Task<StoreDto> GetBattalionPrimaryStoreAsync(int battalionId);
        Task<IEnumerable<BattalionStoreDto>> GetActiveBattalionStoresAsync(DateTime? asOfDate = null);
        Task<IEnumerable<BattalionStoreDto>> GetBattalionStoreHistoryAsync(int battalionId);
        Task EndBattalionStoreAssignmentAsync(int battalionId, int? storeId, DateTime effectiveTo);
    }

    public interface IApprovalService
    {
        Task<ApprovalRequest> CreateApprovalRequestAsync(ApprovalRequestDto dto);
        Task<bool> ApproveRequestAsync(ApprovalActionDto dto);
        Task<bool> RejectRequestAsync(ApprovalActionDto dto);
        Task<bool> EscalateRequestAsync(int requestId, string escalatedBy, string reason);
        Task<IEnumerable<PendingApprovalDto>> GetPendingApprovalsAsync(string userId);
        Task<IEnumerable<ApprovalHistoryDto>> GetApprovalHistoryAsync(string entityType, int entityId);
        Task<bool> DelegateApprovalAsync(DelegationDto dto);
        Task<ApprovalWorkflowDto> SetupWorkflowAsync(ApprovalWorkflowDto dto);
        Task<List<ApprovalStep>> GetCompletedApprovalsAsync(string entityType, int entityId);
        Task<ApprovalThresholdDto> GetApprovalRequirementAsync(string entityType, decimal value);
        Task<string> GetNextApproverAsync(string entityType, decimal value);
        Task<bool> ValidateApprovalAsync(string userId, string entityType, int entityId, decimal value);
        Task RecordApprovalAsync(string entityType, int entityId, string approvedBy, string comments);
        Task<List<ApprovalThresholdDto>> GetApprovalThresholdsAsync(string entityType);
        Task<ServiceResult> ProcessApprovalAsync(int approvalId, string action, string userId, string remarks = null);
        Task<bool> CanUserApproveAsync(string userId, string entityType, decimal amount);
        Task CreateApprovalWorkflowAsync(string entityType, int entityId, List<ApprovalLevel> levels);
        Task<int> GetCurrentApprovalLevelAsync(string entityType, int entityId);
        Task<bool> CanUserApproveAsync(string userId, int approvalLevel);
        Task ApproveAsync(string entityType, int entityId, string approvedBy, string remarks);
        Task<bool> AreAllApprovalsCompleteAsync(string entityType, int entityId);
        Task<List<ApprovalDto>> GetApprovalChainAsync(string entityType, int entityId);
        Task InitiateApprovalAsync(string entityType, int entityId);
        Task<int> GetPendingCountAsync(string userId);

        // Approval Configuration Management
        Task<List<ApprovalThresholdDto>> GetAllThresholdsAsync();
        Task<ApprovalThresholdDto> GetThresholdByIdAsync(int id);
        Task<ApprovalThresholdDto> CreateThresholdAsync(ApprovalThresholdDto dto);
        Task<ApprovalThresholdDto> UpdateThresholdAsync(ApprovalThresholdDto dto);
        Task<bool> DeleteThresholdAsync(int id);
        Task<bool> ToggleThresholdStatusAsync(int id);

        // Workflow Management
        Task<List<ApprovalWorkflowDto>> GetAllWorkflowsAsync();
        Task<ApprovalWorkflowDto> GetWorkflowByIdAsync(int id);
        Task<ApprovalWorkflowDto> CreateWorkflowAsync(ApprovalWorkflowDto dto);
        Task<ApprovalWorkflowDto> UpdateWorkflowAsync(ApprovalWorkflowDto dto);
        Task<bool> DeleteWorkflowAsync(int id);
        Task<bool> ToggleWorkflowStatusAsync(int id);

        // Entity Type Configuration
        Task<List<string>> GetConfiguredEntityTypesAsync();
        Task<bool> IsApprovalRequiredForEntityAsync(string entityType);
        Task<bool> ToggleEntityApprovalAsync(string entityType, bool isRequired);
    }

    public interface IStockAlertService
    {
        Task<PersonalizedDashboardDto> GetPersonalizedAlertsAsync(string userId);
        Task<List<StockAlertDto>> GetStoreAlertsAsync(int? storeId);
        Task<List<StockAlertDto>> CheckAllStoresForAlertsAsync();
        Task SendLowStockEmailsAsync();
        Task AcknowledgeAlertAsync(int itemId, int? storeId, string userId);
        Task<StockAlertConfigDto> GetAlertConfigurationAsync(int itemId, int? storeId);
        Task UpdateAlertConfigurationAsync(StockAlertConfigDto config);
        Task<Dictionary<StockAlertLevel, int>> GetAlertSummaryAsync(string userId);
        Task CheckAndSendLowStockAlertsAsync();
        //Task<List<StockAlertDto>> GetLowStockItemsAsync(string userId = null);
        Task<IEnumerable<StockAlertDto>> GetLowStockItemsAsync(string storeCode = null);
        Task SendLowStockAlertsAsync();

        Task<List<StockAlertDto>> GetLowStockItems();
        Task<PersonalizedAlertDashboard> GetPersonalizedAlerts(string userName);
        Task<PersonalizedAlertDashboard> GetStockAlertDashboard();
    }

    public interface IStoreTypeService
    {
        Task<IEnumerable<StoreTypeDto>> GetAllStoreTypesAsync();
        Task<StoreTypeDto> GetStoreTypeByIdAsync(int id);
        Task<StoreTypeDto> GetStoreTypeByCodeAsync(string code);
        Task<StoreTypeDto> CreateStoreTypeAsync(StoreTypeDto storeTypeDto);
        Task UpdateStoreTypeAsync(StoreTypeDto storeTypeDto);
        Task DeleteStoreTypeAsync(int id);
        Task<bool> CanStoreTypeHoldCategoryAsync(int storeTypeId, int categoryId);
        Task AssignCategoriesToStoreTypeAsync(int storeTypeId, List<int> categoryIds);
    }

    public interface IUserContext
    {
        string GetCurrentUserId();
        string GetCurrentUserName();
        Task<bool> HasPermissionAsync(Permission permission);
        string CurrentUserName { get; }  // Add this property
        string UserId { get; }  // Added property

        string UserName { get; }
        string UserEmail { get; }
        string UserRole { get; }
        bool IsAuthenticated { get; }
        Task<string> GetCurrentUserIdAsync();
        Task<string> GetCurrentUserNameAsync();
        Task<List<string>> GetCurrentUserRolesAsync();
        bool IsInRole(string role);
        int? GetCurrentStoreId();
    }

    public interface IValidationService
    {
        Task<ValidationResult> ValidateStockAvailability(int itemId, int? storeId, decimal requestedQuantity);
        Task<ValidationResult> ValidatePurchaseOrder(PurchaseDto purchaseDto);
        Task<ValidationResult> ValidateIssue(IssueDto issueDto);
        Task<ValidationResult> ValidateTransfer(TransferDto transferDto);
    }

    public interface IUnionService
    {
        // Basic CRUD operations
        Task<IEnumerable<UnionDto>> GetAllUnionsAsync();
        Task<IEnumerable<UnionDto>> GetActiveUnionsAsync();
        Task<IEnumerable<UnionDto>> GetUnionsByUpazilaAsync(int upazilaId);
        Task<IEnumerable<UnionDto>> GetUnionsByZilaAsync(int zilaId);
        Task<IEnumerable<UnionDto>> GetUnionsWithVDPUnitsAsync();
        Task<IEnumerable<UnionDto>> GetBorderAreaUnionsAsync();
        Task<UnionDto> GetUnionByIdAsync(int id);
        Task<UnionDto> GetUnionByCodeAsync(string code);
        Task<UnionDto> CreateUnionAsync(UnionDto unionDto);
        Task UpdateUnionAsync(UnionDto unionDto);
        Task DeleteUnionAsync(int id);

        // Validation
        Task<bool> UnionExistsAsync(string name, int upazilaId, int? excludeId = null);
        Task<bool> UnionCodeExistsAsync(string code, int? excludeId = null);

        // Statistics and counts
        Task<int> GetUnionStoreCountAsync(int unionId);
        Task<int> GetUnionVDPMemberCountAsync(int unionId);
        Task<UnionStatisticsDto> GetUnionStatisticsAsync(int unionId);
        Task<Dictionary<string, object>> GetUnionDashboardDataAsync(int unionId);

        // Bulk operations
        Task ImportUnionsFromCsvAsync(string filePath);
        Task<byte[]> ExportUnionsAsync();
        Task<byte[]> ExportUnionsByUpazilaAsync(int upazilaId);

        // Search and filtering
        Task<IEnumerable<UnionDto>> SearchUnionsAsync(UnionSearchDto searchDto);
        Task<IEnumerable<UnionDto>> GetRuralUnionsAsync();
        Task<IEnumerable<UnionDto>> GetUrbanUnionsAsync();

        // VDP operations
        Task UpdateVDPMemberCountAsync(int unionId, int maleCount, int femaleCount);
        Task UpdateAnsarMemberCountAsync(int unionId, int count);

        // Helper methods
        Task<string> GenerateUnionCodeAsync(int upazilaId);
        Task<IEnumerable<object>> GetUnionHierarchyAsync(int unionId);
    }

    public interface IStoreItemService
    {
        // Basic CRUD operations
        Task<IEnumerable<StoreItemDto>> GetAllStoreItemsAsync();
        Task<StoreItemDto> GetStoreItemByIdAsync(int id);
        Task<IEnumerable<StoreItemDto>> GetStoreItemsByStoreAsync(int? storeId);
        Task<IEnumerable<StoreItemDto>> GetStoreItemsByItemAsync(int? itemId);
        Task<int> CreateStoreItemAsync(StoreItemDto storeItemDto);
        Task UpdateStoreItemAsync(StoreItemDto storeItemDto);
        Task DeleteStoreItemAsync(int id);

        // Stock management
        Task<decimal> GetAvailableQuantityAsync(int? storeId, int itemId);
        Task<decimal> GetStoreItemQuantityAsync(int? storeId, int itemId);
        Task UpdateStockQuantityAsync(int? storeId, int itemId, decimal quantityChange, string reason, string updatedBy);
        Task TransferStockAsync(int fromStoreId, int toStoreId, int itemId, decimal quantity, string transferredBy);

        // Stock analysis
        Task<IEnumerable<StoreItemDto>> GetLowStockItemsAsync(int? storeId = null);
        Task<IEnumerable<StoreItemDto>> GetOutOfStockItemsAsync(int? storeId = null);
        Task<decimal> GetStockValueByStoreAsync(int? storeId);

    }

    public interface IStockEntryService
    {
        // Stock Entry Operations
        Task<PagedResult<StockEntryDto>> GetStockEntriesAsync(int pageNumber, int pageSize, int? storeId = null, string status = null);
        Task<StockEntryDto> GetStockEntryByIdAsync(int id);
        Task<StockEntryDto> CreateStockEntryAsync(StockEntryDto dto);
        Task<StockEntryDto> UpdateStockEntryAsync(int id, StockEntryDto dto);
        Task<bool> DeleteStockEntryAsync(int id);
        Task<string> GenerateEntryNoAsync();
        Task<bool> CompleteStockEntryAsync(int id);
        Task<bool> CancelStockEntryAsync(int id, string reason);

        // Stock Adjustment Operations
        Task<PagedResult<StockAdjustmentDto>> GetStockAdjustmentsAsync(int pageNumber, int pageSize, int? storeId = null, string status = null);
        Task<StockAdjustmentDto> GetStockAdjustmentByIdAsync(int id);
        Task<StockAdjustmentDto> CreateStockAdjustmentAsync(StockAdjustmentDto dto);
        Task<string> GenerateAdjustmentNoAsync();
        Task<bool> ApproveStockAdjustmentAsync(int id, string approvedBy);
        Task<bool> RejectStockAdjustmentAsync(int id, string rejectedBy, string reason);

        // Bulk Operations
        Task<BulkStockUploadDto> ValidateBulkUploadAsync(Stream fileStream, int? storeId);
        Task<BulkStockUploadDto> ProcessBulkUploadAsync(BulkStockUploadDto dto);
        Task<byte[]> GenerateBulkUploadTemplateAsync();

        // Stock Queries
        Task<decimal> GetCurrentStockAsync(int itemId, int? storeId);
        Task<IEnumerable<StockLevelReportDto>> GetStockLevelsAsync(int? storeId = null, bool lowStockOnly = false);
        Task<IEnumerable<StockLevelReportDto>> GetLowStockItemsAsync(int? storeId);
        Task<bool> CheckStockAvailabilityAsync(int itemId, int? storeId, decimal requiredQuantity);
        Task<Dictionary<int, decimal>> GetMultipleItemStocksAsync(int? storeId, List<int> itemIds);

        // Reports
        Task<byte[]> ExportStockReportAsync(int? storeId = null, string format = "excel");
        Task<IEnumerable<dynamic>> GetStockMovementReportAsync(int? storeId, DateTime fromDate, DateTime toDate);


        // Add these methods to existing interface
        Task<ServiceResult> BulkUploadStockAsync(BulkStockUploadDto bulkUpload);
        Task<Stream> DownloadBulkUploadTemplateAsync();
        Task<ValidationResult> ValidateBulkUploadFileAsync(Stream fileStream);
    }


    public interface ITemperatureLogService
    {
        Task<TemperatureLogDto> LogTemperatureAsync(TemperatureLogDto logDto);
        Task<IEnumerable<TemperatureLogDto>> GetTemperatureLogsAsync(int? storeId, DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<TemperatureLogDto>> GetAlertsAsync(int? storeId);
        Task<bool> CheckTemperatureComplianceAsync(int storeId);
        Task<byte[]> GenerateTemperatureReportAsync(int? storeId, DateTime fromDate, DateTime toDate);
        Task SendTemperatureAlertAsync(int storeId, decimal temperature, string reason);
        Task<TemperatureStatisticsDto> GetTemperatureStatisticsAsync(int? storeId, DateTime? fromDate, DateTime? toDate);
    }

    public interface IExpiryTrackingService
    {
        Task<ExpiryTrackingDto> AddExpiryTrackingAsync(ExpiryTrackingDto dto);
        Task<IEnumerable<ExpiryTrackingDto>> GetExpiringItemsAsync(int daysBeforeExpiry = 30);
        Task<IEnumerable<ExpiryTrackingDto>> GetExpiredItemsAsync();
        Task<bool> ProcessExpiredItemAsync(int id, string disposalReason);
        Task SendExpiryAlertsAsync();
        Task<byte[]> GenerateExpiryReportAsync(DateTime? fromDate, DateTime? toDate);
        Task<ExpiryStatisticsDto> GetExpiryStatisticsAsync(int? storeId);
    }

    public interface IInventoryCycleCountService
    {
        Task<CycleCountDto> CreateCycleCountAsync(CycleCountDto dto);
        Task<CycleCountDto> GetCycleCountByIdAsync(int id);
        Task<IEnumerable<CycleCountDto>> GetActiveCycleCountsAsync();
        Task<bool> StartCycleCountAsync(int id);
        Task<bool> AddCountItemAsync(int cycleCountId, CycleCountItemDto itemDto);
        Task<bool> CompleteCycleCountAsync(int id);
        Task<bool> ApproveAdjustmentsAsync(int id, string approvedBy);
        Task<byte[]> GenerateCycleCountReportAsync(int id);
        Task<CycleCountStatisticsDto> GetCycleCountStatisticsAsync(DateTime? fromDate, DateTime? toDate);

    }

    public interface IPhysicalInventoryService
    {
        Task<PhysicalInventoryDto> SchedulePhysicalCountAsync(ScheduleCountDto dto);
        Task<bool> RecordPhysicalCountAsync(int inventoryId, CountRecordDto dto);
        Task<VarianceAnalysisDto> AnalyzeVarianceAsync(int inventoryId);
        Task<bool> CreateStockAdjustmentAsync(int inventoryId, AdjustmentCreationDto dto);
        Task<bool> ApproveAndPostAdjustmentAsync(int inventoryId, ApprovalDto approvalDto);

        // Core CRUD
        Task<PhysicalInventoryDto> CreatePhysicalInventoryAsync(PhysicalInventoryDto dto);
        Task<PhysicalInventoryDto> GetPhysicalInventoryByIdAsync(int id);
        Task<IEnumerable<PhysicalInventoryDto>> GetAllPhysicalInventoriesAsync();
        Task<IEnumerable<PhysicalInventoryDto>> GetPhysicalInventoriesAsync(
            int? storeId, PhysicalInventoryStatus? status, string fiscalYear);

        // Initiate & Count
        Task<PhysicalInventoryDto> InitiatePhysicalInventoryAsync(PhysicalInventoryDto dto);
        Task<PhysicalInventoryDto> StartCountingAsync(int inventoryId);
        Task UpdateCountAsync(int inventoryId, int itemId, decimal quantity, string countedBy);
        Task UpdatePhysicalCountAsync(int inventoryId, List<PhysicalCountUpdateDto> counts);
        Task CompleteCountingAsync(int inventoryId);
        Task CompleteCountingAsync(int inventoryId, string completedBy);

        // Review & Approval
        Task<PhysicalInventoryDto> ReviewPhysicalInventoryAsync(
            int inventoryId, string reviewedBy, string reviewRemarks);
        Task<PhysicalInventoryDto> ApprovePhysicalInventoryAsync(
            int inventoryId, string approvedBy, string approvalRemarks, bool autoAdjust = false);
        Task<PhysicalInventoryDto> RejectPhysicalInventoryAsync(
            int inventoryId, string rejectedBy, string rejectionReason);

        // Variance & Analysis
        Task<VarianceAnalysisDto> GetVarianceAnalysisAsync(int inventoryId);
        Task RecountItemsAsync(int inventoryId, List<int> itemIds, string requestedBy);
        Task RecountItemAsync(int inventoryId, int itemId, decimal quantity);
        Task VerifyCountAsync(int inventoryId, string verifiedBy);

        // Reconciliation
        //Task CreateReconciliationFromCountAsync(int inventoryId);
        //Task<StockReconciliationDto> GetReconciliationByIdAsync(int reconciliationId);

        // Export & Reports
        Task<byte[]> ExportPhysicalInventoryAsync(int inventoryId, string format = "excel");
        Task<IEnumerable<PhysicalInventoryHistoryDto>> GetInventoryHistoryAsync(
            int? storeId = null, int? itemId = null);

        // Utility
        Task<IEnumerable<PhysicalInventoryDto>> GetScheduledCountsAsync(int storeId);
        Task<bool> CanUserInitiateCountAsync(string userId, int storeId);
        Task<PhysicalInventoryDto> SubmitForApprovalAsync(int inventoryId, string verifiedBy);
        Task<IEnumerable<ItemDto>> GetStoreItemsAsync(int storeId);
        Task<string> GetCurrentFiscalYearAsync();
        Task<IEnumerable<string>> GetAvailableFiscalYearsAsync();
        Task<PhysicalInventoryDto> CancelPhysicalInventoryAsync(
            int inventoryId, string cancelledBy, string cancellationReason);
        Task<List<PhysicalInventoryDto>> GetAllCountsAsync();
        Task<List<PhysicalInventoryDto>> GetPendingCountsAsync();

        Task<PhysicalInventory> GetPhysicalInventoryWithDetailsAsync(int id);
        Task<IEnumerable<PhysicalInventoryDto>> GetCompletedInventoriesAsync();

        // Schedule and Export methods
        Task<PhysicalInventoryDto> SchedulePhysicalInventoryAsync(PhysicalInventoryDto dto);
        Task<byte[]> ExportCountHistoryCsvAsync(int? storeId = null, PhysicalInventoryStatus? status = null, string fiscalYear = null);
        Task<byte[]> ExportCountHistoryPdfAsync(int? storeId = null, PhysicalInventoryStatus? status = null, string fiscalYear = null);
    }
    public interface IDigitalSignatureService
    {
        Task<Signature> SaveSignatureAsync(string referenceType, int referenceId, string signatureType,
    string signatureData, string signerName, string signerBadgeId, string signerDesignation);
        Task<Signature> GetSignatureAsync(string referenceType, int referenceId, string signatureType);
        Task<Signature> GetSignatureByIdAsync(int id);
        Task<bool> ValidateSignatureAsync(string signatureData);
        Task<string> ConvertSignatureToBase64Async(byte[] signatureBytes);
        Task<DigitalSignatureDto> CaptureSignatureAsync(DigitalSignatureDto dto);
        Task<DigitalSignature> CreateSignatureAsync(DigitalSignatureDto dto);
        Task<bool> VerifySignatureAsync(int signatureId, string verificationCode = null);
        Task<IEnumerable<DigitalSignatureDto>> GetSignaturesByEntityAsync(string entityType, int entityId);
        Task<BatchSignatureDto> CreateBatchSignatureAsync(BatchSignatureDto dto);
        Task<byte[]> GenerateSignatureImageAsync(string signatureData);
        Task<string> GenerateSignatureOTPAsync(string userId, string purpose);
        Task<bool> VerifyOTPAsync(string userId, string otp, string purpose);
        Task<SignatureAuditTrailDto> GetSignatureAuditTrailAsync(string entityType, int entityId);
        Task<IEnumerable<DigitalSignatureDto>> GetSignaturesByReferenceAsync(string referenceType, int referenceId);
        Task<bool> HasRequiredSignaturesAsync(string referenceType, int referenceId);
    }
    public interface IBatchTrackingService
    {
        Task<BatchTrackingDto> CreateBatchAsync(BatchTrackingDto dto);
        Task<IEnumerable<BatchTrackingDto>> GetBatchesForIssueAsync(int storeId, int itemId, decimal requiredQuantity, StockRotationMethod method);
        Task<bool> IssueFromBatchAsync(int batchId, decimal quantity, string reference, string issuedBy);
        Task<IEnumerable<ExpiringBatchDto>> GetExpiringBatchesAsync(int storeId, int daysBeforeExpiry = 30);
        Task<bool> TransferBatchAsync(TransferBatchDto dto);
        Task<bool> QuarantineBatchAsync(int batchId, string reason, string quarantinedBy);
        Task<BatchHistoryDto> GetBatchHistoryAsync(string batchNo);
        Task<BatchValuationDto> GetBatchValuationAsync(int storeId, DateTime? asOfDate = null);
        Task<int> GetExpiringCountAsync(int daysToExpire = 30);
        Task<BatchDto> CreateBatchAsync(BatchDto dto);
        Task<StockAllocationDto> AllocateStockAsync(int itemId, int storeId, decimal requiredQuantity, string rotationMethod = "FIFO");
        Task<bool> ConsumeBatchQuantityAsync(string batchNumber, decimal quantity);
        Task<IEnumerable<BatchDto>> GetExpiringBatchesAsync(int daysBeforeExpiry = 30);
        Task<BatchTraceabilityDto> GetBatchTraceabilityAsync(string batchNumber);
        Task<StockRotationReportDto> GetStockRotationAnalysisAsync(int storeId, string rotationMethod);
        Task<BatchDto> GetBatchByIdAsync(int id);
        Task<IEnumerable<BatchDto>> GetBatchesByItemAsync(int? itemId);
        Task<bool> UpdateBatchExpiryAsync(string batchNumber, DateTime newExpiryDate);

    }

    public interface IRequisitionService
    {
        Task<RequisitionDto> CreateRequisitionAsync(RequisitionDto dto);
        Task<RequisitionDto> GetRequisitionByIdAsync(int id);
        Task<IEnumerable<RequisitionDto>> GetAllRequisitionsAsync();
        Task<RequisitionDto> SubmitForApprovalAsync(int id);
        Task<RequisitionDto> ApproveRequisitionAsync(int id, string comments = null);
        Task<RequisitionDto> RejectRequisitionAsync(int id, string reason);
        Task<bool> UpdateFulfillmentStatusAsync(int id);
        Task<IEnumerable<RequisitionDto>> GetPendingApprovalsAsync(string approverRole);
        Task<IEnumerable<RequisitionDto>> GetRequisitionsByPriorityAsync(string priority);
        Task<bool> MarkAsConvertedToPurchaseOrderAsync(int requisitionId, int purchaseOrderId);
        Task<RequisitionDto> UpdatePurchaseOrderReferenceAsync(int requisitionId, int purchaseOrderId, string purchaseOrderNo);
        Task<bool> DeleteRequisitionAsync(int id);

    }

    public interface ICycleCountSchedulingService
    {
        Task<CycleCountScheduleDto> CreateScheduleAsync(CycleCountScheduleDto dto);
    }

    public interface IStockService
    {
        Task AdjustStockAsync(StockAdjustment adjustment);
        Task<decimal> GetAvailableStockAsync(int storeId, int itemId);
        Task<IEnumerable<StockDto>> GetStoreStockAsync(int storeId);
        Task<bool> ReserveStockAsync(int? storeId, int itemId, decimal? quantity);
        Task<bool> IssueStockAsync(int? storeId, int itemId, decimal quantity, string reference, string issuedBy);
        Task<bool> ReceiveStockAsync(int storeId, int itemId, decimal quantity, string reference, string receivedBy);
        Task<bool> TransferOutStockAsync(int fromStoreId, int itemId, decimal quantity, string reference, string transferredBy);
        Task<bool> TransferInStockAsync(int toStoreId, int itemId, decimal quantity, string reference, string receivedBy);
        Task<bool> AdjustStockAsync(int? storeId, int itemId, decimal adjustmentQuantity, string reason, string adjustedBy);
        Task<bool> WriteOffStockAsync(int storeId, int itemId, decimal quantity, string reason, string writtenOffBy);
        Task<IEnumerable<StockMovementDto>> GetStockMovementsAsync(int storeId, int? itemId = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<StockAnalysisDto> GetStockAnalysisAsync(int storeId);

        Task<IEnumerable<StockLevelDto>> GetLowStockItemsAsync(int? storeId);
        Task<decimal> GetTotalStockValueAsync();
        Task<int> GetStockAccuracyPercentageAsync();
        Task<decimal> GetInventoryTurnoverAsync();
        Task<int> GetFillRatePercentageAsync();
        Task<int> GetAverageLeadTimeDaysAsync();
    }

    public interface IPhysicalInventoryRepository : IRepository<PhysicalInventory>
    {
        Task<PhysicalInventory> GetPhysicalInventoryWithDetailsAsync(int id);
    }
    public interface IOrganizationService
    {
        Task<OrganizationStatsDto> GetOrganizationStatsAsync();
    }

    public interface IStockMovementService
    {
        // Stock Movement Operations
        Task<PagedResult<StockMovementDto>> GetStockMovementsAsync(
            int pageNumber, int pageSize, int? storeId = null, int? itemId = null,
            string movementType = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<StockMovementDto> GetStockMovementByIdAsync(int id);
        Task<StockMovementDto> CreateStockMovementAsync(StockMovementDto dto);
        Task<bool> DeleteStockMovementAsync(int id);
        // Movement History
        Task<IEnumerable<StockMovementDto>> GetItemMovementHistoryAsync(int itemId, int? storeId = null);
        Task<IEnumerable<StockMovementDto>> GetStoreMovementHistoryAsync(int storeId, DateTime date);
        Task<IEnumerable<StockMovementDto>> GetMovementsByReferenceAsync(string referenceType, string referenceNo);
        // Movement Analysis
        Task<StockMovementSummaryDto> GetMovementSummaryAsync(int? storeId, DateTime date);
        Task<decimal> GetStockBalanceAtDateAsync(int itemId, int storeId, DateTime date);
        Task<IEnumerable<StockMovementTrendDto>> GetMovementTrendsAsync(int? storeId, DateTime fromDate, DateTime toDate);
        Task<Dictionary<string, int>> GetMovementTypeCountsAsync(int? storeId, DateTime fromDate, DateTime toDate);
        // Stock Card
        Task<StockCardDto> GetStockCardAsync(int itemId, int storeId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<StockLedgerDto>> GetStockLedgerAsync(int? storeId, DateTime fromDate, DateTime toDate);
        // Export Operations
        Task<byte[]> ExportMovementsAsync(int? storeId, int? itemId, string movementType,
            DateTime fromDate, DateTime toDate, string format = "excel");
        Task<byte[]> GenerateStockCardReportAsync(int itemId, int storeId, DateTime fromDate, DateTime toDate);
        // Tracking Operations
        Task RecordMovementAsync(string movementType, int itemId, int? storeId, decimal quantity,
            string referenceType, string referenceNo, string remarks, string movedBy);
        Task RecordTransferMovementAsync(int itemId, int fromStoreId, int toStoreId, decimal quantity,
           string transferNo, string movedBy);
        Task RecordAdjustmentMovementAsync(int itemId, int storeId, decimal oldQuantity, decimal newQuantity,
            string adjustmentNo, string reason, string adjustedBy);
        // Validation
        Task<bool> ValidateMovementAsync(StockMovementDto dto);
        Task<IEnumerable<string>> GetMovementTypesAsync();
    }
    public interface IAllotmentLetterService
    {
        Task<AllotmentLetterDto> GetAllotmentLetterByIdAsync(int id);
        Task<AllotmentLetterDto> GetAllotmentLetterByNoAsync(string allotmentNo);
        Task<IEnumerable<AllotmentLetterDto>> GetAllAllotmentLettersAsync();
        Task<IEnumerable<AllotmentLetterDto>> GetActiveAllotmentLettersAsync();
        Task<IEnumerable<AllotmentLetterDto>> GetAllotmentLettersByStoreAsync(int storeId);
        Task<IEnumerable<AllotmentLetterDto>> GetAllotmentLettersByBattalionAsync(int battalionId);

        Task<AllotmentLetterDto> CreateAllotmentLetterAsync(AllotmentLetterDto dto);
        Task<AllotmentLetterDto> UpdateAllotmentLetterAsync(AllotmentLetterDto dto);
        Task<bool> DeleteAllotmentLetterAsync(int id);

        Task<ServiceResult> SubmitForApprovalAsync(int id, string submittedBy);
        Task<bool> ApproveAllotmentLetterAsync(int id, string approvedBy);
        Task<bool> RejectAllotmentLetterAsync(int id, string rejectedBy, string reason);
        Task<bool> CancelAllotmentLetterAsync(int id, string reason);

        Task<string> GenerateAllotmentNoAsync();
        Task<bool> ValidateAllotmentLetterForIssueAsync(int allotmentLetterId, List<IssueItemDto> items);
        Task<bool> UpdateIssuedQuantitiesAsync(int allotmentLetterId, List<IssueItemDto> items);
    }

    public interface ISignatoryPresetService
    {
        Task<IEnumerable<SignatoryPreset>> GetAllPresetsAsync();
        Task<IEnumerable<SignatoryPreset>> GetActivePresetsAsync();
        Task<SignatoryPreset> GetPresetByIdAsync(int id);
        Task<SignatoryPreset> GetDefaultPresetAsync();
        Task<SignatoryPreset> CreatePresetAsync(SignatoryPreset preset);
        Task<SignatoryPreset> UpdatePresetAsync(SignatoryPreset preset);
        Task<bool> DeletePresetAsync(int id);
        Task<bool> SetDefaultPresetAsync(int id);
    }
    public interface IDataSeederService
    {
        Task SeedComprehensiveDataAsync();
    }

    public interface IVoucherService
    {
        // Issue Voucher Operations
        Task<string> GenerateIssueVoucherAsync(int issueId);
        Task<byte[]> GenerateIssueVoucherPdfAsync(int issueId);
        Task<bool> RegenerateIssueVoucherAsync(int issueId);

        // Receive Voucher Operations
        Task<string> GenerateReceiveVoucherAsync(int receiveId);
        Task<byte[]> GenerateReceiveVoucherPdfAsync(int receiveId);
        Task<bool> RegenerateReceiveVoucherAsync(int receiveId);

        // Voucher Retrieval
        Task<byte[]> GetIssueVoucherPdfAsync(int issueId);
        Task<byte[]> GetReceiveVoucherPdfAsync(int receiveId);
        Task<string> GetVoucherPathAsync(string voucherNo, string voucherType);

        // Voucher Validation
        Task<bool> ValidateVoucherAsync(string voucherNo);
        Task<bool> VoucherExistsAsync(string voucherNo, string voucherType);
    }

    public interface ILedgerBookService
    {
        // CRUD Operations
        Task<LedgerBookDto> GetLedgerBookByIdAsync(int id);
        Task<LedgerBookDto> GetLedgerBookByLedgerNoAsync(string ledgerNo);
        Task<IEnumerable<LedgerBookDto>> GetAllLedgerBooksAsync();
        Task<IEnumerable<LedgerBookDto>> GetActiveLedgerBooksAsync();
        Task<IEnumerable<LedgerBookDto>> GetLedgerBooksByStoreAsync(int storeId);
        Task<IEnumerable<LedgerBookDto>> GetLedgerBooksByTypeAsync(string bookType);
        Task<IEnumerable<LedgerBookDto>> GetActiveLedgerBooksByStoreAndTypeAsync(int? storeId, string bookType);

        Task<int> CreateLedgerBookAsync(LedgerBookCreateDto dto, string createdBy);
        Task<bool> UpdateLedgerBookAsync(int id, LedgerBookDto dto, string updatedBy);
        Task<bool> DeleteLedgerBookAsync(int id);
        Task<bool> CloseLedgerBookAsync(int id, string closedBy);

        // Page Management
        Task<int> GetNextAvailablePageAsync(int ledgerBookId);
        Task<int> GetNextAvailablePageByLedgerNoAsync(string ledgerNo);
        Task<bool> IncrementPageNumberAsync(int ledgerBookId);
        Task<bool> IncrementPageNumberAsync(string ledgerNo);

        // Validation
        Task<bool> IsLedgerBookFullAsync(int ledgerBookId);
        Task<bool> IsLedgerNoUniqueAsync(string ledgerNo, int? excludeId = null);
        Task<bool> CanUseLedgerBookAsync(int ledgerBookId);

        // Statistics
        Task<int> GetPagesUsedAsync(int ledgerBookId);
        Task<int> GetPagesRemainingAsync(int ledgerBookId);
        Task<IEnumerable<LedgerBookDto>> GetAlmostFullLedgerBooksAsync(int threshold = 10);
    }
}