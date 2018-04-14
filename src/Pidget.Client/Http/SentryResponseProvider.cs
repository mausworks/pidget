using Pidget.Client.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pidget.Client.Http
{
    public class SentryResponseProvider
    {
        public const string SentryErrorHeaderName = "X-Sentry-Error";

        public JsonStreamSerializer Serializer { get; }

        public SentryResponseProvider(JsonStreamSerializer serializer)
            => Serializer = serializer;

        public async Task<SentryResponse> GetResponseAsync(
            HttpResponseMessage response)
        {
            if (!ShouldReadBody(response.Content))
            {
                return GetErrorResponse(response);
            }

            return await GetErrorResponseWithBodyAsync(response);
        }

        public string GetSentryError(HttpResponseMessage response)
        {
            var headerExists = response.Headers.TryGetValues(
                name: SentryErrorHeaderName,
                values: out IEnumerable<string> values);

            return headerExists ? string.Join(", ", values) : null;
        }

        private async Task<SentryResponse> GetErrorResponseWithBodyAsync(
            HttpResponseMessage response)
        {
            using (var body = await ReadBodyAsync(response)
                .ConfigureAwait(false))
            {
                var responseData = Serializer
                    .Deserialize<SentryResponse>(body);

                responseData.StatusCode = (int)response.StatusCode;
                responseData.SentryError = GetSentryError(response);
                responseData.RetryAfter = GetRetryAfter(response);

                return responseData;
            }
        }

        private SentryResponse GetErrorResponse(HttpResponseMessage response)
            => new SentryResponse
            {
                StatusCode = (int)response.StatusCode,
                SentryError = GetSentryError(response),
                RetryAfter = GetRetryAfter(response)
            };

        private DateTimeOffset GetRetryAfter(HttpResponseMessage response)
        {
            var retryAfter = response.Headers.RetryAfter;

            if (retryAfter == null)
            {
                return default(DateTimeOffset);
            }

            return retryAfter.Date ?? (retryAfter.Delta.HasValue
                ? DateTimeOffset.UtcNow + retryAfter.Delta.Value
                : default(DateTimeOffset));
        }

        private bool ShouldReadBody(HttpContent content)
            => content != null
            && content.Headers.ContentLength > 0
            && content.Headers.ContentType.MediaType
                .Equals("application/json", StringComparison.OrdinalIgnoreCase);

        private static Task<Stream> ReadBodyAsync(HttpResponseMessage httpResponse)
            => httpResponse.Content.ReadAsStreamAsync();
    }
}
