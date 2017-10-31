
using System;
using System.Collections.Generic;
using System.Linq;
using Pidget.Client.Http;
using Pidget.Client.Test;
using Xunit;

namespace Pidget.Client.Test.Http
{
    public class SentryAuthHeaderTests
    {
        public static readonly Dsn Dsn = DsnTests.SentryDsn;

        public static readonly DateTime IssuedAt = new DateTime(2000, 01, 01);

        public static readonly SentryAuth Auth
            = SentryAuth.Issue(
                new TestableSentryClient(Dsn, _ => null), IssuedAt);

        [Fact]
        public void GetValues_ContainsExpectedValues()
        {
            var values = SentryAuthHeader.GetValues(Auth);

            Assert.Contains(
                $"Sentry sentry_version={Auth.SentryVersion}", values);
            Assert.Contains($"sentry_timestamp={Auth.Timestamp}", values);
            Assert.Contains($"sentry_key={Auth.PublicKey}", values);
            Assert.Contains($"sentry_secret={Auth.SecretKey}", values);
            Assert.Contains($"sentry_client={Auth.ClientVersion}", values);
        }
    }
}
