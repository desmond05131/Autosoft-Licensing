using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
    public class ValidationService : IValidationService
    {
        public ValidationResult ValidateLicenseRequest(LicenseRequest r)
        {
            var ctx = new ValidationContext(r);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(r, ctx, results, true))
                return results[0];

            if (r.LicenseType == LicenseType.Demo && r.RequestedPeriodMonths != 1)
                return new ValidationResult("Demo license must request 1 month.");

            if (r.LicenseType == LicenseType.Subscription)
            {
                var allowed = new[] { 3, 6, 12, 24 };
                if (!allowed.Contains(r.RequestedPeriodMonths))
                    return new ValidationResult("Subscription license must be 3, 6, 12 or 24 months.");
            }

            return ValidationResult.Success;
        }

        public ValidationResult ValidateLicenseData(LicenseData d)
        {
            var ctx = new ValidationContext(d);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(d, ctx, results, true))
                return results[0];
            if (d.ValidToUtc <= d.ValidFromUtc)
                return new ValidationResult("Validity period invalid.");
            return ValidationResult.Success;
        }

        public bool IsExpired(LicenseData d, System.DateTime utcNow) => utcNow > d.ValidToUtc;

        /// <summary>
        /// Build deterministic canonical JSON for the given payload.
        /// Removes the ChecksumSHA256 property and returns pretty-printed (indented) canonical JSON for display.
        /// Matches the UI fallback behaviour.
        /// </summary>
        public string BuildCanonicalJson(LicenseData payload)
        {
            if (payload == null) return string.Empty;
            var json = JsonConvert.SerializeObject(payload, Formatting.None);
            var withoutChecksum = JsonHelper.RemoveProperty(json, "ChecksumSHA256");
            var canon = JsonHelper.Canonicalize(withoutChecksum);
            var token = JToken.Parse(canon);
            return token.ToString(Formatting.Indented);
        }
    }
}