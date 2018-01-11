using Newtonsoft.Json;
using TransmissionRemoteBot.Domain.Transmission.Common;

namespace TransmissionRemoteBot.Domain.Transmission.Entity
{
    /// <summary>
    /// Information of added torrent
    /// </summary>
    public class TorrentInfoBase
	{
		/// <summary>
		/// Torrent ID
		/// </summary>
		[JsonProperty("id")]
		public int ID { get; set; }

		/// <summary>
		/// Torrent name
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Torrent Hash
		/// </summary>
		[JsonProperty("hashString")]
		public string HashString { get; set; }

	}
}
