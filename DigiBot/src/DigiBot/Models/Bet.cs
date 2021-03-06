﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Models
{
    public enum BetState
    {
        Pending,
        Active,
        Cancelled,
        Rejected,
        Complete
    }

    public class Bet
    {
        public int Id { get; set; }
        public string Server { get; set; }
        public string Initiator { get; set; }
        public string Opponent { get; set; }
        public string Arbitor { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public BetState State { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
