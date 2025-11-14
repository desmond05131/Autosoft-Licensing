using Autosoft_Licensing.Data;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Services
{
    public sealed class ServiceRegistry
    {
        public SqlConnectionFactory ConnectionFactory { get; }
        public IClock Clock { get; }
        public IEncryptionService Encryption { get; }
        public IValidationService Validation { get; }
        public IFileService Files { get; }
        public ILicenseDatabaseService LicenseDb { get; }
        public ILicenseRequestService LicenseRequests { get; }
        public ILicenseService Licenses { get; }
        public ILicenseValidationFacade LicenseValidation { get; }

        private ServiceRegistry(
            SqlConnectionFactory cf,
            IClock clock,
            IEncryptionService enc,
            IValidationService val,
            IFileService files,
            ILicenseDatabaseService ldb,
            ILicenseRequestService lreq,
            ILicenseService lsvc,
            ILicenseValidationFacade lval)
        {
            ConnectionFactory = cf;
            Clock = clock;
            Encryption = enc;
            Validation = val;
            Files = files;
            LicenseDb = ldb;
            LicenseRequests = lreq;
            Licenses = lsvc;
            LicenseValidation = lval;
        }

        public static ServiceRegistry CreateDefault()
        {
            var cf = new SqlConnectionFactory("LicensingDb");
            var clock = new SystemClock();
            var enc = new EncryptionService();
            var val = new ValidationService();
            var files = new FileService();
            var ldb = new LicenseDatabaseService(cf, clock);
            var lreq = new LicenseRequestService();
            var lsvc = new LicenseService(enc);
            var lval = new LicenseValidationFacade(ldb, clock);
            return new ServiceRegistry(cf, clock, enc, val, files, ldb, lreq, lsvc, lval);
        }
    }
}