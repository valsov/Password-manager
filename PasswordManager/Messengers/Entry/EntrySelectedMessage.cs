using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to display the EntryView usercontrol with the given Entry as content
    /// </summary>
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
