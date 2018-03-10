using System;

namespace Pidget.Client
{
    /// <summary>
    /// Represents a sentry DSN, or "Data Source Name".
    /// You can set up a project at https://sentry.io to get a new DSN.
    /// </summary>
    public class Dsn
    {
        /// <summary>
        /// The underlaying URI which stores the DSN.
        /// </summary>
        public Uri Uri { get; }

        private Dsn(Uri uri)
            => Uri = uri;

        /// <summary>
        /// Creates a new DSN from the provided DSN string.
        /// </summary>
        public static Dsn Create(string dsn)
        {
            Assert.ArgumentNotNull(dsn, nameof(dsn));

            return new Dsn(new Uri(dsn));
        }

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

        public override string ToString()
            => Uri.ToString();

        /// <summary>
        /// Gets the URL for where to POST events to.
        /// </summary>
        public string GetCaptureUrl()
            => string.Concat(
                Uri.Scheme,
                Uri.SchemeDelimiter,
                Uri.DnsSafeHost,
                !Uri.IsDefaultPort ? $":{Uri.Port}" : null,
                "/api/",
                GetProjectId(),
                "/store/");

        private int LastIndexOfSlash(string input)
            => input.LastIndexOf('/');
    }
}
