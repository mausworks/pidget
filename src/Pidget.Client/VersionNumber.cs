using System.Diagnostics;
using System.Reflection;

namespace Pidget.Client
{
    internal static class VersionNumber
    {
        private static string _versionNumber;

        public static string Get()
            => (_versionNumber ?? (_versionNumber = GetProductVersion()));

        private static string GetProductVersion()
            => FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location).ProductVersion;
    }
}
