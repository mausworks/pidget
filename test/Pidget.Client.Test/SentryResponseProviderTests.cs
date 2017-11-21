using Newtonsoft.Json;
using Pidget.Client.Serialization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Pidget.Client.Http
{
    public class SentryResponseProviderTests
    {
        public const string SentryErrorHeaderName = "X-Sentry-Error";

        public static JsonStreamSerializer Serializer
            = new JsonStreamSerializer(Sentry.ApiEncoding,
                JsonSerializer.CreateDefault());

        public SentryResponseProvider ResponseProvider
            = new SentryResponseProvider(Serializer);

        [Theory, InlineData("{ \"id\": \"event_id\" }", "event_id")]
        public async Task ReturnsEventIdFromJsonBody(string jsonBody,
            string eventId)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(jsonBody, Sentry.ApiEncoding)
            };

            var sentryResponse = await ResponseProvider.GetResponseAsync(response);

            Assert.Equal(eventId, sentryResponse.EventId);
            Assert.Equal(eventId, sentryResponse["id"]);
        }

        [Theory, InlineData(HttpStatusCode.OK)]
        public async Task ReturnsStatusCode(HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent("{ }")
            };

            var sentryResponse = await ResponseProvider.GetResponseAsync(response);

            Assert.Equal(statusCode, sentryResponse.HttpStatusCode);
        }

        [Theory, InlineData("foo")]
        public async Task ReturnsSentryError(string error)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent("{ }")
            };

            response.Headers.Add(SentryResponseProvider.SentryErrorHeaderName,
                error);

            var sentryResponse = await ResponseProvider.GetResponseAsync(response);

            Assert.Equal(error, sentryResponse.SentryError);
        }
    }
}
