using System;
using System.Security.Cryptography;
using System.Text;

namespace ApiTemplate.Helper
{
    public static class HashPassword
    {
        public static byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public static byte[] Hash(string password, byte[] salt, string type = "sha256")
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            if (salt == null || salt.Length == 0)
                throw new ArgumentNullException(nameof(salt));

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            return type.ToLower() switch
            {
                "sha256" => SHA256.Create().ComputeHash(saltedPassword),
                "sha1" => SHA1.Create().ComputeHash(saltedPassword),
                _ => throw new NotSupportedException($"Hash type '{type}' is not supported.")
            };
        }

        public static byte[] StringToByteArray(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentNullException(nameof(hex));

            int length = hex.Length;
            byte[] byteArray = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                byteArray[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return byteArray;
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public static string ByteArrayToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] Base64ToByteArray(string base64)
        {
            return Convert.FromBase64String(base64);
        }

        // 🔐 Recommended: Use PBKDF2 for secure password storage
        public static byte[] HashPBKDF2(string password, byte[] salt, int iterations = 100_000, int keySize = 32)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(keySize);
        }
    }
}
