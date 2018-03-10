using Newtonsoft.Json;
using System.Collections.Generic;
using TransmissionRemoteBot.Domain.Transmission.Common;

namespace TransmissionRemoteBot.Domain.Transmission.Entity
{
    public class TorrentsResponse : IResponseArguments
    {
        [JsonProperty("torrents")]
        public IEnumerable<TorrentInfo> Torrents { get; set; }
    }
}
