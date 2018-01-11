using Newtonsoft.Json;
using TransmissionRemoteBot.Domain.Transmission.Common;

namespace TransmissionRemoteBot.Domain.Transmission.Entity
{
    public class TorrentAddedResponse : IResponseArguments
    {
        [JsonProperty("torrent-added")]
        public TorrentInfoBase TorrentAdded { get; set; }
    }
}
