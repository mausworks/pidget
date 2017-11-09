using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pidget.Client;
using Pidget.Client.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pidget.AspNet.Sanitizing;
using System.Runtime.ExceptionServices;

namespace Pidget.AspNet
{
    public class ExceptionReportingMiddleware
    {
        public ExceptionReportingOptions Options { get; }

        private readonly RequestDelegate _next;

        private readonly Dsn _dsn;

        public ExceptionReportingMiddleware(RequestDelegate next,
            IOptions<ExceptionReportingOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;

            _next = next;
            _dsn = Dsn.Create(Options.Dsn);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var eventId = await CaptureExceptionAsync(ex, context);

                context.Items.Add(Options.EventIdKey, eventId);
                ex.Data.Add(Options.EventIdKey, eventId);

                SilentlyRethrow(ex);
            }
        }

        private async Task<string> CaptureExceptionAsync(Exception ex,
            HttpContext context)
        {
            using (var client = new SentryHttpClient(_dsn))
            {
                return await client.CaptureAsync(e
                    => BuildEvent(ex, context, e));
            }
        }

        private void BuildEvent(Exception ex, HttpContext context,
            SentryEventBuilder sentryEvent)
        {
            var provider = new RequestDataProvider(context.Request,
                new RequestSanitizer(Options.Sanitation));

            sentryEvent.SetException(ex)
                .AddFingerprintData(GetFingerprint(context));

            if (provider.TryGetForm(out var form))
            {
                sentryEvent.AddExtraData("form", form);
            }
            if (provider.TryGetHeaders(out var headers))
            {
                sentryEvent.AddExtraData("request_headers", headers);
            }
            if (provider.TryGetCookies(out var cookies))
            {
                sentryEvent.AddExtraData("request_cookies", cookies);
            }
        }

        private string[] GetFingerprint(HttpContext context)
            => new[]
            {
                "{{ default }}",
                context.Request.Method,
                context.Request.Path.ToString()
            };

        /// <summary>
        /// Re-throws the provided exception without adding to the stack trace.
        /// </summary>
        /// <param name="ex">The exception to re-throw.</param>
        private static void SilentlyRethrow(Exception ex)
            => ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
