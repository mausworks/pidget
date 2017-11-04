using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pidget.AspNet.DataModels;
using Pidget.Client;
using Pidget.Client.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pidget.AspNet.Sanitizing;

namespace Pidget.AspNet
{
    public class ErrorReportingMiddleware
    {
        public ErrorReportingOptions Options { get; }

        private readonly RequestDelegate _next;

        private readonly Dsn _dsn;

        private RequestSanitizer _sanitizer { get; }

        public ErrorReportingMiddleware(RequestDelegate next,
            IOptions<ErrorReportingOptions> options)
        {
            Options = options.Value;

            _next = next;
            _dsn = Dsn.Create(Options.Dsn);
            _sanitizer = new RequestSanitizer(Options.Sanitaztion);
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
            sentryEvent.SetException(ex)
                .AddTags(GetTags(context))
                .AddFingerprintData(GetFingerprint(context));

            if (TryGetForm(context.Request, out var form))
            {
                sentryEvent.AddExtraData("form", form);
            }
            if (TryGetFiles(context.Request, out var files))
            {
                sentryEvent.AddExtraData("form_files", files);
            }
            if (TryGetHeaders(context.Request, out var headers))
            {
                sentryEvent.AddExtraData("request_headers", headers);
            }
            if (TryGetCookies(context.Request, out var cookies))
            {
                sentryEvent.AddExtraData("request_cookies", cookies);
            }
        }

        private string[] GetFingerprint(HttpContext context)
            => new[]
            {
                "{{ default }}",
                context.Request.Method,
                _sanitizer.SanitizeUrl(context.Request)
            };

        private IDictionary<string, string> GetTags(HttpContext context)
            => new Dictionary<string, string>
            {
                { "request_method", context.Request.Method },
                { "request_url", _sanitizer.SanitizeUrl(context.Request) }
            };

        private bool TryGetCookies(HttpRequest request,
            out IDictionary<string, string> cookies)
        {
            if (request.Cookies != null && request.Cookies.Any())
            {
                cookies = _sanitizer.SanitizeCookies(request);

                return true;
            }

            cookies = null;

            return false;
        }

        private bool TryGetHeaders(HttpRequest request,
            out IDictionary<string, string> headers)
        {
            if (request.Headers != null && request.Headers.Any())
            {
                headers = _sanitizer.SanitizeHeaders(request);

                return true;
            }

            headers = null;

            return false;
        }

        private bool TryGetForm(HttpRequest request,
            out IDictionary<string, string> form)
        {
            if (request.Form != null && request.Form.Any())
            {
                form = _sanitizer.SanitizeForm(request);

                return true;
            }

            form = null;

            return false;
        }

        private bool TryGetFiles(HttpRequest request,
            out PostedFile[] postedFiles)
        {
            postedFiles = request?.Form?.Files
                .Select(PostedFile.FromFormFile)
                .ToArray();

            return postedFiles != null && postedFiles.Length > 0;
        }
    }
}
