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
                .GetRequiredService<IOptions<SentryOptions>>();

            return Sentry.CreateClient(GetDsn(optionsAccessor.Value));
        }

        private static Dsn GetDsn(SentryOptions options)
            => !string.IsNullOrEmpty(options.Dsn)
                ? Dsn.Create(options.Dsn)
                : null;
    }
}
