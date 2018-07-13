using System;

namespace Pidget.AspNet
{
    /// <summary>
    /// Maintains a state for a rate limit.
    /// </summary>
    public class RateLimit
    {
        private DateTimeOffset _retryAfter = default;

        /// <summary>
        /// Extends the rate limit to the provided date.
        /// </summary>
        /// <param name="when">The date for which the rate limit expires.</param>
        public void Until(DateTimeOffset when)
            => _retryAfter = when;

        /// <summary>
        /// Whether or not the rate limit is hit at a provided date.
        /// </summary>
        /// <param name="at">The date to check whether the rate limit is hit at.</param>
        public bool IsHit(DateTimeOffset at)
            => at < _retryAfter;
    }
}
