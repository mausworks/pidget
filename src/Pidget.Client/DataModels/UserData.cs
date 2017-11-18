using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pidget.Client.DataModels
{
    public class UserData : ArbitraryData
    {
        public string Id
        {
            get => Get("id") as string;
            set => Set("id", value);
        }

        public string UserName
        {
            get => Get("username") as string;
            set => Set("username", value);
        }

        public string Email
        {
            get => Get("email") as string;
            set => Set("email", value);
        }

        public string IpAddress
        {
            get => Get("ip_address") as string;
            set => Set("ip_address", value);
        }
    }
}
