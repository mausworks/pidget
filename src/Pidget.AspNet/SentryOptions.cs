using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pidget.AspNet.Setup;
using Pidget.Client;

namespace Pidget.AspNet
{
    public class SentryOptions
    {
        public string Dsn { get; set; }

        public SanitationOptions Sanitation { get; set; }
            = SanitationOptions.Default;

        public Func<SentryEventBuilder, HttpContext, Task<bool>> BeforeSendCallback { get; set; }
            = ((b, h) => Task.FromResult(true));
        public Func<SentryResponse, HttpContext, Task> AfterSendCallback { get; set; }
            = ((r, h) => Task.CompletedTask);

        public CallbackSetup Callbacks => new CallbackSetup(this);
    }
}
