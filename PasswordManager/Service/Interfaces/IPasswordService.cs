using PasswordManager.Model;

namespace PasswordManager.Service.Interfaces
{
    public interface IPasswordService
    {
        string GeneratePassword(PasswordTypes type, int length);

        PasswordStrength CheckPasswordStrength(string password);
    }
}
