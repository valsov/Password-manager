namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to display the DatabaseSelectionView usercontrol
    /// </summary>
    public class ShowDatabaseSelectionViewMessage : BaseMessage
    {
        public string Path { get; set; }

        public ShowDatabaseSelectionViewMessage(object sender, string path)
            : base(sender)
        {
            Path = path;
        }
    }
}
