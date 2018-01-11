using Newtonsoft.Json;

namespace TransmissionRemoteBot.Domain.Transmission.Common
{
    /// <summary>
    /// Transmission request 
    /// </summary>
    public class TransmissionRequest : CommunicateBase<IRequestArguments>
	{
		/// <summary>
		/// Name of the method to invoke
		/// </summary>
        [JsonProperty("method")]
		public string Method { get; set; }

        public TransmissionRequest(string method)
        {
            Method = method;
        }

		public TransmissionRequest(string method, IRequestArguments arguments)
		{
            Method = method;
            Arguments = arguments;
		}
	}
}
