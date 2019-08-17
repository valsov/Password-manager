using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal a PasswordEntry was deleted, so the view must be updated accordingly
    /// </summary>
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
