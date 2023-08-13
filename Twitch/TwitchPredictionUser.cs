using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    public class TwitchPredictionUser
    {
        [JsonPropertyName("user_id")]
        public string? Id { get; set; }

        [JsonPropertyName("user_login")]
        public string? Login { get; set; }

        [JsonPropertyName("user_name")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("channel_points_used")]
        public int? ChannelPointsUsed { get; set; }

        [JsonPropertyName("channel_points_won")]
        public int? ChannelPointsWon { get; set; }
    }
}
