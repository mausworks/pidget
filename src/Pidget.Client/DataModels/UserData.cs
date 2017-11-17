using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pidget.Client.DataModels
{
    public class UserData : ArbitraryData
    {
        public string Id
        {
            get => (string)Get("id");
            set => Set("id", value);
        }

        public string UserName
        {
            get => (string)Get("username");
            set => Set("username", value);
        }

        [JsonProperty("email")]
        public string Email
        {
            get => (string)Get("email");
            set => Set("email", value);
        }

        [JsonProperty("ip_address")]
        public string IpAddress
        {
            get => (string)Get("ip_address");
            set => Set("ip_address", value);
        }
    }
}
