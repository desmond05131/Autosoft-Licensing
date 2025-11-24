using System;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseKeyGenerator
    {
        /// <summary>
        /// Generate a new license key for the given company and product.
        /// Implementations should ensure a readable, business-friendly format and attempt uniqueness.
        /// If uniqueness cannot be guaranteed after a small number of retries the method should throw
        /// an exception with message "Operation failed. Contact admin." (exact string required).
        /// </summary>
        /// <param name="companyName">Company name (used as entropy, may be null/empty)</param>
        /// <param name="productId">Product business id (used as prefix, may be null/empty)</param>
        /// <param name="seed">Optional seed for deterministic generation in tests</param>
        /// <returns>Generated license key string</returns>
        string GenerateKey(string companyName, string productId, int? seed = null);
    }
}
