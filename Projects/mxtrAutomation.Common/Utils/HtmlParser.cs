using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace mxtrAutomation.Common.Utils
{
    public class HtmlParser
    {
        private string _url;
        private Lazy<byte[]> _htmlData;

        public string Url { get { return _url; } }

        public HtmlParser(string url)
        {
            _url = url;
            _htmlData = new Lazy<byte[]>(() =>
            {
                WebClient client = new WebClient();
                return client.DownloadData(_url);
            });
        }

        public List<HtmlElement> GetLinks()
        {
            List<HtmlElement> elements = new List<HtmlElement>();

            using (MemoryStream ms = new MemoryStream(_htmlData.Value))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
                {
                    HtmlElement entry = new HtmlElement();
                    entry.Html = link.OuterHtml;
                    entry.InnerHtml = link.InnerHtml;
                    entry.Selector = getSelector(link);
                    entry.Attributes = new Dictionary<string, string>();

                    foreach (var item in link.Attributes)
                    {
                        entry.Attributes.Add(item.Name, item.Value);
                    }

                    elements.Add(entry);
                }
            }

            return elements;
        }

        private string getSelector(HtmlNode node)
        {
            if (node.Attributes.Contains("id"))
            {
                return "#" + node.Attributes["id"].Value;
            }

            if (node.Attributes.Contains("elm"))
            {
                return String.Format("{0}[elm={1}]", node.Name, node.Attributes["elm"].Value);
            }

            return node.Name;
        }
    }
}
