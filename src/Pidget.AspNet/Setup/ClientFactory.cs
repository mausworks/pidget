using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pidget.Client;
using System;

namespace Pidget.AspNet.Setup
{
    public static class ClientFactory
    {
        public static SentryClient CreateClient(
            IServiceProvider serviceProvider)
        {
            var options = GetOptions(serviceProvider);

            return Sentry.CreateClient(GetDsn(options));
        }

        private static SentryOptions GetOptions(
            IServiceProvider serviceProvider)
        {
            var options = new SentryOptions();
            var setup = serviceProvider
                .GetRequiredService<IConfigureOptions<SentryOptions>>();

            setup.Configure(options);

            return options;
        }

        private static Dsn GetDsn(SentryOptions options)
            => !string.IsNullOrEmpty(options.Dsn)
                ? Dsn.Create(options.Dsn)
                : null;
    }
}
