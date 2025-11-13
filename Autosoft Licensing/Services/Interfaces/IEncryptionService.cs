using System;

namespace Autosoft_Licensing.Services
{
    public interface IEncryptionService
    {
        string EncryptJsonToAsl(string jsonWithChecksum, byte[] key, byte[] iv); // returns Base64
        string DecryptAslToJson(string base64Asl, byte[] key, byte[] iv);        // returns JSON
        string ComputeSha256Hex(byte[] data);
        bool VerifyChecksum(string jsonWithoutChecksum, string checksumHex);
    }
}