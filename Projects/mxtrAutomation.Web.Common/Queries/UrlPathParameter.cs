using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Utils;

namespace mxtrAutomation.Web.Common.Queries
{
    public class UrlPathParameter<T> : UrlParameterBase<T>
    {
        #region Ctors
        public UrlPathParameter(string propertyName, bool isRequired) : base(propertyName, isRequired) { }

        public UrlPathParameter(string propertyName) : base(propertyName) { }

        public UrlPathParameter(string propertyName, bool isRequired, Func<T,string> serializer, Func<string,T> deserializer)
            : base(propertyName, isRequired, serializer, deserializer) { }
        #endregion

        public override bool IsUrlPathParameter { get { return true; } }

        public override void SetValue(Match m)
        {
            string input = m.Groups[PropertyName].Value;
            string value = HttpUtility.UrlDecode(input);

            if (Deserializer != null)
                Value = Deserializer(value);
            else
            {
                if (typeof(T).IsEnum)
                {
                    Value = EnumUtils.TryParseEnum<T>(value);
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null && converter.IsValid(value))
                        Value = (T)converter.ConvertFromString(value);
                    else
                        Value = default(T);
                }
            }

            //TypeConverter converter = CustomTypeConverter ?? TypeDescriptor.GetConverter(typeof(T));
            //// If a TypeDescriptor can't be found, then this will probably be null.  Not sure how to 
            //// handle that gracefully yet.
            //Value = (T)converter.ConvertFromString(HttpUtility.UrlDecode(input));
        }

        public override string RegexMatchString
        {
            get
            {
                if (typeof(Enum).IsAssignableFrom(typeof(T)))
                    return "/(?<{0}>({1}))".With(PropertyName, Enum.GetNames(typeof(T)).ToString("|"));

                //if (IsRequired)
                return @"/(?<" + PropertyName + @">" + (IsTypeInteger() ? @"\d" : @"[^/\?$]") + "+)";
                //else
                //    return @"(?(?=/[^?$])/(?<" + PropertyName + @">" + (IsTypeInteger() ? @"\d" : @"[^/\?$]") + "+)|)";
            }
        }

        public override T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                WasUsed = true;
                IsUsed = _value == null
                             ? false
                             : !_value.Equals(default(T)) && !_value.Equals(DefaultValue);
            }
        }

        public override string ToString()
        {
            return IsUsed ? string.Format("/{0}", GetValue()) : string.Empty;
        }
    }
}
