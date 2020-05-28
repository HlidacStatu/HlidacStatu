using System.Text.RegularExpressions;

namespace HlidacStatu.Q
{
    public class RabbitMQBusOptions
    {
        public string ConnectionString { get; set; }

        public override string ToString()
        {
            var match = Regex.Match(ConnectionString, "(host=[^;]*)");
            string host = (match.Success) ? match.Value : "";
            return $"Connected to host: [{host}]";
        }
    }
}