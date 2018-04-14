using System.Diagnostics;
using System.Reflection;

namespace Pidget.Client
{
    internal static class VersionNumber
    {
        private static string _cached;

        public static string Get()
            => _cached ?? (_cached = FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location).ProductVersion);
    }
}
