﻿using System.Windows.Media.Imaging;

namespace PasswordManager.Repository.Interfaces
{
    /// <summary>
    /// Provides an access to the user's websites icons
    /// </summary>
    public interface IIconsRepository
    {
        /// <summary>
        /// Associate a BitmapSource to an hostname and store them in memory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hostname"></param>
        void AddIcon(BitmapSource source, string hostname);

        /// <summary>
        /// Get the BitmapSource associated with the given hostname
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        BitmapSource GetIcon(string hostname);

        /// <summary>
        /// Verify the an entry with the given hostname exists in the repository
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        bool CheckIconExists(string hostname);

        /// <summary>
        /// Empty the repository
        /// </summary>
        void UnloadIcons();
    }
}
