using System;
using System.Threading.Tasks;
using Xunit;

namespace Pidget.Client.Test
{
    public class SentryClientTests
    {
        public static readonly Dsn Dsn = DsnTests.SentryDsn;

        [Fact]
        public async Task Capture_InvokesSend()
        {
            // Arrange

            var didSend = false;

            var client = new TestableSentryClient(Dsn, e =>
            {
                didSend = true;
                return null;
            });

            // Act

            await client.CaptureAsync(e => e
                .SetException(new Exception()));

            // Assert

            Assert.True(didSend);
        }
    }
}
