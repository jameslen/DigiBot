using System.Collections.Generic;

namespace DigiBot
{
    public interface IGamblerManager
    {
        IEnumerable<Bet> ActiveBets(IUser user);
        IEnumerable<Bet> ArbitratedBets(IUser arb);
        bool CheckAccounts(string server, IUser user, int amount);
        Dictionary<string, Account> GetServerAccounts(string server);
        void CreateBet(string serverId, IUser init, IUser opp, IUser arb, int amount, string desc);
        Account GetUserBalance(string serverId, IUser user);
        Bet CompleteBet(string server, IUser arb, IUser winner, int betId);
    }
}