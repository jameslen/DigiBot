using System;
using StructureMap;

namespace DigiBot
{
    internal class ConventionBasedInitializer : IInitializer
    {
        private readonly InitializerMethods _methods;

        public ConventionBasedInitializer(InitializerMethods methods)
        {
            _methods = methods;
        }

        public void Configure(IBotAppBuilder bot)
        {
            _methods.ConfigureDelegate(bot);
        }

        public IContainer ConfigureServices(Registry services)
        {
            return _methods.ConfigureServicesDelegate(services);
        }
    }
}