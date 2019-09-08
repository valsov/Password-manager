namespace PasswordManager.Events
{
    /// <summary>
    /// Event args for DatabaseUploadEnded event
    /// </summary>
    public class DatabaseUploadEndedEventArgs
    {
        /// <summary>
        /// Result of the operation
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// Eventual error from the operation
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result"></param>
        /// <param name="error"></param>
        public DatabaseUploadEndedEventArgs(bool result, string error)
        {
            Result = result;
            Error = error;
        }
    }
}
