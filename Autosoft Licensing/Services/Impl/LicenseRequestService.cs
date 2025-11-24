using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.Services.Impl
{
    public class LicenseRequestService : ILicenseRequestService
    {
        private readonly IValidationService _validator;

        public LicenseRequestService(IValidationService validator)
            => _validator = validator;

        public string SerializeToArl(LicenseRequest r)
        {
            var vr = _validator.ValidateLicenseRequest(r);
            if (vr != ValidationResult.Success)
                throw new ValidationException(vr.ErrorMessage);
            return JsonConvert.SerializeObject(r, Formatting.Indented);
        }

        public void SaveArl(string path, LicenseRequest r)
        {
            var json = SerializeToArl(r);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Parse an ARL file at the given path. Heuristic:
        /// - Read file bytes, convert to UTF8 string.
        /// - If the trimmed string starts with '{' or '[' treat as JSON.
        /// - Otherwise try to Base64-decode the trimmed text and treat decoded bytes as UTF8 JSON.
        /// - On any IO/parse/base64/validation errors throw ValidationException with exact message "Invalid license request file.".
        /// </summary>
        public LicenseRequest ParseArl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ValidationException("Invalid license request file.");

            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch
            {
                throw new ValidationException("Invalid license request file.");
            }

            string text;
            try
            {
                text = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                throw new ValidationException("Invalid license request file.");
            }

            // Heuristic: if it already looks like JSON, use directly; else try base64 decode.
            string trimmed = text?.Trim() ?? string.Empty;
            string json = null;

            if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
            {
                json = text;
            }
            else
            {
                // Try base64 decode of the trimmed text
                try
                {
                    var decoded = Convert.FromBase64String(trimmed);
                    json = Encoding.UTF8.GetString(decoded);
                }
                catch
                {
                    // Fallback: treat bytes as UTF8 JSON (some ARL files may be raw JSON not starting with brace due to BOM/whitespace)
                    if (string.IsNullOrWhiteSpace(trimmed))
                        throw new ValidationException("Invalid license request file.");
                    json = text;
                }
            }

            LicenseRequest req;
            try
            {
                req = JsonConvert.DeserializeObject<LicenseRequest>(json);
            }
            catch
            {
                throw new ValidationException("Invalid license request file.");
            }

            if (req == null)
                throw new ValidationException("Invalid license request file.");

            // Validate using existing validator. Per UI requirements, return the generic message on validation failure.
            var vr = _validator.ValidateLicenseRequest(req);
            if (vr != ValidationResult.Success)
                throw new ValidationException("Invalid license request file.");

            return req;
        }

        /// <summary>
        /// Parse ARL JSON from a Base64-encoded payload and validate.
        /// Throws ValidationException("Invalid license request file.") on failure.
        /// </summary>
        public LicenseRequest ParseArlFromBase64(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                throw new ValidationException("Invalid license request file.");

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
                throw new ValidationException("Invalid license request file.");
            }

            if (req == null)
                throw new ValidationException("Invalid license request file.");

            var vr = _validator.ValidateLicenseRequest(req);
            if (vr != ValidationResult.Success)
                throw new ValidationException("Invalid license request file.");

            return req;
        }
    }
}