using System;
using System.Collections.Generic;
using Xunit;

namespace Pidget.Client.Test
{
    using static Dsns;

    public class DsnTests
    {
        [Fact]
        public void CreateThrows_ForNullDsn()
            => Assert.Throws<ArgumentNullException>(()
                => Dsn.Create(null));

        [Theory]
        [InlineData(SentryDsn)]
        [InlineData(LegacyDsn)]
        public void GetPublicKey(string dsn)
            => Assert.Equal(PublicKey, Dsn.Create(dsn).GetPublicKey());

        [Fact]
        public void LegacyDsn_GetSecretKey()
            => Assert.Equal(SecretKey,
                Dsn.Create(LegacyDsn).GetSecretKey());

        [Fact]
        public void SentryDsn_HasNullSecretKey()
            => Assert.Null(Dsn.Create(SentryDsn).GetSecretKey());

        [Theory]
        [InlineData(SentryDsn)]
        [InlineData(LegacyDsn)]
        public void GetPath(string dsn)
            => Assert.Equal(Path, Dsn.Create(dsn).GetPath());

        [Theory]
        [InlineData(SentryDsn)]
        [InlineData(LegacyDsn)]
        public void GetProjectId(string dsn)
            => Assert.Equal(ProjectId, Dsn.Create(dsn).GetProjectId());

        [Theory]
        [InlineData(SentryDsn)]
        [InlineData(LegacyDsn)]
        public void ToString_IsOriginalString(string dsn)
            => Assert.Equal(dsn, Dsn.Create(dsn).ToString());

        [Theory]
        [InlineData(SentryDsn)]
        [InlineData(LegacyDsn)]
        public void GetCaptureUrl(string dsn)
            => Assert.Equal(
                expected: $"https://{Host}/api/{ProjectId}/store/",
                actual: Dsn.Create(dsn).GetCaptureUrl());

        [Theory]
        [InlineData(5000)]
        public void GetCaptureUrl_NonStandardPort(int port)
        {
            var dsn = Dsn.Create(
                $"https://{PublicKey}:{SecretKey}@{Host}:{port}{Path}{ProjectId}");

            Assert.Equal(
                expected: $"https://{Host}:{port}/api/{ProjectId}/store/",
                actual: dsn.GetCaptureUrl());
        }
    }
}
