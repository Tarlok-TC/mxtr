using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace mxtrAutomation.Common.Extensions
{
    public static class XmlExtensions
    {
        public static string ToXml<T>(this T data)
            where T : class
        {
            StringBuilder sb = new StringBuilder(string.Empty);

            using (StringWriter writer = new StringWriter(sb))
            {
                XmlSerializer xs = new XmlSerializer(data.GetType());
                xs.Serialize(writer, data);
            }

            return sb.ToString();
        }

        public static T ToObject<T>(this string xml)
            where T : class
        {
            if (xml.IsNullOrEmpty())
                return null;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            if (doc.DocumentElement == null)
                return null;

            XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            return xs.Deserialize(reader) as T;
        }

        public static string ToXmlString(this XElement xElement)
        {
            XmlWriterSettings xmlWriterSettings =
                new XmlWriterSettings
                    {
                        Encoding = Encoding.UTF8,
                        OmitXmlDeclaration = true,
                        Indent = false
                    };

            return xElement.ToXmlString(xmlWriterSettings);
        }

        public static string ToXmlString(this XElement xElement, XmlWriterSettings xmlWriterSettings)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (XmlWriter xw = XmlWriter.Create(memoryStream, xmlWriterSettings))
            {
                xElement.WriteTo(xw);
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
