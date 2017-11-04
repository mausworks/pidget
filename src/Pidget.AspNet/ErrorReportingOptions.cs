namespace Pidget.AspNet
{
    public class ErrorReportingOptions
    {
        public string Dsn { get; set; }

        public SanitationOptions Sanitation { get; set; } = SanitationOptions.Default;
    }
}
