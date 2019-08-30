using PasswordManager.Events;
using System;

namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service handling the password database synchronisation
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Event published when a database download operation ended
        /// </summary>
        event EventHandler<DatabaseDownloadEndedEventArgs> DatabaseDownloadEnded;

        /// <summary>
        /// Download the newest file .crypt file from the given url, async
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        void DownloadDatabaseFromUrl(string url, string path);
    }
}
