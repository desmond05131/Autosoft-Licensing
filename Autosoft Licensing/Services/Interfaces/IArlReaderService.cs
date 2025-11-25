using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    /// <summary>
    /// Lightweight UI-facing reader for .ARL request files.
    /// Implementations should throw <see cref="System.ComponentModel.DataAnnotations.ValidationException"/>
    /// with the exact message "Invalid license request file." on parse/validation failures so UI can show that string.
    /// </summary>
    public interface IArlReaderService
    {
        /// <summary>
        /// Parse and validate a .ARL file from disk. Throws ValidationException("Invalid license request file.") on failure.
        /// </summary>
        ArlRequest ParseArl(string path);

        /// <summary>
        /// Parse and validate a .ARL from a Base64 payload. Throws ValidationException("Invalid license request file.") on failure.
        /// </summary>
        ArlRequest ParseArlFromBase64(string base64);
    }
}
