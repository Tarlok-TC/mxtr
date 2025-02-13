using System;

namespace mxtrAutomation.Common.Attributes
{
    public class BoolValueAttribute : Attribute
    {
        public bool Value { get; protected set; }

        public BoolValueAttribute(bool value)
        {
            Value = value;
        }
    }
}
