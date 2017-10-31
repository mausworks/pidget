using System.Diagnostics;
using System.Reflection;

namespace Pidget.Client
{
    internal static class VersionHelper
    {
        public static string GetVersionNumber()
            => FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location).ProductVersion;
    }
}
