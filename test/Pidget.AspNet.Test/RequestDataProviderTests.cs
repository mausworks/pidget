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
    public class RequestDataProviderTests
    {
        public static RequestDataProvider RequestData
            = new RequestDataProvider(RequestSanitizer.Noop);

        [Theory, InlineData("http", "fail.io", "/baz")]
        public void GetUrl_ReturnsProvidedUrl(string scheme,
            string host,
            string path)
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Scheme).Returns(scheme);
            requestMock.SetupGet(r => r.Host).Returns(new HostString(host));
            requestMock.SetupGet(r => r.Path).Returns(new PathString(path));

            var url = RequestData.GetUrl(requestMock.Object);

            Assert.Equal($"{scheme}://{host}{path}", url);
        }

        [Theory, InlineData("?foo=bar&bar=baz")]
        public void GetQuery_ReturnsProvidedQuery(string expectedQuery)
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Query)
                .Returns(new QueryCollection(
                    QueryHelpers.ParseQuery(expectedQuery)));

            var query = RequestData.GetQueryString(requestMock.Object);

            Assert.Equal(expectedQuery, query);
        }

        [Fact]
        public void GetQuery_HandlesNull()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Query)
                .Returns(() => null);

            var query = RequestData.GetQueryString(requestMock.Object);

            Assert.Null(query);
        }

        [Theory, InlineData("foo=bar", "bar=baz")]
        public void GetCookies_ReturnsProvidedCookies(
            params string[] cookieValues)
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Cookies)
                .Returns(new RequestCookieCollection(
                    PairsToDictionary(cookieValues, cv => cv)));

            var cookieValue = RequestData.GetCookies(requestMock.Object);

            Assert.Equal(string.Join("; ", cookieValues), cookieValue);
        }

        [Fact]
        public void NullCookies_ReturnsNull()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Cookies)
                .Returns(() => null);

            var cookieValue = RequestData.GetCookies(requestMock.Object);

            Assert.Null(cookieValue);
        }

        [Fact]
        public void NoCookies_ReturnsNull()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Cookies)
                .Returns(new RequestCookieCollection());

            var cookieValue = RequestData.GetCookies(requestMock.Object);

            Assert.Null(cookieValue);
        }

        [Theory, InlineData("foo=bar", "bar=baz")]
        public void GetHeaders_ReturnsProvidedHeaders(
            params string[] headerValues)
        {
            var requestMock = new Mock<HttpRequest>();

            var headerDictionary = new HeaderDictionary(
                PairsToDictionary(headerValues, s => new StringValues(s)));

            requestMock.SetupGet(r => r.Headers).Returns(headerDictionary);

            var headers = RequestData.GetHeaders(requestMock.Object);

            Assert.All(headers,
                h => Assert.Equal(headerDictionary[h.Key], h.Value));
        }

        [Fact]
        public void NullHeaders_ReturnsNull()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Headers)
                .Returns(() => null);

            var headers = RequestData.GetHeaders(requestMock.Object);

            Assert.Null(headers);
        }

        [Fact]
        public void NoHeaders_ReturnsNull()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Headers)
                .Returns(new HeaderDictionary());

            var headers = RequestData.GetHeaders(requestMock.Object);

            Assert.Null(headers);
        }

        [Theory, InlineData("application/x-www-form-urlencoded")]
        public void GetData_ReturnsFormForContentType(string contentType)
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.ContentType)
                .Returns(contentType);

            requestMock.SetupGet(r => r.Form)
                .Returns(new FormCollection(Enumerable.Range(1, 2)
                    .ToDictionary(i => $"k{i}", i => new StringValues($"{i}"))));

            var data = RequestData.GetData(requestMock.Object);

            Assert.True(((IDictionary<string, string>)data).Count > 0);
        }


        [Theory, InlineData("foo=bar", "bar=baz")]
        public void GetForm_ReturnsForm(params string[] formValues)
        {
            var requestMock = new Mock<HttpRequest>();

            var expectedForm = new FormCollection(
                PairsToDictionary(formValues, s => new StringValues(s)));

            requestMock.SetupGet(r => r.Form).Returns(expectedForm);

            var form = RequestData.GetForm(requestMock.Object);

            Assert.All(expectedForm,
                h => Assert.Equal(expectedForm[h.Key], h.Value));
        }

        [Fact]
        public void NullForm_ReturnsNull()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Form)
                .Returns(() => null);

            var form = RequestData.GetForm(requestMock.Object);

            Assert.Null(form);
        }

        [Theory, InlineData("ASPNET_ENVIRONMENT", "Development")]
        public void SetsEnvironmentVariables(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value);

            var envVars = RequestData.GetEnvironmentVariables();

            Assert.Contains(name, envVars.Keys);
            Assert.Contains(value, envVars.Values);
        }

        public void GetData()
        {
            var requestMock = new Mock<HttpRequest>();

            requestMock.SetupGet(r => r.Scheme).Returns("https").Verifiable();

            requestMock.SetupGet(r => r.Host).Returns(new HostString("foo.bar"))
                .Verifiable();

            requestMock.SetupGet(r => r.Path).Returns(new PathString("/error"))
                .Verifiable();

            requestMock.SetupGet(r => r.Method).Returns("POST")
                .Verifiable();

            requestMock.SetupGet(r => r.Form)
                .Returns(new FormCollection(
                    PairsToDictionary(new[] { "foo=bar" }, s => new StringValues(s))))
                .Verifiable();

            requestMock.SetupGet(r => r.Headers)
                .Returns(new HeaderDictionary(
                    PairsToDictionary(new[] { "foo=bar" }, s => new StringValues(s))))
                .Verifiable();

            requestMock.SetupGet(r => r.Cookies)
                .Returns(new RequestCookieCollection(
                    PairsToDictionary(new[] { "foo=bar" }, s => s)))
                .Verifiable();

            requestMock.SetupGet(r => r.ContentType)
                .Returns("application/x-www-form-urlencoded")
                .Verifiable();

            requestMock.SetupGet(r => r.Query)
                .Returns(new QueryCollection(
                    PairsToDictionary(new[] { "foo=bar" }, s => new StringValues(s))))
                .Verifiable();

            Environment.SetEnvironmentVariable("foo", "bar");

            var request = RequestData.GetRequestData(requestMock.Object);

            requestMock.Verify();

            Assert.NotNull(request.Method);
            Assert.NotNull(request.Url);
            Assert.NotNull(request.QueryString);
            Assert.NotNull(request.Headers);
            Assert.NotNull(request.Cookies);
            Assert.NotNull(request.Data);
            Assert.True(request.Environment
                .Contains(new KeyValuePair<string, string>("foo", "bar")));
        }

        private static Dictionary<string, TValue> PairsToDictionary<TValue>(
            IEnumerable<string> pairs, Func<string, TValue> valueSelector)
            => pairs.ToDictionary(p => p.Split('=')[0], s => valueSelector(s.Split('=')[1]));
    }
}
