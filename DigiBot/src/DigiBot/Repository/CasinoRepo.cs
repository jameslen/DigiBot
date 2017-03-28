using DigiBot.DatabaseContext;
using DigiBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.Repository
{
    public class CasinoRepo : ICasinoRepo
    {
        private DigiBotContext _context;

        public CasinoRepo(DigiBotContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        #region Accounts
        public Account GetUserAccount(string serverId, string userId)
        {
            var accounts = _context.Accounts.Where(a => a.OwnerId == userId && a.ServerId == serverId);

            if(accounts.Any())
            {
                return accounts.Include("History")
                               .FirstOrDefault();
            }
                        
            return null;
        }

        public void AddAccount(Account account)
        {
            _context.Accounts.Add(account);

            _context.Transactions.AddRange(account.History);
        }

        public void UpdateAccount(Account account, Transaction update)
        {
            if(account.Id == -1)
            {
                AddAccount(account);
            }

            _context.Transactions.Add(update);
        }

        private async Task<Account> GetAccountById(int id)
        {
            var account = await _context.Accounts.Include("History")
                                                 .Where(a => a.Id == id)
                                                 .FirstOrDefaultAsync();
            return account;
        }
        public IEnumerable<Account> GetServerAccounts(string serverId)
        {
            var accounts = _context.Accounts.Where(a => a.ServerId == serverId);

            if (accounts.Any())
            {
                return accounts.Include("History");
            }

            return null;
        }
        #endregion endregion

        #region SingleBet
        public void AddBet(Bet bet)
        {
            _context.Bets.Add(bet);
        }
        public void DeleteBet(Bet bet)
        {
            _context.Bets.Remove(bet);
        }
        public IEnumerable<Bet> GetAciveBets(string serverId, string userId)
        {
            return _context.Bets.Where(b => b.Server == serverId && b.State == BetState.Active && (b.Initiator == userId || b.Opponent == userId));
        }
        public IEnumerable<Bet> GetPendingBets(string serverId, string userId)
        {
            return _context.Bets.Where(b => b.Server == serverId && b.State == BetState.Pending && (b.Initiator == userId || b.Opponent == userId));
        }
        public IEnumerable<Bet> GetArbitratedBets(string serverId, string userId)
        {
            return _context.Bets.Where(b => b.Server == serverId && b.State == BetState.Pending && b.Arbitor == userId);
        }
        #endregion

        #region BetPool
        public IEnumerable<BetPool> GetOpenBetPools(string serverId)
        {
            return null;
        }
        public IEnumerable<BetPool> GetOpenHouseBetPools(string serverId)
        {
            return null;
        }
        public IEnumerable<BetPool> GetOpenUserBetPools(string serverId, string ownerId)
        {
            return null;
        }
        public IEnumerable<BetPool> GetUserActiveBetPools(string serverId, string userId)
        {
            return null;
        }
        public void OpenBetPool(BetPool pool)
        {
        }
        public void CloseBetPool(BetPool pool)
        {
        }

        public void UpdateBetPool(BetPool pool, PoolBet bet)
        {

        }
        #endregion

    }
}
