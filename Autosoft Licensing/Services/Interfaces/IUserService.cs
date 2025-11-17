using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface IUserService
    {
        User GetUserById(int id);
        User GetUserByUsername(string username);
        /// <summary>
        /// Validate credentials by username and plain-text password.
        /// Returns true if user exists and password SHA256 hash matches stored PasswordHash.
        /// </summary>
        bool ValidateCredentials(string username, string password);
    }
}