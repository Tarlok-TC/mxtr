using System;
using System.Linq;

namespace mxtrAutomation.Common.Attributes
{
    public class StringValueAttribute : Attribute
    {
        public string Value { get; protected set; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }
}
