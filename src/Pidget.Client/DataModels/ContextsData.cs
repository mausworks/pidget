using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Pidget.Client.DataModels
{
    public class ContextsData : ArbitraryData
    {
        public OperatingSystemData OperatingSystem
        {
            get => Get("os") as OperatingSystemData;
            set => Set("os", value);
        }

        public DeviceData Device
        {
            get => Get("device") as DeviceData;
            set => Set("device", value);
        }

        public RuntimeData Runtime
        {
            get => Get("runtime") as RuntimeData;
            set => Set("runtime", value);
        }
    }
}
