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

        public Transaction AddTransaction(int change)
        {
            int amount = CurrentValue + change;

            var transaction = new Transaction(amount, change);

            History.Add(transaction);

            return transaction;
        }

        public int Id { get; set; } = -1;
        public string ServerId { get; set; }
        public string OwnerId { get; set; }
        public ICollection<Transaction> History = new List<Transaction>();
        public DateTime LastTransaction => History.OrderByDescending(t => t.Date).FirstOrDefault().Date;
        public int CurrentValue => History.OrderByDescending(t => t.Date).FirstOrDefault().Amount;
    }
}
