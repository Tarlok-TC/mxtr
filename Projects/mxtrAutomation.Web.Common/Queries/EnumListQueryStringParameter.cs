using System;

namespace mxtrAutomation.Web.Common.Queries
{
    public class EnumListQueryStringParameter<T> : ListQueryStringParameter<T>
    {
        public EnumListQueryStringParameter(string propertyName, bool isRequired) :
            base(propertyName, isRequired, x => x.ToString(), x => (T) Enum.Parse(typeof(T), x))
        {
        }
    }
}
