using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigiBot.Models
{
    public class Account
    {
        public Account()
        {
        }

        public Account(string owner, string server)
        {
            OwnerId = owner;
            ServerId = server;
            History = new List<Transaction>();
            History.Add(new Transaction(500, 500));

            History.Last().Account = this;
        }

        public Transaction AddTransaction(int change)
        {
            int amount = CurrentValue + change;

            var transaction = new Transaction(amount, change);

            transaction.Account = this;

            History.Add(transaction);

            return transaction;
        }

        public int Id { get; set; }
        public string ServerId { get; set; }
        public string OwnerId { get; set; }

        public ICollection<Transaction> History { get; set; }
        public DateTime LastTransaction => History.OrderByDescending(t => t.Date).FirstOrDefault().Date;
        public int CurrentValue => History.OrderByDescending(t => t.Date).FirstOrDefault().Amount;
    }
}
