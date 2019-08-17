namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal a Category was added, so the views must be updated accordingly
    /// </summary>
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
