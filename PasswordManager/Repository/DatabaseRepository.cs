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
    /// Implementation of IDatabaseRepository interface, provides an access to the current password database
    /// </summary>
    public class DatabaseRepository : IDatabaseRepository
    {
        IEncryptionService encryptionService;

        DatabaseModel cache = null;

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
            if (cache != null)
            {
                cache.MainPassword = password;
            }
            return cache;
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
        /// Delete the database cache
        /// </summary>
        public void UnloadDatabase()
        {
            cache = null;
        }

        /// <summary>
        /// Write the given batabase to the storage
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public bool WriteDatabase(DatabaseModel database)
        {
            cache = database;
            return InternalWriteDatabase();
        }

        /// <summary>
        /// Update the cache's corresponding PasswordEntry with the given entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool UpdatePasswordEntry(PasswordEntryModel entry)
        {
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
            cache.Categories.Add(category);
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
            var index = cache.Categories.IndexOf(oldCategory);
            cache.Categories[index] = newCategory;
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
            cache.Categories.Remove(category);
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
        /// <param name="password"></param>
        /// <returns></returns>
        DatabaseModel InternalGetDatabase(string path, string password)
        {
            if (path is null || password is null || !File.Exists(path))
            {
                return null;
            }

            try
            {
                var encryptedJson = File.ReadAllText(path);
                var json = encryptionService.Decrypt(encryptedJson, password);
                if (string.IsNullOrEmpty(json))
                {
                    throw new Exception();
                }
                return JsonConvert.DeserializeObject<DatabaseModel>(json);
            }
            catch (Exception)
            {
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
                var encryptedJson = encryptionService.Encrypt(json, cache.MainPassword);
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
