using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Moq;
using Pidget.Client.DataModels;
using Xunit;

namespace Pidget.AspNet
{
    public class UserDataProviderTests
    {
        [Fact]
        public void GetUserData_ThrowsArgumentNull_ForNullContext()
            => Assert.Throws<ArgumentNullException>(()
                => UserDataProvider.Default.GetUserData(null));

        [Theory, InlineData("1")]
        public void GetId_UsesNameIdentifierClaim(string id)
        {
            var user = MockUser(Tuple.Create(ClaimTypes.NameIdentifier, id));

            Assert.Equal(id, UserDataProvider.Default.GetId(user));
        }

        [Theory, InlineData("foo@bar")]
        public void GetEmail_ReturnsEmailClaim(string email)
        {
            var user = MockUser(Tuple.Create(ClaimTypes.Email, email));

            Assert.Equal(email, UserDataProvider.Default.GetEmail(user));
        }

        [Theory]
        [InlineData("user")]
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

        [Theory]
        [InlineData("user")]
        public void GetUserName_NoIdentityFallbacksToNameClaim(string userName)
        {
            var user = MockUser(Tuple.Create(ClaimTypes.Name, userName));

            Assert.Equal(userName, UserDataProvider.Default.GetUserName(user));
        }

        [Theory]
        [InlineData("user")]
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
            var connectionMock = new Mock<ConnectionInfo>();

            connectionMock.SetupGet(c => c.RemoteIpAddress)
                .Returns(IPAddress.Parse(ipAddress))
                .Verifiable();

            Assert.Equal(ipAddress,
                UserDataProvider.Default.GetIpAddress(connectionMock.Object));

            connectionMock.Verify();
        }

        [Theory]
        [InlineData("1", "foo", "foo@bar", null)]
        [InlineData("1", "foo", "foo@bar", "0.0.0.0")]
        public void GetUserData(string id,
            string userName,
            string email,
            string ipAddress)
        {
            var user = MockUser(Tuple.Create(ClaimTypes.NameIdentifier, id),
                Tuple.Create(ClaimTypes.Name, userName),
                Tuple.Create(ClaimTypes.Email, email));

            var connectionMock = new Mock<ConnectionInfo>();

            connectionMock.SetupGet(c => c.RemoteIpAddress)
                .Returns(ipAddress != null
                    ? IPAddress.Parse(ipAddress)
                    : null)
                .Verifiable();

            var contextMock = new Mock<HttpContext>();

            contextMock.SetupGet(c => c.User)
                .Returns(user)
                .Verifiable();

            contextMock.SetupGet(c => c.Connection)
                .Returns(connectionMock.Object)
                .Verifiable();

            var data = UserDataProvider.Default.GetUserData(contextMock.Object);

            Assert.Equal(id, data.Id);
            Assert.Equal(userName, data.UserName);
            Assert.Equal(email, data.Email);
            Assert.Equal(ipAddress, data.IpAddress);

            connectionMock.Verify();
            contextMock.Verify();
        }

        [Fact]
        public void GetUserData_NoClaims_NoConnection()
        {
            var user = MockUser();
            var contextMock = new Mock<HttpContext>();

            contextMock.SetupGet(c => c.User)
                .Returns(user)
                .Verifiable();

            var data = UserDataProvider.Default.GetUserData(contextMock.Object);

            Assert.Null(data.Id);
            Assert.Null(data.UserName);
            Assert.Null(data.Email);
            Assert.Null(data.IpAddress);

            contextMock.Verify();
        }

        public static ClaimsPrincipal MockUser(params Tuple<string, string>[] claims)
        {
            var identity = new ClaimsIdentity(claims
                .Select(c => new Claim(c.Item1, c.Item2)));

            return new ClaimsPrincipal(identity);
        }
    }
}
