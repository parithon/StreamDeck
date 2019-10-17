using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK.Events
{
    public abstract class StreamDeckEventArgs : EventArgs
    {
        public Action<string, object> SetSettings { get; internal set; }
        public Action<string> SetTitle { get; internal set; }
    }
}
