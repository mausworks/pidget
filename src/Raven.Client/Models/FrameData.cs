using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

namespace Raven.Client.Models
{
    public class FrameData
    {
        private const string UnknownMemberReplacement = "(unknown)";

        [JsonProperty("abs_path")]
        public string AbsolutePath { get; set; }

        [JsonProperty("lineno")]
        public int LineNumber { get; set; }

        [JsonProperty("colno")]
        public int ColumnNumber { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }

        [JsonProperty("function")]
        public string Function { get; set;}

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
                Module = GetModule(method),
                Function = GetMethodName(method),
                ContextLine = GetMethodSource(method)
            };
        }

        private static bool IsInApp(MethodBase method)
        {
            if (method == null)
            {
                return true;
            }

            var module = GetModule(method);

            return !module.StartsWith("System.", StringComparison.Ordinal)
                && !module.StartsWith("Microsoft.", StringComparison.Ordinal);
        }

        private static int GetLineNumber(StackFrame stackFrame)
        {
            var lineNumber = stackFrame.GetFileLineNumber();

            return lineNumber != 0 ? lineNumber
                : stackFrame.GetILOffset();
        }

        private static string GetMethodSource(MethodBase method)
            => method?.ToString()
            ?? UnknownMemberReplacement;

        private static string GetMethodName(MethodBase method)
            => method?.Name
            ?? UnknownMemberReplacement;

        private static string GetModule(MethodBase method)
            => method?.DeclaringType.FullName
            ?? UnknownMemberReplacement;
    }
}
