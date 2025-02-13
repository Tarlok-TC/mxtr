using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Sharpspring
{
    public class SharpspringOpportunityDataModel
    {
        public long OpportunityID { get; set; }
        public long OwnerID { get; set; }
        public long PrimaryLeadID { get; set; }
        public long DealStageID { get; set; }
        public long AccountID { get; set; }
        public long CampaignID { get; set; }
        public string OpportunityName { get; set; }
        public double Probability { get; set; }
        public double Amount { get; set; }
        public bool IsClosed { get; set; }
        public bool IsWon { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CloseDate { get; set; }
        //Custom Fields
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
