using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
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
    internal class StreamDeckApp : IDisposable
    {
        private readonly bool _isDebugging;
        private readonly ClientWebSocket _socket = new ClientWebSocket();
        private readonly Dictionary<string, IStreamDeckAction> _actions = new Dictionary<string, IStreamDeckAction>();
        private readonly IEnumerable<IStreamDeckAction> _availableActions =
            Assembly.GetEntryAssembly().GetTypes()
                .Where(t => t.GetInterface(nameof(IStreamDeckAction)) != null)
                .Select(type => (IStreamDeckAction)Activator.CreateInstance(type));

        public StreamDeckApp(IConfiguration configuration)
        {
            Configuration = configuration;
            _isDebugging = configuration.GetValue<bool>("EnableDebugger");
        }

        private IConfiguration Configuration { get; }

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

        private void SetSettings(ReceivedPayload payload, string key, object value)
        {
            var valueJSON = JsonConvert.SerializeObject(value);
            if (payload.Payload.Settings.ContainsKey(key))
            {
                payload.Payload.Settings[key] = valueJSON;
            }
            else
            {
                payload.Payload.Settings.Add(key, valueJSON);
            }
            var json = JsonConvert.SerializeObject(new
            {
                @event = "setSettings",
                context = payload.Context,
                payload = payload.Payload.Settings
            });

            _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void SetTitle(ReceivedPayload payload, string title)
        {
            var json = JsonConvert.SerializeObject(new
            {
                @event = "setTitle",
                context = payload.Context,
                payload = new
                {
                    title,
                    target = 0
                }
            });

            _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void OnWillAppear(ReceivedPayload payload)
        {
            this.RegisterAction(payload.Action, payload.Context);
            if (this._actions.ContainsKey(payload.Context))
            {
                var action = this._actions[payload.Context];
                action.OnWillAppear(new SettingsEventArgs
                {
                    Settings = payload.Payload.Settings,
                    SetSettings = (string key, object value) => this.SetSettings(payload, key, value),
                    SetTitle = (string title) => this.SetTitle(payload, title)
                });
            }
        }

        private void OnWillDisappear(ReceivedPayload payload)
        {
            if (this._actions.ContainsKey(payload.Context))
            {
                var action = this._actions[payload.Context];
                action.OnWillDisappear(new SettingsEventArgs
                {
                    Settings = payload.Payload.Settings,
                    SetSettings = (string key, object value) => this.SetSettings(payload, key, value),
                    SetTitle = (string title) => this.SetTitle(payload, title)
                });
            }
            this.UnregisterAction(payload.Context);
        }

        private void OnKeyDown(ReceivedPayload payload)
        {
            if (this._actions.ContainsKey(payload.Context))
            {
                var action = this._actions[payload.Context];
                action.OnKeyDown(new KeyEventArgs
                {
                    Column = payload.Payload.Coordinates.Column,
                    Device = payload.Device,
                    IsInMultiAction = payload.Payload.IsInMultiAction,
                    Row = payload.Payload.Coordinates.Row,
                    Settings = payload.Payload.Settings,
                    State = payload.Payload.State,
                    UserDesiredState = payload.Payload.UserDesiredState,
                    SetSettings = (string key, object value) => this.SetSettings(payload, key, value),
                    SetTitle = (string title) => this.SetTitle(payload, title)
                });
            }
        }

        private void OnKeyUp(ReceivedPayload payload)
        {
            if (this._actions.ContainsKey(payload.Context))
            {
                var action = this._actions[payload.Context];
                action.OnKeyUp(new KeyEventArgs
                {
                    Column = payload.Payload.Coordinates.Column,
                    Device = payload.Device,
                    IsInMultiAction = payload.Payload.IsInMultiAction,
                    Row = payload.Payload.Coordinates.Row,
                    Settings = payload.Payload.Settings,
                    State = payload.Payload.State,
                    UserDesiredState = payload.Payload.UserDesiredState,
                    SetSettings = (string key, object value) => this.SetSettings(payload, key, value),
                    SetTitle = (string title) => this.SetTitle(payload, title)
                });
            }
        }

        public async Task OnExecuteAsync(CancellationToken cancellationToken)
        {

#if DEBUG
            if (_isDebugging &&
                Info != null &&
                Info.Devices.Any(d => d.Type != StreamDeckType.StreamDeckTesting))
            {
                Debugger.Launch();

                while (!cancellationToken.IsCancellationRequested && !Debugger.IsAttached)
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
                            this.OnWillAppear(receivedPayload);
                            break;
                        case ReceivedEventType.willDisappear:
                            this.OnWillDisappear(receivedPayload);
                            break;
                        default:
                            break;
                    }
                }
            }

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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _socket.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StreamDeckApp()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
