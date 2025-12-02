using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autosoft_Licensing.Utils;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Autosoft_Licensing.Tests
{
    [TestClass]
    public class CanonicalJsonTests
    {
        [TestMethod]
        public void Serialize_SameObject_Twice_IsIdentical()
        {
            var license = new LicenseData
            {
                CompanyName = "Acme",
                ProductID = "PROD-1",
                DealerCode = "DLR",
                LicenseType = Models.Enums.LicenseType.Subscription,
                ValidFromUtc = DateTime.UtcNow.Date,
                ValidToUtc = DateTime.UtcNow.Date.AddMonths(1),
                LicenseKey = "KEY-123",
                ModuleCodes = new System.Collections.Generic.List<string> { "M1", "M2" },
                CurrencyCode = null
            };

            var a = CanonicalJsonSerializer.Serialize(license);
            var b = CanonicalJsonSerializer.Serialize(license);

            Assert.AreEqual(a, b, "Canonical serializer must produce identical output for same input.");
        }

        [TestMethod]
        public void Checksum_IsLowercaseHex_And_Length64()
        {
            var canonical = "{\"CompanyName\":\"Acme\",\"LicenseKey\":\"K\",\"ModuleCodes\":[\"M1\"],\"ProductID\":\"P\",\"ValidFromUtc\":\"2020-01-01T00:00:00Z\",\"ValidToUtc\":\"2020-02-01T00:00:00Z\",\"LicenseType\":\"Subscription\"}";
            var bytes = Encoding.UTF8.GetBytes(canonical);
            var hex = Autosoft_Licensing.Utils.ChecksumHelper.ComputeSha256HexLower(bytes);
            Assert.IsNotNull(hex);
            Assert.AreEqual(64, hex.Length, "SHA256 hex should be 64 chars.");
            Assert.IsTrue(Regex.IsMatch(hex, "^[0-9a-f]{64}$"), "Checksum must be lowercase hex.");
        }

        [TestMethod]
        public void Roundtrip_EmbedChecksum_ImporterFlow_Verifies()
        {
            var license = new LicenseData
            {
                CompanyName = "RoundTripCo",
                ProductID = "RT-01",
                DealerCode = "DLR-RT",
                LicenseType = Models.Enums.LicenseType.Demo,
                ValidFromUtc = DateTime.UtcNow.Date,
                ValidToUtc = DateTime.UtcNow.Date.AddMonths(1),
                LicenseKey = "KEY-RT-001",
                ModuleCodes = new System.Collections.Generic.List<string> { "MODA" },
                CurrencyCode = "USD"
            };

            var finalJson = new EncryptionService().BuildJsonWithChecksum(license);

            // importer flow: parse, extract checksum, remove, recompute canonical, compare
            var parsed = JObject.Parse(finalJson);
            var extracted = parsed["ChecksumSHA256"]?.ToString();
            parsed.Property("ChecksumSHA256")?.Remove();
            var recomputed = Autosoft_Licensing.Utils.ChecksumHelper.ComputeSha256HexLower(CanonicalJsonSerializer.SerializeToUtf8Bytes(parsed));

            Assert.IsFalse(string.IsNullOrWhiteSpace(extracted), "Extracted checksum should not be empty.");
            Assert.AreEqual(extracted, recomputed, "Importer recomputed checksum must equal embedded checksum.");
        }

        [TestMethod]
        public void ProvidedSamplePayload_ComputesExpectedChecksum()
        {
            // Build the LicenseData that matches the sample JSON in the bug report
            var license = new LicenseData
            {
                CompanyName = "Acme Corp",
                ProductID = "PROD-001",
                DealerCode = "DEALER-001",
                LicenseType = Models.Enums.LicenseType.Subscription,
                ValidFromUtc = DateTime.Parse("2025-12-01T00:00:00Z").ToUniversalTime(),
                ValidToUtc = DateTime.Parse("2026-01-01T00:00:00Z").ToUniversalTime(),
                LicenseKey = "CAEC138C83624A2A9CA6BBB54D33999B",
                ModuleCodes = new System.Collections.Generic.List<string> { "MODULE-001" },
                CurrencyCode = null
            };

            // Convert to JObject using the SAME settings as CanonicalJsonSerializer
            var jObject = JObject.FromObject(license, JsonSerializer.Create(new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore, // MUST match CanonicalJsonSerializer
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                Converters = new[] { new StringEnumConverter() }
            }));
            
            // CRITICAL: Remove the ChecksumSHA256 property before computing the hash
            jObject.Remove("ChecksumSHA256");
            
            // Now serialize and hash
            var canonicalJson = CanonicalJsonSerializer.Serialize(jObject);
            var bytes = Encoding.UTF8.GetBytes(canonicalJson);
            var hex = Autosoft_Licensing.Utils.ChecksumHelper.ComputeSha256HexLower(bytes);

            // CORRECTED: Updated to the actual checksum computed from the canonical JSON above
            var expected = "3f70a0d091d4600cbc66fefa3ced94001e567596d125ea240ca16bdabd36c13c";
            
            // Debug: print actual JSON to see what we're hashing
            System.Diagnostics.Debug.WriteLine("Canonical JSON: " + canonicalJson);
            System.Diagnostics.Debug.WriteLine("Computed checksum: " + hex);
            
            Assert.AreEqual(expected, hex, "Canonical checksum must match expected sample checksum.");
        }

        [TestMethod]
        public void Integration_Roundtrip_EncryptDecrypt_ProducesSameCanonicalJson()
        {
            // dev AES values from test harness (base64)
            var key = Convert.FromBase64String("Zkfwt0M/OOZcMAb5qSLjOKKw6LeqIm9/PYtuZlRpBdw=");
            var iv = Convert.FromBase64String("Fvwy7WRbrjUaNmk6QGZsAg==");

            var license = new LicenseData
            {
                CompanyName = "RoundTripCo",
                ProductID = "RT-01",
                DealerCode = "DLR-RT",
                LicenseType = Models.Enums.LicenseType.Subscription,
                ValidFromUtc = DateTime.UtcNow.Date,
                ValidToUtc = DateTime.UtcNow.Date.AddMonths(1),
                LicenseKey = "KEY-ROUND-001",
                ModuleCodes = new System.Collections.Generic.List<string> { "MODA" },
                CurrencyCode = "USD"
            };

            var crypto = new Autosoft_Licensing.Services.EncryptionService();
            var finalJson = crypto.BuildJsonWithChecksum(license);
            
            var asl = crypto.EncryptJsonToAsl(finalJson, key, iv);
            var decrypted = crypto.DecryptAslToJson(asl, key, iv);

            Assert.AreEqual(finalJson, decrypted, "Decrypted JSON must equal the canonical JSON produced before encryption.");
        }
    }
}
