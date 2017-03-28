using DigiBot.Commands;
using DigiBot.DatabaseContext;
using DigiBot.Repository;
using Microsoft.EntityFrameworkCore;
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
                              .AddJsonFile("botsettings.dev.json", optional: true)
                              .AddEnvironmentVariables();

            _Config = builder.Build();
        }

        public void ConfigureServices(Registry services)
        {
            Console.WriteLine("Inside custom Configure Services");
            // TODO: Different platforms will be configured here, right now I only have discord
            services.For<IConfigurationRoot>().Singleton().Use(_Config);

            services.AddMessageProcessing();

            var commandConfig = new DefaultCommandConfig(new Dictionary<string, string> { { "Prefix", _Config["Config:Prefix"] } }, null, null);

            services.AddCommandProcessing(commandConfig);

            services.For<ICasinoRepo>().Singleton().Use<CasinoRepo>();
            services.For<ICasinoManager>().Singleton().Use<CasinoManager>();

            var context = new DbContextOptionsBuilder<DigiBotContext>();
            context.UseSqlServer(_Config["ConnectionStrings:DigiBotContextConnection"], options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                    );
            });

            var dbcontext = new DigiBotContext(context.Options);

            services.For<DigiBotContext>().Use(dbcontext);
        }

        // Params TBD
        public void Configure(IBotAppBuilder app)
        {
            Console.WriteLine("Inside custom Configure");
            
        }
    }
}
