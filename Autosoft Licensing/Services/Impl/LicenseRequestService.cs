using System.ComponentModel.DataAnnotations;
using System.IO;
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
            File.WriteAllText(path, json);
        }
    }
}