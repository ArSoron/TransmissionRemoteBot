using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransmissionRemoteBot.Domain.Transmission.Common;
using TransmissionRemoteBot.Domain.Transmission.Entity;

//https://trac.transmissionbt.com/browser/trunk/extras/rpc-spec.txt
namespace TransmissionRemoteBot.Services.Transmission
{
    public class TransmissionService : ITransmissionService
    {
        private readonly ILogger<TransmissionService> _logger;
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;

        public TransmissionService(ILoggerFactory loggerFactory, ISerializer serializer, IDeserializer deserializer)
        {
            _logger = loggerFactory.CreateLogger<TransmissionService>();
            _serializer = serializer;
            _deserializer = deserializer;
            _logger.LogInformation("Initialized");
        }

        public async Task<TorrentInfoBase> AddTorrentAsync(ITransmissionConfiguration config, Uri uri)
        {
            var client = CreateClient(config);
            var request = new RestRequest() { Method = Method.POST };
            request.JsonSerializer = _serializer;
            request.AddJsonBody(new TransmissionRequest("torrent-add", new NewTorrent() {
                Filename = uri.ToString()
            }));
            try
            {
                IRestResponse<TransmissionResponse<TorrentAddedResponse>> response = await client.ExecuteTaskWithCsrfCheckAsync<TransmissionResponse<TorrentAddedResponse>>(request);
                if (response.IsSuccessful)
                {
                    return response.Data.Arguments?.TorrentAdded;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch data from server");
            }
            return null;
        }

        public async Task<Statistic> GetStatusAsync(ITransmissionConfiguration config)
        {
            var client = CreateClient(config);
            var request = new RestRequest() { Method = Method.POST};
            request.JsonSerializer = _serializer;
            request.AddJsonBody(new TransmissionRequest("session-stats"));

            try
            {
                var response = await client.ExecuteTaskWithCsrfCheckAsync<TransmissionResponse<Statistic>>(request);
                if (response.IsSuccessful)
                {
                    return response.Data.Arguments;
                }
            }
            catch (Exception ex){
                _logger.LogError(ex,"Failed to fetch data from server");
            }
            return null;
        }

        public async Task<IEnumerable<TorrentInfo>> GetTorrentsAsync(ITransmissionConfiguration config)
        {
            var client = CreateClient(config);
            var request = new RestRequest() { Method = Method.POST };
            request.JsonSerializer = _serializer;
            request.AddJsonBody(new TransmissionRequest("torrent-get", new TorrentRequestArguments { Fields = TorrentFields.ALL_FIELDS }));

            try
            {
                var response = await client.ExecuteTaskWithCsrfCheckAsync<TransmissionResponse<TorrentsResponse>>(request);
                if (response.IsSuccessful)
                {
                    return response.Data.Arguments.Torrents;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch data from server");
            }
            return null;
        }

        private RestClient CreateClient(ITransmissionConfiguration config)
        {
            var client = new RestClient(config.Url)
            {
                Authenticator = new HttpBasicAuthenticator(config.Login, config.Password),
            };

            // Override with Newtonsoft JSON Handler
            client.AddHandler("application/json", _deserializer);
            client.AddHandler("text/json", _deserializer);
            client.AddHandler("text/x-json", _deserializer);
            client.AddHandler("text/javascript", _deserializer);
            client.AddHandler("*+json", _deserializer);

            return client;
        }
    }
}
