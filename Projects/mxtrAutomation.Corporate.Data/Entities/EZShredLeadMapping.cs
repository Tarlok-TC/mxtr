using mxtrAutomation.Data;
using System;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class EZShredLeadMapping : Entity
    {
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
        /// <summary>
        /// Action Type detail
        /// Create -- If Customer need to be created
        /// Update -- If Customer need to be updated
        /// NoAction  -- If no action is to be taken
        /// </summary>
        public string EZShredActionType { get; set; }
        /// <summary>
        /// Status detail
        /// Failed -- If not successful from Customer Search Page
        /// InProgress --If saving/updating from Customer Search Page
        /// Complete -- If successfully added/updated from Customer Search Page
        /// </summary>
        public string EZShredStatus { get; set; }
        public LeadBuilding Building1 { get; set; }
        public LeadBuilding Building2 { get; set; }
        public LeadBuilding Building3 { get; set; }
        public LeadBuilding Building4 { get; set; }
        public LeadBuilding Building5 { get; set; }
        public string BuildingStage { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
    public class LeadBuilding
    {
        public int? BuildingID { get; set; }
        public string BuildingCompanyName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }
        public string Country { get; set; }
        public long OpportunityID { get; set; }
        public string EZShredBuildingApiRequest { get; set; }
        public string EZShredActionType { get; set; }
        public string EZShredStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
