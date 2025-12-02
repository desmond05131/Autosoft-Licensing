using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Autosoft_Licensing.Utils
{
    /// <summary>
    /// Shared deterministic checksum helper (returns lowercase hex).
    /// </summary>
    public static class ChecksumHelper
    {
        /// <summary>
        /// Compute SHA-256 over provided bytes and return lowercase hex (64 chars).
        /// </summary>
        public static string ComputeSha256HexLower(byte[] data)
        {
            if (data == null) data = Array.Empty<byte>();
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(data);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }
    }
}
