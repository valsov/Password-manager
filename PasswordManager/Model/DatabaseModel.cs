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
        public List<CategoryModel> Categories { get; set; }

        /// <summary>
        /// List of deleted password entries
        /// </summary>
        public List<string> DeletedEntries { get; set; }

        /// <summary>
        /// List of deleted categories
        /// </summary>
        public List<string> DeletedCategories { get; set; }

        /// <summary>
        /// Data relative to the sync process
        /// </summary>
        public SyncData SyncData { get; set; }
    }
}
