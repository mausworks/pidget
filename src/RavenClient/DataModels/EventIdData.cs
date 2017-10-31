using Newtonsoft.Json;

namespace Pidget.Client.DataModels
{
    public class EventIdData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
