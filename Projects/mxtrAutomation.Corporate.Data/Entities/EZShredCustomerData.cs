using mxtrAutomation.Data;
using System;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class EZShredCustomerData : Entity
    {
        public string AccountObjectId { get; set; }
        public string MxtrAccountId { get; set; }
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
        public string EZTimeStamp { get; set; }
        public string Operation { get; set; }
        public string UserID { get; set; }
        public string PipelineStatus { get; set; }
        public string NumberOfEmployees { get; set; }
        public string Mobile { get; set; }
        public string Suite { get; set; }        
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
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }   
}
