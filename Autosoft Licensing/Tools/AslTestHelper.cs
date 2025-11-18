using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Tools
{
    internal static class AslTestHelper
    {
        // Dumps a generated ASL to a temp file and returns path.
        public static string DumpAslToTempFile(LicenseData data)
        {
            var asl = ServiceRegistry.License.GenerateAsl(data, CryptoConstants.AesKey, CryptoConstants.AesIV);
            var path = Path.Combine(Path.GetTempPath(), $"smoke_asl_{DateTime.UtcNow:yyyyMMddHHmmss}.txt");
            File.WriteAllText(path, asl);
            Debug.WriteLine("ASL dumped to: " + path);
            return path;
        }

        // Tamper one character and attempt import (expected: ValidationException)
        public static string TamperAndTryImport(string base64Asl)
        {
            if (string.IsNullOrEmpty(base64Asl)) throw new ArgumentNullException(nameof(base64Asl));
            var arr = base64Asl.ToCharArray();
            arr[arr.Length / 2] = arr[arr.Length / 2] == 'A' ? 'B' : 'A';
            var tampered = new string(arr);
            try
            {
                var imported = ServiceRegistry.License.ImportAslBase64(tampered, CryptoConstants.AesKey, CryptoConstants.AesIV);
                Debug.WriteLine("Unexpected: Import succeeded for tampered file.");
                return "IMPORT_SUCCEEDED";
            }
            catch (ValidationException vx)
            {
                Debug.WriteLine("Expected failure from tampered ASL: " + vx.Message);
                return "EXPECTED_FAILURE: " + vx.Message;
            }
        }

        // Create an expired license and confirm Import raises expiry error
        public static string CreateExpiredAndTryImport()
        {
            var now = DateTime.UtcNow;
            var expired = new LicenseData
            {
                CompanyName = "ExpiredCo",
                ProductID = "SAMPLE-PRODUCT",
                DealerCode = "DEALER-001",
                LicenseKey = "EXPIRED-KEY-001",
                LicenseType = Models.Enums.LicenseType.Demo,
                ValidFromUtc = now.AddMonths(-2).Date,
                ValidToUtc = now.AddMonths(-1).Date,
                CurrencyCode = "USD",
                ModuleCodes = new System.Collections.Generic.List<string> { "MODULE-001" }
            };

            var asl = ServiceRegistry.License.GenerateAsl(expired, CryptoConstants.AesKey, CryptoConstants.AesIV);
            try
            {
                var imported = ServiceRegistry.License.ImportAslBase64(asl, CryptoConstants.AesKey, CryptoConstants.AesIV);
                Debug.WriteLine("Unexpected: expired license imported OK.");
                return "IMPORT_SUCCEEDED";
            }
            catch (ValidationException vx)
            {
                Debug.WriteLine("Expected expiry failure: " + vx.Message);
                return "EXPECTED_FAILURE: " + vx.Message;
            }
        }
    }
}
