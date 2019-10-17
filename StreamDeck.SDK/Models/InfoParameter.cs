using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK.Models
{
    internal class InfoParameter
    {
        public Application Application { get; set; }

        public PluginInfo Plugin { get; set; }

        public int DevicePixelRatio { get; set; }

        public List<Device> Devices { get; set; }
    }
}
