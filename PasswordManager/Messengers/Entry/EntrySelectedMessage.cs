using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    public class EntrySelectedMessage : BaseMessage
    {
        public PasswordEntryModel passwordEntry { get; set; }

        public EntrySelectedMessage(object sender, PasswordEntryModel model)
            : base(sender)
        {
            this.passwordEntry = model;
        }
    }
}
