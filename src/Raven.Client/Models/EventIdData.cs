using Newtonsoft.Json;

namespace Raven.Client.Models
{
    public class EventIdData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
