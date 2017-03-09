using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot
{
    public class BotAppBuilderFactory : IBotAppBuilderFactory
    {
        private readonly IContainer _botService;

        public BotAppBuilderFactory()
        {
        }

        public IBotAppBuilder Create(IContainer services)
        {
            return new BotAppBuilder(services);
        }
    }
}
