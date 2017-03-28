using System;
using System.Threading.Tasks;
using StructureMap;
using System.Reflection;
using System.Linq;
using StructureMap.Pipeline;
using System.Collections.Generic;
using DigiBot.CommandProcessing;

namespace DigiBot
{
    public class CommandProcessorMiddleware : IMiddleware
    {
        private Dictionary<string, IContainer> _serverScopes = new Dictionary<string, IContainer>();
        private CommandMap _commandMap;
        private ICommandConfig _commandConfig;

        public CommandProcessorMiddleware(IContainer services, ICommandConfig configRepo) : base(services)
        {
            _commandMap = new CommandMap(services);
            _commandConfig = configRepo;
        }

        public override async Task Invoke(IBotMessage message, MessageDelegate next)
        {
            if(!_serverScopes.ContainsKey(message.Server.ID))
            {
                _serverScopes.Add(message.Server.ID, Services.CreateChildContainer());
            }

            var config = _commandConfig.GetServerConfig(message.Server.ID);
            var prefix = config["Prefix"];
            var services = _serverScopes[message.Server.ID];

            if (message.Message.StartsWith(prefix))
            {
                var substring = message.Message.Substring(prefix.Length);
                var tokens = substring.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var command = tokens.FirstOrDefault().ToLowerInvariant();

                if (!string.IsNullOrWhiteSpace(command))
                {
                    var commandMethod = _commandMap.GetCommand(command);

                    if (commandMethod != null)
                    {
                        var proc = services.GetInstance(commandMethod.Item1) as ICommandProcessor;

                        if(proc.CheckPermissions(message.User))
                        {
                            proc.SourceMessage = message;

                            var paramText = substring.Substring(command.Length);

                            paramText = paramText.TrimStart(' ');

                            var parameters = BuildCommandParameters(message.Server, commandMethod.Item2.GetParameters(), paramText);

                            commandMethod.Item2.Invoke(proc, parameters);
                        }
                    }
                }
            }

            await next(message);
        }

        private object[] BuildCommandParameters(IServer server, ParameterInfo[] parameters, string stringParams)
        {
            if (parameters.Length != 0)
            {
                var passingParams = new object[parameters.Length];

                var tokens = stringParams.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    var paramType = parameter.ParameterType;
                    var paramTypeInfo = paramType.GetTypeInfo();

                    if (i >= tokens.Length)
                    {
                        passingParams[i] = null;
                        continue;
                    }

                    if (i == parameters.Length - 1 && paramType == typeof(string))
                    {
                        if (i == 0)
                        {
                            passingParams[i] = stringParams;
                        }
                        else
                        {
                            var param = stringParams.Substring(stringParams.IndexOf(tokens[i - 1]) + tokens[i - 1].Length);

                            passingParams[i] = param;
                        }
                    }
                    else if (!paramTypeInfo.IsClass && !paramTypeInfo.IsInterface)
                    {
                        passingParams[i] = Convert.ChangeType(tokens[i], paramType);
                    }
                    else if (paramType == typeof(IUser))
                    {
                        var userId = tokens[i];
                        userId = userId.Remove(userId.Length - 1);
                        userId = userId.Substring(2);

                        if (userId.StartsWith("!"))
                        {
                            userId = userId.Substring(1);
                        }

                        var user = server.GetUser(userId);

                        passingParams[i] = user;
                    }
                    else if (paramType == typeof(IChannel))
                    {
                        var channelId = tokens[i];
                        channelId = channelId.Remove(channelId.Length - 1);
                        channelId = channelId.Substring(2);

                        if (channelId.StartsWith("!"))
                        {
                            channelId = channelId.Substring(1);
                        }

                        var channel = server.GetChannel(channelId);

                        passingParams[i] = channel;
                    }
                }

                return passingParams;
            }

            return null;
        }
    }
}