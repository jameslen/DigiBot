using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DigiBot
{
    public static class BotAppBuilderMiddlewareExtensions
    {
        public static IBotAppBuilder UseMiddleware<MiddlewareProvider>(this IBotAppBuilder builder) where MiddlewareProvider : IMiddleware
        {
            // TODO: Allow middle that doesn't use the IMiddleware Interface
            builder.Use(next =>
            {
                return async message =>
                {
                    Dictionary<string, object> args = new Dictionary<string, object>();
                    args["botServices"] = builder.Services;
                    var provider = builder.Services.GetInstance<MiddlewareProvider>(new ExplicitArguments(args));

                    await provider.Invoke(message, next);
                };
            });

            return builder;
        }
    }
}
