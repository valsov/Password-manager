using Newtonsoft.Json;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
using System.IO;

namespace PasswordManager.Repository
{
    public class DatabaseRepository : IDatabaseRepository
    {
        IEncryptionService encryptionService;

        DatabaseModel cache = null;

        public DatabaseRepository(IEncryptionService encryptionService)
        {
            this.encryptionService = encryptionService;
        }

        public DatabaseModel GetDatabase()
        {
            return cache;
        }

        public DatabaseModel LoadDatabase(string path, string password)
        {
            cache = InternalGetDatabase(path, password);
            if(cache != null)
            {
                cache.MainPassword = password;
            }
            return cache;
        }

        public bool WriteDatabase(DatabaseModel database)
        {
            cache = database;
            return InternalWriteDatabase();
        }

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

        public bool AddPasswordEntry(PasswordEntryModel entry)
        {
            cache.PasswordEntries.Add(entry);
            return InternalWriteDatabase();
        }

        public bool DeletePasswordEntry(PasswordEntryModel entry)
        {
            throw new NotImplementedException();
        }

        public bool CheckDatabaseExists(string path)
        {
            return File.Exists(path);
        }

        public bool UpdateCategory(string oldCategory, string newCategory)
        {
            throw new NotImplementedException();
        }

        public bool AddCategory(string category)
        {
            cache.Categories.Add(category);
            return InternalWriteDatabase();
        }

        public bool DeleteCategory(string category)
        {
            throw new NotImplementedException();
        }

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
