using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Pidget.AspNet.Sanitizing;

namespace Pidget.AspNet
{
    public class RequestDataProvider
    {
        private readonly HttpRequest _request;

        private readonly RequestSanitizer _sanitizer;

        public RequestDataProvider(HttpRequest request, RequestSanitizer sanitizer)
        {
            _request = request;
            _sanitizer = sanitizer;
        }

        public bool TryGetCookies(out IDictionary<string, string> cookies)
        {
            if (_request.Cookies != null && _request.Cookies.Any())
            {
                cookies = _sanitizer.SanitizeCookies(_request);

                return true;
            }

            cookies = null;

            return false;
        }

        public bool TryGetHeaders(out IDictionary<string, string> headers)
        {
            if (_request.Headers != null && _request.Headers.Any())
            {
                headers = _sanitizer.SanitizeHeaders(_request);

                return true;
            }

            headers = null;

            return false;
        }

        public bool TryGetForm(out IDictionary<string, string> form)
        {
            // Using multipart/form-data requires special binding.
            if (IsUrlEncodedForm(_request.ContentType) && _request.Form != null)
            {
                form = _sanitizer.SanitizeForm(_request);

                return true;
            }

            form = null;

            return false;
        }

        private bool IsUrlEncodedForm(string contentType)
            => contentType != null && contentType.Equals(
                value: "application/x-www-form-urlencoded",
                comparisonType: StringComparison.OrdinalIgnoreCase);
    }
}
