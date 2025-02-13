using mxtrAutomation.Common.Attributes;

namespace mxtrAutomation.Corporate.Data.Enums
{
    public enum CRMKind
    {
        [Display("")]
        None,

        [Display("Sharpspring")]
        Sharpspring,

        [Display("Bullseye")]
        Bullseye,

        [Display("GoogleAnalytics")]
        GoogleAnalytics,

        [Display("EZShred")]
        EZShred,

        [Display("EZShred-CustomerLists")]
        EZShredCustomerLists,

        [Display("EZShred-BuildingLists")]
        EZShredBuildingLists,

        [Display("EZShred-ServiceLists")]
        EZShredServiceLists,

        [Display("EZShred-MiscLists")]
        EZShredMiscLists,

        [Display("ShawKPIMiner")]
        ShawKPIMiner,
    }
}
