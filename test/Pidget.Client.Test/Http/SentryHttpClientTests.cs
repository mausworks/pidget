using Moq;
using Newtonsoft.Json;
using Pidget.Client.DataModels;
using Pidget.Client.Http;
using Pidget.Client.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pidget.Client.Test
{
    using static CancellationToken;

    public class SentryHttpClientTests
    {
        public const string EventId = "event_id";

        public static SentryResponse OkResponse { get; }
            = new SentryResponse
            {
                EventId = EventId
            };

        [Fact]
        public void RequiresHttpClient()
            => Assert.Throws<ArgumentNullException>(()
                => new SentryHttpClient(DsnTests.SentryDsn, null));

        [Fact]
        public void Sender_HasExpectedUserAgent()
        {
            var httpClient = SentryHttpClient.CreateDefaultHttpClient();

            Assert.Equal(SentryHttpClient.DefaultUserAgent,
                httpClient.DefaultRequestHeaders.UserAgent.ToString());
        }

        [Theory, InlineData("foo")]
        public async Task SendMessageEvent(string message)
        {
            var senderMock = new Mock<HttpClient>();

            var client = new SentryHttpClient(DsnTests.SentryDsn,
                senderMock.Object);

            senderMock.Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>(), None))
                .ReturnsAsync(CreateOkHttpResponse(JsonSerializer.CreateDefault()))
                .Verifiable();

            var response = await client.SendEventAsync(new SentryEventData
            {
                Message = message
            });

            senderMock.Verify();

            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.EventId);
            Assert.Null(response.SentryError);
        }

        [Fact]
        public async Task DisabledClient_ReturnsEmptyResponse()
        {
            var senderMock = new Mock<HttpClient>();

            var client = new SentryHttpClient(null, senderMock.Object);

            var response = await client.SendEventAsync(
                new SentryEventData());

            senderMock.Verify(s => s.SendAsync(It.IsAny<HttpRequestMessage>(),
                It.IsAny<CancellationToken>()),
                Times.Never());

            Assert.NotNull(response);
            Assert.Null(response.EventId);
        }

        [Fact]
        public void CreateDefault()
            => Assert.NotNull(
                SentryHttpClient.Default(DsnTests.SentryDsn));

        [Fact(Skip = "Manual testing only")]
        public async Task SendException_ReturnsEventId()
        {
            var client = Sentry.CreateClient(Dsn.Create(GetProductionDsn()));

            var value = 0;

            try
            {
                var x = 10 / value;
            }
            catch (Exception ex)
            {
                var response = await client.CaptureAsync(e => e
                    .SetException(ex)
                    .SetErrorLevel(ErrorLevel.Warning)
                    .AddExtraData("test", new
                    {
                        IsTest = true
                    })
                    .AddTag("test_tag", "yes"));

                Assert.NotNull(response.EventId);
            }
        }

        private static HttpResponseMessage CreateOkHttpResponse(
            JsonSerializer serializer)
        {
            var streamSerializer = new JsonStreamSerializer(Sentry.ApiEncoding,
                serializer);

            var content = new StreamContent(
                streamSerializer.Serialize(OkResponse));

            content.Headers.Add("Content-Type", "application/json");

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            };
        }

        private static string GetProductionDsn()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                "dsn.txt");

            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllText(filePath).Trim();
        }
    }
}
