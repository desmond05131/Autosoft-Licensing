using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
    public class ValidationService : IValidationService
    {
        public ValidationResult ValidateArl(LicenseRequest r)
        {
            var ctx = new ValidationContext(r);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(r, ctx, results, true))
                return new ValidationResult(UiMessages.InvalidLicenseRequestFile);

            if (r.RequestedPeriodMonths < 1 || r.RequestedPeriodMonths > 1200)
                return new ValidationResult(UiMessages.InvalidLicenseRequestFile);

            if (r.LicenseType != "Demo" && r.LicenseType != "Paid")
                return new ValidationResult(UiMessages.InvalidLicenseRequestFile);

            return ValidationResult.Success;
        }

        public ValidationResult ValidateAslData(LicenseData d)
        {
            var ctx = new ValidationContext(d);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(d, ctx, results, true))
                return new ValidationResult(UiMessages.InvalidLicenseFile);

            if (d.LicenseType != "Demo" && d.LicenseType != "Paid")
                return new ValidationResult(UiMessages.InvalidLicenseFile);

            return ValidationResult.Success;
        }
    }
}