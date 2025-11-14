using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
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
            // Ensure checksum field is recomputed; ignore any existing value
            data.ChecksumSHA256 = null;
            var vr = _validator.ValidateLicenseData(data);
            if (vr != ValidationResult.Success)
                throw new ValidationException(vr.ErrorMessage);

            // Build canonical JSON with checksum
            var jsonWithChecksum = _crypto.BuildJsonWithChecksum(data);
            return _crypto.EncryptJsonToAsl(jsonWithChecksum, key, iv);
        }

        public LicenseMetadata Activate(LicenseData data, int? importedByUserId)
        {
            // Final status determination
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
                RawAslBase64 = null, // Can set if feature toggle to store
                ModuleCodes = data.ModuleCodes
            };

            // Duplicate key warning (non-blocking) could be surfaced later; for now just insert.
            var id = _db.InsertLicense(meta);
            _db.SetLicenseModules(id, meta.ModuleCodes);
            meta.Id = id;
            return meta;
        }

        public ValidationResult ValidateLicenseData(LicenseData d) => _validator.ValidateLicenseData(d);
    }
}