using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pidget.Client.DataModels
{
    public class RequestData
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("data")]
        public IDictionary<string, string> Data { get; set; }

        [JsonProperty("query_string")]
        public string QueryString { get; set; }

        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; }

        [JsonProperty("cookies")]
        public string Cookies { get; set; }

        [JsonProperty("env")]
        public IDictionary<string, string> Environment { get; set; }
    }
}
