using DigiDiscord;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot.DiscordMiddleware
{
    public class DiscordServer : IServer
    {
        private Guild _guild;

        private Dictionary<string, DiscordChannel> _channels = new Dictionary<string, DiscordChannel>();
        private Dictionary<string, DiscordUser> _users = new Dictionary<string, DiscordUser>();
        private IContainer _scope;

        public DiscordServer(Guild guild, IContainer scope)
        {
            _guild = guild;

            foreach(var channel in _guild.Channels)
            {
                _channels.Add(channel.Key, new DiscordChannel(channel.Value));
            }

            foreach(var member in _guild.Members)
            {
                _users.Add(member.Key, new DiscordUser(member.Value));
            }

            _scope = scope;
        }

        public IEnumerable<IChannel> Channels
        {
            get
            {
                return _channels.Values.Select(c => c as IChannel);
            }
        }

        public string ID
        {
            get
            {
                return _guild.Id;
            }
        }

        public IContainer Scope { get { return _scope; } }

        public IEnumerable<IUser> Users
        {
            get
            {
                return _users.Values.Select(u => u as IUser);
            }
        }

        internal void AddUser(GuildMember member)
        {
            _users.Add(member.User.Id, new DiscordUser(member));
        }

        public IChannel GetChannel(string channelId)
        {
            return _channels[channelId];
        }

        public IUser GetUser(string userId)
        {
            return _users[userId];
        }


        //TODO: Crud Operations

    }
}
