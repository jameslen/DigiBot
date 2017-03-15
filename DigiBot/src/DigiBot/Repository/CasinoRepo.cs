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

        public async Task<Account> GetUserAccount(string serverId, string userId)
        {
            var account = await _context.Accounts.Include(a => a.History)
                                                 .Where(a => a.OwnerId == userId && a.ServerId == serverId)
                                                 .FirstOrDefaultAsync();
            return account;
        }

        public void AddAccount(Account account)
        {
            if(account.Id != -1)
            {
                Console.WriteLine("Account already exists.");
                return;
            }

            _context.Accounts.Add(account);
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
            var account = await _context.Accounts.Include(a => a.History)
                                                 .Where(a => a.Id == id)
                                                 .FirstOrDefaultAsync();
            return account;
        }

        public void AddBet(Bet bet)
        {
            _context.Bets.Add(bet);
        }

        public void DeleteBet(Bet bet)
        {
            _context.Bets.Remove(bet);
        }
    }
}
