using Autosoft_Licensing.Utils;
using Autosoft_Licensing.Data;
using System;
using Autosoft_Licensing.Services.Impl; // added for LicenseRequestService

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
            Database = new LicenseDatabaseService(new SqlConnectionFactory(connectionStringName));
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
    }
}