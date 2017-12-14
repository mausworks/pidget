using System;
using System.Threading.Tasks;
using Pidget.Client.Http;
using Xunit;

namespace Pidget.Client.Test
{
    public class SentryTests
    {
        [Fact]
        public void CreateClient_IsSentryHttpClient()
            => Assert.True(Sentry.CreateClient(DsnTests.SentryDsn) is SentryHttpClient);
    }
}
