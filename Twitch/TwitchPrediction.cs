using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public class TwitchPrediction : TwitchAPIResponse
    {
        [JsonPropertyName("broadcaster_id")]
        public string? BroadcasterId { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("outcomes")]
        public ICollection<TwitchPredictionOutcome>? Outcomes { get; set; }

        [JsonPropertyName("winning_outcome_id")]
        public string? WinningOutcomeId { get; set; }

        [JsonPropertyName("prediction_window")]
        public int? PredictionWindow { get; set; }

        /// <summary>
        /// If the prediction was finished, be it complete or closed
        /// </summary>
        [JsonIgnore]
        public bool IsPredictionComplete => !"ACTIVE".Equals(Status, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// If a prediction was finished successfully
        /// </summary>
        [JsonIgnore]
        public bool IsPredictionSuccessful => "RESOLVED".Equals(Status, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(WinningOutcomeId);
    }
}
