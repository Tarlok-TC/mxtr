namespace mxtrAutomation.Common.Enums
{
    public enum EZShredDataEnum
    {
        BuildingTypes = 0,
        CustomerTypes = 1,
        Routes = 2,
        Salesmen = 3,
        SalesTaxRegions = 4,
        ServiceItems = 5,
        ServiceTypes = 6,
        CustomerList = 7,
        BuildingList = 8,
        FrequencyTypes = 9,
        ReferralSourceTypes = 10,
        TermsTypes = 11,
        InvoiceTypes = 12,
    }

    public enum EZShredActionTypeKind
    {
        Create = 0,
        Update = 1,
        NoAction = 2,
    }

    public enum EZShredStatusKind
    {
        Failed = 0,
        Complete = 1,
        InProgress = 2,
    }
}
