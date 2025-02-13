using mxtrAutomation.Common.Attributes;

namespace mxtrAutomation.Corporate.Data.Enums
{
    public enum PermissionKind
    {
        [Display("Manage Accounts & Users")]
        ManageAccountUsers,

        [Display("View Hierarchy")]
        ViewHierarchy,

        [Display("Create Dashboard")]
        CreateDashboard,

        [Display("View Dashboard")]
        ViewDashboard,

        [Display("View Analytics")]
        ViewAnalytics,

        [Display("View Sales")]
        ViewSales,
    }
}
