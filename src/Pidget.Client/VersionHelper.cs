using System.Diagnostics;
using System.Reflection;

namespace Pidget.Client
{
    internal static class VersionHelper
    {
        private static string _versionNumber;

        public static string GetVersionNumber()
            => (_versionNumber ?? (_versionNumber = GetProductVersion()));

        private static string GetProductVersion()
            => FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location).ProductVersion;
    }
}
