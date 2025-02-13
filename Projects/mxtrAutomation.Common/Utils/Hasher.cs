using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace mxtrAutomation.Common.Utils
{
    public static class Hasher
    {
        public static string GenerateHash(string plaintext, string salt)
        {
            using (HMACSHA1 sha = new HMACSHA1(Encoding.ASCII.GetBytes(salt)))
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(plaintext)))
            {
                return Convert.ToBase64String(sha.ComputeHash(ms));
            }
        }

        public static string GenerateSalt(int len)
        {
            const string avaliableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-=!@#$%^&*()_+";

            StringBuilder retVal = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < len; i++)
            {
                retVal.Append(avaliableChars[rand.Next(avaliableChars.Length - 1)]);
            }

            return retVal.ToString();
        }
    }
}
