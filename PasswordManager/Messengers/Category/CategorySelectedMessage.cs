namespace PasswordManager.Messengers
{
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
