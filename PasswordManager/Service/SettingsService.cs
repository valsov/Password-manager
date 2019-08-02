using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;

namespace PasswordManager.Service
{
    public class SettingsService : ISettingsService
    {
        ISettingsRepository settingsRepository;

        public SettingsService(ISettingsRepository settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }

        public string GetDatabasePath()
        {
            return settingsRepository.GetSettings().DatabasePath;
        }

        public bool SaveDatabasePath(string path)
        {
            var settingsModel = settingsRepository.GetSettings();
            settingsModel.DatabasePath = path;
            return settingsRepository.WriteSettings(settingsModel);
        }
    }
}
