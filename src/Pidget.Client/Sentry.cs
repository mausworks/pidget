using Pidget.Client.Http;
using System.Text;

namespace Pidget.Client
{
    public static class Sentry
    {
        public const string CurrentProtocolVersion = "7";

        public const string DefaultLoggerName = "root";

        internal const string CSharpPlatformIdentifier = "csharp";

        public static Encoding ApiEncoding { get; }
            = new UTF8Encoding(false, false);

        public static SentryClient CreateClient(Dsn dsn)
            => SentryHttpClient.Default(dsn);
    }
}
