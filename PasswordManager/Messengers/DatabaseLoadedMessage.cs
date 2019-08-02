using PasswordManager.Model;

namespace PasswordManager.Messengers
{
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
