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
        public string Value { get; set; }

        [JsonProperty("stacktrace")]
        public StackTraceData Stacktrace { get; set; }

        public static ExceptionData FromException(Exception exception)
        {
            Assert.ArgumentNotNull(exception, nameof(exception));

            return new ExceptionData
            {
                Type = exception.GetType().FullName,
                Module =  exception.TargetSite?.DeclaringType.FullName ?? exception.Source,
                Value = exception.Message,
                Stacktrace = StackTraceData.FromException(exception)
            };
        }
    }
}
