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
    public class CasinoManager : ICasinoManager
    {
        private IConfigurationRoot _config;

        //private Dictionary<string, Dictionary<string, Account>> _perServerUserAccounts = new Dictionary<string, Dictionary<string, Account>>();
        //private List<Bet> _activeBets = new List<Bet>();
        //private List<Bet> _pendingBets = new List<Bet>();
        private ICasinoRepo _repo;

        public CasinoManager(IConfigurationRoot config, ICasinoRepo repo)
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
            var account = _repo.GetUserAccount(server, user);

            if(account == null)
            {
                account = new Account(user, server);

                _repo.AddAccount(account);

                _repo.SaveChangesAsync();
            }

            if (account.CurrentValue < 500 && account.LastTransaction.Date.AddDays(1) <= DateTime.UtcNow.Date)
            {
                var change = 500 - account.CurrentValue;

                account.AddTransaction(change);

                _repo.SaveChangesAsync();
            }

            return account;
        }

        public void CreateBet(IUser init, IUser opp, IUser arb, int amount, string desc)
        {
            Console.WriteLine($"Creating bet between {init.Name} and {opp.Name} for {amount}.");
            var bet = new Bet { Amount = amount, Arbitor = arb.Id, Initiator = init.Id, Opponent = opp.Id, Description = desc, State = BetState.Pending };

            var oppAcc = GetUserAccount(opp);
            var initAcc = GetUserAccount(init);

            initAcc.AddTransaction(-amount);

            _repo.AddBet(bet);

            _repo.SaveChangesAsync();
        }

        public Dictionary<string, Account> GetServerAccounts(string server)
        {
            return _repo.GetServerAccounts(server)?.ToDictionary(g => g.OwnerId, g => g);
        }

        public IEnumerable<Bet> ActiveBets(IUser user)
        {
            return _repo.GetAciveBets(user.Server.ID, user.Id);
        }

        public IEnumerable<Bet> GetPendingBets(IUser user)
        {
            return _repo.GetPendingBets(user.Server.ID, user.Id);
        }

        public IEnumerable<Bet> ArbitratedBets(IUser arb)
        {
            return _repo.GetArbitratedBets(arb.Server.ID, arb.Id);
        }

        public Bet CompleteBet(IUser arb, IUser winner, int betId)
        {
            var bets = ArbitratedBets(arb);

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

            bet.State = BetState.Complete;

            _repo.SaveChangesAsync();

            return bet;
        }

        public Bet ConfirmBet(IUser user, int betId)
        {
            var pendingBets = GetPendingBets(user);

            if(pendingBets.Count() == 0)
            {
                return null;
            }
            
            var bet = pendingBets.ElementAt(betId);

            if(bet.Opponent != user.Id)
            {
                return null;
            }

            var account = GetUserAccount(user);

            if(account.CurrentValue >= bet.Amount)
            {
                var transaction = account.AddTransaction(bet.Amount);

                _repo.UpdateAccount(account, transaction);

                bet.State = BetState.Active;

                _repo.SaveChangesAsync();

                return bet;
            }

            return null;
        }

        public Bet RejectBet(IUser user, int betId)
        {
            var pendingBets = GetPendingBets(user);

            if(pendingBets.Count() == 0)
            {
                return null;
            }

            var bet = pendingBets.ElementAt(betId);

            if(bet.Opponent != user.Id)
            {
                return null;
            }

            bet.State = BetState.Rejected;

            var account = GetUserAccount(bet.Initiator, user.Server.ID);

            var transaction = account.AddTransaction(bet.Amount);
            _repo.UpdateAccount(account, transaction);

            _repo.SaveChangesAsync();

            return bet;
        }

        public Bet CancelBet(IUser user, int betId)
        {
            var pendingBets = GetPendingBets(user);

            if (pendingBets.Count() == 0)
            {
                return null;
            }

            var bet = pendingBets.ElementAt(betId);

            if (bet.Initiator != user.Id)
            {
                return null;
            }

            bet.State = BetState.Cancelled;

            var account = GetUserAccount(bet.Initiator, user.Server.ID);

            var transaction = account.AddTransaction(bet.Amount);
            _repo.UpdateAccount(account, transaction);

            _repo.SaveChangesAsync();

            return bet;
        }

        public void UpdateAccount(Account a, Transaction t)
        {
            _repo.UpdateAccount(a, t);
        }

        public Task<bool> SaveChanges()
        {
            return _repo.SaveChangesAsync();
        }
    }
}
