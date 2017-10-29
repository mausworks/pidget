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
            int timestamp,
            string publicKey,
            string secretKey)
            : this()
        {
            SentryVersion = sentryVersion;
            ClientVersion = clientVersion;
            Timestamp = timestamp;
            PublicKey = publicKey;
            SecretKey = secretKey;
        }

        public static SentryAuth Issue(RavenClient client, DateTimeOffset issuedAt)
            => new SentryAuth(
                sentryVersion: Sentry.CurrentProtocolVersion,
                clientVersion: string.Join("/", $"{client.Name}-csharp", client.Version),
                timestamp: UnixTimestamp.Create(issuedAt),
                publicKey: client.Dsn.GetPublicKey(),
                secretKey: client.Dsn.GetSecretKey());
    }
}
