using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pidget.Client.DataModels
{
    /// <summary>
    /// Contains a list with frames, each with various bits of information describing the context of that frame.
    /// Frames should be sorted from oldest to newest.
    /// </summary>
    public class StackTraceData
    {
        [JsonProperty("frames")]
        public IList<FrameData> Frames { get; set; }

        public static StackTraceData FromException(Exception exception)
        {
            Assert.ArgumentNotNull(exception, nameof(exception));

            return new StackTraceData
            {
                Frames = GetFrames(exception).Reverse()
                    .Select(FrameData.FromStackFrame)
                    .ToArray()
            };
        }

        private static IEnumerable<StackFrame> GetFrames(Exception exception)
            => new StackTrace(exception).GetFrames() ?? new StackFrame[0];
    }
}
