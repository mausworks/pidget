using Microsoft.AspNetCore.Http;

namespace Pidget.AspNet
{
    public class ExceptionReportingOptions
    {
        public string Dsn { get; set; }

        public string EventIdKey { get; set; }
            = "SentryEventId";

        public SanitationOptions Sanitation { get; set; }
            = SanitationOptions.Default;
    }
}
