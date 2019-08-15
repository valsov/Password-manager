using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    public class EntryEditedMessage : BaseMessage
    {
        public PasswordEntryModel Entry { get; set; }

        public EntryEditedMessage(object sender, PasswordEntryModel entry)
            :base(sender)
        {
            this.Entry = entry;
        }
    }
}
