using System;
using System.Web.Mvc;
using System.Collections.Generic;
using mxtrAutomation.Web.Common.UI;
using mxtrAutomation.Websites.Platform.Enums;

namespace mxtrAutomation.Websites.Platform.Models.Customer.ViewData
{
    public class CustomerViewData//Not Updated!
    {
        /**Customer Properties**/

        public string ARCustomerCode { get; set; }//This is auto generated and will need retrieved from api at some point...
        public long CustomerID { get; set; } // Relates to tblBuilding
        public long CustomerTypeID { get; set; }
        public int ReferralSourceID { get; set; }
        public long TermID { get; set; }
        public string CreditStatus { get; set; }//This will be red if there is a credit hold on the particular customer
        public string Attention { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        public int Zipcode { get; set; }
        public string CompanyName { get; set; }
        public string ContactFName { get; set; }
        public string ContactLName { get; set; }
        public string Email { get; set; }

        public DateTime LastServiceDate { get; set; }
        public DateTime NextServiceDate { get; set; }
        public string ScheduleFrequency { get; set; }
        public bool TaxExempt { get; set; }
        public string Notes { get; set; }
        public int NumberOfEmployees { get; set; }//currently no field in the db for this as it's store in the 'Notes' column.. 
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public bool EmailInvoices { get; set; }

        public List<SelectListItem> ReferralSourceDropDown { get; set; }
        public List<SelectListItem> IndustryTypeDropDown { get; set; }
        public List<SelectListItem> TermsDropDown { get; set; }
        public List<SelectList> InvoiceTypeDropDown { get; set; }

        /**Building Properties**/
        //below are building properties.. not sure if there should be a separate model/controller for this?
        public string BuildingName { get; set; }
        public int BuildingTypeID { get; set; }// we need the miner to work for this!
        public string Directions { get; set; }
        public string RouteInstructions { get; set; }
        public string BuildingStreet { get; set; }
        public string BuildingCity { get; set; }
        public string BuildingState { get; set; }
        public string BuildingZip { get; set; }

        /** Service Properties - Again another model and controller? **/
        public int ServiceTypeID { get; set; }//again, need values from api miner... 
        public string ScheduleFreq { get; set; }



    }
}