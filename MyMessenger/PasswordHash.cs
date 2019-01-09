using System;
using System.Text;
using System.Security.Cryptography;

namespace MyMessenger
{
    public static class PasswordHashing
    {
        //A method that takes users' password and encrypts it
        public static String Sha256_hash(string passToEncrypt)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding encoded = Encoding.UTF8;
                Byte[] hashResult = hash.ComputeHash(encoded.GetBytes(passToEncrypt));

                foreach (Byte b in hashResult)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        //A method that generates a random salt to append to users' password
        public static string SaltGenarator()
        {
            var rndm = new Random();

            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            StringBuilder salt = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                salt.Append(characters[(int)(rndm.NextDouble() * characters.Length)]);
            }
            return salt.ToString();
        }
    }
}
