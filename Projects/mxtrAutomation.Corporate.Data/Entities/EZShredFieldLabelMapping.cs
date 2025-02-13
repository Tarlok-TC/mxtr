using mxtrAutomation.Data;

namespace mxtrAutomation.Corporate.Data.Entities
{
    public class EZShredFieldLabelMapping:Entity
    {
        public string EZShredFieldName { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Set { get; set; }

    }
}
