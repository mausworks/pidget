using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pidget.AspNet.Sanitizing;
using Pidget.Client;
using Pidget.Client.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

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
                    => BuildEvent(ex, context.Request, e));
            }
        }

        private void BuildEvent(Exception ex, HttpRequest request,
            SentryEventBuilder sentryEvent)
        {
            var provider = new RequestDataProvider(
                new RequestSanitizer(Options.Sanitation));

            sentryEvent.SetException(ex)
                .SetRequestData(provider.GetRequestData(request));
        }

        /// <summary>
        /// Re-throws the provided exception without adding to the stack trace.
        /// </summary>
        /// <param name="ex">The exception to re-throw.</param>
        private static void SilentlyRethrow(Exception ex)
            => ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
