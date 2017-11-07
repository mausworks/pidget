using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Pidget.AspNet.Test.Site
{
    public class OnExceptionMiddleware
    {
        public readonly string _eventIdKey;

        private readonly RequestDelegate _next;

        public OnExceptionMiddleware(RequestDelegate next,
            IOptions<ExceptionReportingOptions> optionsAccessor)
        {
            _next = next;
            _eventIdKey = optionsAccessor.Value.EventIdKey;
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

                await WriteException(context, ex);
            }
        }

        public async Task WriteException(HttpContext context, Exception ex)
        {
            await context.Response.WriteAsync(
                $"An exception of the type '{ex.GetType().Name}' occurred\r\n");
            await context.Response.WriteAsync(
                $" - Exception.Data[\"{_eventIdKey}\"]: {ex.Data[_eventIdKey]}\r\n");
            await context.Response.WriteAsync(
                $" - HttpContext.Items[\"{_eventIdKey}\"]: {context.Items[_eventIdKey]}");
        }
    }
}
