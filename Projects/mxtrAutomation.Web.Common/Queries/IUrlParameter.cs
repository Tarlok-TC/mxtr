using System.Text.RegularExpressions;

namespace mxtrAutomation.Web.Common.Queries
{
    public interface IUrlParameter
    {
        bool IsRequired { get; set; }
        bool IsUsed { get; }
        bool WasUsed { get; }
        bool IsValid { get; }
        bool IsUrlPathParameter { get; }
        string PropertyName { get; set; }
        string RegexMatchString { get; }
        void SetValue(Match m);
        void SetValue(string rawInput);
        string GetName();
        string GetValue();
        void CopyTo(IUrlParameter destination);
        void Clear();
    }
}
