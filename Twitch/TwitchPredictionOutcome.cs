using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public class TwitchPredictionOutcome
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("users")]
        public int? UserCount { get; set; }

        [JsonPropertyName("channel_points")]
        public int? ChannelPointsSpent { get; set; }

        [JsonPropertyName("top_predictors")]
        public List<TwitchPredictionUser>? TopPredictors { get; set; }
    }
}
