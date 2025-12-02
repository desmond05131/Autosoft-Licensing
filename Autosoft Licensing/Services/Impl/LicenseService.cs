using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;
using Autosoft_Licensing.Utils;
using Newtonsoft.Json.Linq;

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

            JObject parsed;
            try
            {
                parsed = JObject.Parse(json);
            }
            catch
            {
                throw new ValidationException("Invalid or tampered license file.");
            }

            // Extract checksum and remove before canonicalizing
            var extracted = parsed["ChecksumSHA256"]?.ToString();
            if (string.IsNullOrWhiteSpace(extracted))
                throw new ValidationException("Invalid or tampered license file.");

            parsed.Property("ChecksumSHA256")?.Remove();

            // Build canonical JSON from the parsed object (without checksum) and recompute
            var canonicalBytes = CanonicalJson.SerializeCanonicalToUtf8Bytes(parsed);
            var recomputed = ChecksumHelper.ComputeSha256HexLower(canonicalBytes);

            if (!string.Equals(recomputed, extracted, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Invalid or tampered license file.");

            LicenseData data;
            try
            {
                // Deserialize into LicenseData for validation and further use
                data = parsed.ToObject<LicenseData>();
                // Ensure checksum property restored on object for downstream code that expects it
                data.ChecksumSHA256 = extracted;
            }
            catch
            {
                throw new ValidationException("Invalid or tampered license file.");
            }

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

            // Build canonical JSON from object without checksum
            var j = JObject.FromObject(data);
            j.Property("ChecksumSHA256")?.Remove();

            var bytes = CanonicalJson.SerializeCanonicalToUtf8Bytes(j);
            var checksum = ChecksumHelper.ComputeSha256HexLower(bytes);

            // Inject checksum into final payload (checksum computed BEFORE adding it)
            j.AddFirst(new JProperty("ChecksumSHA256", checksum));

            // Final compact canonical JSON (sorted keys)
            var finalJson = CanonicalJson.SerializeCanonical(j);

            return _crypto.EncryptJsonToAsl(finalJson, key, iv);
        }

        public LicenseMetadata Activate(LicenseData data, string rawAslBase64 = null, int? importedByUserId = null)
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
                // Persist raw ASL only if configured
                RawAslBase64 = CryptoConstants.StoreRawFiles ? rawAslBase64 : null,
                ModuleCodes = data.ModuleCodes
            };

            var id = _db.InsertLicense(meta);
            meta.Id = id;
            return meta;
        }

        public ValidationResult ValidateLicenseData(LicenseData d) => _validator.ValidateLicenseData(d);
    }
}