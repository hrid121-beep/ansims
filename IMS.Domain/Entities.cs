using IMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string Department { get; set; }
        public string Designation { get; set; }
        public string BadgeNumber { get; set; }

        public int? BattalionId { get; set; }
        public virtual Battalion Battalion { get; set; }

        public int? RangeId { get; set; }
        public virtual Range Range { get; set; }

        public int? ZilaId { get; set; }
        public virtual Zila Zila { get; set; }

        public int? UpazilaId { get; set; }
        public virtual Upazila Upazila { get; set; }

        public int? UnionId { get; set; }
        public virtual Union Union { get; set; }

        public virtual ICollection<LoginLog> LoginLogs { get; set; } = new List<LoginLog>();
        public virtual ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
        public virtual ICollection<Purchase> CreatedPurchases { get; set; } = new List<Purchase>();
    }
    public class LoginLog : BaseEntity
    {
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }

        public virtual User User { get; set; }
    }
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string NameBn { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }

        public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        public virtual ICollection<StoreTypeCategory> StoreTypeCategories { get; set; } = new List<StoreTypeCategory>();
    }
    public class SubCategory : BaseEntity
    {
        public string Name { get; set; }
        public string NameBn { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
    public class Brand : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }

        public virtual ICollection<ItemModel> ItemModels { get; set; } = new List<ItemModel>();
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
    public class ItemModel : BaseEntity
    {
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public int BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
    public class Item : BaseEntity
    {
        public string Name { get; set; }
        public string NameBn { get; set; }
        public string ItemCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public ItemType Type { get; set; }
        public ItemStatus Status { get; set; } = ItemStatus.Available;

        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public int? ItemModelId { get; set; }
        public int? BrandId { get; set; }

        public decimal? MinimumStock { get; set; }
        public decimal? MaximumStock { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitCost { get; set; }

        public string Manufacturer { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool HasExpiry { get; set; }
        public int? ShelfLife { get; set; }

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

        public string ItemImage { get; set; }
        public string ImagePath { get; set; }
        public string Barcode { get; set; }
        public string BarcodePath { get; set; }
        public string QRCodeData { get; set; }

        // Add these properties to existing Item class
        public string? ItemControlType { get; set; } // "Controlled" or "Uncontrolled"

        // Separate lifespan for Ansar and VDP personnel
        public int? AnsarLifeSpanMonths { get; set; } // Life span in months for Ansar
        public int? VDPLifeSpanMonths { get; set; } // Life span in months for VDP

        // Separate alert days for Ansar and VDP
        public int? AnsarAlertBeforeDays { get; set; } = 30; // Alert before expiry for Ansar (default 30 days)
        public int? VDPAlertBeforeDays { get; set; } = 30; // Alert before expiry for VDP (default 30 days)

        // Legacy field - kept for backward compatibility
        [Obsolete("Use AnsarLifeSpanMonths or VDPLifeSpanMonths instead")]
        public int? LifeSpanMonths { get; set; } // Deprecated - use Ansar/VDP specific fields

        [Obsolete("Use AnsarAlertBeforeDays or VDPAlertBeforeDays instead")]
        public int? AlertBeforeDays { get; set; } = 30; // Deprecated - use Ansar/VDP specific fields

        public bool IsAnsarAuthorized { get; set; } = true;
        public bool IsVDPAuthorized { get; set; } = true;
        public bool RequiresPersonalIssue { get; set; } = false;

        // ===== #2 - Entitlement Properties =====
        public decimal? AnsarEntitlementQuantity { get; set; }
        public decimal? VDPEntitlementQuantity { get; set; }
        public int? EntitlementPeriodMonths { get; set; } = 24; // Default 2 years

        // ===== #6 - Control Item Properties =====
        public bool RequiresHigherApproval { get; set; } = false;
        public string ControlItemCategory { get; set; } // "Weapons", "Electronics", "Vehicles"


        // Navigation property
        public virtual SubCategory SubCategory { get; set; }
        public virtual ItemModel ItemModel { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual ICollection<PersonnelItemIssue>? PersonnelIssues { get; set; }
        public virtual ICollection<StoreItem> StoreItems { get; set; } = new List<StoreItem>();
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        public virtual ICollection<IssueItem> IssueItems { get; set; } = new List<IssueItem>();
        public virtual ICollection<ReceiveItem> ReceiveItems { get; set; } = new List<ReceiveItem>();
        public virtual ICollection<TransferItem> TransferItems { get; set; } = new List<TransferItem>();
        public virtual ICollection<ItemSpecification> Specifications { get; set; } = new List<ItemSpecification>();
        public virtual ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
        public virtual ICollection<ExpiryTracking> ExpiryTrackings { get; set; } = new List<ExpiryTracking>();
        public virtual ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();
    }
    public class StoreType : BaseEntity
    {
        public string Name { get; set; }
        public string NameBn { get; set; } // ✅ ADD THIS
        public string Code { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string DefaultManagerRole { get; set; }

        public int DisplayOrder { get; set; }
        public int MaxCapacity { get; set; }

        public bool RequiresTemperatureControl { get; set; }
        public bool RequiresSecurityClearance { get; set; }
        public bool IsMainStore { get; set; }
        public bool AllowDirectIssue { get; set; }
        public bool AllowTransfer { get; set; }
        public bool RequiresMandatoryDocuments { get; set; } // ✅ ADD THIS

        public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
        public virtual ICollection<StoreTypeCategory> StoreTypeCategories { get; set; } = new List<StoreTypeCategory>();
        public virtual ICollection<StoreTypeCategory> AllowedCategories { get; set; } = new List<StoreTypeCategory>();
    }
    public class StoreTypeCategory : BaseEntity
    {
        public int StoreTypeId { get; set; }
        public int CategoryId { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsAllowed { get; set; }

        public virtual StoreType StoreType { get; set; }
        public virtual Category Category { get; set; }
    }
    public class Store : BaseEntity
    {
        public string Name { get; set; }
        public string NameBn { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }

        public string InCharge { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ManagerName { get; set; }
        public string ManagerId { get; set; }
        public string StoreKeeperName { get; set; }
        public string StoreKeeperId { get; set; }
        public string StoreKeeperContact { get; set; }
        public DateTime StoreKeeperAssignedDate { get; set; }

        public string OperatingHours { get; set; }
        public decimal? Capacity { get; set; }
        public decimal? TotalCapacity { get; set; }
        public decimal? UsedCapacity { get; set; }
        public decimal? AvailableCapacity { get; set; }

        public StoreLevel Level { get; set; }
        public int? StoreTypeId { get; set; }
        public int? LocationId { get; set; }

        public int? BattalionId { get; set; }
        public int? RangeId { get; set; }
        public int? ZilaId { get; set; }
        public int? UpazilaId { get; set; }
        public int? UnionId { get; set; }

        public bool RequiresTemperatureControl { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public decimal? MinTemperature { get; set; }
        public decimal? MaxTemperature { get; set; }

        public string SecurityLevel { get; set; }
        public string AccessRequirements { get; set; }

        public bool IsStockFrozen { get; set; }
        public DateTime StockFrozenAt { get; set; }
        public DateTime? StockUnfrozenAt { get; set; }
        public string StockFrozenReason { get; set; }

        public virtual StoreType StoreType { get; set; }
        public virtual Location LocationDetail { get; set; }
        public virtual Battalion Battalion { get; set; }
        public virtual Range Range { get; set; }
        public virtual Zila Zila { get; set; }
        public virtual Upazila Upazila { get; set; }
        public virtual Union Union { get; set; }

        public virtual ICollection<StoreItem> StoreItems { get; set; } = new List<StoreItem>();
        public virtual ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<IssueItem> IssueItems { get; set; } = new List<IssueItem>();
        public virtual ICollection<Receive> Receives { get; set; } = new List<Receive>();
        public virtual ICollection<ReceiveItem> ReceiveItems { get; set; } = new List<ReceiveItem>();
        public virtual ICollection<Transfer> TransfersFrom { get; set; } = new List<Transfer>();
        public virtual ICollection<Transfer> TransfersTo { get; set; } = new List<Transfer>();
        public virtual ICollection<TemperatureLog> TemperatureLogs { get; set; } = new List<TemperatureLog>();
        public virtual ICollection<StoreConfiguration> StoreConfigurations { get; set; } = new List<StoreConfiguration>();
    }
    public class StoreItem : BaseEntity
    {
        public int ItemId { get; set; }
        public int? StoreId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal MaximumStock { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal? ReservedStock { get; set; }
        public int ReservedQuantity { get; set; }

        public string Location { get; set; }
        public ItemStatus Status { get; set; }

        public DateTime LastUpdated { get; set; }
        public DateTime LastStockUpdate { get; set; }
        public DateTime LastCountDate { get; set; }
        public DateTime LastIssueDate { get; set; }
        public DateTime LastReceiveDate { get; set; }
        public DateTime LastTransferDate { get; set; }
        public DateTime LastAdjustmentDate { get; set; }
        public decimal LastCountQuantity { get; set; }

        public virtual Store Store { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
    public class Vendor : BaseEntity
    {
        public string Name { get; set; }
        public string VendorType { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string TIN { get; set; }
        public string BIN { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
    public class Purchase : BaseEntity
    {
        public string PurchaseOrderNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }  // Make nullable
        public DateTime? ReceivedDate { get; set; }  // Make nullable

        public int? VendorId { get; set; }
        public int? StoreId { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; } = 0;

        public string PurchaseType { get; set; } = "Vendor";
        public string Status { get; set; } = "Draft";
        public string Remarks { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }

        public bool IsMarketplacePurchase { get; set; }
        public string MarketplaceUrl { get; set; }
        public ProcurementType ProcurementType { get; set; } = ProcurementType.GovernmentRevenue;
        public string ProcurementSource { get; set; } // Donor name for private
        public string BudgetCode { get; set; } // For govt revenue tracking

        public bool IsApproved => Status == "Approved";

        public virtual Vendor Vendor { get; set; }
        public virtual Store Store { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

    }
    public class PurchaseItem : BaseEntity
    {
        public int PurchaseId { get; set; }
        public int ItemId { get; set; }
        public int? StoreId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual Purchase Purchase { get; set; }
        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }

        public decimal? ReceivedQuantity { get; set; }
        public decimal? AcceptedQuantity { get; set; }
        public decimal? RejectedQuantity { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ReceiveRemarks { get; set; }
        public string Remarks { get; set; }
    }
    public class Issue : BaseEntity
    {
        // Add signature relationships
        public int? IssuerSignatureId { get; set; }
        public int? ApproverSignatureId { get; set; }
        public int? ReceiverSignatureId { get; set; }

        // Navigation properties
        public virtual Signature IssuerSignature { get; set; }
        public virtual Signature ApproverSignature { get; set; }
        public virtual Signature ReceiverSignature { get; set; }

        // Keep these for backward compatibility or quick access
        public string SignerName { get; set; }
        public string SignerBadgeId { get; set; }
        public string SignerDesignation { get; set; }
        public DateTime? SignedDate { get; set; }

        public string IssueNo { get; set; }
        public string IssueNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string Status { get; set; } = "Draft";

        public string IssuedTo { get; set; }
        public string IssuedToType { get; set; }
        public string Purpose { get; set; }
        public string Remarks { get; set; }

        public string RequestedBy { get; set; }
        public DateTime? RequestedDate { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovalRemarks { get; set; }
        public string ApprovalReferenceNo { get; set; }
        public string ApprovalComments { get; set; }
        public string RejectionReason { get; set; }
        public string ApprovedByName { get; set; }
        public string ApprovedByBadgeNo { get; set; }

        public string SignaturePath { get; set; }
        public DateTime? SignatureDate { get; set; }

        public DateTime ReceivedDate { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceiverDesignation { get; set; }
        public string ReceiverContact { get; set; }

        public int? IssuedToBattalionId { get; set; }
        public int? IssuedToRangeId { get; set; }
        public int? IssuedToZilaId { get; set; }
        public int? IssuedToUpazilaId { get; set; }
        public int? IssuedToUnionId { get; set; }
        public string IssuedToIndividualName { get; set; }
        public string IssuedToIndividualBadgeNo { get; set; }
        public string IssuedToIndividualMobile { get; set; }

        public string ToEntityType { get; set; }
        public int ToEntityId { get; set; }
        public int? FromStoreId { get; set; }
        public string DeliveryLocation { get; set; }

        public string VoucherNo { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public DateTime? VoucherGeneratedDate { get; set; }
        public string VoucherQRCode { get; set; }

        public string QRCode { get; set; }
        public string IssuedBy { get; set; }
        public DateTime? IssuedDate { get; set; }

        public bool IsPartialIssue { get; set; }
        public int? ParentIssueId { get; set; }

        public virtual Battalion IssuedToBattalion { get; set; }
        public virtual Range IssuedToRange { get; set; }
        public virtual Zila IssuedToZila { get; set; }
        public virtual Upazila IssuedToUpazila { get; set; }
        public virtual Union IssuedToUnion { get; set; }
        public virtual Store FromStore { get; set; }
        public virtual Issue ParentIssue { get; set; }
        public virtual User CreatedByUser { get; set; }
        public string RejectedBy { get; set; }
        public DateTime RejectedDate { get; set; }
        public string VoucherDocumentPath { get; set; }
        public string MemoNo { get; set; }
        public DateTime? MemoDate { get; set; }
        public bool HasRequiredDocuments => !string.IsNullOrEmpty(VoucherDocumentPath);

        public bool IsDeleted { get; set; } = false;
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletionReason { get; set; }
        // Computed property:
        public int? AllotmentLetterId { get; set; }
        public string AllotmentLetterNo { get; set; }

        // Navigation
        public virtual AllotmentLetter AllotmentLetter { get; set; }
        public virtual ICollection<Issue> PartialIssues { get; set; } = new List<Issue>();
        public virtual ICollection<IssueItem> Items { get; set; } = new List<IssueItem>();
    }
    public class IssueItem : BaseEntity
    {
        public int IssueId { get; set; }
        public int ItemId { get; set; }
        public int? StoreId { get; set; }

        public decimal Quantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal ApprovedQuantity { get; set; }

        public string Unit { get; set; }
        public string BatchNumber { get; set; }
        public string Condition { get; set; }
        public string Remarks { get; set; }
        public string HandoverRemarks { get; set; }

        // Ledger and Page tracking for voucher
        public int? LedgerBookId { get; set; }
        public string LedgerNo { get; set; }
        public string PageNo { get; set; }

        // Item condition breakdown for voucher
        public decimal? UsableQuantity { get; set; }
        public decimal? PartiallyUsableQuantity { get; set; }
        public decimal? UnusableQuantity { get; set; }

        public virtual Issue Issue { get; set; }
        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual LedgerBook LedgerBook { get; set; }
    }
    public class Receive : BaseEntity
    {
        public string ReceiveNo { get; set; }
        public string ReceiveNumber { get; set; }
        public DateTime ReceiveDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Status { get; set; } = "Draft";

        public string ReceivedFrom { get; set; }
        public string ReceivedFromType { get; set; }
        public string ReceivedBy { get; set; }
        public string Source { get; set; }
        public string Remarks { get; set; }

        public int? ReceivedFromBattalionId { get; set; }
        public int? ReceivedFromRangeId { get; set; }
        public int? ReceivedFromZilaId { get; set; }
        public int? ReceivedFromUpazilaId { get; set; }
        public int? ReceivedFromUnionId { get; set; }
        public string ReceivedFromIndividualName { get; set; }
        public string ReceivedFromIndividualBadgeNo { get; set; }

        public int? StoreId { get; set; }
        public int? OriginalIssueId { get; set; }
        public string OriginalIssueNo { get; set; }
        public string OriginalVoucherNo { get; set; }

        public string ReceiveType { get; set; }
        public string ReceiverSignature { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverBadgeNo { get; set; }
        public string ReceiverDesignation { get; set; }
        public bool IsReceiverSignature { get; set; }
        public bool VerifierSignature { get; set; }

        public string OverallCondition { get; set; }
        public string AssessmentNotes { get; set; }
        public string AssessedBy { get; set; }
        public DateTime? AssessmentDate { get; set; }

        // Voucher fields for Receipt Voucher
        public string VoucherNo { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        public DateTime? VoucherGeneratedDate { get; set; }
        public string VoucherQRCode { get; set; }
        public string VoucherDocumentPath { get; set; }

        public virtual Battalion ReceivedFromBattalion { get; set; }
        public virtual Range ReceivedFromRange { get; set; }
        public virtual Zila ReceivedFromZila { get; set; }
        public virtual Upazila ReceivedFromUpazila { get; set; }
        public virtual Union ReceivedFromUnion { get; set; }
        public virtual Store Store { get; set; }
        public virtual Issue OriginalIssue { get; set; }

        public virtual ICollection<ReceiveItem> ReceiveItems { get; set; } = new List<ReceiveItem>();
    }
    public class ReceiveItem : BaseEntity
    {
        public int ReceiveId { get; set; }
        public int ItemId { get; set; }
        public int? StoreId { get; set; }

        public decimal? Quantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }

        public string BatchNumber { get; set; }
        public string Condition { get; set; }
        public string Remarks { get; set; }
        public string DamageNotes { get; set; }
        public string DamageDescription { get; set; }
        public string DamagePhotoPath { get; set; }

        public bool IsScanned { get; set; }

        // Ledger and Page tracking for voucher
        public int? LedgerBookId { get; set; }
        public string LedgerNo { get; set; }
        public string PageNo { get; set; }

        // Item condition breakdown for voucher
        public decimal? UsableQuantity { get; set; }
        public decimal? PartiallyUsableQuantity { get; set; }
        public decimal? UnusableQuantity { get; set; }

        public virtual Receive Receive { get; set; }
        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual LedgerBook LedgerBook { get; set; }
    }
    public class Transfer : BaseEntity
    {
        public string TransferNo { get; set; }
        public DateTime TransferDate { get; set; }
        public string Status { get; set; } = "Draft";

        public int FromStoreId { get; set; }
        public int ToStoreId { get; set; }
        public int? FromBattalionId { get; set; }
        public int? FromRangeId { get; set; }
        public int? FromZilaId { get; set; }
        public int? ToBattalionId { get; set; }
        public int? ToRangeId { get; set; }
        public int? ToZilaId { get; set; }

        public string TransferType { get; set; }
        public string Purpose { get; set; }
        public string Remarks { get; set; }

        public string RequestedBy { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public string TransferredBy { get; set; }
        public string ShippedBy { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string ShipmentNo { get; set; }
        public string ShippingQRCode { get; set; }

        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string ReceiverSignature { get; set; }
        public string ReceiptRemarks { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }
        public string TransportMode { get; set; }
        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string DriverContact { get; set; }

        public decimal? TotalValue { get; set; }
        public bool HasDiscrepancy { get; set; }

        public bool SenderSignature { get; set; }
        public bool IsReceiverSignature { get; set; }
        public bool ApproverSignature { get; set; }

        public virtual Store FromStore { get; set; }
        public virtual Store ToStore { get; set; }
        public virtual TransferShipment Shipment { get; set; }

        public virtual ICollection<TransferItem> Items { get; set; } = new List<TransferItem>();
    }
    public class TransferItem : BaseEntity
    {
        public int TransferId { get; set; }
        public int ItemId { get; set; }

        public decimal Quantity { get; set; }
        public decimal? RequestedQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public decimal? ShippedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }

        public decimal? UnitPrice { get; set; }
        public decimal? TotalValue { get; set; }
        public string ItemCondition { get; set; } = "Serviceable"; // Serviceable, Unserviceable, Damaged

        public string BatchNo { get; set; }
        public string Remarks { get; set; }
        public string ReceivedCondition { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime ShippedDate { get; set; }

        public int PackageCount { get; set; }
        public string PackageDetails { get; set; }

        public virtual Transfer Transfer { get; set; }
        public virtual Item Item { get; set; }
    }
    public class WriteOff : BaseEntity
    {
        public string WriteOffNo { get; set; }
        public DateTime WriteOffDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal TotalValue { get; set; }

        public int? StoreId { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovalComments { get; set; }

        public string RejectedBy { get; set; }
        public DateTime? RejectionDate { get; set; }
        public string RejectionReason { get; set; }

        public string RequiredApproverRole { get; set; }
        public int ApprovalThreshold { get; set; }

        public string AttachmentPaths { get; set; }

        public bool NotificationSent { get; set; }
        public DateTime? NotificationSentDate { get; set; }
        public string NotifiedUsers { get; set; }

        public virtual ICollection<WriteOffItem> WriteOffItems { get; set; } = new List<WriteOffItem>();
    }
    public class WriteOffItem : BaseEntity
    {
        public int WriteOffId { get; set; }
        public int ItemId { get; set; }
        public int? StoreId { get; set; }
        public int? WriteOffRequestId { get; set; }

        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }

        public string Reason { get; set; }
        public string BatchNo { get; set; }

        public virtual WriteOff WriteOff { get; set; }
        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual WriteOffRequest WriteOffRequest { get; set; }
    }
    public class Damage : BaseEntity
    {
        public string DamageNo { get; set; }
        public DateTime DamageDate { get; set; }
        public string Status { get; set; } = "Pending";

        public int ItemId { get; set; }
        public int? StoreId { get; set; }
        public decimal? Quantity { get; set; }

        public string DamageType { get; set; }
        public string Cause { get; set; }
        public string Description { get; set; }
        public string ActionTaken { get; set; }
        public string Remarks { get; set; }

        public string ReportedBy { get; set; }
        public string PhotoPath { get; set; }
        public int EstimatedLoss { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
    }
    public class Return : BaseEntity
    {
        public string ReturnNo { get; set; }
        public DateTime ReturnDate { get; set; }
        public ReturnStatus Status { get; set; }

        public string ReturnedBy { get; set; }
        public string ReturnedByType { get; set; }
        public string Reason { get; set; }
        public string ReturnType { get; set; }

        public int ItemId { get; set; }
        public int? StoreId { get; set; }
        public int ToStoreId { get; set; }
        public decimal Quantity { get; set; }

        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovalRemarks { get; set; }

        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ReceiptRemarks { get; set; }

        public bool IsRestocked { get; set; }
        public bool RestockApprovalRequired { get; set; }
        public string Remarks { get; set; }

        public bool ReturnerSignature { get; set; }
        public bool IsReceiverSignature { get; set; }
        public bool ApproverSignature { get; set; }

        public int? OriginalIssueId { get; set; }
        public string OriginalIssueNo { get; set; }
        public int? ReceiveId { get; set; }

        public string FromEntityType { get; set; }
        public int FromEntityId { get; set; }
        public int? ConditionCheckId { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual Store ToStore { get; set; }
        public virtual Issue OriginalIssue { get; set; }

        public virtual ICollection<ReturnItem> Items { get; set; } = new List<ReturnItem>();
    }
    public class StockReturn : BaseEntity
    {
        public string ReturnNumber { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Reason { get; set; }
        public string Condition { get; set; }

        public int OriginalIssueId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }

        public string ReturnedBy { get; set; }
        public bool RestockApproved { get; set; }

        public virtual Issue OriginalIssue { get; set; }
        public virtual Item Item { get; set; }
    }
    public class StockOperation : BaseEntity
    {
        public string OperationType { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; } = "Pending";
        public string Remarks { get; set; }

        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public int FromStoreId { get; set; }
        public int? ToStoreId { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store FromStore { get; set; }
        public virtual Store ToStore { get; set; }
    }
    public class IssueVoucher : BaseEntity
    {
        public string VoucherNumber { get; set; }
        public string VoucherNo { get; set; }
        public DateTime IssueDate { get; set; }

        public int IssueId { get; set; }
        public string IssuedTo { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }

        public string AuthorizedBy { get; set; }
        public string ReceivedBy { get; set; }
        public byte[] ReceiverSignature { get; set; }

        public string VoucherBarcode { get; set; }

        public virtual Issue Issue { get; set; }
    }
    public class Barcode : BaseEntity
    {
        public string BarcodeNumber { get; set; }
        public string BarcodeType { get; set; }
        public string BarcodeData { get; set; }

        public string ReferenceType { get; set; }
        public int? ReferenceId { get; set; }

        public int? ItemId { get; set; }
        public int? StoreId { get; set; }
        public string BatchNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }

        public DateTime GeneratedDate { get; set; }
        public string GeneratedBy { get; set; }
        public string PrintedBy { get; set; }
        public DateTime? PrintedDate { get; set; }
        public int PrintCount { get; set; } = 0;

        public DateTime? LastScannedDate { get; set; }
        public string LastScannedBy { get; set; }
        public string LastScannedLocation { get; set; }
        public int ScanCount { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
    }
    public class StockMovement : BaseEntity
    {
        public string MovementType { get; set; }
        public DateTime MovementDate { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public string Remarks { get; set; }

        public int ItemId { get; set; }
        public int? StoreId { get; set; }
        public int? SourceStoreId { get; set; }
        public int? DestinationStoreId { get; set; }

        public decimal? Quantity { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalValue { get; set; }

        public string ReferenceType { get; set; }
        public string ReferenceNo { get; set; }
        public int? ReferenceId { get; set; }

        public string MovedBy { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual Store SourceStore { get; set; }
        public virtual Store DestinationStore { get; set; }
        public virtual User MovedByUser { get; set; }
    }
    public class InventoryValuation : BaseEntity
    {
        public DateTime ValuationDate { get; set; }
        public DateTime CalculationDate { get; set; }
        public string ValuationType { get; set; }
        public string CostingMethod { get; set; }

        public int ItemId { get; set; }
        public int? StoreId { get; set; }

        public decimal? Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalValue { get; set; }

        public string CalculatedBy { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual User CalculatedByUser { get; set; }
    }
    public class ApprovalWorkflow : BaseEntity
    {
        public string WorkflowName { get; set; }
        public string Name { get; set; }
        public string EntityType { get; set; }
        public string TriggerCondition { get; set; }

        public int StepOrder { get; set; }
        public int RequiredLevels { get; set; }

        public string ApproverRole { get; set; }
        public string ApproverUserId { get; set; }
        public bool IsRequired { get; set; }

        public decimal? ThresholdValue { get; set; }
        public string ThresholdField { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public virtual User ApproverUser { get; set; }
        public virtual ICollection<ApprovalWorkflowLevel> Levels { get; set; } = new List<ApprovalWorkflowLevel>();
    }
    public class InventoryCycleCount : BaseEntity
    {
        public string CountNumber { get; set; }
        public DateTime CountDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string CountType { get; set; } = "Full";

        public int? StoreId { get; set; }

        public string CreatedById { get; set; }
        public string ApprovedById { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CountedBy { get; set; }
        public string VerifiedBy { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }

        public int TotalItems { get; set; }
        public int CountedItems { get; set; }
        public int VarianceItems { get; set; }
        public decimal TotalVarianceValue { get; set; }

        public string Notes { get; set; }

        public virtual Store Store { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual User ApprovedByUser { get; set; }
        public virtual User CountedByUser { get; set; }
        public virtual User VerifiedByUser { get; set; }
        public virtual ICollection<InventoryCycleCountItem> Items { get; set; } = new List<InventoryCycleCountItem>();
    }
    public class InventoryCycleCountItem : BaseEntity
    {
        public int CycleCountId { get; set; }
        public int ItemId { get; set; }

        public decimal? SystemQuantity { get; set; }
        public decimal? CountedQuantity { get; set; }
        public decimal? Variance { get; set; }
        public decimal VarianceQuantity { get; set; }
        public decimal VarianceValue { get; set; }

        public string VarianceReason { get; set; }
        public string Comments { get; set; }

        public string CountedById { get; set; }
        public DateTime? CountedDate { get; set; }
        public bool IsRecounted { get; set; }

        public bool IsAdjusted { get; set; }
        public string AdjustedBy { get; set; }
        public DateTime? AdjustmentDate { get; set; }

        public virtual InventoryCycleCount CycleCount { get; set; }
        public virtual Item Item { get; set; }
        public virtual User CountedByUser { get; set; }
        public virtual User AdjustedByUser { get; set; }
    }
    public class TemperatureLog : BaseEntity
    {
        public DateTime LogTime { get; set; }
        public string Status { get; set; } = "Normal";

        public int? StoreId { get; set; }

        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public string Unit { get; set; } = "Celsius";

        public string RecordedBy { get; set; }
        public string Equipment { get; set; }

        public bool IsAlert { get; set; }
        public string AlertReason { get; set; }

        public virtual Store Store { get; set; }
        public virtual User RecordedByUser { get; set; }
    }
    public class ExpiryTracking : BaseEntity
    {
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = "Valid";

        public int ItemId { get; set; }
        public int? StoreId { get; set; }

        public string BatchNumber { get; set; }
        public decimal Quantity { get; set; }

        public DateTime? DisposalDate { get; set; }
        public string DisposalReason { get; set; }
        public string DisposedBy { get; set; }

        public bool IsAlertSent { get; set; }
        public DateTime? AlertSentDate { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual User DisposedByUser { get; set; }
    }
    public class StockAlert : BaseEntity
    {
        public string AlertType { get; set; }
        public DateTime AlertDate { get; set; }
        public string Status { get; set; } = "Active";
        public string Message { get; set; }

        public int ItemId { get; set; }
        public int? StoreId { get; set; }

        public decimal? CurrentStock { get; set; }
        public decimal? MinimumStock { get; set; }
        public decimal? CurrentQuantity { get; set; }
        public decimal? ThresholdQuantity { get; set; }

        public string AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedDate { get; set; }
        public bool IsResolved { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
    }
    public class StockEntry : BaseEntity
    {
        public string EntryNo { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryType { get; set; } = "Direct";
        public string Status { get; set; } = "Draft";
        public string Remarks { get; set; }

        public int? StoreId { get; set; }

        public virtual Store Store { get; set; }
        public virtual ICollection<StockEntryItem> Items { get; set; } = new List<StockEntryItem>();
    }
    public class StockEntryItem : BaseEntity
    {
        public int StockEntryId { get; set; }
        public int ItemId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }

        public string Location { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int BarcodesGenerated { get; set; }

        public virtual StockEntry StockEntry { get; set; }
        public virtual Item Item { get; set; }
    }
    public class StockAdjustment : BaseEntity
    {
        public string AdjustmentNo { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public string AdjustmentType { get; set; }
        public string Status { get; set; } = "Draft";

        public int ItemId { get; set; }
        public int? StoreId { get; set; }
        public int? PhysicalInventoryId { get; set; }

        public decimal Quantity { get; set; }
        public decimal? OldQuantity { get; set; }
        public decimal? NewQuantity { get; set; }
        public decimal? AdjustmentQuantity { get; set; }
        public decimal? TotalValue { get; set; }

        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReferenceDocument { get; set; }
        public string ApprovalReference { get; set; }
        public string FiscalYear { get; set; }

        public string AdjustedBy { get; set; }
        public DateTime? AdjustedDate { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovalRemarks { get; set; }
        public bool IsApproved { get; set; }

        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectionReason { get; set; }

        public string AuditTrailJson { get; set; }

        public virtual Store Store { get; set; }
        public virtual Item Item { get; set; }
        public virtual User ApprovedByUser { get; set; }
        public virtual ICollection<StockAdjustmentItem> Items { get; set; } = new List<StockAdjustmentItem>();
    }
    public class StockAdjustmentItem : BaseEntity
    {
        public int StockAdjustmentId { get; set; }
        public int ItemId { get; set; }

        public decimal SystemQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal AdjustmentQuantity { get; set; }
        public decimal? AdjustmentValue { get; set; }

        public string BatchNo { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }

        public decimal Variance => ActualQuantity - SystemQuantity;

        public virtual StockAdjustment StockAdjustment { get; set; }
        public virtual Item Item { get; set; }
    }
    public class ShipmentTracking : BaseEntity
    {
        public string ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public string TrackingCode { get; set; }
        public string Status { get; set; }

        public string QRCode { get; set; }
        public string LastLocation { get; set; }
        public DateTime? LastUpdated { get; set; }

        public string Carrier { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string TrackingUrl { get; set; }
    }
    public class ActivityLog : BaseEntity
    {
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string IPAddress { get; set; }

        public string EntityType { get; set; }
        public string Entity { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public string Module { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }

        public virtual User User { get; set; }
    }
    public class Notification : BaseEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; } = "Normal";
        public string Category { get; set; }

        public string UserId { get; set; }
        public string TargetRole { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public bool IsSent { get; set; }
        public DateTime SentAt { get; set; }
        public bool? PushSent { get; set; }
        public DateTime? PushSentAt { get; set; }

        public string Url { get; set; }
        public string ActionUrl { get; set; }

        public string RelatedEntity { get; set; }
        public int? RelatedEntityId { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceId { get; set; }
        public string Metadata { get; set; }

        public virtual User User { get; set; }
    }
    public class Setting : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string DataType { get; set; } = "string";
        public bool IsReadOnly { get; set; }
    }
    public class UsageStatistics : BaseEntity
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
    public class SystemConfiguration : BaseEntity
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

        public virtual User ModifiedByUser { get; set; }
    }
    public class Range : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string HeadquarterLocation { get; set; }
        public string CommanderName { get; set; }
        public string CommanderRank { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string CoverageArea { get; set; }
        public string Remarks { get; set; }
        public string NameBn { get; set; }

        public virtual ICollection<Battalion> Battalions { get; set; } = new HashSet<Battalion>();
        public virtual ICollection<Zila> Zilas { get; set; } = new HashSet<Zila>();
        public virtual ICollection<Store> Stores { get; set; } = new HashSet<Store>();
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<Receive> Receives { get; set; } = new List<Receive>();
    }
    public class Battalion : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public BattalionType Type { get; set; }
        public string Location { get; set; }
        public string CommanderName { get; set; }
        public string CommanderRank { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public int? RangeId { get; set; }
        public string Remarks { get; set; }
        public int TotalPersonnel { get; set; } = 0;
        public int OfficerCount { get; set; } = 0;
        public int EnlistedCount { get; set; } = 0;
        public DateTime? EstablishedDate { get; set; }
        public OperationalStatus? OperationalStatus { get; set; }
        public string NameBn { get; set; }

        public virtual Range Range { get; set; }
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
        public virtual ICollection<BattalionStore> BattalionStores { get; set; } = new HashSet<BattalionStore>();
        public virtual ICollection<Store> Stores { get; set; } = new HashSet<Store>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<Receive> Receives { get; set; } = new List<Receive>();
    }
    public class Zila : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string NameBangla { get; set; }
        public int RangeId { get; set; }
        public string Division { get; set; }
        public string DistrictOfficerName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string OfficeAddress { get; set; }
        public decimal? Area { get; set; }
        public int? Population { get; set; }
        public string Remarks { get; set; }
        public string NameBn { get; set; }

        public virtual Range Range { get; set; }
        public virtual ICollection<Upazila> Upazilas { get; set; } = new List<Upazila>();
        public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<Receive> Receives { get; set; } = new List<Receive>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
    public class Upazila : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string NameBangla { get; set; }
        public int ZilaId { get; set; }
        public string UpazilaOfficerName { get; set; }
        public string OfficerDesignation { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string OfficeAddress { get; set; }
        public decimal? Area { get; set; }
        public int? Population { get; set; }
        public int? NumberOfUnions { get; set; }
        public int? NumberOfVillages { get; set; }
        public bool HasVDPUnit { get; set; } = false;
        public int? VDPMemberCount { get; set; }
        public string Remarks { get; set; }
        public string UpazilaChairmanName { get; set; }
        public string VDPOfficerName { get; set; }
        public string NameBn { get; set; }

        public virtual Zila Zila { get; set; }
        public virtual ICollection<Union> Unions { get; set; } = new List<Union>();
        public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<Receive> Receives { get; set; } = new List<Receive>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
    public class Union : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string NameBangla { get; set; }
        public int UpazilaId { get; set; }
        public string ChairmanName { get; set; }
        public string ChairmanContact { get; set; }
        public string SecretaryName { get; set; }
        public string SecretaryContact { get; set; }
        public string VDPOfficerName { get; set; }
        public string VDPOfficerContact { get; set; }
        public string Email { get; set; }
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
        public string Remarks { get; set; }
        public string NameBn { get; set; }

        public int TotalVDPMembers => (VDPMemberCountMale ?? 0) + (VDPMemberCountFemale ?? 0);

        public virtual Upazila Upazila { get; set; }
        public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<Receive> Receives { get; set; } = new List<Receive>();
    }
    public class Location : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? ParentLocationId { get; set; }
        public string LocationType { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }

        public virtual Location ParentLocation { get; set; }
        public virtual ICollection<Location> ChildLocations { get; set; } = new List<Location>();
        public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
    }
    public class UserStore : BaseEntity
    {
        public string UserId { get; set; }
        public int? StoreId { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string AssignedBy { get; set; }
        public bool IsPrimary { get; set; } = false;
        public DateTime? UnassignedDate { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? RemovedDate { get; set; }
        public string Role { get; set; }

        public virtual Store Store { get; set; }
        public virtual User User { get; set; }
    }
    public class BattalionStore : BaseEntity
    {
        public int BattalionId { get; set; }
        public int? StoreId { get; set; }
        public bool IsPrimaryStore { get; set; } = false;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public virtual Battalion Battalion { get; set; }
        public virtual Store Store { get; set; }
    }
    public class StoreConfiguration : BaseEntity
    {
        public int? StoreId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string Description { get; set; }
        public virtual Store Store { get; set; }
    }
    public class QualityCheck : BaseEntity
    {
        public string CheckNumber { get; set; }
        public DateTime CheckDate { get; set; }
        public int ItemId { get; set; }
        public int? PurchaseId { get; set; }
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
        public int GoodsReceiveId { get; set; }
        public DateTime CheckedDate { get; set; }
        public QualityCheckStatus OverallStatus { get; set; }

        public virtual GoodsReceive GoodsReceive { get; set; }
        public virtual Item Item { get; set; }
        public virtual Purchase Purchase { get; set; }
        public virtual User CheckedByUser { get; set; }
        public virtual ICollection<QualityCheckItem> Items { get; set; } = new List<QualityCheckItem>();
    }
    public class Document : BaseEntity
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

        public virtual User UploadedByUser { get; set; }
    }
    public class SupplierEvaluation : BaseEntity
    {
        public int VendorId { get; set; }
        public DateTime EvaluationDate { get; set; }
        public string EvaluatedBy { get; set; }
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

        public virtual Vendor Vendor { get; set; }
        public virtual User EvaluatedByUser { get; set; }
        public virtual User ApprovedByUser { get; set; }
    }
    public class ItemSpecification : BaseEntity
    {
        public int ItemId { get; set; }
        public string SpecificationName { get; set; }
        public string SpecificationValue { get; set; }
        public string Unit { get; set; }
        public string Category { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Item Item { get; set; }
    }
    public class Warranty : BaseEntity
    {
        public int ItemId { get; set; }
        public string WarrantyNumber { get; set; }
        public int? VendorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WarrantyType { get; set; }
        public string Terms { get; set; }
        public string ContactInfo { get; set; }
        public string Status { get; set; } = "Active";
        public decimal CoveredValue { get; set; }
        public string SerialNumber { get; set; }

        public virtual Item Item { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
    public class Requisition : BaseEntity
    {
        public string RequisitionNumber { get; set; }
        public DateTime RequisitionDate { get; set; }
        public string RequestedBy { get; set; }
        public string Status { get; set; } = "Draft";
        public DateTime? RequiredDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }
        public string RejectedBy { get; set; }
        public decimal TotalEstimatedCost { get; set; }
        public string Notes { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApprovalComments { get; set; }
        public DateTime RejectedDate { get; set; }
        public string Priority { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }
        public DateTime RequiredByDate { get; set; }
        public int? FromStoreId { get; set; }
        public int? ToStoreId { get; set; }
        public string FulfillmentStatus { get; set; }
        public bool AutoConvertToPO { get; set; }
        public int? PurchaseOrderId { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal ApprovedValue { get; set; }
        public string Level1ApprovedBy { get; set; }
        public DateTime? Level1ApprovedDate { get; set; }
        public string Level2ApprovedBy { get; set; }
        public DateTime? Level2ApprovedDate { get; set; }
        public string FinalApprovedBy { get; set; }
        public DateTime? FinalApprovedDate { get; set; }
        public int CurrentApprovalLevel { get; set; }

        public virtual User RequestedByUser { get; set; }
        public virtual User ApprovedByUser { get; set; }
        public virtual Store FromStore { get; set; }
        public virtual Store ToStore { get; set; }
        public virtual Purchase PurchaseOrder { get; set; }
        public virtual ICollection<RequisitionItem> RequisitionItems { get; set; } = new List<RequisitionItem>();
    }
    public class RequisitionItem : BaseEntity
    {
        public int RequisitionId { get; set; }
        public int ItemId { get; set; }
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

        public virtual Requisition Requisition { get; set; }
        public virtual Item Item { get; set; }
    }
    public class ApprovalRequest : BaseEntity
    {
        public string RequestType { get; set; }
        public string ApprovalType { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectionReason { get; set; }
        public decimal? ApprovalValue { get; set; }
        public decimal? Amount { get; set; }
        public string ApproverRole { get; set; }
        public int ApprovalLevel { get; set; }
        public int Level { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxLevel { get; set; }
        public int WorkflowId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsEscalated { get; set; }
        public DateTime? EscalatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string Remarks { get; set; }
        public string EntityData { get; set; }
        public string Role { get; set; }
        public string RequiredRole { get; set; }

        public virtual User RequestedByUser { get; set; }
        public virtual User ApprovedByUser { get; set; }
        public virtual ICollection<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
    }
    public class ApprovalLevel : BaseEntity
    {
        public int Level { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public string EntityType { get; set; }
        public bool IsSystemDefined { get; set; } = false;
    }
    public class EmailSettings
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
    public class Audit : BaseEntity
    {
        public string Entity { get; set; }
        public int EntityId { get; set; }
        public string Action { get; set; }
        public string Changes { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
    public class RolePermission : BaseEntity
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public Permission Permission { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public bool IsGranted { get; set; } = true;
    }
    public class TrackingHistory : BaseEntity
    {
        public int ShipmentTrackingId { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Carrier { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string TrackingUrl { get; set; }
        public virtual ShipmentTracking ShipmentTracking { get; set; }
    }
    public class AuditLog : BaseEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual User User { get; set; }
        public string Changes { get; set; }
    }
    public class PhysicalInventory : BaseEntity
    {
        public string CountNo { get; set; }
        public DateTime CountDate { get; set; }
        public int StoreId { get; set; }
        public int? CategoryId { get; set; }
        public string CountedBy { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Remarks { get; set; }
        public decimal? TotalSystemValue { get; set; }
        public decimal? TotalPhysicalValue { get; set; }
        public decimal VarianceValue { get; set; }
        public int TotalItemsCounted { get; set; }
        public int ItemsWithVariance { get; set; }
        public bool IsReconciled { get; set; }
        public DateTime? ReconciliationDate { get; set; }
        public string ReconciliationBy { get; set; }
        public bool IsStockFrozen { get; set; }
        public DateTime? StockFrozenAt { get; set; }
        public string CountTeam { get; set; }
        public string SupervisorId { get; set; }
        public string ReferenceNumber { get; set; }
        public int? BattalionId { get; set; }
        public int? RangeId { get; set; }
        public int? ZilaId { get; set; }
        public int? UpazilaId { get; set; }
        public string FiscalYear { get; set; }
        public PhysicalInventoryStatus Status { get; set; }
        public CountType CountType { get; set; }
        public string InitiatedBy { get; set; }
        public DateTime? InitiatedDate { get; set; }
        public string CompletedBy { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string ApprovalReference { get; set; }
        public string ApprovalRemarks { get; set; }
        public string RejectionReason { get; set; }
        public string CancellationReason { get; set; }
        public string ReviewRemarks { get; set; }
        public decimal? TotalSystemQuantity { get; set; }
        public decimal? TotalPhysicalQuantity { get; set; }
        public decimal? TotalVariance { get; set; }
        public decimal? TotalVarianceValue { get; set; }
        public bool IsAuditRequired { get; set; }
        public string AuditOfficer { get; set; }
        public AdjustmentStatus? AdjustmentStatus { get; set; }
        public DateTime? AdjustedDate { get; set; }
        public DateTime CountEndTime { get; set; }
        public DateTime AdjustmentCreatedDate { get; set; }
        public string AdjustmentNo { get; set; }
        public DateTime PostedDate { get; set; }

        // Navigation properties
        public virtual Store Store { get; set; }
        public virtual Category Category { get; set; }
        public virtual User CountedByUser { get; set; }
        public virtual User VerifiedByUser { get; set; }
        public virtual Battalion Battalion { get; set; }
        public virtual Range Range { get; set; }
        public virtual Zila Zila { get; set; }
        public virtual Upazila Upazila { get; set; }
        public virtual ICollection<PhysicalInventoryDetail> Details { get; set; }
        public virtual ICollection<PhysicalInventoryItem> Items { get; set; } = new List<PhysicalInventoryItem>();

        public PhysicalInventory()
        {
            Details = new HashSet<PhysicalInventoryDetail>();
        }
    }
    public class PhysicalInventoryDetail : BaseEntity
    {
        public int PhysicalInventoryId { get; set; }
        public int ItemId { get; set; }
        public int? CategoryId { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Variance { get; set; }
        public decimal? VarianceValue { get; set; }
        public CountStatus Status { get; set; }
        public string CountedBy { get; set; }
        public DateTime? CountedDate { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string LocationCode { get; set; }
        public string BatchNumbers { get; set; }
        public string SerialNumbers { get; set; }
        public string Remarks { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? SubCategoryId { get; set; }
        public string BatchNo { get; set; }
        public string Location { get; set; }
        public DateTime? LastCountDate { get; set; }
        public decimal? VariancePercentage { get; set; }
        public DateTime? LastIssueDate { get; set; }
        public DateTime? LastReceiveDate { get; set; }
        public string RecountRequestedBy { get; set; }
        public DateTime? RecountRequestedDate { get; set; }
        public decimal FirstCountQuantity { get; set; }
        public string FirstCountBy { get; set; }
        public DateTime FirstCountTime { get; set; }
        public string CountLocation { get; set; }
        public string CountRemarks { get; set; }
        public VarianceType VarianceType { get; set; }
        public decimal RecountQuantity { get; set; }
        public string RecountBy { get; set; }
        public DateTime RecountTime { get; set; }
        public decimal BlindCountQuantity { get; set; }
        public string BlindCountBy { get; set; }
        public DateTime BlindCountTime { get; set; }
        public bool BlindCountCompleted { get; set; }

        // Navigation properties
        public virtual PhysicalInventory PhysicalInventory { get; set; }
        public virtual Item Item { get; set; }
        public virtual Category Category { get; set; }
    }
    public class PhysicalInventoryItem : BaseEntity
    {
        public int PhysicalInventoryId { get; set; }
        public int ItemId { get; set; }
        public decimal SystemQuantity { get; set; }
        public decimal PhysicalQuantity { get; set; }
        public decimal Variance { get; set; }
        public decimal UnitCost { get; set; }
        public decimal SystemValue { get; set; }
        public decimal PhysicalValue { get; set; }
        public decimal VarianceValue { get; set; }
        public string Location { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? CountedAt { get; set; }
        public string CountedBy { get; set; }
        public bool IsRecounted { get; set; }
        public decimal? RecountQuantity { get; set; }
        public string Notes { get; set; }
        public string AdjustmentStatus { get; set; }

        // Navigation properties
        public virtual PhysicalInventory PhysicalInventory { get; set; }
        public virtual Item Item { get; set; }
    }
    //public class StockReconciliation : BaseEntity
    //{
    //    public string ReconciliationNo { get; set; }
    //    public DateTime ReconciliationDate { get; set; }
    //    public int StoreId { get; set; }
    //    public int? PhysicalInventoryId { get; set; }
    //    public decimal TotalVariance { get; set; }
    //    public decimal TotalVarianceValue { get; set; }
    //    public string Status { get; set; }
    //    public DateTime? ApprovedDate { get; set; }
    //    public string ApprovedBy { get; set; }
    //    public string ReconciliationType { get; set; }
    //    public decimal TotalVarianceQuantity { get; set; }
    //    public int TotalItemsReconciled { get; set; }
    //    public string Reason { get; set; }
    //    public string InitiatedBy { get; set; }
    //    public string ApprovalComments { get; set; }
    //    public string RejectedBy { get; set; }
    //    public DateTime? RejectionDate { get; set; }
    //    public string RejectionReason { get; set; }
    //    public bool AutoCreateAdjustments { get; set; }
    //    public int? StockAdjustmentId { get; set; }

    //    // Navigation properties
    //    public virtual Store Store { get; set; }
    //    public virtual PhysicalInventory PhysicalInventory { get; set; }
    //    public virtual User InitiatedByUser { get; set; }
    //    public virtual User ApprovedByUser { get; set; }
    //    public virtual StockAdjustment StockAdjustment { get; set; }
    //    public virtual ICollection<StockReconciliationItem> Items { get; set; } = new List<StockReconciliationItem>();
    //}
    //public class StockReconciliationItem : BaseEntity
    //{
    //    public int StockReconciliationId { get; set; }
    //    public int ItemId { get; set; }
    //    public decimal SystemQuantity { get; set; }
    //    public decimal ActualQuantity { get; set; }
    //    public decimal Variance { get; set; }
    //    public decimal UnitCost { get; set; }
    //    public decimal VarianceValue { get; set; }
    //    public string VarianceType { get; set; }
    //    public string Reason { get; set; }
    //    public string Action { get; set; }
    //    public bool IsAdjusted { get; set; }
    //    public int? StockAdjustmentItemId { get; set; }
    //    public string Notes { get; set; }
    //    public decimal PhysicalQuantity { get; set; }
    //    public string AdjustedQuantity { get; set; }

    //    // Navigation properties
    //    public virtual StockReconciliation StockReconciliation { get; set; }
    //    public virtual Item Item { get; set; }
    //    public virtual StockAdjustmentItem StockAdjustmentItem { get; set; }
    //}
    public class CycleCountSchedule : BaseEntity
    {
        public string ScheduleCode { get; set; }
        public string ScheduleName { get; set; }
        public int StoreId { get; set; }
        public string Frequency { get; set; }
        public string CountMethod { get; set; }
        public DateTime NextScheduledDate { get; set; }
        public DateTime? LastExecutedDate { get; set; }
        public string ABCClass { get; set; }
        public decimal? MinimumValue { get; set; }
        public int? ItemsPerCount { get; set; }
        public string AssignedTo { get; set; }
        public string Notes { get; set; }
        public string Description { get; set; }
        public int DayOfWeek { get; set; }
        public string Status { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual Store Store { get; set; }
        public virtual User AssignedToUser { get; set; }
        public virtual Category Category { get; set; }
    }
    public class DigitalSignature : BaseEntity
    {
        public string ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public string SignatureType { get; set; }
        public string SignedBy { get; set; }
        public DateTime SignedAt { get; set; }
        public string SignatureData { get; set; }
        public string DeviceInfo { get; set; }
        public string IPAddress { get; set; }
        public string LocationInfo { get; set; }
        public bool IsVerified { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public DateTime SignedDate { get; set; }
        public string SignatureHash { get; set; }
        public string DeviceId { get; set; }
        public string Location { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string VerificationCode { get; set; }
        public string VerificationMethod { get; set; }
        public string VerificationFailReason { get; set; }

        // Navigation properties
        public virtual User SignedByUser { get; set; }
        public virtual User VerifiedByUser { get; set; }
    }
    public class BatchTracking : BaseEntity
    {
        public string BatchNumber { get; set; }
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public decimal Quantity { get; set; }
        public decimal InitialQuantity { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SupplierBatchNo { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public DateTime? ConsumedDate { get; set; }
        public string Notes { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public string Supplier { get; set; }
        public decimal? UnitCost { get; set; }
        public string Remarks { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public DateTime LastIssueDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public string VendorId { get; set; }
        public string VendorBatchNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string QualityCheckStatus { get; set; }
        public DateTime? QualityCheckDate { get; set; }
        public string QualityCheckBy { get; set; }
        public string StorageLocation { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public DateTime? QuarantineDate { get; set; }
        public string QuarantinedBy { get; set; }
        public string QuarantineReason { get; set; }
        public int? TransferredFromBatchId { get; set; }
        public string TransferReference { get; set; }

        // Navigation properties
        public virtual ICollection<BatchMovement> BatchMovements { get; set; } = new List<BatchMovement>();
        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
    }
    public class BatchMovement : BaseEntity
    {
        public int BatchId { get; set; }
        public string MovementType { get; set; }
        public decimal Quantity { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime MovementDate { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public int BatchTrackingId { get; set; }
        public int ReferenceId { get; set; }
        public string MovedBy { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal NewBalance { get; set; }
        public decimal OldBalance { get; set; }

        // Navigation properties
        public virtual BatchTracking Batch { get; set; }
        public virtual BatchTracking BatchTracking { get; set; }
    }
    public class ApprovalThreshold : BaseEntity
    {
        public string EntityType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public int ApprovalLevel { get; set; }
        public string RequiredRole { get; set; }
        public string Description { get; set; }
    }
    public class ApprovalHistory : BaseEntity
    {
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string Comments { get; set; }
        public string Action { get; set; }
        public int ApprovalLevel { get; set; }
        public string ApproverRole { get; set; }
        public string ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public string NewStatus { get; set; }
        public string PreviousStatus { get; set; }
        public int ApprovalRequestId { get; set; }
        public int Level { get; set; }

        // Navigation properties
        public virtual ApprovalRequest ApprovalRequest { get; set; }
    }
    public class StoreStock : BaseEntity
    {
        public int StoreId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal MaxQuantity { get; set; }
        public DateTime LastUpdated { get; set; }
        public decimal ReorderLevel { get; set; }
        public string LastUpdatedBy { get; set; }
        public decimal? LastPurchasePrice { get; set; }
        public decimal? AveragePrice { get; set; }

        // Navigation properties
        public virtual Store Store { get; set; }
        public virtual Item Item { get; set; }
    }
    public class AuditReport : BaseEntity
    {
        public string ReferenceNumber { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string AuditType { get; set; }
        public DateTime AuditDate { get; set; }
        public string AuditorName { get; set; }
        public string Findings { get; set; }
        public string Recommendations { get; set; }
        public string ComplianceStatus { get; set; }
        public string FiscalYear { get; set; }
        public int? InventoryId { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public string StoreLevel { get; set; }
        public string BattalionName { get; set; }
        public string RangeName { get; set; }
        public string ZilaName { get; set; }
        public string UpazilaName { get; set; }
        public decimal TotalSystemValue { get; set; }
        public decimal TotalPhysicalValue { get; set; }
        public decimal TotalVarianceValue { get; set; }
        public string AuditFindingsJson { get; set; }

        // Navigation properties
        public virtual PhysicalInventory PhysicalInventory { get; set; }
    }
    public class PurchaseOrder : BaseEntity
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int VendorId { get; set; }
        public int? StoreId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Remarks { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }

        // Navigation properties
        public virtual Vendor Vendor { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }
    public class PurchaseOrderItem : BaseEntity
    {
        public int PurchaseOrderId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? ReceivedQuantity { get; set; }

        // Navigation properties
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Item Item { get; set; }
    }
    public class GoodsReceive : BaseEntity
    {
        public int PurchaseId { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string ReceivedBy { get; set; }
        public string InvoiceNo { get; set; }
        public string ChallanNo { get; set; }
        public ReceiveStatus Status { get; set; }

        // Navigation properties
        public virtual Purchase Purchase { get; set; }
        public virtual ICollection<GoodsReceiveItem> Items { get; set; } = new List<GoodsReceiveItem>();
    }
    public class GoodsReceiveItem : BaseEntity
    {
        public int GoodsReceiveId { get; set; }
        public int ItemId { get; set; }
        public decimal ReceivedQuantity { get; set; }
        public decimal? AcceptedQuantity { get; set; }
        public decimal? RejectedQuantity { get; set; }
        public QualityCheckStatus QualityCheckStatus { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }

        // Navigation properties
        public virtual GoodsReceive GoodsReceive { get; set; }
        public virtual Item Item { get; set; }
    }
    public class QualityCheckItem : BaseEntity
    {
        public int QualityCheckId { get; set; }
        public int ItemId { get; set; }
        public decimal CheckedQuantity { get; set; }
        public decimal PassedQuantity { get; set; }
        public decimal FailedQuantity { get; set; }
        public QualityCheckStatus Status { get; set; }
        public string Remarks { get; set; }
        public string CheckParameters { get; set; }

        // Navigation properties
        public virtual QualityCheck QualityCheck { get; set; }
        public virtual Item Item { get; set; }
    }
    public class ReturnItem : BaseEntity
    {
        public int ReturnId { get; set; }
        public int ItemId { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? AcceptedQuantity { get; set; }
        public decimal? RejectedQuantity { get; set; }
        public string Condition { get; set; }
        public string CheckedCondition { get; set; }
        public string ReturnReason { get; set; }
        public string BatchNo { get; set; }
        public string Remarks { get; set; }
        public string ApprovalRemarks { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string ReceivedBy { get; set; }

        // Navigation properties
        public virtual Return Return { get; set; }
        public virtual Item Item { get; set; }
    }
    public class ConditionCheck : BaseEntity
    {
        public int ReturnId { get; set; }
        public string CheckedBy { get; set; }
        public DateTime CheckedDate { get; set; }
        public string OverallCondition { get; set; }
        public int ItemId { get; set; }
        public string Condition { get; set; }
        public DateTime CheckDate { get; set; }

        // Navigation properties
        public virtual Return Return { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<ConditionCheckItem> Items { get; set; } = new List<ConditionCheckItem>();
    }
    public class ConditionCheckItem : BaseEntity
    {
        public int ConditionCheckId { get; set; }
        public int ItemId { get; set; }
        public decimal CheckedQuantity { get; set; }
        public decimal GoodQuantity { get; set; }
        public decimal DamagedQuantity { get; set; }
        public decimal ExpiredQuantity { get; set; }
        public string Condition { get; set; }
        public string Remarks { get; set; }
        public string Photos { get; set; }

        // Navigation properties
        public virtual ConditionCheck ConditionCheck { get; set; }
        public virtual Item Item { get; set; }
    }
    public class DamageReport : BaseEntity
    {
        public string ReportNo { get; set; }
        public int StoreId { get; set; }
        public DateTime ReportDate { get; set; }
        public string ReportedBy { get; set; }
        public DamageStatus Status { get; set; }
        public decimal TotalValue { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string DamageType { get; set; }
        public string Cause { get; set; }
        public decimal EstimatedLoss { get; set; }

        // Navigation properties
        public virtual Store Store { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<DamageReportItem> Items { get; set; } = new List<DamageReportItem>();
    }
    public class DamageReportItem : BaseEntity
    {
        public int DamageReportId { get; set; }
        public int ItemId { get; set; }
        public decimal DamagedQuantity { get; set; }
        public string DamageType { get; set; }
        public DateTime DamageDate { get; set; }
        public DateTime DiscoveredDate { get; set; }
        public string DamageDescription { get; set; }
        public decimal EstimatedValue { get; set; }
        public string PhotoUrls { get; set; }
        public string BatchNo { get; set; }
        public string Remarks { get; set; }

        // Navigation properties
        public virtual DamageReport DamageReport { get; set; }
        public virtual Item Item { get; set; }
    }
    public class WriteOffRequest : BaseEntity
    {
        public string RequestNo { get; set; }
        public int? DamageReportId { get; set; }
        public string DamageReportNo { get; set; }
        public int StoreId { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestedBy { get; set; }
        public decimal TotalValue { get; set; }
        public WriteOffStatus Status { get; set; }
        public string Justification { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovalRemarks { get; set; }
        public string ApprovalReference { get; set; }
        public string ExecutedBy { get; set; }
        public DateTime? ExecutedDate { get; set; }
        public string DisposalMethod { get; set; }
        public string DisposalRemarks { get; set; }
        public string Reason { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectionReason { get; set; }

        // Navigation properties
        public virtual DamageReport DamageReport { get; set; }
        public virtual Store Store { get; set; }
        public virtual User RequestedByUser { get; set; }
        public virtual ICollection<WriteOffItem> Items { get; set; } = new List<WriteOffItem>();
    }
    public class DisposalRecord : BaseEntity
    {
        public int WriteOffId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string DisposalMethod { get; set; }
        public DateTime DisposalDate { get; set; }
        public string DisposalLocation { get; set; }
        public string DisposalCompany { get; set; }
        public string DisposalCertificateNo { get; set; }
        public string DisposedBy { get; set; }
        public string WitnessedBy { get; set; }
        public string PhotoUrls { get; set; }
        public string Remarks { get; set; }
        public string DisposalNo { get; set; }
        public string AuthorizedBy { get; set; }

        // Navigation properties
        public virtual WriteOffRequest WriteOff { get; set; }
        public virtual Item Item { get; set; }
        public virtual User AuthorizedByUser { get; set; }
    }
    public class DamageRecord : BaseEntity
    {
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public decimal DamagedQuantity { get; set; }
        public DateTime DamageDate { get; set; }
        public string DamageReason { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNo { get; set; }
        public string Status { get; set; }
        public int ReturnId { get; set; }
        public decimal Quantity { get; set; }
        public string DamageType { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }

        // Navigation properties
        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual Return Return { get; set; }
    }
    public class ExpiredRecord : BaseEntity
    {
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public decimal ExpiredQuantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceNo { get; set; }
        public string Status { get; set; }
        public int BatchId { get; set; }
        public decimal Quantity { get; set; }
        public string DisposalMethod { get; set; }

        // Navigation properties
        public virtual Store Store { get; set; }
        public virtual Item Item { get; set; }
        public virtual BatchTracking Batch { get; set; }
    }
    public class BatchSignature : BaseEntity
    {
        public string BatchNo { get; set; }
        public string SignedBy { get; set; }
        public DateTime SignedDate { get; set; }
        public int TotalItems { get; set; }
        public string Purpose { get; set; }
        public string SignatureData { get; set; }
        public string SignatureHash { get; set; }
        public int BatchId { get; set; }
        public string SignatureType { get; set; }
        public DateTime SignedAt { get; set; }

        // Navigation properties
        public virtual BatchTracking Batch { get; set; }
        public virtual User SignedByUser { get; set; }
        public virtual ICollection<BatchSignatureItem> Items { get; set; } = new List<BatchSignatureItem>();
    }
    public class BatchSignatureItem : BaseEntity
    {
        public int BatchSignatureId { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public string Remarks { get; set; }

        // Navigation properties
        public virtual BatchSignature BatchSignature { get; set; }
    }
    public class SignatureOTP : BaseEntity
    {
        public string UserId { get; set; }
        public string OTPCode { get; set; }
        public string Purpose { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public string OTP { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
    public class ApprovalStep : BaseEntity
    {
        public int ApprovalRequestId { get; set; }
        public int StepLevel { get; set; }
        public string ApproverRole { get; set; }
        public string AssignedTo { get; set; }
        public ApprovalStatus Status { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Comments { get; set; }
        public bool IsEscalated { get; set; }
        public DateTime? EscalatedAt { get; set; }
        public string SpecificApproverId { get; set; }
        public string EscalatedBy { get; set; }
        public DateTime EscalatedDate { get; set; }
        public string EscalationReason { get; set; }

        // Navigation properties
        public virtual ApprovalRequest ApprovalRequest { get; set; }
    }
    public class ApprovalWorkflowLevel : BaseEntity
    {
        public int WorkflowId { get; set; }
        public int Level { get; set; }
        public string ApproverRole { get; set; }
        public string SpecificApproverId { get; set; }
        public decimal? ThresholdAmount { get; set; }
        public bool CanEscalate { get; set; }
        public int? TimeoutHours { get; set; }

        // Navigation properties
        public virtual ApprovalWorkflow Workflow { get; set; }
    }
    public class ApprovalDelegation : BaseEntity
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EntityType { get; set; }
        public string Reason { get; set; }

        // Navigation properties
        public virtual User FromUser { get; set; }
        public virtual User ToUser { get; set; }
    }
    public class Unit : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public decimal ConversionFactor { get; set; }
        public string BaseUnit { get; set; }
        public string NameBn { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
    }
    public class TransferShipment : BaseEntity
    {
        public int TransferId { get; set; }
        public string ShipmentNo { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string Carrier { get; set; }
        public string TrackingNo { get; set; }
        public string Status { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? ActualDelivery { get; set; }
        public string Notes { get; set; }
        public DateTime ShippedDate { get; set; }
        public string ShippedBy { get; set; }
        public string PackingListNo { get; set; }
        public string TransportCompany { get; set; }
        public string VehicleNo { get; set; }
        public string DriverName { get; set; }
        public string SealNo { get; set; }
        public string DriverContact { get; set; }
        public DateTime EstimatedArrival { get; set; }
        public DateTime ActualArrival { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceiptCondition { get; set; }

        // Navigation properties
        public virtual Transfer Transfer { get; set; }
        public virtual ICollection<TransferShipmentItem> Items { get; set; } = new List<TransferShipmentItem>();
    }
    public class TransferShipmentItem : BaseEntity
    {
        public int ShipmentId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string PackageNo { get; set; }
        public string Condition { get; set; }
        public decimal ShippedQuantity { get; set; }
        public int PackageCount { get; set; }
        public string PackageDetails { get; set; }
        public string BatchNo { get; set; }

        // Navigation properties
        public virtual TransferShipment Shipment { get; set; }
        public virtual Item Item { get; set; }
    }
    public class TransferDiscrepancy : BaseEntity
    {
        public int TransferId { get; set; }
        public int ItemId { get; set; }
        public decimal ExpectedQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal DiscrepancyQuantity { get; set; }
        public string Reason { get; set; }
        public string Resolution { get; set; }
        public bool IsResolved { get; set; }
        public decimal? ShippedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? Variance { get; set; }
        public string ReportedBy { get; set; }
        public DateTime ReportedDate { get; set; }
        public string Status { get; set; }

        // Navigation properties
        public virtual Transfer Transfer { get; set; }
        public virtual Item Item { get; set; }
    }
    public class UserNotificationPreferences : BaseEntity
    {
        public string UserId { get; set; }
        public bool EmailEnabled { get; set; }
        public bool SmsEnabled { get; set; }
        public bool PushEnabled { get; set; }
        public bool LowStockAlerts { get; set; }
        public bool ExpiryAlerts { get; set; }
        public bool ApprovalAlerts { get; set; }
        public bool SystemAlerts { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }

    public class Signature : BaseEntity
    {
        public string ReferenceType { get; set; } // "Issue", "Purchase", "Transfer", etc.
        public int ReferenceId { get; set; }
        public string SignatureType { get; set; } // "Issuer", "Approver", "Receiver"
        public string SignatureData { get; set; } // Base64 data - nvarchar(max)
        public string SignerName { get; set; }
        public string SignerBadgeId { get; set; }
        public string SignerDesignation { get; set; }
        public DateTime SignedDate { get; set; }
        public string IPAddress { get; set; }
        public string DeviceInfo { get; set; }

        // Navigation properties
        public virtual Issue Issue { get; set; }
    }

    public class AllotmentLetter : BaseEntity
    {
        public string AllotmentNo { get; set; }
        public DateTime AllotmentDate { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }

        public string IssuedTo { get; set; }
        public string IssuedToType { get; set; } // Battalion, Range, Zila, Upazila

        public int? IssuedToBattalionId { get; set; }
        public int? IssuedToRangeId { get; set; }
        public int? IssuedToZilaId { get; set; }
        public int? IssuedToUpazilaId { get; set; }

        public int FromStoreId { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; } // Draft, Approved, Active, Expired, Cancelled

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectionReason { get; set; }

        public string DocumentPath { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }

        // Bengali/Government Format Fields
        public string Subject { get; set; }
        public string SubjectBn { get; set; }
        public string BodyText { get; set; }
        public string BodyTextBn { get; set; }
        public DateTime? CollectionDeadline { get; set; }
        public string SignatoryName { get; set; }
        public string SignatoryDesignation { get; set; }
        public string SignatoryDesignationBn { get; set; }
        public string SignatoryId { get; set; }
        public string SignatoryPhone { get; set; }
        public string SignatoryEmail { get; set; }
        public string BengaliDate { get; set; }

        // Navigation
        public virtual Store FromStore { get; set; }
        public virtual Battalion IssuedToBattalion { get; set; }
        public virtual Range IssuedToRange { get; set; }
        public virtual Zila IssuedToZila { get; set; }
        public virtual Upazila IssuedToUpazila { get; set; }
        public virtual ICollection<AllotmentLetterItem> Items { get; set; } = new List<AllotmentLetterItem>();
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();

        // NEW: Multiple recipients support
        public virtual ICollection<AllotmentLetterRecipient> Recipients { get; set; } = new List<AllotmentLetterRecipient>();
    }

    public class AllotmentLetterItem : BaseEntity
    {
        public int AllotmentLetterId { get; set; }
        public int ItemId { get; set; }
        public decimal AllottedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string Unit { get; set; }
        public string UnitBn { get; set; }
        public string ItemNameBn { get; set; }
        public string Remarks { get; set; }

        public virtual AllotmentLetter AllotmentLetter { get; set; }
        public virtual Item Item { get; set; }
    }

    // New entity for multiple recipients in one allotment letter
    public class AllotmentLetterRecipient : BaseEntity
    {
        public int AllotmentLetterId { get; set; }
        public string RecipientType { get; set; } // "Range", "Battalion", "Zila", "Upazila", "Union"

        // Foreign keys to different entity types (only one will be used based on RecipientType)
        public int? RangeId { get; set; }
        public int? BattalionId { get; set; }
        public int? ZilaId { get; set; }
        public int? UpazilaId { get; set; }
        public int? UnionId { get; set; }

        public string RecipientName { get; set; } // For display/reporting
        public string RecipientNameBn { get; set; } // Bengali name
        public string StaffStrength { get; set; } // কর্মরত জনবল (optional)
        public string Remarks { get; set; }
        public int SerialNo { get; set; } // Display order in PDF

        // Navigation properties
        public virtual AllotmentLetter AllotmentLetter { get; set; }
        public virtual Range Range { get; set; }
        public virtual Battalion Battalion { get; set; }
        public virtual Zila Zila { get; set; }
        public virtual Upazila Upazila { get; set; }
        public virtual Union Union { get; set; }

        // Items for this specific recipient
        public virtual ICollection<AllotmentLetterRecipientItem> Items { get; set; } = new List<AllotmentLetterRecipientItem>();
    }

    // Junction table: Recipient + Item + Quantity
    public class AllotmentLetterRecipientItem : BaseEntity
    {
        public int AllotmentLetterRecipientId { get; set; }
        public int ItemId { get; set; }
        public decimal AllottedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainingQuantity => AllottedQuantity - IssuedQuantity;
        public string Unit { get; set; }
        public string UnitBn { get; set; }
        public string ItemNameBn { get; set; }
        public string Remarks { get; set; }

        // Navigation properties
        public virtual AllotmentLetterRecipient Recipient { get; set; }
        public virtual Item Item { get; set; }
    }

    // Signatory Presets for AllotmentLetter
    public class SignatoryPreset : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string PresetName { get; set; }

        [Required]
        [MaxLength(200)]
        public string PresetNameBn { get; set; }

        [Required]
        [MaxLength(200)]
        public string SignatoryName { get; set; }

        [MaxLength(200)]
        public string SignatoryDesignation { get; set; }

        [MaxLength(200)]
        public string SignatoryDesignationBn { get; set; }

        [MaxLength(100)]
        public string SignatoryId { get; set; }

        [MaxLength(100)]
        public string SignatoryPhone { get; set; }

        [MaxLength(200)]
        public string SignatoryEmail { get; set; }

        [MaxLength(100)]
        public string Department { get; set; }

        public bool IsDefault { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
    }

    /// <summary>
    /// Ledger Book for tracking physical register books
    /// Helps manage multiple ledger books and current page tracking
    /// </summary>
    public class LedgerBook : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string LedgerNo { get; set; } // e.g., "LB-2025-001", "ISSUE-01", "RCV-03"

        [Required]
        [MaxLength(200)]
        public string BookName { get; set; } // e.g., "Issue Ledger Book 2025", "Stock Receipt Register 3"

        [MaxLength(50)]
        public string BookType { get; set; } // "Issue", "Receive", "Transfer", "General"

        [MaxLength(500)]
        public string Description { get; set; }

        public int? StoreId { get; set; }
        public virtual Store Store { get; set; }

        public int TotalPages { get; set; } = 500; // Total pages in the physical book

        public int CurrentPageNo { get; set; } = 1; // Last used page number

        public DateTime StartDate { get; set; } // When the book was opened

        public DateTime? EndDate { get; set; } // When the book was closed/full

        public bool IsClosed { get; set; } = false; // true when book is full or archived

        [MaxLength(100)]
        public string Location { get; set; } // Physical location: "Store Room A", "Office Cabinet 2"

        [MaxLength(500)]
        public string Remarks { get; set; }
    }
}