using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DigiBot.DatabaseContext;
using DigiBot.Models;

namespace DigiBot.Migrations
{
    [DbContext(typeof(DigiBotContext))]
    [Migration("20170319142139_fk-update")]
    partial class fkupdate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DigiBot.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OwnerId");

                    b.Property<string>("ServerId");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("DigiBot.Models.Bet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.Property<string>("Arbitor");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description");

                    b.Property<string>("Initiator");

                    b.Property<string>("Opponent");

                    b.Property<string>("Server");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.ToTable("Bets");
                });

            modelBuilder.Entity("DigiBot.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("DigiBot.Models.Transaction", b =>
                {
                    b.HasOne("DigiBot.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
