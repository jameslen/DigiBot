using DigiBot.Models;
using DigiBot.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot
{
    public class GamblerManager : IGamblerManager
    {
        private IConfigurationRoot _config;

        private Dictionary<string, Dictionary<string, Account>> _perServerUserAccounts = new Dictionary<string, Dictionary<string, Account>>();
        private List<Bet> _activeBets = new List<Bet>();
        private List<Bet> _pendingBets = new List<Bet>();
        private ICasinoRepo _repo;

        public GamblerManager(IConfigurationRoot config, ICasinoRepo repo)
        {
            _config = config;

            _repo = repo;
        }

        public bool CheckAccount(IUser user, int amount)
        {
            Console.WriteLine($"Checking account for {user.Name}");
            var balance = GetUserAccount(user);

            return balance.CurrentValue >= amount;
        }

        public Account GetUserAccount(IUser user)
        {
            Console.WriteLine($"Getting account for {user.Name}");
            return GetUserAccount(user.Server.ID, user.Id);
        }

        public Account GetUserAccount(string server, string user)
        {
            var serverAccounts = GetServerAccounts(server);

            if(!serverAccounts.ContainsKey(user))
            {
                serverAccounts.Add(user, new Account(user, server));
            }

            var account = serverAccounts[user];

            if (account.CurrentValue < 500 && account.LastTransaction.Date.AddDays(1) <= DateTime.UtcNow.Date)
            {
                var change = 500 - account.CurrentValue;

                account.AddTransaction(change);
            }

            return account;
        }

        public void CreateBet(IUser init, IUser opp, IUser arb, int amount, string desc)
        {
            Console.WriteLine($"Creating bet between {init.Name} and {opp.Name} for {amount}.");
            var bet = new Bet { Amount = amount, Arbitor = arb.Id, Initiator = init.Id, Opponent = opp.Id, Description = desc };

            var oppAcc = GetUserAccount(opp);
            var initAcc = GetUserAccount(init);

            initAcc.AddTransaction(-amount);

            _pendingBets.Add(bet);
        }

        public Dictionary<string, Account> GetServerAccounts(string server)
        {
            if (!_perServerUserAccounts.ContainsKey(server))
            {
                _perServerUserAccounts.Add(server, new Dictionary<string, Account>());
            }

            return _perServerUserAccounts[server];
        }

        public IEnumerable<Bet> ActiveBets(IUser user)
        {
            return _activeBets.Where(b => b.Initiator == user.Id || b.Opponent == user.Id);
        }

        public IEnumerable<Bet> ArbitratedBets(IUser arb)
        {
            return _activeBets.Where(b => b.Arbitor == arb.Id).OrderBy(b => b.Date);
        }

        public Bet CompleteBet(IUser arb, IUser winner, int betId)
        {
            var bets = _activeBets.Where(b => b.Arbitor == arb.Id).OrderBy(b => b.Date);

            var bet = bets.ElementAt(betId - 1);

            var oppAcc = GetUserAccount(bet.Opponent, arb.Server.ID);
            var initAcc = GetUserAccount(bet.Initiator, arb.Server.ID);

            if(bet.Opponent == winner.Id)
            {
                oppAcc.AddTransaction(bet.Amount * 2);
            }
            else if(bet.Initiator == winner.Id)
            {
                initAcc.AddTransaction(bet.Amount * 2);
            }
            else
            {
                return null;
            }

            _activeBets.Remove(bet);

            return bet;
        }

        public IEnumerable<Bet> GetPendingBets(IUser user)
        {
            return _pendingBets.OrderBy(b => b.Date).Where(b => b.Opponent == user.Id);
        }

        public IEnumerable<Bet> ConfirmBet(IUser user, int betId)
        {
            var pendingBets = _pendingBets.OrderBy(b => b.Date).Where(b => b.Opponent == user.Id);

            if(pendingBets.Count() == 0)
            {
                return null;
            }
            
            var bet = pendingBets.ElementAt(betId);

            var account = GetUserAccount(user);

            if(account.CurrentValue >= bet.Amount)
            {
                _pendingBets.Remove(bet);
                _activeBets.Add(bet);
                return new Bet[] { bet };
            }

            return null;
        }

        public bool RejectBet(IUser user, int betId)
        {
            var pendingBets = _pendingBets.OrderBy(b => b.Date).Where(b => b.Opponent == user.Id);

            if(pendingBets.Count() == 0)
            {
                return false;
            }

            var bet = pendingBets.ElementAt(betId);
            _pendingBets.Remove(bet);

            var account = GetUserAccount(bet.Initiator, user.Server.ID);

            account.AddTransaction(bet.Amount);

            return true;
        }
    }
}
