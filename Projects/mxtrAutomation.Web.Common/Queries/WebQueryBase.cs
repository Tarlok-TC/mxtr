using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Items;

namespace mxtrAutomation.Web.Common.Queries
{
    public abstract class WebQueryBase : QueryBase
    {
        protected WebQueryBase(WebQueryBase queryToCopyFlagsFrom)
            : this()
        {
            SetFlags(queryToCopyFlagsFrom);
        }

        protected WebQueryBase()
        {
            _debugFlag = Parameters.Add("debug", false, x => x ? "true" : "false", x => x.ToLower() == "true");
        }

        public override string UriTemplate
        {
            get { return UriTemplateCache.GetValue(this); }
        }
        private static readonly LazyTypeSpecificCache<WebQueryBase, Type, string> UriTemplateCache =
            new LazyTypeSpecificCache<WebQueryBase, Type, string>(x => x.GetType(), BuildUri);

        public override IEnumerable<string> RedirectUriTemplates
        {
            get { return RedirectUriTemplatesCache.GetValue(this); }
        }
        private static readonly LazyTypeSpecificCache<WebQueryBase, Type, IEnumerable<string>> RedirectUriTemplatesCache =
            new LazyTypeSpecificCache<WebQueryBase, Type, IEnumerable<string>>(x => x.GetType(), BuildRedirectUris);

        protected void SetFlags(WebQueryBase queryToCopyFlagsFrom)
        {
            if (queryToCopyFlagsFrom == null)
                return;

            DebugFlag = queryToCopyFlagsFrom.DebugFlag;
        }

        public bool DebugFlag
        {
            get { return _debugFlag.Value; }
            set { _debugFlag.Value = value; }
        }
        private readonly QueryStringParameter<bool> _debugFlag;

        public bool IsPostback(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context", "Must supply a valid context");
            return
               (((context.Request.ServerVariables["REQUEST_METHOD"] ??
                  context.Request.HttpMethod ??
                     context.Request.RequestType ?? string.Empty).ToUpper() == "POST") &&
                        (context.Request.Form.Count > 0));
        }

        public virtual bool IsSecuredQuery { get { return false; } }

        public virtual string BreadcrumbName { get; set; }

        public virtual bool RebuildOldQueryString(NameValueCollection oldQueryCollection, out NameValueCollection newQueryCollection)
        {
            newQueryCollection = oldQueryCollection;
            return false;
        }

        public string CreateJscriptUrlFunction()
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<IUrlParameter> myParameters = Parameters.Where(p => p.PropertyName != "debug");

            string functionName = "Make{0}Url".With(GetType().Name);
            string parameters = myParameters.Select(p => p.PropertyName).Where(p => p != "debug").ToString(", ");
            string route = (string)GetType().GetField("Route").GetValue(this);

            sb
                .NewLine()
                .AppendLine("function {0}({1}) {{".With(functionName, parameters))
                .Tab().AppendLine("var url = '{0}';".With(route))
                .NewLine();

            myParameters.Where(p => p.IsUrlPathParameter).ForEach(p => sb.Tab().AppendLine("url += '/' + {0};".With(p.PropertyName)));

            sb.Tab().AppendLine("url += '/?js=y';");

            myParameters.Where(p => !p.IsUrlPathParameter).ForEach(p => sb.Tab().AppendLine("url += '&{0}=' + {0};".With(p.PropertyName)));

            sb
                .NewLine().Tab().AppendLine("return url;")
                .AppendLine("}");

            return sb.ToString();
        }

        public static string BuildUri(QueryBase query)
        {
            FieldInfo routeField = query.GetType().GetField("Route");

            if (routeField == null || !routeField.IsStatic || routeField.FieldType != typeof(string))
                throw new InvalidOperationException("Query requires a public static readonly string Route.");

            return BuildUri((string)routeField.GetValue(query), query.Parameters);
        }

        public static string BuildUri(string route, ICollection<IUrlParameter> parameters)
        {
            IEnumerable<IUrlParameter> urlPathParameters = parameters.Where(p => p.IsUrlPathParameter).ToList();
            IEnumerable<IUrlParameter> queryStringParameters = parameters.Where(p => !p.IsUrlPathParameter).ToList();

            return route +
                    urlPathParameters.Select(p => "[" + p.PropertyName + "]").ToString(string.Empty) +
                    ((!urlPathParameters.Any()) ? "/" : string.Empty) +
                    ((queryStringParameters.Any()) ? "?" : string.Empty) +
                    queryStringParameters.Select(p => "[" + p.PropertyName + "]").ToString(string.Empty);
        }

