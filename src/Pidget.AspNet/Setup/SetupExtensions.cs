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
        public static IServiceCollection ConfigurePidgetMiddleware(
            this IServiceCollection services,
            IConfiguration configuration)
            => ConfigurePidgetMiddleware(services, configuration.Bind);

        public static IServiceCollection ConfigurePidgetMiddleware(
            this IServiceCollection services,
            Action<ExceptionReportingOptions> setup)
            => services.Configure<ExceptionReportingOptions>(setup)
                .AddScoped<SentryClient>(ClientFactory);

        public static IApplicationBuilder UsePidgetMiddleware(
            this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionReportingMiddleware>();

        private static SentryClient ClientFactory(IServiceProvider serviceProvider)
        {
            var optionsAccessor = serviceProvider
                .GetRequiredService<IOptions<ExceptionReportingOptions>>();

            var dsn = Dsn.Create(optionsAccessor.Value.Dsn);

            return Sentry.CreateClient(dsn);
        }
    }
}
