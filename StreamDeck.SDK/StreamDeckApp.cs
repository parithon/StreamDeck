using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using StreamDeck.SDK.Abstractions;
using StreamDeck.SDK.Events;
using StreamDeck.SDK.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamDeck.SDK
{
    internal class StreamDeckApp
    {
        private readonly ClientWebSocket _socket = new ClientWebSocket();
        private readonly Dictionary<string, IStreamDeckAction> _actions = new Dictionary<string, IStreamDeckAction>();
        private readonly IEnumerable<IStreamDeckAction> _availableActions = 
            Assembly.GetEntryAssembly().GetTypes()
                .Where(t => t.GetInterface(nameof(IStreamDeckAction)) != null)
                .Select(type => (IStreamDeckAction) Activator.CreateInstance(type));

        [Option("-port <PORT>", CommandOptionType.SingleValue)]
        public int Port { get; } = -1;

        [Option("-pluginUUID <UUID>", CommandOptionType.SingleValue)]
        public string UUID { get; }

        [Option("-registerEvent <EVENT>", CommandOptionType.SingleValue)]
        public string Event { get; }

        [Option("-info <INFO>", CommandOptionType.SingleValue)]
        public string InfoJSON { get; }

        public InfoParameter Info => !string.IsNullOrWhiteSpace(InfoJSON) ? JsonConvert.DeserializeObject<InfoParameter>(InfoJSON) : null;

        public void RegisterAction(string actionUUID, string actionContext)
        {
            var action = _availableActions.SingleOrDefault(a => a.UUID == actionUUID);
            if (action != null) 
            {
                _actions.Add(actionContext, action);
            }
        }

        public void UnregisterAction(string actionContext)
        {
            _actions.Remove(actionContext);
        }

        private void OnKeyDown(ReceivedPayload payload)
        {
            var action = this._actions[payload.Context];
            action.OnKeyDown(new KeyDownEventArgs());
        }

        private void OnKeyUp(ReceivedPayload payload)
        {
            var action = this._actions[payload.Context];
            action.OnKeyUp(new KeyUpEventArgs());
        }

        public async Task OnExecuteAsync(CancellationToken cancellationToken)
        {

#if DEBUG
            if (Info != null &&
                Info.Devices.Any(d => d.Type != StreamDeckType.StreamDeckTesting))
            {
                Debugger.Launch();

                while (!Debugger.IsAttached)
                {
                    await Task.Delay(300);
                }
            }
#endif

            try
            {
                await _socket.ConnectAsync(new Uri($"ws://localhost:{Port}"), cancellationToken);
                await _socket.SendAsync(GetPluginRegistrationBytes(), WebSocketMessageType.Text, true, cancellationToken);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            while (!cancellationToken.IsCancellationRequested && _socket.IsAvailable())
            {
                var buffer = new byte[65536];
                var segment = new ArraySegment<byte>(buffer, 0, buffer.Length);

                await _socket.ReceiveAsync(segment, cancellationToken);

                var receivedPayloadJSON = Encoding.UTF8.GetString(buffer).TrimEnd('\0');

                if (!string.IsNullOrEmpty(receivedPayloadJSON) && !receivedPayloadJSON.StartsWith("\0"))
                {
                    var receivedPayload = JsonConvert.DeserializeObject<ReceivedPayload>(receivedPayloadJSON);
                    switch (receivedPayload.Event)
                    {
                        case ReceivedEventType.keyDown:
                            this.OnKeyDown(receivedPayload);
                            break;
                        case ReceivedEventType.keyUp:
                            this.OnKeyUp(receivedPayload);
                            break;
                        case ReceivedEventType.willAppear:
                            this.RegisterAction(receivedPayload.Action, receivedPayload.Context);
                            break;
                        case ReceivedEventType.willDisappear:
                            this.UnregisterAction(receivedPayload.Context);
                            break;
                        default:
                            break;
                    }
                }
            }

            _socket.Dispose();

            ArraySegment<byte> GetPluginRegistrationBytes()
            {
                var registration = new
                {
                    @event = Event,
                    uuid = UUID
                };

                var outString = JsonConvert.SerializeObject(registration);
                var outBytes = Encoding.UTF8.GetBytes(outString);
                return new ArraySegment<byte>(outBytes);
            }
        }
    }
}
