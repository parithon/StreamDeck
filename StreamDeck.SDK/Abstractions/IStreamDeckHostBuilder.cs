using Microsoft.Extensions.DependencyInjection;
using System;

namespace StreamDeck.SDK.Abstractions
{
    public interface IStreamDeckHostBuilder
    {
        internal void ConfigureServices(Action<IServiceCollection> configureServices);
        IStreamDeckHost Build();
    }
}
