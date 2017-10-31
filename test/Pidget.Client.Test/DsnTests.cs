using System;
using System.Collections.Generic;
using Xunit;

namespace Pidget.Client.Test
{
    public class DsnTests
    {
        public const string PublicKey = "PUBLIC_KEY";

        public const string SecretKey = "SECRET_KEY";

        public const string Host = "HOST";

        public const string ProjectId = "PROJECT_ID";

        public const string Path = "/PATH/";

        public static readonly Dsn SentryDsn = Dsn.Create(
            $"https://{PublicKey}:{SecretKey}@{Host}{Path}{ProjectId}");

        [Fact]
        public void GetPublicKey()
            => Assert.Equal(PublicKey, SentryDsn.GetPublicKey());

        [Fact]
        public void GetPrivateKey()
            => Assert.Equal(SecretKey, SentryDsn.GetSecretKey());

        [Fact]
        public void GetPath()
            => Assert.Equal(Path, SentryDsn.GetPath());

        [Fact]
        public void GetProjectId()
            => Assert.Equal(ProjectId, SentryDsn.GetProjectId());
    }
}
