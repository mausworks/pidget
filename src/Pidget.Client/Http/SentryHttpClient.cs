using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pidget.Client.DataModels;
using Pidget.Client.Serialization;

namespace Pidget.Client.Http
{
    public class SentryHttpClient : SentryClient, IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly JsonStreamSerializer _serializer;

        public SentryHttpClient(Dsn dsn)
            : base(dsn)
        {
            _httpClient = CreateHttpClient();
            _serializer = new JsonStreamSerializer(
                encoding: Sentry.ApiEncoding,
                jsonSerializer: GetJsonSerializer());
        }

        private static JsonSerializer GetJsonSerializer()
        {
            var settings = new JsonSerializerSettings();

            settings.Converters.Add(
                new StringEnumConverter(camelCaseText: true));

            return JsonSerializer.Create(settings);
        }

        public override async Task<SentryResponse> SendEventAsync(
            SentryEventData eventData)
        {
            using (var stream = _serializer.Serialize(eventData))
            {
                var httpResponse = await _httpClient.SendAsync(
                    GetRequest(IssueAuth(), stream)).ConfigureAwait(false);

                var responseProvider = new SentryResponseProvider(_serializer);

                return await responseProvider.GetResponseAsync(httpResponse);
            }
        }

        public void Dispose()
            => _httpClient.Dispose();

        private SentryAuth IssueAuth()
            => SentryAuth.Issue(this, DateTimeOffset.UtcNow);

        private HttpRequestMessage GetRequest(SentryAuth auth, Stream stream)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                Dsn.GetCaptureUrl());

            request.Headers.Add(SentryAuthHeader.Name,
                SentryAuthHeader.GetValues(auth));

            request.Content = GetContent(stream);

            return request;
        }

        private static StreamContent GetContent(Stream stream)
        {
            var content = new StreamContent(stream);

            content.Headers.Add("Content-Type", "application/json");

            return content;
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders
                .Add("User-Agent", $"{Name}/{Version}");

            return client;
        }
    }
}
