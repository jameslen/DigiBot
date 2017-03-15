using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigiBot.Models
{
    public class Account
    {
        public Account(string owner, string server)
        {
            OwnerId = owner;
            ServerId = server;
            History.Add(new Transaction(500, 500));
        }

        public void AddTransaction(int change)
        {
            int amount = CurrentValue + change;

            History.Add(new Transaction(amount, change));
        }

        public string ServerId { get; set; }
        public string OwnerId { get; set; }
        public ICollection<Transaction> History = new List<Transaction>();
        public DateTime LastTransaction => History.OrderByDescending(t => t.Date).FirstOrDefault().Date;
        public int CurrentValue => History.OrderByDescending(t => t.Date).FirstOrDefault().Amount;
    }
}
