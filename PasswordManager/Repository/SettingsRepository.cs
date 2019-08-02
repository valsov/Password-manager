using Newtonsoft.Json;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using System;
using System.IO;

namespace PasswordManager.Repository
{
    public class SettingsRepository : ISettingsRepository
    {
        const string SETTINGS_FILE_NAME = "settings.json";

        SettingsModel cache;

        public SettingsModel GetSettings()
        {
            if (cache is null)
            {
                cache = InternalGetSettings();
            }
            return cache;
        }

        public bool WriteSettings(SettingsModel settings)
        {
            cache = settings;
            return InternalWriteSettings();
        }

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
