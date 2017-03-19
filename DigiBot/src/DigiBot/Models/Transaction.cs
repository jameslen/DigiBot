using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DigiBot.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public DateTime Date { get; set; }
        public int Change { get; set; }
        public int Amount { get; set; }

        public Transaction(int amount, int change)
        {
            Date = DateTime.UtcNow;
            Change = change;
            Amount = amount;
        }

        public Transaction()
        {

        }
    }
}
