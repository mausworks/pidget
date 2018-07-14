using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pidget.AspNet.Setup;
using Pidget.Client;

namespace Pidget.AspNet
{
    using BeforeSendFunc = Func<SentryEventBuilder, HttpContext, Task<bool>>;
    using AfterSendFunc = Func<SentryResponse, HttpContext, Task>;

    public class SentryOptions
    {
        public string Dsn { get; set; }

        public SanitationOptions Sanitation { get; set; }
            = SanitationOptions.Default;

        public BeforeSendFunc BeforeSendCallback { get; set; }
            = ((b, h) => Task.FromResult(true));
        public AfterSendFunc AfterSendCallback { get; set; }
            = ((r, h) => Task.CompletedTask);

        public CallbackSetup Callbacks => new CallbackSetup(this);
    }
}
