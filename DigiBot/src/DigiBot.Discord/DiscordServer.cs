using DigiBot.Discord.Utilities;
using Newtonsoft.Json;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot.Discord
{
    public enum Permissions
    {
        CREATE_INSTANT_INVITE = 0x00000001,
        KICK_MEMBERS = 0x00000002,
        BAN_MEMBERS = 0x00000004,
        ADMINISTRATOR = 0x00000008,
        MANAGE_CHANNELS = 0x00000010,
        MANAGE_GUILD = 0x00000020,
        ADD_REACTIONS = 0x00000040,
        READ_MESSAGES = 0x00000400,
        SEND_MESSAGES = 0x00000800,
        SEND_TTS_MESSAGES = 0x00001000,
        MANAGE_MESSAGES = 0x00002000,
        EMBED_LINKS = 0x00004000,
        ATTACH_FILES = 0x00008000,
        READ_MESSAGE_HISTORY = 0x00010000,
        MENTION_EVERYONE = 0x00020000,
        USE_EXTERNAL_EMOJIS = 0x00040000,
        CONNECT = 0x00100000,
        SPEAK = 0x00200000,
        MUTE_MEMBERS = 0x00400000,
        DEAFEN_MEMBERS = 0x00800000,
        MOVE_MEMBERS = 0x01000000,
        USE_VAD = 0x02000000,
        CHANGE_NICKNAME = 0x04000000,
        MANAGE_NICKNAMES = 0x08000000,
        MANAGE_ROLES = 0x10000000,
        MANAGE_WEBHOOKS = 0x20000000,
        MANAGE_EMOJIS = 0x40000000
    }

    public class DiscordServer : IServer
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Splash { get; set; }
        public string Owner_Id { get; set; }
        public string Region { get; set; }
        public bool EmbedEnabled { get; set; }
        public int Multifactor_Auth_Level { get; set; }
        public DateTime Joined_At { get; set; }
        public bool Large { get; set; }
        public bool? Unavailable { get; set; }
        public int Member_Count { get; set; }
        public int AFK_Timeout { get; set; }
        public string AFK_Channel_Id { get; set; }

        public List<string> Features { get; set; }
        [JsonConverter(typeof(JsonListToDictionaryByUserId<Presence>))]
        public Dictionary<string, Presence> Presences { get; set; }
        [JsonConverter(typeof(JsonListToDictionaryById<Role>))]
        public Dictionary<string, Role> Roles { get; set; }
        [JsonConverter(typeof(JsonListToDictionaryById<Emoji>))]
        public Dictionary<string, Emoji> Emojis { get; set; }
        [JsonConverter(typeof(JsonListToDictionaryByUserId<DiscordServerUser>))]
        public Dictionary<string, DiscordServerUser> Members { get; set; }
        [JsonConverter(typeof(JsonListToDictionaryById<DiscordChannel>))]
        [JsonProperty(PropertyName = "Channels")]
        public Dictionary<string, DiscordChannel> DiscordChannels { get; set; }

        public DiscordServer()
        {
            
        }

        [JsonIgnore]
        public IEnumerable<IChannel> Channels
        {
            get
            {
                return DiscordChannels.Values.Select(c => c as IChannel);
            }
        }

        [JsonIgnore]
        public IEnumerable<IUser> Users
        {
            get
            {
                return Members.Values.Select(u => u as IUser);
            }
        }

        internal void AddUser(DiscordServerUser member)
        {
            Members.Add(member.Id, member);
        }

        public IChannel GetChannel(string channelId)
        {
            return DiscordChannels[channelId];
        }

        public IUser GetUser(string userId)
        {
            return Members[userId];
        }

        public DiscordServerUser GetServerUser(string userId)
        {
            return Members[userId];
        }

        public void UpdateAllUserPermissions()
        {
            foreach (var member in Members.Values)
            {
                UpdateUserPermission(member);
            }
        }

        public void UpdateUserPermission(DiscordServerUser member)
        {
            member.UserPermissions = 0;
            foreach (var role in member.UserRoles)
            {
                member.UserPermissions |= Roles[role].Permissions;
            }
        }

        //TODO: Crud Operations

    }
}
