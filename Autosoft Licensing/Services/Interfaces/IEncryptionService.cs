using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    // Keep ONLY the encryption interface here. Remove any other types from this file.
    public interface IEncryptionService
    {
        string EncryptJsonToAsl(string jsonWithChecksum, byte[] key, byte[] iv);
        string DecryptAslToJson(string base64Asl, byte[] key, byte[] iv);

        string ComputeSha256Hex(byte[] data);
        bool VerifyChecksum(string jsonWithoutChecksum, string checksumHex);

        // Build canonical JSON with computed checksum injected
        string BuildJsonWithChecksum(LicenseData licenseWithoutChecksum);
    }
}