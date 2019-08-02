using PasswordManager.Model;

namespace PasswordManager.Service.Interfaces
{
    public interface IDatabaseService
    {
        DatabaseModel ReadDatabase(string path, string password);

        bool WriteDatabase(DatabaseModel database);

        DatabaseModel CreateDatabase(string path, string name, string password);
    }
}
