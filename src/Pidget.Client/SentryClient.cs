using Pidget.Client.DataModels;
using System;
using System.Threading.Tasks;

namespace Pidget.Client
{
    public abstract class SentryClient
    {
        public const string Name = "pidget";

        public static string Version { get; }
            = VersionNumber.Get();

        public Dsn Dsn { get; }

        public bool IsEnabled => Dsn != null;

        protected SentryClient(Dsn dsn)
            => Dsn = dsn;

        public Task<SentryResponse> CaptureAsync(
            Action<SentryEventBuilder> builderAccessor)
        {
            var builder = new SentryEventBuilder();
            builderAccessor(builder);

            return SendEventAsync(builder.Build());
        }

        public abstract Task<SentryResponse> SendEventAsync(
            SentryEventData sentryEvent);
    }
}
