using StreamDeck.SDK.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK.Models
{
    internal class Manifest
    {
        public List<IStreamDeckAction> Actions { get; set; } = new List<IStreamDeckAction>();
        public int SDKVersion { get; set; }
        public string Author { get; set; }
        public string CodePath { get; set; }
        public string CodePathMac { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string URL { get; set; }
        public string Version { get; set; }
        public dynamic OS { get; set; }
        public dynamic Software { get; set; }
    }
}
