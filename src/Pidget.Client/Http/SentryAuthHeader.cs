using System.Collections.Generic;

namespace Pidget.Client.Http
{
    public static class SentryAuthHeader
    {
        public const string Name = "X-Sentry-Auth";

        public static IEnumerable<string> GetValues(SentryAuth auth)
        {
            yield return Combine("Sentry sentry_version", auth.SentryVersion);
            yield return Combine("sentry_timestamp", auth.Timestamp);
            yield return Combine("sentry_key", auth.PublicKey);

            if (auth.SecretKey != null)
            {
                yield return Combine("sentry_secret", auth.SecretKey);
            }

            yield return Combine("sentry_client", auth.ClientVersion);
        }

        private static string Combine(string key, object value)
            => $"{key}={value}";
    }
}
