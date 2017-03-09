using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot
{
    public class CommandProcessorFilter : IBotFilter
    {
        public Action<IBotAppBuilder> Configure(Action<IBotAppBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<CommandProcessorMiddleware>();
                next(builder);
            };
        }
    }
}
