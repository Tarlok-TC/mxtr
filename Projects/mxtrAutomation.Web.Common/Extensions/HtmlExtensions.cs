using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Optimization;
using System.Web.Routing;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Ioc;
using mxtrAutomation.Web.Common.Helpers;
using mxtrAutomation.Web.Common.UI;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class HtmlExtensions
    {
        private static readonly Dictionary<string, string> StateList =
            new Dictionary<string, string>
                    {
                        {"", "Choose a State"},
                        {"Other", "Other"},
                        {"AL", " Alabama"},
                        {"AK", " Alaska"},
                        {"AB", " Alberta"},
                        {"AZ", " Arizona"},
                        {"AR", " Arkansas"},
                        {"BC", " British Columbia"},
                        {"CA", " California"},
                        {"CO", " Colorado"},
                        {"CT", " Connecticut"},
                        {"DE", " Delaware"},
                        {"FL", " Florida"},
                        {"GA", " Georgia"},
                        {"HI", " Hawaii"},
                        {"ID", " Idaho"},
                        {"IL", " Illinois"},
                        {"IN", " Indiana"},
                        {"IA", " Iowa"},
                        {"KS", " Kansas"},
                        {"KY", " Kentucky"},
                        {"LA", " Louisiana"},
                        {"ME", " Maine"},
                        {"MB", " Manitoba"},
                        {"MD", " Maryland"},
                        {"MA", " Massachusetts"},
                        {"MI", " Michigan"},
                        {"MN", " Minnesota"},
                        {"MS", " Mississippi"},
                        {"MO", " Missouri"},
                        {"MT", " Montana"},
                        {"NE", " Nebraska"},
                        {"NV", " Nevada"},
                        {"NB", " New Brunswick"},
                        {"NL", " Newfoundland and Labrador"},
                        {"NH", " New Hampshire"},
                        {"NJ", " New Jersey"},
                        {"NM", " New Mexico"},
                        {"NY", " New York"},
                        {"NC", " North Carolina"},
                        {"ND", " North Dakota"},
                        {"NS", " Nova Scotia"},
                        {"OH", " Ohio"},
                        {"OK", " Oklahoma"},
                        {"ON", " Ontario"},
                        {"OR", " Oregon"},
                        {"PA", " Pennsylvania"},
                        {"PE", " Prince Edward Island"},
                        {"QC", " Quebec"},
                        {"RI", " Rhode Island"},
                        {"SK", " Saskatchewan"},
                        {"SC", " South Carolina"},
                        {"SD", " South Dakota"},
                        {"TN", " Tennessee"},
                        {"TX", " Texas"},
                        {"UT", " Utah"},
                        {"VT", " Vermont"},
                        {"VA", " Virginia"},
                        {"WA", " Washington"},
                        {"WV", " West Virginia"},
                        {"WI", " Wisconsin"},
                        {"WY", " Wyoming"},
                        {"AS", " American Samoa"},
                        {"DC", " District of Columbia"},
                        {"FM", " Federated States of Micronesia"},
                        {"MH", " Marshall Islands"},
                        {"MP", " Northern Mariana Islands"},
                        {"PW", " Palau"},
                        {"PR", " Puerto Rico"},
                        {"VI", " Virgin Islands"},
                        {"GU", " Guam"}
                    };

        private static readonly Dictionary<string, string> CountryList =
            new Dictionary<string, string>
                    {
                        {"", "Choose Country"},
                        {"BRA", " Brazil"},
                        {"CAN", " Canada"},
                        {"FRA", " France"},
                        {"DEU", " Germany"},
                        {"GBR", " United Kingdom"},
                        {"USA", " United States"}
                    };

        public static void IncludeCss<T>(this HtmlHelper htmlHelper, T resourceKind, BundleKind bundleKind = BundleKind.Page)
            where T : CssKindBase
        {
            IncludeResource<CssKindBase>(htmlHelper, resourceKind, bundleKind);
        }

        public static void IncludeJS<T>(this HtmlHelper htmlHelper, T resourceKind, BundleKind bundleKind = BundleKind.Page)
            where T : JSKindBase
        {
            IncludeResource<JSKindBase>(htmlHelper, resourceKind, bundleKind);
        }

        public static void IncludeResource<T>(this HtmlHelper htmlHelper, T resourceKind, BundleKind bundleKind = BundleKind.Page)
        {
            List<IncludedResource<T>> resourceList =
                htmlHelper.ViewContext.HttpContext.Items[typeof(T)] as List<IncludedResource<T>>;

            IncludedResource<T> includedResource = new IncludedResource<T>(resourceKind, bundleKind);

            if (resourceList != null && !resourceList.Contains(includedResource))
                resourceList.Add(includedResource);
            else
                htmlHelper.ViewContext.HttpContext.Items[typeof(T)] =
                    new List<IncludedResource<T>> { includedResource };
        }

        public static IHtmlString RenderCssBundle<T>(this HtmlHelper htmlHelper)
        {
            IClientResourceBundleManager bundleManager =
                ServiceLocator.Current.TryGetInstance<IClientResourceBundleManager>();

            List<IncludedResource<T>> resources =
                htmlHelper.ViewContext.HttpContext.Items[typeof(T)] as List<IncludedResource<T>>;

            if (bundleManager == null || resources == null)
                return new HtmlString(string.Empty);

            string[] bundles = bundleManager.CreateCssBundles<T, CssCategoryKind>(resources).ToArray();

            return Styles.Render(bundles);
        }

        public static IHtmlString RenderJSBundle<T>(this HtmlHelper htmlHelper)
        {
            IClientResourceBundleManager bundleManager =
                ServiceLocator.Current.TryGetInstance<IClientResourceBundleManager>();

            List<IncludedResource<T>> resources =
                htmlHelper.ViewContext.HttpContext.Items[typeof(T)] as List<IncludedResource<T>>;

            if (bundleManager == null || resources == null)
                return new HtmlString(string.Empty);

            string[] bundles = bundleManager.CreateJSBundles<T, JSCategoryKind>(resources).ToArray();

            return Scripts.Render(bundles);
        }

        public static IHtmlString Partial(this HtmlHelper htmlHelper, ViewKindBase viewKind, ViewModelBase model)
        {
            IViewCollection viewCollection = ServiceLocator.Current.GetInstance<IViewCollection>();

            return htmlHelper.Partial(viewCollection[viewKind].Value, model);
        }

        public static IHtmlString Partial(this HtmlHelper htmlHelper, ViewKindBase viewKind)
        {
            IViewCollection viewCollection = ServiceLocator.Current.GetInstance<IViewCollection>();

            return htmlHelper.Partial(viewCollection[viewKind].Value);
        }

        public static IHtmlString PageLink(this HtmlHelper htmlHelper, ViewLink viewLink)
        {
            return new HtmlString(new PageLink(viewLink).ToString());
        }

        public static MvcHtmlString StateDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            RouteValueDictionary attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            string name = (expression.Body as MemberExpression).Member.Name;
            TValue value = expression.Compile()(html.ViewData.Model);

            return html.DropDownList(name, new SelectList(StateList, "key", "value", value), attrs);
        }

        public static MvcHtmlString CountryDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            RouteValueDictionary attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            string name = (expression.Body as MemberExpression).Member.Name;
            TValue value = expression.Compile()(html.ViewData.Model);

            return html.DropDownList(name, new SelectList(CountryList, "key", "value", value), attrs);
        }

        public static MvcHtmlString TimeZoneDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            Dictionary<string, string> timeZoneList =
                new Dictionary<string, string>
                    {
                        {"", ""},
                        {"Hawaiian Standard Time", " Hawaiian Standard Time"},
                        {"Alaskan Standard Time", " Alaskan Standard Time"},
                        {"Pacific Standard Time", " Pacific Standard Time"},
                        {"Mountain Standard Time", " Mountain Standard Time"},
                        {"Central Standard Time", " Central Standard Time"},
                        {"Eastern Standard Time", " Eastern Standard Time"},
                        {"Atlantic Standard Time", " Atlantic Standard Time"},
                        {"SA Eastern Standard Time", " SA Eastern Standard Time"},
                        {"Mid-Atlantic Standard Time", " Mid-Atlantic Standard Time"},
                        {"GMT Standard Time", " GMT Standard Time"},
                        {"W. Europe Standard Time", " W. Europe Standard Time"},
                        {"E. Europe Standard Time", " E. Europe Standard Time"},
                        {"Arabic Standard Time", " Arabic Standard Time"},
                        {"Arabian Standard Time", " Arabian Standard Time"}
                    };

            RouteValueDictionary attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            string name = (expression.Body as MemberExpression).Member.Name;
            TValue value = expression.Compile()(html.ViewData.Model);

            return html.DropDownList(name, new SelectList(timeZoneList, "key", "value", value), attrs);
        }

        public static void AddIDAndNameAttributes<TModel, TValue>(RouteValueDictionary attrs, Expression<Func<TModel, TValue>> expression)
        {
            string name = (expression.Body as MemberExpression).Member.Name;

            if (!attrs.ContainsKey("id"))
                attrs.Add("id", name);

            if (!attrs.ContainsKey("name"))
                attrs.Add("name", name);
        }

        public static IHtmlString SimpleTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string cssClass)
        {
            string name = (expression.Body as MemberExpression).Member.Name;
            TValue value = expression.Compile()(html.ViewData.Model);

#if true
            TagBuilder tag = new TagBuilder("input");

            tag.GenerateId(name);
            tag.MergeAttribute("type", "text");
            tag.MergeAttribute("name", name);
            tag.MergeAttribute("value", value.Coalesce(x => x.ToString()));
            tag.AddCssClass(cssClass);

            return new MvcHtmlString(tag.ToString(TagRenderMode.SelfClosing));
#endif

#if false
            IDictionary<string,string> attrs =
                new Dictionary<string, string> { { "class", cssClass }};

            return html.TextBox(name, value, attrs);
#endif
        }

        public static IHtmlString SimpleLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string text, string cssClass)
        {
            string name = (expression.Body as MemberExpression).Member.Name;

            TagBuilder tag =
                new TagBuilder("label")
                    .Attribute("for", name)
                    .Attribute("class", cssClass)
                    .InnerHtml(text);

            return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
        }

        public static IHtmlString NavList(this HtmlHelper htmlHelper, IEnumerable<NavViewData> navList)
        {
            return NavList(htmlHelper, navList, string.Empty);
        }

        public static IHtmlString NavList(this HtmlHelper htmlHelper, IEnumerable<NavViewData> navList, string cssClass)
        {
            TagBuilder ul = SubNavList(navList, 0);

            return new MvcHtmlString(ul.Attribute("class", cssClass).ToString(TagRenderMode.Normal));
        }

        public static TagBuilder SubNavList(IEnumerable<NavViewData> navList, int level)
        {
            if (navList == null)
                return null;

            string subs = Enumerable.Repeat("-sub", level).ToString(string.Empty);

            IEnumerable<TagBuilder> listItems =
                navList
                    .Where(navItem => navItem != null)
                    .Select(navItem =>
                            new TagBuilder("li")
                                .Attribute("class", "nav-list" + subs + "-item")
                                .Attribute("class", navItem.Link.CssClass)
                                .Attribute("class", navItem.IsSelected ? "selected" : null)
                                .Attribute("class", NavItemIsParent(navItem) ? "nav" + subs + "-item-parent" : null)
                                .Attribute("class", NavItemChildIsSelected(navItem) ? "selected-child" : null)
                                .InnerHtml(new PageLink(navItem.Link).ToString(false, false))
                                .InnerHtml(SubNavList(navItem.SubLinks, level + 1).Coalesce(x => x.ToString())));

            return
                new TagBuilder("ul")
                    .Attribute("class", "nav" + subs + "-list")
                    .InnerHtml(listItems.ToString(string.Empty));
        }

        public static bool NavItemChildIsSelected(NavViewData navItem)
        {
            if (navItem == null || navItem.SubLinks == null)
                return false;

            return navItem.SubLinks.Any(x => x.IsSelected || NavItemChildIsSelected(x));
        }

        public static bool NavItemIsParent(NavViewData navItem)
        {
            if (navItem == null)
                return false;

            return !navItem.SubLinks.IsNullOrEmpty();
        }

        public static ScriptTag BeginAngularTemplate(this HtmlHelper helper, string id)
        {
            return new ScriptTag(helper, "text/ng-template", id);
        }
    }

