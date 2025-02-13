using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.UI
{
    public class JSKind : CommonJSKind
    {
        public static readonly JSKind Modernizr = new JSKind("Modernizr");
        public static readonly JSKind Bootstrap = new JSKind("Bootstrap");
        public static readonly JSKind BootstrapProgress = new JSKind("BootstrapProgress");
        public static readonly JSKind Angular = new JSKind("Angular");
        public static readonly JSKind Gentelella = new JSKind("Gentelella");
        public static readonly JSKind Select2 = new JSKind("Select2");
        public static readonly JSKind iCheck = new JSKind("iCheck");
        public static readonly JSKind Switchery = new JSKind("Switchery");
        public static readonly JSKind CDDatepicker = new JSKind("CDDatepicker");
        public static readonly JSKind ChartJS = new JSKind("ChartJS");
        public static readonly JSKind NProgress = new JSKind("NProgress");
        public static readonly JSKind PNotify = new JSKind("PNotify");
        public static readonly JSKind pnotifyButtons = new JSKind("pnotifyButtons");
        public static readonly JSKind pnotifyNonblock = new JSKind("pnotifyNonblock");

        // JQUERY PLUGINS
        public static readonly JSKind JQuery = new JSKind("JQuery");
        public static readonly JSKind JQueryChosenPlugin = new JSKind("JQueryChosenPlugin");
        //public static readonly JSKind JQueryBootgridPlugin = new JSKind("JQueryBootgridPlugin");
        //public static readonly JSKind JQueryBootstrapDateTimePickerPlugin = new JSKind("JQueryBootstrapDateTimePickerPlugin");
        public static readonly JSKind MomentJS = new JSKind("MomentJS");
        public static readonly JSKind JQueryCalendarPlugin = new JSKind("JQueryCalendarPlugin");
        //public static readonly JSKind JQueryColorbox = new JSKind("JQueryColorbox");
        public static readonly JSKind JQueryDataTables = new JSKind("JQueryDataTables");
        
        public static readonly JSKind JQueryValidationPlugin = new JSKind("JQueryValidationPlugin");
        public static readonly JSKind JQueryValidationDefaults = new JSKind("JQueryValidationDefaults");
        public static readonly JSKind JQuerySmartWizard = new JSKind("JQuerySmartWizard");
        public static readonly JSKind Gauge = new JSKind("Gauge");

        // MODULES
        public static readonly JSKind Global = new JSKind("Global");
        public static readonly JSKind AngularTreeview = new JSKind("AngularTreeview");

        // SelectDropdown
        public static readonly JSKind AngularAnimate = new JSKind("AngularAnimate");
        public static readonly JSKind AngularSanitize = new JSKind("AngularSanitize");
        public static readonly JSKind AngularStrap = new JSKind("AngularStrap");
        public static readonly JSKind AngularStraptpl = new JSKind("AngularStraptpl");
        public static readonly JSKind AngularStrapDocstpl = new JSKind("AngularStrapDocstpl");

        // PAGES
        public static readonly JSKind AccountManagement = new JSKind("AccountManagement");
        public static readonly JSKind Login = new JSKind("Login");
        public static readonly JSKind Logout = new JSKind("Logout");
        public static readonly JSKind AccountHierarchy = new JSKind("AccountHierarchy");
        public static readonly JSKind AngularUnique = new JSKind("AngularUnique");
        public static readonly JSKind WorkspaceFilter = new JSKind("WorkspaceFilter");
        public static readonly JSKind Dashboard = new JSKind("Dashboard");
        public static readonly JSKind Leads = new JSKind("Leads");
        public static readonly JSKind RetailerActivityReport = new JSKind("RetailerActivityReport");
        public static readonly JSKind EmailOverview = new JSKind("EmailOverview");
        public static readonly JSKind Retailer = new JSKind("Retailer");
        public static readonly JSKind Lead = new JSKind("Lead");
        public static readonly JSKind IndexActivityReport = new JSKind("IndexActivityReport");
        public static readonly JSKind ManageMenu = new JSKind("ManageMenu");
        public static readonly JSKind DealerPerformanceDetail = new JSKind("DealerPerformanceDetail");

        //KLIPFOLIO 
        public static readonly JSKind KlipFolio = new JSKind("KlipFolio");
        public JSKind(string value) : base(value) { }
    }
}