using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    public class EntryDeletedMessage : BaseMessage
    {
        public PasswordEntryModel Entry { get; set; }

        public EntryDeletedMessage(object sender, PasswordEntryModel entry)
            : base(sender)
        {
            Entry = entry;
        }
    }
}
