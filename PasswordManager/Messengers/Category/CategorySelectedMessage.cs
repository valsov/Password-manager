namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal a category was selected, refining the Password entries list
    /// </summary>
    public class CategorySelectedMessage : BaseMessage
    {
        public string SelectedCategory { get; set; }

        public CategorySelectedMessage(object sender, string category)
            : base(sender)
        {
            SelectedCategory = category;
        }
    }
}
