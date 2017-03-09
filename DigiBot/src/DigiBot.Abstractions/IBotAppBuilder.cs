using StructureMap;
using System;
using System.Threading.Tasks;

namespace DigiBot
{
    public delegate Task MessageDelegate(IBotMessage message);

    public interface IBotAppBuilder
    {
        IContainer Services { get; set; }

        IBotAppBuilder Use(Func<MessageDelegate, MessageDelegate> middleware);
        MessageDelegate Build();
    }
}