using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pidget.Client.DataModels;
using Pidget.Client.Serialization;

using static System.Threading.CancellationToken;

namespace Pidget.Client.Http
{
    public class SentryHttpClient : SentryClient, IDisposable
    {
        public static TimeSpan Timeout { get; } = TimeSpan.FromSeconds(3);

        public static JsonSerializer JsonSerializer { get; }
            = GetJsonSerializer();

        public static string UserAgent => string.Join("/", Name, Version);

        private static readonly JsonStreamSerializer _streamSerializer
            = new JsonStreamSerializer(
                encoding: Sentry.ApiEncoding,
                jsonSerializer: JsonSerializer);

        private readonly HttpMessageInvoker _sender;

        public SentryHttpClient(Dsn dsn, HttpMessageInvoker sender)
            : base(dsn)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));

            _sender = sender;
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
            using (var stream = _streamSerializer.Serialize(eventData))
            {
                var httpResponse = await _sender
                    .SendAsync(GetRequest(stream), None)
                    .ConfigureAwait(false);

                var responseProvider = new SentryResponseProvider(_streamSerializer);

                return await responseProvider.GetResponseAsync(httpResponse);
            }
        }

        public void Dispose() => _sender.Dispose();

        public static HttpMessageInvoker CreateSender()
        {
            var client = new HttpClient { Timeout = Timeout };

            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            return client;
        }

        public static SentryHttpClient CreateDefault(Dsn dsn)
            => new SentryHttpClient(dsn, CreateSender());

        private HttpRequestMessage GetRequest(Stream stream)
        {
            var request = new HttpRequestMessage(HttpMethod.Post,
                Dsn.GetCaptureUrl());

            AddSentryAuthHeader(request);

            request.Content = GetContent(stream);

            return request;
        }

        private void AddSentryAuthHeader(HttpRequestMessage request)
            => request.Headers.Add(SentryAuthHeader.Name,
                SentryAuthHeader.GetValues(
                    SentryAuth.Issue(this, DateTimeOffset.Now)));

        private static StreamContent GetContent(Stream stream)
        {
            var content = new StreamContent(stream);

            content.Headers.Add("Content-Type", "application/json");

            return content;
        }
    }
}
