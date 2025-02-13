using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringLeadDataModel
    {
        public long LeadID { get; set; }
        public long AccountID { get; set; }
        public long OwnerID { get; set; }
        public long CampaignID { get; set; }
        public string LeadStatus { get; set; }
        public int LeadScore { get; set; }
        public int LeadScoreWeighted { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Description { get; set; }
        public string Industry { get; set; }
        public bool IsUnsubscribed { get; set; }

        //Customer Custom Fields
        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }

        public string BillingCompanyName { get; set; }
        public string ARCustomerCode { get; set; }
        public string NumberOfEmployees { get; set; }
        public string Attention { get; set; }
        public string ReferralSourceID { get; set; }
        public string Notes { get; set; }
        public string EmailInvoice { get; set; }
        public string CustomerID { get; set; }
        public string PurchaseOrder { get; set; }
        public string PurchaseOrderExpire { get; set; }
        public string CustomerTypeID { get; set; }
        public string InvoiceTypeID { get; set; }
        public string EmailCOD { get; set; }
        public string TermID { get; set; }        
        public string PipelineStatus { get; set; }
        public string Suite { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Datasource { get; set; }
        public string TaxExempt { get; set; }
        public string BillingContactSameAsMainContact { get; set; }
        public string BillingContact { get; set; }
        public string BillingContactPhone { get; set; }
        public string BillingContactExtension { get; set; }
        public string BillingCountryCode { get; set; }
        public string NumberOfBoxes { get; set; }
        public string ServicesProfessionalType { get; set; }
        public string TravelTourismType { get; set; }

        //Building Custom Fields

        public string BuildingCompanyName { get; set; }
        public string BuildingStreet { get; set; }
        public string BuildingCity { get; set; }
        public string BuildingState { get; set; }
        public string BuildingZip { get; set; }
        public string BuildingContactFirstName1 { get; set; }
        public string BuildingContactLastName1 { get; set; }
        public string BuildingContactFirstName2 { get; set; }
        public string BuildingContactLastName2 { get; set; }
        public string BuildingContactPhone1 { get; set; }
        public string BuildingContactPhone2 { get; set; }
        public string SalesmanID { get; set; }
        public string Directions { get; set; }
        public string RoutineInstructions { get; set; }
        public string ScheduleFrequency { get; set; }
        public string ServiceTypeID { get; set; }
        public string RouteID { get; set; }
        public string BuildingTypeID { get; set; }
        public string BuildingSuite { get; set; }
        public string LastServiceDate { get; set; }
        public string NextServiceDate { get; set; }
        public string CompanyCountryCode { get; set; }

        //Opportunity Fields
        public SharpspringOpportunityDataModel SharpspringOpportunityDataModel { get; set; }
    }
    public class SSMapField
    {
        public string EZShredFieldName { get; set; }
        public string SSSystemName { get; set; }
        public string Label { get; set; }
    }
    public class SharpspringCustomFieldsDataModel
    {
        public string SSSystemName { get; set; }
        public string EZShredFieldName { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Set { get; set; }
    }
}
