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
        Bet ConfirmBet(IUser user, int betId);
        void CreateBet(IUser init, IUser opp, IUser arb, int amount, string desc);
        IEnumerable<Bet> GetPendingBets(IUser user);
        Dictionary<string, Account> GetServerAccounts(string server);
        Account GetUserAccount(IUser user);
        Account GetUserAccount(string server, string user);
        Bet RejectBet(IUser user, int betId);

        Task<bool> SaveChanges();
    }
}