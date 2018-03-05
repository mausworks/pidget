using System.Threading.Tasks;
using Xunit;

namespace Pidget.AspNet
{
    public class SentryOptionsTests
    {
        [Fact]
        public void CallbackSetup_SetsOptions()
        {
            var opts = new SentryOptions();
            var initialBeforeSend = opts.BeforeSendCallback;
            var initialAfterSend = opts.AfterSendCallback;

            opts.Callbacks.BeforeSend((b, h) => Task.FromResult(true))
                .AfterSend((r, h) => Task.CompletedTask);

            Assert.NotEqual(initialBeforeSend, opts.BeforeSendCallback);
            Assert.NotEqual(initialAfterSend, opts.AfterSendCallback);
        }
    }
}
