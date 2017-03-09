using DigiDiscord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.DiscordMiddleware
{
    public class DiscordUser : IUser
    {
        public string Id => BaseUser.User.Id;

        public string Name => BaseUser.Nick ?? BaseUser.User.Username;

        public DiscordUser(GuildMember m)
        {
            BaseUser = m;
        }

        public IEnumerable<string> Roles
        {
            get
            {
                return BaseUser.Guild.Roles.Where(r => BaseUser.Roles.Contains(r.Key)).Select(v => v.Value.Name);
            }
        }

        public bool IsAdmin 
        {
            get
            {
                return (BaseUser.Permissions & (int)Permissions.ADMINISTRATOR) != 0;
            }
        }

        public bool IsBot => BaseUser.User.Bot;

        public GuildMember BaseUser { get; }
    }
}
