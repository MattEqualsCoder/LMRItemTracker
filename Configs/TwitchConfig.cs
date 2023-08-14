using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMRItemTracker.Configs
{
    public class TwitchConfig : MergeableConfig
    {
        public override string Key { get; set; } = "";
        public List<string> RecognizedGreetings { get; set; } = new();
        public SchrodingersString WhenConnected { get; set; } = new();
        public SchrodingersString GreetingResponses { get; set; } = new();
        public SchrodingersString GreetedTwice { get; set; } = new();
        public SchrodingersString GreetedBySelf { get; set; } = new();
        public SchrodingersString NoCurrentPrediction { get; set; } = new();
        public SchrodingersString PredictionAlreadyExists { get; set; } = new();
        public SchrodingersString PredictionLocked { get; set; } = new();
        public SchrodingersString PredictionCancelled { get; set; } = new();
        public SchrodingersString NotConnectedToChat { get; set; } = new();
        public Dictionary<string, string> UserNamePronunciations { get; set; } = new();
        public List<TwitchPredictionConfig> Predictions { get; set; } = new();
        public SchrodingersString AskChatAboutContent { get; set; } = new();
        public SchrodingersString AskChatAboutContentYes { get; set; } = new();
        public SchrodingersString AskChatAboutContentNo { get; set; } = new();
        public SchrodingersString PollComplete { get; set; } = new();
        public SchrodingersString PollOpened { get; set; } = new();
        public SchrodingersString PollError { get; set; } = new();
        public RollupResponses ContentTracked { get; set; } = new();
    }
}
