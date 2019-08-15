namespace PasswordManager.Messengers
{
    public class CategoryDeletedMessage : BaseMessage
    {
        public string Category { get; set; }

        public CategoryDeletedMessage(object sender, string category)
            : base(sender)
        {
            Category = category;
        }
    }
}
