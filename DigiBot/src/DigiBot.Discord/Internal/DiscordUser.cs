using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Discord.Internal
{
    public class DiscordUser
    {
        public string Username { get; set; }
        public string Id { get; set; }
        public string Discriminator { get; set; }
        public string Avatar { get; set; }
        public bool Bot { get; set; }
        public bool MFA_Enabled { get; set; }
        public bool Verified { get; set; }
        public string Email { get; set; }

        public List<DiscordServer> Guilds { get; set; }

        // TODO: Add events to users: join/leave/presence change/message sent
    }
}
