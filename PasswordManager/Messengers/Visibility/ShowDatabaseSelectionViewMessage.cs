namespace PasswordManager.Messengers
{
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
