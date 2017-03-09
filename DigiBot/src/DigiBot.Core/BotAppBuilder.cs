using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace DigiBot
{
    public class BotAppBuilder : IBotAppBuilder
    {
        private readonly IList<Func<MessageDelegate, MessageDelegate>> _components = new List<Func<MessageDelegate, MessageDelegate>>();

        private IContainer _botService;

        public BotAppBuilder(IContainer botService)
        {
            _botService = botService;
        }

        public IContainer MessagePipelineSources { get; set; }

        public IContainer Services
        {
            get
            {
                return _botService;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IBotAppBuilder Use(Func<MessageDelegate, MessageDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public MessageDelegate Build()
        {
            MessageDelegate app = message =>
            {
                return Task.CompletedTask;
            };

            foreach(var component in _components.Reverse())
            {
                app = component(app);
            }

            return app;
        }
    }
}
