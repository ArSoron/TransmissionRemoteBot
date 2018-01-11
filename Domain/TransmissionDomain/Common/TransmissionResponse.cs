using Newtonsoft.Json;

namespace TransmissionRemoteBot.Domain.Transmission.Common
{
    /// <summary>
    /// Transmission response 
    /// </summary>
    public class TransmissionResponse<T> : CommunicateBase<T> where T: IResponseArguments
	{
        /// <summary>
        /// Contains "success" on success, or an error string on failure.
        /// </summary>
        [JsonProperty("result")]
        public string Result { get; set; }
	}
}
