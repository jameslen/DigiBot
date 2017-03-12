using DigiBot.Discord.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DigiBot.Discord
{
    public class ConsoleLogger : ILogger
    {
        private LogLevel m_minLevel;

        public ConsoleLogger(LogLevel minLevel = LogLevel.Verbose)
        {
            m_minLevel = minLevel;
        }

        public override void Log(LogLevel level, string logLine)
        {
            if (level >= m_minLevel)
            {
                PrintLevel(level);
                PrintLine(logLine);
            }
        }

        private void PrintLevel(LogLevel level)
        {
            var bg = Console.BackgroundColor;
            var fg = Console.ForegroundColor;

            switch (level)
            {
                case LogLevel.Error:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogLevel.Verbose:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            Console.Write($"[{level}] ");

            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
        }

        private void PrintLine(string line)
        {
            Console.WriteLine(line);
        }
    }

    // TODO: Merge this with DigiDiscord into DigiBot.Discord
    public class DiscordSource : IBotSource, IDisposable
    {
        private IBotHostApplication _botHostApplication;

        private string _token;

        //private Dictionary<string, DiscordServer> Guilds = new Dictionary<string, DiscordServer>();
        //private Dictionary<string, DiscordMiddleware.DiscordUser> Users = new Dictionary<string, DiscordMiddleware.DiscordUser>();

        private DiscordHttpClient _client;
        private GatewayManager _gateway;
        private GuildManager _guilds;

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private string _gatewayAddress;
        private int _currentShard;
        private int _totalShards;

        public DiscordSource(DiscordOptions options)
        {
            _token = options.Token;
            _gatewayAddress = options.Gateway;
            _totalShards = options.TotalShards;
            _currentShard = options.CurrentShard;

            _client = new DiscordHttpClient(null);
            _client.AddBotAuth(_token);
        }

        public async Task Start(IBotHostApplication botHostApplication)
        {
            _botHostApplication = botHostApplication;

            if(_gatewayAddress == null)
            {
                _gatewayAddress = await GetGatewayAddress();
            }

            _gateway = new GatewayManager(_gatewayAddress, _token, _tokenSource.Token, new ConsoleLogger(LogLevel.Debug));
            _guilds = new GuildManager(_client, new ConsoleLogger(LogLevel.Debug));

            _gateway.EventDispatched += _guilds.GatewayMessageHandler;

            _guilds.MessageCreate += ProcessMessage;
            _guilds.GuildCreated += AddServer;

            _gateway.Initialize(_currentShard, _totalShards);

            // TODO: Logger comes from logger factory
            //var discord = DigiDiscord.Discord.Instance;
            //discord.InitializeBot(_token, new ConsoleLogger(LogLevel.Debug)).Wait();
            //
            //discord.GuildManager.MessageCreate += ProcessMessage;
            //
            //discord.GuildManager.GuildCreated += AddServer;
            //discord.GuildManager.MemberAdd += AddMember;
        }

        //private void AddMember(DiscordServer guild, DiscordServerUser member)
        //{
        //    var g = Guilds[guild.Id];
        //
        //    g.AddUser(member);
        //}
        //
        private void AddServer(DiscordServer g)
        {
            g.Scope = _botHostApplication.GetNewScope(g.ID);
        }
        
        public void ProcessMessage(DiscordChannel c, DiscordMessage m)
        {
            _botHostApplication.ProcessMessage(m);
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
        }

        private async Task<string> GetGatewayAddress()
        {
            //_Logger?.Verbose("Retrieving gateway...");
            var payload = await _client.Get(DigiBot.Discord.APIs.Gateway.Bot);

            return JObject.Parse(payload)["url"].ToString();
        }
    }
}