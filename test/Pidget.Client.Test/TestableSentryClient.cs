using System;
using System.Threading.Tasks;
using Pidget.Client.DataModels;

namespace Pidget.Client.Test
{
    public class TestableSentryClient : SentryClient
    {

        public override string Name => nameof(TestableSentryClient);

        public override string Version => "1.0.0";

        private readonly Func<SentryEventData, string> _onSend;

        public TestableSentryClient(Dsn dsn,
            Func<SentryEventData, string> onSend) : base(dsn)
            => _onSend = onSend;

        protected override Task<string> SendEventAsync(SentryEventData sentryEvent)
            => Task.FromResult(_onSend(sentryEvent));
    }
}
