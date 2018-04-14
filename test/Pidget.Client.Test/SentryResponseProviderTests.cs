using Newtonsoft.Json;
using Pidget.Client.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Pidget.Client.Http
{
    public class SentryResponseProviderTests
    {
        /**
         * The sentry response provider provides
         * a SentryResponse from a HttpResponseMessage
         *
         * It provides things like: The event ID, HTTP status code, 
         * and the X-Sentry-Error and- Retry-After header.
         * It then abstracts these into a model, and returns it.
         *
         * Philosophy:
         *  Be forgiving: Attempt to provide each component independent of each other.
         *  E.g. The body should be parsed independent of whatever status-code is served.
         *  The idea as much information as possible from the HTTP response.
         *
         *  Be paranoid: take care when reading the HttpResponse.
         *  E.g. do not attempt to read the body as JSON, if the response
         *  is served with a non-JSON Content-Type.
         */

        public const string SentryErrorHeaderName = "X-Sentry-Error";

        public static JsonStreamSerializer Serializer
            = new JsonStreamSerializer(Sentry.ApiEncoding,
                JsonSerializer.CreateDefault());

        public SentryResponseProvider ResponseProvider
            = new SentryResponseProvider(Serializer);

        public const string ExampleEventId = "EVENT_ID";

        public static readonly string ExampleJson =
            $"{{ \"id\": \"{ExampleEventId}\" }}";

        public async Task ParsesExampleEventId()
        {
            var response = new HttpResponseMessage
            {
                Content = JsonContent(ExampleJson)
            };

            var sentryResponse = await ResponseProvider.GetResponseAsync(response);

            Assert.Equal(ExampleEventId, sentryResponse.EventId);
            Assert.Equal(ExampleEventId, sentryResponse["id"]);
        }


        [Theory, InlineData(200)]
        public async Task CapturesStatusCode(int statusCode)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)statusCode
            };

            var sentryResponse = await ResponseProvider
                .GetResponseAsync(response);

            Assert.Equal(statusCode, sentryResponse.StatusCode);
        }

        [Theory]
        [InlineData("text/plain")]
        public async Task HandlesBadContentTypeSafely(string contentType)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(ExampleJson,
                    Sentry.ApiEncoding,
                    contentType)
            };

            var sentryResponse = await ResponseProvider
                .GetResponseAsync(response);

            // The idea is to not attempt to parse the
            // body if it's not 'application/json'
            Assert.Null(sentryResponse.EventId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("NOT_JSON")]
        public async Task HandlesBadContentGracefully(string badContent)
        {
            var response = new HttpResponseMessage
            {
                Content = JsonContent(badContent)
            };

            var sentryResponse = await ResponseProvider
                .GetResponseAsync(response);

            Assert.Null(sentryResponse.EventId);
        }

        [Theory, InlineData("foo")]
        public async Task CapturesSentryErrorHeader(string error)
        {
            var response = new HttpResponseMessage();
            response.Headers.Add(SentryErrorHeaderName, error);

            var sentryResponse = await ResponseProvider.GetResponseAsync(response);

            Assert.Equal(error, sentryResponse.SentryError);
        }

        [Theory, InlineData(10)]
        public Task CapturesRetryAfterHeader_AsDelta(int deltaSeconds)
            => RetryAfter_Compare(deltaSeconds,
                new RetryConditionHeaderValue(TimeSpan.FromSeconds(deltaSeconds)));

        [Theory, InlineData(10)]
        public Task CapturesRetryAfterHeader_AsDate(int deltaSeconds)
            => RetryAfter_Compare(deltaSeconds,
                new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddSeconds(deltaSeconds)));

        private async Task RetryAfter_Compare(int deltaSeconds,
            RetryConditionHeaderValue headerValue)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)429
            };

            response.Headers.RetryAfter = headerValue;

            var sentryResponse = await ResponseProvider
                .GetResponseAsync(response);

            // This is perhaps a "dirty compare", but useful.
            Assert.Equal(
                expected: (DateTimeOffset.UtcNow + TimeSpan.FromSeconds(deltaSeconds))
                    .ToString("yyyyMMddHHmmss"),
                actual: sentryResponse.RetryAfter
                    .ToString("yyyyMMddHHmmss"));
        }

        private StringContent JsonContent(string json)
            => new StringContent(json,
                Sentry.ApiEncoding,
                "application/json");
    }
}
