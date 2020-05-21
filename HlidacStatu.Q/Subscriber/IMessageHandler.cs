namespace HlidacStatu.Q.Subscriber
{
    public interface IMessageHandler<T> where T : class
    {
        void Handle(T message);
    }
}