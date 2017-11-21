using System.Collections.Generic;
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
            HttpResponseMessage httpResponse)
        {
            using (var body = await httpResponse.Content.ReadAsStreamAsync())
            {
                var responseData = Serializer
                    .Deserialize<SentryResponse>(body);

                responseData.HttpStatusCode = httpResponse.StatusCode;
                responseData.SentryError = GetSentryError(httpResponse);

                return responseData;
            }
        }

        public string GetSentryError(HttpResponseMessage response)
        {
            var header = response.Headers.TryGetValues(
                name: SentryErrorHeaderName,
                values: out IEnumerable<string> values);

            return values != null && values.Any()
                ? string.Join(", ", values)
                : null;
        }
    }
}
