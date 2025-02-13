using System;

namespace mxtrAutomation.Common.Utils
{
    public sealed class NullPhoneNumber : IPhoneNumber
    {
        public string CallingCode { get { return String.Empty; } }
        public string Npa { get { return String.Empty; } }
        public string Nxx { get { return String.Empty; } }
        public string Xxxx { get { return String.Empty; } }
        public string Raw { get { return String.Empty; } }
        public string Sanitized { get { return String.Empty; } }
        public int Length { get { return 0; } }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}
