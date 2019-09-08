namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to display or hide the SyncView usercontrol
    /// </summary>
    public class ToggleSyncViewMessage : BaseMessage
    {
        public bool Visibility { get; private set; }

        public ToggleSyncViewMessage(object sender, bool visibility)
            : base(sender)
        {
            Visibility = visibility;
        }
    }
}
