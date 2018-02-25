using System.Diagnostics;
using System.Reflection;

namespace Pidget.Client
{
    internal static class VersionNumber
    {
        public static string Get()
            => FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location).ProductVersion;
    }
}
