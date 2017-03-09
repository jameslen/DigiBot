using System;
using System.Threading.Tasks;
using StructureMap;
using System.Reflection;
using System.Linq;
using StructureMap.Pipeline;
using System.Collections.Generic;

namespace DigiBot
{
    public class CommandProcessorMiddleware : IMiddleware
    {
        private Dictionary<string, IEnumerable<ICommandProcessor>> _serverScopes = new Dictionary<string, IEnumerable<ICommandProcessor>>();

        public CommandProcessorMiddleware(IContainer services) : base(services)
        {
            
        }

        // TODO: Build map of commands so we don't have to look up every time
        public override async Task Invoke(IBotMessage message, MessageDelegate next)
        {
            var services = message.Server.Scope;

            var processors = services.GetAllInstances<ICommandProcessor>();

            foreach (var processor in processors)
            {
                if (message.Message.StartsWith(processor.Prefix))
                {
                    var substring = message.Message.Substring(processor.Prefix.Length);
                    var tokens = substring.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var command = tokens.FirstOrDefault().ToLowerInvariant();

                    if(string.IsNullOrWhiteSpace(command) || !processor.CheckPermissions(message.User))
                    {
                        continue;
                    }

                    var methods = processor.GetType().GetTypeInfo().GetMethods();

                    foreach(var method in methods)
                    {
                        if(command.StartsWith(method.Name.ToLowerInvariant()))
                        {
                            processor.SourceMessage = message;
                            object[] passingParams = null;

                            var parameters = method.GetParameters();

                            if(parameters.Length != 0)
                            {
                                passingParams = new object[parameters.Length];

                                for(int i = 0; i < parameters.Length; ++i)
                                {
                                    var parameter = parameters[i];
                                    var paramType = parameter.ParameterType;
                                    var paramTypeInfo = paramType.GetTypeInfo();

                                    if(i + 1 >= tokens.Length)
                                    {
                                        passingParams[i] = null;
                                        continue;
                                    }

                                    if (i == parameters.Length - 1 && paramType == typeof(string))
                                    {
                                        var param = substring.Substring(substring.IndexOf(tokens[i]) + tokens[i].Length);

                                        passingParams[i] = param;
                                    }
                                    else if(!paramTypeInfo.IsClass && !paramTypeInfo.IsInterface)
                                    {
                                        passingParams[i] = Convert.ChangeType(tokens[i + 1], paramType);
                                    }
                                    else if(paramType == typeof(IUser))
                                    {
                                        var userId = tokens[i + 1];
                                        userId = userId.Remove(userId.Length - 1);
                                        userId = userId.Substring(2);

                                        if(userId.StartsWith("!"))
                                        {
                                            userId = userId.Substring(1);
                                        }

                                        var user = message.Server.GetUser(userId);

                                        passingParams[i] = user;
                                    }
                                    else if (paramType == typeof(IChannel))
                                    {
                                        var channelId = tokens[i + 1];
                                        channelId = channelId.Remove(channelId.Length - 1);
                                        channelId = channelId.Substring(2);

                                        if (channelId.StartsWith("!"))
                                        {
                                            channelId = channelId.Substring(1);
                                        }

                                        var channel = message.Server.GetChannel(channelId);

                                        passingParams[i] = channel;
                                    }
                                }

                                
                            }

                            method.Invoke(processor, passingParams);
                            goto next;
                        }
                    }
                }
            }

next:       await next(message);
        }
    }
}