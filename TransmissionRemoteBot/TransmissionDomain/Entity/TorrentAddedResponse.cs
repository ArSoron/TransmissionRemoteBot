using Newtonsoft.Json;
using TransmissionRemoteBot.Domain.Common;

namespace TransmissionRemoteBot.Domain.Entity
{
    public class TorrentAddedResponse : IResponseArguments
    {
        [JsonProperty("torrent-added")]
        public TorrentInfoBase TorrentAdded { get; set; }
    }
}
