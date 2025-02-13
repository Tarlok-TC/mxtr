
namespace mxtrAutomation.Common.Utils
{
    public class HashedPassword
    {
        public string Value { get; set; }
        public string Salt { get; set; }

        public HashedPassword(string hash, string salt)
        {
            Value = hash;
            Salt = salt;
        }

        public bool IsMatch(string plaintextPassword)
        {
            string hash = Hasher.GenerateHash(plaintextPassword, Salt);

            return hash == Value;
        }
    }
}
