namespace Pidget.Client.DataModels
{
    /// <summary>
    /// Defines the operating system which caused an event.
    /// </summary>
    public class OperatingSystemData : ArbitraryData
    {
        public string Name
        {
            get => this["name"] as string;
            set => this["name"] = value;
        }

        public string Version
        {
            get => this["version"] as string;
            set => this["version"] = value;
        }

        public string Build
        {
            get => this["build"] as string;
            set => this["build"] = value;
        }

        public string KernelVersion
        {
            get => this["kernel_version"] as string;
            set => this["kernel_version"] = value;
        }

        public bool? Rooted
        {
            get => this["rooted"] as bool?;
            set => this["rooted"] = value;
        }
    }
}
