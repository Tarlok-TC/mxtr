using System;
using System.IO;
using System.Web.Mvc;

namespace mxtrAutomation.Web.Common.UI
{
    public class ScriptTag : IDisposable
    {
        private readonly TextWriter _writer;
        private readonly TagBuilder _builder;

        public ScriptTag(HtmlHelper helper, string type, string id)
        {
            _writer = helper.ViewContext.Writer;
            _builder = new TagBuilder("script");
            _builder.MergeAttribute("type", type);
            _builder.MergeAttribute("id", id);
            _writer.WriteLine(_builder.ToString(TagRenderMode.StartTag));
        }

        public void Dispose()
        {
            _writer.WriteLine(_builder.ToString(TagRenderMode.EndTag));
        }
    }
}
