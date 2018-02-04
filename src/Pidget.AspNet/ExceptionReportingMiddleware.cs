using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pidget.AspNet.Sanitizing;
using Pidget.Client;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Pidget.Client.DataModels;
using System.Net;

namespace Pidget.AspNet
{
    public class ExceptionReportingMiddleware
    {
        public const string EventIdKey = "SentryEventId";

        public ExceptionReportingOptions Options { get; }

        public static DateTimeOffset RetryAfter { get; private set; }

        private readonly RequestDelegate _next;

        private readonly SentryClient _sentryClient;

        public ExceptionReportingMiddleware(RequestDelegate next,
            IOptions<ExceptionReportingOptions> optionsAccessor,
            SentryClient sentryClient)
        {
            _next = next;
            _sentryClient = sentryClient;
            Options = optionsAccessor.Value;
        }

        public async Task Invoke(HttpContext http)
        {
            try
            {
                await _next(http);
            }
            catch (Exception ex)
            {
                var response = await CaptureAsync(ex, http);

                http.Items[EventIdKey] = response?.EventId;
                ex.Data[EventIdKey] = response?.EventId;

                SilentlyRethrow(ex);
            }
        }

        private async Task<SentryResponse> CaptureAsync(Exception ex,
            HttpContext http)
        {
            var sentryEvent = BuildEventData(ex, http);

            if (IsRateLimited())
            {
                return null;
            }

            var sentryResponse = await _sentryClient
                .SendEventAsync(sentryEvent);

            if (IsTooManyRequests(sentryResponse))
            {
                RetryAfter = DateTimeOffset.UtcNow
                    + sentryResponse.RetryAfter.Value;
            }

            return sentryResponse;
        }

        private bool IsTooManyRequests(SentryResponse response)
            => response.HttpStatusCode == (HttpStatusCode)429
            && response.RetryAfter != null;

        private SentryEventData BuildEventData(Exception ex, HttpContext http)
            => new SentryEventBuilder()
                .SetException(ex)
                .SetUserData(GetUserData(http))
                .SetRequestData(GetRequestData(http.Request))
                .Build();

        private bool IsRateLimited()
            => DateTimeOffset.UtcNow < RetryAfter;

        private UserData GetUserData(HttpContext http)
            => UserDataProvider.Default.GetUserData(http);

        private RequestData GetRequestData(HttpRequest req)
        {
            var provider = new RequestDataProvider(
                new RequestSanitizer(Options.Sanitation));

            return provider.GetRequestData(req);
        }

        /// <summary>
        /// Re-throws the provided exception without adding to the stack trace.
        /// </summary>
        /// <param name="ex">The exception to re-throw.</param>
        private static void SilentlyRethrow(Exception ex)
            => ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
