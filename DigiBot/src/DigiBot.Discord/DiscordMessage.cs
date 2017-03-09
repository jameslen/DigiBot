using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.DiscordMiddleware
{
    public class DiscordMessage : IBotMessage
    {
        public string Message => SourceMessage.Content;

        public string Source => "Discord";

        public IUser User { get; set; }
        public IChannel Channel { get; set; }
        public IServer Server { get; set; }

        public DigiDiscord.Message SourceMessage { get; set; }
    }
}
