using System;

namespace Raven.Client
{
    public class Dsn
    {
        public Uri Uri { get; }

        private Dsn(Uri uri)
            => Uri = uri;

        public string GetProjectId()
            => Uri.AbsoluteUri.Substring(
                LastIndexOfSlash(Uri.AbsoluteUri) + 1);

        public string GetPath()
            => Uri.AbsolutePath.Substring(0,
                LastIndexOfSlash(Uri.AbsolutePath) + 1);

        public string GetPublicKey()
            => Uri.UserInfo.Split(':')[0];

        public string GetSecretKey()
            => Uri.UserInfo.Split(':')[1];

        private int LastIndexOfSlash(string input)
            => input.LastIndexOf('/');

        public override string ToString()
            => Uri.ToString();

        public string GetCaptureUrl()
            => string.Concat(
                Uri.Scheme,
                Uri.SchemeDelimiter,
                Uri.DnsSafeHost,
                !Uri.IsDefaultPort ? $":{Uri.Port}" : null,
                "/api/",
                GetProjectId(),
                "/store/");

        public static Dsn Create(string dsn)
        {
            Assert.ArgumentNotNull(dsn, nameof(dsn));

            return new Dsn(new Uri(dsn));
        }
    }
}
