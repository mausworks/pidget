using Pidget.Client.DataModels;
using System.Net;
using System;

namespace Pidget.Client
{
    public class SentryResponse : ArbitraryData
    {
        public static SentryResponse Empty => new SentryResponse();

        public string EventId
        {
            get => this["id"] as string;
            set => this["id"] = value;
        }

        public string SentryError { get; set; }

        public int StatusCode { get; set; }

        public TimeSpan RetryAfter { get; set; }
    }
}
