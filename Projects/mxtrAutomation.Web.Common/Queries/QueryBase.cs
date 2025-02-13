using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Items;
using mxtrAutomation.Web.Common.Extensions;
using mxtrAutomation.Web.Common.Helpers;

namespace mxtrAutomation.Web.Common.Queries
{
    public enum TrailingSlashKind
    {
        Auto,
        NoSlash,
        ForceSlash
    }

    public abstract class QueryBase : ICloneable
    {
        protected QueryBase()
        {
            Parameters = new QueryParameters();
        }

        protected QueryBase(Uri input)
            : this()
        {
            Deserialize(input);
        }

        public static implicit operator ViewLink(QueryBase query)
        {
            return query.AsViewLink();
        }

        #region Properties

        public abstract string UriTemplate { get; }
        public virtual IEnumerable<string> RedirectUriTemplates { get { return null; } }

        private string _basePath;
        /// <summary>
        /// The basepath can be set at any point.  There are no default enforced by the query
        /// objects themselves, because they should not be tethered to any particular configuration
        /// implementation.  Instead, the basepath can be provided via dependency, or constructor
        /// injection.  Because the endpoint is hard coded into the specific implementations, the
        /// basepath needs to be able to be dropped in directly without modification.  That means
        /// that the UriTemplate should know what to expect.
        /// </summary>
        public virtual string BasePath
        {
            get { return _basePath; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Contains("?"))
                    throw new ArgumentException("Base paths may not contain '?'");
                _basePath = value;
            }
        }

        private QueryParameters _parameters;
        /// <summary>
        /// All of the parameters for a query are added to this dictionary
        /// </summary>
        public QueryParameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public virtual string EndPoint
        {
            get
            {
                if (string.IsNullOrEmpty(_endPoint))
                {
                    lock (_endpointLock)
                    {
                        if (string.IsNullOrEmpty(_endPoint))
                        {
                            _endPoint = _reEndPoint.Match(UriTemplate).Groups["endpoint"].Value;
                            if (_endPoint.EndsWith("/") && _endPoint.Length > 1)
                                _endPoint = _endPoint.Substring(0, _endPoint.Length - 1);
                        }
                    }
                }
                return _endPoint.ToLower();
            }
        }

        private string _endPoint;
        private object _endpointLock = new object();

        public virtual List<string> RedirectEndPoints
        {
            get
            {
                if (_301EndPoints == null)
                {
                    lock (_301EndPointsLock)
                    {
                        if (_301EndPoints == null)
                        {
                            _301EndPoints =
                                RedirectUriTemplates
                                    .Select(uriTemplate => _reEndPoint.Match(uriTemplate).Groups["endpoint"].Value)
                                    .Select(endpoint =>
                                            endpoint.EndsWith("/") && endpoint.Length > 1
                                                ? endpoint.Substring(0, endpoint.Length - 1)
                                                : endpoint)
                                    .Select(endpoint => endpoint.ToLower())
                                    .ToList();
                        }
                    }
                }
                return _301EndPoints;
            }
        }
        private List<string> _301EndPoints;
        private object _301EndPointsLock = new object();