#if false
    [TestFixture]
    public class NavListTests
    {
        [Test]
        public void Test()
        {
            NavViewData[] xx =
                new[]
                    {
                        new NavViewData
                            {
                                Link = new ViewLink {Text = "Link 1", Url = "/Test1"},
                                SubLinks = new []
                                               {
                                                   new NavViewData { Link = new ViewLink {Text = "Sub Link 1", Url = "/SubTest1"}, IsSelected = true},
                                                   new NavViewData { Link = new ViewLink {Text = "Sub Link 2", Url = "/SubTest2"}},
                                               }
                            },
                        new NavViewData {Link = new ViewLink {Text = "Link 2", Url = "/Test2"}},
                        new NavViewData {Link = new ViewLink {Text = "Link 3", Url = "/Test3"}},
                    };

            var html = HtmlExtensions.NavList(null, xx, "css");
            var yy = html.ToHtmlString();
            yy = yy + string.Empty;
        }
    }
#endif

#if false
    public static class ResourceType
    {
        public const string Css = "css";
        public const string Js = "js";
    }

    public static class HtmlExtensions
    {
        public static IHtmlString Resource(this HtmlHelper htmlHelper, Func<object, dynamic> template, string type)
        {
            if (htmlHelper.ViewContext.HttpContext.Items[type] != null)
                ((List<Func<object, dynamic>>)htmlHelper.ViewContext.HttpContext.Items[type]).Add(template);
            else
                htmlHelper.ViewContext.HttpContext.Items[type] = new List<Func<object, dynamic>> { template };

            return new HtmlString(String.Empty);
        }

        public static IHtmlString RenderResources(this HtmlHelper htmlHelper, string type)
        {
            if (htmlHelper.ViewContext.HttpContext.Items[type] != null)
            {
                List<Func<object, dynamic>> resources =
                    (List<Func<object, dynamic>>)htmlHelper.ViewContext.HttpContext.Items[type];

                foreach (Func<object, dynamic> resource in resources.Where(resource => resource != null))
                    htmlHelper.ViewContext.Writer.Write(resource(null));
            }

            return new HtmlString(String.Empty);
        }

        public static Func<object, dynamic> ScriptTag(this HtmlHelper htmlHelper, string url)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            TagBuilder script = new TagBuilder("script");
            script.Attributes["type"] = "text/javascript";
            script.Attributes["src"] = urlHelper.Content("~/" + url);
            return x => new HtmlString(script.ToString(TagRenderMode.Normal));
        }
    }
#endif
}
