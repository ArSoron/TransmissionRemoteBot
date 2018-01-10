///https://bytefish.de/blog/restsharp_custom_json_serializer/
using Newtonsoft.Json;
using RestSharp;
using System.IO;

namespace TransmissionRemoteBot.TransmissionService
{
    public class NewtonsoftJsonSerializer : RestSharp.Serializers.ISerializer, RestSharp.Deserializers.IDeserializer
    {
        private readonly JsonSerializer serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };

        public string DateFormat { get; set; }
        public string Namespace { get; set; }
        public string RootElement { get; set; }

        public string ContentType
        {
            get { return "application/json"; }
            set { }
        }

        public T Deserialize<T>(IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }
    }
}
