using PasswordManager.Events;
using PasswordManager.Model;
using System;

namespace PasswordManager.Service.Interfaces
{
    /// <summary>
    /// Service handling the password database synchronisation, each function is async
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Event published when a database download operation ended
        /// </summary>
        event EventHandler<DatabaseDownloadEndedEventArgs> DatabaseDownloadEnded;

        /// <summary>
        /// Event published when a datases merge operation ended
        /// </summary>
        event EventHandler<DatabasesMergedEventArgs> DatabasesMerged;

        /// <summary>
        /// Event published when a database upload operation ended
        /// </summary>
        event EventHandler<DatabaseUploadEndedEventArgs> DatabaseUploadEnded;

        /// <summary>
        /// Download a database from the given url to the specified path
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        void DownloadDatabaseFromUrl(string url, string path);

        /// <summary>
        /// Download the last edited database from the given account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="path"></param>
        void DownloadDatabase(string username, string password, string path);

        /// <summary>
        /// Merge the given databases into one
        /// </summary>
        /// <param name="baseModel"></param>
        /// <param name="incomingModel"></param>
        void MergeDatabases(DatabaseModel baseModel, DatabaseModel incomingModel);

        /// <summary>
        /// Upload the file at the given path to the given account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="path"></param>
        void UploadDatabase(string username, string password, string path);
    }
}
