using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HlidacStatu.Q
{
    public class RabbitMQBusService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger _logger;
        private readonly RabbitMQBusOptions _options;
        public IBus RabbitBus { get; private set; }

        public RabbitMQBusService(
            IHostApplicationLifetime appLifetime,
            ILogger<RabbitMQBusService> logger,
            IOptionsMonitor<RabbitMQBusOptions> options)
        {
            _appLifetime = appLifetime;
            _logger = logger;
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
                RabbitBus = RabbitHutch.CreateBus(_options.ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection failed.");
                throw;
            }
            _logger.LogInformation("Connected.");
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disconnecting from RabbitMQ.");
            RabbitBus.Dispose();
            _logger.LogInformation("Disconnected.");
            return Task.CompletedTask;
        }
        
    }
}