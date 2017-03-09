using StructureMap;
using System.Threading.Tasks;
using System;

namespace DigiBot
{
    // Gives the Source access to the rest of the bot application
    public class BotHostApplication : IBotHostApplication
    {
        private readonly IMessageFactory _messageFactory;
        private readonly MessageDelegate _botMain;
        private IContainer _services;

        public BotHostApplication(MessageDelegate botMain, IContainer services, IMessageFactory messageFactory)
        {
            _botMain = botMain;
            _messageFactory = messageFactory;
            _services = services;
        }

        public Task ProcessMessage(IBotMessage message)
        {
            return _botMain(message);
        }

        public IContainer GetNewScope(string id)
        {
            return _services.GetNestedContainer(id);
        }
    }
}