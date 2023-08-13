using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMRItemTracker.Twitch
{
    /// <summary>
    /// Base class to be extended for TwitchAPI responses
    /// </summary>
    public class TwitchAPIResponse
    {
        /// <summary>
        /// If the Twitch API returned a successful response and was able to be
        /// parsed successfully
        /// </summary>
        [JsonIgnore]
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the status code of the Twitch API response, or <see
        /// langword="null"/> for most successful API calls.
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the error message, or <see langword="null"/> for most
        /// successful API calls.
        /// </summary>
        [JsonPropertyName("messages")]
        public string? Message { get; set; }
    }
}
