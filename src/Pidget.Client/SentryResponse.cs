using Pidget.Client.DataModels;
using System.Net;
using System;

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

        public int StatusCode { get; set; }

        public DateTimeOffset RetryAfter { get; set; }

        public static SentryResponse Empty() => new SentryResponse();
    }
}
