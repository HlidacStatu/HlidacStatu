using RabbitMQ.Client;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HlidacStatu.Q.Simple
{
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

            factory = new ConnectionFactory() { HostName = host, UserName=usrn, Password= pswd };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        }

        public void Send(T message)
        {
            var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange: "", routingKey: this.QueueName, basicProperties: null, body: body);
        }

        public T Get()
        {
            //var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            var res = channel.BasicGet(this.QueueName, true);
            if (res == null)
                return default(T);

            var body = Encoding.UTF8.GetString(res.Body.ToArray());
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);
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
