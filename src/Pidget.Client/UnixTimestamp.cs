using System;

namespace Pidget.Client
{
    internal static class UnixTimestamp
    {
        public static DateTime Epoch { get; }
            = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int Create(DateTimeOffset when)
            => (int)(when.Subtract(Epoch)).TotalSeconds;
    }
}
