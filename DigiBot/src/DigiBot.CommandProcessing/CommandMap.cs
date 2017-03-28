using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DigiBot.CommandProcessing
{
    public class CommandMap
    {
        private Dictionary<string, Tuple<Type, MethodInfo>> _commandMapping = new Dictionary<string, Tuple<Type, MethodInfo>>();

        private static int i = 0;
        public CommandMap(IContainer container)
        {
            var services = container.GetNestedContainer();

            var commandProcessors = services.GetAllInstances<ICommandProcessor>();

            foreach(var processor in commandProcessors)
            {
                var type = processor.GetType();
                var typeInfo = type.GetTypeInfo();
                var methods = typeInfo.DeclaredMethods;

                Console.WriteLine($"Processing: {processor.GetType().Name}");

                foreach(var method in methods.Where(m => !m.Name.Contains("CheckPermissions")))
                {
                    var name = method.Name.ToLowerInvariant();

                    if (_commandMapping.ContainsKey(name))
                    {
                        var prevCommand = _commandMapping[method.Name.ToLowerInvariant()];

                        Console.WriteLine($"Command named {name} has already been found in {prevCommand.Item1.Name}.");
                        continue;
                    }

                    _commandMapping.Add(name, new Tuple<Type, MethodInfo>(processor.GetType(), method));
                }
            }
        }

        public bool Contains(string cmdName)
        {
            return _commandMapping.ContainsKey(cmdName);
        }

        public Tuple<Type, MethodInfo> GetCommand(string cmdName)
        {
            if(Contains(cmdName) == false)
            {
                return null;
            }

            return _commandMapping[cmdName];
        }
    }
}
