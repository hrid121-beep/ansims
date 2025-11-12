using IMS.Domain.Entities;
using IMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using ApprovalLevel = IMS.Domain.Entities.ApprovalLevel;
using Range = IMS.Domain.Entities.Range;
using UserStore = IMS.Domain.Entities.UserStore;

namespace IMS.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region Core DbSets - Organization Hierarchy (Ansar & VDP)
        public DbSet<Battalion> Battalions { get; set; }
        public DbSet<BattalionStore> BattalionStores { get; set; }
        public DbSet<Range> Ranges { get; set; }
        public DbSet<Zila> Zilas { get; set; }
        public DbSet<Upazila> Upazilas { get; set; }
        public DbSet<Union> Unions { get; set; }
        public DbSet<Location> Locations { get; set; }
        #endregion

        #region Catalog Management
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ItemModel> ItemModels { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemSpecification> ItemSpecifications { get; set; }
        public DbSet<Unit> Units { get; set; }
        #endregion

        #region Store Management
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreType> StoreTypes { get; set; }
        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<StoreTypeCategory> StoreTypeCategories { get; set; }
        public DbSet<StoreConfiguration> StoreConfigurations { get; set; }
        public DbSet<UserStore> UserStores { get; set; }
        public DbSet<StoreStock> StoreStocks { get; set; }
        public DbSet<LedgerBook> LedgerBooks { get; set; }
        #endregion

        #region Vendor & Procurement
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<GoodsReceive> GoodsReceives { get; set; }
        public DbSet<GoodsReceiveItem> GoodsReceiveItems { get; set; }
        #endregion

        #region Requisition & Approval
        public DbSet<Requisition> Requisitions { get; set; }
        public DbSet<RequisitionItem> RequisitionItems { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<ApprovalStep> ApprovalSteps { get; set; }
        public DbSet<ApprovalWorkflow> ApprovalWorkflows { get; set; }
        public DbSet<ApprovalWorkflowLevel> ApprovalWorkflowLevels { get; set; }
        public DbSet<ApprovalLevel> ApprovalLevels { get; set; }
        public DbSet<ApprovalDelegation> ApprovalDelegations { get; set; }
        public DbSet<ApprovalHistory> ApprovalHistories { get; set; }
        public DbSet<ApprovalThreshold> ApprovalThresholds { get; set; }
        #endregion

        #region Transaction Management
        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueItem> IssueItems { get; set; }
        public DbSet<IssueVoucher> IssueVouchers { get; set; }
        public DbSet<Receive> Receives { get; set; }
        public DbSet<ReceiveItem> ReceiveItems { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferItem> TransferItems { get; set; }
        public DbSet<TransferShipment> TransferShipments { get; set; }
        public DbSet<TransferShipmentItem> TransferShipmentItems { get; set; }
        public DbSet<TransferDiscrepancy> TransferDiscrepancies { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnItem> ReturnItems { get; set; }
        #endregion

        #region Stock Management
        public DbSet<StockEntry> StockEntries { get; set; }
        public DbSet<StockEntryItem> StockEntryItems { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }
        public DbSet<StockAdjustmentItem> StockAdjustmentItems { get; set; }
        public DbSet<StockAlert> StockAlerts { get; set; }
        public DbSet<StockReturn> StockReturns { get; set; }
        public DbSet<StockOperation> StockOperations { get; set; }
        #endregion

        #region Loss & Damage Management
        public DbSet<WriteOff> WriteOffs { get; set; }
        public DbSet<WriteOffItem> WriteOffItems { get; set; }
        public DbSet<WriteOffRequest> WriteOffRequests { get; set; }
        public DbSet<Damage> Damages { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<DamageReportItem> DamageReportItems { get; set; }
        public DbSet<DamageRecord> DamageRecords { get; set; }
        public DbSet<ExpiredRecord> ExpiredRecords { get; set; }
        public DbSet<DisposalRecord> DisposalRecords { get; set; }
        public DbSet<ConditionCheck> ConditionChecks { get; set; }
        public DbSet<ConditionCheckItem> ConditionCheckItems { get; set; }
        #endregion

        #region Physical Inventory & Counting
        public DbSet<PhysicalInventory> PhysicalInventories { get; set; }
        public DbSet<PhysicalInventoryItem> PhysicalInventoryItems { get; set; }
        public DbSet<PhysicalInventoryDetail> PhysicalInventoryDetails { get; set; }
        //public DbSet<StockReconciliation> StockReconciliations { get; set; }
        //public DbSet<StockReconciliationItem> StockReconciliationItems { get; set; }
        public DbSet<CycleCountSchedule> CycleCountSchedules { get; set; }
        public DbSet<InventoryCycleCount> InventoryCycleCounts { get; set; }
        public DbSet<InventoryCycleCountItem> InventoryCycleCountItems { get; set; }
        public DbSet<InventoryValuation> InventoryValuations { get; set; }
        #endregion

        #region Tracking & Monitoring
        public DbSet<Barcode> Barcodes { get; set; }
        public DbSet<BatchTracking> BatchTrackings { get; set; }
        public DbSet<BatchMovement> BatchMovements { get; set; }
        public DbSet<ExpiryTracking> ExpiryTrackings { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<TemperatureLog> TemperatureLogs { get; set; }
        public DbSet<ShipmentTracking> ShipmentTrackings { get; set; }
        public DbSet<TrackingHistory> TrackingHistories { get; set; }
        #endregion

        #region Quality & Compliance
        public DbSet<QualityCheck> QualityChecks { get; set; }
        public DbSet<QualityCheckItem> QualityCheckItems { get; set; }
        public DbSet<SupplierEvaluation> SupplierEvaluations { get; set; }
        #endregion

        #region Security & Access Control
        public DbSet<RolePermission> RolePermissions { get; set; }
        #endregion

        #region System Management
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<UserNotificationPreferences> UserNotificationPreferences { get; set; }
        #endregion

        #region Digital Signature & OTP
        public DbSet<DigitalSignature> DigitalSignatures { get; set; }
        public DbSet<SignatureOTP> SignatureOTPs { get; set; }
        public DbSet<BatchSignature> BatchSignatures { get; set; }
        #endregion

        #region Audit & Logging
        public DbSet<Audit> Audits { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuditReport> AuditReports { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
        #endregion

        #region Document Management
        public DbSet<Document> Documents { get; set; }
        #endregion

        #region Reporting & Analytics
        public DbSet<UsageStatistics> UsageStatistics { get; set; }
        #endregion
        public DbSet<Signature> Signatures { get; set; }
        public DbSet<PersonnelItemIssue> PersonnelItemIssues { get; set; }
        public DbSet<AllotmentLetter> AllotmentLetters { get; set; }
        public DbSet<AllotmentLetterItem> AllotmentLetterItems { get; set; }

        // NEW: Multiple recipients support
        public DbSet<AllotmentLetterRecipient> AllotmentLetterRecipients { get; set; }
        public DbSet<AllotmentLetterRecipientItem> AllotmentLetterRecipientItems { get; set; }
        public DbSet<SignatoryPreset> SignatoryPresets { get; set; }
        public DbSet<AllotmentLetterDistribution> AllotmentLetterDistributions { get; set; }


        // FIXED: Properly configure conventions for EF Core 9
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            try
            {
                Console.WriteLine("Configuring conventions for EF Core 9...");

                // Call base configuration first
                base.ConfigureConventions(configurationBuilder);

                // Configure all string properties to ensure they're not mistaken for collections
                configurationBuilder.Properties<string>()
                    .AreUnicode(true)
                    .HaveMaxLength(4000); // Set a reasonable default max length

                Console.WriteLine("Successfully configured conventions.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring conventions: {ex.Message}");
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            try
            {
                // Apply all configurations step by step
                Console.WriteLine("Configuring Identity Entities...");
                ConfigureIdentityEntities(modelBuilder);

                Console.WriteLine("Configuring Organization Entities...");
                ConfigureOrganizationEntities(modelBuilder);

                Console.WriteLine("Configuring Organizational Hierarchy...");
                ConfigureOrganizationalHierarchy(modelBuilder);

                Console.WriteLine("Configuring Catalog Entities...");
                ConfigureCatalogEntities(modelBuilder);

                Console.WriteLine("Configuring Product Entities...");
                ConfigureProductEntities(modelBuilder);

                Console.WriteLine("Configuring Store Entities...");
                ConfigureStoreEntities(modelBuilder);

                Console.WriteLine("Configuring Procurement Entities...");
                ConfigureProcurementEntities(modelBuilder);

                Console.WriteLine("Configuring Requisition Entities...");
                ConfigureRequisitionEntities(modelBuilder);

                Console.WriteLine("Configuring Transaction Entities...");
                ConfigureTransactionEntities(modelBuilder);

                Console.WriteLine("Configuring Stock Entities...");
                ConfigureStockEntities(modelBuilder);

                Console.WriteLine("Configuring Stock Management Entities...");
                ConfigureStockManagementEntities(modelBuilder);

                Console.WriteLine("Configuring Loss Management Entities...");
                ConfigureLossManagementEntities(modelBuilder);

                Console.WriteLine("Configuring Physical Inventory Entities...");
                ConfigurePhysicalInventoryEntities(modelBuilder);

                Console.WriteLine("Configuring Tracking Entities...");
                ConfigureTrackingEntities(modelBuilder);

                Console.WriteLine("Configuring Quality Entities...");
                ConfigureQualityEntities(modelBuilder);

                Console.WriteLine("Configuring System Entities...");
                ConfigureSystemEntities(modelBuilder);

                Console.WriteLine("Configuring Audit Entities...");
                ConfigureAuditEntities(modelBuilder);

                Console.WriteLine("Configuring Decimal Precisions...");
                ConfigureDecimalPrecisions(modelBuilder);

                Console.WriteLine("Configuring Batch Tracking Entities...");
                ConfigureBatchTrackingEntities(modelBuilder);

                Console.WriteLine("Configuring Indexes...");
                ConfigureIndexes(modelBuilder);

                // IMPORTANT: Configure problematic string properties last
                Console.WriteLine("Configuring problematic string properties...");
                ConfigureProblematicStringProperties(modelBuilder);

                Console.WriteLine("Model configuration completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnModelCreating: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        #region Configuration Methods

        private void ConfigureIdentityEntities(ModelBuilder modelBuilder)
        {
            // User entity extension configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.FullName).HasMaxLength(200);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Designation).HasMaxLength(100);
                entity.Property(e => e.BadgeNumber).HasMaxLength(50);

                entity.HasIndex(e => e.BadgeNumber).IsUnique().HasFilter("[BadgeNumber] IS NOT NULL");
            });

            // ApprovalLevel configuration
            modelBuilder.Entity<ApprovalLevel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Level).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.EntityType).HasMaxLength(100);
                entity.Property(e => e.IsSystemDefined).HasDefaultValue(false);

                entity.HasIndex(e => new { e.Level, e.EntityType })
                    .HasDatabaseName("IX_ApprovalLevel_Level_EntityType");
            });
        }

        private void ConfigureOrganizationEntities(ModelBuilder modelBuilder)
        {
            // Battalion
            modelBuilder.Entity<Battalion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameBn).HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.CommanderName).HasMaxLength(100);
                entity.Property(e => e.CommanderRank).HasMaxLength(50);
                entity.Property(e => e.ContactNumber).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);

                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Range
            modelBuilder.Entity<Range>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameBn).HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.HeadquarterLocation).HasMaxLength(200);
                entity.Property(e => e.CommanderName).HasMaxLength(100);
                entity.Property(e => e.CommanderRank).HasMaxLength(50);
                entity.Property(e => e.ContactNumber).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.CoverageArea).HasMaxLength(500);
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Zila
            modelBuilder.Entity<Zila>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameBn).HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);

                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Upazila
            modelBuilder.Entity<Upazila>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameBn).HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);

                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Union
            modelBuilder.Entity<Union>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameBn).HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);

                entity.HasIndex(e => e.Code).IsUnique();
            });
        }

        private void ConfigureOrganizationalHierarchy(ModelBuilder modelBuilder)
        {
            // Location configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.LocationType).HasMaxLength(50);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.ContactPhone).HasMaxLength(50);
                entity.Property(e => e.Latitude).HasPrecision(10, 6);
                entity.Property(e => e.Longitude).HasPrecision(10, 6);

                entity.HasIndex(e => e.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            });

            // BattalionStore configuration
            modelBuilder.Entity<BattalionStore>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => new { e.BattalionId, e.StoreId })
                    .IsUnique()
                    .HasFilter("[EffectiveTo] IS NULL");
            });

            // UserStore configuration
            modelBuilder.Entity<UserStore>(entity =>
            {
                entity.HasKey(us => us.Id);
                entity.Property(us => us.AssignedBy).HasMaxLength(450);

                entity.HasIndex(us => new { us.UserId, us.StoreId })
                    .IsUnique()
                    .HasFilter("[UnassignedDate] IS NULL");
            });
        }

        private void ConfigureCatalogEntities(ModelBuilder modelBuilder)
        {
            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameBn).HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // SubCategory configuration
            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameBn).HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // Brand configuration
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            });

            // Unit configuration
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
                entity.Property(e => e.ConversionFactor).HasColumnType("decimal(18,6)");
                entity.Property(e => e.BaseUnit).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.HasIndex(e => e.Code).IsUnique();
            });
        }

        private void ConfigureProductEntities(ModelBuilder modelBuilder)
        {
            // Item configuration
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ItemCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NameBn).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MinimumStock).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MaximumStock).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18,2)");

                entity.HasIndex(e => e.ItemCode).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
            });
        }

        private void ConfigureStoreEntities(ModelBuilder modelBuilder)
        {
            // Store configuration
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NameBn).HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Type).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.ContactNumber).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Capacity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalCapacity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UsedCapacity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AvailableCapacity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OperatingHours).HasMaxLength(100);
                entity.Property(e => e.Level).HasConversion<int>();

                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);

                // Configure relationships
                entity.HasOne(s => s.StoreType)
                    .WithMany(st => st.Stores)
                    .HasForeignKey(s => s.StoreTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Battalion)
                    .WithMany(b => b.Stores)
                    .HasForeignKey(s => s.BattalionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Range)
                    .WithMany(r => r.Stores)
                    .HasForeignKey(s => s.RangeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Zila)
                    .WithMany(z => z.Stores)
                    .HasForeignKey(s => s.ZilaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Upazila)
                    .WithMany(u => u.Stores)
                    .HasForeignKey(s => s.UpazilaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Union)
                    .WithMany(u => u.Stores)
                    .HasForeignKey(s => s.UnionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.LocationDetail)
                    .WithMany(l => l.Stores)
                    .HasForeignKey(s => s.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Collections are configured in their respective entity configurations
                // TransfersFrom and TransfersTo are configured in ConfigureTransactionEntities
                // Issues, Receives, StoreItems, etc. are configured in their respective methods
            });

            // StoreType configuration
            modelBuilder.Entity<StoreType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.Color).HasMaxLength(20);
                entity.Property(e => e.DefaultManagerRole).HasMaxLength(50);
                entity.HasIndex(e => e.Code).IsUnique();
            });

            // StoreTypeCategory configuration
            modelBuilder.Entity<StoreTypeCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.StoreTypeId, e.CategoryId }).IsUnique();

                entity.HasOne(stc => stc.StoreType)
                    .WithMany(st => st.StoreTypeCategories)
                    .HasForeignKey(stc => stc.StoreTypeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(stc => stc.Category)
                    .WithMany(c => c.StoreTypeCategories)
                    .HasForeignKey(stc => stc.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // StoreConfiguration configuration
            modelBuilder.Entity<StoreConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ConfigKey).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ConfigValue).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(sc => sc.Store)
                    .WithMany(s => s.StoreConfigurations)
                    .HasForeignKey(sc => sc.StoreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // StoreItem configuration
            modelBuilder.Entity<StoreItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.CurrentStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.MinimumStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.MaximumStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReorderLevel).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReservedStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.LastCountQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.Status).HasConversion<int>();

                // CRITICAL FIX: Configure RowVersion for optimistic concurrency control
                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(si => si.Store)
                    .WithMany(s => s.StoreItems)
                    .HasForeignKey(si => si.StoreId)
                    .OnDelete(DeleteBehavior.Restrict); // CRITICAL FIX: Prevent cascade delete of all stock

                entity.HasOne(si => si.Item)
                    .WithMany(i => i.StoreItems)
                    .HasForeignKey(si => si.ItemId)
                    .OnDelete(DeleteBehavior.Restrict); // CRITICAL FIX: Prevent cascade delete across all stores

                entity.HasIndex(e => new { e.StoreId, e.ItemId }).IsUnique();
            });

            // UserStore configuration
            modelBuilder.Entity<UserStore>(entity =>
            {
                entity.HasKey(us => us.Id);
                entity.Property(us => us.AssignedBy).HasMaxLength(450);
                entity.Property(us => us.Role).HasMaxLength(50);

                entity.HasOne(us => us.User)
                    .WithMany(u => u.UserStores)
                    .HasForeignKey(us => us.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(us => us.Store)
                    .WithMany(s => s.UserStores)
                    .HasForeignKey(us => us.StoreId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(us => new { us.UserId, us.StoreId })
                    .IsUnique()
                    .HasFilter("[UnassignedDate] IS NULL");
            });

            // StoreStock configuration (if entity exists)
            modelBuilder.Entity<StoreStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MinQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.MaxQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");

                entity.HasOne<Store>()
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Item>()
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.StoreId, e.ItemId }).IsUnique();
            });
        }

        private void ConfigureProcurementEntities(ModelBuilder modelBuilder)
        {
            // Vendor configuration
            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.VendorType).HasMaxLength(50);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Mobile).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.TIN).HasMaxLength(50);
                entity.Property(e => e.BIN).HasMaxLength(50);

                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Email);
            });

            // Purchase configuration
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PurchaseOrderNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.PurchaseOrderNo).IsUnique();
            });
        }

        private void ConfigureRequisitionEntities(ModelBuilder modelBuilder)
        {
            // Requisition configuration
            modelBuilder.Entity<Requisition>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequisitionNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalEstimatedCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EstimatedValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ApprovedValue).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.RequisitionNumber).IsUnique();
            });
        }

        private void ConfigureTransactionEntities(ModelBuilder modelBuilder)
        {
            // Issue configuration - FIXED: Use FromStore instead of Store
            modelBuilder.Entity<Issue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IssueNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Purpose).HasMaxLength(500);
                entity.Property(e => e.Remarks).HasMaxLength(500);

                // ✅ FIX: Support large Base64 signature data
                entity.Property(e => e.SignaturePath).HasMaxLength(int.MaxValue);

                entity.HasIndex(e => e.IssueNo).IsUnique();

                entity.HasOne(i => i.FromStore)
                    .WithMany(s => s.Issues)
                    .HasForeignKey(i => i.FromStoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ADD THIS relationship:
                entity.HasOne(e => e.AllotmentLetter)
                    .WithMany(a => a.Issues)
                    .HasForeignKey(e => e.AllotmentLetterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // IssueItem configuration
            modelBuilder.Entity<IssueItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ApprovedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.IssuedQuantity).HasColumnType("decimal(18,3)");

                entity.HasOne(ii => ii.Issue)
                    .WithMany(i => i.Items)
                    .HasForeignKey(ii => ii.IssueId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ii => ii.Item)
                    .WithMany(i => i.IssueItems)
                    .HasForeignKey(ii => ii.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ii => ii.Store)
                    .WithMany(s => s.IssueItems)
                    .HasForeignKey(ii => ii.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Receive configuration
            modelBuilder.Entity<Receive>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReceiveNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.HasIndex(e => e.ReceiveNo).IsUnique();

                entity.HasOne(r => r.Store)
                    .WithMany(s => s.Receives)
                    .HasForeignKey(r => r.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ReceiveItem configuration
            modelBuilder.Entity<ReceiveItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReceivedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.IssuedQuantity).HasColumnType("decimal(18,3)");

                entity.HasOne(ri => ri.Receive)
                    .WithMany(r => r.ReceiveItems)
                    .HasForeignKey(ri => ri.ReceiveId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ri => ri.Item)
                    .WithMany()
                    .HasForeignKey(ri => ri.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ri => ri.Store)
                    .WithMany(s => s.ReceiveItems)
                    .HasForeignKey(ri => ri.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Transfer configuration - IMPORTANT: Configure both FromStore and ToStore relationships
            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TransferNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.TransferType).HasMaxLength(50);
                entity.Property(e => e.Purpose).HasMaxLength(200);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.TransferNo).IsUnique();

                // Configure the FromStore relationship
                entity.HasOne(t => t.FromStore)
                    .WithMany(s => s.TransfersFrom)
                    .HasForeignKey(t => t.FromStoreId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Transfer_Store_FromStoreId");

                // Configure the ToStore relationship
                entity.HasOne(t => t.ToStore)
                    .WithMany(s => s.TransfersTo)
                    .HasForeignKey(t => t.ToStoreId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Transfer_Store_ToStoreId");

                // Configure the Shipment relationship
                entity.HasOne(t => t.Shipment)
                    .WithOne(ts => ts.Transfer)
                    .HasForeignKey<TransferShipment>(ts => ts.TransferId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TransferItem configuration
            modelBuilder.Entity<TransferItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.RequestedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ApprovedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ShippedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReceivedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)");

                entity.HasOne(ti => ti.Transfer)
                    .WithMany(t => t.Items)
                    .HasForeignKey(ti => ti.TransferId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ti => ti.Item)
                    .WithMany(i => i.TransferItems)
                    .HasForeignKey(ti => ti.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TransferShipment configuration
            modelBuilder.Entity<TransferShipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ShipmentNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Carrier).HasMaxLength(100);
                entity.Property(e => e.TrackingNo).HasMaxLength(100);
                entity.Property(e => e.TransportCompany).HasMaxLength(100);
                entity.Property(e => e.VehicleNo).HasMaxLength(50);
                entity.Property(e => e.DriverName).HasMaxLength(100);
                entity.Property(e => e.DriverContact).HasMaxLength(50);
                entity.Property(e => e.SealNo).HasMaxLength(50);
                entity.HasIndex(e => e.ShipmentNo).IsUnique();
            });

            // TransferShipmentItem configuration
            modelBuilder.Entity<TransferShipmentItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ShippedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.PackageNo).HasMaxLength(50);
                entity.Property(e => e.Condition).HasMaxLength(50);
                entity.Property(e => e.BatchNo).HasMaxLength(50);

                entity.HasOne(tsi => tsi.Shipment)
                    .WithMany(ts => ts.Items)
                    .HasForeignKey(tsi => tsi.ShipmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tsi => tsi.Item)
                    .WithMany()
                    .HasForeignKey(tsi => tsi.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TransferDiscrepancy configuration
            modelBuilder.Entity<TransferDiscrepancy>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExpectedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ActualQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.DiscrepancyQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ShippedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReceivedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Variance).HasColumnType("decimal(18,3)");

                entity.HasOne(td => td.Transfer)
                    .WithMany()
                    .HasForeignKey(td => td.TransferId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(td => td.Item)
                    .WithMany()
                    .HasForeignKey(td => td.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Return configuration
            modelBuilder.Entity<Return>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReturnNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.HasIndex(e => e.ReturnNo).IsUnique();

                entity.HasOne(r => r.Store)
                    .WithMany()
                    .HasForeignKey(r => r.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.ToStore)
                    .WithMany()
                    .HasForeignKey(r => r.ToStoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Item)
                    .WithMany()
                    .HasForeignKey(r => r.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ReturnItem configuration
            modelBuilder.Entity<ReturnItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReturnQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ApprovedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReceivedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.AcceptedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.RejectedQuantity).HasColumnType("decimal(18,3)");

                entity.HasOne(ri => ri.Return)
                    .WithMany(r => r.Items)
                    .HasForeignKey(ri => ri.ReturnId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ri => ri.Item)
                    .WithMany()
                    .HasForeignKey(ri => ri.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // IssueVoucher configuration
            modelBuilder.Entity<IssueVoucher>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.VoucherNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.VoucherNo).HasMaxLength(50);
                entity.Property(e => e.IssuedTo).HasMaxLength(200);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Purpose).HasMaxLength(500);
                entity.Property(e => e.AuthorizedBy).HasMaxLength(100);
                entity.Property(e => e.ReceivedBy).HasMaxLength(100);
                entity.Property(e => e.VoucherBarcode).HasMaxLength(200);

                entity.HasOne(iv => iv.Issue)
                    .WithMany()
                    .HasForeignKey(iv => iv.IssueId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.VoucherNumber).IsUnique();
            });

            // Signature configuration
            modelBuilder.Entity<Signature>(entity =>
            {
                entity.ToTable("Signatures");
                entity.HasIndex(e => new { e.ReferenceType, e.ReferenceId });

                entity.Property(e => e.ReferenceType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SignatureType)
                    .HasMaxLength(50);

                entity.Property(e => e.SignatureData)
                    .HasColumnType("nvarchar(max)");
            });

            // Issue-Signature relationships
            modelBuilder.Entity<Issue>()
                .HasOne(i => i.IssuerSignature)
                .WithMany()
                .HasForeignKey(i => i.IssuerSignatureId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.ApproverSignature)
                .WithMany()
                .HasForeignKey(i => i.ApproverSignatureId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Issue>()
                .HasOne(i => i.ReceiverSignature)
                .WithMany()
                .HasForeignKey(i => i.ReceiverSignatureId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PersonnelItemIssue>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.IssueNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PersonnelType)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.PersonnelName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PersonnelBadgeNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(e => e.Item)
                    .WithMany(i => i.PersonnelIssues)
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Battalion)
                    .WithMany()
                    .HasForeignKey(e => e.BattalionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Store)
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CRITICAL FIX: Add missing foreign key constraints
                entity.HasOne<Issue>()
                    .WithMany()
                    .HasForeignKey(e => e.OriginalIssueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Receive>()
                    .WithMany()
                    .HasForeignKey(e => e.ReceiveId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PersonnelBadgeNo);
                entity.HasIndex(e => e.LifeExpiryDate);
                entity.HasIndex(e => e.Status);
            });

            // AllotmentLetter configuration
            modelBuilder.Entity<AllotmentLetter>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AllotmentNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.AllotmentNo)
                    .IsUnique();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                // Relationships
                entity.HasOne(e => e.FromStore)
                    .WithMany()
                    .HasForeignKey(e => e.FromStoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.IssuedToBattalion)
                    .WithMany()
                    .HasForeignKey(e => e.IssuedToBattalionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.IssuedToRange)
                    .WithMany()
                    .HasForeignKey(e => e.IssuedToRangeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.IssuedToZila)
                    .WithMany()
                    .HasForeignKey(e => e.IssuedToZilaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.IssuedToUpazila)
                    .WithMany()
                    .HasForeignKey(e => e.IssuedToUpazilaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // AllotmentLetterItem configuration
            modelBuilder.Entity<AllotmentLetterItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AllottedQuantity)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.IssuedQuantity)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RemainingQuantity)
                    .HasColumnType("decimal(18,2)");

                // Relationships
                entity.HasOne(e => e.AllotmentLetter)
                    .WithMany(a => a.Items)
                    .HasForeignKey(e => e.AllotmentLetterId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Item)
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CRITICAL FIX: Add AllotmentLetterRecipient configuration
            modelBuilder.Entity<AllotmentLetterRecipient>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RecipientType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RecipientName)
                    .IsRequired()
                    .HasMaxLength(200);

                // AllotmentLetter FK
                entity.HasOne<AllotmentLetter>()
                    .WithMany()
                    .HasForeignKey(e => e.AllotmentLetterId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Organization FKs
                entity.HasOne<Range>()
                    .WithMany()
                    .HasForeignKey(e => e.RangeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Battalion>()
                    .WithMany()
                    .HasForeignKey(e => e.BattalionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Zila>()
                    .WithMany()
                    .HasForeignKey(e => e.ZilaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Upazila>()
                    .WithMany()
                    .HasForeignKey(e => e.UpazilaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Union>()
                    .WithMany()
                    .HasForeignKey(e => e.UnionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AllotmentLetterId);
                entity.HasIndex(e => e.RecipientType);
            });

            // CRITICAL FIX: Add AllotmentLetterRecipientItem configuration
            modelBuilder.Entity<AllotmentLetterRecipientItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AllottedQuantity)
                    .HasColumnType("decimal(18,3)");

                entity.Property(e => e.IssuedQuantity)
                    .HasColumnType("decimal(18,3)");

                entity.HasOne(e => e.Recipient)
                    .WithMany()
                    .HasForeignKey(e => e.AllotmentLetterRecipientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Item)
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AllotmentLetterRecipientId);
                entity.HasIndex(e => e.ItemId);
            });

            // CRITICAL FIX: Add AllotmentLetterDistribution configuration
            modelBuilder.Entity<AllotmentLetterDistribution>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RecipientTitle)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.RecipientTitleBn)
                    .HasMaxLength(500);

                entity.Property(e => e.Address)
                    .HasMaxLength(1000);

                entity.Property(e => e.AddressBn)
                    .HasMaxLength(1000);

                entity.Property(e => e.Purpose)
                    .HasMaxLength(500);

                entity.Property(e => e.PurposeBn)
                    .HasMaxLength(500);

                entity.HasOne(e => e.AllotmentLetter)
                    .WithMany()
                    .HasForeignKey(e => e.AllotmentLetterId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.AllotmentLetterId);
                entity.HasIndex(e => e.SerialNo);
            });
        }

        private void ConfigureStockEntities(ModelBuilder modelBuilder)
        {
            // StockMovement configuration
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

                // CRITICAL FIX: Add foreign key constraints for StockMovement
                entity.HasOne<Item>()
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Store>()
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Store>()
                    .WithMany()
                    .HasForeignKey(e => e.SourceStoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Store>()
                    .WithMany()
                    .HasForeignKey(e => e.DestinationStoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ItemId);
                entity.HasIndex(e => e.StoreId);
                entity.HasIndex(e => e.MovementDate);
                entity.HasIndex(e => new { e.ReferenceType, e.ReferenceId });
            });
        }

        private void ConfigureStockManagementEntities(ModelBuilder modelBuilder)
        {
            // StockAdjustment configuration
            modelBuilder.Entity<StockAdjustment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AdjustmentNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AdjustmentType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.OldQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.NewQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.AdjustmentQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
                entity.Property(e => e.ReferenceDocument).HasMaxLength(200);
                entity.Property(e => e.ApprovalReference).HasMaxLength(50);
                entity.Property(e => e.FiscalYear).HasMaxLength(20);
                entity.Property(e => e.AdjustedBy).HasMaxLength(450);
                entity.Property(e => e.ApprovedBy).HasMaxLength(450);
                entity.Property(e => e.ApprovalRemarks).HasMaxLength(500);
                entity.Property(e => e.RejectedBy).HasMaxLength(450);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => e.AdjustmentNo).IsUnique();
                entity.HasIndex(e => e.AdjustmentDate);

                entity.HasOne<Item>()
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Store>()
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<PhysicalInventory>()
                    .WithMany()
                    .HasForeignKey(e => e.PhysicalInventoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StockAdjustmentItem configuration
            modelBuilder.Entity<StockAdjustmentItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SystemQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ActualQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.PhysicalQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.AdjustmentQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.AdjustmentValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BatchNo).HasMaxLength(50);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasOne(sai => sai.StockAdjustment)
                    .WithMany(sa => sa.Items)
                    .HasForeignKey(sai => sai.StockAdjustmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sai => sai.Item)
                    .WithMany()
                    .HasForeignKey(sai => sai.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StockEntry configuration
            modelBuilder.Entity<StockEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntryNo).HasMaxLength(50);
                entity.Property(e => e.EntryType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => e.EntryNo).IsUnique().HasFilter("[EntryNo] IS NOT NULL");
                entity.HasIndex(e => e.EntryDate);

                entity.HasOne(se => se.Store)
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StockEntryItem configuration
            modelBuilder.Entity<StockEntryItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.BatchNumber).HasMaxLength(50);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasOne(sei => sei.StockEntry)
                    .WithMany(se => se.Items)
                    .HasForeignKey(sei => sei.StockEntryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sei => sei.Item)
                    .WithMany()
                    .HasForeignKey(sei => sei.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StockAlert configuration
            modelBuilder.Entity<StockAlert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AlertType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.CurrentStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.MinimumStock).HasColumnType("decimal(18,3)");
                entity.Property(e => e.CurrentQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ThresholdQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Message).HasMaxLength(1000);
                entity.Property(e => e.AcknowledgedBy).HasMaxLength(450);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => e.AlertDate);
                entity.HasIndex(e => e.Status);

                entity.HasOne(sa => sa.Item)
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sa => sa.Store)
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StockReturn configuration
            modelBuilder.Entity<StockReturn>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReturnNumber).HasMaxLength(50);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Condition).HasMaxLength(100);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReturnedBy).HasMaxLength(450);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => e.ReturnNumber).IsUnique().HasFilter("[ReturnNumber] IS NOT NULL");
                entity.HasIndex(e => e.ReturnDate);

                entity.HasOne(sr => sr.OriginalIssue)
                    .WithMany()
                    .HasForeignKey(e => e.OriginalIssueId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sr => sr.Item)
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // StockOperation configuration
            modelBuilder.Entity<StockOperation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OperationType).HasMaxLength(50);
                entity.Property(e => e.ReferenceNumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ApprovedBy).HasMaxLength(450);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(so => so.Item)
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(so => so.FromStore)
                    .WithMany()
                    .HasForeignKey(e => e.FromStoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(so => so.ToStore)
                    .WithMany()
                    .HasForeignKey(e => e.ToStoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //// StockReconciliation configuration
            //modelBuilder.Entity<StockReconciliation>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.ReconciliationNo).HasMaxLength(50);
            //    entity.Property(e => e.Status).HasMaxLength(20);
            //    entity.Property(e => e.TotalVariance).HasColumnType("decimal(18,3)");
            //    entity.Property(e => e.TotalVarianceValue).HasColumnType("decimal(18,2)");
            //    entity.Property(e => e.ReconciliationType).HasMaxLength(50);
            //    entity.Property(e => e.TotalVarianceQuantity).HasColumnType("decimal(18,3)");
            //    entity.Property(e => e.Reason).HasMaxLength(500);
            //    entity.Property(e => e.InitiatedBy).HasMaxLength(450);
            //    entity.Property(e => e.ApprovedBy).HasMaxLength(450);
            //    entity.Property(e => e.ApprovalComments).HasMaxLength(500);
            //    entity.Property(e => e.RejectedBy).HasMaxLength(450);
            //    entity.Property(e => e.RejectionReason).HasMaxLength(500);
            //    entity.Property(e => e.CreatedBy).HasMaxLength(450);
            //    entity.Property(e => e.UpdatedBy).HasMaxLength(450);

            //    entity.HasIndex(e => e.ReconciliationNo).IsUnique().HasFilter("[ReconciliationNo] IS NOT NULL");
            //    entity.HasIndex(e => e.ReconciliationDate);

            //    entity.HasOne(sr => sr.Store)
            //        .WithMany()
            //        .HasForeignKey(e => e.StoreId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(sr => sr.PhysicalInventory)
            //        .WithMany()
            //        .HasForeignKey(e => e.PhysicalInventoryId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(sr => sr.StockAdjustment)
            //        .WithMany()
            //        .HasForeignKey(e => e.StockAdjustmentId)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});

            //// StockReconciliationItem configuration
            //modelBuilder.Entity<StockReconciliationItem>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.SystemQuantity).HasColumnType("decimal(18,3)");
            //    entity.Property(e => e.ActualQuantity).HasColumnType("decimal(18,3)");
            //    entity.Property(e => e.Variance).HasColumnType("decimal(18,3)");
            //    entity.Property(e => e.PhysicalQuantity).HasColumnType("decimal(18,3)");
            //    entity.Property(e => e.AdjustedQuantity).HasMaxLength(50);
            //    entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
            //    entity.Property(e => e.VarianceValue).HasColumnType("decimal(18,2)");
            //    entity.Property(e => e.VarianceType).HasMaxLength(50);
            //    entity.Property(e => e.Reason).HasMaxLength(500);
            //    entity.Property(e => e.Action).HasMaxLength(100);
            //    entity.Property(e => e.Notes).HasMaxLength(500);

            //    entity.HasOne(sri => sri.StockReconciliation)
            //        .WithMany(sr => sr.Items)
            //        .HasForeignKey(sri => sri.StockReconciliationId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    entity.HasOne(sri => sri.Item)
            //        .WithMany()
            //        .HasForeignKey(sri => sri.ItemId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(sri => sri.StockAdjustmentItem)
            //        .WithMany()
            //        .HasForeignKey(sri => sri.StockAdjustmentItemId)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});

            modelBuilder.Entity<CycleCountSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ScheduleCode).HasMaxLength(50);
                entity.Property(e => e.ScheduleName).HasMaxLength(100);
                entity.Property(e => e.Frequency).HasMaxLength(50);
                entity.Property(e => e.CountMethod).HasMaxLength(50);
                entity.Property(e => e.ABCClass).HasMaxLength(20);
                entity.Property(e => e.MinimumValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AssignedTo).HasMaxLength(450);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => e.NextScheduledDate);

                entity.HasOne(ccs => ccs.Store)
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ccs => ccs.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // IGNORE User navigation property - no foreign key
                entity.Ignore(e => e.AssignedToUser);
            });

            modelBuilder.Entity<InventoryCycleCount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CountNumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.CountType).HasMaxLength(50);
                entity.Property(e => e.CreatedById).HasMaxLength(450);
                entity.Property(e => e.ApprovedById).HasMaxLength(450);
                entity.Property(e => e.CountedBy).HasMaxLength(450);
                entity.Property(e => e.VerifiedBy).HasMaxLength(450);
                entity.Property(e => e.TotalVarianceValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => e.CountNumber).IsUnique().HasFilter("[CountNumber] IS NOT NULL");
                entity.HasIndex(e => e.CountDate);

                entity.HasOne(icc => icc.Store)
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                // IGNORE all User navigation properties - no foreign keys
                entity.Ignore(e => e.CreatedByUser);
                entity.Ignore(e => e.ApprovedByUser);
                entity.Ignore(e => e.CountedByUser);
                entity.Ignore(e => e.VerifiedByUser);
            });

            // InventoryCycleCountItem configuration
            modelBuilder.Entity<InventoryCycleCountItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SystemQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.CountedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Variance).HasColumnType("decimal(18,3)");

                entity.HasOne(icci => icci.CycleCount)
                    .WithMany(icc => icc.Items)
                    .HasForeignKey(icci => icci.CycleCountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(icci => icci.Item)
                    .WithMany()
                    .HasForeignKey(icci => icci.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // InventoryValuation configuration - FIXED user ID lengths
            modelBuilder.Entity<InventoryValuation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ValuationDate);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CostingMethod).HasMaxLength(50);
                entity.Property(e => e.ValuationType).HasMaxLength(50);
                entity.Property(e => e.CalculatedBy).HasMaxLength(450);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.HasIndex(e => e.ValuationDate);
                entity.HasIndex(e => new { e.ItemId, e.StoreId, e.ValuationDate });

                entity.HasOne<Item>()
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Store>()
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PhysicalInventoryDetail configuration - FIXED user ID lengths
            modelBuilder.Entity<PhysicalInventoryDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SystemQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.PhysicalQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Variance).HasColumnType("decimal(18,3)");
                entity.Property(e => e.VarianceValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.CountedBy).HasMaxLength(450);
                entity.Property(e => e.VerifiedBy).HasMaxLength(450);
                entity.Property(e => e.LocationCode).HasMaxLength(100);
                entity.Property(e => e.BatchNumbers).HasMaxLength(500);
                entity.Property(e => e.SerialNumbers).HasMaxLength(500);
                entity.Property(e => e.Remarks).HasMaxLength(500);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BatchNo).HasMaxLength(50);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.VariancePercentage).HasColumnType("decimal(5,2)");
                entity.Property(e => e.RecountRequestedBy).HasMaxLength(450);
                entity.Property(e => e.FirstCountQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.FirstCountBy).HasMaxLength(450);
                entity.Property(e => e.CountLocation).HasMaxLength(200);
                entity.Property(e => e.CountRemarks).HasMaxLength(500);
                entity.Property(e => e.VarianceType).HasConversion<int>();
                entity.Property(e => e.RecountQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.RecountBy).HasMaxLength(450);
                entity.Property(e => e.BlindCountQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.BlindCountBy).HasMaxLength(450);

                entity.HasOne(pid => pid.PhysicalInventory)
                    .WithMany(pi => pi.Details)
                    .HasForeignKey(e => e.PhysicalInventoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pid => pid.Item)
                    .WithMany()
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pid => pid.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PhysicalInventoryItem configuration - FIXED user ID lengths
            modelBuilder.Entity<PhysicalInventoryItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SystemQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.PhysicalQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Variance).HasColumnType("decimal(18,3)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SystemValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PhysicalValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.VarianceValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.BatchNumber).HasMaxLength(50);
                entity.Property(e => e.CountedBy).HasMaxLength(450);
                entity.Property(e => e.RecountQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.AdjustmentStatus).HasMaxLength(50);

                entity.HasOne(pii => pii.PhysicalInventory)
                    .WithMany(pi => pi.Items)
                    .HasForeignKey(pii => pii.PhysicalInventoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pii => pii.Item)
                    .WithMany()
                    .HasForeignKey(pii => pii.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureLossManagementEntities(ModelBuilder modelBuilder)
        {
            // WriteOff configuration
            modelBuilder.Entity<WriteOff>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.WriteOffNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.WriteOffNo).IsUnique();

                entity.HasOne<Store>()
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // WriteOffItem configuration
            modelBuilder.Entity<WriteOffItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Value).HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");

                // FIX: Use WriteOffId instead of WriteOffRequestId
                entity.HasOne(wi => wi.WriteOff)
                    .WithMany(w => w.WriteOffItems)
                    .HasForeignKey(wi => wi.WriteOffId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(wi => wi.Item)
                    .WithMany()
                    .HasForeignKey(wi => wi.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(wi => wi.Store)
                    .WithMany()
                    .HasForeignKey(wi => wi.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(wi => wi.WriteOffRequest)
                    .WithMany()
                    .HasForeignKey(wi => wi.WriteOffRequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // WriteOffRequest configuration
            modelBuilder.Entity<WriteOffRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.RequestNo).IsUnique();

                entity.HasOne(wr => wr.DamageReport)
                    .WithMany()
                    .HasForeignKey(wr => wr.DamageReportId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(wr => wr.Store)
                    .WithMany()
                    .HasForeignKey(wr => wr.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(wr => wr.RequestedByUser)
                    .WithMany()
                    .HasForeignKey(wr => wr.RequestedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Damage configuration
            modelBuilder.Entity<Damage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DamageNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EstimatedLoss).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.DamageNo).IsUnique();

                // FIX: Add explicit navigation properties
                entity.HasOne(d => d.Item)
                    .WithMany()
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Store)
                    .WithMany()
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DamageReport configuration
            modelBuilder.Entity<DamageReport>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReportNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EstimatedLoss).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.ReportNo).IsUnique();

                entity.HasOne(dr => dr.Store)
                    .WithMany()
                    .HasForeignKey(dr => dr.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dr => dr.Item)
                    .WithMany()
                    .HasForeignKey(dr => dr.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DamageReportItem configuration
            modelBuilder.Entity<DamageReportItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DamagedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.EstimatedValue).HasColumnType("decimal(18,2)");

                entity.HasOne(dri => dri.DamageReport)
                    .WithMany(dr => dr.Items)
                    .HasForeignKey(dri => dri.DamageReportId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(dri => dri.Item)
                    .WithMany()
                    .HasForeignKey(dri => dri.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DamageRecord configuration
            modelBuilder.Entity<DamageRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DamagedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");

                entity.HasOne(dr => dr.Item)
                    .WithMany()
                    .HasForeignKey(dr => dr.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dr => dr.Store)
                    .WithMany()
                    .HasForeignKey(dr => dr.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dr => dr.Return)
                    .WithMany()
                    .HasForeignKey(dr => dr.ReturnId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DisposalRecord configuration
            modelBuilder.Entity<DisposalRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DisposalNo).HasMaxLength(50);

                entity.HasOne(dr => dr.WriteOff)
                    .WithMany()
                    .HasForeignKey(dr => dr.WriteOffId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dr => dr.Item)
                    .WithMany()
                    .HasForeignKey(dr => dr.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(dr => dr.AuthorizedByUser)
                    .WithMany()
                    .HasForeignKey(dr => dr.AuthorizedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ConditionCheck configuration
            modelBuilder.Entity<ConditionCheck>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CheckedBy).HasMaxLength(450);
                entity.Property(e => e.OverallCondition).HasMaxLength(100);
                entity.Property(e => e.Condition).HasMaxLength(100);

                entity.HasOne(cc => cc.Return)
                    .WithMany()
                    .HasForeignKey(cc => cc.ReturnId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cc => cc.Item)
                    .WithMany()
                    .HasForeignKey(cc => cc.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ConditionCheckItem configuration
            modelBuilder.Entity<ConditionCheckItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CheckedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.GoodQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DamagedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ExpiredQuantity).HasColumnType("decimal(18,2)");

                entity.HasOne(cci => cci.ConditionCheck)
                    .WithMany(cc => cc.Items)
                    .HasForeignKey(cci => cci.ConditionCheckId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cci => cci.Item)
                    .WithMany()
                    .HasForeignKey(cci => cci.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigurePhysicalInventoryEntities(ModelBuilder modelBuilder)
        {
            // PhysicalInventory configuration
            modelBuilder.Entity<PhysicalInventory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CountNo).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.CountNo).IsUnique();
            });
        }

        private void ConfigureTrackingEntities(ModelBuilder modelBuilder)
        {
            // BatchTracking configuration is now in ConfigureBatchTrackingEntities

            // Barcode configuration
            modelBuilder.Entity<Barcode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BarcodeNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BarcodeType).HasMaxLength(50);
                entity.Property(e => e.BarcodeData).HasMaxLength(int.MaxValue);
                entity.Property(e => e.ReferenceType).HasMaxLength(50);
                entity.Property(e => e.BatchNumber).HasMaxLength(50);
                entity.Property(e => e.SerialNumber).HasMaxLength(100);
                entity.HasIndex(e => e.BarcodeNumber).IsUnique();
            });

            // ExpiryTracking configuration
            modelBuilder.Entity<ExpiryTracking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.BatchNumber).HasMaxLength(50);

                entity.HasOne(et => et.Item)
                    .WithMany()
                    .HasForeignKey(et => et.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(et => et.Store)
                    .WithMany()
                    .HasForeignKey(et => et.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ExpiryDate);
                entity.HasIndex(e => new { e.ItemId, e.StoreId });
            });

            // Warranty configuration
            modelBuilder.Entity<Warranty>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.WarrantyNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CoveredValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.WarrantyType).HasMaxLength(50);
                entity.Property(e => e.Terms).HasMaxLength(2000);
                entity.Property(e => e.ContactInfo).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.SerialNumber).HasMaxLength(100);

                entity.HasOne(w => w.Item)
                    .WithMany()
                    .HasForeignKey(w => w.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.Vendor)
                    .WithMany()
                    .HasForeignKey(w => w.VendorId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.WarrantyNumber).IsUnique();
                entity.HasIndex(e => e.EndDate);
            });

            // TemperatureLog configuration
            modelBuilder.Entity<TemperatureLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Temperature).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Humidity).HasColumnType("decimal(5,2)");
                entity.Property(e => e.RecordedBy).HasMaxLength(450);
                entity.Property(e => e.Equipment).HasMaxLength(100);
                entity.Property(e => e.Unit).HasMaxLength(20);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(tl => tl.Store)
                    .WithMany()
                    .HasForeignKey(tl => tl.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.LogTime);
            });

            // ShipmentTracking configuration
            modelBuilder.Entity<ShipmentTracking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TrackingCode).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ReferenceType).HasMaxLength(50);
                entity.Property(e => e.Carrier).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.LastLocation).HasMaxLength(200);
                entity.Property(e => e.TrackingUrl).HasMaxLength(500);
                entity.Property(e => e.QRCode).HasMaxLength(500);

                entity.HasIndex(e => e.TrackingCode).IsUnique();
            });

            // TrackingHistory configuration
            modelBuilder.Entity<TrackingHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.Carrier).HasMaxLength(100);
                entity.Property(e => e.TrackingUrl).HasMaxLength(500);

                entity.HasOne(th => th.ShipmentTracking)
                    .WithMany()
                    .HasForeignKey(th => th.ShipmentTrackingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ShipmentTrackingId);
                entity.HasIndex(e => e.LastUpdated);
            });
        }

        private void ConfigureQualityEntities(ModelBuilder modelBuilder)
        {
            // QualityCheck configuration
            modelBuilder.Entity<QualityCheck>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CheckNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CheckedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PassedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FailedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CheckType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Comments).HasMaxLength(1000);
                entity.Property(e => e.FailureReasons).HasMaxLength(500);
                entity.Property(e => e.CorrectiveActions).HasMaxLength(500);

                entity.HasIndex(e => e.CheckNumber).IsUnique();
                entity.HasIndex(e => e.CheckDate);
            });

            // QualityCheckItem configuration
            modelBuilder.Entity<QualityCheckItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CheckedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PassedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FailedQuantity).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CheckParameters).HasMaxLength(500);
                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.HasOne(qci => qci.QualityCheck)
                    .WithMany(qc => qc.Items)
                    .HasForeignKey(qci => qci.QualityCheckId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(qci => qci.Item)
                    .WithMany()
                    .HasForeignKey(qci => qci.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SupplierEvaluation configuration
            modelBuilder.Entity<SupplierEvaluation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QualityRating).HasColumnType("decimal(3,2)");
                entity.Property(e => e.DeliveryRating).HasColumnType("decimal(3,2)");
                entity.Property(e => e.PriceRating).HasColumnType("decimal(3,2)");
                entity.Property(e => e.ServiceRating).HasColumnType("decimal(3,2)");
                entity.Property(e => e.OverallRating).HasColumnType("decimal(3,2)");
                entity.Property(e => e.Comments).HasMaxLength(1000);
                entity.Property(e => e.Recommendations).HasMaxLength(1000);

                entity.HasOne(se => se.Vendor)
                    .WithMany()
                    .HasForeignKey(se => se.VendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.EvaluationDate);
            });
        }

        private void ConfigureSystemEntities(ModelBuilder modelBuilder)
        {
            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.Type).HasMaxLength(50);
            });

            // SystemConfiguration configuration
            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ModuleName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ConfigurationKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ConfigurationValue).IsRequired();
            });
        }

        private void ConfigureAuditEntities(ModelBuilder modelBuilder)
        {
            // ActivityLog configuration
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Details).HasMaxLength(1000);
            });
        }

        private void ConfigureBatchTrackingEntities(ModelBuilder modelBuilder)
        {
            // BatchTracking configuration
            modelBuilder.Entity<BatchTracking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BatchNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.RemainingQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReceivedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.ReservedQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.CurrentQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Temperature).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Humidity).HasColumnType("decimal(5,2)");

                entity.HasIndex(e => e.BatchNumber);
                entity.HasIndex(e => new { e.ItemId, e.StoreId });
            });

            // BatchMovement configuration - FIXED: Explicitly configure both relationships
            modelBuilder.Entity<BatchMovement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.BalanceAfter).HasColumnType("decimal(18,3)");
                entity.Property(e => e.BalanceBefore).HasColumnType("decimal(18,3)");
                entity.Property(e => e.NewBalance).HasColumnType("decimal(18,3)");
                entity.Property(e => e.OldBalance).HasColumnType("decimal(18,3)");

                // Configure the first relationship with BatchTracking via BatchId
                entity.HasOne(bm => bm.Batch)
                    .WithMany(bt => bt.BatchMovements)
                    .HasForeignKey(bm => bm.BatchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_BatchMovement_BatchTracking_BatchId");

                // Configure the second relationship with BatchTracking via BatchTrackingId
                entity.HasOne(bm => bm.BatchTracking)
                    .WithMany()
                    .HasForeignKey(bm => bm.BatchTrackingId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_BatchMovement_BatchTracking_BatchTrackingId");

                entity.HasIndex(e => e.BatchId);
                entity.HasIndex(e => e.BatchTrackingId);
                entity.HasIndex(e => e.MovementDate);
            });

            // BatchSignature configuration
            modelBuilder.Entity<BatchSignature>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BatchNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SignatureData).HasMaxLength(int.MaxValue);
                entity.Property(e => e.SignatureHash).HasMaxLength(500);

                entity.HasOne(bs => bs.Batch)
                    .WithMany()
                    .HasForeignKey(bs => bs.BatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bs => bs.SignedByUser)
                    .WithMany()
                    .HasForeignKey(bs => bs.SignedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BatchSignatureItem configuration
            modelBuilder.Entity<BatchSignatureItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");

                entity.HasOne(bsi => bsi.BatchSignature)
                    .WithMany(bs => bs.Items)
                    .HasForeignKey(bsi => bsi.BatchSignatureId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ✅ DigitalSignature configuration
            modelBuilder.Entity<DigitalSignature>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SignatureData).HasMaxLength(int.MaxValue); // Support large Base64 signatures
                entity.Property(e => e.SignatureHash).HasMaxLength(500);
                entity.Property(e => e.EntityType).HasMaxLength(50);
                entity.Property(e => e.SignatureType).HasMaxLength(50);

                entity.HasOne(ds => ds.SignedByUser)
                    .WithMany()
                    .HasForeignKey(ds => ds.SignedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ds => ds.VerifiedByUser)
                    .WithMany()
                    .HasForeignKey(ds => ds.VerifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.EntityType, e.EntityId });
                entity.HasIndex(e => e.SignedAt);
            });

            // ExpiredRecord configuration
            modelBuilder.Entity<ExpiredRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExpiredQuantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");

                entity.HasOne(er => er.Batch)
                    .WithMany()
                    .HasForeignKey(er => er.BatchId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(er => er.Item)
                    .WithMany()
                    .HasForeignKey(er => er.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(er => er.Store)
                    .WithMany()
                    .HasForeignKey(er => er.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureDecimalPrecisions(ModelBuilder modelBuilder)
        {
            // Approval entities - CRITICAL FOR OVERFLOW FIX
            modelBuilder.Entity<ApprovalRequest>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.ApprovalValue).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ApprovalThreshold>(entity =>
            {
                entity.Property(e => e.MinAmount).HasPrecision(18, 2);
                entity.Property(e => e.MaxAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ApprovalWorkflow>(entity =>
            {
                entity.Property(e => e.MinAmount).HasPrecision(18, 2);
                entity.Property(e => e.MaxAmount).HasPrecision(18, 2);
                entity.Property(e => e.ThresholdValue).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ApprovalWorkflowLevel>(entity =>
            {
                entity.Property(e => e.ThresholdAmount).HasPrecision(18, 2);
            });

            // Audit entities
            modelBuilder.Entity<AuditReport>(entity =>
            {
                entity.Property(e => e.TotalPhysicalValue).HasPrecision(18, 2);
                entity.Property(e => e.TotalSystemValue).HasPrecision(18, 2);
                entity.Property(e => e.TotalVarianceValue).HasPrecision(18, 2);
            });

            // Batch tracking
            modelBuilder.Entity<BatchTracking>(entity =>
            {
                entity.Property(e => e.CostPrice).HasPrecision(18, 2);
                entity.Property(e => e.SellingPrice).HasPrecision(18, 2);
                entity.Property(e => e.InitialQuantity).HasPrecision(18, 3);
            });

            // Goods receive
            modelBuilder.Entity<GoodsReceiveItem>(entity =>
            {
                entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.AcceptedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.RejectedQuantity).HasPrecision(18, 3);
            });

            // Inventory cycle count
            modelBuilder.Entity<InventoryCycleCountItem>(entity =>
            {
                entity.Property(e => e.VarianceQuantity).HasPrecision(18, 3);
                entity.Property(e => e.VarianceValue).HasPrecision(18, 2);
            });

            // Inventory valuation - Only if entity exists
            if (modelBuilder.Model.FindEntityType(typeof(InventoryValuation)) != null)
            {
                modelBuilder.Entity<InventoryValuation>(entity =>
                {
                    entity.Property(e => e.Quantity).HasPrecision(18, 3);
                    entity.Property(e => e.UnitCost).HasPrecision(18, 2);
                    entity.Property(e => e.TotalValue).HasPrecision(18, 2);
                });
            }

            // Issue
            modelBuilder.Entity<IssueItem>(entity =>
            {
                entity.Property(e => e.RequestedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.IssuedQuantity).HasPrecision(18, 3);
            });

            // Item
            modelBuilder.Entity<Item>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.MinimumStock).HasPrecision(18, 3);
                entity.Property(e => e.Weight).HasPrecision(18, 3);
            });

            // Physical Inventory
            modelBuilder.Entity<PhysicalInventory>(entity =>
            {
                entity.Property(e => e.TotalPhysicalQuantity).HasPrecision(18, 3);
                entity.Property(e => e.TotalPhysicalValue).HasPrecision(18, 2);
                entity.Property(e => e.TotalSystemQuantity).HasPrecision(18, 3);
                entity.Property(e => e.TotalSystemValue).HasPrecision(18, 2);
                entity.Property(e => e.TotalVariance).HasPrecision(18, 3);
                entity.Property(e => e.TotalVarianceValue).HasPrecision(18, 2);
                entity.Property(e => e.VarianceValue).HasPrecision(18, 2);
            });

            // Purchase
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            });

            // Requisition
            modelBuilder.Entity<RequisitionItem>(entity =>
            {
                entity.Property(e => e.RequestedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.ApprovedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.IssuedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.EstimatedUnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.EstimatedTotalPrice).HasPrecision(18, 2);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            });

            // Return
            modelBuilder.Entity<Return>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
            });

            // Return Item
            modelBuilder.Entity<ReturnItem>(entity =>
            {
                entity.Property(e => e.ReturnQuantity).HasPrecision(18, 3);
                entity.Property(e => e.ApprovedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.AcceptedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.RejectedQuantity).HasPrecision(18, 3);
            });

            // Stock movement
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                entity.Property(e => e.OldBalance).HasPrecision(18, 3);
                entity.Property(e => e.NewBalance).HasPrecision(18, 3);
                entity.Property(e => e.TotalValue).HasPrecision(18, 2);
            });

            // Stock adjustment
            modelBuilder.Entity<StockAdjustment>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                entity.Property(e => e.OldQuantity).HasPrecision(18, 3);
                entity.Property(e => e.NewQuantity).HasPrecision(18, 3);
                entity.Property(e => e.AdjustmentQuantity).HasPrecision(18, 3);
                entity.Property(e => e.TotalValue).HasPrecision(18, 2);
            });

            // Store
            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(e => e.Temperature).HasPrecision(5, 2);
                entity.Property(e => e.MinTemperature).HasPrecision(5, 2);
                entity.Property(e => e.MaxTemperature).HasPrecision(5, 2);
                entity.Property(e => e.Humidity).HasPrecision(5, 2);
            });

            // Store stock
            modelBuilder.Entity<StoreStock>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                entity.Property(e => e.ReorderLevel).HasPrecision(18, 3);
                entity.Property(e => e.LastPurchasePrice).HasPrecision(18, 2);
                entity.Property(e => e.AveragePrice).HasPrecision(18, 2);
            });

            // Location entities
            modelBuilder.Entity<Union>(entity =>
            {
                entity.Property(e => e.Area).HasPrecision(10, 2);
                entity.Property(e => e.Latitude).HasPrecision(10, 6);
                entity.Property(e => e.Longitude).HasPrecision(10, 6);
            });

            modelBuilder.Entity<Upazila>(entity =>
            {
                entity.Property(e => e.Area).HasPrecision(10, 2);
            });

            modelBuilder.Entity<Zila>(entity =>
            {
                entity.Property(e => e.Area).HasPrecision(10, 2);
            });

            // Usage statistics
            modelBuilder.Entity<UsageStatistics>(entity =>
            {
                entity.Property(e => e.MetricValue).HasPrecision(18, 4);
            });

            // WriteOff
            modelBuilder.Entity<WriteOff>(entity =>
            {
                entity.Property(e => e.TotalValue).HasPrecision(18, 2);
            });

            // WriteOffItem - CORRECTED based on actual entity properties
            modelBuilder.Entity<WriteOffItem>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                entity.Property(e => e.Value).HasPrecision(18, 2);
                entity.Property(e => e.UnitCost).HasPrecision(18, 2);
                entity.Property(e => e.TotalCost).HasPrecision(18, 2);
            });

            // Damage - CORRECTED: EstimatedLoss is int, Quantity is decimal
            modelBuilder.Entity<Damage>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                // EstimatedLoss is int, no need for precision configuration
            });

            // DamageReport & DamageReportItem
            modelBuilder.Entity<DamageReport>(entity =>
            {
                entity.Property(e => e.TotalValue).HasPrecision(18, 2);
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
                entity.Property(e => e.EstimatedLoss).HasPrecision(18, 2);
            });

            modelBuilder.Entity<DamageReportItem>(entity =>
            {
                entity.Property(e => e.DamagedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.EstimatedValue).HasPrecision(18, 2);
            });

            // ConditionCheckItem
            modelBuilder.Entity<ConditionCheckItem>(entity =>
            {
                entity.Property(e => e.CheckedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.GoodQuantity).HasPrecision(18, 3);
                entity.Property(e => e.DamagedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.ExpiredQuantity).HasPrecision(18, 3);
            });

            // Expiry tracking
            modelBuilder.Entity<ExpiryTracking>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
            });

            // ReceiveItem - CORRECTED based on actual entity properties
            modelBuilder.Entity<ReceiveItem>(entity =>
            {
                entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 3);
                // Other properties don't exist in ReceiveItem
            });

            // TransferItem (from search results)
            modelBuilder.Entity<TransferItem>(entity =>
            {
                entity.Property(e => e.RequestedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.ApprovedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.ShippedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.ReceivedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalValue).HasPrecision(18, 2);
            });

            // Temperature log - Only if entity exists
            if (modelBuilder.Model.FindEntityType(typeof(TemperatureLog)) != null)
            {
                modelBuilder.Entity<TemperatureLog>(entity =>
                {
                    entity.Property(e => e.Temperature).HasPrecision(5, 2);
                    entity.Property(e => e.Humidity).HasPrecision(5, 2);
                });
            }

            // WriteOffRequest
            modelBuilder.Entity<WriteOffRequest>(entity =>
            {
                entity.Property(e => e.TotalValue).HasPrecision(18, 2);
            });

            // DisposalRecord
            modelBuilder.Entity<DisposalRecord>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(18, 3);
            });

            // QualityCheckItem
            modelBuilder.Entity<QualityCheckItem>(entity =>
            {
                entity.Property(e => e.CheckedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.PassedQuantity).HasPrecision(18, 3);
                entity.Property(e => e.FailedQuantity).HasPrecision(18, 3);
            });
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Item indexes
            modelBuilder.Entity<Item>()
                .HasIndex(i => i.Name);
            modelBuilder.Entity<Item>()
                .HasIndex(i => i.IsActive);

            // Store indexes
            modelBuilder.Entity<Store>()
                .HasIndex(s => s.Name);
            modelBuilder.Entity<Store>()
                .HasIndex(s => s.IsActive);

            // Transaction indexes
            modelBuilder.Entity<Issue>()
                .HasIndex(i => i.IssueDate);
            modelBuilder.Entity<Transfer>()
                .HasIndex(t => t.TransferDate);
            modelBuilder.Entity<WriteOff>()
                .HasIndex(w => w.WriteOffDate);

            // Performance indexes
            modelBuilder.Entity<StockMovement>()
                .HasIndex(sm => new { sm.ItemId, sm.MovementDate });
        }

        // CRITICAL: This method fixes the collection mapping issues in EF Core 9
        private void ConfigureProblematicStringProperties(ModelBuilder modelBuilder)
        {
            Console.WriteLine("Explicitly configuring problematic string properties...");

            // WriteOff entity - properties that might be mistaken for collections
            modelBuilder.Entity<WriteOff>(entity =>
            {
                entity.Property(e => e.AttachmentPaths)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false)
                    .HasMaxLength(int.MaxValue);

                Console.WriteLine("Explicitly configured WriteOff.AttachmentPaths as string");

                entity.Property(e => e.NotifiedUsers)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false)
                    .HasMaxLength(int.MaxValue);

                Console.WriteLine("Explicitly configured WriteOff.NotifiedUsers as string");

                entity.Property(e => e.ApprovalComments)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false)
                    .HasMaxLength(int.MaxValue);

                Console.WriteLine("Explicitly configured WriteOff.ApprovalComments as string");
            });

            // ConditionCheckItem entity
            modelBuilder.Entity<ConditionCheckItem>(entity =>
            {
                entity.Property(e => e.Photos)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false)
                    .HasMaxLength(int.MaxValue);

                Console.WriteLine("Explicitly configured ConditionCheckItem.Photos as string");
            });

            // DamageReportItem entity
            modelBuilder.Entity<DamageReportItem>(entity =>
            {
                entity.Property(e => e.PhotoUrls)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false)
                    .HasMaxLength(int.MaxValue);

                Console.WriteLine("Explicitly configured DamageReportItem.PhotoUrls as string");
            });

            // DisposalRecord entity
            modelBuilder.Entity<DisposalRecord>(entity =>
            {
                entity.Property(e => e.PhotoUrls)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false)
                    .HasMaxLength(int.MaxValue);

                Console.WriteLine("Explicitly configured DisposalRecord.PhotoUrls as string");
            });

            // TransferShipment entity
            modelBuilder.Entity<TransferShipment>(entity =>
            {
                entity.Property(e => e.PackingListNo)
                    .HasColumnType("nvarchar(200)")
                    .IsRequired(false)
                    .HasMaxLength(200);

                Console.WriteLine("Explicitly configured TransferShipment.PackingListNo as string");
            });

            // TransferShipmentItem entity
            modelBuilder.Entity<TransferShipmentItem>(entity =>
            {
                entity.Property(e => e.PackageDetails)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false)
                    .HasMaxLength(int.MaxValue);

                Console.WriteLine("Explicitly configured TransferShipmentItem.PackageDetails as string");
            });

            // TransferItem entity
            modelBuilder.Entity<TransferItem>(entity =>
            {
                entity.Property(e => e.PackageDetails)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false)
                    .HasMaxLength(int.MaxValue);

                Console.WriteLine("Explicitly configured TransferItem.PackageDetails as string");
            });

            // SystemConfiguration entity
            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.Property(e => e.ValidValues)
                    .HasColumnType("nvarchar(1000)")
                    .IsRequired(false)
                    .HasMaxLength(1000);

                Console.WriteLine("Explicitly configured SystemConfiguration.ValidValues as string");
            });

            // Range entity - CoverageArea might be mistaken for a collection
            modelBuilder.Entity<Range>(entity =>
            {
                entity.Property(e => e.CoverageArea)
                    .HasColumnType("nvarchar(500)")
                    .IsRequired(false)
                    .HasMaxLength(500);

                Console.WriteLine("Explicitly configured Range.CoverageArea as string");
            });

            Console.WriteLine("Completed configuration of problematic string properties");
        }

        #endregion
    }
}