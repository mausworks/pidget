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
    }
}
