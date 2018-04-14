using Pidget.Client.DataModels;
using System;
using System.Threading.Tasks;

namespace Pidget.Client
{
    public abstract class SentryClient
    {
        /// <summary>
        /// The name of the client, used in e.g. the User-Agent.
        /// </summary>
        public const string Name = "pidget";

        /// <summary>
        /// The current version of Pidget.
        /// </summary>
        public static string Version => VersionNumber.Get();

        /// <summary>
        /// The DSN which determines which project this client is configured for.
        /// </summary>
        public Dsn Dsn { get; }

        /// <summary>
        /// Determines whether the client can send events.
        /// The client gets disabled if a DSN of null is provided.
        /// </summary>
        public bool IsEnabled => Dsn != null;

        /// <summary>
        /// Sets the provided DSN for the client.
        /// </summary>
        protected SentryClient(Dsn dsn)
            => Dsn = dsn;

        /// <summary>
        /// Builds and captures a sentry event, using <see cref="SendEventAsync" />
        /// </summary>
        /// <param name="builderConfig">Configures the event to send.</param>
        public Task<SentryResponse> CaptureAsync(
            Action<SentryEventBuilder> builderConfig)
        {
            var builder = new SentryEventBuilder();
            builderConfig(builder);

            return SendEventAsync(builder.Build());
        }

        /// <summary>
        /// Sends the provided event data to Sentry and returns its response.
        /// </summary>
        /// <param name="sentryEvent">The sentry event data</param>
        public abstract Task<SentryResponse> SendEventAsync(
            SentryEventData sentryEvent);
    }
}
