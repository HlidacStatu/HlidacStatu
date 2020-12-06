using RabbitMQ.Client;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HlidacStatu.Q.Simple
{
    public class Response<T>
    {
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public T Value { get; set; }
        public ulong? ResponseId { get; set; } = null;
    }

    public class Queue<T>
        : IDisposable

    {
        public string QueueName { get; }

        private readonly ConnectionFactory factory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private bool disposedValue;

        public Queue(string queueName, string connectionString)
        {
            QueueName = queueName;
            //parse connectionString
            //host=ip;username=usr;password=pswd
            string[] cnnstrParts = connectionString.Split(';');
            string host = cnnstrParts.Where(m => m.StartsWith("host=")).Select(m => m.Split('=')).FirstOrDefault()?[1];
            string usrn = cnnstrParts.Where(m => m.StartsWith("username=")).Select(m => m.Split('=')).FirstOrDefault()?[1];
            string pswd = cnnstrParts.Where(m => m.StartsWith("password=")).Select(m => m.Split('=')).FirstOrDefault()?[1];

            factory = new ConnectionFactory() { HostName = host, UserName = usrn, Password = pswd };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        }

        public void Send(T message)
        {
            var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange: "", routingKey: this.QueueName, basicProperties: null, body: body);
        }

        public T GetAndAck()
        {
            var res = Get();
            if (res.ResponseId.HasValue)
                AckMessage(res.ResponseId.Value);

            return res.Value;

        }
        public Response<T> Get()
        {
            //var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            var res = channel.BasicGet(this.QueueName, false);
            if (res == null)
                return new Response<T>() { Value = default(T) };

            var body = Encoding.UTF8.GetString(res.Body.ToArray());
            return new Response<T>()
            {
                Value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body),
                ResponseId = res.DeliveryTag
            };
        }

        public void AckMessage(ulong responseId)
        {
            channel.BasicAck(responseId, false);
        }
        public void RejectMessage(ulong responseId)
        {
            channel.BasicReject(responseId, true);
        }
        public void RejectMessageOnTheEnd(ulong responseId,T value)
        {
            AckMessage(responseId);
            Send(value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    channel.Dispose();
                    connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Queue()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
