using Newtonsoft.Json;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using System;
using System.IO;

namespace PasswordManager.Repository
{
    /// <summary>
    /// Implementation of ISettingsRepository interface, provides an access to the application's configuration
    /// </summary>
    public class SettingsRepository : ISettingsRepository
    {
        const string SETTINGS_FILE_NAME = "settings.json";

        SettingsModel cache;

        /// <summary>
        /// Load the application's settings into the cache
        /// </summary>
        /// <returns></returns>
        public SettingsModel GetSettings()
        {
            if (cache is null)
            {
                cache = InternalGetSettings();
            }
            return cache;
        }

        /// <summary>
        /// Write the given settings to the storage
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool WriteSettings(SettingsModel settings)
        {
            cache = settings;
            return InternalWriteSettings();
        }

        /// <summary>
        /// Read the settings from the disk
        /// </summary>
        /// <returns></returns>
        SettingsModel InternalGetSettings()
        {
            try
            {
                var data = File.ReadAllText(SETTINGS_FILE_NAME);
                return JsonConvert.DeserializeObject<SettingsModel>(data);
            }
            catch (Exception)
            {
                return new SettingsModel();
            }
        }

        /// <summary>
        /// Write the cache settings to the disk
        /// </summary>
        /// <returns></returns>
        bool InternalWriteSettings()
        {
            try
            {
                var json = JsonConvert.SerializeObject(cache);
                File.WriteAllText(SETTINGS_FILE_NAME, json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
