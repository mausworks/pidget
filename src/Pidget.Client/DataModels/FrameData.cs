using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Pidget.Client.DataModels
{
    public class FrameData
    {
        [JsonProperty("lineno")]
        public int LineNumber { get; set; }

        [JsonProperty("colno")]
        public int ColumnNumber { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }

        [JsonProperty("function")]
        public string Function { get; set; }

        [JsonProperty(PropertyName = "context_line")]
        public string ContextLine { get; set; }

        [JsonProperty("in_app")]
        public bool InApp { get; set; }

        public static FrameData FromStackFrame(StackFrame stackFrame)
        {
            Assert.ArgumentNotNull(stackFrame, nameof(stackFrame));

            var method = stackFrame.GetMethod();

            return new FrameData
            {
                FileName = stackFrame.GetFileName(),
                LineNumber = GetLineNumber(stackFrame),
                ColumnNumber = stackFrame.GetFileColumnNumber(),
                InApp = IsInApp(method),
                Module = method.DeclaringType.FullName,
                Function = method.Name,
                ContextLine = method.ToString()
            };
        }

        private static bool IsInApp(MethodBase method)
        {
            var module = method.DeclaringType.FullName;

            return !module.StartsWith("System.", StringComparison.Ordinal)
                && !module.StartsWith("Microsoft.", StringComparison.Ordinal);
        }

        private static int GetLineNumber(StackFrame stackFrame)
        {
            var lineNumber = stackFrame.GetFileLineNumber();

            return lineNumber != 0 ? lineNumber
                : stackFrame.GetILOffset();
        }
    }
}
