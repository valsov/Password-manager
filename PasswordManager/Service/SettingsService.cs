using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;

namespace PasswordManager.Service
{
    /// <summary>
    /// Service Handling the user's settings
    /// </summary>
    public class SettingsService : ISettingsService
    {
        ISettingsRepository settingsRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingsRepository"></param>
        public SettingsService(ISettingsRepository settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }

        /// <summary>
        /// Get the setting's database path
        /// </summary>
        /// <returns></returns>
        public string GetDatabasePath()
        {
            return settingsRepository.GetSettings().DatabasePath;
        }

        /// <summary>
        /// Save the given database path in the settings
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SaveDatabasePath(string path)
        {
            var settingsModel = settingsRepository.GetSettings();
            settingsModel.DatabasePath = path;
            return settingsRepository.WriteSettings(settingsModel);
        }
    }
}
