using System;

namespace IMS.Application.DTOs
{
    public class PersonnelItemIssueDto
    {
        public int Id { get; set; }
        public string IssueNo { get; set; }

        // Personnel Information
        public string PersonnelId { get; set; }
        public string PersonnelType { get; set; }
        public string PersonnelName { get; set; }
        public string PersonnelBadgeNo { get; set; }
        public string PersonnelUnit { get; set; }
        public string PersonnelDesignation { get; set; }
        public string PersonnelMobile { get; set; }

        // Item Information
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public int? LifeSpanMonths { get; set; }

        // Dates
        public DateTime IssueDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? LifeExpiryDate { get; set; }
        public DateTime? AlertDate { get; set; }
        public int? RemainingDays { get; set; }

        // Status
        public string Status { get; set; }
        public DateTime? ReplacedDate { get; set; }
        public string ReplacementReason { get; set; }

        // Location
        public int? BattalionId { get; set; }
        public string BattalionName { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }

        // Alert Info
        public bool IsAlertSent { get; set; }
        public DateTime? LastAlertDate { get; set; }
        public int AlertCount { get; set; }

        // For UI Display
        public string StatusClass => Status switch
        {
            "Active" when RemainingDays <= 0 => "danger",
            "Active" when RemainingDays <= 7 => "warning",
            "Active" when RemainingDays <= 30 => "info",
            "Expired" => "danger",
            "Replaced" => "secondary",
            _ => "success"
        };

        public string Remarks { get; set; }
    }
}