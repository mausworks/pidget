using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pidget.AspNet.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigurePidgetMiddleware(
            this IServiceCollection services, IConfiguration configuration)
            => services.Configure<ExceptionReportingOptions>(
                configuration.Bind);

        public static IServiceCollection ConfigurePidgetMiddleware(
            this IServiceCollection service,
            Action<ExceptionReportingOptions> setup)
            => service.Configure<ExceptionReportingOptions>(setup);
    }
}
