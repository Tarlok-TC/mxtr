using System;
using System.Text.RegularExpressions;
using mxtrAutomation.Web.Common.Extensions;

namespace mxtrAutomation.Web.Common.Queries
{
    public enum UrlParameterFormat
    {
        LowercaseEncoded,
        LowercaseNotEncoded,
        MaintainCaseEncoded,
        MaintainCaseNotEncoded,
    }

    public abstract class UrlParameterBase<T> : IUrlParameter
    {
        #region ctors

        protected UrlParameterBase(string propertyName) : this(propertyName, false) { }

        protected UrlParameterBase(string propertyName, bool isRequired) : this(propertyName, isRequired, null, null) { }

        protected UrlParameterBase(string propertyName, bool isRequired, Func<T,string> serializer, Func<string,T> deserializer)
        {
            PropertyName = propertyName;
            IsRequired = isRequired;
            Serializer = serializer;
            Deserializer = deserializer;
            Format = UrlParameterFormat.LowercaseEncoded;

            _isUsed = false;
            _wasUsed = false;
        }

        #endregion

        #region Properties

        public abstract bool IsUrlPathParameter { get; }

        public virtual Func<T,string> Serializer { get; set; }

        public virtual Func<string,T> Deserializer { get; set; }

        /// <summary>
        /// Maintains whether or not this parameter has a value set or not.
        /// </summary>
        public virtual bool IsUsed
        {
            get { return _isUsed; }
            protected set { _isUsed = value; }
        }
        private bool _isUsed;

        public virtual bool WasUsed
        {
            get { return _wasUsed; }
            protected set { _wasUsed = value; }
        }
        private bool _wasUsed;

        /// <summary>
        /// Specifies whether the value should be passed to cloned objects.
        /// </summary>
        private bool InternalIsCloneable = true;
        public virtual bool IsCloneable
        {
            get { return InternalIsCloneable; }
            set { InternalIsCloneable = value; }
        }

        /// <summary>
        /// By default, it is expected that parameter names do NOT include spaces.
        /// If the name has been omitted, or if it contains a space, then it is
        /// invalid.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid
        {
            get
            {
                if (DeserializeFailed)
                    return false;

                return !string.IsNullOrEmpty(PropertyName) && PropertyName.IndexOf(' ') == -1;
            }
        }

        protected bool DeserializeFailed { get; set; }

        #region IUrlParameter Members



        #endregion

        private bool _isRequired;
        /// <summary>
        /// When the value of IsRequired is changed, then the matching regular
        /// expression is also changed.
        /// </summary>
        public bool IsRequired
        {
            get { return _isRequired; }
            set
            {
                if (_isRequired != value)
                {
                    _reValueMatch = null;
                    _isRequired = value;
                }
            }
        }

        public string PropertyName { get; set; }

        private T _defaultValue;
        public virtual T DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                if (!IsUsed)
                {
                    _value = value;
                }
            }
        }

        protected T _value;
        /// <summary>
        /// The value of the property.  When set, the property is set
        /// </summary>
        public virtual T Value
        {
            get { return _value; }
            set
            {
                _value = value;

                _wasUsed = true;
                _isUsed = !Equals(_value, DefaultValue);

                // Special check for strings
                if (typeof(T) == typeof(string) && _isUsed)
                {
                    string str = (_value as string) == null ? null : _value.ToString().Trim();
                    _isUsed = !string.IsNullOrEmpty(str);
                }
            }
        }

        private Regex _reValueMatch;
        /// <summary>
        /// Creates a regular expression to match against an entire input url, looking for the value
        /// specified by this parameter.  
        /// </summary>
        public Regex RegexMatch
        {
            get
            {
                _reValueMatch = new Regex(RegexMatchString, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                return _reValueMatch;
            }
        }

        public UrlParameterFormat Format { get; set; }

        #endregion

        #region Abstracts

        /// <summary>
        /// Outputs a string that is to be used to create a regular expression to match input
        /// data.
        /// </summary>
        public abstract string RegexMatchString { get; }

        public virtual void SetValue(string rawInput)
        {
        }

        public virtual string GetName()
        {
            switch (Format)
            {
                case UrlParameterFormat.LowercaseEncoded:
                    return PropertyName.ToLowerInvariant().UrlEncode();
                case UrlParameterFormat.LowercaseNotEncoded:
                    return PropertyName.ToLowerInvariant();
                case UrlParameterFormat.MaintainCaseEncoded:
                    return PropertyName.UrlEncode();
                default:
                    return PropertyName;
            }
        }

        /// <summary>
        /// Used to accept an input string, and somehow convert that into the value of the parameter.
        /// </summary>
        /// <param name="m"></param>
        public virtual void SetValue(Match m)
        {
        }

        public virtual void CopyTo(IUrlParameter destination)
        {
            if (destination == null) return;

            var destinationParameter = destination as UrlParameterBase<T>;
            if (destinationParameter == null)
                throw new InvalidOperationException("Unable to copy parameter of type " + GetType().Name + " to parameter of type " + destination.GetType().Name);

            if (IsUsed && InternalIsCloneable)
            {
                var cloneMe = Value as ICloneable;
                if (null == cloneMe)
                    destinationParameter.Value = Value;
                else
                    destinationParameter.Value = (T)cloneMe.Clone();
            }
        }

        protected virtual bool IsTypeInteger()
        {
            Type t = typeof(T);
            return (
                   (t.IsAssignableFrom(typeof(long))) ||
                   (t.IsAssignableFrom(typeof(int))) ||
                   (t.IsAssignableFrom(typeof(short))) ||
                   (t.IsAssignableFrom(typeof(byte)))
                   );
        }

        protected virtual bool IsTypeGuid()
        {
            Type t = typeof(T);
            return (
                   (t.IsAssignableFrom(typeof(Guid)))
                   );
        }

        //protected virtual bool TryParseValue(string value, out T outputValue)
        //{
        //    bool success = false;
        //    if (typeof (T) is long)
        //    {
        //        long lngValue;
        //        success = long.TryParse(value, out lngValue);
        //        if (success)
        //            outputValue = Convert.ChangeType(lngValue, typeof(T));
        //    }
        //    return success;
        //}

        #endregion

        /// <summary>
        /// This will probably become a problem as we have different characters that mean 
        /// different things that need to be converted.  
        /// </summary>
        /// <returns></returns>
        protected virtual string GetValue()
        {
            var temp = Serializer == null
                           ? Value == null ? string.Empty : Value.ToString()
                           : Serializer(Value);

            switch (Format)
            {
                case UrlParameterFormat.LowercaseEncoded:
                    temp = temp.ToLowerInvariant().UrlEncode();
                    break;

                case UrlParameterFormat.LowercaseNotEncoded:
                    temp = temp.ToLowerInvariant();
                    break;

                case UrlParameterFormat.MaintainCaseEncoded:
                    temp = temp.UrlEncode();
                    break;
            }

            return temp;
        }

        string IUrlParameter.GetValue()
        {
            return GetValue();
        }

        public void Clear()
        {
            IsUsed = false;
            WasUsed = false;
            _value = DefaultValue;
        }
    }
}
