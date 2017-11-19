namespace Pidget.Client.DataModels
{
    /// <summary>
    /// Describes the runtime in more detail.
    /// </summary>
    public class RuntimeData : ArbitraryData
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
    }
}
