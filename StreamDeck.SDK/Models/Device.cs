using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK.Models
{
    internal enum StreamDeckType
    {
        StreamDeck = 0,
        StreamDeckMini = 1,
        StreamDeckXL = 2,
        StreamDeckMobile = 3,
        StreamDeckTesting = 999
    }

    internal class Device
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Size Size { get; set; }

        public StreamDeckType Type { get; set; }
    }
}
