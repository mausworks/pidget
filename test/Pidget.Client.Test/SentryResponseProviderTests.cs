using Newtonsoft.Json;
using Pidget.Client.Serialization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Headers;
using Moq;
using System.IO;
using System;

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
        public async Task ReturnsEventIdFromJsonBody(string json,
            string eventId)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(json,
                    Sentry.ApiEncoding,
                    "application/json")
            };

            var sentryResponse = await ResponseProvider.GetResponseAsync(response);

            Assert.Equal(eventId, sentryResponse.EventId);
            Assert.Equal(eventId, sentryResponse["id"]);
        }

        [Theory]
        [InlineData(200, "application/json")]
        [InlineData(200, "text/plain")]
        public async Task EmptyContent(int statusCode, string contentType)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)statusCode,
                Content = new StringContent("",
                    Sentry.ApiEncoding,
                    contentType)
            };

            var sentryResponse = await ResponseProvider
                .GetResponseAsync(response);

            Assert.Equal(200, sentryResponse.StatusCode);
            Assert.Null(sentryResponse.EventId);
        }

        [Theory]
        [InlineData(200, "")]
        [InlineData(200, "NOT_JSON")]
        public async Task WrongContentType(int statusCode, string content)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)statusCode,
                Content = new StringContent(content,
                    Sentry.ApiEncoding,
                    "text/plain")
            };

            var sentryResponse = await ResponseProvider
                .GetResponseAsync(response);

            Assert.Equal(200, sentryResponse.StatusCode);
            Assert.Null(sentryResponse.EventId);
        }

        [Theory, InlineData(HttpStatusCode.OK)]
        public async Task ReturnsStatusCode(HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent("{ }")
            };

            var sentryResponse = await ResponseProvider
                .GetResponseAsync(response);

            Assert.Equal(200, sentryResponse.StatusCode);
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

        [Theory, InlineData(10)]
        public async Task RetryAfter_Delta(int deltaSeconds)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)429,
                ReasonPhrase = "Too Many Requests",
                Content = new StringContent("{ }"),
            };

            response.Headers.RetryAfter = new RetryConditionHeaderValue(
                TimeSpan.FromSeconds(deltaSeconds));

            var sentryResponse = await ResponseProvider.GetResponseAsync(response);

            Assert.Equal(
                expected: deltaSeconds,
                actual: sentryResponse.RetryAfter.Value.TotalSeconds);
        }

        [Theory, InlineData(10)]
        public async Task RetryAfter_Date(int deltaSeconds)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)429,
                ReasonPhrase = "Too Many Requests",
                Content = new StringContent("{ }"),
            };

            response.Headers.RetryAfter = new RetryConditionHeaderValue(
                DateTimeOffset.UtcNow.AddSeconds(deltaSeconds));

            var sentryResponse = await ResponseProvider.GetResponseAsync(response);

            var actualSeconds = sentryResponse.RetryAfter.Value.TotalSeconds;

            Assert.Equal(
                expected: deltaSeconds,
                actual: Math.Round(actualSeconds));
        }
    }
}
