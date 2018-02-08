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
    public class SetupExtensionsTests
    {
        [Fact]
        public void AddPidgetMiddleware_AddsClient()
        {
            var servicesMock = new Mock<IServiceCollection>();

            servicesMock.Setup(m => m
                .Add(It.Is<ServiceDescriptor>(s => typeof(SentryClient) == s.ServiceType)))
                .Verifiable();

            servicesMock.Object.AddPidgetMiddleware(_ => {});

            servicesMock.Verify();
        }

        [Fact]
        public void AddPidgetMiddleware_AddsRateLimit()
        {
            var servicesMock = new Mock<IServiceCollection>();

            servicesMock.Setup(m => m
                .Add(It.Is<ServiceDescriptor>(s
                    => typeof(RateLimit) == s.ServiceType
                    && s.Lifetime == ServiceLifetime.Singleton)))
                .Verifiable();

            servicesMock.Object.AddPidgetMiddleware(_ => {});

            servicesMock.Verify();
        }

        [Fact]
        public void AddPidgetMiddleware_Setup_AddsOptions()
        {
            var servicesMock = new Mock<IServiceCollection>();

            servicesMock.Setup(m => m
                .Add(It.Is<ServiceDescriptor>(
                    s => typeof(IConfigureOptions<SentryOptions>) == s.ServiceType)))
                .Verifiable();

            servicesMock.Object.AddPidgetMiddleware(_ => {});

            servicesMock.Verify();
        }

        [Fact]
        public void AddPidgetMiddleware_Configuration_AddsOptions()
        {
            var servicesMock = new Mock<IServiceCollection>();

            servicesMock.Setup(m => m
                .Add(It.Is<ServiceDescriptor>(
                    s => typeof(IConfigureOptions<SentryOptions>) == s.ServiceType)))
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
    }
}
