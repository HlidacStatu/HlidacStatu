using HlidacStatu.Q.Messages;
using HlidacStatu.Q.Subscriber;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http.Headers;

namespace HlidacStatu.ClassificationRepair
{
    public static class Program
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
                services.AddHostedService<RabbitMQListenerServiceAsync<ClassificationFeedback>>();
                services.AddScoped<IMessageHandlerAsync<ClassificationFeedback>, ProcessClassificationFeedback>();

                // email service
                services.Configure<SmtpSettings>(hostContext.Configuration.GetSection("Smtp"));
                services.AddTransient<IEmailService, EmailService>();

                services.AddHttpClient<IStemmerService, StemmerService>(config =>
                {
                    config.BaseAddress = new Uri(hostContext.Configuration.GetValue<string>("ClassificatorUri"));
                });

                services.AddHttpClient<IHlidacService, HlidacService>(config =>
                {
                    config.BaseAddress = new Uri(hostContext.Configuration.GetValue<string>("HlidacApiUri"));
                    config.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Token",
                            hostContext.Configuration.GetValue<string>("HlidacApiToken"));
                });
            });
    }
}