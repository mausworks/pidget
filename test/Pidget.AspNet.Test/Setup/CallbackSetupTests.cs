using System.Threading.Tasks;
using Pidget.AspNet.Setup;
using Xunit;

namespace Pidget.AspNet.Test.Setup
{
    public class CallbackSetupTests
    {

        [Fact]
        public void SetsBeforeSendCallback()
        {
            var options = new SentryOptions();
            var initialCallback = options.BeforeSendCallback;

            var setup = new CallbackSetup(options);

            setup.BeforeSend((b, h) => Task.FromResult(true));

            Assert.NotEqual(initialCallback, options.BeforeSendCallback);
        }

        [Fact]
        public void SetsAfterSendCallback()
        {
            var options = new SentryOptions();
            var initialCallback = options.AfterSendCallback;

            var setup = new CallbackSetup(options);

            setup.AfterSend((b, h) => Task.CompletedTask);

            Assert.NotEqual(initialCallback, options.AfterSendCallback);
        }
    }
}
