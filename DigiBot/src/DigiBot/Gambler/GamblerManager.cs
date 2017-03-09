using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot
{
    public class Bet
    {
        public IUser Initiator { get; set; }
        public IUser Opponent { get; set; }
        public IUser Arbitor { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

    public class Account
    {
        public class Transaction
        {
            public DateTime Date { get; } = DateTime.UtcNow;
            public int Change { get; }
            public int Amount { get; }

            public Transaction(int amount, int change)
            {
                Change = change;
                Amount = amount;
            }
        }

        public Account(IUser owner)
        {
            Owner = owner;
            History.Add(new Transaction(500, 500));
        }

        public void AddTransaction(int change)
        {
            int amount = CurrentValue + change;

            History.Add(new Transaction(amount, change));
        }

        public IUser Owner { get; }
        public List<Transaction> History = new List<Transaction>();
        public DateTime LastTransaction => History.OrderByDescending(t => t.Date).FirstOrDefault().Date;
        public int CurrentValue => History.OrderByDescending(t => t.Date).FirstOrDefault().Amount;
    }

    public class GamblerManager : IGamblerManager
    {
        private IConfigurationRoot _config;

        private Dictionary<string, Dictionary<string, Account>> _perServerUserAccounts = new Dictionary<string, Dictionary<string, Account>>();
        private List<Bet> _activeBets = new List<Bet>();

        public GamblerManager(IConfigurationRoot config)
        {
            _config = config;
        }

        public bool CheckAccounts(string server, IUser user, int amount)
        {
            Console.WriteLine($"Checking account for {user.Name}");
            var balance = GetUserBalance(server, user);

            return balance.CurrentValue > amount;
        }

        public Account GetUserBalance(string serverId, IUser user)
        {
            Console.WriteLine($"Getting account for {user.Name}");
            var serverAccounts = GetServerAccounts(serverId);

            if(!serverAccounts.ContainsKey(user.Id))
            {
                serverAccounts.Add(user.Id, new Account(user));
            }

            var account = serverAccounts[user.Id];

            if (account.CurrentValue < 500 && account.LastTransaction.Date.AddDays(1) <= DateTime.UtcNow.Date)
            {
                var change = 500 - account.CurrentValue;

                account.AddTransaction(change);
            }

            return account;
        }

        public void CreateBet(string serverId, IUser init, IUser opp, IUser arb, int amount, string desc)
        {
            Console.WriteLine($"Creating bet between {init.Name} and {opp.Name} for {amount}.");
            var bet = new Bet { Amount = amount, Arbitor = arb, Initiator = init, Opponent = opp, Description = desc };

            var oppAcc = GetUserBalance(serverId, opp);
            var initAcc = GetUserBalance(serverId, init);

            oppAcc.AddTransaction(-amount);
            initAcc.AddTransaction(-amount);

            _activeBets.Add(bet);
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
            return _activeBets.Where(b => b.Initiator == user || b.Opponent == user);
        }

        public IEnumerable<Bet> ArbitratedBets(IUser arb)
        {
            return _activeBets.Where(b => b.Arbitor == arb).OrderBy(b => b.Date);
        }

        public Bet CompleteBet(string server, IUser arb, IUser winner, int betId)
        {
            var bets = _activeBets.Where(b => b.Arbitor == arb).OrderBy(b => b.Date);

            var bet = bets.ElementAt(betId - 1);

            var oppAcc = GetUserBalance(server, bet.Opponent);
            var initAcc = GetUserBalance(server, bet.Initiator);

            if(bet.Opponent == winner)
            {
                oppAcc.AddTransaction(bet.Amount * 2);
            }
            else if(bet.Initiator == winner)
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
    }
}
