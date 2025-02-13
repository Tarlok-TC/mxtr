using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace mxtrAutomation.Common.Utils
{
    public class AesEncryptor
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public string Key { get; private set; }
        public string Iv { get; private set; }

        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;

        /// <summary>
        /// Encrypts text using the AES Algorith. Generates a random initialization vector.
        /// </summary>
        /// <param name="key">A 128, 192 or 256 bit key in base64</param>
        public AesEncryptor(string key) : this(key, GenerateIv()) { }

        /// <summary>
        /// Encrypts text using the AES Algorithm
        /// </summary>
        /// <param name="key">A 128, 192 or 256 bit key in base64</param>
        /// <param name="iv">A 128 bit initilization vector in base64</param>
        public AesEncryptor(string key, string iv)
        {
            RijndaelManaged rm = new RijndaelManaged();

            Key = key;
            Iv = iv;

            byte[] keyBytes = Convert.FromBase64String(key);
            byte[] ivBytes = Convert.FromBase64String(iv);

            encryptor = rm.CreateEncryptor(keyBytes, ivBytes);
            decryptor = rm.CreateDecryptor(keyBytes, ivBytes);
        }

        /// <summary>
        /// Encrypts a string value.
        /// </summary>
        /// <param name="value">The value to encrypt</param>
        /// <returns>An encrypted value in base64</returns>
        public string Encrypt(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();

                ms.Position = 0;
                byte[] encrypted = new byte[ms.Length];
                ms.Read(encrypted, 0, encrypted.Length);

                return Convert.ToBase64String(encrypted);
            }
        }

        /// <summary>
        /// Decrypts a base64 string value.
        /// </summary>
        /// <param name="value">The value to decrypt.</param>
        /// <returns>The original string value.</returns>
        public string Decrypt(string value)
        {
            byte[] bytes = Convert.FromBase64String(value);

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();

                ms.Position = 0;
                byte[] decrypted = new byte[ms.Length];
                ms.Read(decrypted, 0, decrypted.Length);

                return Encoding.UTF8.GetString(decrypted);
            }
        }

        /// <summary>
        /// Generates a 16 byte initialization vector in base64.
        /// </summary>
        /// <returns>A new initialization vector.</returns>
        public static string GenerateIv()
        {
            byte[] ivBytes = new byte[16];
            rng.GetBytes(ivBytes);

            return Convert.ToBase64String(ivBytes);
        }
    }
}
