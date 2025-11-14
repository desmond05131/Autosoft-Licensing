using Autosoft_Licensing.Utils;
using Autosoft_Licensing.Data;

namespace Autosoft_Licensing.Services
{
    public static class ServiceRegistry
    {
        static ServiceRegistry()
        {
            // Wire the DB service once
            var factory = new SqlConnectionFactory("LicensingDb");
            Database = new LicenseDatabaseService(factory);
        }

        public static IClock Clock { get; } = new SystemClock();
        public static IValidationService Validation { get; } = new ValidationService();
        public static IEncryptionService Encryption { get; } = new EncryptionService();

        public static ILicenseDatabaseService Database { get; private set; }

        public static ILicenseRequestService LicenseRequest =>
            new LicenseRequestService(Validation);

        public static ILicenseService License =>
            new LicenseService(Encryption, Validation, Database, Clock);
    }
}