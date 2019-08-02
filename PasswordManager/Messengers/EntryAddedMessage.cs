using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    public class EntryAddedMessage : BaseMessage
    {
        public PasswordEntryModel Entry { get; set; }

        public EntryAddedMessage(object sender, PasswordEntryModel entry)
            : base(sender)
        {
            this.Entry = entry;
        }
    }
}
