namespace PasswordManager.Events
{
    /// <summary>
    /// Event args for DatabaseDownloadEndedEvent
    /// </summary>
    public class DatabaseDownloadEndedEventArgs
    {
        /// <summary>
        /// Result of the download operation
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// Path of the downloaded file, if the download was a success
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Eventual error from the operation
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result"></param>
        /// <param name="path"></param>
        /// <param name="error"></param>
        public DatabaseDownloadEndedEventArgs(bool result, string path, string error)
        {
            Result = result;
            Path = path;
            Error = error;
        }
    }
}
