using StructureMap;
using System.Collections.Generic;

namespace DigiBot
{
    public interface IServer
    {
        IEnumerable<IChannel> Channels { get; }
        IEnumerable<IUser> Users { get; }

        string ID { get; }

        IUser GetUser(string userId);
        IChannel GetChannel(string channelId);
    }
}