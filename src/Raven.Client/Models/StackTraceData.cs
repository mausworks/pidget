using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace Raven.Client.Models
{
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
