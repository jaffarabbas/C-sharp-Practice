using NUnit.Framework;
using ApiTemplate.Helper;
using System;
using System.Text;

namespace ApiTemplate.Tests
{
    public class HashPasswordTests
    {
        private string password;

        [SetUp]
        public void Setup()
        {
            password = "MyTestPassword123!";
        }

        [Test]
        public void GenerateSalt_ShouldReturnDifferentSalts()
        {
            var salt1 = HashPassword.GenerateSalt();
            var salt2 = HashPassword.GenerateSalt();

            Assert.That(salt1.Length, Is.EqualTo(16));
            Assert.That(salt2.Length, Is.EqualTo(16));
            Assert.That(Convert.ToBase64String(salt2), Is.Not.EqualTo(Convert.ToBase64String(salt1)));
        }

        [Test]
        public void Hash_WithSHA256_ShouldProduceConsistentResult()
        {
            var salt = HashPassword.GenerateSalt();
            var hash1 = HashPassword.Hash(password, salt, "sha256");
            var hash2 = HashPassword.Hash(password, salt, "sha256");

            Assert.That(Convert.ToBase64String(hash2), Is.EqualTo(Convert.ToBase64String(hash1)));
        }

        [Test]
        public void Hash_WithSHA1_ShouldProduceConsistentResult()
        {
            var salt = HashPassword.GenerateSalt();
            var hash1 = HashPassword.Hash(password, salt, "sha1");
            var hash2 = HashPassword.Hash(password, salt, "sha1");

            Assert.That(Convert.ToBase64String(hash2), Is.EqualTo(Convert.ToBase64String(hash1)));
        }

        [Test]
        public void Hash_WithUnsupportedType_ShouldThrow()
        {
            var salt = HashPassword.GenerateSalt();

            Assert.Throws<NotSupportedException>(() =>
                HashPassword.Hash(password, salt, "md5"));
        }

        [Test]
        public void HashPBKDF2_ShouldBeDeterministic()
        {
            var salt = HashPassword.GenerateSalt();
            var hash1 = HashPassword.HashPBKDF2(password, salt);
            var hash2 = HashPassword.HashPBKDF2(password, salt);

            Assert.That(Convert.ToBase64String(hash2), Is.EqualTo(Convert.ToBase64String(hash1)));
        }

        [Test]
        public void ByteArrayToBase64_And_Back_ShouldMatchOriginal()
        {
            var salt = HashPassword.GenerateSalt();
            var base64 = HashPassword.ByteArrayToBase64(salt);
            var decoded = HashPassword.Base64ToByteArray(base64);

            Assert.That(Convert.ToBase64String(decoded), Is.EqualTo(Convert.ToBase64String(salt)));
        }

        [Test]
        public void ByteArrayToHexString_ShouldBeCorrect()
        {
            byte[] sample = Encoding.UTF8.GetBytes("abc");
            var hex = HashPassword.ByteArrayToHexString(sample);

            Assert.That(hex, Is.EqualTo("616263")); // 'a' = 61, 'b' = 62, 'c' = 63
        }

        [Test]
        public void StringToByteArray_ShouldParseHexCorrectly()
        {
            var hex = "616263";
            var bytes = HashPassword.StringToByteArray(hex);
            var result = Encoding.UTF8.GetString(bytes);

            Assert.That(result, Is.EqualTo("abc"));
        }

        [Test]
        public void Password_Verification_ShouldSucceed()
        {
            var salt = HashPassword.GenerateSalt();
            var hash = HashPassword.Hash(password, salt, "sha256");

            var enteredHash = HashPassword.Hash(password, salt, "sha256");

            Assert.That(Convert.ToBase64String(enteredHash), Is.EqualTo(Convert.ToBase64String(hash)));
        }
    }
}
