namespace Pidget.Client.DataModels
{
    public class DeviceData : ArbitraryData
    {
        public string Name
        {
            get => Get("name") as string;
            set => Set("name", value);
        }

        public string Family
        {
            get => Get("family") as string;
            set => Set("family", value);
        }

        public string Model
        {
            get => Get("model") as string;
            set => Set("model", value);
        }

        public string ModelId
        {
            get => Get("model_id") as string;
            set => Set("model_id", value);
        }

        public string Architecture
        {
            get => Get("arch") as string;
            set => Set("arch", value);
        }

        public double BatteryLevel
        {
            get => Get("battery_level") as string;
            set => Set("battery_level", value);
        }

        public string Orientation
        {
            get => Get("orientation") as string;
            set => Set("orientation", value);
        }
    }
}
