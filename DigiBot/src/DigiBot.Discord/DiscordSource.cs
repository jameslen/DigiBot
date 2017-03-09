using DigiBot.DiscordMiddleware;
using DigiDiscord;
using DigiDiscord.Utilities;
using System;
using System.Collections.Generic;

namespace DigiBot
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
    public class DiscordSource : IBotSource
    {
        private IBotHostApplication _botHostApplication;

        private string _token;

        private Dictionary<string, DiscordServer> Guilds = new Dictionary<string, DiscordServer>();
        private Dictionary<string, DiscordMiddleware.DiscordUser> Users = new Dictionary<string, DiscordMiddleware.DiscordUser>();

        public DiscordSource(DiscordOptions options)
        {
            _token = options.Token;
        }

        public void Start(IBotHostApplication botHostApplication)
        {
            var discord = DigiDiscord.Discord.Instance;
            _botHostApplication = botHostApplication;

            // TODO: REMOVE THIS and use env var
            discord.InitializeBot(_token, new ConsoleLogger(LogLevel.Debug)).Wait();

            discord.GuildManager.MessageCreate += ProcessMessage;

            discord.GuildManager.GuildCreated += AddServer;
            discord.GuildManager.MemberAdd += AddMember;
        }

        private void AddMember(Guild guild, GuildMember member)
        {
            var g = Guilds[guild.Id];

            g.AddUser(member);
        }

        private void AddServer(Guild g)
        {
            Guilds.Add(g.Id, new DiscordServer(g, _botHostApplication.GetNewScope(g.Id)));
        }

        public void ProcessMessage(GuildChannel c, Message m)
        {
            //Console.WriteLine($"{c.Name}: {m.Content}");

            var message = new DiscordMessage();
            message.SourceMessage = m;
            message.Server = Guilds[c.Guild_Id];
            message.Channel = message.Server.GetChannel(c.Id);
            message.User = message.Server.GetUser(m.Author.Id);

            // TODO: Clean up handling, use message factory, etc
            _botHostApplication.ProcessMessage(message);
        }
    }
}