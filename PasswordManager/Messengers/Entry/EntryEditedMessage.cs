using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal a PasswordEntry was edited, so the view must be updated accordingly
    /// </summary>
    public class EntryEditedMessage : BaseMessage
    {
        public PasswordEntryModel Entry { get; private set; }

        public EntryEditedMessage(object sender, PasswordEntryModel entry)
            :base(sender)
        {
            this.Entry = entry;
        }
    }
}
