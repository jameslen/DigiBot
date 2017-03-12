using DigiBot.Discord.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Discord
{
    public class Emoji
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
        public bool Require_Colons { get; set; }
        public bool Managed { get; set; }
    }

    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Permissions { get; set; }
        public bool Mentionable { get; set; }
        public bool Hoist { get; set; }
        public bool Managed { get; set; }
        public int Color { get; set; }
        public int Position { get; set; }
    }

    public class Presence
    {
        public DiscordUser User { get; set; }
        public string Status { get; set; }
        public Game Game { get; set; }
    }

    public class Game
    {
        public string Url { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
    }
}
