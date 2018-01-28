using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pidget.AspNet.Sanitizing
{
    public class RequestSanitizer
    {
        private readonly NameValueSanitizer _itemSanitizer;

        public RequestSanitizer(SanitationOptions options)
            => _itemSanitizer = new NameValueSanitizer(options);

        public string SanitizeValue(string key, string value)
            => _itemSanitizer.SanitizeValue(key, value);

        public IDictionary<string, string> SanitizeForm(HttpRequest request)
            => request.Form.ToDictionary(f => f.Key, f =>
                _itemSanitizer.SanitizeValue(f.Key, f.Value));

        public IDictionary<string, string> SanitizeHeaders(HttpRequest request)
            => request.Headers.Where(h => !IsCookieHeader(h.Key))
                .ToDictionary(f => f.Key, SanitizeHeaderValue);

        public IDictionary<string, string> SanitizeCookies(HttpRequest request)
            => request.Cookies.ToDictionary(c => c.Key, SanitizeCookieValue);

        public IDictionary<string, string> SanitizeQuery(HttpRequest request)
            => request.Query.ToDictionary(k => k.Key, SanitizeHeaderValue);

        private string SanitizeCookieValue(
            KeyValuePair<string, string> kvp)
            => IsAuth(kvp.Key) || IsSession(kvp.Key)
                ? _itemSanitizer.ReplacementValue
                : _itemSanitizer.SanitizeValue(kvp.Key, kvp.Value);

        private string SanitizeHeaderValue(
            KeyValuePair<string, StringValues> kvp)
            => IsAuth(kvp.Key)
                ? _itemSanitizer.ReplacementValue
                : _itemSanitizer.SanitizeValue(kvp.Key, kvp.Value);

        private bool IsSession(string name)
            => name.IndexOf("sess", StringComparison.OrdinalIgnoreCase) > -1;

        private bool IsAuth(string name)
            => name.IndexOf("auth", StringComparison.OrdinalIgnoreCase) > -1
            || name.IndexOf("token", StringComparison.OrdinalIgnoreCase) > -1;

        private bool IsCookieHeader(string name)
            => name.IndexOf("cookie", StringComparison.OrdinalIgnoreCase) > -1;

        /// <summary>
        /// Returns a new instance of a request sanitizer with default options.
        /// </summary>
        public static RequestSanitizer Default
            => new RequestSanitizer(SanitationOptions.Default);

        /// <summary>
        /// Returns a new instance of a no-op request sanitizer.
        /// </summary>
        public static RequestSanitizer Noop
            => new RequestSanitizer(SanitationOptions.None);

    }
}
