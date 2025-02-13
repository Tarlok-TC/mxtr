using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;


namespace mxtrAutomation.Websites.Platform.Helpers
{
    public static class Helper
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var type = typeof(TDestination);

            foreach (var prop in sourceProps)
            {
                object value = prop.GetValue(source, null);

                var destinationProps = type.GetProperty(prop.Name);
                if (destinationProps == null)
                    continue;

                if (prop.PropertyType != destinationProps.PropertyType)
                    continue;

                destinationProps.SetValue(destination, value, null);
            }
        }

        public static void WriteErrorLog(Exception ex, string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter str = new StreamWriter(fs);
            str.BaseStream.Seek(0, SeekOrigin.End);
            str.WriteLine(DateTime.Now.ToLongTimeString() + " " +
                          DateTime.Now.ToLongDateString());
            str.WriteLine("Message=> " + ex.Message);
            if (ex.InnerException != null)
            {
                str.WriteLine("InnerException=> " + ex.InnerException.Message);
            }
            str.WriteLine("---------------------------------");
            str.WriteLine(System.Environment.NewLine);
            str.Flush();
            str.Close();
            fs.Close();
        }

        public static void WriteErrorLog(String message, string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter str = new StreamWriter(fs);
            str.BaseStream.Seek(0, SeekOrigin.End);
            str.WriteLine(DateTime.Now.ToLongTimeString() + " " +
                          DateTime.Now.ToLongDateString());
            str.WriteLine("Message=> " + message);
            str.WriteLine("---------------------------------");
            str.WriteLine(System.Environment.NewLine);
            str.Flush();
            str.Close();
            fs.Close();
        }

        public static string GetDomainName()
        {
            string hostUrl = System.Web.HttpContext.Current.Request.Url.Host;
            if (hostUrl != "localhost")
            {
                string originalPath = new System.Uri(System.Web.HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
                hostUrl = originalPath.Substring(0, originalPath.LastIndexOf("/"));
                var host = new System.Uri(hostUrl.ToLower()).Host;
                return host.Substring(host.LastIndexOf('.', host.LastIndexOf('.') - 1) + 1);
            }
            else
            {
                return hostUrl;
            }
        }
    }
}