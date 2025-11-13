using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseService
    {
        LicenseData ImportAslBase64(string base64Asl, byte[] key, byte[] iv); // throws on invalid/tampered
        ValidationResult ValidateLicenseData(LicenseData data);
    }
}