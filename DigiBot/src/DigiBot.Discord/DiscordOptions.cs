using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot.Discord
{
    public class DiscordOptions
    {
        public string Token { get; set; }
        public string Gateway { get; set; } = null;
        public int TotalShards { get; set; } = 1;
        public int CurrentShard { get; set; } = 0;
    }
}
