using PasswordManager.Model;

namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service Handling the user's settings
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Get the setting's database path
        /// </summary>
        /// <returns></returns>
        string GetDatabasePath();

        /// <summary>
        /// Save the given database path in the settings
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool SaveDatabasePath(string path);

        /// <summary>
        /// Get the setting's language
        /// </summary>
        /// <returns></returns>
        Languages GetLanguage();

        /// <summary>
        /// Save the given language in the settings
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        bool SaveLanguage(Languages language);

        /// <summary>
        /// Get the setting's theme
        /// </summary>
        /// <returns></returns>
        ColorThemes GetTheme();

        /// <summary>
        /// Save the given theme in the settings
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        bool SaveTheme(ColorThemes theme);

        /// <summary>
        /// Get the setting's clipboard timer duration
        /// </summary>
        /// <returns></returns>
        int GetClipboardTimerDuration();

        /// <summary>
        /// Save the given clmipboard timer duration in the settings
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        bool SaveClipboardTimerDuration(int duration);
    }
}
