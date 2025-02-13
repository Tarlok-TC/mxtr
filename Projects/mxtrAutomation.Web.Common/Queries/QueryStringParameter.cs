using System;
using System.ComponentModel;
using mxtrAutomation.Common;

namespace mxtrAutomation.Web.Common.Queries
{
    /// <summary>
    /// Parameters that are used in classes that derive from QueryBase.  This class is designed to make it easy to convert 
    /// potential input parameters into querystrings.
    /// </summary>
    /// <typeparam name="T">The type of input to be passed.</typeparam>
    public class QueryStringParameter<T> : UrlParameterBase<T>
    {
        #region Ctors

        public QueryStringParameter(string propertyName, bool isRequired) : base(propertyName, isRequired) { }

        public QueryStringParameter(string propertyName) : base(propertyName) { }

        public QueryStringParameter(string propertyName, bool isRequired, Func<T,string> serializer, Func<string,T> deserializer)
            : base(propertyName, isRequired, serializer, deserializer) { }
        #endregion

        public override bool IsUrlPathParameter { get { return false; } }

        /// <summary>
        /// Formats the querystring for output in the format "{0}={1}&".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IsUsed && IsValid
                ? string.Format("{0}={1}&", GetName(), GetValue())
                : string.Empty;
        }

        public override string RegexMatchString
        {
            get
            {
                return string.Format(@"(\?)(([^\?]*?&)|)({0}=(?<{0}>" + (IsTypeInteger() ? @"\d+" : (IsTypeGuid() ? RegularExpressions.PatternGuid : "[^$&]+")) + "))", PropertyName);
            }
        }

        public override void SetValue(string rawInput)
        {
            try
            {
                Value = (Deserializer != null) ? Deserializer(rawInput) :
                    (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(rawInput);
            }
            catch
            {
                DeserializeFailed = true;
            }
        }
    }
}
