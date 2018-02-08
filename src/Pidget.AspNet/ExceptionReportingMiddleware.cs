using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pidget.AspNet.Sanitizing;
using Pidget.Client;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Pidget.Client.DataModels;

using static System.DateTimeOffset;

namespace Pidget.AspNet
{
    public class ExceptionReportingMiddleware
    {
        public ExceptionReportingOptions Options { get; }

        private readonly RateLimit _rateLimit;

        private readonly RequestDelegate _next;

        private readonly SentryClient _sentryClient;

        public ExceptionReportingMiddleware(RequestDelegate next,
            IOptions<ExceptionReportingOptions> optionsAccessor,
            SentryClient sentryClient,
            RateLimit rateLimiter)
        {
            _next = next;
            _sentryClient = sentryClient;
            _rateLimit = rateLimiter;
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
                await CaptureAsync(ex, http);

                SilentlyRethrow(ex);
            }
        }

        private async Task CaptureAsync(Exception ex, HttpContext http)
        {
            var builder = BuildEventData(ex, http);

            if (!await Options.BeforeSendCallback(builder, http))
            {
                return;
            }

            var response = await SendEventAsync(builder.Build());

            await Options.AfterSendCallback(response, http);
        }

        private async Task<SentryResponse> SendEventAsync(SentryEventData eventData)
        {
            if (_rateLimit.IsHit(UtcNow))
            {
                return null;
            }

            var response = await _sentryClient
                .SendEventAsync(eventData);

            if (IsTooManyRequests(response))
            {
                _rateLimit.Until(response.RetryAfter);
            }

            return response;
        }

        private bool IsTooManyRequests(SentryResponse response)
            => response.StatusCode == 429
            && response.RetryAfter != null;

        private SentryEventBuilder BuildEventData(Exception ex, HttpContext http)
            => new SentryEventBuilder()
                .SetException(ex)
                .SetUserData(GetUserData(http))
                .SetRequestData(GetRequestData(http.Request));

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
