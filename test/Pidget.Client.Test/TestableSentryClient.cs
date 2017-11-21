using System;
using System.Threading.Tasks;
using Pidget.Client.DataModels;

namespace Pidget.Client.Test
{
    public class TestableSentryClient : SentryClient
    {
        private readonly Func<SentryEventData, SentryResponse> _onSend;

        public TestableSentryClient(Dsn dsn,
            Func<SentryEventData, SentryResponse> onSend) : base(dsn)
            => _onSend = onSend;

        public override Task<SentryResponse> SendEventAsync(SentryEventData sentryEvent)
            => Task.FromResult(_onSend(sentryEvent));
    }
}
