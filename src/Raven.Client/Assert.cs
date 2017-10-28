using System;

namespace Raven.Client
{
    internal static class Assert
    {
        public static void ArgumentNotNull(object obj, string name)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
