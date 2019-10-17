using StreamDeck.SDK;
using StreamDeck.SDK.Abstractions;
using StreamDeck.SDK.Events;
using StreamDeck.SDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeck.Template.Actions
{
    public class DefaultCounterState : StreamDeckBaseActionState
    {
        public DefaultCounterState()
        {
            Image = "Resources/actionDefaultImage";
            TitleAlignment = "middle";
            FontSize = "16";
        }
    }

    public class CounterAction : StreamDeckBaseAction
    {
        public CounterAction()
        {
            var states = new List<IStreamDeckActionState>
            {
                new DefaultCounterState()
            };

            UUID = "com.elgato.counter.action";
            Icon = "Resources/actionIcon";
            Name = "Counter";
            States = states;
            SupportedInMultiActions = false;
            Tooltip = "How many times did you get pwned today? Keep track with this counter.";

            KeyDown += CounterAction_KeyDown;
            KeyUp += CounterAction_KeyUp;
        }

        private void CounterAction_KeyUp(object sender, KeyUpEventArgs e)
        {
        }

        private void CounterAction_KeyDown(object sender, KeyDownEventArgs e)
        {
        }
    }
}
