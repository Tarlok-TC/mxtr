using System.Web;
using System.Web.Optimization;

namespace mxtrAutomation.Websites.Platform.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                          "~/Scripts/jquery.min.js",
                          "~/Scripts/gauge.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/flot").Include(
                "~/Scripts/echarts.min.js",
                "~/Scripts/DateJS/build/date.js",
                "~/Scripts/Flot/jquery.flot.js",
                "~/Scripts/Flot/jquery.flot.pie.js",
                "~/Scripts/Flot/jquery.flot.time.js",
                "~/Scripts/Flot/jquery.flot.stack.js",
                "~/Scripts/Flot/jquery.flot.resize.js",
                "~/Scripts/flot.orderbars/js/jquery.flot.orderBars.js",
                "~/Scripts/flot-spline/js/jquery.flot.spline.min.js",
                "~/Scripts/flot.curvedlines/curvedLines.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jvectormap").Include(
                 "~/Scripts/jquery-jvectormap-2.0.3.min.js",
                 "~/Scripts/jquery-jvectormap-us-aea-en.js",
                 "~/Scripts/jquery-jvectormap-world-mill-en.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/vmap").Include(
                "~/Scripts/jquery.vmap.min.js",
                "~/Scripts/jquery.vmap.world.js",
                "~/Scripts/jquery.vmap.usa.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/pnotify").Include(
                 "~/Scripts/pnotify.js",
                 "~/Scripts/pnotify.buttons.js",
                 "~/Scripts/pnotify.nonblock.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/ladda").Include(
                 "~/Scripts/ladda/spin.min.js",
                 "~/Scripts/ladda/ladda.min.js"
                ));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}