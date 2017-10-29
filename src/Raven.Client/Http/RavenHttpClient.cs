using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Raven.Client.DataModels;
using Raven.Client.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Raven.Client.Http
{
    public class RavenHttpClient : RavenClient, IDisposable
    {
        public override string Version { get; }

        private readonly HttpClient _httpClient;

        private readonly JsonStreamSerializer _serializer;

        public RavenHttpClient(Dsn dsn)
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
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };

            settings.Converters.Add(
                new StringEnumConverter(camelCaseText: true));

            return JsonSerializer.Create(settings);
        }

        protected override async Task<string> SendEventAsync(
            SentryEventData eventData)
        {
            using (var stream = _serializer.Serialize(eventData))
            {
                var response = await HandleRequestAsync(stream);

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

        private Task<HttpResponseMessage> HandleRequestAsync(Stream stream)
            => _httpClient.SendAsync(
                GetRequest(IssueAuth(), stream));

        private SentryAuth IssueAuth()
            => SentryAuth.Issue(this, DateTimeOffset.UtcNow);

        private HttpRequestMessage GetRequest(SentryAuth auth, Stream stream)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                GetCaptureUrl(Dsn));

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

        private HttpMessageHandler GetClientHandler()
            => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            };

        private static string GetCaptureUrl(Dsn dsn)
            => string.Concat(
                dsn.Uri.Scheme,
                Uri.SchemeDelimiter,
                dsn.Uri.DnsSafeHost,
                !dsn.Uri.IsDefaultPort ? $":{dsn.Uri.Port}" : null,
                "/api/",
                dsn.GetProjectId(),
                "/store/");
    }
}
