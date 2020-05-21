using EasyNetQ;
using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HlidacStatu.Q.ClassificationRepair
{
    public class RabbitMQListenerService<T> : IHostedService where T : class   
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger _logger;
        private readonly IMessageHandler<T> _messageHandler;
        private readonly RabbitMQOptions _options;
        private IBus _rabbitBus;

        public RabbitMQListenerService(
            IHostApplicationLifetime appLifetime,
            ILogger<RabbitMQListenerService<T>> logger,
            IMessageHandler<T> messageHandler,
            IOptionsMonitor<RabbitMQOptions> options )
        {
            _appLifetime = appLifetime;
            _logger = logger;
            _messageHandler = messageHandler;
            _options = options.CurrentValue;
            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                _logger.LogError("RabbitMQ connection string is missing.");
                throw new Exception("Missing connection string.");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Connecting to RabbitMQ.");
            try
            {
                _rabbitBus = RabbitHutch.CreateBus(_options.ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Connection failed.");
                throw;
            }
            _logger.LogInformation("Connected.");
            _appLifetime.ApplicationStarted.Register(SubscribeToQueue);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disconnecting from RabbitMQ.");
            _rabbitBus.Dispose();
            _logger.LogInformation("Disconnected.");
            return Task.CompletedTask;
        }

        private void SubscribeToQueue()
        {
            _logger.LogInformation("Subscribing to Queue.");
            _logger.LogInformation(_options.ToString());
            _rabbitBus.SubscribeAsync<T>(_options.SubscriberName, _messageHandler.Handle, configure => {
                configure.WithPrefetchCount(_options.PrefetchCount);
            });
        }
    }
}
