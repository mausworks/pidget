using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pidget.Client;

namespace Pidget.AspNet.Setup
{
    using BeforeSendFunc = Func<SentryEventBuilder, HttpContext, Task<bool>>;
    using AfterSendFunc = Func<SentryResponse, HttpContext, Task>;

    public readonly struct CallbackSetup
    {
        private readonly SentryOptions _options;

        public CallbackSetup(SentryOptions options)
            => _options = options;

        public CallbackSetup BeforeSend(BeforeSendFunc callback)
        {
            _options.BeforeSendCallback = callback;

            return this;
        }

        public CallbackSetup AfterSend(AfterSendFunc callback)
        {
            _options.AfterSendCallback = callback;

            return this;
        }
    }
}
