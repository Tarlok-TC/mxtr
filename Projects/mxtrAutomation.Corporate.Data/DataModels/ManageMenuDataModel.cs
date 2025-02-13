using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.DataModels
{
    public class ManageMenuDataModel
    {
        public string MenuID { get; set; }
        // For mapping between menumaster and menu
        public string MenuMasterId { get; set; }
        public string Name { get; set; }
        public string MasterMenuName { get; set; }
        public string MenuIdentifier { get; set; }
        public string PageUrl { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        public string AccountObjectID { get; set; }
        public string MenuScope { get; set; }
        public bool Status { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<ManageMenuDataModel> SubMenu { get; set; }
        public List<OrganizationAccount> OrganizationAccounts { get; set; }
    }

    public class OrganizationAccount
    {
        public string AccountObjectID { get; set; }
        public string AccountName { get; set; }
        public bool IsSelected { get; set; }
    }
}