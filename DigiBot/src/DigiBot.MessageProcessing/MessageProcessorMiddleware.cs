using StructureMap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigiBot
{
    public class MessageProcessorMiddleware : IMiddleware
    {
        private Dictionary<string, IContainer> _serverScopes = new Dictionary<string, IContainer>();

        public MessageProcessorMiddleware(IContainer botServices) : base(botServices)
        {
            
        }

        public override async Task Invoke(IBotMessage message, MessageDelegate next)
        {
            if (!_serverScopes.ContainsKey(message.Server.ID))
            {
                _serverScopes.Add(message.Server.ID, Services.CreateChildContainer());
            }

            var services = _serverScopes[message.Server.ID];

            var processors = services.GetAllInstances<IMessageProcessor>();

            foreach (var processor in processors)
            {
                processor.ProcessMessage(message);
            }

            await next(message);
        }
    }
}