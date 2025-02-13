using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;
using mxtrAutomation.Corporate.Data.DataModels;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class CRMLead : Entity
    {
        public string AccountObjectID { get; set; }
        public string MxtrAccountID { get; set; }
        public string CRMKind { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public long LeadID { get; set; }
        public long AccountID { get; set; }
        public long OwnerID { get; set; }
        public long CampaignID { get; set; }
        public string LeadStatus { get; set; }
        public int LeadScore { get; set; }
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
        /// <summary>
        /// Id of the Cloned Lead in Sharpspring
        /// </summary>
        public long ClonedLeadID { get; set; }
        /// <summary>
        /// Id of the Cloned Lead in Database
        /// </summary>
        public string OriginalLeadID { get; set; }
        public List<CRMLeadEventDataModel> Events { get; set; }
        /// <summary>
        /// To find if lead is copied to dealer or parent
        /// </summary>
        public bool CopiedToParent { get; set; }
        public int LeadScoreWeighted { get; set; }
        public DateTime CreatedOnMXTR { get; set; }
    }
}
