using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Newtonsoft.Json;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly IEncryptionService _crypto;
        public LicenseService(IEncryptionService crypto) => _crypto = crypto;

        public LicenseData ImportAslBase64(string base64Asl, byte[] key, byte[] iv)
        {
            string json;
            try
            {
                json = _crypto.DecryptAslToJson(base64Asl, key, iv);
            }
            catch
            {
                throw new ValidationException("Invalid or tampered license file.");
            }

            LicenseData data;
            try
            {
                data = JsonConvert.DeserializeObject<LicenseData>(json);
            }
            catch
            {
                throw new ValidationException("Invalid or tampered license file.");
            }

            // Verify checksum
            var withoutChecksum = JsonHelper.RemoveProperty(json, "ChecksumSHA256");
            var canon = JsonHelper.Canonicalize(withoutChecksum);
            if (!_crypto.VerifyChecksum(canon, data.ChecksumSHA256))
                throw new ValidationException("Invalid or tampered license file.");

            var vr = ValidateLicenseData(data);
            if (vr != ValidationResult.Success)
                throw new ValidationException(vr.ErrorMessage ?? "Invalid license file.");

            if (data.ValidToUtc < DateTime.UtcNow)
                throw new ValidationException(data.LicenseType == "Demo" ? "Demo license expired." : "License expired.");

            return data;
        }

        public ValidationResult ValidateLicenseData(LicenseData d)
        {
            var ctx = new ValidationContext(d);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(d, ctx, results, true))
                return results[0];
            if (d.LicenseType != "Demo" && d.LicenseType != "Paid")
                return new ValidationResult("Invalid license file.");
            return ValidationResult.Success;
        }
    }
}