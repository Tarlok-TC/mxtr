using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Enums;
using mxtrAutomation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class EZShredData : Entity
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
        public List<Customers> Customers { get; set; }
        public List<Buildings> Buildings { get; set; }
        public List<Frequencys> Frequencys { get; set; }
        public List<ReferralSources> ReferralSources { get; set; }
        public List<TermTypes> TermTypes { get; set; }
        public List<InvoiceTypes> InvoiceTypes { get; set; }
        public List<SSField> SSField { get; set; }
        public CRMKind CRMKind { get; set; }
        public DateTime LastModifiedDate { get; set; }

    }
    public class Customers
    {
        public string CustomerID { get; set; }
        public string Company { get; set; }
        public string Attention { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Notes { get; set; }
        public string PurchaseOrder { get; set; }
        public string PurchaseOrderExpire { get; set; }
        public string ARCustomerCode { get; set; }
        public string CustomerTypeID { get; set; }
        public string ReferralSourceID { get; set; }
        public string InvoiceTypeID { get; set; }
        public string EmailAddress { get; set; }
        public string EmailInvoice { get; set; }
        public string EmailCOD { get; set; }
        public string CertificateDestruction { get; set; }
        public string AllowZeroInvoices { get; set; }
        public string CreditHold { get; set; }
        public string PaidInFull { get; set; }
        public string InvoiceNote { get; set; }
        public string TermID { get; set; }
        public string ezTimestamp { get; set; }
        public string operation { get; set; }
        public string userID { get; set; }

        //Fields that will not get pushed to EZ Shred and will only be in our DB and SS
        public string PipelineStatus { get; set; }
        public string NumberOfEmployees { get; set; }
        public string Mobile { get; set; }
        public string Suite { get; set; }
        public string DataSource { get; set; }
        public long LeadID { get; set; }
        public long OpportunityID { get; set; }
        public string BillingContactSameAsMainContact { get; set; }
        public string BillingContact { get; set; }
        public string BillingContactPhone { get; set; }
        public string BillingContactExtension { get; set; }
        public string BillingCountryCode { get; set; }
        public string NumberOfBoxes { get; set; }
        public string ServicesProfessionalType { get; set; }
        public string TravelTourismType { get; set; }
    }

    public class Buildings
    {
        public string BuildingID { get; set; }
        public string CustomerID { get; set; }
        public long OpportunityID { get; set; }
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
        public string ezTimestamp { get; set; }
        public string operation { get; set; }
        public string userID { get; set; }
        public string TaxExempt { get; set; }
        //Fields that will not get pushed to EZ Shred and will only be in our DB and SS
        public string Suite { get; set; }
        public string CompanyCountryCode { get; set; }
    }
}
