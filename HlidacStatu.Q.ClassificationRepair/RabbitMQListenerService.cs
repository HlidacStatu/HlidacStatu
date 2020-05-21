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
        private readonly ILogger _logger;
        private readonly IMessageHandler<T> _messageHandler;
        private readonly RabbitMQOptions _options;
        public RabbitMQListenerService(
            ILogger<RabbitMQListenerService<T>> logger,
            IMessageHandler<T> messageHandler,
            IOptionsMonitor<RabbitMQOptions> options )
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _options = options.CurrentValue;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var bus = RabbitHutch.CreateBus(_options.ConnectionString))
            {
                bus.Subscribe<T>("test", _messageHandler.Handle);

                _logger.LogInformation("Connected to queue. Listening started.");

                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    // todo: heart beat
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disconnected");
            return Task.CompletedTask;
        }
    }
}
