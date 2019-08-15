namespace PasswordManager.Messengers
{
    public class CategoryEditedMessage : BaseMessage
    {
        public string BaseCategory { get; set; }

        public string NewCategory { get; set; }

        public CategoryEditedMessage(object sender, string baseCategory, string newCategory)
            : base(sender)
        {
            BaseCategory = baseCategory;
            NewCategory = newCategory;
        }
    }
}
