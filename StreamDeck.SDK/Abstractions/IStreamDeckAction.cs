using StreamDeck.SDK.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK.Abstractions
{
    public interface IStreamDeckAction
    {
        string UUID { get; }
        string Icon { get; }
        string Name { get; }
        IEnumerable<IStreamDeckActionState> States { get; }
        bool SupportedInMultiActions { get; }
        string Tooltip { get; }
        void OnKeyDown(KeyDownEventArgs e);
        void OnKeyUp(KeyUpEventArgs e);
    }
}
