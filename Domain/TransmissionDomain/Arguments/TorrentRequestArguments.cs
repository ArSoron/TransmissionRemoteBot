using Newtonsoft.Json;
using TransmissionRemoteBot.Domain.Transmission.Common;

namespace TransmissionRemoteBot.Domain.Transmission.Entity
{
    /// <summary>
    /// Array of fields from TorrentFields to request from server
    /// </summary>
    public class TorrentRequestArguments : IRequestArguments
    {
        [JsonProperty("ids")]
        public string Ids { get; set; }

        [JsonProperty("fields")]
        public string[] Fields { get; set; }
    }
}
