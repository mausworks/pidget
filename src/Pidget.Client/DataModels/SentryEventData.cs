using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Pidget.Client.DataModels
{
    public class SentryEventData
    {
        [JsonProperty("event_id")]
        public string EventId { get; set; }

        [JsonProperty("culprit")]
        public string Culprit { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("exception")]
        public ExceptionData Exception { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("fingerprint")]
        public IList<string> Fingerprint { get; set; }

        [JsonProperty("logger")]
        public string Logger { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("level")]
        public ErrorLevel Level { get; set; }

        [JsonProperty("extra")]
        public IDictionary<string, object> Extra { get; set; }

        [JsonProperty("tags")]
        public IDictionary<string, string> Tags { get; set; }

        [JsonProperty("request")]
        public RequestData Request { get; set; }

        [JsonProperty("user")]
        public UserData User { get; set; }

        [JsonProperty("contexts")]
        public ContextsData Contexts { get; set; }
    }
}
