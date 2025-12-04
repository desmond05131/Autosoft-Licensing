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
                // TODO: log the exception
                return string.Empty;
            }
        }

        public IEnumerable<ModuleDto> GetModulesByProductId(string productId)
        {
            try
            {
                // Pass-through now returns ModuleDto with Description populated by the DB service
                var modules = _database.GetModulesForProduct(productId);
                return modules ?? new List<ModuleDto>();
            }
            catch
            {
                // TODO: log the exception
                return new List<ModuleDto>();
            }
        }
    }
}
