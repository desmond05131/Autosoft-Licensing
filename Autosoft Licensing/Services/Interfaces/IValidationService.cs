using Autosoft_Licensing.Models;
using System.ComponentModel.DataAnnotations;

namespace Autosoft_Licensing.Services
{
    public interface IValidationService
    {
        ValidationResult ValidateLicenseRequest(LicenseRequest request);
        ValidationResult ValidateLicenseData(LicenseData data);
        bool IsExpired(LicenseData data, System.DateTime utcNow);

        // Build a deterministic canonical JSON representation for display (checksum removed).
        string BuildCanonicalJson(LicenseData payload);
    }
}