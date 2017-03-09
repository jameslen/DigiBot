using DigiDiscord;
using System;

namespace DigiBot.DiscordMiddleware
{
    public class DiscordChannel : IChannel
    {
        public GuildChannel BaseChannel { get; }

        public DiscordChannel(GuildChannel c)
        {
            BaseChannel = c;
        }

        public void Send(string message)
        {
            BaseChannel.SendMessage(message);
        }
    }
}