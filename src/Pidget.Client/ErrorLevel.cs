using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pidget.Client
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum ErrorLevel
    {
        Error,
        Fatal,
        Warning,
        Info,
        Debug
    }
}
