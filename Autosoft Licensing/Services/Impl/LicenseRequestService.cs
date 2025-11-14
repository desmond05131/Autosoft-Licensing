using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
    public class LicenseRequestService : ILicenseRequestService
    {
        public ValidationResult ValidateRequest(LicenseRequest r)
        {
            var ctx = new ValidationContext(r);
            var results = new List<ValidationResult>();
            var ok = Validator.TryValidateObject(r, ctx, results, true);
            if (!ok) return results[0];
            if (r.RequestedPeriodMonths < 1 || r.RequestedPeriodMonths > 1200)
                return new ValidationResult("RequestedPeriodMonths out of range.");
            if (r.LicenseType != "Demo" && r.LicenseType != "Paid")
                return new ValidationResult("Invalid LicenseType.");
            return ValidationResult.Success;
        }

        public string ToArlJson(LicenseRequest r)
        {
            return JsonConvert.SerializeObject(r, Formatting.Indented);
        }

        public void SaveArl(string path, LicenseRequest r)
        {
            var vr = ValidateRequest(r);
            if (vr != ValidationResult.Success)
                throw new ValidationException(vr.ErrorMessage ?? UiMessages.InvalidLicenseRequest);
            File.WriteAllText(path, ToArlJson(r));
        }
    }
}