using DigiBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.DatabaseContext
{
    public class DigiBotContext : DbContext
    {
        public DigiBotContext(DbContextOptions<DigiBotContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bet> Bets { get; set; }
    }

    // This exists so that dotnet ef can connect to the database and update the schema
    public class DigiBotContextFactory : IDbContextFactory<DigiBotContext>
    {
        public DigiBotContext Create(DbContextFactoryOptions options)
        {
            // Using a separate settings json because i don't want to check in the connection string
            var config = new ConfigurationBuilder()
                              .SetBasePath(options.ContentRootPath)
                              .AddJsonFile("efsettings.json")
                              //.AddEnvironmentVariables()
                              .Build();

            var context = new DbContextOptionsBuilder<DigiBotContext>();
            context.UseSqlServer(config["ConnectionStrings:DigiBotContextConnection"]);

            return new DigiBotContext(context.Options);
        }
    }
}
