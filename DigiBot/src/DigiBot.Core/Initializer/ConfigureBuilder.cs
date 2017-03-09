using System;
using System.Reflection;

namespace DigiBot
{
    internal class ConfigureBuilder
    {
        public MethodInfo MethodInfo { get; }

        public ConfigureBuilder(MethodInfo configureMethod)
        {
            MethodInfo = configureMethod;
        }

        public Action<IBotAppBuilder> Build(object instance) => builder => Invoke(instance, builder);

        private void Invoke(object instance, IBotAppBuilder builder)
        {
            var services = builder.Services;
            var paramInfos = MethodInfo.GetParameters();
            var parameters = new object[paramInfos.Length];

            for(int i = 0; i < paramInfos.Length; ++i)
            {
                var info = paramInfos[i];

                if(info.ParameterType == typeof(IBotAppBuilder))
                {
                    parameters[i] = builder;
                }
                else
                {
                    parameters[i] = services.TryGetInstance(info.ParameterType);
                }
            }

            MethodInfo.Invoke(instance, parameters);
        }
    }
}