using System;
using System.Collections.Generic;

namespace IMS.Domain.Entities
{
    public class PersonnelItemIssue
    {
        public int Id { get; set; }
        public string IssueNo { get; set; }

        // Personnel Information
        public string PersonnelId { get; set; }
        public string PersonnelType { get; set; } // "Ansar" or "VDP"
        public string PersonnelName { get; set; }
        public string PersonnelBadgeNo { get; set; }
        public string PersonnelUnit { get; set; }
        public string PersonnelDesignation { get; set; }
        public string PersonnelMobile { get; set; }

        // Item Information
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }

        // Issue/Receive Information
        public int? OriginalIssueId { get; set; }
        public virtual Issue? OriginalIssue { get; set; }

        public int? ReceiveId { get; set; }
        public virtual Receive? Receive { get; set; }

        // Life Tracking
        public DateTime IssueDate { get; set; }
        public DateTime? ReceivedDate { get; set; } // When received at Battalion/VDP
        public DateTime? LifeExpiryDate { get; set; } // Calculated: ReceivedDate + LifeSpanMonths
        public DateTime? AlertDate { get; set; } // ExpiryDate - AlertBeforeDays
        public int? RemainingDays { get; set; } // Days remaining before expiry

        public string Status { get; set; } // "Active", "Expired", "Replaced", "Returned"
        public DateTime? ReplacedDate { get; set; }
        public string ReplacementReason { get; set; }
        public int? ReplacementIssueId { get; set; }

        // Location Information
        public int? BattalionId { get; set; }
        public virtual Battalion? Battalion { get; set; }

        public int? RangeId { get; set; }
        public virtual Range? Range { get; set; }

        public int? ZilaId { get; set; }
        public virtual Zila? Zila { get; set; }

        public int? UpazilaId { get; set; }
        public virtual Upazila? Upazila { get; set; }

        public int? StoreId { get; set; }
        public virtual Store Store { get; set; }

        // Alert Tracking
        public bool IsAlertSent { get; set; } = false;
        public DateTime? LastAlertDate { get; set; }
        public int AlertCount { get; set; } = 0;

        // Audit fields
        public string Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}