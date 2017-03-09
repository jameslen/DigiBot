using StructureMap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigiBot
{
    public class MessageProcessorMiddleware : IMiddleware
    {
        private Dictionary<string, IEnumerable<IMessageProcessor>> _serverScopes = new Dictionary<string, IEnumerable<IMessageProcessor>>();

        public MessageProcessorMiddleware(IContainer botServices) : base(botServices)
        {
            
        }

        public override async Task Invoke(IBotMessage message, MessageDelegate next)
        {
            var services = message.Server.Scope;

            var processors = services.GetAllInstances<IMessageProcessor>();

            foreach (var processor in processors)
            {
                processor.ProcessMessage(message);
            }

            await next(message);
        }
    }
}