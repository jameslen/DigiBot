using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Models
{
    public class Bet
    {
        public string Initiator { get; set; }
        public string Opponent { get; set; }
        public string Arbitor { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
