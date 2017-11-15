
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Pidget.AspNet.Sanitizing;
using Pidget.Client;
using Pidget.Client.DataModels;
using Xunit;

namespace Pidget.AspNet.Test
{
    public class ExceptionReportingMiddlewareTests
    {
        public RequestDelegate Next_Throw = _ => throw new InvalidOperationException();

        public RequestDelegate Next_Noop = _ => Task.CompletedTask;

        public ExceptionReportingOptions ExceptionReportingOptions
            = new ExceptionReportingOptions
            {
                Dsn = "https://PUBLIC:SECRET@sentry.io/PROJECT_ID"
            };

        [Fact]
        public async Task SuccessfulInvoke_DoesNotSend()
        {
            var clientMock = new Mock<SentryClient>(
                Dsn.Create(ExceptionReportingOptions.Dsn));

            var middleware = CreateMiddleware(Next_Noop, clientMock.Object);

            await middleware.Invoke(null);

            clientMock.Verify(c => c.SendEventAsync(It.IsAny<SentryEventData>()), Times.Never);
        }

        [Theory, InlineData("1")]
        public async Task CapturesExceptionOnInvokation(string eventId)
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupAllProperties();

            var contextMock = new Mock<HttpContext>();

            contextMock.Setup(m => m.Items)
                .Returns(new Dictionary<object, object>());

            contextMock.SetupGet(c => c.Request)
                .Returns(requestMock.Object);

            var clientMock = new Mock<SentryClient>(
                Dsn.Create(ExceptionReportingOptions.Dsn));

            clientMock.Setup(c => c.SendEventAsync(It.IsAny<SentryEventData>()))
                .ReturnsAsync(eventId)
                .Verifiable();

            var middleware = CreateMiddleware(Next_Throw, clientMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => middleware.Invoke(contextMock.Object));

            clientMock.Verify();
        }

        public ExceptionReportingMiddleware CreateMiddleware(RequestDelegate next,
            SentryClient client)
            => new ExceptionReportingMiddleware(next,
                Options.Create(ExceptionReportingOptions),
                client);
    }
}
