using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigiBot.Models;

namespace DigiBot
{
    public interface ICasinoManager
    {
        IEnumerable<Bet> ActiveBets(IUser user);
        IEnumerable<Bet> ArbitratedBets(IUser arb);
        Bet CancelBet(IUser user, int betId);
        bool CheckAccount(IUser user, int amount);
        Bet CompleteBet(IUser arb, IUser winner, int betId);
        BetPool CompletePool(int betId, IUser user, bool result);
        Bet ConfirmBet(IUser user, int betId);
        void CreateBet(IUser init, IUser opp, IUser arb, int amount, string desc);
        void CreateBetPool(IUser owner, bool houseBet, float odds, DateTime close, string description);
        BetPool EnterPool(int betId, IUser user, int amount, bool _for);
        IEnumerable<BetPool> GetAllOpenUserPools(IUser user);
        IEnumerable<BetPool> GetHousePools(string server);
        IEnumerable<BetPool> GetOpenPools(IUser user);
        IEnumerable<Bet> GetPendingBets(IUser user);
        IEnumerable<BetPool> GetPoolsByOwner(IUser user);
        Dictionary<string, Account> GetServerAccounts(string server);
        Account GetUserAccount(IUser user);
        Account GetUserAccount(string server, string user);
        void KillBetPool(int betId, IUser user);
        Bet RejectBet(IUser user, int betId);
        Task<bool> SaveChanges();
        void UpdateAccount(Account a, Transaction t);
    }
}