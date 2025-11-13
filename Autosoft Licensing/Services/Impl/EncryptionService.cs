using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string EncryptJsonToAsl(string jsonWithChecksum, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                var plainBytes = Encoding.UTF8.GetBytes(jsonWithChecksum);
                using var encryptor = aes.CreateEncryptor();
                var cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(cipher);
            }
        }

        public string DecryptAslToJson(string base64Asl, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                var cipher = Convert.FromBase64String(base64Asl);
                using var decryptor = aes.CreateDecryptor();
                var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                return Encoding.UTF8.GetString(plain);
            }
        }

        public string ComputeSha256Hex(byte[] data)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(data);
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        public bool VerifyChecksum(string jsonWithoutChecksum, string checksumHex)
        {
            var hex = ComputeSha256Hex(Encoding.UTF8.GetBytes(jsonWithoutChecksum));
            return string.Equals(hex, checksumHex, StringComparison.OrdinalIgnoreCase);
        }
    }
}