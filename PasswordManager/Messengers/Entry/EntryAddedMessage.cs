using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal a PasswordEntry was added, so the view must be updated accordingly
    /// </summary>
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
