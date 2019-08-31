namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal a Category was deleted, so the views must be updated accordingly
    /// </summary>
    public class CategoryDeletedMessage : BaseMessage
    {
        public string Category { get; private set; }

        public CategoryDeletedMessage(object sender, string category)
            : base(sender)
        {
            Category = category;
        }
    }
}
