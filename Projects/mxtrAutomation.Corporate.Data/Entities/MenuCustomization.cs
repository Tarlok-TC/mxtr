using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mxtrAutomation.Data;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class MenuCustomization : Entity
    {
        public string MenuMasterId { get; set; } // For mapping between menumaster and menu
        public string Name { get; set; }
        //public string MenuIdentifier { get; set; }
        //public string PageUrl { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; }
        public string AccountObjectID { get; set; }
        public string LastModifiedBy { get; set; }
        public List<SubMenuCustomization> SubMenu { get; set; }
    }

    public class SubMenuCustomization
    {
        public string Id { get; set; }
        public string SubMenuMasterId { get; set; } // For mapping between submenumaster and submenu
        public string Name { get; set; }
        //public string MenuIdentifier { get; set; }
        //public string PageUrl { get; set; }
        public string Icon { get; set; }
        public int SortOrder { get; set; } 
        public List<SubMenuCustomization> SubMenus { get; set; }
    }
}
