using EasyNetQ;

namespace HlidacStatu.Q.Publisher
{
    public static class QuickPublisher
    {
        public static void Publish<T>(T msg, string connectionString, string topic = null)
            where T : class
        {
            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                if (!string.IsNullOrWhiteSpace(topic))
                    bus.PubSub.Publish<T>(msg, topic);
                else
                    bus.PubSub.Publish<T>(msg);
            }
        }
    }
}