        /// <summary>
        /// returns true if no parameters have been set 
        /// other then the debug flag
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                return !Parameters.Exists(p => p.PropertyName.ToLower() != "debug" && p.PropertyName.ToLower() != "showads" && p.PropertyName.ToLower() != "outputformat" && p.PropertyName.ToLower() != "where" && p.IsUsed);
            }
        }

        private string _urlPathTemplateSegment;
        /// <summary>
        /// Used to get the Url Path portion of a UriTemplate.
        /// </summary>
        public string UrlPathTemplateSegment
        {
            get
            {
                if (_urlPathTemplateSegment == null)
                    SetPathTemplateSegments();
                return _urlPathTemplateSegment;
            }
        }

        private string _queryStringUrlTemplateSegment;
        /// <summary>
        /// Used to get the Query String portion of a UriTemplate.  Returns string.Empty if
        /// the template does not have a query string segment.
        /// </summary>
        public string QueryStringUrlTemplateSegment
        {
            get
            {
                if (_queryStringUrlTemplateSegment == null)
                    SetPathTemplateSegments();
                return _queryStringUrlTemplateSegment;
            }
        }

        protected Regex MatchingRegex
        {
            get { return MatchingRegexCache.GetValue(this); }
        }
        private static readonly LazyTypeSpecificCache<QueryBase, Type, Regex> MatchingRegexCache =
            new LazyTypeSpecificCache<QueryBase, Type, Regex>(x => x.GetType(), x => x.BuildMatchingRegex(x.EndPoint));

        protected IDictionary<string, Regex> MatchingRedirectRegexes
        {
            get { return MatchingRedirectRegexCache.GetValue(this); }
        }
        private static readonly LazyTypeSpecificCache<QueryBase, Type, IDictionary<string, Regex>> MatchingRedirectRegexCache =
            new LazyTypeSpecificCache<QueryBase, Type, IDictionary<string, Regex>>(x => x.GetType(), x => x.RedirectEndPoints.ToDictionary(y => y, x.BuildMatchingRegex));

        protected Regex BuildMatchingRegex(string endPoint)
        {
            StringBuilder sb =
                (endPoint == "/") ? new StringBuilder("^") : new StringBuilder("^" + endPoint);

            IEnumerator<IUrlParameter> urlParameters = Parameters.GetUrlParameterEnumerator();

            if (urlParameters.MoveNext())
                GenerateRegex(sb, urlParameters);
            else // there are no UrlPathParameters at all, so I need to make sure that the last parameter code is inserted.
                sb.Append(@"(?=($|/\?|/$|\?))");

            return
                new Regex(sb.ToString(),
                          RegexOptions.IgnoreCase |
                          RegexOptions.Multiline |
                          RegexOptions.ExplicitCapture |
                          RegexOptions.Compiled |
                          RegexOptions.IgnorePatternWhitespace);

        }
        #endregion

        #region Regular Expressions

        /// <summary>
        /// Because a string of optional urlpaths may not be populated, extraneous /'s may
        /// creep in to a generated URL.  As a result, this regex is used to remove them.
        /// </summary>
        private static readonly Regex _reMultiSlashes = new Regex(@"(?<=([^:]|^))(?<multislashes>[/]{2,})",
                                                         RegexOptions.IgnoreCase |
                                                         RegexOptions.Multiline |
                                                         RegexOptions.ExplicitCapture |
                                                         RegexOptions.Compiled |
                                                         RegexOptions.IgnorePatternWhitespace);
        /// <summary>
        /// Each UrlParameter assumes that another will follow it.  For example, querystring 
        /// parameters end in '&'.  Additionally, Querystring parameters, being optional in a url,
        /// may require the existence of a '?' in the UrlTemplate.  If no QueryString parameters 
        /// are specified though, then the '?' may also be hanging out at the end.  So this regex
        /// finds them.
        /// </summary>
        private static readonly Regex _reBadUrlEnding = new Regex(@"(?<BadUrlEnding>([&\?#]+$|&(?=[#]{1})))",
                RegexOptions.IgnoreCase |
                RegexOptions.Multiline |
                RegexOptions.ExplicitCapture |
                RegexOptions.Compiled |
                RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// This regex is used to get all of the parameter names defined in a UriTemplate.
        /// </summary>
        private static readonly Regex _reUrlPath = new Regex(@"(?<VAR>\[[\w_]+\])",
                                                    RegexOptions.IgnoreCase |
                                                    RegexOptions.Multiline |
                                                    RegexOptions.ExplicitCapture |
                                                    RegexOptions.Compiled |
                                                    RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex _reHasExtension = new Regex(@"
                \.[\w]{2,4}(\?|$)",
                                                         RegexOptions.IgnoreCase |
                                                         RegexOptions.Multiline |
                                                         RegexOptions.Compiled |
                                                         RegexOptions.IgnorePatternWhitespace);


        private static readonly Regex _reEndPoint = new Regex(@"(?<endpoint>^[^\[\?]+)($|/\?|/|\[|(?<=\.[\w]{2,4})?)",
                                                     RegexOptions.IgnoreCase |
                                                     RegexOptions.Multiline |
                                                     RegexOptions.ExplicitCapture |
                                                     RegexOptions.Compiled |
                                                     RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex _reHashParse = new Regex(@"\#(?<Hash>[\w\W]*)",
                RegexOptions.IgnoreCase |
                RegexOptions.Multiline |
                RegexOptions.ExplicitCapture |
                RegexOptions.Compiled |
                RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Verification

        protected string _errorMessage;
        /// <summary>
        /// IsValid is checked EVERY TIME this method is called.  It may be smarter to 
        /// cache the result, and only reset the error when a value changes, but it's also
        /// a pain.
        /// </summary>
        public string Error
        {
            get { return IsValid ? string.Empty : _errorMessage; }
        }

        /// <summary>
        /// This is used to ignore the fix to the query string when the scenario is:
        /// 
        /// http://www.foo.com?test=goo
        /// 
        /// Some scenarios we do not want to automatically fix this as:
        /// 
        /// http://www.foo.com/?test=goo
        /// </summary>
        public virtual bool DoNotAddSlashPriorToQueryString { get { return false; } }

        /// <summary>
        /// One of Auto, NoSlash, ForceSlash.  Used to determine if a / appears at the end of the base
        /// route or not.  If left at Auto, then a / is placed iff there are any url path parameters.
        /// </summary>
        public TrailingSlashKind TrailingSlash { get; set; }

        /// <summary>
        /// Verifies that a BasePath exists, and that it does not end in a '?' unless
        /// there are other parameters being provided.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid
        {
            get { return IsValidWebQuery; }
        }

        protected bool IsValidWebQuery
        {
            get
            {
                StringBuilder sbErrorMessage = new StringBuilder();
                if (BasePath == null)
                    sbErrorMessage.Append("You must specify a base url.");
                else if (BasePath.EndsWith("?") && Parameters.Count > 0)
                    sbErrorMessage.Append(
                        @"The BaseUrl can not end in a '?' character if there are url parameters associated with the query, because the url parameters would wind up being rendered in the querystring.");

                Parameters.ForEach(p => VerifyParameterIsValid(sbErrorMessage, p));

                Parameters.ForEach(p => VerifyRequiredParameter(sbErrorMessage, p));

                _errorMessage = sbErrorMessage.ToString();
                return string.IsNullOrEmpty(_errorMessage);
            }
        }

        /// <summary>
        /// Ensure that if a parameter is required, but has no value, that an error message is provided
        /// </summary>
        /// <param name="sbErrorMessage"></param>
        /// <param name="parameter"></param>
        private static void VerifyRequiredParameter(StringBuilder sbErrorMessage, IUrlParameter parameter)
        {
            if (parameter.IsRequired & !parameter.IsUsed)
                sbErrorMessage.AppendFormat("{0} was required, but not supplied.{1}",
                                            parameter.PropertyName, Environment.NewLine);
        }

        private static void VerifyParameterIsValid(StringBuilder sbErrorMessage, IUrlParameter parameter)
        {
            if (!parameter.IsValid)
                sbErrorMessage.AppendFormat("{0} was invalid.{1}", parameter.PropertyName, Environment.NewLine);
        }

        #endregion

        #region Serializing

        public static implicit operator string(QueryBase query)
        {
            if (query == null)
                return null;

            return query.ToString();
        }

        /// <summary>
        /// Outputs a url using a base Uri.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Iterate through each parameter, 
            string outputString = (BasePath ?? string.Empty) + UriTemplate;

            // Get the parameters from the UriTemplate;
            MatchCollection matches = _reUrlPath.Matches(outputString);

            // If there are no matches, then just return what we have.
            if (matches == null) return outputString;

            for (int i = 0; i < matches.Count; i++)
            {
                outputString = outputString.Replace(
                    matches[i].ToString(),
                    Parameters[matches[i].ToString()] != null ?
                    Parameters[matches[i].ToString()].ToString() : "");
            }

            // Now remove all of the unneeded parameters, and any & or ? or #s at the end.
            outputString = _reMultiSlashes.Replace(outputString, "/");
            outputString = _reBadUrlEnding.Replace(outputString, "");
            // If (HasExtension) return
            // else if (HasQueryString)
            //      replace ? with /? if necessary
            // else
            //      append / if missing
            if (_reHasExtension.IsMatch(outputString))
                return outputString;
            if (outputString.Contains("?"))
                return outputString.Contains("/?") ? (DoNotAddSlashPriorToQueryString ? outputString.Replace("/?", "?") : outputString) : outputString;
            if (TrailingSlash == TrailingSlashKind.NoSlash || (TrailingSlash == TrailingSlashKind.Auto && Parameters.Any(p => p.IsUrlPathParameter)))
                return outputString.EndsWith("/") ? outputString.Substring(0, outputString.Length - 1) : outputString;
            return outputString.EndsWith("/") ? outputString : outputString + "/";
        }

        public Uri ToUri()
        {
            if (BasePath == null)
                throw new Exception("A base uri must be provided in order to generate a uri.");
            return new Uri(ToString());
        }

        #endregion

        #region Deserializing and Matching

        private static void GenerateRegex(StringBuilder sb, IEnumerator<IUrlParameter> enumerator)
        {
            IUrlParameter parameter = enumerator.Current;

            if (!parameter.IsRequired)
                sb.Append(@"((?=/[^\?$])");

            sb.Append(parameter.RegexMatchString);

            // Write more variables, which needs to call self
            if (enumerator.MoveNext())
                GenerateRegex(sb, enumerator);
            else // this is the last parameter, so make sure it doesn't match urls with extra url params.
                sb.Append(@"(?=($|/\?|/$|\?))");

            // write ending, if this was an optional parameter
            if (!parameter.IsRequired)
                sb.Append(@"|(?=($|/\?|/$|\?)))");
        }

        private void SetPathTemplateSegments()
        {
            string[] queryTemplate = UriTemplate.Split('?');
            _urlPathTemplateSegment = queryTemplate[0];
            _queryStringUrlTemplateSegment = (queryTemplate.Length == 2) ? queryTemplate[1] : string.Empty;
        }

        public virtual bool IsMatch(Uri input)
        {
            if (input == null)
                return false;

            string path = input.AbsolutePath.ToLower();

            return IsMatch(path) || MatchesDepricatedRoute(path);
        }

        /// <summary>
        /// Returns whether the url matches based on the UrlPath
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual bool IsMatch(string input)
        {
            return input.StartsWith(EndPoint) && MatchingRegex.IsMatch(input.Substring(input.IndexOf(EndPoint)));
        }

        public virtual bool MatchesDepricatedRoute(Uri input)
        {
            return input != null && MatchesDepricatedRoute(input.AbsolutePath.ToLower());
        }

        public virtual bool MatchesDepricatedRoute(string input)
        {
            return
                MatchingRedirectRegexes
                    .Any(x => input.Contains(x.Key) && x.Value.IsMatch(input.Substring(input.IndexOf(x.Key))));
        }

        /// <summary>
        /// Takes a Uri as input, and sets all of the values.
        /// </summary>
        /// <param name="input"></param>
        public void Deserialize(Uri input)
        {
            Deserialize(input, null);            
        }

        public void Deserialize(Uri input, NameValueCollection requestForm)
        {
            if (!IsMatch(input))
            {
                if (!MatchesDepricatedRoute(input))
                    throw new ArgumentException("Unable to deserialize " + input, "input");

                string uriAsString = input.ToUrl().ToLower();
                string oldEndPoint = RedirectEndPoints.Where(uriAsString.Contains).OrderByDescending(s => s.Length).First();

                input = new Uri(uriAsString.Replace(oldEndPoint, EndPoint));
            }
            DeserializeInternal(input, requestForm);
        }

        protected virtual void DeserializeInternal(Uri input, NameValueCollection requestForm)
        {
            ExtractBasePath(input);

            string query = input.Query;

            if (requestForm != null && requestForm.HasKeys())
            {
                if (query.IsNullOrEmpty())
                    query += "?" + requestForm;
                else
                    query += "&" + requestForm;
            }

            var queryString = ParseQueryString(query);

            Match match = MatchingRegex.Match(input.AbsolutePath);
            if (match == null) return;
            foreach (IUrlParameter parameter in Parameters)
            {
                parameter.Clear();

                // Special logic for hash parameters
                if (parameter is UrlHashParameter)
                {
                    Match hashMatch = _reHashParse.Match(input.ToString());
                    if (hashMatch.Success)
                        parameter.SetValue(hashMatch);
                }
                else
                {
                    // Set it from either the url or the querystring
                    if (match.Groups[parameter.PropertyName].Success)
                        parameter.SetValue(match);
                    else if (queryString[parameter.PropertyName] != null)
                        parameter.SetValue(queryString[parameter.PropertyName]);
                }
            }
        }

        protected virtual NameValueCollection ParseQueryString(string query)
        {
            return HttpUtility.ParseQueryString(query);
        }

        private void ExtractBasePath(Uri input)
        {
            // Set the basepath            
            if (string.Compare(EndPoint, "/") == 0)
            {
                BasePath = input.ToHost();
            }
            else
            {
                string uriAsString = input.ToUrl().ToLower();

                IEnumerable<string> allEndpoints = RedirectEndPoints.Union(new List<string> {EndPoint});

                int index =
                    allEndpoints
                        .Select(endpoint => uriAsString.IndexOf(endpoint))
                        .FirstOrDefault(x => x >= 0);


                BasePath = uriAsString.Substring(0, index);
            }
        }

        public bool ParameterWasUsed(string parameterName)
        {
            IUrlParameter parameter = Parameters[parameterName];
            return (parameter != null) ? parameter.WasUsed : false;
        }
        #endregion

        #region ICloneable Members

        ///<summary>
        ///Creates a new object that is a copy of the current instance.
        ///</summary>
        ///
        ///<returns>
        ///A new object that is a copy of this instance.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public virtual object Clone()
        {
            ConstructorInfo info = GetType().GetConstructor(new Type[] { typeof(Uri) });
            if (info == null)
                throw new NotImplementedException(
                    "This query is not cloneable, because it does not implement a constructor that accepts a Uri.");
            return info.Invoke(new Object[] { ToUri() });
        }

        public QueryBase CloneNoBasePath()
        {
            ConstructorInfo info = GetType().GetConstructor(new Type[0]);
            if (info == null)
                throw new NotImplementedException(
                    "This query does not accept an empty constructor.");
            var clonedQuery = (QueryBase)info.Invoke(null);
            foreach (IUrlParameter parameter in Parameters)
                parameter.CopyTo(clonedQuery.Parameters[parameter.PropertyName]);
            //Parameters.ForEach(p=> p.CopyTo(clonedQuery.Parameters[p.PropertyName]));
            return clonedQuery;
        }

        #endregion

        public void ApplyUsedParametersFrom(QueryBase query)
        {
            query.Parameters.Where(p => p.WasUsed)
                .ForEach(p => Parameters.Single(q => q.PropertyName == p.PropertyName).SetValue(p.GetValue()));
        }

        public string GetParameterKeyFromProperty(string propertyName)
        {
            string privateUrlParameterName = "_" + propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);

            return
                GetType()
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.Name == privateUrlParameterName)
                    .Select(f => f.GetValue(this) as IUrlParameter)
                    .Select(p => p.PropertyName)
                    .SingleOrDefault();
        }
    }
}
