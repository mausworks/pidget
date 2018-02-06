using System;

namespace Pidget.AspNet
{
    public class RateLimiter
    {
        private DateTimeOffset _retryAfter;

        public void LimitFor(TimeSpan duration)
            => _retryAfter = DateTimeOffset.UtcNow + duration;

        public bool IsRateLimited(DateTimeOffset when)
            => when < _retryAfter;
    }
}
