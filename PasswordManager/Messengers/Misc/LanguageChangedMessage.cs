using PasswordManager.Model;

namespace PasswordManager.Messengers
{
    /// <summary>
    /// Message to signal the application language changed
    /// </summary>
    public class LanguageChangedMessage : BaseMessage
    {
        public Languages Language{ get; private set;}

        public LanguageChangedMessage(object sender, Languages language)
            : base(sender)
        {
            Language = language;
        }
    }
}
