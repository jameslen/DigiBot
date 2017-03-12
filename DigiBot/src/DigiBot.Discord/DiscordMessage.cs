using DigiBot.Discord.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.Discord
{
    public class DiscordMessage : IBotMessage
    {
        public string Message => Content;
        public string Source => "Discord";
        public IUser User => Server.GetUser(Author.Id);
        public IChannel Channel => DiscordChannel;
        public IServer Server => DiscordChannel.Server;


        public string Id { get; set; }
        public string Channel_Id { get; set; }
        public DiscordChannel DiscordChannel { get; set; }
        public DiscordUser Author { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime Edit_Timestamp { get; set; }
        [JsonProperty(PropertyName = "tts")]
        public bool Text_To_Speech { get; set; }
        public bool Mention_Everyone { get; set; }
        public List<DiscordUser> Mentions { get; set; }
        public List<string> Mention_Roles { get; set; }
        public List<object> Attachments { get; set; }
        public List<object> Embeds { get; set; }
        public List<Reaction> Reactions { get; set; }
        public string Nonce { get; set; }
        public bool Pinned { get; set; }
        public string Webhook_Id { get; set; }

        public class Reaction
        {
        }
    }
}
