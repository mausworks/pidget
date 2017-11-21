
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
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

            var contextMock = new Mock<HttpContext>();

            contextMock.Setup(m => m.Items)
                .Returns(new Dictionary<object, object>());

            contextMock.SetupGet(c => c.Request)
                .Returns(requestMock.Object);

            var clientMock = new Mock<SentryClient>(
                Dsn.Create(ExceptionReportingOptions.Dsn));

            clientMock.Setup(c => c.SendEventAsync(It.IsAny<SentryEventData>()))
                .ReturnsAsync(new SentryResponse { EventId = eventId })
                .Verifiable();

            var middleware = CreateMiddleware(Next_Throw, clientMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => middleware.Invoke(contextMock.Object));

            clientMock.Verify();
        }

        [Theory, InlineData("1", "POST", "https://foo.bar/baz?foo=bar")]
        public async Task CapturesRequestData(string eventId,
            string method,
            string url)
        {
            var uri = new Uri(url, UriKind.Absolute);
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Method).Returns(method);

            requestMock.SetupGet(r => r.Scheme).Returns(uri.Scheme);
            requestMock.SetupGet(r => r.Host).Returns(new HostString(uri.Host));
            requestMock.SetupGet(r => r.Path).Returns(uri.AbsolutePath);

            requestMock.SetupGet(r => r.QueryString)
                .Returns(QueryString.FromUriComponent(uri.Query));
            requestMock.SetupGet(r => r.Query)
                .Returns(new QueryCollection(QueryHelpers.ParseQuery(uri.Query)));

            var contextMock = new Mock<HttpContext>();

            contextMock.Setup(m => m.Items)
                .Returns(new Dictionary<object, object>());

            contextMock.SetupGet(c => c.Request)
                .Returns(requestMock.Object);

            var clientMock = new Mock<SentryClient>(
                Dsn.Create(ExceptionReportingOptions.Dsn));

            clientMock.Setup(c => c.SendEventAsync(It.Is<SentryEventData>(r
                => r.Request.Url == url.Split('?', StringSplitOptions.None)[0]
                && r.Request.Method == method
                && r.Request.QueryString == uri.Query)))
                .ReturnsAsync(new SentryResponse { EventId = eventId })
                .Verifiable();

            var middleware = CreateMiddleware(Next_Throw, clientMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => middleware.Invoke(contextMock.Object));

            clientMock.Verify();
        }

        [Theory, InlineData("1", "1", "user", "foo@bar", "0.0.0.0")]
        public async Task CapturesUserData(string eventId,
            string userId,
            string userName,
            string email,
            string ipAddress)
        {
            var connectionMock = new Mock<ConnectionInfo>();

            connectionMock.SetupGet(c => c.RemoteIpAddress)
                .Returns(IPAddress.Parse(ipAddress))
                .Verifiable();

            var user = UserDataProviderTests.MockUser(
                Tuple.Create(ClaimTypes.NameIdentifier, userId),
                Tuple.Create(ClaimTypes.Name, userName),
                Tuple.Create(ClaimTypes.Email, email));

            var contextMock = new Mock<HttpContext>();

            contextMock.SetupGet(c => c.Connection)
                .Returns(connectionMock.Object)
                .Verifiable();

            contextMock.SetupGet(c => c.User)
                .Returns(user)
                .Verifiable();

            contextMock.Setup(m => m.Items)
                .Returns(new Dictionary<object, object>());

            contextMock.SetupGet(c => c.Request)
                .Returns(Mock.Of<HttpRequest>());

            var clientMock = new Mock<SentryClient>(
                Dsn.Create(ExceptionReportingOptions.Dsn));

            clientMock.Setup(c => c.SendEventAsync(It.Is<SentryEventData>(r
                 => r.User.Id == userId
                 && r.User.UserName == userName
                 && r.User.Email == email
                 && r.User.IpAddress == ipAddress)))
                .ReturnsAsync(new SentryResponse { EventId = eventId })
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
