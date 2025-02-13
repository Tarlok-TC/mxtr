using System.Web.Script.Serialization;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        public static object FromJson(this string jsonString)
        {
            return new JavaScriptSerializer().DeserializeObject(jsonString);
        }

        public static T FromJson<T>(this string jsonString)
        {
            return new JavaScriptSerializer().Deserialize<T>(jsonString);
        }
    }
}
