using System.Net.Http;

namespace Pidget.Client.Test
{
    public class TestSender : HttpMessageInvoker
    {
        public bool IsDisposed { get; private set; }

        public TestSender(HttpMessageHandler handler)
            : base(handler)
        {
        }

        public TestSender(HttpMessageHandler handler, bool disposeHandler)
            : base(handler, disposeHandler)
        {
        }

        protected override void Dispose(bool disposing)
            => IsDisposed = true;
    }
}
