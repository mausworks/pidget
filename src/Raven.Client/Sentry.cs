using System.Text;
using Newtonsoft.Json.Serialization;
using Raven.Client.Http;

namespace Raven.Client
{
    public static class Sentry
    {
        public const string CurrentProtocolVersion = "7";

        public const string DefaultLoggerName = "root";

        internal const string CSharpPlatformIdentifier = "csharp";

        public static Encoding ApiEncoding { get; }
            = new UTF8Encoding(false, false);

        public static RavenClient GetRavenClient(Dsn dsn)
            => new RavenHttpClient(dsn);
    }
}
