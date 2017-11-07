using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pidget.AspNet
{
    public static class SetupExtensions
    {
        public static IServiceCollection ConfigurePidgetMiddleware(
            this IServiceCollection services, IConfiguration configuration)
            => ConfigurePidgetMiddleware(services, configuration.Bind);

        public static IServiceCollection ConfigurePidgetMiddleware(
            this IServiceCollection services,
            Action<ExceptionReportingOptions> setup)
            => services.Configure<ExceptionReportingOptions>(setup);

        public static IApplicationBuilder UsePidgetMiddleware(
            this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionReportingMiddleware>();
    }
}
