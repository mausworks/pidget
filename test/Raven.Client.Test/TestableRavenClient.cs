using System;
using System.Threading.Tasks;
using Raven.Client.DataModels;

namespace Raven.Client.Test
{
    public class TestableRavenClient : RavenClient
    {

        public override string Name => nameof(TestableRavenClient);

        public override string Version => "1.0.0";

        private readonly Func<SentryEventData, string> _onSend;

        public TestableRavenClient(Dsn dsn,
            Func<SentryEventData, string> onSend) : base(dsn)
            => _onSend = onSend;

        protected override Task<string> SendEventAsync(SentryEventData sentryEvent)
            => Task.FromResult(_onSend(sentryEvent));
    }
}
