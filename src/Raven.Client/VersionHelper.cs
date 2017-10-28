using System.Diagnostics;
using System.Reflection;

namespace Raven.Client
{
    internal static class VersionHelper
    {
        public static string GetVersionNumber()
            => FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location).ProductVersion;
    }
}
