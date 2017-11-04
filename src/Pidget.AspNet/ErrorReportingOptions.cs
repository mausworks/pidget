namespace Pidget.AspNet
{
    public class ErrorReportingOptions
    {
        public string Dsn { get; set; }

        public SanitationOptions Sanitaztion { get; set; } = SanitationOptions.Default;
    }
}
