using System.Text;
using System.Web;

namespace mxtrAutomation.Web.Common.Extensions
{
    public static class Base64Extensions
    {
        public static string ToBase64(this string toEncode)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(toEncode);
            return HttpServerUtility.UrlTokenEncode(encbuff);
        }

        public static string FromBase64(this string toDecode)
        {
            byte[] decbuff = HttpServerUtility.UrlTokenDecode(toDecode);
            return Encoding.UTF8.GetString(decbuff);
        }

#if false
        public static string EncodeTo64(this string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.Unicode.GetBytes(toEncode);

            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string DecodeFrom64(this string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);

            return Encoding.Unicode.GetString(encodedDataAsBytes);
        }
#endif
    }
}
