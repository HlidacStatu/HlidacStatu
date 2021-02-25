using EasyNetQ;
using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HlidacStatu.Q.Subscriber
{
    public class RabbitMQListenerService<T> : IHostedService where T : class   
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IMessageHandler<T> _messageHandler;
        private readonly RabbitMQOptions _options;
        private IBus _rabbitBus;

        public RabbitMQListenerService(
            IHostApplicationLifetime appLifetime,
            IMessageHandler<T> messageHandler,
            IOptionsMonitor<RabbitMQOptions> options )
        {
            _appLifetime = appLifetime;
            _messageHandler = messageHandler;
            _options = options.CurrentValue;
            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                throw new Exception("Missing connection string.");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _rabbitBus = RabbitHutch.CreateBus(_options.ConnectionString);
            }
            catch (Exception ex)
            {
                throw;
            }
            
            _appLifetime.ApplicationStarted.Register(SubscribeToQueue);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            _rabbitBus.Dispose();

            return Task.CompletedTask;
        }

        private void SubscribeToQueue()
        {
            _rabbitBus.PubSub.Subscribe<T>(_options.SubscriberName, _messageHandler.Handle, configure => {
                configure.WithPrefetchCount(_options.PrefetchCount);
            });
        }
    }
}
