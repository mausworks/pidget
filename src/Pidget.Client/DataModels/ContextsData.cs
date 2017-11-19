namespace Pidget.Client.DataModels
{
    /// <summary>
    /// Provides additional contextual data, typically related to the current user.
    /// See also: https://docs.sentry.io/clientdev/interfaces/contexts/
    /// </summary>
    public class ContextsData : ArbitraryData
    {
        public OperatingSystemData OperatingSystem
        {
            get => this["os"] as OperatingSystemData;
            set => this["os"] = value;
        }

        public DeviceData Device
        {
            get => this["device"] as DeviceData;
            set => this["device"] = value;
        }

        public RuntimeData Runtime
        {
            get => this["runtime"] as RuntimeData;
            set => this["runtime"] = value;
        }
    }
}
