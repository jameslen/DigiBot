using DigiBot.Discord.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.Discord
{
    public class DiscordServerUser : IUser
    {
        public string Id => User.Id;

        public string Name => Nick ?? User.Username;

        public DiscordServerUser()
        {
        }

        [JsonIgnore]
        public IEnumerable<string> Roles
        {
            get
            {
                return Server.Roles.Where(r => UserRoles.Contains(r.Key)).Select(v => v.Value.Name);
            }
        }

        public bool IsAdmin 
        {
            get
            {
                return (UserPermissions & (int)Permissions.ADMINISTRATOR) != 0;
            }
        }

        public bool IsBot => User.Bot;

        public DiscordUser User { get; set; }
        [JsonProperty(PropertyName = "Roles")]
        public List<string> UserRoles { get; set; }
        public bool Mute { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool Deaf { get; set; }
        public string Nick { get; set; }
        [JsonProperty(PropertyName = "Permissions")]
        public int UserPermissions { get; set; }
        public DiscordServer Server { get; set; }
    }
}
