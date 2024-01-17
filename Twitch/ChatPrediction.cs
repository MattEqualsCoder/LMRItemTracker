using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public class ChatPrediction
    {
        public string Id { get; set; } = "";
        public bool IsPredictionComplete { get; set; }
        public bool IsPredictionSuccessful { get; set; }
        public Dictionary<string, string> OutcomeIds { get; set; } = new();
        public string? WinningChoice { get; set; }
        public int? WinningCount { get; set; }
        public int? PointsWon { get; set; }
        public int? LosingCount { get; set; }
        public int? PointsLost { get; set; }
        public List<string> Winners { get; set; } = new();
        public List<string> Losers { get; set; } = new();
    }
}
