using Newtonsoft.Json;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PasswordManager.Service
{
    /// <summary>
    /// Implementation of IIconsService interface, service handling the user's websites icons management
    /// </summary>
    public class IconsService : IIconsService
    {
        private IEncryptionService encryptionService;

        private IIconsRepository iconsRepository;

        const string iconsPath = "icons";

        const string iconsMappingFile = "mapping";

        // Alternative : https://icons.duckduckgo.com/ip2/
        const string iconsQueryUrl = "https://logo.clearbit.com/";

        /// <summary>
        /// Mapping data : [GUID, hostname]
        /// </summary>
        Dictionary<string, string> mapping;

        /// <summary>
        /// Triggered when the icons are loaded
        /// </summary>
        public event EventHandler IconsLoadedEvent;

        /// <summary>
        /// Triggered when an icons was downloaded
        /// </summary>
        public event EventHandler IconDownloadedEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="encryptionService"></param>
        public IconsService(IEncryptionService encryptionService,
                            IIconsRepository iconsRepository)
        {
            this.encryptionService = encryptionService;
            this.iconsRepository = iconsRepository;
            mapping = new Dictionary<string, string>();
        }

        /// <summary>
        /// Load the icons from their directory, async
        /// </summary>
        /// <param name="password"></param>
        public void LoadIcons(string password)
        {
            Task.Run(() => InternalLoadIcons(password));
        }

        /// <summary>
        /// Unload the icons from the memory
        /// </summary>
        public void UnloadIcons()
        {
            iconsRepository.UnloadIcons();
        }

        /// <summary>
        /// Download an icon and save it to the disk, encrypted
        /// </summary>
        /// <param name="url"></param>
        /// <param name="password"></param>
        public void DownloadIcon(string url, string password)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            CheckIconsDirectory();

            var hostname = GetHostnameFromUrl(url);
            if (iconsRepository.CheckIconExists(hostname))
            {
                return;
            }

            var client = new WebClient();
            client.DownloadDataCompleted += DownloadFileCompleted(hostname, password);
            var uri = new Uri(string.Concat(iconsQueryUrl, hostname, "?size=30"));
            client.DownloadDataAsync(uri);
        }

        /// <summary>
        /// Get an icon from the memory
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public BitmapSource GetIcon(string url)
        {
            var hostname = GetHostnameFromUrl(url);
            return iconsRepository.GetIcon(hostname);
        }

        /// <summary>
        /// Load the mapping file and then load the icons based on it
        /// </summary>
        /// <param name="password"></param>
        void InternalLoadIcons(string password)
        {
            CheckIconsDirectory();

            if (!LoadMapping(password))
            {
                // No mapping file was loaded, can't load the icons
                IconsLoadedEvent?.Invoke(this, new EventArgs());
                return;
            }

            foreach (var file in Directory.GetFiles(iconsPath))
            {
                if (!mapping.TryGetValue(Path.GetFileName(file), out string hostname))
                {
                    // File isn't listed in mapping, skip
                    continue;
                }

                var encryptedBytes = File.ReadAllBytes(file);
                var iconBytes = encryptionService.Decrypt(encryptedBytes, password);

                BitmapSource bitmapSource;
                try
                {
                    bitmapSource = ConvertBytesToBitmapSource(iconBytes);
                }
                catch (Exception)
                {
                    // Data error
                    continue;
                }
                // Freeze to allow cross-thread usage
                bitmapSource.Freeze();
                iconsRepository.AddIcon(bitmapSource, hostname);
            }

            IconsLoadedEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Load mapping data from disk
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool LoadMapping(string password)
        {
            var path = Path.Combine(iconsPath, iconsMappingFile);
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                var encryptedContent = File.ReadAllText(path);
                var json = encryptionService.Decrypt(encryptedContent, password);
                mapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update mapping data and mapping file
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hostname"></param>
        /// <param name="password"></param>
        private void UpdateMapping(string key, string hostname, string password)
        {
            var path = Path.Combine(iconsPath, iconsMappingFile);
            mapping.Add(key, hostname);

            try
            {
                var json = JsonConvert.SerializeObject(mapping);
                var encryptedData = encryptionService.Encrypt(json, password);
                File.WriteAllText(path, encryptedData);
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        /// <summary>
        /// Convert a byte array representing a bitmap to a BitmapSource
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        BitmapSource ConvertBytesToBitmapSource(byte[] bytes)
        {
            Bitmap bitmap;
            using (var ms = new MemoryStream(bytes))
            {
                bitmap = new Bitmap(ms);
            }

            //Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            return Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// Retrieve the hostname part of an url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetHostnameFromUrl(string url)
        {
            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
            {
                url = $"http://{url}";
            }
            var uri = new Uri(url);
            url = uri.Host;

            if (url.StartsWith("www."))
            {
                url = url.Replace("www.", string.Empty);
            }

            return url;
        }

        /// <summary>
        /// Check if the icons directory exists, if it doesn't, create it
        /// </summary>
        void CheckIconsDirectory()
        {
            if (!Directory.Exists(iconsPath))
            {
                Directory.CreateDirectory(iconsPath);
            }
        }

        /// <summary>
        /// Download file completed event handler, update the mapping and load the bitmap into the repository
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        DownloadDataCompletedEventHandler DownloadFileCompleted(string hostname, string password)
        {
            Action<object, DownloadDataCompletedEventArgs> action = (sender, e) =>
            {
                var _hostname = hostname;
                try
                {
                    var bitmapSource = ConvertBytesToBitmapSource(e.Result);
                    iconsRepository.AddIcon(bitmapSource, _hostname);
                }
                catch (Exception)
                {
                    // Data error, abort
                    return;
                }

                var encryptedBytes = encryptionService.Encrypt(e.Result, password);
                var guid = Guid.NewGuid().ToString();
                File.WriteAllBytes(Path.Combine(iconsPath, guid), encryptedBytes);
                UpdateMapping(guid, _hostname, password);

                IconDownloadedEvent?.Invoke(this, new EventArgs());
            };
            return new DownloadDataCompletedEventHandler(action);
        }
    }
}
