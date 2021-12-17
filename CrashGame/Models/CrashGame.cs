using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public decimal Multiplier { get; set; }
        public long GameNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompleteAt { get; set; }

        public List<CrashBet> Bets { get; set; }

        public bool IsFinished()
        {
            return CompleteAt > DateTime.MinValue;
        }
    }

    public class CrashBet
    {
        [Key]
        public int Id { get; set; }
        public string OwnerId { get; set; }
        [Column(TypeName = "decimal(16,8)")]
        public decimal Amount { get; set; }
    }


    public class CrashGameResult
    {
        [Key]
        public int Id { get; set; }
        public decimal Multiplier { get; set; }
        public long GameNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompleteAt { get; set; }

        public List<CrashBet> WinBets { get; set; }
        public List<CrashBet> LoseBets { get; set; }
    }
}
