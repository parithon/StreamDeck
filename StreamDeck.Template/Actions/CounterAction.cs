using StreamDeck.SDK;
using StreamDeck.SDK.Abstractions;
using StreamDeck.SDK.Events;
using StreamDeck.SDK.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

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
        private Timer _timer;

        public CounterAction()
        {
            var states = new List<IStreamDeckActionState>
            {
                new DefaultCounterState()
            };

            UUID = "com.elgato.counter-csharp.action";
            Icon = "Resources/actionIcon";
            Name = "Counter (C#)";
            States = states;
            SupportedInMultiActions = false;
            Tooltip = "How many times did you get pwned today? Keep track with this counter.";

            KeyDown += CounterAction_KeyDown;
            KeyUp += CounterAction_KeyUp;
            WillAppear += CounterAction_WillAppear;
        }

        private void CounterAction_WillAppear(object sender, SettingsEventArgs e)
        {
            if (e.Settings.ContainsKey("keyPressCounter"))
            {
                if (!int.TryParse(e.Settings["keyPressCounter"], out var keyPressCounter))
                {
                    SetDefaultValue();
                }
                else
                {
                    e.SetTitle($"{keyPressCounter}");
                }
            }
            else
            {
                SetDefaultValue();
            }

            void SetDefaultValue()
            {
                e.SetSettings("keyPressCounter", 0);
                e.SetTitle("0");
            }
        }

        private void CounterAction_KeyUp(object sender, KeyEventArgs e)
        {
            _timer.Dispose();
            int.TryParse(e.Settings["keyPressCounter"], out var keyPressCounter);
            keyPressCounter++;
            e.SetSettings("keyPressCounter", keyPressCounter);
            e.SetTitle($"{keyPressCounter}");
        }

        private void CounterAction_KeyDown(object sender, KeyEventArgs e)
        {
            _timer = new Timer(TimeSpan.FromSeconds(1.5).TotalMilliseconds)
            {
                AutoReset = false,
                Enabled = true
            };
            _timer.Elapsed += (_, ee) =>
            {
                e.SetSettings("keyPressCounter", -1);
                e.SetTitle("0");
            };
        }
    }
}
