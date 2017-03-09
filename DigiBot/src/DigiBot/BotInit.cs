using DigiBot.Commands;
using Microsoft.Extensions.Configuration;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot
{
    public class BotInit
    {
        private IConfigurationRoot _Config;

        public BotInit()
        {
            var builder = new ConfigurationBuilder()
                              .AddJsonFile("botsettings.json")
                              .AddEnvironmentVariables();

            _Config = builder.Build();
        }

        public void ConfigureServices(Registry services)
        {
            Console.WriteLine("Inside custom Configure Services");
            // TODO: Different platforms will be configured here, right now I only have discord
            services.For<IConfigurationRoot>().Singleton().Use(_Config);

            services.AddMessageProcessing();
            services.AddCommandProcessing();

            services.For<IGamblerManager>().Singleton().Use<GamblerManager>();

            //services.For<ICommandProcessor>().Use<GamblerCommands>();
        }

        // Params TBD
        public void Configure(IBotAppBuilder app)
        {
            Console.WriteLine("Inside custom Configure");
            
        }
    }
}
