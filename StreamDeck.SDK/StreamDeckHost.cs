using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using StreamDeck.SDK.Abstractions;
using System.IO;
using Newtonsoft.Json;
using StreamDeck.SDK.Models;
using System.Reflection;
using System.Linq;
using System;
using System.Text;

namespace StreamDeck.SDK
{
    internal class StreamDeckActionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IStreamDeckAction);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(StreamDeckAction));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    public static class StreamDeckHost
    {
        public static IStreamDeckHostBuilder CreateDefaultHost(string[] args)
        {
            return new StreamDeckHostBuilder(args);
        }

        public static async Task<int> GenerateManifest()
        {
            string manifestJSON;

            try
            {
                manifestJSON = await File.ReadAllTextAsync("manifest.json");
            }
            catch (FileLoadException ex)
            {
                return ex.HResult;
            }
            catch (FileNotFoundException ex)
            {
                return ex.HResult;
            }

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StreamDeckActionConverter());

            Manifest manifest;
            
            try
            {
                manifest = JsonConvert.DeserializeObject<Manifest>(manifestJSON, settings);
            }
            catch(JsonSerializationException ex) {
                return ex.HResult;
            }

            manifest.Actions.Clear();

            var actionTypes = Assembly.GetEntryAssembly().GetTypes().Where(type => type.GetInterface(nameof(IStreamDeckAction)) != null);

            foreach (var actionType in actionTypes)
            {
                var action = Activator.CreateInstance(actionType);
                manifest.Actions.Add(action as IStreamDeckAction);
            }

            try
            {
                manifestJSON = JsonConvert.SerializeObject(manifest);
            }
            catch (JsonSerializationException ex)
            {
                return ex.HResult;
            }

            await File.WriteAllTextAsync("manifest.json", manifestJSON, Encoding.UTF8);
            return 0;
        }
    }

    internal class StreamDeckHost<TApp> : IStreamDeckHost, IDisposable where TApp : class
    {
        private readonly CommandLineApplication<TApp> app;
        private readonly string[] args;

        public StreamDeckHost(CommandLineApplication<TApp> app, string[] args)
        {
            this.app = app;
            this.args = args;
        }

        ~StreamDeckHost()
        {
            this.Dispose(false);
        }

        public Task<int> RunAsync()
        {
            return this.app.ExecuteAsync(args);
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
                    this.app.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StreamDeckHost()
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
