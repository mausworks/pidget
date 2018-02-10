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
            IConfiguration configuration,
            Action<CallbackSetup> callbackSetup = null)
            => AddPidgetMiddleware(services, opts =>
            {
                configuration.Bind(opts);

                if (callbackSetup != null)
                {
                    callbackSetup(new CallbackSetup(opts));
                }
            });

        public static IServiceCollection AddPidgetMiddleware(
            this IServiceCollection services,
            Action<SentryOptions> setup)
            => services.Configure<SentryOptions>(setup)
                .AddScoped<SentryClient>(ClientFactory.CreateClient)
                .AddSingleton<RateLimit>();

        public static IApplicationBuilder UsePidgetMiddleware(
            this IApplicationBuilder builder)
            => builder.UseMiddleware<SentryMiddleware>();
    }
}
