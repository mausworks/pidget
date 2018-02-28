using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pidget.Client
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum ErrorLevel : byte
    {
        Error,
        Fatal,
        Warning,
        Info,
        Debug
    }
}
