using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class EZShredLeadMappingDataModel
    {
        public string Id { get; set; }
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string UserID { get; set; }
        public string MxtrUserID { get; set; }
        public long LeadID { get; set; }
        public long OpportunityID { get; set; }
        public int? CustomerID { get; set; }
        public string Company { get; set; }
        public string Street { get; set; }
        public string ZIP { get; set; }
        public string EZShredApiRequest { get; set; }
        public string EZShredBuildingApiRequest { get; set; }
        public LeadBuildingDataModel Building1 { get; set; }
        public LeadBuildingDataModel Building2 { get; set; }
        public LeadBuildingDataModel Building3 { get; set; }
        public LeadBuildingDataModel Building4 { get; set; }
        public LeadBuildingDataModel Building5 { get; set; }
        public string BuildingStage { get; set; }
        public string EZShredActionType { get; set; }
        public string EZShredStatus { get; set; }
    }
    public class LeadBuildingDataModel
    {
        public int? BuildingID { get; set; }
        public string BuildingCompanyName { get; set; }
        public long OpportunityID { get; set; }
        public string EZShredBuildingApiRequest { get; set; }
        public string EZShredActionType { get; set; }
        public string EZShredStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }
        public string Country { get; set; }
    }
}
