namespace Pidget.AspNet
{
    /// <summary>
    /// Options used for sanitizing data, e.g. posted for data.
    /// See also: https://docs.sentry.io/clientdev/data-handling/#sensitive-data
    /// </summary>
    public class SanitationOptions
    {
        public static SanitationOptions Default { get; }
            = new SanitationOptions
            {
                ReplacementValue = "OMITTED",
                ValuePatterns = new[]
                {
                    "^(?:\\d[ -]*?){13,16}$"
                },
                NamePatterns = new[]
                {
                    ".?passw.?",
                    ".?secret.?"
                }
            };

        public string[] NamePatterns { get; set; }

        public string[] ValuePatterns { get; set; }

        public string ReplacementValue { get; set; }
    }
}
