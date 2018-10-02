using System.Diagnostics;
using Pidget.Client.DataModels;
using Xunit;

namespace Pidget.Client.Test.DataModels
{
    public class FrameDataTests
    {
        [Fact]
        public void FromDefaultStackFrame()
        {
            // The default constructor for StackFrame
            // will extract the current executing function's data.
            var data = FrameData.FromStackFrame(new StackFrame());

            Assert.NotNull(data.Module);
            Assert.NotNull(data.Function);
            Assert.True(data.InApp);
            Assert.True(data.LineNumber > 0);
        }

        [Fact]
        public void FromNullStackFrame()
        {
            // One way to extract a "null" stack frame
            // (one that has erroneous values set)
            // is to just "skip all frames".
            // It seems to be fine to "overshoot".
            var stackFrame = new StackFrame(
                skipFrames: int.MaxValue);

            var data = FrameData.FromStackFrame(stackFrame);

            Assert.Null(data.Module);
            Assert.Null(data.Function);
            Assert.False(data.InApp);
            Assert.True(data.LineNumber <= 0);
        }
    }
}
