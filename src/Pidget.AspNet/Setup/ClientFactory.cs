using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pidget.Client;
using System;

namespace Pidget.AspNet.Setup
{
    public static class ClientFactory
    {
        public static SentryClient CreateClient(IServiceProvider serviceProvider)
        {
            var optionsAccessor = serviceProvider
                .GetRequiredService<IOptions<ExceptionReportingOptions>>();

            var dsn = Dsn.Create(optionsAccessor.Value.Dsn);

            return Sentry.CreateClient(dsn);
        }
    }
}
