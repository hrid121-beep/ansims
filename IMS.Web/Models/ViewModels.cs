using IMS.Application;
using IMS.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IMS.Web.Models;

public class DashboardViewModel
{
    // Basic Statistics
    public int TotalItems { get; set; }
    public int ActiveItems { get; set; }
    public int TotalStores { get; set; }
    public int TotalUsers { get; set; }
    public decimal TotalStockValue { get; set; }
    public int StockAccuracy { get; set; }

    // Alerts & Issues
    public int CriticalAlerts { get; set; }
    public int LowStockItems { get; set; }
    public int PendingIssues { get; set; }
    public int ExpiringItems { get; set; }

    // Transactions
    public int TodayTransactions { get; set; }
    public int TransactionGrowth { get; set; }
    public int PendingRequisitions { get; set; }
    public int ActivePurchases { get; set; }
    public decimal PurchaseValue { get; set; }

    // Approval Counts
    public int TotalPendingApprovals { get; set; }
    public int PendingRequisitionApprovals { get; set; }
    public int PendingPurchaseApprovals { get; set; }
    public int PendingIssueApprovals { get; set; }
    public int PendingAdjustmentApprovals { get; set; }

    // Physical Inventory
    public int ScheduledCounts { get; set; }
    public int InProgressCounts { get; set; }
    public int PendingCountReview { get; set; }

    // Organization Stats
    public int TotalRanges { get; set; }
    public int TotalBattalions { get; set; }
    public int TotalZilas { get; set; }
    public int TotalUpazilas { get; set; }

    // Performance Metrics
    public decimal InventoryTurnover { get; set; }
    public int FillRate { get; set; }
    public int AverageLeadTime { get; set; }

    // Lists for Display
    public List<PurchaseDto> RecentPurchases { get; set; } = new();
    public List<IssueDto> RecentIssues { get; set; } = new();
    public List<ItemDto> LowStockItemsList { get; set; } = new();
    public List<ExpiringItemDto> ExpiringItemsList { get; set; } = new();
    public List<ActivityDto> RecentActivities { get; set; } = new();
    public List<StockAlertDto> CriticalAlertsList { get; set; } = new();
    public List<DashboardAlertDto> Alerts { get; set; } = new();

    // Stats object for compatibility with view
    public dynamic Stats { get; set; }

    // Valuation Summary
    public ValuationSummaryDto ValuationSummary { get; set; }

    // Chart Data
    public List<string> ChartLabels { get; set; } = new();
    public List<int> IssueData { get; set; } = new();
    public List<int> ReceiveData { get; set; } = new();
    public List<int> PurchaseData { get; set; } = new();
    public List<string> CategoryLabels { get; set; } = new();
    public List<int> CategoryData { get; set; } = new();
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]  // CHANGED
    [Display(Name = "Username")]  // CHANGED
    public string UserName { get; set; }  // CHANGED from Email

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; }
}

public class UserViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }  // ADD this
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Designation { get; set; }
    public string Department { get; set; }  // ADD
    public string PhoneNumber { get; set; }  // ADD
    public List<string> Roles { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }  // ADD
}

public class CreateUserViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Required]
    [Display(Name = "Designation")]
    public string Designation { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "Role")]
    public string Role { get; set; }


    [Required]
    [Display(Name = "Username")]  // NEW - username primary
    public string UserName { get; set; }

    [EmailAddress]
    [Display(Name = "Email (Optional)")]  // MODIFIED - now optional
    public string Email { get; set; }

    [Display(Name = "Department")]
    public string Department { get; set; }

    [Display(Name = "Badge Number")]
    public string BadgeNumber { get; set; }

    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}


public class EditUserViewModel
{
    public string Id { get; set; }

    [Required]
    [Display(Name = "Username")]  // ADD this
    public string UserName { get; set; }

    [EmailAddress]
    [Display(Name = "Email (Optional)")]  // MODIFIED - optional
    public string Email { get; set; }

    public string Department { get; set; }
    public string BadgeNumber { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }

    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Required]
    [Display(Name = "Designation")]
    public string Designation { get; set; }

    [Display(Name = "Current Role")]
    public string CurrentRole { get; set; }
}

public class ResetPasswordViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; }
}

public class CreateRoleViewModel
{
    [Required]
    [Display(Name = "Role Name")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }
}

public class EditRoleViewModel
{
    public string Id { get; set; }

    [Required]
    [Display(Name = "Role Name")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }
}


public class ReceivePurchaseViewModel
{
    public int PurchaseId { get; set; }
    public string PurchaseOrderNo { get; set; }
    public string VendorName { get; set; }
    public string Remarks { get; set; }
    public string InvoiceNo { get; set; }
    public string ChallanNo { get; set; }
    public List<ReceiveItemViewModel> Items { get; set; } = new List<ReceiveItemViewModel>();

}

public class ReceiveItemViewModel
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public decimal OrderedQuantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal ReceivedQuantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal AcceptedQuantity { get; set; }

    public decimal RejectedQuantity { get; set; }
    public string BatchNo { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Remarks { get; set; }
    public string ItemCode { get; set; }
}

