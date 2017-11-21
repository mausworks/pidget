using System;
using System.Threading.Tasks;
using Pidget.Client.DataModels;

namespace Pidget.Client.Test
{
    public class TestableSentryClient : SentryClient
    {
        public override string Name => nameof(TestableSentryClient);

        public override string Version => "1.0.0";

        private readonly Func<SentryEventData, SentryResponse> _onSend;

        public TestableSentryClient(Dsn dsn,
            Func<SentryEventData, SentryResponse> onSend) : base(dsn)
            => _onSend = onSend;

        public override Task<SentryResponse> SendEventAsync(SentryEventData sentryEvent)
            => Task.FromResult(_onSend(sentryEvent));
    }
}
