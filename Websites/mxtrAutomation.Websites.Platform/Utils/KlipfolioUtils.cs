using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;

namespace mxtrAutomation.Websites.Platform.Utils
{
    public interface IKlipfolioUtils
    {
        string GenerateSSOToken(string KlipfolioSSOSecretKey, string KlipfolioCompanyID, string userDetails);
    }
    public class KlipfolioUtils : IKlipfolioUtils
    {
        public string GenerateSSOToken(string KlipfolioSSOSecretKey, string KlipfolioCompanyID, string userDetails)
        {
            //Generates random initialization vector
            string initVector = generateInitVector();
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);

            byte[] keyBytesLong;
            using (SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider())
            {
                keyBytesLong = sha.ComputeHash(Encoding.UTF8.GetBytes(KlipfolioSSOSecretKey + KlipfolioCompanyID));
            }
            byte[] keyBytes = new byte[16];
            Array.Copy(keyBytesLong, keyBytes, 16);

            byte[] textBytes = Encoding.UTF8.GetBytes(userDetails);

            // Encrypt the string to an array of bytes
            byte[] encrypted = encryptStringToBytes_AES(textBytes, keyBytes, initVectorBytes);
            string encoded = Convert.ToBase64String(encrypted);
            return encoded;
            //return HttpUtility.UrlEncode(encoded);
        }
        #region Private Method
        private static string generateInitVector()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 16; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
        static byte[] encryptStringToBytes_AES(byte[] textBytes, byte[] Key, byte[] IV)
        {
            // Declare the stream used to encrypt to an in memory
            // array of bytes and the RijndaelManaged object
            // used to encrypt the data.
            using (MemoryStream msEncrypt = new MemoryStream())
            using (RijndaelManaged aesAlg = new RijndaelManaged())
            {
                // Provide the RijndaelManaged object with the specified key and IV.
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor();

                // Create the streams used for encryption.
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(IV, 0, IV.Length);
                    csEncrypt.Write(textBytes, 0, textBytes.Length);
                    csEncrypt.FlushFinalBlock();
                }

                byte[] encrypted = msEncrypt.ToArray();
                // Return the encrypted bytes from the memory stream.
                return encrypted;
            }
        }
        #endregion
    }
}