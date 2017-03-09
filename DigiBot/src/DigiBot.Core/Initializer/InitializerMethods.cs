using System;
using StructureMap;

namespace DigiBot
{
    internal class InitializerMethods
    {
        public Action<IBotAppBuilder> ConfigureDelegate { get; }
        public Func<Registry, IContainer> ConfigureServicesDelegate { get; }

        public InitializerMethods(Action<IBotAppBuilder> configureMethod, Func<Registry, IContainer> configureServices)
        {
            ConfigureDelegate = configureMethod;
            ConfigureServicesDelegate = configureServices;
        }
    }
}