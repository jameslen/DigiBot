﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigiBot.Commands
{
    public class GamblerCommands : ICommandProcessor
    {
        private IConfigurationRoot _config;
        private IGamblerManager _manager;

        public GamblerCommands(IConfigurationRoot config, IGamblerManager manager)
        {
            _config = config;
            _manager = manager;

            Prefix = _config["Config:Prefix"];
        }

        public void CheckAccount(IUser user)
        {
            if (user == null)
            {
                var account = _manager.GetUserBalance(SourceMessage.Server.ID, SourceMessage.User);
                Reply($"You currently have ${account.CurrentValue}.");
            }
            else
            {
                var account = _manager.GetUserBalance(SourceMessage.Server.ID, user);
                Reply($"{user.Name} currently has ${account.CurrentValue}.");
            }
        }

        public void TopAccounts()
        {
            var accounts = _manager.GetServerAccounts(SourceMessage.Server.ID);

            var sb = new StringBuilder();

            sb.Append("```\\n");

            int i = 1;
            foreach(var user in accounts.Values.OrderByDescending(u => u.CurrentValue))
            {
                sb.Append($"  #{i++} {user.Owner.Name.PadRight(10)}: {user.CurrentValue}\\n");
            }

            sb.Append("```\\n");

            Reply(sb.ToString());
        }

        public void CreateBet(IUser opp, IUser arb, int amount, string desc)
        {
            if(SourceMessage.User == opp)
            {
                Reply("Cannot bet yourself.");
                return;
            }
            
            if (SourceMessage.User == arb || opp == arb)
            {
                Reply("Cannot arbitrate your own bet.");
                return;
            }

            if (!_manager.CheckAccounts(SourceMessage.Server.ID,SourceMessage.User, amount))
            {
                Reply("You do not have enough funds to cover that bet.");
                return;
            }

            if (!_manager.CheckAccounts(SourceMessage.Server.ID, opp, amount))
            {
                Reply("Your opponent does not have enough funds to cover that bet.");
                return;
            }

            _manager.CreateBet(SourceMessage.Server.ID, SourceMessage.User, opp, arb, amount, desc);

            Reply($"Bet between {SourceMessage.User.Name} and {opp.Name} for {amount} created.");
        }

        public void MyBets()
        {
            Console.WriteLine("mybets");
            var bets = _manager.ActiveBets(SourceMessage.User);

            if(!bets.Any())
            {
                Reply("You have no active bets.");
                return;
            }

            var sb = new StringBuilder();

            sb.Append("Your active bets are:\\n");

            sb.Append("```\\n");

            sb.Append("   |Date     |  Amount  |    Initiator   |    Against     |   Arbitrator   |\\n");
            sb.Append("---|---------|----------|----------------|----------------|----------------|\\n");

            int i = 1;
            foreach (var bet in bets)
            {
                //sb.Append("00/00/00|Amount|TemplatedOneHaha|TemplatedOneHaha|TemplatedOneHaha");
                sb.Append($"{i++.ToString().PadLeft(3)}|{bet.Date.Date.ToString("MM/dd/yy")} |{bet.Amount.ToString().PadLeft(10)}|{bet.Initiator.Name.PadLeft(16)}|{bet.Opponent.Name.PadLeft(16)}|{bet.Arbitor.Name.PadLeft(16)}|{bet.Description}\\n");
            }
            sb.Append("---|---------|----------|----------------|----------------|----------------|\\n");
            sb.Append("```\\n");

            Reply(sb.ToString());
        }

        public void MyArbitrations()
        {
            Console.WriteLine("MyArbitrations");
            var bets = _manager.ArbitratedBets(SourceMessage.User);

            if (!bets.Any())
            {
                Reply("You have no active bets.");
                return;
            }

            var sb = new StringBuilder();

            sb.Append("Your active bets are:\\n");

            sb.Append("```\\n");

            sb.Append("   |Date     |  Amount  |    Initiator   |    Against     |   Arbitrator   |\\n");
            sb.Append("---|---------|----------|----------------|----------------|----------------|\\n");

            int i = 1;
            foreach (var bet in bets)
            {
                //sb.Append("00/00/00|Amount|TemplatedOneHaha|TemplatedOneHaha|TemplatedOneHaha");
                sb.Append($"{i++.ToString().PadLeft(3)}|{bet.Date.Date.ToString("MM/dd/yy")} |{bet.Amount.ToString().PadLeft(10)}|{bet.Initiator.Name.PadLeft(16)}|{bet.Opponent.Name.PadLeft(16)}|{bet.Arbitor.Name.PadLeft(16)}|{bet.Description}\\n");
            }
            sb.Append("---|---------|----------|----------------|----------------|----------------|\\n");
            sb.Append("```\\n");

            Reply(sb.ToString());
        }

        public void DeclareWinner(int betId, IUser winner)
        {
            Console.WriteLine("Declaring Winner");
            var bet = _manager.CompleteBet(SourceMessage.Server.ID, SourceMessage.User, winner, betId);

            if(bet == null)
            {
                Reply("Error processing bet.");
                return;
            }

            Reply($"{winner.Name} has been declared the winner of {bet.Amount}!");
        }

        public void Casino()
        {
            var sb = new StringBuilder();
            sb.Append("```\\n");
            sb.Append("Available Commands:\\n");
            sb.Append(" - CheckAccount <User>: If no user is passed, shows your account.\\n");
            sb.Append(" - TopAccounts        : List everyone's accounts by value.\\n");
            sb.Append(" - CreateBet <Against> <Arbitrator> <Amount> <descrption>:\\n");
            sb.Append("       <Against>     - Person to bet against.\\n");
            sb.Append("       <Arbitrator>  - Person that will declare the winner of the bet.\\n");
            sb.Append("       <Amount>      - Value of the bet.\\n");
            sb.Append("       <Description> - Note about the bet.\\n");
            sb.Append(" - MyBets             : List of your current, active bets.\\n");
            sb.Append(" - MyArbitrations     : List bets that you control the fate of.\\n");
            sb.Append(" - DeclareWinner <ID> <Winner> :\\n");
            sb.Append("       <ID>     - ID of the bet from your list of arbitrations.\\n" +
                      "                  Note: check the ID again before resolving multiple bets\\n");
            sb.Append("       <Winner> - User that made a sucker out of someone else.\\n");
            sb.Append("```\\n");

            Reply(sb.ToString());
            return;
        }
    }
}