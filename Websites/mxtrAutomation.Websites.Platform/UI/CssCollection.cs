using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.UI
{
    public class CssCollection : ClientResourceCollection<CssKindBase, CssCategoryKind>
    {
        public CssCollection()
        {
            // SITE WIDE CSS

            //Add(CssKind.Admin, new CssResource("~/css/p-admin.css", CssCategoryKind.Page));

            Add(CssKind.Base, new CssResource("~/css/base.css", CssCategoryKind.Base));
            Add(CssKind.Bootstrap, new CssResource("~/css/bootstrap.min.css", CssCategoryKind.Base));
            Add(CssKind.BootstrapTheme, new CssResource("~/css/bootstrap-theme.min.css", CssCategoryKind.Base));
            Add(CssKind.MainLayout, new CssResource("~/css/main-layout.css", CssCategoryKind.Base));
            Add(CssKind.ModuleForms, new CssResource("~/css/m-forms.css", CssCategoryKind.Base));
            Add(CssKind.BaseButtons, new CssResource("~/css/base-buttons.css", CssCategoryKind.Base));
            Add(CssKind.Navigation, new CssResource("~/css/navigation.css", CssCategoryKind.Base));
            Add(CssKind.ModuleReportFilters, new CssResource("~/css/m-report-filters.css", CssCategoryKind.Module));
            Add(CssKind.ModuleScoreBoxes, new CssResource("~/css/m-score-boxes.css", CssCategoryKind.Module));
            Add(CssKind.States, new CssResource("~/css/states.css", CssCategoryKind.State));
            Add(CssKind.Gentelella, new CssResource("~/css/gentelella.css", CssCategoryKind.Base));

            Add(CssKind.Select2, new CssResource("~/css/select2.min.css", CssCategoryKind.Module));
            Add(CssKind.Animate, new CssResource("~/css/animate.min.css", CssCategoryKind.Module));
            
            Add(CssKind.JQueryChosen, new CssResource("~/css/jquery-chosen.css", CssCategoryKind.Module));
            Add(CssKind.iCheckFlat, new CssResource("~/css/flat.css", CssCategoryKind.Module));
            Add(CssKind.Switchery, new CssResource("~/css/switchery.min.css", CssCategoryKind.Module));
            Add(CssKind.Treeview, new CssResource("~/css/m-treeview.css", CssCategoryKind.Module));
            Add(CssKind.JQueryDataTables, new CssResource("~/css/jquery.dataTables.min.css", CssCategoryKind.Module));

            //Add(CssKind.JQueryColorbox, new CssResource("~/css/jquery-colorbox.css", CssCategoryKind.Module));
            //Add(CssKind.BootstrapDateTimePicker, new CssResource("~/css/bootstrap-datetimepicker.min.css", CssCategoryKind.Module));
            Add(CssKind.pnotify, new CssResource("~/css/pnotify.css", CssCategoryKind.Base));
            Add(CssKind.pnotifyButtons, new CssResource("~/css/pnotify.buttons.css", CssCategoryKind.Base));
            Add(CssKind.pnotifyNonblocks, new CssResource("~/css/pnotify.nonblock.css", CssCategoryKind.Base));
            Add(CssKind.publicLayout, new CssResource("~/css/publicLayout.css", CssCategoryKind.Base));
            Add(CssKind.Custom, new CssResource("~/css/custom.css", CssCategoryKind.Base));
            Add(CssKind.Ladda, new CssResource("~/css/ladda.min.css", CssCategoryKind.Base));
        }
    }
}