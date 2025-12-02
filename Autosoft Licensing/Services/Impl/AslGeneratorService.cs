using System;
using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using System.Text;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services.Impl
{
    public class AslGeneratorService : IAslGeneratorService
    {
        private readonly ILicenseService _licenseService;
        private readonly IFileService _fileService;
        private readonly ILicenseKeyGenerator _keyGenerator;
        private readonly IValidationService _validation;

        /// <summary>
        /// Construct the adapter with explicit dependencies to make wiring testable and avoid static lookups.
        /// </summary>
        public AslGeneratorService(ILicenseService licenseService,
                                   IFileService fileService,
                                   ILicenseKeyGenerator keyGenerator,
                                   IValidationService validation)
        {
            _licenseService = licenseService ?? throw new ArgumentNullException(nameof(licenseService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _keyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
            _validation = validation ?? throw new ArgumentNullException(nameof(validation));
        }

        /// <summary>
        /// Create a Base64 ASL using the core LicenseService. If ensureLicenseKey is true and the provided
        /// LicenseData lacks a LicenseKey, this method will request one from the injected ILicenseKeyGenerator.
        /// NOTE: Key/IV provisioning for production must be implemented securely (DPAPI/HSM). TODO.
        /// </summary>
        public string CreateAsl(LicenseData data, byte[] key, byte[] iv, bool ensureLicenseKey = true)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (iv == null) throw new ArgumentNullException(nameof(iv));

            try
            {
                if (ensureLicenseKey && string.IsNullOrWhiteSpace(data.LicenseKey))
                {
                    data.LicenseKey = _keyGenerator.GenerateKey(data.CompanyName, data.ProductID);
                }

                // Build canonical JSON (without checksum) using centralized validation service
                string canonicalWithoutChecksum = _validation.BuildCanonicalJson(data);
                // Compute checksum over canonical JSON bytes (UTF-8, no BOM)
                string checksum;
                if (ServiceRegistry.Encryption != null)
                {
                    checksum = ServiceRegistry.Encryption.ComputeSha256Hex(Encoding.UTF8.GetBytes(canonicalWithoutChecksum ?? string.Empty));
                }
                else
                {
                    // fallback - local simple compute
                    using var sha = System.Security.Cryptography.SHA256.Create();
                    var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(canonicalWithoutChecksum ?? string.Empty));
                    checksum = string.Concat(Array.ConvertAll(hash, b => b.ToString("x2")));
                }

                // Inject checksum into payload
                data.ChecksumSHA256 = checksum;

                // Serialize final canonical JSON including checksum (sorted keys)
                var finalJson = CanonicalJsonSerializer.Serialize(data);

                // Encrypt final JSON and return Base64 ASL
                if (ServiceRegistry.Encryption != null)
                {
                    return ServiceRegistry.Encryption.EncryptJsonToAsl(finalJson, key, iv);
                }
                else
                {
                    // If encryption service unavailable, fallback to using ILicenseService if it handles encryption
                    // Keep compatibility: try license service as last resort
                    return _licenseService.GenerateAsl(data, key, iv);
                }
            }
            catch (ValidationException)
            {
                // Preserve validation exceptions (invalid LicenseData etc.) so callers can present precise feedback.
                throw;
            }
            catch
            {
                // Hide internal details from UI; present user-safe message per spec.
                throw new InvalidOperationException("Operation failed. Contact admin.");
            }
        }

        /// <summary>
        /// Create ASL (see CreateAsl) and persist to disk using injected IFileService.WriteFileBase64.
        /// Wrap IO/crypto errors and expose a user-safe InvalidOperationException on failure.
        /// </summary>
        public void CreateAndSaveAsl(LicenseData data, string path, byte[] key, byte[] iv, bool ensureLicenseKey = true)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            try
            {
                var base64Asl = CreateAsl(data, key, iv, ensureLicenseKey);
                _fileService.WriteFileBase64(path, base64Asl);
            }
            catch (ValidationException)
            {
                // Re-throw validation so UI can show exact messages where appropriate.
                throw;
            }
            catch
            {
                // Hide internal details from UI; present user-safe message per spec.
                throw new InvalidOperationException("Operation failed. Contact admin.");
            }
        }
    }
}
