using Microsoft.Extensions.DependencyInjection;
using StreamDeck.SDK.Abstractions;
using System;

namespace StreamDeck.SDK
{
    public static class StreamDeckHostBuilderExtensions
    {
        public static IStreamDeckHostBuilder UseStartup<TStartup>(this IStreamDeckHostBuilder builder) where TStartup : class
        {
            var startup = Activator.CreateInstance<TStartup>();
            var methodInfo = typeof(TStartup).GetMethod("ConfigureServices");

            var deligate = Delegate.CreateDelegate(typeof(Action<TStartup, IServiceCollection>), methodInfo);
            void caller(TStartup startup, IServiceCollection param) => deligate.DynamicInvoke(startup, param);

            builder.ConfigureServices((IServiceCollection services) => caller(startup, services));

            return builder;
        }
    }
}
