using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pidget.AspNet.Sanitizing;
using Pidget.Client;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Pidget.Client.DataModels;

namespace Pidget.AspNet
{
    public class SentryMiddleware
    {
        public SentryOptions Options { get; }

        private readonly RequestDelegate _next;

        private readonly SentryClient _sentryClient;

        private readonly RateLimit _rateLimit;

        public SentryMiddleware(RequestDelegate next,
            IConfigureOptions<SentryOptions> optionSetup,
            SentryClient sentryClient,
            RateLimit rateLimit)
        {
            _next = next;
            _sentryClient = sentryClient;
            _rateLimit = rateLimit;
            Options = GetOptions(optionSetup);
        }

        private SentryOptions GetOptions(IConfigureOptions<SentryOptions> optionSetup)
        {
            var options = new SentryOptions();

            optionSetup.Configure(options);

            return options;
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

                ForwardException(ex);
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
            if (_rateLimit.IsHit(DateTimeOffset.UtcNow))
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
            => UserDataProvider.GetUserData(http);

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
        private static void ForwardException(Exception ex)
            => ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
