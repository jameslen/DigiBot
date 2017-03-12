using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.Discord
{
    public static class DiscordExtentions
    {
        public static IBotBuilder UseDiscord(this IBotBuilder builder, Action<DiscordOptions> config)
        {
            return builder.ConfigureServices(services =>
            {
                var options = new DiscordOptions();

                config(options);

                services.For<IBotSource>().Singleton().Use<DiscordSource>().Ctor<DiscordOptions>("options").Is(options);
            });
        }

        //public static IBotAppBuilder UseDiscord(this IBotAppBuilder app, Action<DiscordInitializer> botConfig)
        //{
        //
        //    //var discord = app.MessagePipelineSources.GetInstance<DiscordPipeline>();
        //    //
        //    //var discordConfig = new DiscordInitializer();
        //    //
        //    //botConfig(discordConfig);
        //    //
        //    //discord.Initialize(discordConfig);
        //    //
        //    //return app.AddPipeline(discord);
        //}
    }
}
