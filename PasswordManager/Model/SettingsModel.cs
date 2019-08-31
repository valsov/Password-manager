namespace PasswordManager.Model
{
    /// <summary>
    /// Class containing all the applciation settings
    /// </summary>
    public class SettingsModel
    {
        /// <summary>
        /// Current database path
        /// </summary>
        public string DatabasePath { get; set; }

        /// <summary>
        /// Language to use in the application
        /// </summary>
        public Languages Language { get; set; }

        /// <summary>
        /// Color theme of the application
        /// </summary>
        public ColorThemes Theme { get; set; }

        /// <summary>
        /// Duration in seconds the copied data should stay in the clipboard
        /// </summary>
        public int ClipboardTimerDuration { get; set; }
    }
}
