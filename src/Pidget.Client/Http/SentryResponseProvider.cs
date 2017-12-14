using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Pidget.Client.DataModels;
using Pidget.Client.Serialization;

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
            using (var body = await ReadBodyAsync(response)
                .ConfigureAwait(false))
            {
                var responseData = ParseSentryResponse(body);

                responseData.HttpStatusCode = response.StatusCode;
                responseData.SentryError = GetSentryError(response);

                return responseData;
            }
        }

        private static Task<Stream> ReadBodyAsync(HttpResponseMessage httpResponse)
            => httpResponse.Content.ReadAsStreamAsync();

        private SentryResponse ParseSentryResponse(Stream body)
        {
            if  (body == null || !body.CanRead)
            {
                return new SentryResponse();
            }

            return Serializer.Deserialize<SentryResponse>(body);
        }

        public string GetSentryError(HttpResponseMessage response)
        {
            var headerExists = response.Headers.TryGetValues(
                name: SentryErrorHeaderName,
                values: out IEnumerable<string> values);

            return headerExists && values != null && values.Any()
                ? string.Join(", ", values)
                : null;
        }
    }
}
