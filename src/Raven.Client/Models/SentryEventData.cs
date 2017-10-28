using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Raven.Client.Models
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

        public static SentryEventData FromEvent(SentryEvent sentryEvent)
        {
            var exceptionData = ExceptionData
                .FromException(sentryEvent.Exception);

            return new SentryEventData
            {
                Level = sentryEvent.ErrorLevel,
                Timestamp = DateTime.UtcNow,
                Culprit = sentryEvent.Transaction ?? exceptionData.Module,
                Platform = Sentry.CSharpPlatformIdentifier,
                Logger = Sentry.DefaultLoggerName,
                Exception = exceptionData,
                Message = sentryEvent.Message,
                Tags = sentryEvent.Tags.ToDictionary(t => t.Key, t => t.Value),
                Extra = sentryEvent.ExtraData.ToDictionary(t => t.Key, t => t.Value),
                Fingerprint = sentryEvent.Fingerprint.ToList()
            };
        }
    }
}
