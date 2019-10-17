using StreamDeck.SDK.Abstractions;
using StreamDeck.SDK.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK
{
    public abstract class StreamDeckBaseAction : IStreamDeckAction
    {
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<SettingsEventArgs> WillAppear;
        public event EventHandler<SettingsEventArgs> WillDisappear;

        public string UUID { get; protected set; }

        public string Icon { get; protected set; }

        public string Name { get; protected set; }

        public IEnumerable<IStreamDeckActionState> States { get; protected set; }

        public bool SupportedInMultiActions { get; protected set; }

        public string Tooltip { get; protected set; }

        public virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }
        public virtual void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        public virtual void OnWillAppear(SettingsEventArgs e)
        {
            WillAppear?.Invoke(this, e);
        }
        
        public virtual void OnWillDisappear(SettingsEventArgs e)
        {
            WillDisappear?.Invoke(this, e);
        }
    }
}
