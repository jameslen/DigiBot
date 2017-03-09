using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot
{
    public static class MessageProcessorMiddlewareExtensions
    {
        public static void AddMessageProcessing(this Registry services)
        {
            services.Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scanner.TheCallingAssembly();

                scanner.AddAllTypesOf<IMessageProcessor>();
            });
            services.For<IMessageProcessor>().ContainerScoped();
        }
    }
}
