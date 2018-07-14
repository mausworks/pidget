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
                var responseData = DeserializeBody(body);

                responseData.StatusCode = (int)response.StatusCode;
                responseData.SentryError = GetSentryError(response);
                responseData.RetryAfter = GetRetryAfter(response);

                return responseData;
            }
        }

        private SentryResponse DeserializeBody(Stream body)
        {
            try
            {
                return Serializer.Deserialize<SentryResponse>(body);
            }
            catch
            {
                return SentryResponse.Empty();
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
                return default;
            }

            return retryAfter.Date ?? (retryAfter.Delta.HasValue
                ? DateTimeOffset.UtcNow + retryAfter.Delta.Value
                : default);
        }

        private bool ShouldReadBody(HttpContent content)
            => content != null
            && content.Headers.ContentLength > 0
            && IsJsonMediaType(content.Headers.ContentType.MediaType);

        private bool IsJsonMediaType(string mediaType)
            => mediaType != null
            && mediaType.Equals("application/json", StringComparison.Ordinal);

        private static Task<Stream> ReadBodyAsync(HttpResponseMessage httpResponse)
            => httpResponse.Content.ReadAsStreamAsync();
    }
}
