using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Pidget.Client.DataModels;
using Pidget.Client.Http;
using Pidget.Client.Serialization;
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
        public void RequiresDsn()
            => Assert.Throws<ArgumentNullException>(()
                => new SentryHttpClient(null,
                    SentryHttpClient.CreateSender()));

        [Fact]
        public void HasExpectedUserAgent()
        {
            var httpClient = SentryHttpClient.CreateSender() as HttpClient;

            Assert.Equal(SentryHttpClient.UserAgent,
                httpClient.DefaultRequestHeaders.UserAgent.ToString());
        }

        [Theory, InlineData("foo")]
        public async Task SendMessageEvent(string message)
        {
            var senderMock = new Mock<HttpClient>();

            var client = new SentryHttpClient(DsnTests.SentryDsn,
                senderMock.Object);

            senderMock.Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>(), None))
                .ReturnsAsync(CreateOkHttpResponse(SentryHttpClient.JsonSerializer))
                .Verifiable();

            var response = await client.SendEventAsync(new SentryEventData
            {
                Message = message
            });

            senderMock.Verify();

            Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.NotNull(response.EventId);
            Assert.Null(response.SentryError);
        }

        [Fact]
        public void DisposesSender()
        {
            var sender = new TestSender(new HttpClientHandler());

            var client = new SentryHttpClient(DsnTests.SentryDsn,
                sender);

            client.Dispose();

            Assert.True(sender.IsDisposed);
        }

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

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(streamSerializer.Serialize(OkResponse))
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
