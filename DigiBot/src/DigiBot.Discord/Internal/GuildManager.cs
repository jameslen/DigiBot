using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.Discord.Internal
{
    public enum Events
    {
        READY,
        RESUMED,
        CHANNEL_CREATE,
        CHANNEL_UPDATE,
        CHANNEL_DELETE,
        GUILD_CREATE,
        GUILD_UPDATE,
        GUILD_DELETE,
        GUILD_BAN_ADD,
        GUILD_BAN_REMOVE,
        GUILD_EMOJIS_UPDATE,
        GUILD_INTEGRATIONS_UPDATE,
        GUILD_MEMBER_ADD,
        GUILD_MEMBER_REMOVE,
        GUILD_MEMBER_UPDATE,
        GUILD_MEMBERS_CHUNK,
        GUILD_ROLE_CREATE,
        GUILD_ROLE_UPDATE,
        GUILD_ROLE_DELETE,
        MESSAGE_CREATE,
        MESSAGE_UPDATE,
        MESSAGE_DELETE,
        MESSAGE_DELETE_BULK,
        PRESENCE_UPDATE,
        TYPING_START,
        USER_SETTINGS_UPDATE,
        VOICE_STATE_UPDATE,
        VOICE_SERVER_UPDATE
    }

    public class GuildManager
    {
        private Dictionary<string, DiscordServer> Guilds = new Dictionary<string, DiscordServer>();
        private Dictionary<string, DiscordChannel> Channels = new Dictionary<string, DiscordChannel>();
        private Dictionary<string, DiscordUser> Users = new Dictionary<string, DiscordUser>();
        private GatewayManager _Gateway;
        private ILogger _Logger;
        private DiscordHttpClient _Http;

        public GuildManager(DiscordHttpClient http, ILogger logger)
        {
            _Logger = logger;
            _Http = http;
        }

        public DiscordServer GetGuild(string id)
        {
            return Guilds[id];
        }

        public delegate void GuildUpdate(DiscordServer g);
        public event GuildUpdate GuildCreated;
        public event GuildUpdate GuildUpdated;
        public event GuildUpdate GuildDeleted;

        public delegate void GuildChannelEventHandler(DiscordServer guild, DiscordChannel channel);
        public event GuildChannelEventHandler ChannelCreate;
        public event GuildChannelEventHandler ChannelUpdate;
        public event GuildChannelEventHandler ChannelDelete;

        public delegate void GuildBanUpdate(DiscordServer guild, DiscordServerUser user);
        public event GuildBanUpdate BanAdd;
        public event GuildBanUpdate BanRemove;

        //public delegate void GuildEmojiUpdate(DiscordServer guild, List<DiscordServer.Emoji> emojis);
        //public event GuildEmojiUpdate EmojiUpdate;

        public delegate void GuildMemberUpdate(DiscordServer guild, DiscordServerUser member);
        public event GuildMemberUpdate MemberAdd;
        public event GuildMemberUpdate MemberRemove;
        public event GuildMemberUpdate MemberUpdate;

        public delegate void GuildMemberUpdateChunk(DiscordServer guild, List<DiscordServerUser> members);
        public event GuildMemberUpdateChunk MemberChunkUpdate;

        //public delegate void GuildRoleUpdate(DiscordServer guild, DiscordServer.Role role);
        //public event GuildRoleUpdate RoleCreate;
        //public event GuildRoleUpdate RoleUpdate;
        //public event GuildRoleUpdate RoleDelete;

        public delegate void GuildChannelMessageUpdate(DiscordChannel channel, DiscordMessage message);
        public event GuildChannelMessageUpdate MessageCreate;
        public event GuildChannelMessageUpdate MessageUpdate;
        public event GuildChannelMessageUpdate MessageDelete;

        public delegate void GuildChannelMessageDeleteBulk(DiscordChannel channel, List<DiscordMessage> messages);
        public event GuildChannelMessageDeleteBulk MessagesBulkDelete;

        public delegate void MemberPresenceUpdate(DiscordServer guild, Presence presence);
        public event MemberPresenceUpdate PresenceUpdate;

        //TODO: Typing Start
        //TODO: Integrations
        //TODO: User Updates
        //TODO: Voice Chat

        public void GatewayMessageHandler(string eventName, string payload)
        {
            _Logger?.Debug($"{eventName} - {payload}");

            var eventValue = (Events)Enum.Parse(typeof(Events), eventName);

            var eventPayload = JObject.Parse(payload);

            switch (eventValue)
            {
                case Events.READY:
                    {
                        var guilds = eventPayload["guilds"] as JArray;

                        foreach (var guild in guilds)
                        {
                            var g = new DiscordServer(null) { ID = guild["id"].ToString(), Unavailable = guild["unavailable"].ToObject<bool>() };

                            Guilds.Add(g.ID, g);
                        }

                        break;
                    }
                case Events.GUILD_CREATE:
                    {
                        DiscordServer g = null;
                        if (Guilds.ContainsKey(eventPayload["id"].ToString()))
                        {
                            g = Guilds[eventPayload["id"].ToString()];
                        }
                        else
                        {
                            g = new DiscordServer(null) { ID = eventPayload["id"].ToString() };
                            Guilds.Add(g.ID, g);
                        }

                        JsonConvert.PopulateObject(payload, g);

                        foreach(var channel in g.DiscordChannels)
                        {
                            if(!Channels.ContainsKey(channel.Key))
                            {
                                Channels.Add(channel.Key, channel.Value);
                            }

                            channel.Value.Guild_Id = g.ID;
                            channel.Value.Server = g;
                            channel.Value.Http = _Http;
                        }

                        foreach(var member in g.Members)
                        {
                            if(!Users.ContainsKey(member.Key))
                            {
                                Users.Add(member.Key, member.Value.User);
                            }
                            else if(member.Value.User != Users[member.Key])
                            {
                                member.Value.User = Users[member.Key];
                            }

                            member.Value.Server = g;
                        }

                        foreach(var presence in g.Presences.Values)
                        {
                            presence.User = Users[presence.User.Id];
                        }

                        g.UpdateAllUserPermissions();

                        GuildCreated?.Invoke(g);
                        break;
                    }
                case Events.GUILD_UPDATE:
                    {
                        var g = Guilds[eventPayload["id"].ToString()];

                        // TODO: Double check that this is the correct behavior.
                        JsonConvert.PopulateObject(payload, g);

                        GuildUpdated?.Invoke(g);
                        break;
                    }
                case Events.GUILD_DELETE:
                    {
                        var g = Guilds[eventPayload["id"].ToString()];
                        Guilds.Remove(eventPayload["id"].ToString());

                        GuildDeleted?.Invoke(g);
                        break;
                    }
                //case Events.CHANNEL_CREATE:
                //    {
                //        var c = eventPayload.ToObject<GuildChannel>();
                //        var g = Guilds[c.Guild_Id];
                //
                //        c.Guild = g;
                //        g.Channels.Add(c.Id, c);
                //        Channels.Add(c.Id, c);
                //
                //        ChannelCreate?.Invoke(g, c);
                //        break;
                //    }
                //case Events.CHANNEL_UPDATE:
                //    {
                //        var g = Guilds[eventPayload["guild_id"].ToString()];
                //        var c = g.Channels[eventPayload["id"].ToString()];
                //
                //        JsonConvert.PopulateObject(payload, c);
                //
                //        ChannelUpdate?.Invoke(g, c);
                //        break;
                //    }
                //case Events.CHANNEL_DELETE:
                //    {
                //        var g = Guilds[eventPayload["guild_id"].ToString()];
                //        var c = g.Channels[eventPayload["id"].ToString()];
                //
                //        g.Channels.Remove(c.Id);
                //        Channels.Remove(c.Id);
                //
                //        ChannelDelete?.Invoke(g, c);
                //        break;
                //    }
                //case Events.GUILD_BAN_ADD:
                //    {
                //        // TODO: User knows about bans
                //        var g = Guilds[eventPayload["guild_id"].ToString()];
                //        var m = g.Members[eventPayload["id"].ToString()];
                //
                //        Users[m.User.Id].Guilds.Remove(g);
                //        g.Members.Remove(m.User.Id);
                //
                //        BanAdd?.Invoke(g, m.User);
                //        break;
                //    }
                //case Events.GUILD_BAN_REMOVE:
                //    {
                //        var g = Guilds[eventPayload["guild_id"].ToString()];
                //
                //        BanRemove?.Invoke(g, eventPayload.ToObject<DiscordUser>());
                //        break;
                //    }
                //case Events.GUILD_EMOJIS_UPDATE:
                //    {
                //        var g = Guilds[eventPayload["guild_id"].ToString()];
                //
                //        g.Emojis.Clear();
                //
                //        JsonConvert.PopulateObject(payload, g);
                //
                //        EmojiUpdate?.Invoke(g, eventPayload["emojis"].ToObject<List<Guild.Emoji>>());
                //        break;
                //    }
                case Events.GUILD_MEMBER_ADD:
                    {
                        var g = Guilds[eventPayload["guild_id"].ToString()];
                        var m = eventPayload.ToObject<DiscordServerUser>();

                        if (!Users.ContainsKey(m.User.Id))
                        {
                            Users.Add(m.User.Id, m.User);
                        }
                        else
                        {
                            m.User = Users[m.User.Id];
                        }

                        g.Members.Add(m.User.Id, m);
                        m.User.Guilds.Add(g);

                        m.Server = g;
                        //g.UpdateUserPermission(m);

                        MemberAdd?.Invoke(g, m);
                        break;
                    }
                case Events.GUILD_MEMBER_REMOVE:
                    {
                        var g = Guilds[eventPayload["guild_id"].ToString()];
                        var m = g.Members[eventPayload["user"]["id"].ToString()];

                        g.Members.Remove(m.User.Id);
                        m.User.Guilds.Remove(g);

                        MemberRemove?.Invoke(g, m);
                        break;
                    }
                case Events.GUILD_MEMBER_UPDATE:
                    {
                        var g = Guilds[eventPayload["guild_id"].ToString()];
                        var m = g.Members[eventPayload["user"]["id"].ToString()];

                        JsonConvert.PopulateObject(payload, m);

                        //g.UpdateUserPermission(m);

                        MemberUpdate?.Invoke(g, m);
                        break;
                    }
                case Events.GUILD_MEMBERS_CHUNK:
                    {
                        var g = Guilds[eventPayload["guild_id"].ToString()];
                        var members = eventPayload["members"].ToObject<List<DiscordServerUser>>();

                        foreach(var member in members)
                        {
                            if(Users.ContainsKey(member.User.Id))
                            {
                                member.User = Users[member.User.Id];
                            }
                            else
                            {
                                Users.Add(member.User.Id, member.User);
                            }

                            g.Members.Remove(member.User.Id);
                            g.Members.Add(member.User.Id, member);
                        }
                        
                        MemberChunkUpdate?.Invoke(g, members);
                        break;
                    }
                //case Events.GUILD_ROLE_CREATE:
                //    {
                //        var g = Guilds[eventPayload["guild_id"].ToString()];
                //        var r = eventPayload["role"].ToObject<Guild.Role>();
                //
                //        g.Roles.Add(r.Id, r);
                //
                //        RoleCreate?.Invoke(g, r);
                //        break;
                //    }
                //case Events.GUILD_ROLE_UPDATE:
                //    {
                //        var g = Guilds[eventPayload["guild_id"].ToString()];
                //        var r = g.Roles[eventPayload["role"]["id"].ToString()];
                //
                //        JsonConvert.PopulateObject(eventPayload["role"].ToString(), r);
                //
                //        RoleUpdate?.Invoke(g, r);
                //        break;
                //    }
                //case Events.GUILD_ROLE_DELETE:
                //    {
                //        var g = Guilds[eventPayload["guild_id"].ToString()];
                //        var r = g.Roles[eventPayload["role_id"].ToString()];
                //
                //        g.Roles.Remove(r.Id);
                //
                //        RoleDelete?.Invoke(g, r);
                //        break;
                //    }
                case Events.MESSAGE_CREATE:
                    {
                        var message = eventPayload.ToObject<DiscordMessage>();
                        var c = Channels[message.Channel_Id];

                        message.DiscordChannel = c;

                        if(string.IsNullOrEmpty(message.Webhook_Id))
                        {
                            var server = c.Server;
                            if(Users.ContainsKey(message.Author.Id))
                            {

                                message.Author = Users[message.Author.Id];
                            }
                        }

                        MessageCreate?.Invoke(c, message);
                        break;
                    }
                case Events.MESSAGE_UPDATE:
                    {
                        var message = eventPayload.ToObject<DiscordMessage>();
                        var c = Channels[message.Channel_Id];

                        MessageUpdate?.Invoke(c, message);
                        break;
                    }
                case Events.MESSAGE_DELETE:
                    {
                        var message = eventPayload.ToObject<DiscordMessage>();
                        var c = Channels[message.Channel_Id];

                        MessageDelete?.Invoke(c, message);
                        break;
                    }
                case Events.MESSAGE_DELETE_BULK:
                    {
                        var messages = eventPayload.ToObject<List<DiscordMessage>>();
                        var c = Channels[eventPayload["channel_id"].ToString()];

                        MessagesBulkDelete?.Invoke(c, messages);
                        break;
                    }
                case Events.PRESENCE_UPDATE:
                    {
                        var presense = eventPayload.ToObject<Presence>();
                        var g = Guilds[eventPayload["guild_id"].ToString()];

                        if(g.Presences.ContainsKey(presense.User.Id))
                        {
                            var p = g.Presences[presense.User.Id];

                            p.Game = presense.Game;
                            p.Status = presense.Status;

                            presense = p;
                        }
                        else
                        {
                            presense.User = Users[presense.User.Id];
                            g.Presences.Add(presense.User.Id, presense);
                        }

                        PresenceUpdate?.Invoke(g, presense);
                        break;
                    }
            }
        }
    }
}
