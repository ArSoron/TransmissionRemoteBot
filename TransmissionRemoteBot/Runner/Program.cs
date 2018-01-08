using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using TransmissionRemoteBot.Logic;

namespace TransmissionRemoteBot.Runner
{
    class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IService, Service>()
            .BuildServiceProvider();

            serviceProvider
            .GetService<ILoggerFactory>()
            .AddConsole(LogLevel.Information);

            var service = serviceProvider.GetService<IService>();
            service.StayingAlive();

        }
    }
}
