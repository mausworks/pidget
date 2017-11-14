using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pidget.Client;
using Pidget.Client.Http;

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
                .AddScoped<SentryClient>(ClientFactory.CreateClient);

        public static IApplicationBuilder UsePidgetMiddleware(
            this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionReportingMiddleware>();
    }
}
