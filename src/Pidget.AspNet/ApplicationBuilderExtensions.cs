using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Pidget.AspNet
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePidgetMiddleware(
            this IApplicationBuilder builder, IConfiguration configuration)
        {
            var options = new ExceptionReportingOptions();

            configuration.Bind(options);

            return builder.UseMiddleware<ExceptionReportingMiddleware>(options);
        }

        public static IApplicationBuilder UsePidgetMiddleware(
            this IApplicationBuilder builder, Action<ExceptionReportingOptions> setup)
        {
            var options = new ExceptionReportingOptions();

            setup(options);

            return builder.UseMiddleware<ExceptionReportingMiddleware>(options);
        }
    }
}
