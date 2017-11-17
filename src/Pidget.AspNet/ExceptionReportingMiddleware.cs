using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pidget.AspNet.Sanitizing;
using Pidget.Client;

namespace Pidget.AspNet
{
    public class ExceptionReportingMiddleware
    {
        public const string EventIdKey = "SentryEventId";

        public ExceptionReportingOptions Options { get; }

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

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var eventId = await CaptureAsync(ex, context);

                context.Items.Add(EventIdKey, eventId);
                ex.Data.Add(EventIdKey, eventId);

                SilentlyRethrow(ex);
            }
        }

        private async Task<string> CaptureAsync(Exception ex, HttpContext context)
            => await _sentryClient.CaptureAsync(e
                => BuildEvent(ex, context, e));

        private void BuildEvent(Exception ex, HttpContext context,
            SentryEventBuilder sentryEvent)
        {
            sentryEvent.SetException(ex);

            AddUserData(sentryEvent, context);
            AddRequestData(sentryEvent, context.Request);
        }

        private void AddUserData(SentryEventBuilder sentryEvent, HttpContext context)
        {
            var user = UserDataProvider.Default.GetUserData(context);

            if (user != null)
            {
                sentryEvent.SetUserData(user);
            }
        }

        private void AddRequestData(SentryEventBuilder sentryEvent, HttpRequest request)
        {
            var provider = new RequestDataProvider(
                new RequestSanitizer(Options.Sanitation));

            sentryEvent.SetRequestData(provider.GetRequestData(request));
        }

        /// <summary>
        /// Re-throws the provided exception without adding to the stack trace.
        /// </summary>
        /// <param name="ex">The exception to re-throw.</param>
        private static void SilentlyRethrow(Exception ex)
            => ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
