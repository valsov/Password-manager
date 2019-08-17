namespace PasswordManager.Messengers
{
    /// <summary>
    /// Base message including the Sender object
    /// </summary>
    public abstract class BaseMessage
    {
        public object Sender { get; set; }

        public BaseMessage(object sender)
        {
            this.Sender = sender;
        }
    }
}
