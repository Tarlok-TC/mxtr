using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.UI
{
    public class CssKind : CommonCssKind
    {
        // SITE WIDE CSS

        public static readonly CssKind Bootstrap = new CssKind("Bootstrap");
        public static readonly CssKind BootstrapTheme = new CssKind("BootstrapTheme");
        public static readonly CssKind MainLayout = new CssKind("MainLayout");
        public static readonly CssKind ModuleForms = new CssKind("ModuleForms");
        public static readonly CssKind Base = new CssKind("Base");
        public static readonly CssKind BaseButtons = new CssKind("BaseButtons");
        public static readonly CssKind ToolsLayout = new CssKind("ToolsLayout");
        public static readonly CssKind Navigation = new CssKind("Navigation");
        public static readonly CssKind ModuleReportFilters = new CssKind("ModuleReportFilters");
        public static readonly CssKind ModuleScoreBoxes = new CssKind("ModuleScoreBoxes");
        public static readonly CssKind States = new CssKind("States");
        public static readonly CssKind Gentelella = new CssKind("Gentelella");
        public static readonly CssKind Select2 = new CssKind("Select2");
        public static readonly CssKind Animate = new CssKind("Animate");
        public static readonly CssKind iCheckFlat = new CssKind("iCheckFlat");
        public static readonly CssKind Switchery = new CssKind("Switchery");

        public static readonly CssKind JQueryChosen = new CssKind("JQueryChosen");
        public static readonly CssKind Treeview = new CssKind("Treeview");
        public static readonly CssKind JQueryDataTables = new CssKind("JQueryDataTables");
        //public static readonly CssKind BootstrapDateTimePicker = new CssKind("BootstrapDateTimePicker");
        //public static readonly CssKind JQueryColorbox = new CssKind("JQueryColorbox");


        //public static readonly CssKind Admin = new CssKind("Admin");

        public static readonly CssKind pnotify = new CssKind("pnotify");
        public static readonly CssKind pnotifyNonblocks = new CssKind("pnotifyNonblock");
        public static readonly CssKind pnotifyButtons = new CssKind("pnotifyButtons");
        public static readonly CssKind publicLayout = new CssKind("publicLayout");
        public static readonly CssKind Custom = new CssKind("custom");
        public static readonly CssKind Ladda = new CssKind("ladda");

        public CssKind(string value) : base(value) {}

    }
 }