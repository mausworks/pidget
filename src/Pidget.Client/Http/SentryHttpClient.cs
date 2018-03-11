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
    /// <summary>
    /// The shipped default implementation of the SentryClient.
    /// </summary>
    public class SentryHttpClient : SentryClient
    {
        /// <summary>
        /// The maximum time the default client will wait for a response.
        /// </summary>
        public static TimeSpan DefaultTimeout { get; }
            = TimeSpan.FromSeconds(3);

        /// <summary>
        /// The user agent used by the default client.
        /// </summary>
        public static string DefaultUserAgent { get; }
            = string.Join("/", Name, Version);

        private static readonly JsonStreamSerializer _serializer
            = new JsonStreamSerializer(
                encoding: Sentry.ApiEncoding,
                jsonSerializer: JsonSerializer.CreateDefault());

        private readonly HttpMessageInvoker _sender;

        /// <summary>
        /// Creates a new Sentry HTTP client using the provided DSN and sender.
        /// See also: SentryHttpClient.Default(dsn).
        /// </summary>
        /// <param name="dsn">The DSN to use for the client.</param>
        /// <param name="sender">The sending mechanism.</param>
        public SentryHttpClient(Dsn dsn, HttpMessageInvoker sender)
            : base(dsn)
        {
            Assert.ArgumentNotNull(sender, nameof(sender));

            _sender = sender;
        }

        /// <summary>
        /// Creates a new Sentry HTTP client using the default sender.
        /// </summary>
        /// <param name="dsn">The DSN to use for the client.</param>
        public static SentryHttpClient Default(Dsn dsn)
            => new SentryHttpClient(dsn, CreateDefaultHttpClient());

        public override async Task<SentryResponse> SendEventAsync(
            SentryEventData eventData)
        {
            if (!IsEnabled)
            {
                return SentryResponse.Empty;
            }

            using (var stream = _serializer.Serialize(eventData))
            {
                using (var res = await SendMessageAsync(stream))
                {
                    var responseProvider = new SentryResponseProvider(_serializer);

                    return await responseProvider.GetResponseAsync(res);
                }
            }
        }

        /// <summary>
        /// Creates a HttpClient using the default user agent and timeout.
        /// </summary>
        public static HttpClient CreateDefaultHttpClient()
        {
            var client = new HttpClient { Timeout = DefaultTimeout };

            client.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);

            return client;
        }

        private async Task<HttpResponseMessage> SendMessageAsync(Stream stream)
            => await _sender.SendAsync(ComposeMessage(stream), None)
                .ConfigureAwait(false);

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
                SentryAuth.Issue(Dsn, DateTimeOffset.Now));

        private static StreamContent GetContent(Stream stream)
        {
            var content = new StreamContent(stream);

            content.Headers.Add("Content-Type", "application/json");

            return content;
        }
    }
}
