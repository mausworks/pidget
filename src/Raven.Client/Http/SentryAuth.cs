using System;

namespace Raven.Client.Http
{
    public struct SentryAuth
    {
        public string SentryVersion { get; }

        public string ClientVersion { get; }

        public int Timestamp { get; }

        public string PublicKey { get; }

        public string SecretKey { get; }

        private SentryAuth(string sentryVersion,
            string clientVersion,
            int timestap,
            string publicKey,
            string secretKey)
            : this()
        {
            SentryVersion = sentryVersion;
            ClientVersion = clientVersion;
            Timestamp = timestap;
            PublicKey = publicKey;
            SecretKey = secretKey;
        }

        public static SentryAuth Issue(RavenClient client)
            => new SentryAuth(
                sentryVersion: Sentry.CurrentProtocolVersion,
                clientVersion: string.Join("/", $"{client.Name}-csharp", client.Version),
                timestap: UnixTimestamp.Create(DateTimeOffset.UtcNow),
                publicKey: client.Dsn.GetPublicKey(),
                secretKey: client.Dsn.GetSecretKey());
    }
}
