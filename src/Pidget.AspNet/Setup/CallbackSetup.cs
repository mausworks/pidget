using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pidget.Client;

namespace Pidget.AspNet.Setup
{
    public struct CallbackSetup
    {
        private readonly SentryOptions _options;

        public CallbackSetup(SentryOptions options) : this()
            => _options = options;

        public CallbackSetup BeforeSend(Func<SentryEventBuilder, HttpContext, Task<bool>> callback)
        {
            _options.BeforeSendCallback = callback;

            return this;
        }

        public CallbackSetup AfterSend(Func<SentryResponse, HttpContext, Task> callback)
        {
            _options.AfterSendCallback = callback;

            return this;
        }
    }
}