        public static IEnumerable<string> BuildRedirectUris(QueryBase query)
        {
            IHave301Redirects queryWithRedirect = query as IHave301Redirects;

            if (queryWithRedirect == null || queryWithRedirect.DeprecatedRoutes.IsNullOrEmpty())
                return new List<string>();

            return queryWithRedirect.DeprecatedRoutes.Select(route => BuildUri(route, query.Parameters));
        }
    }

    public class WebQueryParameter
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool Required { get; set; }
    }

    public abstract class WebQueryBase<T> : WebQueryBase
        where T : class, new()
    {
        protected List<WebQueryParameter> WebQueryParameters
        {
            get { return ParameterCache.GetValue(this); }
        }
        private static readonly LazyTypeSpecificCache<WebQueryBase<T>, Type, List<WebQueryParameter>> ParameterCache =
            new LazyTypeSpecificCache<WebQueryBase<T>,Type,List<WebQueryParameter>>(x => x.GetType(), q => q.BuildParameterCache<T>());

        protected WebQueryBase()
        {
            WebQueryParameters
                .Where(x => !(x.Type.IsGenericType && x.Type.GetGenericTypeDefinition() == typeof(ICollection<>)))
                .Select(x => Activator.CreateInstance(typeof(QueryStringParameter<>).MakeGenericType(x.Type), x.Name, x.Required))
                .Cast<IUrlParameter>()
                .ForEach(p => Parameters.Add(p));

            WebQueryParameters
                .Where(x => x.Type.IsGenericType && x.Type.GetGenericTypeDefinition() == typeof(ICollection<>) && x.Type.GetGenericArguments()[0] == typeof(string))
                .Select(x => Activator.CreateInstance(typeof(StringListQueryStringParameter), x.Name, x.Required))
                .Cast<IUrlParameter>()
                .ForEach(p => Parameters.Add(p));

            WebQueryParameters
                .Where(x => x.Type.IsGenericType && x.Type.GetGenericTypeDefinition() == typeof(ICollection<>) && x.Type.GetGenericArguments()[0] == typeof(Guid))
                .Select(x => Activator.CreateInstance(typeof(GuidListQueryStringParameter), x.Name, x.Required))
                .Cast<IUrlParameter>()
                .ForEach(p => Parameters.Add(p));

            WebQueryParameters
                .Where(x => x.Type.IsGenericType && x.Type.GetGenericTypeDefinition() == typeof(ICollection<>) && x.Type.GetGenericArguments()[0].IsEnum)
                .Select(x => Activator.CreateInstance(typeof(EnumListQueryStringParameter<>).MakeGenericType(x.Type.GetGenericArguments()[0]), x.Name, x.Required))
                .Cast<IUrlParameter>()
                .ForEach(p => Parameters.Add(p));

        }

        private List<WebQueryParameter> BuildParameterCache<U>()
        {
            return
                typeof (U).GetProperties()
                    .Select(p => new WebQueryParameter {Name = p.Name, Type = p.PropertyType, Required = false})
                    .ToList();
        }

        public void Set<U>(Expression<Func<T, U>> exp, U value)
        {
            GetParameter(exp).Value = value;
        }

        public U Get<U>(Expression<Func<T, U>> exp)
        {
            return GetParameter(exp).Value;
        }

        protected string GetPropertyName<U>(Expression<Func<T, U>> exp)
        {
            if (!(exp.Body is MemberExpression))
                throw new InvalidOperationException("Invalid lambda in GetPropertyname().  Must be a property reference.");

            return (exp.Body as MemberExpression).Member.Name;
        }

        protected UrlParameterBase<U> GetParameter<U>(Expression<Func<T, U>> exp)
        {
            return Parameters[GetPropertyName(exp)] as UrlParameterBase<U>;
        }

        public T GetValues()
        {
            T data = new T();

            foreach (var p in typeof(T).GetProperties())
            {
                dynamic parameter = Parameters[p.Name];

                p.SetValue(data, parameter.Value, null);
            }

            return data;
        }

        public void SetValues(T data)
        {
            foreach (var p in typeof(T).GetProperties())
            {
                dynamic parameter = Parameters[p.Name];
                dynamic value = p.GetValue(data, null);

                parameter.Value = value;
            }
        }
    }
}
