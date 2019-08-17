namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to display the EntryView usercontrol with the new entry form
    /// </summary>
    public class ShowNewEntryViewMessage : BaseMessage
    {
        public ShowNewEntryViewMessage(object sender)
            : base(sender)
        {

        }
    }
}