public class ApprovalViewModel
{
    public int Id { get; set; }
    public string EntityType { get; set; }
    public int EntityId { get; set; }
    public decimal? Amount { get; set; }
    public string RequestedBy { get; set; }
    public string RequestedByName { get; set; }
    public string RequestedByRole { get; set; }
    public DateTime RequestedDate { get; set; }
    public string Description { get; set; }
    public string Priority { get; internal set; }
    public int CurrentLevel { get; internal set; }
}

public class TransferViewModel
{
    public int FromStoreId { get; set; }
    public int ToStoreId { get; set; }
    public string Remarks { get; set; }
    public List<TransferItemViewModel> Items { get; set; } = new();
}

public class TransferItemViewModel
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public decimal Quantity { get; set; }
    public string Remarks { get; set; }
}

public class TransferReceiptViewModel
{
    public int TransferId { get; set; }
    public string TransferNo { get; set; }
    public string FromStoreName { get; set; }
    public string ToStoreName { get; set; }
    public DateTime TransferDate { get; set; }
    public string Status { get; set; }
    public List<TransferReceiptItemViewModel> Items { get; set; } = new();
}

public class TransferReceiptItemViewModel
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public decimal TransferredQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string Location { get; set; }
    public string Remarks { get; set; }
}

public class EmergencyRequestViewModel
{
    public string IssuedToType { get; set; }
    public string RecipientName { get; set; }
    public string BadgeNo { get; set; }
    public string Purpose { get; set; }
    public int? PreferredStoreId { get; set; }
    public List<EmergencyItemViewModel> Items { get; set; } = new();
}

public class EmergencyItemViewModel
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public decimal Quantity { get; set; }
}


public class AddUserToStoreViewModel
{
    [Required]
    public int StoreId { get; set; }

    [Required]
    public string UserId { get; set; }

    public bool SetAsPrimary { get; set; }
}

public class StoreUserAssignmentViewModel
{
    public int StoreId { get; set; }
    public string StoreName { get; set; }
    public IEnumerable<UserDto> AssignedUsers { get; set; } = new List<UserDto>();
    public List<SelectListItem> AvailableUsers { get; set; } = new List<SelectListItem>();
    public List<string> SelectedUserIds { get; set; } = new List<string>();
}

public class AddItemsToStoreViewModel
{
    public int StoreId { get; set; }

    [Required(ErrorMessage = "Please select at least one item")]
    public List<int> SelectedItemIds { get; set; } = new List<int>();

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Minimum stock must be 0 or greater")]
    public decimal MinStock { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Maximum stock must be 0 or greater")]
    public decimal MaxStock { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Reorder level must be 0 or greater")]
    public decimal ReorderLevel { get; set; }
}

public class StoreUsersViewModel
{
    public int StoreId { get; set; }
    public string StoreName { get; set; }
    public string StoreCode { get; set; }
    public string PrimaryKeeper { get; set; }

    public List<AssignedUserDto> AssignedUsers { get; set; } = new List<AssignedUserDto>();
    public List<AvailableUserDto> AvailableUsers { get; set; } = new List<AvailableUserDto>();
}

// For Store Distribution Modal
public class StoreDistributionViewModel
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public List<StoreDistributionDto> StoreDistributions { get; set; }
}

public class LowStockItemViewModel
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public int StoreId { get; set; }
    public string StoreName { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal MaximumStock { get; set; }
    public decimal SuggestedQuantity { get; set; }
}
public class InspectionViewModel
{
    public int PurchaseId { get; set; }
    public string PurchaseOrderNo { get; set; }
    public int FromStoreId { get; set; }
    public string FromStoreName { get; set; }
    public DateTime InspectionDate { get; set; }
    public string InspectedBy { get; set; }
    public int? ToStoreId { get; set; }
    public string ToStoreName { get; set; }
    public string InspectorName { get; set; }
    public string Remarks { get; set; }
    public List<InspectionItemViewModel> Items { get; set; } = new List<InspectionItemViewModel>();
}

public class InspectionItemViewModel
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public decimal Quantity { get; set; }
    public string Condition { get; set; } = "Serviceable";
    public string InspectionRemarks { get; set; }
    public bool ApprovedForTransfer { get; set; } = true;
    public string Remarks { get; set; }
}

public class PurchaseInspectionViewModel
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public decimal Quantity { get; set; }
    public string Condition { get; set; }
    public bool ApprovedForTransfer { get; set; }
    public string InspectionRemarks { get; set; }

    public int PurchaseId { get; set; }
    public string PurchaseOrderNo { get; set; }
    public string VendorName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime InspectionDate { get; set; }
    public string InspectorName { get; set; }
    public string InspectorRemarks { get; set; }
    public List<InspectionItemViewModel> Items { get; set; } = new();
}

public class AllotmentLetterApprovalViewModel
{
    public int AllotmentLetterId { get; set; }
    public string AllotmentNo { get; set; }
    public string UnitName { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ValidUntil { get; set; }
    public string MemoNo { get; set; }
    public string Purpose { get; set; }
    public bool AuthorizationValid { get; set; }
    public List<AllotmentItemViewModel> Items { get; set; } = new();
}

public class AllotmentItemViewModel
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal AvailableStock { get; set; }
    public bool IsEligible { get; set; }
    public string Remarks { get; set; }
}
