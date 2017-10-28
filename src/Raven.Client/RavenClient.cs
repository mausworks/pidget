using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Raven.Client
{
    public abstract class RavenClient
    {
        public const string DefaultName = "mausworks-raven";

        public Dsn Dsn { get; }

        public virtual string Name => DefaultName;

        public abstract string Version { get; }

        protected RavenClient(Dsn dsn)
            => Dsn = dsn;

        public Task<string> CaptureAsync(
            Action<SentryEventBuilder> builderAccessor)
        {
            var builder = new SentryEventBuilder();
            builderAccessor(builder);

            return SendEventAsync(builder.Build());
        }

        protected abstract Task<string> SendEventAsync(
            SentryEvent sentryEvent);
    }
}
