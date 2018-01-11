using Newtonsoft.Json;
using System.Collections.Generic;

namespace TransmissionRemoteBot.Domain.Transmission.Common
{
    public abstract class CommunicateBase<T>
    {
        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty("arguments")]
        public T Arguments;

        /// <summary>
        /// Number (id)
        /// </summary>
        [JsonProperty("tag")]
        public int Tag;
    }
}
