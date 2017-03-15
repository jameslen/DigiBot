using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigiBot.Models
{
    public class Account
    {
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

        public string ServerId => Owner.Server.ID;
        public string OwnerId => Owner.Id;
        public IUser Owner { get; }
        public ICollection<Transaction> History = new List<Transaction>();
        public DateTime LastTransaction => History.OrderByDescending(t => t.Date).FirstOrDefault().Date;
        public int CurrentValue => History.OrderByDescending(t => t.Date).FirstOrDefault().Amount;
    }
}
