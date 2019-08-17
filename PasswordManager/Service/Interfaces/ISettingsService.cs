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
    }
}
