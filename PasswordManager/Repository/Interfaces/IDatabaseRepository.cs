using PasswordManager.Model;

namespace PasswordManager.Repository.Interfaces
{
    /// <summary>
    /// Provides an access to the current password database
    /// </summary>
    public interface IDatabaseRepository
    {
        /// <summary>
        /// Load the database into the cache from the given path with the given password
        /// </summary>
        /// <param name="path"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        DatabaseModel LoadDatabase(string path, string password);

        /// <summary>
        /// Get the current database cache
        /// </summary>
        /// <returns></returns>
        DatabaseModel GetDatabase();

        /// <summary>
        /// Delete the database cache
        /// </summary>
        void UnloadDatabase();

        /// <summary>
        /// Write the given batabase to the storage with the given encryption key
        /// </summary>
        /// <param name="database"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool WriteDatabase(DatabaseModel database, string password);

        /// <summary>
        /// Update the cache's corresponding PasswordEntry with the given entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool UpdatePasswordEntry(PasswordEntryModel entry);

        /// <summary>
        /// Add the given password entry to the cache
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool AddPasswordEntry(PasswordEntryModel entry);

        /// <summary>
        /// Delete the given password entry from the cache
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool DeletePasswordEntry(PasswordEntryModel entry);

        /// <summary>
        /// Add the given category to the cache
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        bool AddCategory(string category);

        /// <summary>
        /// Update the category in the cache
        /// </summary>
        /// <param name="oldCategory"></param>
        /// <param name="newCategory"></param>
        /// <returns></returns>
        bool UpdateCategory(string oldCategory, string newCategory);

        /// <summary>
        /// Delete the given category from the cache
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        bool DeleteCategory(string category);

        /// <summary>
        /// Check if a file exists at the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool CheckDatabaseExists(string path);
    }
}
