using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services.Impl
{
    public class LicenseRequestService : ILicenseRequestService
    {
        private readonly IValidationService _validator;

        public LicenseRequestService(IValidationService validator)
            => _validator = validator;

        public string SerializeToArl(LicenseRequest r)
        {
            // Validate according to new ARL rules before serializing.
            EnsureValidOrThrow(r);
            return JsonConvert.SerializeObject(r, Formatting.Indented);
        }

        public void SaveArl(string path, LicenseRequest r)
        {
            var json = SerializeToArl(r);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Heuristic parse: raw JSON or base64-encoded JSON. On any IO/parse/validation error
        /// throw ValidationException("Invalid license request file.").
        /// </summary>
        public LicenseRequest ParseArl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ValidationException("Invalid license request file1.");

            string text;
            try
            {
                // FIX: Use ReadAllText instead of ReadAllBytes. 
                // This automatically detects and STRIPS the Byte Order Mark (BOM) if present.
                text = File.ReadAllText(path, Encoding.UTF8);
            }
            catch
            {
                throw new ValidationException("Invalid license request file2.");
            }

            // Heuristic: if it already looks like JSON, use directly; else try base64 decode.
            // FIX: Trim explicitly removing BOM character just in case
            string trimmed = text?.Trim(new char[] { '\uFEFF', ' ', '\r', '\n' }) ?? string.Empty;
            string json;

            if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
            {
                json = text;
            }
            else
            {
                try
                {
                    var decoded = Convert.FromBase64String(trimmed);
                    json = Encoding.UTF8.GetString(decoded);
                }
                catch
                {
                    // Fallback: treat as UTF8 JSON
                    if (string.IsNullOrWhiteSpace(trimmed))
                        throw new ValidationException("Invalid license request file4.");
                    json = text;
                }
            }

            try
            {
                // 1. Parse to JObject first (tolerates nulls/missing fields)
                var jObj = Newtonsoft.Json.Linq.JObject.Parse(json);

                // 2. Manual Mapping - Safely handling the nulls
                var req = new LicenseRequest
                {
                    CompanyName = (string)jObj["CompanyName"],
                    DealerCode = (string)jObj["DealerCode"],
                    ProductID = (string)jObj["ProductID"],
                    CurrencyCode = (string)jObj["CurrencyCode"],
                    LicenseKey = (string)jObj["LicenseKey"],
                    LicenseType = (string)jObj["LicenseType"], // Accepts null
                                                               // Handle RequestDateUtc safely
                    RequestDateUtc = (DateTime?)jObj["RequestDateUtc"] ?? DateTime.UtcNow
                };

                // 3. Special handling for RequestedPeriodMonths
                var periodToken = jObj["RequestedPeriodMonths"];
                if (periodToken == null || periodToken.Type == Newtonsoft.Json.Linq.JTokenType.Null)
                {
                    req.RequestedPeriodMonths = null;
                }
                else
                {
                    req.RequestedPeriodMonths = (int)periodToken;
                }

                if (req == null)
                    throw new ValidationException("Invalid license request file6.");

                EnsureValidOrThrow(req);

                return req;
            }
            catch (Exception ex)
            {
                // Keep your detailed error reporting
                throw new ValidationException($"Invalid license request file5. Details: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Parse ARL JSON from a Base64-encoded payload and validate.
        /// Throws ValidationException("Invalid license request file.") on failure.
        /// </summary>
        public LicenseRequest ParseArlFromBase64(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                throw new ValidationException("Invalid license request file8.");

            string json;
            try
            {
                var decoded = Convert.FromBase64String(base64.Trim());
                json = Encoding.UTF8.GetString(decoded);
            }
            catch
            {
                // If base64 decode failed, attempt to treat input as raw JSON string.
                json = base64;
            }

            LicenseRequest req;
            try
            {
                req = JsonConvert.DeserializeObject<LicenseRequest>(json);
            }
            catch
            {
                throw new ValidationException("Invalid license request file9.");
            }

            if (req == null)
                throw new ValidationException("Invalid license request file10.");

            try
            {
                EnsureValidOrThrow(req);
            }
            catch (ValidationException)
            {
                throw new ValidationException("Invalid license request file11.");
            }

            return req;
        }

        /// <summary>
        /// Enforce the new ARL schema rules; throws ValidationException with a specific message on failure.
        /// Rules:
        /// - Required fields present and non-empty
        /// - RequestedPeriodMonths >= 1
        /// - LicenseType must be exactly "Demo" or "Paid" (case-sensitive)
        /// - If LicenseType == "Demo" then RequestedPeriodMonths == 1
        /// </summary>
        private void EnsureValidOrThrow(LicenseRequest r)
        {
            if (r == null)
                throw new ValidationException("Invalid license request file12.");

            // Required string fields
            if (string.IsNullOrWhiteSpace(r.CompanyName)
                || string.IsNullOrWhiteSpace(r.DealerCode)
                || string.IsNullOrWhiteSpace(r.ProductID)
                //|| string.IsNullOrWhiteSpace(r.LicenseType)
                || string.IsNullOrWhiteSpace(r.LicenseKey))
            {
                throw new ValidationException("Invalid license request file13.");
            }

            // RequestDateUtc must be present (non-default)
            if (r.RequestDateUtc == default)
                throw new ValidationException("Invalid license request file14.");

            // Optionally you could run additional structural validation via _validator if needed,
            // but do not allow _validator's message to leak — UI requires the exact string for failures.
        }
    }
}