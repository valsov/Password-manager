using System;
using System.Windows.Media.Imaging;

namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service handling the user's websites icons management
    /// </summary>
    public interface IIconsService
    {
        /// <summary>
        /// Triggered when the icons are loaded
        /// </summary>
        event EventHandler IconsLoadedEvent;

        /// <summary>
        /// Triggered when an icons was downloaded
        /// </summary>
        event EventHandler IconDownloadedEvent;

        /// <summary>
        /// Load the icons from their directory, async
        /// </summary>
        /// <param name="password"></param>
        void LoadIcons(string password);

        /// <summary>
        /// Unload the icons from the memory
        /// </summary>
        void UnloadIcons();

        /// <summary>
        /// Download an icon and save it to the disk, encrypted
        /// </summary>
        /// <param name="url"></param>
        /// <param name="password"></param>
        void DownloadIcon(string url, string password);

        /// <summary>
        /// Get an icon from the memory
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        BitmapSource GetIcon(string url);
    }
}
