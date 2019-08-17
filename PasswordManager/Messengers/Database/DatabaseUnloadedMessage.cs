namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal the database was unloaded, so all its related ressources must be deleted
    /// </summary>
    public class DatabaseUnloadedMessage : BaseMessage
    {
        public DatabaseUnloadedMessage(object sender)
            : base(sender)
        {

        }
    }
}
