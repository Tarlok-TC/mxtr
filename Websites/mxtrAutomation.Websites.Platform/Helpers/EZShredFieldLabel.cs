using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Corporate.Data.DataModels;
using mxtrAutomation.Corporate.Data.Services;
using System.Collections.Generic;
using mxtrAutomation.Api.Sharpspring;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.Helpers
{
    public class EZShredFieldLabel
    {
        public static void CheckForEZShredFieldLabelData()
        {
            IEZShredFieldLabelMappingService _eZShredFieldLabelMappingService = ServiceLocator.Current.GetInstance<IEZShredFieldLabelMappingServiceInternal>();
            if (GetData().Count > _eZShredFieldLabelMappingService.IsEZShredFieldLabelExist())
            {
                _eZShredFieldLabelMappingService.AddFieldLabel(GetData());
            }
        }
        private static List<EZShredFieldLabelMappingDataModel> GetData()
        {
            List<EZShredFieldLabelMappingDataModel> lst = new List<EZShredFieldLabelMappingDataModel>();            
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "EmailInvoice",
                Label = "Email Invoices?",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set=""
            });
            //For Lead Custom Field
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ARCustomerCode",
                Label = "ARCustomerCode",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            //For Opportunity Custom Field
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "OpportunityARCustomerCode",
                Label = "AR Customer Code",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "NumberOfEmployees",
                Label = "Number of Employees",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingStreet",
                Label = "Billing Street",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingCity",
                Label = "Billing City",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingState",
                Label = "Billing State",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingZip",
                Label = "Billing Zip Code",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "CustomerTypeID",
                Label = "Industry Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ReferralSourceID",
                Label = "Referral Source",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Attention",
                Label = "Attention:",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Attention",
                Label = "Attention",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "TermID",
                Label = "Terms",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Email3",
                Label = "Email #3",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Notes",
                Label = "Additional Notes",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Email2",
                Label = "Email #2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "LastServiceDate",
                Label = "Last Service Date",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "NextServiceDate",
                Label = "Next Service Date",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });


            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "InvoiceTypeID",
                Label = "Invoice Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "PipelineStatus",
                Label = "Customer Status (Pipeline)",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingPhone1",
                Label = "Building Phone",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingPhone2",
                Label = "Building Phone 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Datasource",
                Label = "Database Location",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Suite",
                Label = "Billing Suite # / Unit # / Apt. #",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingCompanyName",
                Label = "Billing Building Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });

            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "EmailCOD",
                Label = "Email COD from App",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingContactSameAsMainContact",
                Label = "Billing Contact Same as Main Contact",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingContact",
                Label = "Billing Contact",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "CompanyContact",
                Label = "Company Contact",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingContactPhone",
                Label = "Billing Contact Phone",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingContactExtension",
                Label = "Billing Contact Extension",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "CompanyCountryCode",
                Label = "Company Country",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "NumberOfBoxes",
                Label = "Number of Boxes",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "AdditionalTips",
                Label = "Quote for Additional Tips",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "JobQuantitySize",
                Label = "Job Quantity Size",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ServicesProfessionalType",
                Label = "Services: Professional Sub Menu",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "TravelTourismType",
                Label = "Travel/Tourism: Sub Menu",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "CertificateOfInsurance",
                Label = "Certificate of Insurance",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "HoursOfBusiness",
                Label = "Hours of Business",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Stairs",
                Label = "Stairs",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ProposedConsoleDeliveryDate",
                Label = "Proposed Console Delivery Date",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ProposedStartDate",
                Label = "Proposed Start Date",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ProposedDateOfService",
                Label = "Proposed Date of Service",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ECUnits",
                Label = "EC # of Units",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ECPriceUnit",
                Label = "EC Price 1st Unit",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ECAdditionalPrice",
                Label = "EC Price/Additional",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "GallonUnits_64",
                Label = "64 Gallon # Units",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "GallonPriceUnit_64",
                Label = "64 Gallon Price 1st Unit",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "GallonAdditionalPrice_64",
                Label = "64 Price/Additional",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "GallonUnits_96",
                Label = "96 Gallon # Units",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "GallonPriceUnit_96",
                Label = "96 Gallon Price 1st Unit",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "GallonAdditionalPrice_96",
                Label = "96 Gallon Price/Additional",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "NumberOfTips",
                Label = "Number of Tips",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BankerBoxes",
                Label = "Banker Boxes",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "FileBoxes",
                Label = "File Boxes",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Bags",
                Label = "Bags",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Cabinets",
                Label = "Cabinets",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Skids",
                Label = "Skids",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "HardDrivers",
                Label = "Hard Drives",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Media",
                Label = "Media",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Other",
                Label = "Other",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "SellerComments",
                Label = "Seller Comments",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "HardDrive_Media_Other_Comment",
                Label = "Hard Drive/Media/Other Comments",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "PriceQuotedForFirstTip",
                Label = "Price Quoted for First Tip",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName",
                Label = "Building Contact First Name",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName",
                Label = "Building Contact Last Name",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone",
                Label = "Building Contact Phone",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingName",
                Label = "Building Name",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingStreet",
                Label = "Building Street",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCity",
                Label = "Building City",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingState",
                Label = "Building State",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ZipCode",
                Label = "Zip Code",
                Type = SharpspringCustomFieldKind.Opportunity.ToString(),
                Set = ""
            });
            //Building1 Set
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "SalesmanID",
                Label = "Account Manager",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCity",
                Label = "Building City",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName1",
                Label = "Building Contact First Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName1",
                Label = "Building Contact Last Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName2",
                Label = "Building Contact First Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName2",
                Label = "Building Contact Last Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone1",
                Label = "Building Contact Phone",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone2",
                Label = "Building Contact Phone 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCompanyName",
                Label = "Building Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingState",
                Label = "Building State",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingStreet",
                Label = "Building Street",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingTypeID",
                Label = "Building Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingZip",
                Label = "Building Zip",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingCountryCode",
                Label = "Country Code",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Directions",
                Label = "Directions to location",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "RoutineInstructions",
                Label = "Route Instructions",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ScheduleFrequency",
                Label = "Schedule Frequency",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ServiceTypeID",
                Label = "Service Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingSuite",
                Label = "Suite # / Unit # / Apt. #",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "TaxExempt",
                Label = "Tax Exempt",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building1.ToString()
            });
            //Building2 Set
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "SalesmanID",
                Label = "#2 Account Manager",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCity",
                Label = "#2 Building City",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName1",
                Label = "#2 Building Contact First Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName1",
                Label = "#2 Building Contact Last Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName2",
                Label = "#2 Building Contact First Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName2",
                Label = "#2 Building Contact Last Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone1",
                Label = "#2 Building Contact Phone",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone2",
                Label = "#2 Building Contact Phone 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCompanyName",
                Label = "#2 Building Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingState",
                Label = "#2 Building State",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingStreet",
                Label = "#2 Building Street",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingTypeID",
                Label = "#2 Building Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingZip",
                Label = "#2 Building Zip",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingCountryCode",
                Label = "#2 Country Code",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Directions",
                Label = "#2 Directions to location",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "RoutineInstructions",
                Label = "#2 Route Instructions",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ScheduleFrequency",
                Label = "#2 Schedule Frequency",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ServiceTypeID",
                Label = "#2 Service Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingSuite",
                Label = "#2 Suite # / Unit # / Apt. #",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "TaxExempt",
                Label = "#2 Tax Exempt",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building2.ToString()
            });
            //Building3 Set
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "SalesmanID",
                Label = "#3 Account Manager",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCity",
                Label = "#3 Building City",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName1",
                Label = "#3 Building Contact First Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName1",
                Label = "#3 Building Contact Last Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName2",
                Label = "#3 Building Contact First Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName2",
                Label = "#3 Building Contact Last Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone1",
                Label = "#3 Building Contact Phone",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone2",
                Label = "#3 Building Contact Phone 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCompanyName",
                Label = "#3 Building Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingState",
                Label = "#3 Building State",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingStreet",
                Label = "#3 Building Street",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingTypeID",
                Label = "#3 Building Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingZip",
                Label = "#3 Building Zip",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingCountryCode",
                Label = "#3 Country Code",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Directions",
                Label = "#3 Directions to location",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "RoutineInstructions",
                Label = "#3 Route Instructions",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ScheduleFrequency",
                Label = "#3 Schedule Frequency",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ServiceTypeID",
                Label = "#3 Service Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingSuite",
                Label = "#3 Suite # / Unit # / Apt. #",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "TaxExempt",
                Label = "#3 Tax Exempt",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building3.ToString()
            });
            //Building4 Set
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "SalesmanID",
                Label = "#4 Account Manager",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCity",
                Label = "#4 Building City",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName1",
                Label = "#4 Building Contact First Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName1",
                Label = "#4 Building Contact Last Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName2",
                Label = "#4 Building Contact First Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName2",
                Label = "#4 Building Contact Last Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone1",
                Label = "#4 Building Contact Phone",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone2",
                Label = "#4 Building Contact Phone 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCompanyName",
                Label = "#4 Building Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingState",
                Label = "#4 Building State",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingStreet",
                Label = "#4 Building Street",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingTypeID",
                Label = "#4 Building Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingZip",
                Label = "#4 Building Zip",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingCountryCode",
                Label = "#4 Country Code",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Directions",
                Label = "#4 Directions to location",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "RoutineInstructions",
                Label = "#4 Route Instructions",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ScheduleFrequency",
                Label = "#4 Schedule Frequency",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ServiceTypeID",
                Label = "#4 Service Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingSuite",
                Label = "#4 Suite # / Unit # / Apt. #",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "TaxExempt",
                Label = "#4 Tax Exempt",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building4.ToString()
            });
            //Building5 Set
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "SalesmanID",
                Label = "#5 Account Manager",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCity",
                Label = "#5 Building City",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName1",
                Label = "#5 Building Contact First Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName1",
                Label = "#5 Building Contact Last Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactFirstName2",
                Label = "#5 Building Contact First Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactLastName2",
                Label = "#5 Building Contact Last Name 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone1",
                Label = "#5 Building Contact Phone",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingContactPhone2",
                Label = "#5 Building Contact Phone 2",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingCompanyName",
                Label = "#5 Building Name",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingState",
                Label = "#5 Building State",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingStreet",
                Label = "#5 Building Street",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingTypeID",
                Label = "#5 Building Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingZip",
                Label = "#5 Building Zip",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BillingCountryCode",
                Label = "#5 Country Code",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "Directions",
                Label = "#5 Directions to location",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "RoutineInstructions",
                Label = "#5 Route Instructions",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ScheduleFrequency",
                Label = "#5 Schedule Frequency",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "ServiceTypeID",
                Label = "#5 Service Type",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "BuildingSuite",
                Label = "#5 Suite # / Unit # / Apt. #",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            lst.Add(new EZShredFieldLabelMappingDataModel()
            {
                EZShredFieldName = "TaxExempt",
                Label = "#5 Tax Exempt",
                Type = SharpspringCustomFieldKind.Lead.ToString(),
                Set = LeadBuildingSet.Building5.ToString()
            });
            return lst;
        }
    }
}