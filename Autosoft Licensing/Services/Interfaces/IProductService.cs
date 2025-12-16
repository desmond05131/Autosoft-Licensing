using System.Collections.Generic;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Returns the display name for a product by business ProductID.
        /// Returns empty string if product not found.
        /// </summary>
        string GetProductName(string productId);

        /// <summary>
        /// Returns module DTOs for a product. Returns empty enumerable if none or on error.
        /// </summary>
        IEnumerable<ModuleDto> GetModulesByProductId(string productId);

        /// <summary>
        /// Returns true if the product exists and is marked as soft-deleted; false otherwise.
        /// </summary>
        bool IsProductDeleted(string productId);
    }
}
