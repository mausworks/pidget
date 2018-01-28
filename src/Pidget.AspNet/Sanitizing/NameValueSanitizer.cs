using System.Linq;
using System.Text.RegularExpressions;

namespace Pidget.AspNet.Sanitizing
{
    public class NameValueSanitizer
    {
        public string ReplacementValue { get; }

        private readonly Regex[] _namePatterns;

        private readonly Regex[] _valuePatterns;

        public NameValueSanitizer(SanitationOptions options)
        {
            _namePatterns = options.NamePatterns
                .Select(CreateRegex).ToArray();

            _valuePatterns = options.ValuePatterns
                .Select(CreateRegex).ToArray();

            ReplacementValue = options.ReplacementValue;
        }

        public string SanitizeValue(string name, string value)
            => IsAllowed(name, value) ? value : ReplacementValue;

        private bool IsAllowed(string name, string value)
            => !_namePatterns.Any(p => p.IsMatch(name))
            && !_valuePatterns.Any(p => p.IsMatch(value));

        private static Regex CreateRegex(string pattern)
            => new Regex(pattern,
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
