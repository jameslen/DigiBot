using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot
{
    public static class CommandProcessorMiddlewareExtensions
    {
        public static void AddCommandProcessing(this Registry services)
        {
            services.Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scanner.TheCallingAssembly();

                scanner.AddAllTypesOf<ICommandProcessor>();
            });
            services.For<ICommandProcessor>().ContainerScoped();
        }
    }
}
