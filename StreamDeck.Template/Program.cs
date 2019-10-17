using StreamDeck.SDK;
using StreamDeck.SDK.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StreamDeck.Template
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Any(arg => arg.ToLower() == "--generatemanifest"))
            {
                return await StreamDeckHost.GenerateManifest();
            }

            return await CreateStreamDeckHost(args)
                .Build()
                .RunAsync();
        }

        static IStreamDeckHostBuilder CreateStreamDeckHost(string[] args) =>
            StreamDeckHost.CreateDefaultHost(args)
                .UseStartup<Startup>();
    }
}
