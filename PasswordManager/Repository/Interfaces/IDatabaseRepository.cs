using PasswordManager.Model;

namespace PasswordManager.Repository.Interfaces
{
    public interface IDatabaseRepository
    {
        DatabaseModel GetDatabase();

        DatabaseModel LoadDatabase(string path, string password);

        bool WriteDatabase(DatabaseModel database);

        bool UpdatePasswordEntry(PasswordEntryModel entry);

        bool AddPasswordEntry(PasswordEntryModel entry);

        bool DeletePasswordEntry(PasswordEntryModel entry);

        bool UpdateCategory(string oldCategory, string newCategory);

        bool AddCategory(string category);

        bool DeleteCategory(string category);

        bool CheckDatabaseExists(string path);
    }
}
