namespace PasswordManager.Messengers
{
    public abstract class BaseMessage
    {
        public object Sender { get; set; }

        public BaseMessage(object sender)
        {
            this.Sender = sender;
        }
    }
}
