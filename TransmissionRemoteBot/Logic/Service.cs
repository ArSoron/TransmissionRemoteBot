using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace TransmissionRemoteBot.Logic
{
    public class Service : IService
    {
        private readonly ILogger<Service> _logger;
        public Service(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<Service>();
        }
        public void StayingAlive()
        {
            var message = $"I'm alive at {DateTime.UtcNow}";
#if DEBUG
            Debug.Write(message);
#endif
            _logger.LogInformation(message);
        }
    }
}
