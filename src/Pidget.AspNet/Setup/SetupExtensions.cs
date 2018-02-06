using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pidget.Client;
using System;

namespace Pidget.AspNet.Setup
{
    public static class SetupExtensions
    {
        public static IServiceCollection AddPidgetMiddleware(
            this IServiceCollection services,
            IConfiguration configuration)
            => AddPidgetMiddleware(services, configuration.Bind);

        public static IServiceCollection AddPidgetMiddleware(
            this IServiceCollection services,
            Action<ExceptionReportingOptions> setup)
            => services.Configure<ExceptionReportingOptions>(setup)
                .AddScoped<SentryClient>(ClientFactory.CreateClient)
                .AddSingleton<RateLimiter>();

        public static IApplicationBuilder UsePidgetMiddleware(
            this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionReportingMiddleware>();
    }
}
