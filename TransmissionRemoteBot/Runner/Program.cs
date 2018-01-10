using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using TransmissionRemoteBot.TelegramService;
using TransmissionRemoteBot.TransmissionService;

namespace TransmissionRemoteBot.Runner
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
            .AddSingleton<ITelegramService, TelegramService.TelegramService>()
            .AddSingleton<ITelegramConfiguration, TelegramConfiguration>()
            .AddSingleton(transmissionConfiguration)
            .AddSingleton<RestSharp.Serializers.ISerializer, NewtonsoftJsonSerializer>()
            .AddSingleton<RestSharp.Deserializers.IDeserializer, NewtonsoftJsonSerializer>()
            .AddSingleton<ITransmissionService, TransmissionService.TransmissionService>()
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
