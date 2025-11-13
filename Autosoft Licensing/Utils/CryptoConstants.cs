using System;
using System.Configuration;

namespace Autosoft_Licensing.Utils
{
    public static class CryptoConstants
    {
        // Base64-encoded 32-byte key and 16-byte IV for AES-256 (dev only; replace in production)
        public static byte[] AesKey => ReadBase64("Crypto:AesKey", required: true, expectedLength: 32);
        public static byte[] AesIV  => ReadBase64("Crypto:AesIV",  required: true, expectedLength: 16);

        // Feature toggle: whether to store raw Base64 .ASL/.ARL in DB
        public static bool StoreRawFiles =>
            ReadBool("FeatureToggles:StoreRawFiles", defaultValue: true);

        private static byte[] ReadBase64(string key, bool required, int expectedLength)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                if (required)
                    throw new InvalidOperationException($"Missing appSetting '{key}'.");
                return Array.Empty<byte>();
            }

            try
            {
                var bytes = Convert.FromBase64String(value);
                if (expectedLength > 0 && bytes.Length != expectedLength)
                    throw new InvalidOperationException($"appSetting '{key}' must decode to {expectedLength} bytes.");
                return bytes;
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException($"appSetting '{key}' must be Base64.", ex);
            }
        }

        private static bool ReadBool(string key, bool defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            return value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value.Equals("1");
        }
    }
}