using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Pidget.Client.Http;
using Pidget.Client.Test;
using Xunit;

namespace Pidget.Client.Test.Http
{
    public class SentryAuthHeaderTests
    {
        public static readonly DateTime IssuedAt = new DateTime(2000, 01, 01);

        [Theory]
        [InlineData(Dsns.SentryDsn)]
        [InlineData(Dsns.LegacyDsn)]
        public void SentryDsn_ContainsExpectedValues(string dsnValue)
        {
            var dsn = Dsn.Create(dsnValue);
            var auth = SentryAuth.Issue(dsn, IssuedAt);
            var values = SentryAuthHeader.GetValues(auth);

            Assert.Contains($"Sentry sentry_version={auth.SentryVersion}", values);
            Assert.Contains($"sentry_timestamp={auth.Timestamp}", values);
            Assert.Contains($"sentry_key={auth.PublicKey}", values);

            if (dsn.GetSecretKey() != null)
            {
                Assert.Contains($"sentry_secret={auth.SecretKey}", values);
            }
            else
            {
                Assert.DoesNotContain("sentry_secret", values);
            }

            Assert.Contains($"sentry_client={auth.ClientVersion}", values);
        }
    }
}
