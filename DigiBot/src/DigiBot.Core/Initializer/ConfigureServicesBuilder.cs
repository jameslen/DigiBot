using StructureMap;
using System;
using System.Reflection;

namespace DigiBot
{
    internal class ConfigureServicesBuilder
    {
        public MethodInfo MethodInfo { get; }

        public ConfigureServicesBuilder(MethodInfo configureMethod)
        {
            MethodInfo = configureMethod;
        }

        public Func<Registry, IContainer> Build(object instance) => builder => Invoke(instance, builder);

        private IContainer Invoke(object instance, Registry services)
        {
            return MethodInfo.Invoke(instance, new object[] { services }) as IContainer;
        }
    }
}