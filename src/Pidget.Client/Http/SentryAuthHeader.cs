using System.Collections.Generic;

namespace Pidget.Client.Http
{
    public static class SentryAuthHeader
    {
        public const string Name = "X-Sentry-Auth";

        public static IEnumerable<string> GetValues(SentryAuth auth)
            => new[]
            {
                Combine("Sentry sentry_version", auth.SentryVersion),
                Combine("sentry_timestamp", auth.Timestamp),
                Combine("sentry_key", auth.PublicKey),
                Combine("sentry_secret", auth.SecretKey),
                Combine("sentry_client", auth.ClientVersion)
            };

        private static string Combine(string key, object value)
            => $"{key}={value}";
    }
}
