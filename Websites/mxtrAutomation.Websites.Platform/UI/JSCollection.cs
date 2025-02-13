using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.UI
{
    public class JSCollection : ClientResourceCollection<JSKindBase, JSCategoryKind>
    {
        public JSCollection()
        {
            Add(JSKind.Modernizr, new JSResource("~/Scripts/modernizr.js"));
            Add(JSKind.Bootstrap, new JSResource("~/Scripts/bootstrap.min.js"));
            Add(JSKind.Angular, new JSResource("~/Scripts/angular.min.js"));
            Add(JSKind.Gentelella, new JSResource("~/Scripts/gentelella.js"));
            Add(JSKind.Select2, new JSResource("~/Scripts/select2.full.js"));
            Add(JSKind.iCheck, new JSResource("~/Scripts/icheck.min.js"));
            Add(JSKind.Switchery, new JSResource("~/Scripts/switchery.min.js"));
            Add(JSKind.ChartJS, new JSResource("~/Scripts/chart.min.js"));
            Add(JSKind.BootstrapProgress, new JSResource("~/Scripts/bootstrap-progressbar.min.js"));

            // PLUGINS
            Add(JSKind.JQuery, new JSResource("~/Scripts/jquery.min.js"));
            Add(JSKind.JQueryChosenPlugin, new JSResource("~/Scripts/jquery.chosen.plugin.js"));
            Add(JSKind.Gauge, new JSResource("~/Scripts/gauge.min.js"));
            //Add(JSKind.JQueryBootgridPlugin, new JSResource("~/Scripts/jquery.bootgrid.min.js"));
            //Add(JSKind.JQueryBootstrapDateTimePickerPlugin, new JSResource("~/Scripts/bootstrap-datetimepicker.min.js"));
            Add(JSKind.MomentJS, new JSResource("~/Scripts/moment.min.js"));
            Add(JSKind.JQueryCalendarPlugin, new JSResource("~/Scripts/jquery.calendar.js", new[] { JSKind.JQueryChosenPlugin, JSKind.MomentJS }));
            Add(JSKind.CDDatepicker, new JSResource("~/Scripts/cd.datepicker.js", new[] { JSKind.JQueryCalendarPlugin, JSKind.JQueryChosenPlugin, JSKind.MomentJS }));
            Add(JSKind.JQueryDataTables, new JSResource("~/Scripts/jquery.dataTables.min.js"));
            //Add(JSKind.JQueryColorbox, new JSResource("~/Scripts/colorbox/jquery.colorbox-min.js"));

            Add(JSKind.JQueryValidationDefaults, new JSResource("~/Scripts/jquery.validation.defaults.js", new[] { JSKind.JQueryValidationPlugin }));
            Add(JSKind.JQueryValidationPlugin, new JSResource("~/Scripts/jquery.validate.min.js"));
            Add(JSKind.JQuerySmartWizard, new JSResource("~/Scripts/jquery.smartWizard.js"));
            Add(JSKind.NProgress, new JSResource("~/Scripts/nprogress.js"));
            Add(JSKind.AngularUnique, new JSResource("~/Scripts/angular.unique.js"));
            Add(JSKind.PNotify, new JSResource("~/Scripts/pnotify.js"));
            Add(JSKind.pnotifyButtons, new JSResource("~/Scripts/pnotify.buttons.js"));
            Add(JSKind.pnotifyNonblock, new JSResource("~/Scripts/pnotify.nonblock.js"));

            // MODULES
            Add(JSKind.Global, new JSResource("~/Scripts/global.js"));
            Add(JSKind.AngularTreeview, new JSResource("~/Scripts/angular.treeview.js"));

             // SelectDropdown
            Add(JSKind.AngularAnimate, new JSResource("~/Scripts/selectdropdown/angular-animate.min.js"));
            Add(JSKind.AngularSanitize, new JSResource("~/Scripts/selectdropdown/angular-sanitize.min.js"));
            Add(JSKind.AngularStrap, new JSResource("~/Scripts/selectdropdown/angular-strap.js"));
            Add(JSKind.AngularStraptpl, new JSResource("~/Scripts/selectdropdown/angular-strap.tpl.js"));
            Add(JSKind.AngularStrapDocstpl, new JSResource("~/Scripts/selectdropdown/angular-strap.docs.tpl.js"));


            // PAGES
            Add(JSKind.AccountManagement, new JSResource("~/Scripts/account.management.js"));
            Add(JSKind.AccountHierarchy, new JSResource("~/Scripts/account.hierarchy.js"));
            Add(JSKind.WorkspaceFilter, new JSResource("~/Scripts/workspace.filter.js"));
            Add(JSKind.Login, new JSResource("~/Scripts/login.js"));
            Add(JSKind.Logout, new JSResource("~/Scripts/logout.js"));
            Add(JSKind.Dashboard, new JSResource("~/Scripts/dashboard.js"));
            Add(JSKind.Leads, new JSResource("~/Scripts/leads.js"));
            Add(JSKind.RetailerActivityReport, new JSResource("~/Scripts/p.retaileractivityreport.js"));
            Add(JSKind.EmailOverview, new JSResource("~/Scripts/p.email.overview.js"));
            Add(JSKind.Retailer, new JSResource("~/Scripts/p.retailer.js"));
            Add(JSKind.Lead, new JSResource("~/Scripts/lead.js"));
            Add(JSKind.IndexActivityReport, new JSResource("~/Scripts/p.indexactivityreport.js"));
            Add(JSKind.ManageMenu, new JSResource("~/Scripts/menu.management.js"));
            Add(JSKind.DealerPerformanceDetail, new JSResource("~/Scripts/Shaw/dealerperformance-detail.js"));
            //KLIPFOLIO
            Add(JSKind.KlipFolio, new JSResource("~/Scripts/klipfolio.custom.js"));
        }
    }
}