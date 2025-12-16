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

            // LicenseRequest.LicenseType is a string ("Demo" or "Paid")
            if (string.Equals(r.LicenseType, "Demo", StringComparison.Ordinal) && r.RequestedPeriodMonths != 1)
                return new ValidationResult("Demo license must request 1 month.");

            // Treat "Paid" ARL requests as subscription business rules
            if (string.Equals(r.LicenseType, "Paid", StringComparison.Ordinal))
            {
                var allowed = new[] { 3, 6, 12, 24 };
                if (!r.RequestedPeriodMonths.HasValue || !allowed.Contains(r.RequestedPeriodMonths.Value))
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
        /// Returns compact canonical JSON (no indentation) to match checksum computation everywhere.
        /// </summary>
        public string BuildCanonicalJson(LicenseData payload)
        {
            return CanonicalJsonSerializer.Serialize(payload);
        }
    }
}