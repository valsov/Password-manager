namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to display or hide the SettingsView usercontrol
    /// </summary>
    public class ToggleSettingsViewMessage : BaseMessage
    {
        public bool Visibility { get; private set; }

        public ToggleSettingsViewMessage(object sender, bool visibility)
            : base(sender)
        {
            Visibility = visibility;
        }
    }
}
