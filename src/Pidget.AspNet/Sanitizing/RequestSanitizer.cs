using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pidget.AspNet.Sanitizing
{
    public class RequestSanitizer
    {
        private readonly NameValueSanitizer _itemSanitizer;

        public RequestSanitizer(SanitationOptions options)
            => _itemSanitizer = new NameValueSanitizer(options);

        public IDictionary<string, string> SanitizeForm(HttpRequest request)
            => request.Form?.ToDictionary(f => f.Key, f =>
                _itemSanitizer.SanitizeValue(f.Key, f.Value));

        public IDictionary<string, string> SanitizeHeaders(HttpRequest request)
            => request.Headers.Where(h => !IsCookieHeader(h.Key))
                .ToDictionary(f => f.Key, SanitizeHeaderValue);

        public IDictionary<string, string> SanitizeCookies(HttpRequest request)
            => request.Cookies.ToDictionary(c => c.Key, SanitizeCookieValue);

        public string SanitizeUrl(HttpRequest request)
            => string.Concat(request.Scheme,
                Uri.SchemeDelimiter,
                request.Host,
                request.Path,
                QueryString.Create(SanitizeQuery(request)).ToUriComponent());

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
    }
}
