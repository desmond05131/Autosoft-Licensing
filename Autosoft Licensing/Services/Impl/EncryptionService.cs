using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string EncryptJsonToAsl(string jsonWithChecksum, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
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

        public string DecryptAslToJson(string base64Asl, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
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

        public string ComputeSha256Hex(byte[] data)
        {
            return Autosoft_Licensing.Utils.ChecksumHelper.ComputeSha256HexLower(data);
        }

        public bool VerifyChecksum(string jsonWithoutChecksum, string checksumHex)
        {
            var bytes = CanonicalJsonSerializer.SerializeToUtf8Bytes(JObject.Parse(jsonWithoutChecksum));
            var hex = Autosoft_Licensing.Utils.ChecksumHelper.ComputeSha256HexLower(bytes);
            return string.Equals(hex, checksumHex, StringComparison.OrdinalIgnoreCase);
        }

        public string BuildJsonWithChecksum(LicenseData licenseWithoutChecksum)
        {
            // Create a JObject from the license data.
            var jObject = JObject.FromObject(licenseWithoutChecksum);

            // Remove the checksum property for hashing.
            jObject.Property("ChecksumSHA256")?.Remove();

            // Create the canonical representation for hashing.
            var canonicalForHashing = CanonicalJsonSerializer.Serialize(jObject);
            var bytes = Encoding.UTF8.GetBytes(canonicalForHashing);
            var checksum = Autosoft_Licensing.Utils.ChecksumHelper.ComputeSha256HexLower(bytes);

            // Add the checksum back to the original JObject.
            jObject.AddFirst(new JProperty("ChecksumSHA256", checksum));

            // Return the final canonical JSON with the checksum.
            return CanonicalJsonSerializer.Serialize(jObject);
        }
    }
}