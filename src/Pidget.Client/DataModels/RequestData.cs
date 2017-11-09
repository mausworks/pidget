using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pidget.Client.DataModels
{
    public class RequestData
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("query_string")]
        public string QueryString { get; set; }

        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; }

        [JsonProperty("environment")]
        public IDictionary<string, string> Environment { get; set; }
    }
}
