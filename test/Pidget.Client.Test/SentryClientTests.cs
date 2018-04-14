using System;
using System.Threading.Tasks;
using Moq;
using Pidget.Client.DataModels;
using Xunit;

namespace Pidget.Client.Test
{
    public class SentryClientTests
    {
        [Fact]
        public async Task CaptureAsync_InvokesSendAsync()
        {
            // Arrange

            var clientMock = new Mock<SentryClient>(null);

            clientMock.Setup(m => m.SendEventAsync(It.IsAny<SentryEventData>()))
                .ReturnsAsync(SentryResponse.Empty)
                .Verifiable();

            // Act

            await clientMock.Object.CaptureAsync(e => e
                .SetException(new Exception()));

            // Assert

            clientMock.Verify();
        }
    }
}
