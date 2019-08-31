using PasswordManager.Model;
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
            return GetSettings().DatabasePath;
        }

        /// <summary>
        /// Save the given database path in the settings
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SaveDatabasePath(string path)
        {
            var settingsModel = GetSettings();
            settingsModel.DatabasePath = path;
            return SaveSettings(settingsModel);
        }

        /// <summary>
        /// Get the setting's language
        /// </summary>
        /// <returns></returns>
        public Languages GetLanguage()
        {
            return GetSettings().Language;
        }

        /// <summary>
        /// Save the given language in the settings
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool SaveLanguage(Languages language)
        {
            var settingsModel = GetSettings();
            settingsModel.Language = language;
            return SaveSettings(settingsModel);
        }

        /// <summary>
        /// Get the setting's theme
        /// </summary>
        /// <returns></returns>
        public ColorThemes GetTheme()
        {
            return GetSettings().Theme;
        }

        /// <summary>
        /// Save the given theme in the settings
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public bool SaveTheme(ColorThemes theme)
        {
            var settingsModel = GetSettings();
            settingsModel.Theme = theme;
            return SaveSettings(settingsModel);
        }

        /// <summary>
        /// Get the setting's clipboard timer duration
        /// </summary>
        /// <returns></returns>
        public int GetClipboardTimerDuration()
        {
            return GetSettings().ClipboardTimerDuration;
        }

        /// <summary>
        /// Save the given clmipboard timer duration in the settings
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool SaveClipboardTimerDuration(int duration)
        {
            var settingsModel = GetSettings();
            settingsModel.ClipboardTimerDuration = duration;
            return SaveSettings(settingsModel);
        }

        /// <summary>
        /// Retrieve the settings from the repository
        /// </summary>
        /// <returns></returns>
        private SettingsModel GetSettings()
        {
            return settingsRepository.GetSettings();
        }
        
        /// <summary>
        /// Save the settings via the repository
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool SaveSettings(SettingsModel model)
        {
            return settingsRepository.WriteSettings(model);
        }
    }
}
