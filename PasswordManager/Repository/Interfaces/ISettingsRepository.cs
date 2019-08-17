using PasswordManager.Model;

namespace PasswordManager.Repository.Interfaces
{
    /// <summary>
    /// Provides an access to the application's configuration
    /// </summary>
    public interface ISettingsRepository
    {
        /// <summary>
        /// Load the application's settings into the cache
        /// </summary>
        /// <returns></returns>
        SettingsModel GetSettings();

        /// <summary>
        /// Write the given settings to the storage
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        bool WriteSettings(SettingsModel settings);
    }
}
