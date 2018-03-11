using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Pidget.Client;
using System;
using System.Collections.Generic;
using Xunit;

namespace Pidget.AspNet.Setup
{
    public class SetupExtensionsTests
    {
        public static Dictionary<Type, ServiceLifetime> ExpectedServices =
            new Dictionary<Type, ServiceLifetime>()
            {
                { typeof(SentryClient), ServiceLifetime.Singleton },
                { typeof(RateLimit), ServiceLifetime.Singleton },
                { typeof(IConfigureOptions<SentryOptions>), ServiceLifetime.Singleton }
            };

        private static Mock<IServiceCollection> GetExpectedServicesMock()
        {
            var servicesMock = new Mock<IServiceCollection>();

            foreach(var (serviceType, lifetime) in ExpectedServices)
            {
                servicesMock.Setup(m => m
                    .Add(It.Is<ServiceDescriptor>(s
                        => s.ServiceType == serviceType
                        && s.Lifetime == lifetime)))
                    .Verifiable();
            }

            return servicesMock;
        }

        [Fact]
        public void AddPidgetMiddleware_UsingSetup_AddsExpectedServices()
        {
            var servicesMock = GetExpectedServicesMock();

            servicesMock.Object.AddPidgetMiddleware(_ => {});

            servicesMock.Verify();
        }

        [Fact]
        public void AddPidgetMiddleware_UsingConfig_AddsExpectedServices()
        {
            var servicesMock = GetExpectedServicesMock();

            servicesMock.Object.AddPidgetMiddleware(
                Mock.Of<IConfigurationSection>());

            servicesMock.Verify();
        }

        [Fact]
        public void UsePidgetMiddleware_AddsDelegateToPipeline()
        {
            var appMock = new Mock<IApplicationBuilder>();

            appMock.Setup(m => m
                .Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()));

            appMock.Object.UsePidgetMiddleware();

            appMock.Verify();
        }

        [Theory, InlineData("https://pub:secret@sentry.io/1")]
        public void ActivatedClient_HasExpectedDsn(string expectedDsn)
        {
            var services = new ServiceCollection();

            services.AddPidgetMiddleware(sentry =>
            {
                sentry.Dsn = expectedDsn;
            });

            var provider = services.BuildServiceProvider();

            var client = (SentryClient)provider.GetService(typeof(SentryClient));

            Assert.NotNull(client);
            Assert.Equal(expectedDsn, client.Dsn.ToString());
        }
    }
}
