using DigiDiscord;
using DigiDiscord.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DigiDiscord.Guild;

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

    public abstract class IUserCommand
    {
        protected string m_command;
        protected List<Permissions> m_permissions;

        public static readonly List<Permissions> NoRestriction = null;

        public class CommandArgs
        {
            public GuildMember User { get; set; }
            public GuildChannel Channel { get; set; }
            public string Command { get; set; }
        }

        public IUserCommand(string command, List<Permissions> permissions)
        {
            m_command = command;
            m_permissions = permissions;
        }

        public string Command
        {
            get
            {
                return m_command;
            }
        }

        public abstract bool CheckPermissions(GuildMember member);
        public abstract void Invoke(CommandArgs args);
    }

    public class CommandProcessor
    {
        public string Prefix { get; set; }
        private Dictionary<string, IUserCommand> m_commands = new Dictionary<string, IUserCommand>();

        public IEnumerable<string> CommandStrings { get { return m_commands.Keys.Select(c => Prefix + c); } }

        public CommandProcessor(Discord discord, string prefix = "Bot:")
        {
            Prefix = prefix;

            discord.GuildManager.MessageCreate += Process;
        }

        private void Process(GuildChannel c, Message m)
        {
            Console.WriteLine(m.Content);

            if(m.Content.StartsWith(Prefix))
            {
                var commandString = m.Content.Substring(Prefix.Length);

                foreach(var command in m_commands.Values)
                {
                    if(commandString.StartsWith(command.Command))
                    {
                        IUserCommand.CommandArgs args = new IUserCommand.CommandArgs {
                            Channel = c,
                            User = c.Guild.Members[m.Author.Id],
                            Command = commandString.Substring(command.Command.Length).TrimStart(' ')
                        };

                        if(command.CheckPermissions(args.User))
                        {
                            command.Invoke(args);
                        }

                        break;
                    }
                }
            }
        }

        public void AddCommand(IUserCommand command)
        {
            m_commands.Add(command.Command, command);
        }
    }

    public class SimpleCommand : IUserCommand
    {
        private Action<CommandArgs> m_action;

        public SimpleCommand(string command, List<Permissions> permissions, Action<CommandArgs> action) : base(command, permissions)
        {
            m_action = action;
        }

        public SimpleCommand(string command, Permissions permission, Action<CommandArgs> action) : this(command, new List<Permissions> { permission }, action)
        {
        }

        public SimpleCommand(string command, Action<CommandArgs> action) : this(command, null, action)
        {
        }

        public override void Invoke(CommandArgs args)
        {
            m_action.Invoke(args);
        }

        public override bool CheckPermissions(GuildMember member)
        {
            if (m_permissions != null)
            {
                foreach (var p in m_permissions)
                {
                    if ((member.Permissions & (int)p) == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public class Program
    {
        internal static string Token = "";

        public static void Main(string[] args)
        {
            Token = File.ReadAllText("Token.txt");

            try
            {
                var discord = Discord.Instance;

                discord.InitializeBot(Token, Logger).Wait();

                var cmdProc = new CommandProcessor(discord, "db:");

                cmdProc.AddCommand(new SimpleCommand("help", (arg) =>
                {
                    StringBuilder s = new StringBuilder();
                    s.Append("```\\n");

                    foreach(var command in cmdProc.CommandStrings)
                    {
                        s.Append($"{command}\\n");
                    }

                    s.Append("```\\n");

                    arg.Channel.SendMessage(s.ToString());
                }));

                cmdProc.AddCommand(new SimpleCommand("echo", (arg) => 
                {
                    arg.Channel.SendMessage(arg.Command);
                }));

                cmdProc.AddCommand(new SimpleCommand("prefix", Permissions.ADMINISTRATOR, (arg) =>
                {
                    cmdProc.Prefix = arg.Command;
                }));

                cmdProc.AddCommand(new SimpleCommand("listChannels", Permissions.ADMINISTRATOR, (arg) =>
                {
                    var channels = arg.Channel.Guild.Channels.AsEnumerable();

                    if (arg.Command.StartsWith("text"))
                    {
                        channels = channels.Where(kv => kv.Value.Type == "text");
                    }
                    else if (arg.Command.StartsWith("voice"))
                    {
                        channels = channels.Where(kv => kv.Value.Type == "voice");
                    }

                    StringBuilder response = new StringBuilder();

                    foreach (var channel in channels)
                    {
                        response.Append($"<#{channel.Key}>\\n");
                    }

                    arg.Channel.SendMessage(response.ToString());
                }));

                cmdProc.AddCommand(new SimpleCommand("tochannel", Permissions.ADMINISTRATOR, (arg) =>
                {
                    var g = arg.Channel.Guild;

                    var channelId = arg.Command.Substring(2, arg.Command.IndexOf('>') - 2);
                    var message = arg.Command.Substring(arg.Command.IndexOf('>') + 2);

                    var channel = g.Channels[channelId];

                    channel.SendMessage(message);

                }));

                cmdProc.AddCommand(new SimpleCommand("users", Permissions.ADMINISTRATOR, (arg) =>
                {
                    StringBuilder s = new StringBuilder();
                    var g = arg.Channel.Guild;

                    //var text = m.Content.Substring("db:users ".Length);

                    s.Append("```\\n");

                    foreach (var user in g.Members.Values.OrderBy(u => u.User.Discriminator))
                    {
                        s.Append($"{user.User.Username}#{user.User.Discriminator}:".PadRight(25));
                        
                        var roles = g.Roles.Where(k => user.Roles.Contains(k.Key)).Select(r => r.Value);

                        int permissions = 0;

                        foreach (var r in roles)
                        {
                            permissions |= r.Permissions;
                        }

                        s.Append($"{permissions:X8}\\n");

                    }

                    s.Append("```\\n");

                    arg.Channel.SendMessage(s.ToString());
                }));

                for (;;)
                {
                }
            }
            catch (Exception e)
            {
                Log(LogLevel.Error, $"{e.Message}");
            }

            Console.ReadLine();
        }

        public static ILogger Logger = new ConsoleLogger(LogLevel.Warning);

        public static void Log(LogLevel level, string message)
        {
            Logger.Log(level, message);
        }
    }
}
