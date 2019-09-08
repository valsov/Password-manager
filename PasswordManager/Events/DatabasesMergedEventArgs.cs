using PasswordManager.Model;

namespace PasswordManager.Events
{
    /// <summary>
    /// Event args for DatabaseMerged event
    /// </summary>
    public class DatabasesMergedEventArgs
    {
        /// <summary>
        /// Resulting merged database
        /// </summary>
        public DatabaseModel MergedDatabase { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mergedDatabase"></param>
        public DatabasesMergedEventArgs(DatabaseModel mergedDatabase)
        {
            MergedDatabase = mergedDatabase;
        }
    }
}
