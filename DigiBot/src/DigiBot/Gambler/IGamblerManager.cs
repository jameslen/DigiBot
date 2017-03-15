using DigiBot.Models;
using System.Collections.Generic;

namespace DigiBot
{
    public interface IGamblerManager
    {
        IEnumerable<Bet> ActiveBets(IUser user);
        IEnumerable<Bet> ArbitratedBets(IUser arb);
        bool CheckAccount(IUser user, int amount);
        Dictionary<string, Account> GetServerAccounts(string server);
        void CreateBet(IUser init, IUser opp, IUser arb, int amount, string desc);
        Account GetUserAccount(IUser user);
        Bet CompleteBet(IUser arb, IUser winner, int betId);
        IEnumerable<Bet> GetPendingBets(IUser user);
        IEnumerable<Bet> ConfirmBet(IUser user, int betId);
        bool RejectBet(IUser user, int betId);
    }
}