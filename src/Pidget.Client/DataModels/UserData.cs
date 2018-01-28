namespace Pidget.Client.DataModels
{
    public class UserData : ArbitraryData
    {
        public string Id
        {
            get => this["id"] as string;
            set => this["id"] = value;
        }

        public string UserName
        {
            get => this["username"] as string;
            set => this["username"] = value;
        }

        public string Email
        {
            get => this["email"] as string;
            set => this["email"] = value;
        }

        public string IpAddress
        {
            get => this["ip_address"] as string;
            set => this["ip_address"] = value;
        }
    }
}
