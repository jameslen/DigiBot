using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot
{
    [Singleton]
    public abstract class IMiddleware
    {
        public IMiddleware(IContainer services)
        {
            Services = services;
        }

        protected IContainer Services { get; }

        public abstract Task Invoke(IBotMessage message, MessageDelegate next);
    }
}
