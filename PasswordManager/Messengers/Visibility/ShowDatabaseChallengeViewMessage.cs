namespace PasswordManager.Messengers
{
    public class ShowDatabaseChallengeViewMessage : BaseMessage
    {
        public string DatabasePath { get; set; }

        public ShowDatabaseChallengeViewMessage(object sender, string path)
            : base(sender)
        {
            this.DatabasePath = path;
        }
    }
}
