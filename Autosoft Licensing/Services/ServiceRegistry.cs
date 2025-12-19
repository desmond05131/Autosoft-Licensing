using Autosoft_Licensing.Utils;
using Autosoft_Licensing.Data;
using System;
using Autosoft_Licensing.Services.Impl; // added for LicenseRequestService and adapters

namespace Autosoft_Licensing.Services
{
    public static class ServiceRegistry
    {
        public static IClock Clock { get; } = new SystemClock();
        public static IValidationService Validation { get; } = new ValidationService();
        public static IEncryptionService Encryption { get; } = new EncryptionService();

        private static ILicenseDatabaseService _database;
        public static ILicenseDatabaseService Database
        {
            get => _database ?? throw new InvalidOperationException("ServiceRegistry.Database has not been initialized.");
            set => _database = value;
        }

        /// <summary>
        /// Convenience helper to initialize the database service from a connection string name.
        /// Call this from Program.Main, tests, or the Immediate Window instead of duplicating the construction.
        /// </summary>
        public static void InitializeDatabase(string connectionStringName = "LicensingDb")
        {
            // SqlConnectionFactory is now a static helper (SqlConnectionFactory.GetConnectionString()).
            // LicenseDatabaseService uses that helper internally, so don't attempt to construct or inject it.
            Database = new LicenseDatabaseService();
        }

        // Lightweight factories for other services
        public static ILicenseRequestService LicenseRequest =>
            new LicenseRequestService(Validation);

        public static ILicenseService License =>
            new LicenseService(Encryption, Validation, Database, Clock);

        public static IUserService User =>
            new UserService(Database, Encryption);

        // File IO helper
        public static IFileService File =>
            new FileService();

        // Facade used by the plugin to validate license at AutoCount login
        public static ILicenseValidationFacade LicenseValidation =>
            new LicenseValidationFacade(Database, Clock);

        // --- New lazy-backed services (TASK #5) ---
        private static IProductService _product;
        public static IProductService Product => _product ??= new ProductService(Database);

        private static ILicenseKeyGenerator _keyGen;
        public static ILicenseKeyGenerator KeyGenerator => _keyGen ??= new LicenseKeyGenerator(Database);

        private static IAslGeneratorService _aslGen;
        public static IAslGeneratorService AslGenerator => _aslGen ??= new AslGeneratorService(License, File, KeyGenerator, Validation);

        // ARL reader adapter (UI-friendly). Delegates to LicenseRequestService.
        private static IArlReaderService _arlReader;
        public static IArlReaderService ArlReader => _arlReader ??= new ArlReaderService(LicenseRequest);
    }
}