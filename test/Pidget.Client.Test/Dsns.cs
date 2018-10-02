namespace Pidget.Client.Test
{
    public static class Dsns
    {
        public const string PublicKey = "public_key";

        public const string SecretKey = "secret_key";

        public const string Host = "host";

        public const string ProjectId = "project_id";

        public const string Path = "/path/";


        public const string SentryDsn =
            "https://" + PublicKey + "@" + Host + Path + ProjectId;

        public const string LegacyDsn =
            "https://" + PublicKey + ":" + SecretKey + "@" +
            Host + Path + ProjectId;
    }
}
