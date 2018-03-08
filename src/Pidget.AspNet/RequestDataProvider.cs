using Microsoft.AspNetCore.Http;
using Pidget.AspNet.Sanitizing;
using Pidget.Client.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pidget.AspNet
{
    public class RequestDataProvider
    {
        private readonly RequestSanitizer _sanitizer;

        public RequestDataProvider(RequestSanitizer sanitizer)
            => _sanitizer = sanitizer;

        public RequestData GetRequestData(HttpRequest request)
            => new RequestData
            {
                Method = request.Method,
                Url = GetUrl(request),
                Data = GetData(request),
                Cookies = GetCookies(request),
                Headers = GetHeaders(request),
                QueryString = GetQueryString(request),
                Environment = GetEnvironmentVariables()
            };

        public string GetUrl(HttpRequest request)
            => string.Concat(request.Scheme,
                Uri.SchemeDelimiter,
                request.Host,
                request.Path);

        public string GetQueryString(HttpRequest request)
            => request.Query != null
                ? QueryString.Create(_sanitizer.SanitizeQuery(request))
                    .ToUriComponent()
                : null;

        public string GetCookies(HttpRequest request)
            => request.Cookies != null && request.Cookies.Any()
                ? StringifyCookies(_sanitizer.SanitizeCookies(request))
                : null;

        public IDictionary<string, string> GetHeaders(HttpRequest request)
            => request.Headers != null && request.Headers.Any()
                ? _sanitizer.SanitizeHeaders(request)
                : null;

        public object GetData(HttpRequest request)
            => IsUrlEncodedForm(request.ContentType)
                ? GetForm(request)
                : null;

        public IDictionary<string, string> GetForm(HttpRequest request)
            => request.Form != null
                ? _sanitizer.SanitizeForm(request)
                : null;

        public IDictionary<string, string> GetEnvironmentVariables()
        {
            var envVars = Environment.GetEnvironmentVariables();

            return envVars.Keys.Cast<string>()
                .ToDictionary(k => k, k => (string)envVars[k]);
        }

        private bool IsUrlEncodedForm(string contentType)
            => contentType != null && contentType.Equals(
                value: "application/x-www-form-urlencoded",
                comparisonType: StringComparison.OrdinalIgnoreCase);

        private string StringifyCookies(IDictionary<string, string> cookies)
            => string.Join("; ", cookies.Select(c => string.Join("=",
                Uri.EscapeDataString(c.Key),
                Uri.EscapeDataString(c.Value))));
    }
}
