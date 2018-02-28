using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pidget.Client.DataModels;
using Pidget.Client.Serialization;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using static System.Threading.CancellationToken;

namespace Pidget.Client.Http
{
    public class SentryHttpClient : SentryClient
    {
        public static TimeSpan Timeout { get; }
            = TimeSpan.FromSeconds(3);

        public static JsonSerializer JsonSerializer { get; }
            = GetJsonSerializer();

        public static string UserAgent { get; }
            = string.Join("/", Name, Version);

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
            if (!IsEnabled)
            {
                return SentryResponse.Empty;
            }

            using (var stream = _streamSerializer.Serialize(eventData))
            {
                var httpResponse = await _sender
                    .SendAsync(ComposeMessage(stream), None)
                    .ConfigureAwait(false);

                var responseProvider = new SentryResponseProvider(_streamSerializer);

                return await responseProvider.GetResponseAsync(httpResponse);
            }
        }

        public static HttpClient CreateHttpClient()
        {
            var client = new HttpClient { Timeout = Timeout };

            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            return client;
        }

        public static SentryHttpClient Default(Dsn dsn)
            => new SentryHttpClient(dsn, CreateHttpClient());

        private HttpRequestMessage ComposeMessage(Stream stream)
        {
            var message = new HttpRequestMessage(HttpMethod.Post,
                Dsn.GetCaptureUrl());

            message.Headers.Add(SentryAuthHeader.Name,
                GetSentryAuthHeader());

            message.Content = GetContent(stream);

            return message;
        }

        private IEnumerable<string> GetSentryAuthHeader()
            => SentryAuthHeader.GetValues(
                SentryAuth.Issue(this, DateTimeOffset.Now));

        private static StreamContent GetContent(Stream stream)
        {
            var content = new StreamContent(stream);

            content.Headers.Add("Content-Type", "application/json");

            return content;
        }
    }
}
