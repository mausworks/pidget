using Pidget.Client.Http;
using System.Text;

namespace Pidget.Client
{
    public static class Sentry
    {
        /// <summary>
        /// The Sentry protocol version which the client uses.
        /// </summary>
        public const string CurrentProtocolVersion = "7";

        internal const string CSharpPlatformIdentifier = "csharp";

        /// <summary>
        /// The encoding which the Sentry API uses.
        /// </summary>
        public static Encoding ApiEncoding { get; }
            = new UTF8Encoding(false, false);

        /// <summary>
        /// Creates a new client using the provided DSN.
        /// </summary>
        /// <param name="dsn">The Sentry DSN, or Data Source Name.</param>
        public static SentryClient CreateClient(Dsn dsn)
            => SentryHttpClient.Default(dsn);
    }
}
