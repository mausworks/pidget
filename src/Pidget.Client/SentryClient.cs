using System;
using System.Net.Http;
using System.Threading.Tasks;
using Pidget.Client.DataModels;

namespace Pidget.Client
{
    public abstract class SentryClient
    {
        public const string DefaultName = "pidget";

        public Dsn Dsn { get; }

        public virtual string Name => DefaultName;

        public abstract string Version { get; }

        protected SentryClient(Dsn dsn)
        {
            Assert.ArgumentNotNull(dsn, nameof(dsn));

            Dsn = dsn;
        }

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
