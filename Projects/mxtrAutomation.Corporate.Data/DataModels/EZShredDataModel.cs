using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class EZShredDataModel
    {
        public string AccountObjectId { get; set; }
        public string MxtrAccountId { get; set; }
        public List<BuildingTypes> BuildingTypes { get; set; }
        public List<CustomerTypes> CustomerTypes { get; set; }
        public List<Routes> Routes { get; set; }
        public List<Salesmans> Salesman { get; set; }
        public List<SalesTaxRegions> SalesTaxRegions { get; set; }
        public List<ServiceItems> ServiceItems { get; set; }
        public List<ServiceTypes> ServiceTypes { get; set; }
        //public List<Customers> Customers { get; set; }
        //public List<Buildings> Buildings { get; set; }
        public List<EZShredCustomerDataModel> Customer { get; set; }
        public List<EZShredBuildingDataModel> Building { get; set; }
        public List<Frequencys> Frequencys { get; set; }
        public List<ReferralSources> ReferralSources { get; set; }
        public List<TermTypes> TermTypes { get; set; }
        public List<InvoiceTypes> InvoiceTypes { get; set; }
        public List<SSField> SSField { get; set; }
        public List<SSOpportunity> SSOpportunity { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
    public class BuildingTypes
    {
        public string BuildingTypeID { get; set; }
        public string BuildingType { get; set; }
    }

    public class CustomerTypes
    {
        public string CustomerTypeID { get; set; }
        public string CustomerType { get; set; }
    }

    public class ServiceTypes
    {
        public string ServiceTypeID { get; set; }
        public string ServiceType { get; set; }
    }
    public class Routes
    {
        public string RouteID { get; set; }
        public string Route { get; set; }
        public string order { get; set; }
    }
    public class ServiceItems
    {
        public string ServiceItemsID { get; set; }
        public string ServiceItem { get; set; }
    }
    public class SalesTaxRegions
    {
        public string SalesTaxRegionID { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
    }
    public class Salesmans
    {
        public string SalesmanID { get; set; }
        public string Salesman { get; set; }
        public string order { get; set; }
    }
    public class EZShredCustomerDataModel : EZShredCustomerDataModelMini
    {
        public string MxtrAccountId { get; set; }
        public string Attention { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Notes { get; set; }
        public string PurchaseOrder { get; set; }
        public string PurchaseOrderExpire { get; set; }
        public string CustomerTypeID { get; set; }
        public string ReferralSourceID { get; set; }
        public string InvoiceTypeID { get; set; }
        public string EmailAddress { get; set; }
        public string EmailInvoice { get; set; }
        public string EmailCOD { get; set; }
        public string CertificateDestruction { get; set; }
        public string CreditHold { get; set; }
        public string PaidInFull { get; set; }
        public string InvoiceNote { get; set; }
        public string TermID { get; set; }
        public string EZTimestamp { get; set; }
        public string Operation { get; set; }
        public string UserID { get; set; }
        public string PipelineStatus { get; set; }
        public string NumberOfEmployees { get; set; }
        public string Mobile { get; set; }
        public string Suite { get; set; }
        public string BillingContactSameAsMainContact { get; set; }
        public string BillingContact { get; set; }
        public string BillingContactPhone { get; set; }
        public string BillingContactExtension { get; set; }
        public string BillingCountryCode { get; set; }
        public string NumberOfBoxes { get; set; }
        public string ServicesProfessionalType { get; set; }
        public string TravelTourismType { get; set; }
    }
    public class EZShredCustomerDataModelMini
    {
        public string AccountObjectId { get; set; }
        public string CustomerID { get; set; }
        public long LeadID { get; set; }
        public long OpportunityID { get; set; }
        public string Company { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string ARCustomerCode { get; set; }
        public string AllowZeroInvoices { get; set; }
        public string DataSource { get; set; }
        public LeadBuildingDataModel Building1 { get; set; }
        public LeadBuildingDataModel Building2 { get; set; }
        public LeadBuildingDataModel Building3 { get; set; }
        public LeadBuildingDataModel Building4 { get; set; }
        public LeadBuildingDataModel Building5 { get; set; }
    }
    public class EZShredBuildingDataModel
    {
        public string AccountObjectId { get; set; }
        public string MxtrAccountId { get; set; }
        public string BuildingID { get; set; }
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string SiteContact1 { get; set; }
        public string Phone1 { get; set; }
        public string SiteContact2 { get; set; }
        public string Phone2 { get; set; }
        public string SalesmanID { get; set; }
        public string Directions { get; set; }
        public string RoutineInstructions { get; set; }
        public string ScheduleDescription { get; set; }
        public string ScheduleFrequency { get; set; }
        public string ScheduleWeek1 { get; set; }
        public string ScheduleWeek2 { get; set; }
        public string ScheduleWeek3 { get; set; }
        public string ScheduleWeek4 { get; set; }
        public string ScheduleWeek5 { get; set; }
        public string ScheduleDOWsun { get; set; }
        public string ScheduleDOWmon { get; set; }
        public string ScheduleDOWtue { get; set; }
        public string ScheduleDOWwed { get; set; }
        public string ScheduleDOWthu { get; set; }
        public string ScheduleDOWfri { get; set; }
        public string ScheduleDOWsat { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ServiceTypeID { get; set; }
        public string RouteID { get; set; }
        public string Stop { get; set; }
        public string TimeTaken { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Notes { get; set; }
        public string BuildingTypeID { get; set; }
        public string SalesTaxRegionID { get; set; }
        public string LastServiceDate { get; set; }
        public string NextServiceDate { get; set; }
        public string EZTimestamp { get; set; }
        public string Operation { get; set; }
        public string UserID { get; set; }
        public string Suite { get; set; }
        public string CompanyCountryCode { get; set; }
        //only for Data transfer
        public long OpportunityID { get; set; }
        public long LeadID { get; set; }
        public string DataSource { get; set; }
        public string TaxExempt { get; set; }
    }
    public class EZShredBuildingDataModelMini
    {
        public string BuildingID { get; set; }
        public string CompanyName { get; set; }
    }
    public class CustomerSearchResult : EZShredCustomerDataModelMini
    {
        public string BuildingID { get; set; }
        public string BuildingName { get; set; }
    }
    public class Frequencys
    {
        public string ScheduleFrequency { get; set; }
        public string Frequency { get; set; }
    }
    public class ReferralSources
    {
        public string ReferralSourceID { get; set; }
        public string ReferralSource { get; set; }
    }
    public class TermTypes
    {
        public string TermID { get; set; }
        public string Terms { get; set; }
    }
    public class InvoiceTypes
    {
        public string InvoiceTypeID { get; set; }
        public string InvoiceType { get; set; }
    }
    public class RootObject
    {
        public bool Success { get; set; }
        public string FailureInformation { get; set; }
        public string Result { get; set; }
    }
    public class Result
    {
        public string Request { get; set; }
        public string status { get; set; }
        public List<BuildingTypes> tblBuildingType { get; set; }
        public List<CustomerTypes> tblCustomerType { get; set; }
        public List<Routes> tblRoutes { get; set; }
        public List<SalesTaxRegions> tblSalesTaxRegion { get; set; }
        public List<ServiceItems> tblServiceItems { get; set; }
        public List<ServiceTypes> tblServiceTypes { get; set; }
        public List<EZShredCustomerDataModel> tblCustomers { get; set; }
        public List<EZShredBuildingDataModel> tblBuilding { get; set; }
        public List<Frequencys> Frequencies { get; set; }
        public List<ReferralSources> tblReferralSource { get; set; }
        public List<TermTypes> tblTerms { get; set; }
        public List<InvoiceTypes> tblInvoiceType { get; set; }
        public List<Salesmans> tblSalesman { get; set; }
    }
    public class SSField
    {
        public string EZShredFieldName { get; set; }
        public string SSSystemName { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Set { get; set; }
    }
    public class SSOpportunity
    {
        //Oppourtunity Native & Custom Fields
        public double Amount { get; set; }
        public string AdditionalTips { get; set; }
        public string JobQuantitySize { get; set; }
        public string CertificateOfInsurance { get; set; }
        public string HoursOfBusiness { get; set; }
        public string Stairs { get; set; }
        public string ProposedConsoleDeliveryDate { get; set; }
        public string ProposedStartDate { get; set; }
        public string ProposedDateOfService { get; set; }
        public string ECUnits { get; set; }
        public string ECPriceUnit { get; set; }
        public string ECAdditionalPrice { get; set; }
        public string GallonUnits_64 { get; set; }
        public string GallonPriceUnit_64 { get; set; }
        public string GallonAdditionalPrice_64 { get; set; }
        public string GallonUnits_96 { get; set; }
        public string GallonPriceUnit_96 { get; set; }
        public string GallonAdditionalPrice_96 { get; set; }
        public string NumberOfTips { get; set; }
        public string BankerBoxes { get; set; }
        public string FileBoxes { get; set; }
        public string Bags { get; set; }
        public string Cabinets { get; set; }
        public string Skids { get; set; }
        public string HardDrivers { get; set; }
        public string Media { get; set; }
        public string Other { get; set; }
        public string SellerComments { get; set; }
        public string HardDrive_Media_Other_Comment { get; set; }
        public string PriceQuotedForFirstTip { get; set; }
        public string BuildingContactFirstName { get; set; }
        public string BuildingContactLastName { get; set; }
        public string BuildingContactPhone { get; set; }
        public string BuildingName { get; set; }
        public string BuildingStreet { get; set; }
        public string BuildingCity { get; set; }
        public string BuildingState { get; set; }
        public string ZipCode { get; set; }

    }
}
