using Newtonsoft.Json;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace PasswordManager.Repository
{
    /// <summary>
    /// Provides an access to the current password database
    /// </summary>
    public class DatabaseRepository : IDatabaseRepository
    {
        private IEncryptionService encryptionService;

        private DatabaseModel cache = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="encryptionService"></param>
        public DatabaseRepository(IEncryptionService encryptionService)
        {
            this.encryptionService = encryptionService;
        }

        /// <summary>
        /// Load the database into the cache from the given path with the given password
        /// </summary>
        /// <param name="path"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DatabaseModel LoadDatabase(string path, string password)
        {
            cache = InternalGetDatabase(path, password);
            return cache;
        }

        /// <summary>
        /// Load the given database into the cache
        /// </summary>
        /// <param name="database"></param>
        public void LoadDatabase(DatabaseModel database)
        {
            cache = database;
        }

        /// <summary>
        /// Get the current database cache
        /// </summary>
        /// <returns></returns>
        public DatabaseModel GetDatabase()
        {
            return cache;
        }

        /// <summary>
        /// Retrieve a database from the given path, with the currently loaded encryption details
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DatabaseModel GetDatabase(string path)
        {
            return InternalGetDatabase(path);
        }

        /// <summary>
        /// Delete the database cache
        /// </summary>
        public void UnloadDatabase()
        {
            cache = null;
        }

        /// <summary>
        /// Write the database stored in the cache with the currently loaded encryption details
        /// </summary>
        /// <returns></returns>
        public bool WriteDatabase()
        {
            return InternalWriteDatabase();
        }

        /// <summary>
        /// Write the given batabase to the storage
        /// </summary>
        /// <param name="database"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool WriteDatabase(DatabaseModel database, string password)
        {
            encryptionService.Initialize(password);
            cache = database;
            var result = InternalWriteDatabase();
            if (!result)
            {
                encryptionService.Clear();
            }
            return result;
        }

        /// <summary>
        /// Update the cache's corresponding PasswordEntry with the given entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool UpdatePasswordEntry(PasswordEntryModel entry)
        {
            entry.LastEditionDate = DateTime.Now;
            for (int i = 0; i < cache?.PasswordEntries?.Count; i++)
            {
                if (cache.PasswordEntries[i].Id == entry.Id)
                {
                    cache.PasswordEntries[i] = entry;
                    InternalWriteDatabase();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add the given password entry to the cache
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool AddPasswordEntry(PasswordEntryModel entry)
        {
            entry.LastEditionDate = DateTime.Now;
            cache.PasswordEntries.Add(entry);
            return InternalWriteDatabase();
        }

        /// <summary>
        /// Delete the given password entry from the cache
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool DeletePasswordEntry(PasswordEntryModel entry)
        {
            cache.DeletedEntries.Add(entry.Id);
            cache.PasswordEntries.RemoveAll(x => x.Id == entry.Id);
            return InternalWriteDatabase();
        }

        /// <summary>
        /// Add the given category to the cache
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool AddCategory(string category)
        {
            var categoryModel = new CategoryModel()
            {
                Name = category,
                LastEditionDate = DateTime.Now
            };
            cache.Categories.Add(categoryModel);
            return InternalWriteDatabase();
        }

        /// <summary>
        /// Update the category in the cache
        /// </summary>
        /// <param name="oldCategory"></param>
        /// <param name="newCategory"></param>
        /// <returns></returns>
        public bool UpdateCategory(string oldCategory, string newCategory)
        {
            var category = cache.Categories.First(x => x.Name == oldCategory);
            category.LastEditionDate = DateTime.Now;
            category.Name = newCategory;
            foreach (var entry in cache.PasswordEntries.Where(x => x.Category == oldCategory))
            {
                entry.Category = newCategory;
            }
            return InternalWriteDatabase();
        }

        /// <summary>
        /// Delete the given category from the cache
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool DeleteCategory(string category)
        {
            var id = cache.Categories.First(x => x.Name == category).Id;
            cache.DeletedCategories.Add(id);
            cache.Categories.RemoveAll(x => x.Name == category);
            foreach (var entry in cache.PasswordEntries.Where(x => x.Category == category))
            {
                entry.Category = null;
            }
            return InternalWriteDatabase();
        }

        /// <summary>
        /// Check if a file exists at the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CheckDatabaseExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Read the database from the disk and decrypt it with the given password
        /// </summary>
        /// <param name="path"></param>
        /// <param name="password">If no password is given, use the one from the encryptionRepository if it has been set</param>
        /// <returns></returns>
        DatabaseModel InternalGetDatabase(string path, string password = "")
        {
            if (path is null || password is null || !File.Exists(path))
            {
                return null;
            }

            try
            {
                var encryptedJson = File.ReadAllText(path);
                if (password != "")
                {
                    // Initialize Encryption service with the given key
                    encryptionService.Initialize(password);
                }
                var json = encryptionService.Decrypt(encryptedJson);
                if (string.IsNullOrEmpty(json))
                {
                    throw new Exception();
                }
                return JsonConvert.DeserializeObject<DatabaseModel>(json);
            }
            catch (Exception)
            {
                encryptionService.Clear();
                return null;
            }
        }

        /// <summary>
        /// Encrypt and write the cache to the disk
        /// </summary>
        /// <returns></returns>
        bool InternalWriteDatabase()
        {
            try
            {
                var json = JsonConvert.SerializeObject(cache);
                var encryptedJson = encryptionService.Encrypt(json);
                File.WriteAllText(cache.Path, encryptedJson);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
