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
        public static ExceptionReportingOptions Options =
            new ExceptionReportingOptions
            {
                Dsn = "https://PUBLIC:SECRET@sentry.io/PROJECT_ID"
            };

        [Fact]
        public void CreateClient()
        {
            var providerMock = new Mock<IServiceProvider>();
            var optionsMock = new Mock<IOptions<ExceptionReportingOptions>>();

            optionsMock.SetupGet(o => o.Value)
                .Returns(Options)
                .Verifiable();

            providerMock.Setup(sp => sp.GetService(typeof(IOptions<ExceptionReportingOptions>)))
                .Returns(optionsMock.Object)
                .Verifiable();

            var client = ClientFactory.CreateClient(providerMock.Object);

            providerMock.Verify();

            Assert.Equal(Options.Dsn, client.Dsn.ToString());
            AssertAreDefaultSanitationOptions(Options.Sanitation);
        }

        private void AssertAreDefaultSanitationOptions(SanitationOptions sanitation)
        {
            var defaults = SanitationOptions.Default;

            Assert.Equal(defaults.NamePatterns, sanitation.NamePatterns);
            Assert.Equal(defaults.ValuePatterns, sanitation.ValuePatterns);
            Assert.Equal(defaults.ReplacementValue, sanitation.ReplacementValue);
        }
    }
}
