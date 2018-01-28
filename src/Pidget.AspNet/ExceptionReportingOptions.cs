namespace Pidget.AspNet
{
    public class ExceptionReportingOptions
    {
        public string Dsn { get; set; }

        public SanitationOptions Sanitation { get; set; }
            = SanitationOptions.Default;
    }
}
