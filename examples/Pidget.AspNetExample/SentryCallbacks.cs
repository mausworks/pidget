using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pidget.Client;

namespace Pidget.AspNetExample
{
    public static class SentryCallbacks
    {
        public static async Task<bool> BeforeSendAsync(
            SentryEventBuilder builder,
            HttpContext http)
        {
            var data = builder.Build();

            if (data.Message == "You shall not pass!")
            {
                builder.SetErrorLevel(ErrorLevel.Debug);

                // Send event
                return true;
            }

            await Task.Delay(100);

            // Reject event
            return false;
        }

        public static Task AfterSendAsync(SentryResponse response, HttpContext http)
            => Task.Run(() => http.Items["SentryEventId"] = response?.EventId);
    }
}
