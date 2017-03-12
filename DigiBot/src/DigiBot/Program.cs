using DigiBot.Discord;
using Microsoft.Extensions.Configuration;
using System;

namespace DigiBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var settings = new ConfigurationBuilder()
                              .AddJsonFile("bot.json")
                              .AddEnvironmentVariables()
                              .Build();

            var bot = new BotBuilder()
                          .UseDiscord(options =>
                          {
                              options.Token = settings["Discord:Token"];
                          })
                          .UseInitializer<BotInit>()
                          .Build();

            bot.Run();
        }
    }
}
