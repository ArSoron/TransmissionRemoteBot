using RestSharp;
using System;
using System.Collections.Concurrent;

using System.Linq;
using System.Threading.Tasks;

namespace TransmissionRemoteBot.Services.Transmission
{
    public static class RestClientExtensions
    {
        private const string csrfHeaderName = "X-Transmission-Session-Id";
        private static readonly ConcurrentDictionary<Uri, string> knownCsrfs = new ConcurrentDictionary<Uri, string>();
        public static async Task<IRestResponse<T>> ExecuteTaskWithCsrfCheckAsync<T>(this RestClient restClient, IRestRequest request, string csrfHeader = null)
        {
            if (!string.IsNullOrEmpty(csrfHeader)) {
                request.AddHeader(csrfHeaderName, csrfHeader);
            } else if (knownCsrfs.TryGetValue(restClient.BaseUrl, out string storedCsrfHeader))
            {
                request.AddHeader(csrfHeaderName, storedCsrfHeader);
            }
            var response = await restClient.ExecuteTaskAsync<T>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                string receivedCsrfHeader = response.Headers.FirstOrDefault(header => header.Name == csrfHeaderName).Value.ToString();
                knownCsrfs[restClient.BaseUrl] = receivedCsrfHeader;
                request.AddHeader(csrfHeaderName, receivedCsrfHeader);
                response = await restClient.ExecuteTaskAsync<T>(request);
            }
            return response;
        }
    }
}
