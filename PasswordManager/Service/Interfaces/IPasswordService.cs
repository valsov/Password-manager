using PasswordManager.Model;

namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service handling password generation and management
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Generate a password with the given characteristics
        /// </summary>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        string GeneratePassword(PasswordTypes type, int length);

        /// <summary>
        /// Compute the strength level of the given string as a password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        PasswordStrength CheckPasswordStrength(string password);
    }
}
