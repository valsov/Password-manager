using PasswordManager.Model;

namespace PasswordManager.Repository.Interfaces
{
    public interface ISettingsRepository
    {
        SettingsModel GetSettings();

        bool WriteSettings(SettingsModel settings);
    }
}
