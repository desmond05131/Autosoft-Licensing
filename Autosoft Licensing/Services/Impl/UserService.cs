using System;
using System.Text;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public class UserService : IUserService
    {
        private readonly ILicenseDatabaseService _db;
        private readonly IEncryptionService _crypto;

        public UserService(ILicenseDatabaseService db, IEncryptionService crypto)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
        }

        public User GetUserById(int id) => _db.GetUserById(id);

        public User GetUserByUsername(string username) => _db.GetUserByUsername(username);

        public bool ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || password == null)
                return false;

            var user = GetUserByUsername(username);
            if (user == null)
                return false;

            var hash = _crypto.ComputeSha256Hex(Encoding.UTF8.GetBytes(password));
            return string.Equals(hash, user.PasswordHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}