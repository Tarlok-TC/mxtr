using System;
using System.Security.Cryptography;
using System.Text;

namespace mxtrAutomation.Common.Utils
{
    public class Password
    {
        public string Value { get; private set; }

        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static readonly string keyspace = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public Password(string password)
        {
            Value = password;
        }

        public HashedPassword Hash(int saltLength)
        {
            string salt = Hasher.GenerateSalt(saltLength);
            string hashValue = Hasher.GenerateHash(Value, salt);

            return new HashedPassword(hashValue, salt);
        }

        /// <summary>
        /// Creates a new alphanumeric password.
        /// </summary>
        /// <param name="length">The length of the password.</param>
        public static Password Create(int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                byte[] container = new byte[sizeof(Int64)];
                rng.GetBytes(container);

                long randomNumber = Math.Abs(BitConverter.ToInt64(container, 0));
                int randomIndex = (int)(randomNumber % keyspace.Length);
                char randomCharacter = keyspace[randomIndex];

                sb.Append(randomCharacter);
            }

            return new Password(sb.ToString());
        }
    }
}
