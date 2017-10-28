using System;
using System.Threading.Tasks;

namespace Raven.Client.Test
{
    public class TestableRavenClient : RavenClient
    {

        public override string Name => nameof(TestableRavenClient);

        public override string Version =>
            throw new NotImplementedException();

        private readonly Func<SentryEvent, string> _onSend;

        public TestableRavenClient(Dsn dsn,
            Func<SentryEvent, string> onSend) : base(dsn)
            => _onSend = onSend;

        protected override Task<string> SendEventAsync(SentryEvent sentryEvent)
            => Task.FromResult(_onSend(sentryEvent));
    }
}
