using StreamDeck.SDK.Abstractions;
using StreamDeck.SDK.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK
{
    public abstract class StreamDeckBaseActionState : IStreamDeckActionState
    {
        public string Image { get; protected set; }

        public string TitleAlignment { get; protected set; }

        public string FontSize { get; protected set; }
    }
}
