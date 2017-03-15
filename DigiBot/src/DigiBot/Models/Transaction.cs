using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }

        public DateTime Date { get; } = DateTime.UtcNow;
        public int Change { get; }
        public int Amount { get; }

        public Transaction(int amount, int change)
        {
            Change = change;
            Amount = amount;
        }
    }
}
