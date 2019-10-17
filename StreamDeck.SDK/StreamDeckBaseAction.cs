using StreamDeck.SDK.Abstractions;
using StreamDeck.SDK.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK
{
    public abstract class StreamDeckBaseAction : IStreamDeckAction
    {
        public event EventHandler<KeyDownEventArgs> KeyDown;
        public event EventHandler<KeyUpEventArgs> KeyUp;

        public string UUID { get; protected set; }

        public string Icon { get; protected set; }

        public string Name { get; protected set; }

        public IEnumerable<IStreamDeckActionState> States { get; protected set; }

        public bool SupportedInMultiActions { get; protected set; }

        public string Tooltip { get; protected set; }

        public virtual void OnKeyDown(KeyDownEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }
        public virtual void OnKeyUp(KeyUpEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }
    }
}
