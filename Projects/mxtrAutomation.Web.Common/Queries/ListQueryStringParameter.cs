using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Web.Common.Queries
{
    public class ListQueryStringParameter<T> : QueryStringParameter<ICollection<T>>
    {
        public ListQueryStringParameter(string propertyName, bool isRequired, Func<T, string> serializer, Func<string, T> deserilizer)
            : base(propertyName, isRequired, x => SerializationHelpers.SerialzeListToString(x, serializer), x => SerializationHelpers.DeserializeListFromString(x, deserilizer))
        {
            
        }

        public override bool IsUsed { get { return !Value.IsNullOrEmpty(); } }

        public override bool WasUsed { get { return IsUsed && base.WasUsed ; } }

        public override ICollection<T> Value
        {
            get { return base.Value ?? (base.Value = new List<T>()); }
            set { base.Value = value ?? new List<T>(); }
        }

#if false
        public override void SetValue(string rawInput)
        {
            base.Value = SerializationHelpers.DeserializeListFromString(HttpUtility.UrlDecode(rawInput), Deserializer);

            base.Value = SerializationHelpers.DeserializeStringListFromString(HttpUtility.UrlDecode(rawInput));
        }

        public virtual void SetValue(ICollection<string> input)
        {
            base.Value = SerializationHelpers.DeserializeStringListFromStringList(input);
        }

        public override void CopyTo(IUrlParameter destination)
        {
            ((ListQueryStringParameter)destination).Value = SerializationHelpers.DeserializeStringListFromStringList(Value);
        }
#endif
    }
}
