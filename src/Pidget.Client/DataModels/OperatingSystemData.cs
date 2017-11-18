namespace Pidget.Client.DataModels
{
    public class OperatingSystemData : ArbitraryData
    {
        public string Name
        {
            get => Get("name") as string;
            set => Set("name", value);
        }

        public string Version
        {
            get => Get("version") as string;
            set => Set("version", value);
        }

        public string Build
        {
            get => Get("build") as string;
            set => Set("build", value);
        }

        public string KernelVersion
        {
            get => Get("kernel_version") as string;
            set => Set("kernel_version", value);
        }

        public bool? Rooted
        {
            get => Get("rooted") as bool?;
            set => Set("rooted", value);
        }
    }
}
