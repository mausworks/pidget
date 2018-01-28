using Pidget.Client.DataModels;
using System.Net;

namespace Pidget.Client
{
    public class SentryResponse : ArbitraryData
    {
        public string EventId
        {
            get => this["id"] as string;
            set => this["id"] = value;
        }

        public string SentryError { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
