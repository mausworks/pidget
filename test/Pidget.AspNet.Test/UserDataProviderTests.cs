using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace Pidget.AspNet
{
    public class UserDataProviderTests
    {
        [Theory, InlineData("1")]
        public void GetId_UsesNameIdentifierClaim(string id)
        {
            var user = MakeUser(Tuple.Create(ClaimTypes.NameIdentifier, id));

            Assert.Equal(id, UserDataProvider.Default.GetId(user));
        }

        [Theory, InlineData("foo@bar")]
        public void GetEmail_ReturnsEmailClaim(string email)
        {
            var user = MakeUser(Tuple.Create(ClaimTypes.Email, email));

            Assert.Equal(email, UserDataProvider.Default.GetEmail(user));
        }

        [Theory, InlineData("user")]
        public void GetUserName_ReturnsIdentityName(string userName)
        {
            var identityMock = new Mock<IIdentity>();

            identityMock.SetupGet(i => i.Name)
                .Returns(userName)
                .Verifiable();

            var user = new ClaimsPrincipal(identityMock.Object);

            Assert.Equal(userName, UserDataProvider.Default.GetUserName(user));

            identityMock.Verify();
        }

        [Theory, InlineData("user")]
        public void GetUserName_NoIdentityFallbacksToNameClaim(string userName)
        {
            var user = MakeUser(Tuple.Create(ClaimTypes.Name, userName));

            Assert.Equal(userName, UserDataProvider.Default.GetUserName(user));
        }

        [Theory, InlineData("user")]
        public void GetUserName_NoIdentityNameFallbacksToNameClaim(
            string userName)
        {
            var identityMock = new Mock<ClaimsIdentity>();

            identityMock.SetupGet(i => i.Name)
                .Returns(null as string)
                .Verifiable();

            identityMock.Setup(i => i.FindFirst(ClaimTypes.Name))
                .Returns(new Claim(ClaimTypes.Name, userName))
                .Verifiable();

            var user = new ClaimsPrincipal(identityMock.Object);

            Assert.Equal(userName, UserDataProvider.Default.GetUserName(user));

            identityMock.Verify();
        }

        [Theory, InlineData("0.0.0.0")]
        public void GetIpAddress(string ipAddress)
        {
            var connMock = new Mock<ConnectionInfo>();

            connMock.SetupGet(c => c.RemoteIpAddress)
                .Returns(IPAddress.Parse(ipAddress))
                .Verifiable();

            var reqMock = new Mock<HttpRequest>();

            reqMock.SetupGet(r => r.Headers)
                .Returns(new HeaderDictionary());

            var httpMock = new Mock<HttpContext>();

            httpMock.SetupGet(c => c.Connection)
                .Returns(connMock.Object);

            httpMock.SetupGet(c => c.Request)
                .Returns(reqMock.Object);

            Assert.Equal(ipAddress,
                UserDataProvider.Default.GetIpAddress(httpMock.Object));

            connMock.Verify();
        }

        [Theory, InlineData("1.1.1.1", "0.0.0.0")]
        public void GetIpAddress_PrefersHttpXForwardedFor(
            string xForwardedFor, string ipAddress)
        {
            var headers = new HeaderDictionary();
            headers.Add("X-Forwarded-For", xForwardedFor);

            var reqMock = new Mock<HttpRequest>();

            reqMock.SetupGet(r => r.Headers)
                .Returns(headers);

            var connectionMock = new Mock<ConnectionInfo>();

            connectionMock.SetupGet(c => c.RemoteIpAddress)
                .Returns(IPAddress.Parse(ipAddress));

            var httpMock = new Mock<HttpContext>();

            httpMock.SetupGet(c => c.Connection)
                .Returns(connectionMock.Object);

            httpMock.SetupGet(c => c.Request)
                .Returns(reqMock.Object);

            Assert.Equal(xForwardedFor,
                UserDataProvider.Default.GetIpAddress(httpMock.Object));
        }

        [Theory]
        [InlineData("1", "foo", "foo@bar", null)]
        [InlineData("1", "foo", "foo@bar", "0.0.0.0")]
        public void GetUserData(string id,
            string userName,
            string email,
            string ipAddress)
        {
            var user = MakeUser(Tuple.Create(ClaimTypes.NameIdentifier, id),
                Tuple.Create(ClaimTypes.Name, userName),
                Tuple.Create(ClaimTypes.Email, email));

            var connMock = new Mock<ConnectionInfo>();

            connMock.SetupGet(c => c.RemoteIpAddress)
                .Returns(ipAddress != null
                    ? IPAddress.Parse(ipAddress)
                    : null)
                .Verifiable();

            var httpMock = new Mock<HttpContext>();

            httpMock.SetupGet(c => c.User)
                .Returns(user)
                .Verifiable();

            httpMock.SetupGet(c => c.Connection)
                .Returns(connMock.Object)
                .Verifiable();

            httpMock.SetupGet(c => c.Request)
                .Returns(Mock.Of<HttpRequest>());

            var data = UserDataProvider.Default.GetUserData(httpMock.Object);

            Assert.Equal(id, data.Id);
            Assert.Equal(userName, data.UserName);
            Assert.Equal(email, data.Email);
            Assert.Equal(ipAddress, data.IpAddress);

            connMock.Verify();
            httpMock.Verify();
        }

        [Fact]
        public void GetUserData_NoClaims()
        {
            var user = MakeUser();

            var httpMock = new Mock<HttpContext>();

            httpMock.SetupGet(c => c.User)
                .Returns(user)
                .Verifiable();

            httpMock.SetupGet(r => r.Request)
                .Returns(Mock.Of<HttpRequest>());

            var data = UserDataProvider.Default.GetUserData(httpMock.Object);

            Assert.Null(data.Id);
            Assert.Null(data.UserName);
            Assert.Null(data.Email);
            Assert.Null(data.IpAddress);

            httpMock.Verify();
        }

        public static ClaimsPrincipal MakeUser(params Tuple<string, string>[] claims)
        {
            var identity = new ClaimsIdentity(claims
                .Select(c => new Claim(c.Item1, c.Item2)));

            return new ClaimsPrincipal(identity);
        }
    }
}
