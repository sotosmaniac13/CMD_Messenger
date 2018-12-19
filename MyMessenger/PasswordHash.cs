using System;
using System.Text;
using System.Security.Cryptography;

namespace MyMessenger
{
    public class PasswordHashing
    {
        public static String sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding encoded = Encoding.UTF8;
                Byte[] hashResult = hash.ComputeHash(encoded.GetBytes(value));

                foreach (Byte b in hashResult)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
