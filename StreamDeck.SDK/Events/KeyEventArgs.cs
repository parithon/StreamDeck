using System;
using System.Collections.Generic;

namespace StreamDeck.SDK.Events
{
    public class KeyEventArgs : StreamDeckEventArgs
    {
        public string Device { get; internal set; }
        public Dictionary<string, string> Settings { get; internal set; }
        public int Column { get; internal set; }
        public int Row { get; internal set; }
        public int State { get; internal set; }
        public int UserDesiredState { get; internal set; }
        public bool IsInMultiAction { get; internal set; }
    }
}
