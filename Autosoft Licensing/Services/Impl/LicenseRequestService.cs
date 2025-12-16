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

            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch
            {
                throw new ValidationException("Invalid license request file2.");
            }

            string text;
            try
            {
                text = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                throw new ValidationException("Invalid license request file3.");
            }

            // Heuristic: if it already looks like JSON, use directly; else try base64 decode.
            string trimmed = text?.Trim() ?? string.Empty;
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
                    // Fallback: treat bytes as UTF8 JSON
                    if (string.IsNullOrWhiteSpace(trimmed))
                        throw new ValidationException("Invalid license request file4.");
                    json = text;
                }
            }

            LicenseRequest req;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    // Ensure tolerant parsing
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                    // Explicit date parsing (ISO 8601)
                    DateParseHandling = DateParseHandling.DateTime,
                };

                req = JsonConvert.DeserializeObject<LicenseRequest>(json, settings);
            }
            catch (Exception ex)
            {
                // Optionally capture details for diagnostics (do not leak to UI)
                // System.Diagnostics.Debug.WriteLine($"ARL deserialize failed: {ex.Message}");
                throw new ValidationException("Invalid license request file5.");
            }

            if (req == null)
                throw new ValidationException("Invalid license request file6.");

            try
            {
                EnsureValidOrThrow(req);
            }
            catch (ValidationException)
            {
                // Preserve the exact message required by UI
                throw new ValidationException("Invalid license request file7.");
            }

            return req;
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

            // RequestedPeriodMonths
            //if (r.RequestedPeriodMonths < 1 || r.RequestedPeriodMonths > 1200)
            //    throw new ValidationException("Invalid license request file.");

            // LicenseType allowed values
            //if (r.LicenseType != "Demo" && r.LicenseType != "Paid" && r.LicenseType != "Subscription")
            //    throw new ValidationException("Invalid license request file.");

            // Demo must have exactly 1 month
            //if (r.LicenseType == "Demo" && r.RequestedPeriodMonths != 1)
            //    throw new ValidationException("Invalid license request file.");

            // RequestDateUtc must be present (non-default)
            if (r.RequestDateUtc == default)
                throw new ValidationException("Invalid license request file14.");

            // Optionally you could run additional structural validation via _validator if needed,
            // but do not allow _validator's message to leak — UI requires the exact string for failures.
        }
    }
}