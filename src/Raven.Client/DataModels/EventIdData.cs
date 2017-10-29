using Newtonsoft.Json;

namespace Raven.Client.DataModels
{
    public class EventIdData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
