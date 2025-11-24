using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services.Impl
{
    /// <summary>
    /// Generates readable license keys using SHA256(company|product|timestamp|guid).
    /// Format: {PREFIX}-{XXXX}-{XXXX}-{XXXX}-{XXXX}-{XXXX}
    /// where PREFIX is up to 4 uppercase alphanumeric characters derived from productId (or "GEN"),
    /// and each XXXX is 4 hex characters (uppercase).
    ///
    /// Algorithm notes:
    /// - Create entropy string from inputs (company, product, timestamp, guid, optional seed)
    /// - Compute SHA-256, take first 20 hex chars (80 bits) and split into 5 groups of 4.
    /// - Prepend a short product-derived prefix for readability.
    /// - If ILicenseDatabaseService is provided it will check LicenseKeyExists and retry up to maxAttempts.
    ///
    /// TODO: For production, consider stronger uniqueness enforcement (DB unique constraint) and/or HSM.
    /// </summary>
    public class LicenseKeyGenerator : ILicenseKeyGenerator
    {
        private readonly Services.ILicenseDatabaseService _db;
        private readonly int _maxAttempts;

        public LicenseKeyGenerator(Services.ILicenseDatabaseService database = null, int maxAttempts = 5)
        {
            _db = database;
            _maxAttempts = Math.Max(1, maxAttempts);
        }

        public string GenerateKey(string companyName, string productId, int? seed = null)
        {
            for (int attempt = 0; attempt < _maxAttempts; attempt++)
            {
                var raw = BuildRawInput(companyName, productId, seed, attempt);
                var hex = ComputeSha256Hex(raw).ToUpperInvariant();

                // use first 20 hex chars => 5 groups of 4
                if (hex.Length < 20) // extremely unlikely
                    throw new InvalidOperationException("Operation failed. Contact admin.");

                var groups = Enumerable.Range(0, 5)
                    .Select(i => hex.Substring(i * 4, 4))
                    .ToArray();

                var prefix = BuildPrefix(productId);

                var key = $"{prefix}-{string.Join("-", groups)}";

                // If DB is present, check uniqueness; otherwise accept first generated.
                try
                {
                    if (_db == null || !_db.LicenseKeyExists(key))
                        return key;
                }
                catch
                {
                    // If DB check fails for any reason, do not expose internal error. Try again up to attempts.
                    // Continue to next attempt.
                }
            }

            // If we reach here we couldn't produce a unique key within attempts
            throw new InvalidOperationException("Operation failed. Contact admin.");
        }

        private static string BuildRawInput(string companyName, string productId, int? seed, int attempt)
        {
            var now = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var guid = Guid.NewGuid().ToString("N");
            var sb = new StringBuilder();
            sb.Append(companyName ?? string.Empty);
            sb.Append("|");
            sb.Append(productId ?? string.Empty);
            sb.Append("|");
            sb.Append(now);
            sb.Append("|");
            sb.Append(guid);
            sb.Append("|");
            if (seed.HasValue) sb.Append(seed.Value);
            sb.Append("|");
            sb.Append(attempt);
            return sb.ToString();
        }

        private static string ComputeSha256Hex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        private static string BuildPrefix(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return "GEN";

            // Keep only alphanumeric upper chars, take up to 4 chars.
            var filtered = new string(productId.ToUpperInvariant().Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrEmpty(filtered)) return "GEN";
            if (filtered.Length <= 4) return filtered;
            return filtered.Substring(0, 4);
        }
    }
}
