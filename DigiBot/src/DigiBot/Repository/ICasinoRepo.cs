using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigiBot.Models;

namespace DigiBot.Repository
{
    public interface ICasinoRepo
    {
        void AddAccount(Account account);
        void AddBet(Bet bet);
        void DeleteBet(Bet bet);
        IEnumerable<Bet> GetAciveBets(string serverId, string userId);
        IEnumerable<Bet> GetArbitratedBets(string serverId, string userId);
        IEnumerable<BetPool> GetOpenBetPools(string serverId);
        IEnumerable<BetPool> GetOpenHouseBetPools(string serverId);
        IEnumerable<BetPool> GetOpenUserBetPools(string serverId, string ownerId);
        IEnumerable<Bet> GetPendingBets(string serverId, string userId);
        IEnumerable<Account> GetServerAccounts(string serverId);
        Account GetUserAccount(string serverId, string userId);
        IEnumerable<BetPool> GetUserActiveBetPools(string serverId, string userId);
        Task<bool> SaveChangesAsync();
        void UpdateAccount(Account account, Transaction update);
    }
}