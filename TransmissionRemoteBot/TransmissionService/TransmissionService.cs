using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TransmissionRemoteBot.TransmissionService
{
    public class TransmissionService : ITransmissionService
    {
        private readonly ILogger<TransmissionService> _logger;

        public TransmissionService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TransmissionService>();
            _logger.LogInformation("Initialized");
        }
        public async Task<TransmissionStatus> GetStatusAsync(ITransmissionConfiguration config)
        {
            var client = new RestClient(config.Url)
            {
                Authenticator = new HttpBasicAuthenticator(config.Login, config.Password),
                
            };
            var request = new RestRequest() { Method = Method.POST};
            request.AddJsonBody(new
            {
                method = "session-stats"
            });

            try
            {
                IRestResponse<TransmissionStatus> response = await client.ExecuteTaskWithCsrfCheckAsync<TransmissionStatus>(request);
                if (response.IsSuccessful)
                {
                    return response.Data;
                }
            }
            catch (Exception ex){
                _logger.LogError(ex,"Failed to fetch data from server");
            }
            return null;
        }
    }
}
