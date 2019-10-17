using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using StreamDeck.SDK.Abstractions;
using System;

namespace StreamDeck.SDK
{
    public class StreamDeckHostBuilder : IStreamDeckHostBuilder
    {
        private readonly string[] args;
        private readonly CommandLineApplication<StreamDeckApp> app;
        private readonly IServiceCollection services;

        public StreamDeckHostBuilder(string[] args)
        {
            this.args = args;
            this.app = new CommandLineApplication<StreamDeckApp>();
            this.services = new ServiceCollection();
        }

        void IStreamDeckHostBuilder.ConfigureServices(Action<IServiceCollection> configureServices)
        {
            configureServices(this.services);
        }

        public IStreamDeckHost Build()
        {
            var serviceProvider = this.services.BuildServiceProvider();

            this.app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(serviceProvider);

            return new StreamDeckHost<StreamDeckApp>(this.app, this.args);
        }

        IStreamDeckHost IStreamDeckHostBuilder.Build() => this.Build();
    }
}
