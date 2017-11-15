using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Moq;
using Pidget.AspNet.Sanitizing;
using Xunit;

namespace Pidget.AspNet.Test
{
    public class HeaderSanitizerTests
    {
        [Theory, InlineData("password", "")]
        public void SanitizeValue(string key, string value)
        {
            var result = RequestSanitizer.Default.SanitizeValue(key, value);

            Assert.Equal(SanitationOptions.Default.ReplacementValue, result);
        }

        [Fact]
        public void SanitizeHeaders()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Headers).Returns(new HeaderDictionary(
                new Dictionary<string, StringValues>
                {
                    { "password", "" },
                    { "passwd", "" },
                    { "clearTextPassword", "" },
                    { "user_passwd", "" },
                    { "secret", "" },
                    { "secretKey", "" },
                    { "user_secret", "" },
                    { "Authentication", "" },
                    { "Token", "" }
                }));

            var headers = RequestSanitizer.Default.SanitizeHeaders(requestMock.Object);

            foreach (var kvp in headers)
            {
                Assert.Equal(SanitationOptions.Default.ReplacementValue,
                    kvp.Value);
            }
        }

        [Fact]
        public void SanitizeForm()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.ContentType)
                .Returns("application/x-www-form-urlencoded");
            requestMock.SetupGet(r => r.Form).Returns(new FormCollection(
                new Dictionary<string, StringValues>
                {
                    { "password", "" },
                    { "passwd", "" },
                    { "clearTextPassword", "" },
                    { "user_passwd", "" },
                    { "secret", "" },
                    { "secretKey", "" },
                    { "user_secret", "" },
                    { "cc", "1234 4567 9876 5432" }
                }));

            var form = RequestSanitizer.Default.SanitizeForm(requestMock.Object);

            foreach (var kvp in form)
            {
                Assert.Equal(SanitationOptions.Default.ReplacementValue,
                    kvp.Value);
            }
        }

        [Fact]
        public void SanitizeCookies()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Cookies).Returns(
                new RequestCookieCollection(
                    new Dictionary<string, string>
                    {
                        { "session", "" },
                        { "auth", "" },
                        { "token", "" },
                        { "password", "" },
                        { "secret", "" }
                    }));

            var cookies = RequestSanitizer.Default.SanitizeCookies(requestMock.Object);

            foreach (var kvp in cookies)
            {
                Assert.Equal(SanitationOptions.Default.ReplacementValue,
                    kvp.Value);
            }
        }

        [Theory]
        [InlineData("?foo=bar&bar=baz", "?foo=bar&bar=baz")]
        [InlineData("?passw=a", "?passw=OMITTED")]
        public void GetQuery_ReturnsProvidedQuery(string query, string expectedQuery)
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Query)
                .Returns(new QueryCollection(
                    QueryHelpers.ParseQuery(expectedQuery)));

            var sanitizedParams = RequestSanitizer.Default.SanitizeQuery(requestMock.Object);

            Assert.Equal(expectedQuery,
                actual: QueryString.Create(sanitizedParams).ToUriComponent());
        }
    }
}
