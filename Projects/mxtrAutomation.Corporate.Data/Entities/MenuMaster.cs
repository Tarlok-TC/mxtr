using mxtrAutomation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class MenuMaster : Entity
    {
        public string Name { get; set; }
        public string MenuIdentifier { get; set; }
        public string PageUrl { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        public string MenuScope { get; set; }
        public bool Status { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<SubMenuMaster> SubMenu { get; set; }
    }
    public class SubMenuMaster
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MenuIdentifier { get; set; }
        public string PageUrl { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        public bool Status { get; set; }
        public List<SubMenuMaster> SubMenus { get; set; }
    }

}
