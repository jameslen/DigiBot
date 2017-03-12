namespace DigiBot.Discord
{
    public class DiscordInitializer
    {
        public class GatewayInfo
        {
            public string  Address { get; set; }
            public int CurrentShard { get; set; } = 0;
            public int TotalShards { get; set; } = 1;
        }

        public string Token { get; set; }
        public string Prefix { get; set; }
        public GatewayInfo Gateway { get; set; }
    }
}