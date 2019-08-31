using Newtonsoft.Json;
using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace PasswordManager.Repository
{
    /// <summary>
    /// Provides an access to the user's websites icons
    /// </summary>
    public class IconsRepository : IIconsRepository
    {
        private IEncryptionService encryptionService;

        private Dictionary<string, BitmapSource> cache;

        /// <summary>
        /// Mapping data : [GUID, hostname]
        /// </summary>
        Dictionary<string, string> mapping;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="encryptionService"></param>
        public IconsRepository(IEncryptionService encryptionService)
        {
            this.encryptionService = encryptionService;

            cache = new Dictionary<string, BitmapSource>();
            mapping = new Dictionary<string, string>();
        }

        /// <summary>
        /// Associate a BitmapSource to an hostname and store them in memory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hostname"></param>
        /// <returns>GUID of the entry</returns>
        public string AddIcon(BitmapSource source, string hostname)
        {
            if (cache.ContainsKey(hostname))
            {
                // This case shouldn't happend
                cache[hostname] = source;
                return null;
            }
            else
            {
                cache.Add(hostname, source);
                var key = Guid.NewGuid().ToString();
                UpdateMapping(key, hostname);
                return key;
            }
        }

        /// <summary>
        /// Get the BitmapSource associated with the given hostname
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public BitmapSource GetIcon(string hostname)
        {
            if(cache.TryGetValue(hostname, out BitmapSource source))
            {
                return source;
            }

            return null;
        }

        /// <summary>
        /// Verify the an entry with the given hostname exists in the repository
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public bool CheckIconExists(string hostname)
        {
            return cache.ContainsKey(hostname);
        }

        /// <summary>
        /// Empty the repository
        /// </summary>
        public void UnloadIcons()
        {
            cache.Clear();
        }

        /// <summary>
        /// Load the mapping file into memory
        /// </summary>
        /// <returns></returns>
        public bool LoadMapping()
        {
            var path = Path.Combine(Constants.IconsPath, Constants.IconsMappingFile);
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                var encryptedContent = File.ReadAllText(path);
                var json = encryptionService.Decrypt(encryptedContent);
                mapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Try to get the associated hostname in the mapping
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public bool TryGetMappingValue(string key, out string hostname)
        {
            return mapping.TryGetValue(key, out hostname);
        }

        /// <summary>
        /// Update mapping data and mapping file
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hostname"></param>
        private void UpdateMapping(string key, string hostname)
        {
            var path = Path.Combine(Constants.IconsPath, Constants.IconsMappingFile);
            mapping.Add(key, hostname);

            try
            {
                var json = JsonConvert.SerializeObject(mapping);
                var encryptedData = encryptionService.Encrypt(json);
                File.WriteAllText(path, encryptedData);
            }
            catch (Exception)
            {
                // Ignore
            }
        }
    }
}
