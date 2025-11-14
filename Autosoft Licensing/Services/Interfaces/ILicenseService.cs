using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseService
    {
        // Import (decrypt & validate)
        LicenseData ImportAslBase64(string base64Asl, byte[] key, byte[] iv);

        // Generate encrypted .ASL from LicenseData (LicenseData must not yet contain Checksum; it will be added)
        string GenerateAsl(LicenseData data, byte[] key, byte[] iv);

        // Activate (persist license & modules) returns metadata
        LicenseMetadata Activate(LicenseData data, int? importedByUserId);

        // Utility validation result
        ValidationResult ValidateLicenseData(LicenseData data);
    }
}