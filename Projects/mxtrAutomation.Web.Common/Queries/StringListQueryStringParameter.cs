namespace mxtrAutomation.Web.Common.Queries
{
    public class StringListQueryStringParameter : ListQueryStringParameter<string>
    {
        public StringListQueryStringParameter(string propertyName, bool isRequired)
            : base(propertyName, isRequired, s => s, s => s)
        {
        }
    }

}
