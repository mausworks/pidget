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
