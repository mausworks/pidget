using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using Pidget.AspNet.Sanitizing;
using Xunit;

namespace Pidget.AspNet.Test
{
    public class HeaderSanitizerTests
    {
        public static RequestSanitizer Sanitizer
            = new RequestSanitizer(SanitationOptions.Default);

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

            var headers = Sanitizer.SanitizeHeaders(requestMock.Object);

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

            var form = Sanitizer.SanitizeForm(requestMock.Object);

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

            var cookies = Sanitizer.SanitizeCookies(requestMock.Object);

            foreach (var kvp in cookies)
            {
                Assert.Equal(SanitationOptions.Default.ReplacementValue,
                    kvp.Value);
            }
        }

        [Theory]
        [InlineData("https", "fail.io", "/culprit")]
        public void SanitizeQuery(string scheme, string host, string path)
        {
            var requestMock = new Mock<HttpRequest>();

            var query = new QueryCollection(
                new Dictionary<string, StringValues>
                {
                    { "auth", "" },
                    { "secret", "" },
                    { "token", "" },
                    { "cc", "1234 4567 9876 5432" }
                });

            requestMock.SetupGet(r => r.Scheme).Returns(scheme);
            requestMock.SetupGet(r => r.Host).Returns(new HostString(host));
            requestMock.SetupGet(r => r.Path).Returns(path);
            requestMock.SetupGet(r => r.Query).Returns(query);

            var url = Sanitizer.SanitizeUrl(requestMock.Object);

            Assert.Equal($"{scheme}://{host}{path}?" + string.Join("&",
                query.Select(q =>
                    $"{q.Key}={SanitationOptions.Default.ReplacementValue}")
            ), url);
        }
    }
}
