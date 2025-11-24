using Autosoft_Licensing.Models;
using System;

namespace Autosoft_Licensing.Services
{
    /// <summary>
    /// UI-friendly adapter to create .ASL payloads and persist them to disk.
    /// Delegates actual encryption to existing LicenseService and file IO to FileService.
    /// </summary>
    public interface IAslGeneratorService
    {
        /// <summary>
        /// Create a Base64-encoded .ASL from the given LicenseData.
        /// If <paramref name="ensureLicenseKey"/> is true and <paramref name="data"/>.LicenseKey is null/empty,
        /// the service will call the configured ILicenseKeyGenerator to generate one.
        /// </summary>
        /// <param name="data">Populated LicenseData (ValidFromUtc/ValidToUtc, LicenseType, etc.).</param>
        /// <param name="key">AES key bytes to use for encryption (dev/testing).</param>
        /// <param name="iv">AES IV bytes to use for encryption (dev/testing).</param>
        /// <param name="ensureLicenseKey">If true, generate a LicenseKey when missing.</param>
        /// <returns>Base64 .ASL string</returns>
        /// <exception cref="ArgumentNullException">If data, key or iv are null.</exception>
        /// <exception cref="InvalidOperationException">On internal crypto or IO failure ("Operation failed. Contact admin.").</exception>
        string CreateAsl(LicenseData data, byte[] key, byte[] iv, bool ensureLicenseKey = true);

        /// <summary>
        /// Create a Base64-encoded .ASL and persist it to <paramref name="path"/>.
        /// Uses ServiceRegistry.File.WriteFileBase64 for the actual write.
        /// </summary>
        /// <param name="data">Populated LicenseData.</param>
        /// <param name="path">Target file path to write the .ASL (Base64 decoded by FileService).</param>
        /// <param name="key">AES key bytes to use for encryption (dev/testing).</param>
        /// <param name="iv">AES IV bytes to use for encryption (dev/testing).</param>
        /// <param name="ensureLicenseKey">If true, generate a LicenseKey when missing.</param>
        /// <exception cref="ArgumentNullException">If data, key, iv or path are null.</exception>
        /// <exception cref="InvalidOperationException">On internal crypto or IO failure ("Operation failed. Contact admin.").</exception>
        void CreateAndSaveAsl(LicenseData data, string path, byte[] key, byte[] iv, bool ensureLicenseKey = true);
    }
}
