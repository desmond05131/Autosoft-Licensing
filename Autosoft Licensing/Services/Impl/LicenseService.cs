using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly IEncryptionService _crypto;
        private readonly IValidationService _validator;
        private readonly ILicenseDatabaseService _db;
        private readonly IClock _clock;

        public LicenseService(IEncryptionService crypto,
                              IValidationService validator,
                              ILicenseDatabaseService db,
                              IClock clock)
        {
            _crypto = crypto;
            _validator = validator;
            _db = db;
            _clock = clock;
        }

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

            var withoutChecksum = JsonHelper.RemoveProperty(json, "ChecksumSHA256");
            var canon = JsonHelper.Canonicalize(withoutChecksum);
            if (!_crypto.VerifyChecksum(canon, data.ChecksumSHA256))
                throw new ValidationException("Invalid or tampered license file.");

            var vr = _validator.ValidateLicenseData(data);
            if (vr != ValidationResult.Success)
                throw new ValidationException(vr.ErrorMessage ?? "Invalid license file.");

            if (_validator.IsExpired(data, _clock.UtcNow))
            {
                if (data.LicenseType == LicenseType.Demo)
                    throw new ValidationException("Demo license expired.");
                throw new ValidationException("License expired.");
            }

            return data;
        }

        public string GenerateAsl(LicenseData data, byte[] key, byte[] iv)
        {
            // Validation should run against the required business fields,
            // but the ChecksumSHA256 property is computed by this method and therefore must be allowed missing.
            // Create a lightweight copy of the incoming data with a placeholder checksum so DataAnnotations
            // validation (which requires ChecksumSHA256) succeeds without mutating the original object.
            var copy = new LicenseData
            {
                CompanyName = data.CompanyName,
                ProductID = data.ProductID,
                DealerCode = data.DealerCode,
                CurrencyCode = data.CurrencyCode,
                LicenseType = data.LicenseType,
                ValidFromUtc = data.ValidFromUtc,
                ValidToUtc = data.ValidToUtc,
                LicenseKey = data.LicenseKey,
                ModuleCodes = data.ModuleCodes == null ? new System.Collections.Generic.List<string>() : new System.Collections.Generic.List<string>(data.ModuleCodes),
                ChecksumSHA256 = "placeholder-checksum"
            };

            var vr = _validator.ValidateLicenseData(copy);
            if (vr != ValidationResult.Success)
                throw new ValidationException(vr.ErrorMessage);

            // Ensure original object has no checksum before building the canonical JSON and computing real checksum.
            data.ChecksumSHA256 = null;
            var jsonWithChecksum = _crypto.BuildJsonWithChecksum(data);
            return _crypto.EncryptJsonToAsl(jsonWithChecksum, key, iv);
        }

        public LicenseMetadata Activate(LicenseData data, int? importedByUserId)
        {
            var status = _validator.IsExpired(data, _clock.UtcNow)
                ? LicenseStatus.Expired
                : LicenseStatus.Valid;

            var meta = new LicenseMetadata
            {
                CompanyName = data.CompanyName,
                ProductID = data.ProductID,
                DealerCode = data.DealerCode,
                LicenseKey = data.LicenseKey,
                LicenseType = data.LicenseType,
                ValidFromUtc = data.ValidFromUtc,
                ValidToUtc = data.ValidToUtc,
                CurrencyCode = data.CurrencyCode,
                Status = status,
                ImportedOnUtc = _clock.UtcNow,
                ImportedByUserId = importedByUserId,
                RawAslBase64 = null,
                ModuleCodes = data.ModuleCodes
            };

            var id = _db.InsertLicense(meta);
            _db.SetLicenseModules(id, meta.ModuleCodes);
            meta.Id = id;
            return meta;
        }

        public ValidationResult ValidateLicenseData(LicenseData d) => _validator.ValidateLicenseData(d);
    }
}