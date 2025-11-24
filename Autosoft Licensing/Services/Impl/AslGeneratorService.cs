using System;
using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;

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

                var base64Asl = _licenseService.GenerateAsl(data, key, iv);
                return base64Asl;
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
