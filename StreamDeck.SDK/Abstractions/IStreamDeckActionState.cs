using StreamDeck.SDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.SDK.Abstractions
{
    public interface IStreamDeckActionState
    {
        string Image { get; }
        string TitleAlignment { get; }
        string FontSize { get; }
    }
}
