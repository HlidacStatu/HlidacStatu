using HlidacStatu.Q.Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HlidacStatu.Q.ClassificationRepair
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            // Following are set automatically:
            //   Sets the content root to the path returned by GetCurrentDirectory.
            //   Loads host configuration from:
            //     Environment variables prefixed with DOTNET_.
            //     Command-line arguments.
            //   Loads app configuration from:
            //     appsettings.json.
            //     appsettings.{ Environment}.json.
            //     Secret Manager when the app runs in the Development environment.
            //     Environment variables.
            //     Command-line arguments.
            //   Adds the following logging providers:
            //     Console
            //     Debug
            //     EventSource
            //     EventLog (only when running on Windows)
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.host.createdefaultbuilder?view=dotnet-plat-ext-3.1
            //.ConfigureAppConfiguration(configure =>
            //{
            //    configure.SetBasePath(Directory.GetCurrentDirectory());
            //    configure.AddJsonFile("appsettings.json", optional: true);
            //    configure.AddEnvironmentVariables(prefix: "PREFIX_");
            //})
            //.ConfigureLogging((hostContext, logging) =>
            //{
            //    logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
            //    logging.AddConsole(); // time doesn't need timestamp to it, because it is appended by docker
            //})
            .ConfigureServices((hostContext, services) =>
            {
                // rabbit configuration
                services.Configure<RabbitMQOptions>(hostContext.Configuration.GetSection("RabbitMQConnection"));
                // rabbit permanent connection - listener
                services.AddHostedService<RabbitMQListenerService<ClassificationFeedback>>();

            });
    }
}
