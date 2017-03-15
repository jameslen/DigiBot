using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Models
{
    public class Bet
    {
        public IUser Initiator { get; set; }
        public IUser Opponent { get; set; }
        public IUser Arbitor { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
