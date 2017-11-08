using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Pidget.AspNet.Test.Site
{
    public class OnExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionReportingOptions Options { get; }

        public OnExceptionMiddleware(RequestDelegate next,
            IOptions<ExceptionReportingOptions> optionsAccessor)
        {
            _next = next;
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
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/plain";

                await WriteExceptionAsync(context, ex);
            }
        }

        public async Task WriteExceptionAsync(HttpContext context, Exception ex)
        {
            await context.Response.WriteAsync($"{ex}\r\n\r\n");
            await context.Response.WriteAsync(
                $"Sentry event ID: {context.Items[Options.EventIdKey]}");
        }
    }
}
