using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Models
{
    public class PoolBet
    {
        public int Id { get; set; }
        public bool For { get; set; }
        public string User { get; set; }
        public int Amount { get; set; }
        public DateTime BetDate { get; set; }
    }

    public class BetPool
    {
        public int Id { get; set; }
        public bool HousePool { get; set; }
        public BetState State { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public float Odds { get; set; }
        public DateTime Close { get; set; }
        public ICollection<PoolBet> For { get; set; }
        public ICollection<PoolBet> Against { get; set; }
    }
}
