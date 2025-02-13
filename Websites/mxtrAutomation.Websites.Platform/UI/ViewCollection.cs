using System;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Websites.Platform.UI
{
    public partial class ViewCollection : ClientResourceCollection<ViewKindBase, ViewCategoryKind>, IViewCollection
    {
        #region Current

        public static IViewCollection Current { get { return CurrentViewCollection.Value; }}
        private static readonly Lazy<IViewCollection> CurrentViewCollection =
            new Lazy<IViewCollection>(() => ServiceLocator.Current.GetInstance<IViewCollection>());

        #endregion

        partial void AddViews();

        public ViewCollection()
        {            
            AddLayout(ViewKind.MainLayout, "~/Views/Shared/MainLayout.cshtml");
            AddLayout(ViewKind.PublicLayout, "~/Views/Shared/PublicLayout.cshtml");

           AddViews();
        }

        #region Protected Add() methods.

        protected void AddPage(ViewKindBase key, string value)
        {
            Add(key, new ViewResource(value, ViewCategoryKind.Page));
        }

        protected void AddPartial(ViewKindBase key, string value)
        {
            Add(key, new ViewResource(value, ViewCategoryKind.Partial));
        }

        protected void AddLayout(ViewKindBase key, string value)
        {
            Add(key, new ViewResource(value, ViewCategoryKind.Layout));
        }

        #endregion
    }
}