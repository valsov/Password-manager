namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal a Category was edited, so the views must be updated accordingly
    /// </summary>
    public class CategoryEditedMessage : BaseMessage
    {
        public string BaseCategory { get; private set; }

        public string NewCategory { get; private set; }

        public CategoryEditedMessage(object sender, string baseCategory, string newCategory)
            : base(sender)
        {
            BaseCategory = baseCategory;
            NewCategory = newCategory;
        }
    }
}
