using System.Reflection;
using System.Text.RegularExpressions;

namespace HlidacStatu.Q.Subscriber
{
    public class RabbitMQOptions
    {
        public string ConnectionString { get; set; }
        public string SubscriberName { get; set; } = Assembly.GetExecutingAssembly().GetName().Name;
        public ushort PrefetchCount { get; set; } = 5;

        public override string ToString()
        {
            var match = Regex.Match(ConnectionString, "(host=[^;]*)");
            string host = (match.Success) ? match.Value : "";
            string result = $@"RabbitMQ Configuration:
  Connection string host: [{host}]
  Subscriber name: [{SubscriberName}]
  Prefetch count: [{PrefetchCount}]";
            return result;
        }
    }
}