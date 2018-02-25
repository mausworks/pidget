using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Pidget.Client;
using Pidget.Client.Http;
using Xunit;

namespace Pidget.AspNet.Setup
{
    public class ClientFactoryTests
    {
        public const string Dsn = "https://PUBLIC:SECRET@sentry.io/PROJECT_ID";

        public static Action<SentryOptions> SetDsn = (SentryOptions opts) =>
        {
            opts.Dsn = Dsn;
        };

        [Fact]
        public void CreateClient_SetsDsn()
        {
            var providerMock = new Mock<IServiceProvider>();

            providerMock.Setup(sp => sp.GetService(typeof(IConfigureOptions<SentryOptions>)))
                .Returns(new ConfigureOptions<SentryOptions>(SetDsn))
                .Verifiable();

            var client = ClientFactory.CreateClient(providerMock.Object);

            providerMock.Verify();

            Assert.Equal(Dsn, client.Dsn.ToString());
        }

        [Theory, InlineData(null), InlineData("")]
        public void NoDsn_CreatesDisabledClient(string dsn)
        {
            var providerMock = new Mock<IServiceProvider>();

            providerMock.Setup(sp => sp.GetService(typeof(IConfigureOptions<SentryOptions>)))
                .Returns(new ConfigureOptions<SentryOptions>(opts => opts.Dsn = dsn))
                .Verifiable();

            var client = ClientFactory.CreateClient(providerMock.Object);

            Assert.Null(client.Dsn);
            Assert.False(client.IsEnabled);
        }
    }
}
