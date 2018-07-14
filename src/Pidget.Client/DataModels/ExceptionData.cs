using Newtonsoft.Json;
using System;

namespace Pidget.Client.DataModels
{
    public class ExceptionData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }

        [JsonProperty("value")]
        public string Message { get; set; }

        [JsonProperty("stacktrace")]
        public StackTraceData Stacktrace { get; set; }

        public static ExceptionData FromException(Exception exception)
        {
            Assert.ArgumentNotNull(exception, nameof(exception));

            return new ExceptionData
            {
                Type = exception.GetType().FullName,
                Message = exception.Message,
                Stacktrace = StackTraceData.FromException(exception),
                Module = exception.TargetSite?.DeclaringType.FullName
                    ?? exception.Source
            };
        }
    }
}
