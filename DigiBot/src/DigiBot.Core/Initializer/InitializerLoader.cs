using System;
using StructureMap;
using System.Reflection;
using System.Linq;

namespace DigiBot
{
    internal class InitializerLoader
    {
        internal static InitializerMethods LoadMethods(IContext botServiceProvider, Type type)
        {
            var configureMethod = FindConfigureDelegate(type);
            var servicesMethod = FindConfigureServicseDelegate(type);

            object instance = null;

            if(!configureMethod.MethodInfo.IsStatic || (servicesMethod != null && !servicesMethod.MethodInfo.IsStatic))
            {
                instance = botServiceProvider.GetInstance(type);
            }

            var configCallback = configureMethod.Build(instance);
            var servicesCallback = servicesMethod.Build(instance);

            Func<Registry, IContainer> configureServices = services =>
            {
                IContainer appServiceProvider = servicesCallback.Invoke(services);

                if(appServiceProvider != null)
                {
                    return appServiceProvider;
                }

                return new Container(services);
            };

            return new InitializerMethods(configureMethod.Build(instance), configureServices);
        }

        private static ConfigureServicesBuilder FindConfigureServicseDelegate(Type type)
        {
            var configureMethod = FindMethod(type, "ConfigureServices");
            return new ConfigureServicesBuilder(configureMethod);
        }

        private static ConfigureBuilder FindConfigureDelegate(Type type)
        {
            var configureMethod = FindMethod(type, "Configure");
            return new ConfigureBuilder(configureMethod);
        }

        private static MethodInfo FindMethod(Type type, string name)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            var selectedMethods = methods.Where(method => method.Name == name).ToList();

            if(selectedMethods.Count > 1)
            {
                throw new InvalidOperationException($"Can only have 1 method named {name}.");
            }

            var methodInfo = selectedMethods.FirstOrDefault();

            if(methodInfo == null)
            {
                throw new InvalidOperationException($"A public method named {name} is required but cannot be found.");
            }

            return methodInfo;
        }
    }
}