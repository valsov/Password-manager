using Newtonsoft.Json;
using System.Collections.Generic;

namespace PasswordManager.Model
{
    /// <summary>
    /// Base class for a password database
    /// </summary>
    public class DatabaseModel
    {
        /// <summary>
        /// Absolute path of the database file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Name of the database
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of password entries
        /// </summary>
        public List<PasswordEntryModel> PasswordEntries { get; set; }

        /// <summary>
        /// List of categories
        /// </summary>
        public List<string> Categories { get; set; }

        /// <summary>
        /// Password used to encrypt the database, it is never written on disk
        /// </summary>
        [JsonIgnore]
        public string MainPassword { get; set; }
    }
}
