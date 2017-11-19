namespace Pidget.Client.DataModels
{
    /// <summary>
    /// Describes the device that caused an event,
    /// most appropriate for mobile applications.
    /// </summary>
    public class DeviceData : ArbitraryData
    {
        public string Name
        {
            get => this["name"] as string;
            set => this["name"] = value;
        }

        public string Family
        {
            get => this["family"] as string;
            set => this["family"] = value;
        }

        public string Model
        {
            get => this["model"] as string;
            set => this["model"] = value;
        }

        public string ModelId
        {
            get => this["model_id"] as string;
            set => this["model_id"] = value;
        }

        public string Architecture
        {
            get => this["arch"] as string;
            set => this["arch"] = value;
        }

        public double? BatteryLevel
        {
            get => this["arch"] as double?;
            set => this["battery_level"] = value;
        }

        public string Orientation
        {
            get => this["orientation"] as string;
            set => this["orientation"] = value;
        }
    }
}
