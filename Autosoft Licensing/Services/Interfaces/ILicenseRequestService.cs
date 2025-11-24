using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseRequestService
    {
        string SerializeToArl(LicenseRequest request);
        void SaveArl(string path, LicenseRequest request);

        // Parse an .ARL file from disk, validate and return the LicenseRequest.
        // Throws ValidationException with exact message "Invalid license request file." on parse/IO/validation failures.
        LicenseRequest ParseArl(string path);

        // Parse an .ARL from a Base64 payload (helper). Throws ValidationException("Invalid license request file.") on failure.
        LicenseRequest ParseArlFromBase64(string base64);
    }
}