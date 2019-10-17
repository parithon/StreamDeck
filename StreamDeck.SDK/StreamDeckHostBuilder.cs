using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StreamDeck.SDK.Abstractions;
using System;
using System.IO;

namespace StreamDeck.SDK
{
    public class StreamDeckHostBuilder : IStreamDeckHostBuilder
    {
        private readonly string[] _args;
        private readonly CommandLineApplication<StreamDeckApp> _app;
        private readonly IServiceCollection _services;

        public StreamDeckHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            this._args = args;
            this._app = new CommandLineApplication<StreamDeckApp>();
            this._services = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration);
        }

        void IStreamDeckHostBuilder.ConfigureServices(Action<IServiceCollection> configureServices)
        {
            configureServices(this._services);
        }

        public IStreamDeckHost Build()
        {
            var serviceProvider = this._services.BuildServiceProvider();

            this._app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(serviceProvider);

            return new StreamDeckHost<StreamDeckApp>(this._app, this._args);
        }

        IStreamDeckHost IStreamDeckHostBuilder.Build() => this.Build();
    }
}
