using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.EZShred
{
    public class CustomerData
    {
        public string Company { get; set; }
        public string Attention { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string CustomerTypeID { get; set; }
        public string InvoiceTypeID { get; set; }
        public string EmailAddress { get; set; }
        public string EmailInvoice { get; set; }
        public string EmailCOD { get; set; }
        public string ReferralsourceID { get; set; }
        public string TermID { get; set; }
        public string Notes { get; set; }
        public string CustomerId { get; set; }
    }

    public class CustomerDataModel
    {
        public string Request { get; set; }
        public string UserId { get; set; }
        public List<CustomerData> CustomerData { get; set; }
    }

    public class AddUpdateCustomerResponse
    {
        public bool Success { get; set; }
        public string FailureInformation { get; set; }
        public string Result { get; set; }
    }

    public class AddUpdateCustomerResult
    {
        public string Request { get; set; }
        public string status { get; set; }
        public string CustomerID { get; set; }
    }

    //----------- Building ------------------

    public class BuildingData
    {
        public string BuildingTypeID { get; set; }
        public string CustomerID { get; set; }
        public string BuildingID { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string SalesmanID { get; set; }
        public string Directions { get; set; }
        public string RoutineInstructions { get; set; }
        public string SiteContact1 { get; set; }
        public string SiteContact2 { get; set; }
        public string RouteID { get; set; }
        public string Stop { get; set; }
        public string ScheduleFrequency { get; set; }
        public string ServiceTypeID { get; set; }
        public string SalesTaxRegionID { get; set; }
        public string TimeTaken { get; set; }
    }

    public class BuildingDataModel
    {
        public string Request { get; set; }
        public string UserId { get; set; }
        public List<BuildingData> BuildingData { get; set; }
    }


    public class AddUpdateBuildingResponse
    {
        public bool Success { get; set; }
        public string FailureInformation { get; set; }
        public string Result { get; set; }
    }

    public class AddUpdateBuildingResult
    {
        public string Request { get; set; }
        public string status { get; set; }
        public string BuildingId { get; set; }
    }
    //--------------- Customer Info--------------------
    public class CustomerInfo
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
    }

    public class CustomerInfoResult
    {
        public string Request { get; set; }
        public string status { get; set; }
        public string CustomerID { get; set; }
        public List<CustomerInfo> tblCustomers { get; set; }
    }
    public class EZShredResponse
    {
        public bool Success { get; set; }
        public string FailureInformation { get; set; }
        public string Result { get; set; }
    }

    public class NextServiceDateInfo
    {
        public string Request { get; set; }
        public string status { get; set; }
        public string NextServiceDate { get; set; }
    }
}
