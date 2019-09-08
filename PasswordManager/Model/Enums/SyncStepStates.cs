namespace PasswordManager.Model
{
    /// <summary>
    /// Possible states for a database sync step
    /// </summary>
    public enum SyncStepStates
    {
        Inactive,
        InProgress,
        Done,
        Skipped,
        Failed
    }
}
