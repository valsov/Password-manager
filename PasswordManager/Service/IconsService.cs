using PasswordManager.Model;
using PasswordManager.Repository.Interfaces;
using PasswordManager.Service.Interfaces;
using System;
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
    /// Service handling the user's websites icons management
    /// </summary>
    public class IconsService : IIconsService
    {
        private IEncryptionService encryptionService;

        private IIconsRepository iconsRepository;

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
        }

        /// <summary>
        /// Load the icons from their directory, async
        /// </summary>
        public void LoadIcons()
        {
            Task.Run(InternalLoadIcons);
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
        public void DownloadIcon(string url)
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
            client.DownloadDataCompleted += DownloadFileCompleted(hostname);
            var uri = new Uri(string.Concat(Constants.IconsQueryUrl, hostname, "?size=30"));
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
        void InternalLoadIcons()
        {
            CheckIconsDirectory();

            if (!iconsRepository.LoadMapping())
            {
                // No mapping file was loaded, can't load the icons
                IconsLoadedEvent?.Invoke(this, new EventArgs());
                return;
            }

            foreach (var file in Directory.GetFiles(Constants.IconsPath))
            {
                if (!iconsRepository.TryGetMappingValue(Path.GetFileName(file), out string hostname))
                {
                    // File isn't listed in mapping, skip
                    continue;
                }

                var encryptedBytes = File.ReadAllBytes(file);
                var iconBytes = encryptionService.Decrypt(encryptedBytes);

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
            if (!Directory.Exists(Constants.IconsPath))
            {
                Directory.CreateDirectory(Constants.IconsPath);
            }
        }

        /// <summary>
        /// Download file completed event handler, load the bitmap into the repository
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        DownloadDataCompletedEventHandler DownloadFileCompleted(string hostname)
        {
            Action<object, DownloadDataCompletedEventArgs> action = (sender, e) =>
            {
                var _hostname = hostname;
                BitmapSource bitmapSource;
                try
                {
                    bitmapSource = ConvertBytesToBitmapSource(e.Result);
                }
                catch (Exception)
                {
                    // Data error, abort
                    return;
                }

                var guid = iconsRepository.AddIcon(bitmapSource, _hostname);
                var encryptedBytes = encryptionService.Encrypt(e.Result);
                File.WriteAllBytes(Path.Combine(Constants.IconsPath, guid), encryptedBytes);

                IconDownloadedEvent?.Invoke(this, new EventArgs());
            };
            return new DownloadDataCompletedEventHandler(action);
        }
    }
}
