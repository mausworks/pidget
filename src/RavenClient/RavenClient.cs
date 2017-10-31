using System;
using System.Net.Http;
using System.Threading.Tasks;
using Pidget.Client.DataModels;

namespace Pidget.Client
{
    public abstract class RavenClient
    {
        public const string DefaultName = "mausworks-raven";

        public Dsn Dsn { get; }

        public virtual string Name => DefaultName;

        public abstract string Version { get; }

        protected RavenClient(Dsn dsn)
        {
            Assert.ArgumentNotNull(dsn, nameof(dsn));

            Dsn = dsn;
        }

        public Task<string> CaptureAsync(
            Action<SentryEventBuilder> builderAccessor)
        {
            var builder = new SentryEventBuilder();
            builderAccessor(builder);

            return SendEventAsync(builder.Build());
        }

        protected abstract Task<string> SendEventAsync(
            SentryEventData sentryEvent);
    }
}
