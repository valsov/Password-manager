namespace PasswordManager.Messengers
{
    public class CategoryAddedMessage : BaseMessage
    {
        public string NewCategory { get; set; }

        public CategoryAddedMessage(object sender, string category)
            : base(sender)
        {
            NewCategory = category;
        }
    }
}
