using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pidget.AspNet;

namespace Pidget.AspNetExample
{
    public class OnExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public SentryOptions Options { get; }

        public OnExceptionMiddleware(RequestDelegate next,
            IOptions<SentryOptions> optionsAccessor)
        {
            _next = next;
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
                http.Response.StatusCode = 200;
                http.Response.ContentType = "text/plain";

                await WriteExceptionAsync(http, ex);
            }
        }

        public async Task WriteExceptionAsync(HttpContext http, Exception ex)
        {
            await http.Response.WriteAsync($"{ex}\r\n\r\n");
            await http.Response.WriteAsync(
                $"Sentry event ID: {http.Items["SentryEventId"]}");
        }
    }
}
