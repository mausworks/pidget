using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Pidget.Client;
using Xunit;

namespace Pidget.AspNet.Setup
{
    public class SetupExtensionsTests
    {
        [Fact]
        public void AddPidgetMiddleware_AddsSingletonClient()
        {
            var servicesMock = new Mock<IServiceCollection>();

            servicesMock.Setup(m => m
                .Add(It.Is<ServiceDescriptor>(s
                    => s.ServiceType == typeof(SentryClient)
                    && s.Lifetime == ServiceLifetime.Singleton)))
                .Verifiable();

            servicesMock.Object.AddPidgetMiddleware(_ => {});

            servicesMock.Verify();
        }

        [Fact]
        public void AddPidgetMiddleware_AddsSingletonRateLimit()
        {
            var servicesMock = new Mock<IServiceCollection>();

            servicesMock.Setup(m => m
                .Add(It.Is<ServiceDescriptor>(s
                    => s.ServiceType == typeof(RateLimit)
                    && s.Lifetime == ServiceLifetime.Singleton)))
                .Verifiable();

            servicesMock.Object.AddPidgetMiddleware(_ => {});

            servicesMock.Verify();
        }

        [Fact]
        public void AddPidgetMiddleware_Setup_AddsSingletonOptions()
        {
            var servicesMock = new Mock<IServiceCollection>();

            servicesMock.Setup(m => m
                .Add(It.Is<ServiceDescriptor>(s
                    => s.ServiceType == typeof(IConfigureOptions<SentryOptions>)
                    && s.Lifetime == ServiceLifetime.Singleton)))
                .Verifiable();

            servicesMock.Object.AddPidgetMiddleware(_ => {});

            servicesMock.Verify();
        }

        [Fact]
        public void AddPidgetMiddleware_Configuration_AddsSingletonOptions()
        {
            var servicesMock = new Mock<IServiceCollection>();

            servicesMock.Setup(m => m
                .Add(It.Is<ServiceDescriptor>(s
                    => s.ServiceType == typeof(IConfigureOptions<SentryOptions>)
                    && s.Lifetime == ServiceLifetime.Singleton)))
                .Verifiable();

            servicesMock.Object.AddPidgetMiddleware(
                Mock.Of<IConfigurationSection>());

            servicesMock.Verify();
        }

        [Fact]
        public void UsePidgetMiddleware_AddsMiddleware()
        {
            var appMock = new Mock<IApplicationBuilder>();

            appMock.Setup(m => m
                .Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()));

            appMock.Object.UsePidgetMiddleware();

            appMock.Verify();
        }

        [Theory, InlineData("https://pub:secreet@sentry.io/1")]
        public void ActivateClient_HasCorrectDsn(string dsn)
        {
            var services = new ServiceCollection();

            services.AddPidgetMiddleware(sentry =>
            {
                sentry.Dsn = dsn;
            });

            var provider = services.BuildServiceProvider();

            var client = (SentryClient)provider.GetService(typeof(SentryClient));

            Assert.NotNull(client);
            Assert.Equal(dsn, client.Dsn.ToString());
        }
    }
}
