using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using Telegram.Bot;
using TransmissionRemoteBot.Services.Telegram;
using TransmissionRemoteBot.Services.Telegram.Commands;
using TransmissionRemoteBot.Services.Telegram.Helpers;
using TransmissionRemoteBot.Services.Transmission;

namespace TransmissionRemoteBot.ConsoleRunner
{
    public class Program
    {
        private static ManualResetEvent _completionEvent = new ManualResetEvent(false);
        private static IConfigurationRoot _configurationRoot;

        public static void Main(string[] args)
        {
            _configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.local.json", optional: true)
            .Build();

            ITransmissionConfiguration transmissionConfiguration;
            try
            {
                transmissionConfiguration = new TransmissionConfiguration()
                {
                    Url = new Uri(_configurationRoot["transmission:url"]),
                    Login = _configurationRoot["transmission:login"],
                    Password = _configurationRoot["transmission:password"]
                };
            }
            catch
            {
                transmissionConfiguration = null;
            }

            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddScoped<ITelegramService, TelegramService>()
            .AddSingleton<ITelegramConfiguration, TelegramConfiguration>()
            .AddSingleton<ITelegramBotClient>((sp) => {
                var config = sp.GetRequiredService<ITelegramConfiguration>();
                return new TelegramBotClient(config.Apikey);
            })
            .AddScoped<SelfUpdatingMessage>()
            .AddSingleton(transmissionConfiguration)
            .AddSingleton<RestSharp.Serializers.ISerializer, NewtonsoftJsonSerializer>()
            .AddSingleton<RestSharp.Deserializers.IDeserializer, NewtonsoftJsonSerializer>()
            .AddScoped<ITransmissionService, TransmissionService>()
            .AddScoped<ICommandFactory, CommandFactory>()
            .BuildServiceProvider();

#if DEBUG
            serviceProvider
            .GetService<ILoggerFactory>()
            .AddConsole(LogLevel.Trace, true);
#else
            serviceProvider
            .GetService<ILoggerFactory>()
            .AddConsole(LogLevel.Information, true);
#endif

            using (var service = serviceProvider.GetService<ITelegramService>())
            {

                service.Register();

                service.StayingAlive();

                _completionEvent.WaitOne();
            }
        }
        private class TelegramConfiguration : ITelegramConfiguration
        {
            public string Apikey => _configurationRoot["telegram:apiKey"];
        }
    }
}
