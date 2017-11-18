namespace Pidget.Client.DataModels
{
    public class RuntimeData : ArbitraryData
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
    }
}
