namespace PasswordManager.Service.Interfaces
{
    public interface ISettingsService
    {
        string GetDatabasePath();

        bool SaveDatabasePath(string path);
    }
}
