using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace PasswordManager.Model
{
    /// <summary>
    /// Class to identify a password with its details
    /// </summary>
    public class PasswordEntryModel : INotifyPropertyChanged, ISyncEntry
    {
        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Unique id (GUID)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the password entry
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Website the password is used on
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Category of the password entry
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        [JsonIgnore]
        private string password;
        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }

        /// <summary>
        /// Corresponding password Strength
        /// </summary>
        public PasswordStrength PasswordStrength { get; set; }

        /// <summary>
        /// Notes of the password entry
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Last time the entry has been created / edited
        /// </summary>
        public DateTime LastEditionDate { get; set; }

        /// <summary>
        /// Constructor, set GUID
        /// </summary>
        public PasswordEntryModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
