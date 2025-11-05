namespace IMS.Web.Attributes
{
    // Add to Domain/Constants folder
    public static class ApprovalStatusConstants
    {
        // Central Store Purchase Workflow
        public const string PendingGRN = "Pending GRN";
        public const string PendingInspection = "Pending Inspection";
        public const string PendingDDGApproval = "Pending DDG Approval";
        public const string ApprovedForTransfer = "Approved for Transfer";
        public const string TransferredToProvision = "Transferred to Provision";

        // Provision Store Workflow
        public const string PendingDDProvisionApproval = "Pending DD Provision";
        public const string AllotmentApproved = "Allotment Approved";
        public const string PendingIssueApproval = "Pending Issue Approval";
        public const string Issued = "Issued";

        // Common Status
        public const string Draft = "Draft";
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";

        // Approval Levels
        public const string Level1_Storekeeper = "Level 1 - Storekeeper";
        public const string Level2_ADStore = "Level 2 - AD/DD Store";
        public const string Level3_DDGAdmin = "Level 3 - DDG Admin";
        public const string Level1_ProvisionStorekeeper = "Level 1 - Provision Storekeeper";
        public const string Level2_DDProvision = "Level 2 - DD Provision";

        // Roles
        public const string RoleStorekeeperCentral = "StorekeeperCentral";
        public const string RoleADStore = "ADStore";
        public const string RoleDDStore = "DDStore";
        public const string RoleDDGAdmin = "DDGAdmin";
        public const string RoleStorekeeperProvision = "StorekeeperProvision";
        public const string RoleDDProvision = "DDProvision";
    }
}
