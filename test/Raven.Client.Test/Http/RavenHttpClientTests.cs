using System;
using System.IO;
using System.Threading.Tasks;
using Raven.Client.Http;
using Xunit;

namespace Raven.Client.Test
{
    public class RavenHttpClientTests
    {
        public static readonly Dsn Dsn = Dsn.Create(GetDsn());

        [Fact]
        public async Task SendException_ReturnsEventId()
        {
            var client = Sentry.GetRavenClient(Dsn);

            var value = 0;

            try
            {
                var x = 10 / value;
            }
            catch (Exception ex)
            {
                var id = await client.CaptureAsync(e => e
                    .SetException(ex)
                    .SetErrorLevel(ErrorLevel.Warning)
                    .AddExtraData("test", new
                    {
                        IsTest = true
                    })
                    .AddTag("test_tag", "yes"));

                Assert.NotNull(id);
            }
        }

        private static string GetDsn()
        {
            var cwd = Directory.GetCurrentDirectory();

            return File.ReadAllText(Path.Combine(cwd, "dsn.txt")).Trim();
        }
    }
}
