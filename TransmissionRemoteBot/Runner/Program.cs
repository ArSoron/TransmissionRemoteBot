using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using TransmissionRemoteBot.StorageService;
using TransmissionRemoteBot.TelegramService;

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

            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<ITelegramConfiguration, TelegramConfiguration>()
            .AddSingleton<IMongoStorageConfiguration, MongoStorageConfiguration>()
            .AddSingleton<ITelegramService, TelegramService.TelegramService>()
            .AddSingleton<IStorageService, MongoStorageService>()
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

        private class MongoStorageConfiguration : IMongoStorageConfiguration
        {
            public string ConnectionString => _configurationRoot["mongo:connectionString"];
        }
    }
}
