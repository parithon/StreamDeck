using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK.Events
{
    public class SettingsEventArgs : StreamDeckEventArgs
    {
        public Dictionary<string, string> Settings { get; internal set; } = new Dictionary<string, string>();
    }
}
