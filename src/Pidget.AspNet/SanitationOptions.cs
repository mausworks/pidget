namespace Pidget.AspNet
{
    /// <summary>
    /// Options used for sanitizing form, header, cookie and query parameters.
    /// See also: https://docs.sentry.io/clientdev/data-handling/#sensitive-data
    /// </summary>
    public class SanitationOptions
    {
        public static SanitationOptions Default
            => new SanitationOptions
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

        public static SanitationOptions None
            => new SanitationOptions();

        public string[] NamePatterns { get; set; }

        public string[] ValuePatterns { get; set; }

        public string ReplacementValue { get; set; }
    }
}
