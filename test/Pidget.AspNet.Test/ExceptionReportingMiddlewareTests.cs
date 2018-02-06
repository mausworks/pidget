using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Moq;
using Pidget.Client;
using Pidget.Client.DataModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Pidget.AspNet.Test
{
    public class ExceptionReportingMiddlewareTests
    {
        public RequestDelegate Next_Throw = _ =>
            throw new InvalidOperationException("Hey, look at me!");

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
            var reqMock = new Mock<HttpRequest>();

            var httpMock = new Mock<HttpContext>();

            httpMock.Setup(m => m.Items)
                .Returns(new Dictionary<object, object>());

            httpMock.SetupGet(c => c.Request)
                .Returns(reqMock.Object);

            var clientMock = new Mock<SentryClient>(
                Dsn.Create(ExceptionReportingOptions.Dsn));

            clientMock.Setup(c => c.SendEventAsync(It.IsAny<SentryEventData>()))
                .ReturnsAsync(new SentryResponse { EventId = eventId })
                .Verifiable();

            var middleware = CreateMiddleware(Next_Throw, clientMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => middleware.Invoke(httpMock.Object));

            clientMock.Verify();
        }

        [Theory, InlineData("1", "POST", "https://foo.bar/baz?foo=bar")]
        public async Task CapturesRequestData(string eventId,
            string method,
            string url)
        {
            var uri = new Uri(url, UriKind.Absolute);

            var reqMock = new Mock<HttpRequest>();

            reqMock.SetupGet(r => r.Method).Returns(method);
            reqMock.SetupGet(r => r.Scheme).Returns(uri.Scheme);
            reqMock.SetupGet(r => r.Host).Returns(new HostString(uri.Host));
            reqMock.SetupGet(r => r.Path).Returns(uri.AbsolutePath);

            reqMock.SetupGet(r => r.QueryString)
                .Returns(QueryString.FromUriComponent(uri.Query));
            reqMock.SetupGet(r => r.Query)
                .Returns(new QueryCollection(QueryHelpers.ParseQuery(uri.Query)));

            var httpMock = new Mock<HttpContext>();

            httpMock.Setup(m => m.Items)
                .Returns(new Dictionary<object, object>());

            httpMock.SetupGet(c => c.Request)
                .Returns(reqMock.Object);

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
                () => middleware.Invoke(httpMock.Object));

            clientMock.Verify();
        }

        [Theory, InlineData("1", "1", "user", "foo@bar", "0.0.0.0")]
        public async Task CapturesUserData(string eventId,
            string userId,
            string userName,
            string email,
            string ipAddress)
        {
            var user = UserDataProviderTests.MakeUser(
                Tuple.Create(ClaimTypes.NameIdentifier, userId),
                Tuple.Create(ClaimTypes.Name, userName),
                Tuple.Create(ClaimTypes.Email, email));

            var reqMock = new Mock<HttpRequest>();

            reqMock.SetupGet(r => r.Headers)
                .Returns(new HeaderDictionary());

            var connectionMock = new Mock<ConnectionInfo>();

            connectionMock.SetupGet(c => c.RemoteIpAddress)
                .Returns(IPAddress.Parse(ipAddress))
                .Verifiable();

            var httpMock = new Mock<HttpContext>();

            httpMock.SetupGet(c => c.Connection)
                .Returns(connectionMock.Object)
                .Verifiable();

            httpMock.SetupGet(c => c.User)
                .Returns(user)
                .Verifiable();

            httpMock.Setup(m => m.Items)
                .Returns(new Dictionary<object, object>());

            httpMock.SetupGet(c => c.Request)
                .Returns(reqMock.Object);

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
                () => middleware.Invoke(httpMock.Object));

            clientMock.Verify();
        }

        [Fact]
        public async Task Honors429RetryAfter()
        {
            var reqMock = new Mock<HttpRequest>();

            var httpMock = new Mock<HttpContext>();

            httpMock.Setup(m => m.Items)
                .Returns(new Dictionary<object, object>());

            httpMock.SetupGet(c => c.Request)
                .Returns(reqMock.Object);

            var clientMock = new Mock<SentryClient>(
                Dsn.Create(ExceptionReportingOptions.Dsn));

            var retryAfter = TimeSpan.FromMilliseconds(100);

            clientMock.Setup(m => m
                .SendEventAsync(It.IsAny<SentryEventData>()))
                .ReturnsAsync(new SentryResponse
                {
                    StatusCode = 429,
                    RetryAfter = retryAfter,
                })
                .Verifiable();

            var middleware = CreateMiddleware(Next_Throw, clientMock.Object);

            // Invoke once, get "RetryAfter"

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => middleware.Invoke(httpMock.Object));

            // Invoke twice: reject

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => middleware.Invoke(httpMock.Object));

            clientMock.Verify(m => m
                .SendEventAsync(It.IsAny<SentryEventData>()),
                Times.Once());

            // Delay for requested period

            await Task.Delay(retryAfter);

            // Inoke again, responds with 429, but sends request.

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => middleware.Invoke(httpMock.Object));

            clientMock.Verify(m => m
                .SendEventAsync(It.IsAny<SentryEventData>()),
                Times.Exactly(2));
        }

        public ExceptionReportingMiddleware CreateMiddleware(RequestDelegate next,
            SentryClient client)
            => new ExceptionReportingMiddleware(next,
                Options.Create(ExceptionReportingOptions),
                client,
                new RateLimiter());
    }
}
