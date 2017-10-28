using System;
using System.Threading.Tasks;
using Xunit;

namespace Raven.Client.Test
{
    public class RavenClientTests
    {
        public static readonly Dsn Dsn = DsnTests.SentryDsn;

        [Fact]
        public async Task Capture_InvokesSend()
        {
            // Arrange

            var didSend = false;

            var client = new TestableRavenClient(Dsn, e =>
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
