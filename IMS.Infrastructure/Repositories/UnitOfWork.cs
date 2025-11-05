using IMS.Application.DTOs;
using IMS.Application.Interfaces;
using IMS.Domain.Entities;
using IMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Range = IMS.Domain.Entities.Range;

namespace IMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        // Repository field declarations (private fields)
        private IRepository<Category> _categories;
        private IRepository<SubCategory> _subCategories;
        private IRepository<Brand> _brands;
        private IRepository<ItemModel> _itemModels;
        private IRepository<Item> _items;
        private IRepository<Store> _stores;
        private IRepository<StoreItem> _storeItems;
        private IRepository<LedgerBook> _ledgerBooks;
        private IRepository<Vendor> _vendors;
        private IRepository<Purchase> _purchases;
        private IRepository<PurchaseItem> _purchaseItems;
        private IRepository<Issue> _issues;
        private IRepository<IssueItem> _issueItems;
        private IRepository<Receive> _receives;
        private IRepository<ReceiveItem> _receiveItems;
        private IRepository<Transfer> _transfers;
        private IRepository<TransferItem> _transferItems;
        private IRepository<WriteOff> _writeOffs;
        private IRepository<WriteOffItem> _writeOffItems;
        private IRepository<Damage> _damages;
        private IRepository<Return> _returns;
        private IRepository<Barcode> _barcodes;
        private IRepository<LoginLog> _loginLogs;
        private IRepository<ActivityLog> _activityLogs;
        private IRepository<StockAdjustment> _stockAdjustments;
        private IRepository<Notification> _notifications;
        private IRepository<Setting> _settings;
        private IRepository<Audit> _audits;
        private IRepository<RolePermission> _rolePermissions;
        private IRepository<UserStore> _userStores;
        private IRepository<Battalion> _battalions;
        private IRepository<Range> _ranges;
        private IRepository<Zila> _zilas;
        private IRepository<Upazila> _upazilas;
        private IRepository<StoreConfiguration> _storeConfigurations;
        private IRepository<BattalionStore> _battalionStores;
        private IRepository<User> _users;
        private IRepository<StoreType> _storeTypes;
        private IRepository<StoreTypeCategory> _storeTypeCategories;
        private IRepository<ApprovalRequest> _approvals;
        private IRepository<Union> _unions;
        private IRepository<StockOperation> _stockOperations;
        private IRepository<IssueVoucher> _issueVouchers;
        private IRepository<StockReturn> _stockReturns;
        private IRepository<StockAlert> _stockAlerts;
        private IRepository<StockMovement> _stockMovements;
        private IRepository<ExpiryTracking> _expiryTrackings;
        private IRepository<StockEntry> _stockEntries;
        private IRepository<StockEntryItem> _stockEntryItems;
        private IRepository<StockAdjustmentItem> _stockAdjustmentItems;
        private IRepository<ShipmentTracking> _shipmentTrackings;
        private IRepository<TrackingHistory> _trackingHistories;
        private IRepository<AuditLog> _auditLogs;
        private IRepository<InventoryCycleCount> _inventoryCycleCounts;
        private IRepository<InventoryCycleCountItem> _inventoryCycleCountItems;
        private IRepository<InventoryValuation> _inventoryValuations;
        private IRepository<PhysicalInventory> _physicalInventories;
        private IRepository<PhysicalInventoryItem> _physicalInventoryItems;
        //private IRepository<StockReconciliation> _stockReconciliations;
        //private IRepository<StockReconciliationItem> _stockReconciliationItems;
        private IRepository<CycleCountSchedule> _cycleCountSchedules;
        private IRepository<DigitalSignature> _digitalSignatures;
        private IRepository<BatchTracking> _batchTrackings;
        private IRepository<BatchMovement> _batchMovements;
        private IRepository<Requisition> _requisitions;
        private IRepository<RequisitionItem> _requisitionItems;
        private IRepository<ApprovalThreshold> _approvalThresholds;
        private IRepository<ApprovalHistory> _approvalHistories;
        private IRepository<StoreStock> _storeStocks;
        private IRepository<AuditReport> _auditReports;
        private IRepository<PurchaseOrder> _purchaseOrders;
        private IRepository<PhysicalInventoryDetail> _physicalInventoryDetails;
        private IRepository<PurchaseOrderItem> _purchaseOrderItems;
        // Add these private fields:
        private IRepository<ApprovalRequest> _approvalRequests;
        private IRepository<ApprovalStep> _approvalSteps;
        private IRepository<ApprovalWorkflow> _approvalWorkflows;
        private IRepository<ApprovalDelegation> _approvalDelegations;
        private IRepository<WriteOffRequest> _writeOffRequests;
        private IRepository<UserNotificationPreferences> _userNotificationPreferences;
        private IRepository<UserStore> _storeUsers;
        private IRepository<TransferShipment> _transferShipments;
        private IRepository<TransferDiscrepancy> _transferDiscrepancies;
        private IRepository<DamageReport> _damageReports;
        private IRepository<DamageReportItem> _damageReportItems;
        private IRepository<DamageRecord> _damageRecords;
        private IRepository<ExpiredRecord> _expiredRecords;
        private IRepository<DisposalRecord> _disposalRecords;
        private IRepository<ConditionCheck> _conditionChecks;
        private IRepository<SignatureOTP> _signatureOTPs;
        private IRepository<BatchSignature> _batchSignatures;
        private IRepository<Unit> _units;
        private IRepository<Signature> _signatures;
        private IRepository<PersonnelItemIssue> _personnelItemIssues;
        private IRepository<AllotmentLetter> _allotmentLetters;
        private IRepository<AllotmentLetterItem> _allotmentLetterItems;

        // NEW: Multiple recipients support
        private IRepository<AllotmentLetterRecipient> _allotmentLetterRecipients;
        private IRepository<AllotmentLetterRecipientItem> _allotmentLetterRecipientItems;
        private IRepository<SignatoryPreset> _signatoryPresets;

        private IRepository<Document> _documents;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Repository properties - Matching IUnitOfWork interface exactly
        public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
        public IRepository<SubCategory> SubCategories => _subCategories ??= new Repository<SubCategory>(_context);
        public IRepository<Brand> Brands => _brands ??= new Repository<Brand>(_context);
        public IRepository<ItemModel> ItemModels => _itemModels ??= new Repository<ItemModel>(_context);
        public IRepository<Item> Items => _items ??= new Repository<Item>(_context);
        public IRepository<Store> Stores => _stores ??= new Repository<Store>(_context);
        public IRepository<StoreItem> StoreItems => _storeItems ??= new Repository<StoreItem>(_context);
        public IRepository<LedgerBook> LedgerBooks => _ledgerBooks ??= new Repository<LedgerBook>(_context);
        public IRepository<Vendor> Vendors => _vendors ??= new Repository<Vendor>(_context);
        public IRepository<Purchase> Purchases => _purchases ??= new Repository<Purchase>(_context);
        public IRepository<PurchaseItem> PurchaseItems => _purchaseItems ??= new Repository<PurchaseItem>(_context);
        public IRepository<Issue> Issues => _issues ??= new Repository<Issue>(_context);
        public IRepository<IssueItem> IssueItems => _issueItems ??= new Repository<IssueItem>(_context);
        public IRepository<Receive> Receives => _receives ??= new Repository<Receive>(_context);
        public IRepository<ReceiveItem> ReceiveItems => _receiveItems ??= new Repository<ReceiveItem>(_context);
        public IRepository<Transfer> Transfers => _transfers ??= new Repository<Transfer>(_context);
        public IRepository<TransferItem> TransferItems => _transferItems ??= new Repository<TransferItem>(_context);
        public IRepository<WriteOff> WriteOffs => _writeOffs ??= new Repository<WriteOff>(_context);
        public IRepository<WriteOffItem> WriteOffItems => _writeOffItems ??= new Repository<WriteOffItem>(_context);
        public IRepository<Damage> Damages => _damages ??= new Repository<Damage>(_context);
        public IRepository<Return> Returns => _returns ??= new Repository<Return>(_context);
        public IRepository<Barcode> Barcodes => _barcodes ??= new Repository<Barcode>(_context);
        public IRepository<LoginLog> LoginLogs => _loginLogs ??= new Repository<LoginLog>(_context);
        public IRepository<ActivityLog> ActivityLogs => _activityLogs ??= new Repository<ActivityLog>(_context);
        public IRepository<StockAdjustment> StockAdjustments => _stockAdjustments ??= new Repository<StockAdjustment>(_context);
        public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);
        public IRepository<Setting> Settings => _settings ??= new Repository<Setting>(_context);
        public IRepository<Audit> Audits => _audits ??= new Repository<Audit>(_context);
        public IRepository<RolePermission> RolePermissions => _rolePermissions ??= new Repository<RolePermission>(_context);
        public IRepository<UserStore> UserStores => _userStores ??= new Repository<UserStore>(_context);
        public IRepository<Battalion> Battalions => _battalions ??= new Repository<Battalion>(_context);
        public IRepository<Range> Ranges => _ranges ??= new Repository<Range>(_context);
        public IRepository<Zila> Zilas => _zilas ??= new Repository<Zila>(_context);
        public IRepository<Upazila> Upazilas => _upazilas ??= new Repository<Upazila>(_context);
        public IRepository<StoreConfiguration> StoreConfigurations => _storeConfigurations ??= new Repository<StoreConfiguration>(_context);
        public IRepository<BattalionStore> BattalionStores => _battalionStores ??= new Repository<BattalionStore>(_context);
        public IRepository<User> Users => _users ??= new Repository<User>(_context);
        public IRepository<StoreType> StoreTypes => _storeTypes ??= new Repository<StoreType>(_context);
        public IRepository<StoreTypeCategory> StoreTypeCategories => _storeTypeCategories ??= new Repository<StoreTypeCategory>(_context);
        public IRepository<ApprovalRequest> Approvals => _approvals ??= new Repository<ApprovalRequest>(_context);
        public IRepository<Union> Unions => _unions ??= new Repository<Union>(_context);
        public IRepository<StockOperation> StockOperations => _stockOperations ??= new Repository<StockOperation>(_context);
        public IRepository<IssueVoucher> IssueVouchers => _issueVouchers ??= new Repository<IssueVoucher>(_context);
        public IRepository<StockReturn> StockReturns => _stockReturns ??= new Repository<StockReturn>(_context);
        public IRepository<StockAlert> StockAlerts => _stockAlerts ??= new Repository<StockAlert>(_context);
        public IRepository<StockMovement> StockMovements => _stockMovements ??= new Repository<StockMovement>(_context);
        public IRepository<ExpiryTracking> ExpiryTrackings => _expiryTrackings ??= new Repository<ExpiryTracking>(_context);
        public IRepository<StockEntry> StockEntries => _stockEntries ??= new Repository<StockEntry>(_context);
        public IRepository<StockEntryItem> StockEntryItems => _stockEntryItems ??= new Repository<StockEntryItem>(_context);
        public IRepository<StockAdjustmentItem> StockAdjustmentItems => _stockAdjustmentItems ??= new Repository<StockAdjustmentItem>(_context);
        public IRepository<ShipmentTracking> ShipmentTrackings => _shipmentTrackings ??= new Repository<ShipmentTracking>(_context);
        public IRepository<TrackingHistory> TrackingHistories => _trackingHistories ??= new Repository<TrackingHistory>(_context);
        public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_context);
        public IRepository<InventoryCycleCount> InventoryCycleCounts => _inventoryCycleCounts ??= new Repository<InventoryCycleCount>(_context);
        public IRepository<InventoryCycleCountItem> InventoryCycleCountItems => _inventoryCycleCountItems ??= new Repository<InventoryCycleCountItem>(_context);
        public IRepository<InventoryValuation> InventoryValuations => _inventoryValuations ??= new Repository<InventoryValuation>(_context);
        public IRepository<PhysicalInventory> PhysicalInventories => _physicalInventories ??= new Repository<PhysicalInventory>(_context);
        public IRepository<PhysicalInventoryItem> PhysicalInventoryItems => _physicalInventoryItems ??= new Repository<PhysicalInventoryItem>(_context);
        //public IRepository<StockReconciliation> StockReconciliations => _stockReconciliations ??= new Repository<StockReconciliation>(_context);
        //public IRepository<StockReconciliationItem> StockReconciliationItems => _stockReconciliationItems ??= new Repository<StockReconciliationItem>(_context);
        public IRepository<CycleCountSchedule> CycleCountSchedules => _cycleCountSchedules ??= new Repository<CycleCountSchedule>(_context);
        public IRepository<DigitalSignature> DigitalSignatures => _digitalSignatures ??= new Repository<DigitalSignature>(_context);
        public IRepository<BatchTracking> BatchTrackings => _batchTrackings ??= new Repository<BatchTracking>(_context);
        public IRepository<BatchMovement> BatchMovements => _batchMovements ??= new Repository<BatchMovement>(_context);
        public IRepository<Requisition> Requisitions => _requisitions ??= new Repository<Requisition>(_context);
        public IRepository<RequisitionItem> RequisitionItems => _requisitionItems ??= new Repository<RequisitionItem>(_context);
        public IRepository<ApprovalThreshold> ApprovalThresholds => _approvalThresholds ??= new Repository<ApprovalThreshold>(_context);
        public IRepository<ApprovalHistory> ApprovalHistories => _approvalHistories ??= new Repository<ApprovalHistory>(_context);
        public IRepository<StoreStock> StoreStocks => _storeStocks ??= new Repository<StoreStock>(_context);
        public IRepository<AuditReport> AuditReports => _auditReports ??= new Repository<AuditReport>(_context);
        public IRepository<PurchaseOrder> PurchaseOrders => _purchaseOrders ??= new Repository<PurchaseOrder>(_context);
        public IRepository<PhysicalInventoryDetail> PhysicalInventoryDetails => _physicalInventoryDetails ??= new Repository<PhysicalInventoryDetail>(_context);
        public IRepository<PurchaseOrderItem> PurchaseOrderItems => _purchaseOrderItems ??= new Repository<PurchaseOrderItem>(_context);
        public IRepository<ApprovalRequest> ApprovalRequests => _approvalRequests ??= new Repository<ApprovalRequest>(_context);
        public IRepository<ApprovalStep> ApprovalSteps => _approvalSteps ??= new Repository<ApprovalStep>(_context);
        public IRepository<ApprovalWorkflow> ApprovalWorkflows => _approvalWorkflows ??= new Repository<ApprovalWorkflow>(_context);
        public IRepository<ApprovalDelegation> ApprovalDelegations => _approvalDelegations ??= new Repository<ApprovalDelegation>(_context);
        public IRepository<WriteOffRequest> WriteOffRequests => _writeOffRequests ??= new Repository<WriteOffRequest>(_context);
        public IRepository<UserNotificationPreferences> UserNotificationPreferences => _userNotificationPreferences ??= new Repository<UserNotificationPreferences>(_context);
        public IRepository<UserStore> StoreUsers => _storeUsers ??= new Repository<UserStore>(_context);
        public IRepository<TransferShipment> TransferShipments => _transferShipments ??= new Repository<TransferShipment>(_context);
        public IRepository<TransferDiscrepancy> TransferDiscrepancies => _transferDiscrepancies ??= new Repository<TransferDiscrepancy>(_context);
        public IRepository<DamageReport> DamageReports => _damageReports ??= new Repository<DamageReport>(_context);
        public IRepository<DamageReportItem> DamageReportItems => _damageReportItems ??= new Repository<DamageReportItem>(_context);
        public IRepository<DamageRecord> DamageRecords => _damageRecords ??= new Repository<DamageRecord>(_context);
        public IRepository<ExpiredRecord> ExpiredRecords => _expiredRecords ??= new Repository<ExpiredRecord>(_context);
        public IRepository<DisposalRecord> DisposalRecords => _disposalRecords ??= new Repository<DisposalRecord>(_context);
        public IRepository<ConditionCheck> ConditionChecks => _conditionChecks ??= new Repository<ConditionCheck>(_context);
        public IRepository<SignatureOTP> SignatureOTPs => _signatureOTPs ??= new Repository<SignatureOTP>(_context);
        public IRepository<BatchSignature> BatchSignatures => _batchSignatures ??= new Repository<BatchSignature>(_context);
        public IRepository<Unit> Units => _units ??= new Repository<Unit>(_context);
        public IRepository<Signature> Signatures => _signatures ??= new Repository<Signature>(_context);
        public IRepository<PersonnelItemIssue> PersonnelItemIssues => _personnelItemIssues ??= new Repository<PersonnelItemIssue>(_context);
        public IRepository<AllotmentLetter> AllotmentLetters => _allotmentLetters ??= new Repository<AllotmentLetter>(_context);
        public IRepository<AllotmentLetterItem> AllotmentLetterItems => _allotmentLetterItems ??= new Repository<AllotmentLetterItem>(_context);

        // NEW: Multiple recipients support
        public IRepository<AllotmentLetterRecipient> AllotmentLetterRecipients => _allotmentLetterRecipients ??= new Repository<AllotmentLetterRecipient>(_context);
        public IRepository<AllotmentLetterRecipientItem> AllotmentLetterRecipientItems => _allotmentLetterRecipientItems ??= new Repository<AllotmentLetterRecipientItem>(_context);
        public IRepository<SignatoryPreset> SignatoryPresets => _signatoryPresets ??= new Repository<SignatoryPreset>(_context);

        public IRepository<Document> Documents => _documents ??= new Repository<Document>(_context);


        // Transaction methods
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}