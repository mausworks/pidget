using System;

namespace Pidget.AspNet
{
    public class RateLimit
    {
        private DateTimeOffset _retryAfter;

        public void Until(DateTimeOffset when)
            => _retryAfter = when;

        public bool IsHit(DateTimeOffset at)
            => at < _retryAfter;
    }
}
