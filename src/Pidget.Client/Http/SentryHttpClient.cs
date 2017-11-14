using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Pidget.Client.DataModels;
using Pidget.Client.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pidget.Client.Http
{
    public class SentryHttpClient : SentryClient, IDisposable
    {
        public override string Version { get; }

        private readonly HttpClient _httpClient;

        private readonly JsonStreamSerializer _serializer;

        public SentryHttpClient(Dsn dsn)
            : base(dsn)
        {
            Version = VersionHelper.GetVersionNumber();
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

        public override async Task<string> SendEventAsync(
            SentryEventData eventData)
        {
            using (var stream = _serializer.Serialize(eventData))
            {
                var response = await _httpClient.SendAsync(
                    GetRequest(IssueAuth(), stream));

                using (var body = await response.Content.ReadAsStreamAsync())
                {
                    var eventCreated = _serializer
                        .Deserialize<EventIdData>(body);

                    return eventCreated.Id;
                }
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
