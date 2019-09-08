using System;

namespace PasswordManager.Model
{
    /// <summary>
    /// Informations to perform a database sync
    /// </summary>
    public class SyncData
    {
        /// <summary>
        /// Username of the storage account
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password of the storage account
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Last time the sync was performed
        /// </summary>
        public DateTime LastSync { get; set; }
    }
}
