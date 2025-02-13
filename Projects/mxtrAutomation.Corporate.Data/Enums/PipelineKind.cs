using mxtrAutomation.Common.Attributes;
using System.ComponentModel;

namespace mxtrAutomation.Corporate.Data.Enums
{
    public enum PipelineKind
    {
        [Description("Lead")]
        Lead,

        [Description("Contact")]
        Contact,

        [Description("Quote Sent")]
        QuoteSent,

        [Description("Won/Not Scheduled")]
        Scheduled,
    }
    public enum PipelineStatus
    {
        [Description("Lost")]
        Lost,

        [Description("Won")]
        Won,
    }
}
