using System;
using System.Collections.Generic;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly ILicenseDatabaseService _database;

        public ProductService(ILicenseDatabaseService database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public string GetProductName(string productId)
        {
            try
            {
                var p = _database.GetProductByProductId(productId);
                return p?.Name ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public IEnumerable<ModuleDto> GetModulesByProductId(string productId)
        {
            try
            {
                var modules = _database.GetModulesForProduct(productId);
                return modules ?? new List<ModuleDto>();
            }
            catch
            {
                return new List<ModuleDto>();
            }
        }

        // --- NEW METHOD: Check status ---
        // Ensure you add "bool IsProductDeleted(string productId);" to your IProductService interface as well.
        public bool IsProductDeleted(string productId)
        {
            try
            {
                var p = _database.GetProductByProductId(productId);
                return p != null && p.IsDeleted;
            }
            catch
            {
                return false;
            }
        }
    }
}