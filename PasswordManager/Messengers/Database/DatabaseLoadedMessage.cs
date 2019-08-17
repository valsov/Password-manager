using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal a database was unloaded, so the views displaying its data must do so
    /// </summary>
    public class DatabaseLoadedMessage : BaseMessage
    {
        public DatabaseModel DatabaseModel { get; set; }

        public DatabaseLoadedMessage(object sender, DatabaseModel model)
            : base(sender)
        {
            this.DatabaseModel = model;
        }
    }
}
